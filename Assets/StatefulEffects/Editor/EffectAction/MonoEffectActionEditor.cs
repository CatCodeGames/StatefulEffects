using UnityEditor;
using UnityEngine;

namespace CatCode.StatefulEffects.EditorTools
{
    [CustomEditor(typeof(MonoEffectAction), true)]
    [CanEditMultipleObjects]
    public class MonoEffectActionEditor : Editor
    {
        public override bool RequiresConstantRepaint() => Application.isPlaying;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.Space();

            if (Application.isPlaying)
                EditorGUILayout.LabelField("State", ((MonoEffectAction)target).State.ToString());

            using (new EditorGUI.DisabledScope(!Application.isPlaying))
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Play"))
                    foreach (MonoEffectAction effectAction in targets)
                        effectAction.Play();

                if (GUILayout.Button("Prepare"))
                    foreach (MonoEffectAction effectAction in targets)
                        effectAction.Prepare();

                if (GUILayout.Button("Stop"))
                    foreach (MonoEffectAction effectAction in targets)
                        effectAction.Stop();
            }
        }
    }
}
