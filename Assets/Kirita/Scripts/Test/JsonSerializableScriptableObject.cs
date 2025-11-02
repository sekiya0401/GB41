using UnityEngine;
using Newtonsoft.Json;
using System;
using Newtonsoft.Json.Linq;
using System.Reflection;
using System.IO;

public abstract class JsonSerializableScriptableObject : ScriptableObject
{
    public virtual string ToJson(bool prettyPrint = true)
    {
        var settings = new JsonSerializerSettings
        {
            Formatting = prettyPrint ? Formatting.Indented : Formatting.None,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            TypeNameHandling = TypeNameHandling.None,
            Converters = { new ScriptableObjectObjectConverter() }
        };
        return JsonConvert.SerializeObject(this, settings);
    }

    public virtual object FromJson(string json)
    {
        //カスタムコンバーターを含めた設定
        var settings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            TypeNameHandling = TypeNameHandling.None,
            Converters = { new ScriptableObjectObjectConverter() }
        };

        //JSONからScriptableObjectへ変換
        return JsonConvert.DeserializeObject(json, GetType(), settings);
    }

    public virtual void LoadJson(string path)
    {
        string json = File.ReadAllText(path);

        var source = FromJson(json);
        CopyFields(source);
    }

    /// <summary>
    /// デシリアライズされた新しいインスタンスから、既存のSOへ値をコピー
    /// </summary>
    private void CopyFields(object source)
    {
        if (source == null) return;

        var type = GetType();
        var fields = type.GetFields(System.Reflection.BindingFlags.Instance |
                                    System.Reflection.BindingFlags.Public |
                                    System.Reflection.BindingFlags.NonPublic);

        foreach (var field in fields)
        {
            // Unityのシリアライズ対象フィールドのみをチェック
            if (field.IsPublic || field.GetCustomAttribute<SerializeField>() != null)
            {
                var newValue = field.GetValue(source);
                var existingValue = field.GetValue(this); // 既存のSOアセットの値

                //ネストされたScriptableObjectの場合
                if (newValue is ScriptableObject newSO && existingValue is ScriptableObject existingSO)
                {
                    //既存のSOアセット（existingSO）に対して、新しいSO（newSO）の内容を再帰的にコピー
                    //型が一致しない場合は処理をスキップ（miss type防止）
                    if (newSO.GetType() == existingSO.GetType())
                    {
                        //NOTE: 既存のSOに新しいSOのデータを上書き
                        //      existingSOの型をJsonSerializableScriptableObjectにキャストできれば、
                        //      そのCopyFieldsを呼び出すことで、SO内のSOのフィールドコピーが可能
                        if (existingSO is JsonSerializableScriptableObject jss)
                        {
                            jss.CopyFields(newSO);
                        }
                    //そうでない場合は、リフレクションで再度コピー
                    else
                        {
                            //フィールドを上書き
                            InternalCopyFields(newSO, existingSO);
                        }

                        //NOTE: 参照自体は既存のexistingSOのままなので、参照切れは発生しない
                        field.SetValue(this, existingSO);
                    }
                    else
                    {
                        //JSONとアセットでSOの型が異なる場合
                        Debug.LogWarning($"Skipped copying nested SO field '{field.Name}' due to type mismatch: JSON is {newSO.GetType()}, existing is {existingSO.GetType()}.");
                    }
                }
                //ネストされたSOがnullの場合
                else if (newValue == null && existingValue is ScriptableObject existingSOToClear)
                {
                    // JSONでnullになった場合、既存のSOアセットへの参照をnullにする
                    field.SetValue(this, null);
                }
                //プリミティブやその他の型、またはSOでないクラスの場合
                else
                {
                    // SO以外のフィールド（string, int, floatなど）は直接値を上書き
                    field.SetValue(this, newValue);
                }
            }
        }
    }

    /// <summary>
    /// ScriptableObjectを継承していないSOのベースクラスのコピー用ヘルパー
    /// </summary>
    private static void InternalCopyFields(object source, object destination)
    {
        if (source == null || destination == null || source.GetType() != destination.GetType()) return;

        var type = source.GetType();
        var fields = type.GetFields(System.Reflection.BindingFlags.Instance |
                                    System.Reflection.BindingFlags.Public |
                                    System.Reflection.BindingFlags.NonPublic);

        foreach (var field in fields)
        {
            //シリアライズ対象フィールドのみをチェック
            if (field.IsPublic || field.GetCustomAttribute<SerializeField>() != null)
            {
                var value = field.GetValue(source);
                field.SetValue(destination, value);
            }
        }
    }
}

/// <summary>
/// ScriptableObjetからJSON形式への変換制御
/// </summary>
public class ScriptableObjectObjectConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return typeof(UnityEngine.ScriptableObject).IsAssignableFrom(objectType);
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        //nullチェック
        if (value == null)
        {
            return;
        }

        //NOTE: CanVonvertでScriptableObject以外では呼ばれなことが確定しているので、そのままキャスト
        ScriptableObject so = (ScriptableObject)value;

        JObject jObj = new JObject();
        var fields = so.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        foreach (var field in fields)
        {
            if (field.IsPublic || field.GetCustomAttribute<SerializeField>() != null)
            {
                var fieldValue = field.GetValue(value);
                jObj[field.Name] = field != null
                    ? JToken.FromObject(fieldValue, serializer)
                    : JValue.CreateNull();
            }
        }
        jObj.WriteTo(writer);
        return;
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        //JSONが空ならnullを返す
        if (reader.TokenType == JsonToken.Null)
            return null;

        //JObjectとして読み取る
        JObject jObj = JObject.Load(reader);

        //ScriptableObjectを生成
        ScriptableObject so = ScriptableObject.CreateInstance(objectType);

        //すべてのフィールドを走査
        var fields = objectType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        foreach (var field in fields)
        {
            //publicまたは[SerializeField]がついてるものだけ対象
            if (!field.IsPublic && field.GetCustomAttribute<SerializeField>() == null)
            {
                continue;
            }

            //JSON側に同名のフィールドがない場合はスキップ
            if (!jObj.TryGetValue(field.Name, out var token))
            {
                continue;
            }

            //値がオブジェクトかつScriptableObjectの場合
            if (token.Type == JTokenType.Object && typeof(ScriptableObject).IsAssignableFrom(field.FieldType))
            {
                //ネストSOを再帰的に生成
                var nested = (ScriptableObject)ReadJson(token.CreateReader(), field.FieldType, null, serializer);
                field.SetValue(so, nested);
            }
            else
            {
                //プリミティブな型
                var value = token.ToObject(field.FieldType, serializer);
                field.SetValue(so, value);
            }
        }

        return so;
    }
}