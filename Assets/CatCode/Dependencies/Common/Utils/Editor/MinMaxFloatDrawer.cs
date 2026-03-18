using UnityEditor;
using UnityEngine;

namespace CatCode.Common.Editor
{
    [CustomPropertyDrawer(typeof(MinMaxFloat))]
    public class MinMaxFloatDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var min = property.FindPropertyRelative("min");
            var max = property.FindPropertyRelative("max");

            // Рисуем имя поля и получаем оставшуюся область
            position = EditorGUI.PrefixLabel(position, label);

            float half = position.width * 0.5f;

            var minRect = new Rect(position.x, position.y, half - 2, position.height);
            var maxRect = new Rect(position.x + half + 2, position.y, half - 2, position.height);

            EditorGUI.PropertyField(minRect, min, GUIContent.none);
            EditorGUI.PropertyField(maxRect, max, GUIContent.none);

            EditorGUI.EndProperty();
        }
    }
}