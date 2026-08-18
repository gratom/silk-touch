#if UNITY_EDITOR

using System.Drawing;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Tools
{
    using UMP = UniversalMousePosition;

    /// <summary>
    /// Add-on for the Unity editor, which helps to quickly move from one scene to another
    /// </summary>
    internal class ScenePicker : EditorWindow
    {
        private static SceneInfo[] sceneInfos = default;
        private const int WIDTH = 200;
        private const int LINE_HEIGHT = 21;
        private const int BOTTOM_OUTFIT = 3;

        [MenuItem("Scenes/Choose scene", priority = 1)]
        private static void Init()
        {
            LoadSceneInfos();
            Vector2Int size = GetWindowSize();
            ScenePicker window = GetWindow<ScenePicker>();
            Vector2Int pos = UMP.GetCursorPosition();
            window.maxSize = size;
            window.minSize = size;
            window.position = new Rect(pos.x, pos.y, size.x, size.y);
            window.Show();
        }

        private static void LoadSceneInfos()
        {
            sceneInfos = SceneTool.GetSceneInfosInBuild();
        }

        private static Vector2Int GetWindowSize()
        {
            return new Vector2Int(WIDTH, sceneInfos.Length * LINE_HEIGHT + BOTTOM_OUTFIT);
        }

        private static void OpenScene(string path)
        {
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            EditorSceneManager.OpenScene(path);
        }

        private void OnGUI()
        {
            for (int i = 0; i < sceneInfos.Length; i++)
            {
                SceneInfo t = sceneInfos[i];
                GUI.enabled = t.Existing;
                if (GUILayout.Button(t.Name + $" (alt + {i})" + (t.Existing ? "" : "(deleted)")))
                {
                    OpenScene(t.Path);
                    Close();
                }
                GUI.enabled = true;
            }
        }

        // --- Hotkey access to first 3 scenes ---

        [MenuItem("Scenes/Quick Load/Scene 1 &1")]
        private static void LoadScene1()
        {
            LoadSceneAtIndex(0);
        }

        [MenuItem("Scenes/Quick Load/Scene 2 &2")]
        private static void LoadScene2()
        {
            LoadSceneAtIndex(1);
        }

        [MenuItem("Scenes/Quick Load/Scene 3 &3")]
        private static void LoadScene3()
        {
            LoadSceneAtIndex(2);
        }

        private static void LoadSceneAtIndex(int index)
        {
            SceneInfo[] scenes = SceneTool.GetSceneInfosInBuild();

            if (index < scenes.Length && scenes[index].Existing)
            {
                Debug.Log($"Opening scene: {scenes[index].Name}");
                EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
                EditorSceneManager.OpenScene(scenes[index].Path);
            }
            else
            {
                Debug.LogWarning($"Scene with index {index + 1} is not assigned or missing.");
            }
        }
    }
}

#endif