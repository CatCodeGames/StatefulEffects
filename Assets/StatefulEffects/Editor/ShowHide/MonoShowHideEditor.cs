
namespace CatCode.StatefulEffects.EditorTools
{
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(MonoShowHide), true)]
    public class MonoShowHideEditor : Editor
    {
        private void OnEnable()
        {
            EditorApplication.update += RepaintInspector;
        }

        private void OnDisable()
        {
            EditorApplication.update -= RepaintInspector;
        }

        private void RepaintInspector()
        {
            if (target != null)
                Repaint();
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("State", EditorStyles.boldLabel);

            var showHide = (MonoShowHide)target;

            string stateText = "n/a";
            if (Application.isPlaying && showHide != null && showHide.State != null)
                stateText = showHide.State.Value.ToString();

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.LabelField("State", stateText);
            EditorGUI.EndDisabledGroup();
        }
    }
}