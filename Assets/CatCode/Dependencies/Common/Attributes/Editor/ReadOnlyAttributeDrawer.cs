using UnityEditor;
using UnityEngine;

namespace CatCode.Common.Editor
{
    [CustomPropertyDrawer(typeof(ReadOnlyAttribute), false)]
    public class ReadOnlyAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var readOnly = (ReadOnlyAttribute)attribute;

            bool shouldDisable =
                (readOnly.Mode & ReadOnlyMode.InRuntime) != 0 && Application.isPlaying ||
                (readOnly.Mode & ReadOnlyMode.InEditor) != 0 && !Application.isPlaying;

            bool prev = GUI.enabled;
            GUI.enabled = !shouldDisable;

            EditorGUI.PropertyField(position, property, label, true);

            GUI.enabled = prev;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }
    }
}