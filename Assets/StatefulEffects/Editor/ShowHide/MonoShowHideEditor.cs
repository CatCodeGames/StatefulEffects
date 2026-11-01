using UnityEditor;
using UnityEngine;

namespace CatCode.StatefulEffects.EditorTools
{
    [CustomEditor(typeof(MonoShowHide), true)]
    [CanEditMultipleObjects]
    public class MonoShowHideEditor : Editor
    {
        public override bool RequiresConstantRepaint() => Application.isPlaying;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.Space();

            if (Application.isPlaying)
                EditorGUILayout.LabelField("State", ((MonoShowHide)target).State.ToString());

            using (new EditorGUI.DisabledScope(!Application.isPlaying))
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("Show"))
                        foreach (MonoShowHide showHide in targets)
                            showHide.Show();

                    if (GUILayout.Button("Hide"))
                        foreach (MonoShowHide showHide in targets)
                            showHide.Hide();
                }

                using (new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("Set Shown"))
                        foreach (MonoShowHide showHide in targets)
                            showHide.SetShown();

                    if (GUILayout.Button("Set Hidden"))
                        foreach (MonoShowHide showHide in targets)
                            showHide.SetHidden();
                }

                if (GUILayout.Button("Stop"))
                    foreach (MonoShowHide showHide in targets)
                        showHide.Stop();
            }
        }
    }
}
