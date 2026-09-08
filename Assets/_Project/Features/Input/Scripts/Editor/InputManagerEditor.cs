using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
namespace Features.Input.Editor
{
    [CustomEditor(typeof(InputManager))]
    public class InputManagerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var manager = (InputManager)target;

            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Disponível apenas em Play Mode.", MessageType.Info);
                return;
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Controles", EditorStyles.boldLabel);

            foreach (var map in manager.RegisteredMaps)
            {
                bool isEnabled = manager.IsMapEnabled(map);

                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField(
                    $"{map} {(isEnabled ? "●" : "○")}",
                    GUILayout.Width(150)
                );

                using (new EditorGUI.DisabledScope(isEnabled))
                {
                    if (GUILayout.Button("Enable"))
                        manager.EnableMap(map);
                }

                using (new EditorGUI.DisabledScope(!isEnabled))
                {
                    if (GUILayout.Button("Disable"))
                        manager.DisableMap(map);
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Switch", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();
            foreach (var map in manager.RegisteredMaps)
            {
                if (GUILayout.Button($"→ {map}"))
                    manager.SwitchToMap(map);
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}
#endif