#if UNITY_EDITOR

using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Tools
{
    public static class AssetExtensions
    {
        private const string additionSavePath = "/Standart_Saves/dynamicData";

        [MenuItem("Tools/BUILDS/Open Saves Folder &s",false,3000)]
        private static void OpenPlayerPrefsFolder()
        {
            string path = Application.persistentDataPath + additionSavePath;

            if (!Directory.Exists(path))
            {
                Debug.LogWarning($"PlayerPrefs folder not found: {path}");
                return;
            }

            EditorUtility.RevealInFinder(path);
            Debug.Log($"Opened PlayerPrefs folder: {path}");
        }

        /// <summary>
        /// Open in Project Window folder starting from relative path ("Assets/...") and selecting it
        /// </summary>
        public static async Task OpenFolder(this string path, Action OnComplete = null)
        {
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            string folderPath = Path.GetDirectoryName(path)?.Replace('\\', '/');
            if (string.IsNullOrEmpty(folderPath) || !folderPath.StartsWith("Assets"))
            {
                return;
            }

            DefaultAsset folderObj = AssetDatabase.LoadAssetAtPath<DefaultAsset>(folderPath)
                                     ?? AssetDatabase.LoadAssetAtPath<DefaultAsset>(folderPath + "/");
            if (folderObj == null)
            {
                return;
            }

            int count = path.Split('/').Length;
            for (int i = 0; i < count; i++)
            {
                OpenFolderAndShowContents(folderObj);
                await UniTask.Delay(100);
                await UniTask.SwitchToMainThread();
            }
            await UniTask.Delay(100);
            OnComplete?.Invoke();
        }

        /// <summary>
        /// Open folder of where the asset placed
        /// </summary>
        public static void OpenPrefabFolder(this Object obj)
        {
            if (obj == null)
            {
                return;
            }

            string path = AssetDatabase.GetAssetPath(obj);
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            path.OpenFolder();
        }

        private static void OpenFolderAndShowContents(Object folderObj)
        {
            EditorUtility.FocusProjectWindow();

            Type projectBrowserType = typeof(Editor).Assembly.GetType("UnityEditor.ProjectBrowser");
            if (projectBrowserType == null)
            {
                return;
            }

            EditorApplication.ExecuteMenuItem("Window/General/Project");
            EditorWindow.FocusWindowIfItsOpen(projectBrowserType);

            Object[] browsers = Resources.FindObjectsOfTypeAll(projectBrowserType);
            if (browsers == null || browsers.Length == 0)
            {
                return;
            }

            MethodInfo showMethod = projectBrowserType.GetMethod(
                "ShowFolderContents",
                BindingFlags.Instance | BindingFlags.NonPublic,
                null,
                new[] { typeof(int), typeof(bool) },
                null
            );

            if (showMethod == null)
            {
                return;
            }

            int id = folderObj.GetInstanceID();

            showMethod.Invoke(browsers[0], new object[] { id, true });

            Selection.activeObject = folderObj;
        }
    }
}
#endif