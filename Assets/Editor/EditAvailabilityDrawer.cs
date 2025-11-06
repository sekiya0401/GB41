using UnityEditor;
using UnityEngine;

namespace MS.Extensions
{
    [CustomPropertyDrawer(typeof(EditAvailabilityAttribute))]
    public class EditAvailabilityDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var attr = (EditAvailabilityAttribute)attribute;

            bool isPlaying = Application.isPlaying;
            bool shouldDisable = ShouldDisable(attr.Mode, isPlaying);

            EditorGUI.BeginDisabledGroup(shouldDisable);
            EditorGUI.PropertyField(position, property, label, true);
            EditorGUI.EndDisabledGroup();
        }

        private bool ShouldDisable(EditAvailabilityMode state, bool isPlaying)
        {
            switch (state)
            {
                case EditAvailabilityMode.AlwaysDisabled:
                    return true;

                case EditAvailabilityMode.PlayModeOnly:
                    return isPlaying;

                case EditAvailabilityMode.EditModeOnly:
                    return !isPlaying;

                default:
                    return false;
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }
    }
}