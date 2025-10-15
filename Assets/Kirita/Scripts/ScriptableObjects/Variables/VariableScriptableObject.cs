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

        // ���x���`�悵�Ďc��Rect���擾
        position = EditorGUI.PrefixLabel(position, label);

        var target = property.objectReferenceValue;

        if (target is not null)
        {
            //�L���b�V���X�V
            if (target != m_CachedTarget)
            {
                m_CachedTarget = target;
                m_CachedSO = new SerializedObject(target);
            }

            // �g�O���ƃI�u�W�F�N�g�t�B�[���h�̕��𕪊�
            float toggleWidth = 20f;  // �`�F�b�N�{�b�N�X�̕�
            float spacing = 2f;
            float objectWidth = position.width - toggleWidth - spacing;

            // �eRect���`
            Rect toggleRect = new Rect(position.x, position.y, toggleWidth, EditorGUIUtility.singleLineHeight);
            Rect objectRect = new Rect(toggleRect.xMax + spacing, position.y, objectWidth, EditorGUIUtility.singleLineHeight);

            // �g�O��
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