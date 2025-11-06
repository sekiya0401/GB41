#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;

[CustomEditor(typeof(JsonSerializableScriptableObject),true)] 
public class JsonSerializableScriptableObjectEditor : Editor 
{
    protected override void OnHeaderGUI()
    {
        base.OnHeaderGUI();
        var so = (JsonSerializableScriptableObject)target;

        //コンソール出力
        if (GUILayout.Button("🔄JSON変換し、コンソールに出力"))
        {
            string json = so.ToJson();
            Debug.Log(json);
        }

        //ファイル保存
        if(GUILayout.Button("💾 JSONファイルとして保存"))
        {
            string path = EditorUtility.SaveFilePanel("JSON形式で保存", Application.dataPath, so.name, "json");

            if (!string.IsNullOrEmpty(path))
            {
                File.WriteAllText(path, so.ToJson());
                EditorUtility.RevealInFinder(path);
                Debug.Log($"✅ JSONファイルを保存しました: {path}");
            }
        }

        // JSONファイルを読み込み（ScriptableObjectへ反映）
        if (GUILayout.Button("📥 JSONファイルを読み込み＆適用"))
        {
            string path = EditorUtility.OpenFilePanel("JSON形式で読み込み", Application.dataPath, "json");

            if (!string.IsNullOrEmpty(path) && File.Exists(path))
            {
                try
                {
                    Undo.RecordObject(so, "Load JSON");

                    so.LoadJson(path);

                    EditorUtility.SetDirty(so);
                    AssetDatabase.SaveAssets();

                    Debug.Log($"✅ JSONからデータを読み込み・反映しました: {path}");
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"❌ JSONの読み込みに失敗しました: {ex.Message}");
                }
            }
        }

        EditorGUILayout.Space();
    }
}
#endif