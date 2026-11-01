using UnityEditor;

namespace CatCode.StatefulEffects.EditorTools
{
    [CustomEditor(typeof(AnimatorShowHideConfig))]
    public sealed class AnimatorShowHideConfigEditor : Editor
    {
        private static readonly string[] HashFields =
        {
            "_shownStateHash",
            "_hiddenStateHash",
            "_showTriggerHash",
            "_hideTriggerHash",
            "_setShownTriggerHash",
            "_setHiddenTriggerHash"
        };

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Hashes", EditorStyles.boldLabel);

            using (new EditorGUI.DisabledScope(true))
            {
                foreach (var field in HashFields)
                {
                    var property = serializedObject.FindProperty(field);
                    EditorGUILayout.IntField(property.displayName, property.intValue);
                }
            }
        }
    }
}
