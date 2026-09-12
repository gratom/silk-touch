#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Tools
{
    public class ProjectLinksData : ScriptableObject
    {
        public const string DIRECTORY_PATH = "Assets/silk-touch/generated/projectLinks";
        public const string DEFAULT_ASSET_PATH = DIRECTORY_PATH + "/ProjectLinksData.asset";

        public List<LinkItem> links = new List<LinkItem>();

        public bool IsInDefaultLocation
        {
            get
            {
                string currentPath = AssetDatabase.GetAssetPath(this);
                return currentPath == DEFAULT_ASSET_PATH;
            }
        }

        [Serializable]
        public class LinkItem
        {
            public string title = "New Link";
            public string menuPath = "Tools/Open Links/My Link";
            public string url = "https://";
            public string shortcut = "%#l"; // Ctrl+Shift+L
        }

        [MenuItem("Tools/Project Links/Settings", false, 0)]
        public static void OpenOrCreateSettings()
        {
            ProjectLinksData asset = FindAsset();

            if (asset == null)
            {
                asset = CreateAsset();
            }

            Selection.activeObject = asset;
            EditorGUIUtility.PingObject(asset);
        }

        public void MoveToDefaultLocation()
        {
            string currentPath = AssetDatabase.GetAssetPath(this);
            if (currentPath == DEFAULT_ASSET_PATH)
            {
                return;
            }

            if (!Directory.Exists(DIRECTORY_PATH))
            {
                Directory.CreateDirectory(DIRECTORY_PATH);
            }

            AssetDatabase.MoveAsset(currentPath, DEFAULT_ASSET_PATH);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static ProjectLinksData FindAsset()
        {
            string[] guids = AssetDatabase.FindAssets($"t:{nameof(ProjectLinksData)}");
            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                return AssetDatabase.LoadAssetAtPath<ProjectLinksData>(path);
            }

            return null;
        }

        private static ProjectLinksData CreateAsset()
        {
            if (!Directory.Exists(DIRECTORY_PATH))
            {
                Directory.CreateDirectory(DIRECTORY_PATH);
            }

            ProjectLinksData asset = CreateInstance<ProjectLinksData>();
            AssetDatabase.CreateAsset(asset, DEFAULT_ASSET_PATH);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return asset;
        }
    }
}
#endif