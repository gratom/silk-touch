#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace SilkTouch.Managers
{
    using Tools;
    public static class ManagerInstanceCreator
    {
        private const string MENU_PATH = "Assets/Create Manager Instance";
        private const string PREFAB_FOLDER = "Assets/prefabs/managers";

        [MenuItem(MENU_PATH, true)]
        private static bool Validate_CreateManagerInstance()
        {
            MonoScript mono = Selection.activeObject as MonoScript;
            if (mono == null)
            {
                return false;
            }

            Type t = mono.GetClass();
            if (t == null)
            {
                return false;
            }

            return !t.IsAbstract
                   && typeof(MonoBehaviour).IsAssignableFrom(t)
                   && t.IsSubclassOf(typeof(BaseManager));
        }

        public static void CreateFromType(Type type)
        {
            Type managerType = type;
            if (managerType == null)
            {
                EditorUtility.DisplayDialog("Create Manager Instance",
                    "Error during creation prefab.\nCan not get class type from this script.", "OK");
                return;
            }

            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            GameObject go = new GameObject(managerType.Name);
            try
            {
                go.AddComponent(managerType);
            }
            catch (Exception e)
            {
                UnityEngine.Object.DestroyImmediate(go);
                EditorUtility.DisplayDialog("Create Manager Instance",
                    $"Error during creation prefab.\nCan not add component type of [{managerType.Name}.\n{e.GetType().Name}]: {e.Message}", "OK");
                return;
            }

            string fullPrefabPath = SaverLoaderModule.OverGeneratePath(Path.Combine(PREFAB_FOLDER, managerType.Name + ".prefab"));

            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(go, fullPrefabPath);

            if (prefab != null)
            {
                AssetDatabase.Refresh();
                AssetDatabase.OpenAsset(prefab);
            }
            else
            {
                EditorUtility.DisplayDialog("Create Manager Instance",
                    "Error during save prefab in folder.", "OK");
            }

            UnityEngine.Object.DestroyImmediate(go);
        }

        [MenuItem(MENU_PATH, false, 2000)]
        private static void CreateManagerInstance()
        {
            MonoScript mono = Selection.activeObject as MonoScript;
            if (mono == null)
            {
                return;
            }
            CreateFromType(mono.GetClass());
        }
    }
}
#endif