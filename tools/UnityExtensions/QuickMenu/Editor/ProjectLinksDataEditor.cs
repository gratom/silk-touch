#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace SilkTouch.Tools
{
    [CustomEditor(typeof(ProjectLinksData))]
    public class ProjectLinksDataEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            ProjectLinksData data = (ProjectLinksData)target;

            if (!data.IsInDefaultLocation)
            {
                EditorGUILayout.HelpBox(
                    $"Asset is not in the default directory!\nExpected location: {ProjectLinksData.DEFAULT_ASSET_PATH}",
                    MessageType.Warning);
            }

            DrawDefaultInspector();

            EditorGUILayout.Space(15);
            EditorGUILayout.HelpBox(
                "Shortcut keys format:\n" +
                "% = Ctrl (Windows) / Cmd (macOS)\n" +
                "# = Shift\n" +
                "& = Alt\n" +
                "Example: '%#1' means Ctrl+Shift+1",
                MessageType.Info);

            EditorGUILayout.Space(10);

            GUI.backgroundColor = new Color(0.35f, 0.75f, 0.35f);
            if (GUILayout.Button("Generate / Update Menu", GUILayout.Height(32)))
            {
                DynamicMenuGenerator.GenerateMenu(data);
            }

            GUI.backgroundColor = new Color(0.85f, 0.35f, 0.35f);
            if (GUILayout.Button("Remove Generated Menu", GUILayout.Height(24)))
            {
                if (EditorUtility.DisplayDialog("Remove Menu", "Are you sure you want to delete the generated menu script?", "Yes", "No"))
                {
                    DynamicMenuGenerator.RemoveGeneratedMenu();
                }
            }

            if (!data.IsInDefaultLocation)
            {
                EditorGUILayout.Space(5);
                GUI.backgroundColor = new Color(0.95f, 0.75f, 0.25f);
                if (GUILayout.Button("Move to Default Location", GUILayout.Height(28)))
                {
                    data.MoveToDefaultLocation();
                }
            }

            GUI.backgroundColor = Color.white;

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif