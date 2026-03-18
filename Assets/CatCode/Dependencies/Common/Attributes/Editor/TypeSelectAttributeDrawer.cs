using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace CatCode.Common.Editor
{
    [CustomPropertyDrawer(typeof(TypeSelectAttribute))]
    public class TypeSelectElementDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var fieldType = fieldInfo.FieldType;
            var baseType = GetBaseType(fieldType);

            float line = EditorGUIUtility.singleLineHeight;

            var foldoutRect = new Rect(position.x, position.y, position.width - 80, line);
            var foldoutContent = $"{label.text}: {GetTypeName(property)}";
            property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, foldoutContent, true);

            var buttonRect = new Rect(position.x + position.width - 80, position.y, 80, line);
            var buttonContent = "Type";
            if (GUI.Button(buttonRect, buttonContent))
                ShowTypeMenu(baseType, property);

            if (property.isExpanded && property.managedReferenceValue != null)
            {
                var contentRect = new Rect(position.x, position.y, position.width, EditorGUI.GetPropertyHeight(property, true));
                EditorGUI.PropertyField(contentRect, property, GUIContent.none, true);
            }
        }

        private Type GetBaseType(Type fieldType) 
        {
            if (fieldType.IsArray)
                return fieldType.GetElementType();
            if(fieldType.IsGenericType)
                return fieldType.GetGenericArguments()[0];
            return fieldType;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float h = 0;
            if (property.managedReferenceValue != null)
                h += EditorGUI.GetPropertyHeight(property, true);
            else
                h = EditorGUIUtility.singleLineHeight;
            return h;
        }

        private void ShowTypeMenu(Type baseType, SerializedProperty property)
        {
            var menu = new GenericMenu();

            menu.AddItem(new GUIContent("None"), false,
                () => SetValue(property, null));

            foreach (var type in FindAssignableTypes(baseType))
            {
                menu.AddItem(new GUIContent(type.Name), false,
                    () => SetValue(property, type));
            }

            menu.ShowAsContext();
        }

        private void SetValue(SerializedProperty property, Type type)
        {
            property.serializedObject.Update();
            property.managedReferenceValue = type == null ? null : Activator.CreateInstance(type);
            property.serializedObject.ApplyModifiedProperties();
        }

        private IEnumerable<Type> FindAssignableTypes(Type baseType)
        {         
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.IsDynamic)
                .SelectMany(a =>
                {
                    try { return a.GetTypes(); }
                    catch { return Array.Empty<Type>(); }
                })
                .Where(t => baseType.IsAssignableFrom(t)
                        && !t.IsInterface
                        && !t.IsAbstract
                        && t.GetCustomAttribute<SerializableAttribute>() != null
                        && t.GetConstructor(Type.EmptyTypes) != null)
                .OrderBy(t => t.Name);
            return types;
        }

        private string GetTypeName(SerializedProperty property)
        {
            if (string.IsNullOrEmpty(property.managedReferenceFullTypename))
                return "Null";

            var parts = property.managedReferenceFullTypename.Split(' ');
            if (parts.Length < 2)
                return "Unknown";

            string className = parts[1];

            var type = AppDomain.CurrentDomain.GetAssemblies()
                .Select(a => a.GetType(className))
                .FirstOrDefault(t => t != null);

            return type != null ? type.Name : className;
        }
    }
}