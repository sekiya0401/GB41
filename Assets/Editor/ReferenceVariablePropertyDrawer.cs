#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ReferenceVariable<>), true)]
public class ReferenceVariablePropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var constantProp = property.FindPropertyRelative("m_ConstantValue");
        var variableProp = property.FindPropertyRelative("m_Variable");
        var isUseConstantProp = property.FindPropertyRelative("m_IsUseConstant");

        EditorGUI.BeginProperty(position, label, property);

        var rect = EditorGUI.PrefixLabel(position, label);

        EditorGUI.BeginChangeCheck();

        if (isUseConstantProp.boolValue)
        {
            EditorGUI.PropertyField(rect, constantProp, GUIContent.none);
        }
        else
        {
            EditorGUI.PropertyField(rect, variableProp, GUIContent.none);
        }

        if (EditorGUI.EndChangeCheck())
        {
            property.serializedObject.ApplyModifiedProperties();
        }

        Event e = Event.current;
        if (e.type == EventType.ContextClick && position.Contains(e.mousePosition))
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Use Constant"), isUseConstantProp.boolValue, () =>
            {
                EditorApplication.delayCall += () =>
                {
                    isUseConstantProp.boolValue = true;
                    property.serializedObject.ApplyModifiedProperties();
                };
            });
            menu.AddItem(new GUIContent("Use Variable"), !isUseConstantProp.boolValue, () =>
            {
                EditorApplication.delayCall += () =>
                {
                    isUseConstantProp.boolValue = false;
                    property.serializedObject.ApplyModifiedProperties();
                };
            });
            menu.ShowAsContext();
            e.Use();
        }

        EditorGUI.EndProperty();
    }
}
#endif

