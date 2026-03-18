using CatCode.StatefulEffects;
using UnityEditor;
using UnityEngine;

namespace CatCode.ShowHide.EditorTools
{
    [CustomEditor(typeof(AnimatorShowHide), true)]
    public class AnimatorShowHideEditor : Editor
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