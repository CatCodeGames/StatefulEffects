
namespace CatCode.StatefulEffects.EditorTools
{
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(MonoEffectAction), true)]
    public class MonoEffectActionEditor : Editor
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

            var effectAction = (MonoEffectAction)target;

            string stateText = "n/a";
            if (Application.isPlaying && effectAction != null && effectAction.State != null)
                stateText = effectAction.State.Value.ToString();
            
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.LabelField("State", stateText);
            EditorGUILayout.LabelField("IsRunning", effectAction.IsRunning.ToString());
            EditorGUILayout.LabelField("Initialized", effectAction.Initialized.ToString());
            EditorGUI.EndDisabledGroup();
        }
    }
}