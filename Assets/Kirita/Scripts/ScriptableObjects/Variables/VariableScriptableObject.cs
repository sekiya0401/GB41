using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public abstract class VariableScriptableObject<T> : ScriptableObject
{
    [SerializeField]
    protected T m_Value;

    public virtual T Value
    {
        get => m_Value;
        set => m_Value = value;
    }
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(VariableScriptableObject<>), true)]
public class VariableScriptableObjectProperty : PropertyDrawer
{
    private bool m_IsShowValue = false;
    private SerializedObject m_CachedSO;
    private Object m_CachedTarget;
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        label = EditorGUI.BeginProperty(position, label, property);

        // ラベル描画して残りRectを取得
        position = EditorGUI.PrefixLabel(position, label);

        var target = property.objectReferenceValue;

        if (target is not null)
        {
            //キャッシュ更新
            if (target != m_CachedTarget)
            {
                m_CachedTarget = target;
                m_CachedSO = new SerializedObject(target);
            }

            // トグルとオブジェクトフィールドの幅を分割
            float toggleWidth = 20f;  // チェックボックスの幅
            float spacing = 2f;
            float objectWidth = position.width - toggleWidth - spacing;

            // 各Rectを定義
            Rect toggleRect = new Rect(position.x, position.y, toggleWidth, EditorGUIUtility.singleLineHeight);
            Rect objectRect = new Rect(toggleRect.xMax + spacing, position.y, objectWidth, EditorGUIUtility.singleLineHeight);

            // トグル
            m_IsShowValue = EditorGUI.Toggle(toggleRect, m_IsShowValue);

            if (m_IsShowValue)
            {
                var valueProp = m_CachedSO.FindProperty("m_Value");

                m_CachedSO.Update();
                EditorGUI.BeginChangeCheck();
                EditorGUI.PropertyField(objectRect, valueProp, GUIContent.none, true);
                if (EditorGUI.EndChangeCheck())
                {
                    m_CachedSO.ApplyModifiedProperties();
                    EditorUtility.SetDirty(m_CachedSO.targetObject);
                }
            }
            else
            {
                EditorGUI.PropertyField(objectRect, property, GUIContent.none);
            }
        }

        if (property.objectReferenceValue is not null)
        {

        }
        else
        {
            EditorGUI.PropertyField(position, property, GUIContent.none);
        }

        EditorGUI.EndProperty();
    }
}
#endif