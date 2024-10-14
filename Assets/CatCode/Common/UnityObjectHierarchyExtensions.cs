using UnityEngine;

namespace CatCode
{
    public static class UnityObjectHierarchyExtensions
    {
        public static bool TryGetComponentInParent<T>(this Component source, out T component) where T : Component
        {
            component = source.GetComponentInParent<T>();
            return component != null;
        }

        public static bool TryGetComponentInParent<T>(this GameObject source, out T component) where T : Component
        {
            component = source.GetComponentInParent<T>();
            return component != null;
        }

        public static bool TryGetComponentsInChildren<T>(this Component component, out T[] components) where T : Component
        {
            components = component.GetComponentsInChildren<T>();
            return components != null && components.Length > 0;
        }

        public static bool TryGetComponentsInChildren<T>(this GameObject gameObject, out T[] components) where T : Component
        {
            components = gameObject.GetComponentsInChildren<T>();
            return components != null && components.Length > 0;
        }

        public static bool TryGetComponentInChildren<T>(this GameObject gameObject, out T components) where T : Component
        {
            components = gameObject.GetComponentInChildren<T>();
            return components != null;
        }
    }
}