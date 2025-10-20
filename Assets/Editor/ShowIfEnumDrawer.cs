#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ShowIfEnumAttribute))]
public class ShowIfEnumDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        ShowIfEnumAttribute showIf = (ShowIfEnumAttribute)attribute;

        SerializedProperty enumProp = property.serializedObject.FindProperty(showIf.EnumFieldName);

        if (enumProp != null && enumProp.propertyType == SerializedPropertyType.Enum)
        {
            int currentValue = enumProp.enumValueIndex;

            if (System.Array.Exists(showIf.EnumValues, val => val == currentValue))
            {
                EditorGUI.PropertyField(position, property, label, true);
            }
        }
        else
        {
            Debug.LogWarning($"ShowIfEnum: enum field '{showIf.EnumFieldName}' not found or not enum.");
            EditorGUI.PropertyField(position, property, label, true);
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        ShowIfEnumAttribute showIf = (ShowIfEnumAttribute)attribute;
        SerializedProperty enumProp = property.serializedObject.FindProperty(showIf.EnumFieldName);

        if (enumProp != null && enumProp.propertyType == SerializedPropertyType.Enum)
        {
            int currentValue = enumProp.enumValueIndex;

            if (System.Array.Exists(showIf.EnumValues, val => val == currentValue))
            {
                return EditorGUI.GetPropertyHeight(property, label, true);
            }
            else
            {
                return -EditorGUIUtility.standardVerticalSpacing;
            }
        }

        return EditorGUI.GetPropertyHeight(property, label, true);
    }
}
#endif
