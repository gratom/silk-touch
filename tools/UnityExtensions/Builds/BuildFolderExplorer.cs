#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System;

namespace Tools
{
    public static class BuildFolderExporter
    {
        [MenuItem("Tools/BUILDS/Open Build Folder #b", false, 3000)]
        public static void OpenAndSelectLastBuild()
        {
            string projectRoot = Directory.GetParent(Application.dataPath)?.Parent?.Parent?.FullName ?? Application.dataPath; //try find here, but not stable
            string buildsPath = Path.Combine(projectRoot, "builds");

            if (!Directory.Exists(buildsPath))
            {
                EditorUtility.DisplayDialog("Folder Not Found", "Folder not found:\n" + buildsPath, "Ок");
                return;
            }

            string[] entries = Directory.GetFileSystemEntries(buildsPath);

            if (entries.Length == 0)
            {
                EditorUtility.RevealInFinder(buildsPath);
                return;
            }

            string lastBuild = entries
                .OrderByDescending(path => GetVersion(Path.GetFileName(path)))
                .FirstOrDefault();

            EditorUtility.RevealInFinder(!string.IsNullOrEmpty(lastBuild) ? lastBuild : buildsPath);
        }

        private static Version GetVersion(string fileName)
        {
            Match match = Regex.Match(fileName, @"\d+\.\d+(\.\d+)?");
            if (match.Success && Version.TryParse(match.Value, out Version version))
            {
                return version;
            }

            return new Version(0, 0, 0);
        }
    }
    
#endif
}