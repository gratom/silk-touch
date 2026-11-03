#if UNITY_EDITOR

using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace SilkTouch.Tools.ProjectDeployment
{
    public static class ProjectDeployer
    {
        private const string TEMPLATES_FOLDER_NAME = "projectDeploymentTemplate";
        private const string DESTINATION_RELATIVE_PATH = "scripts/proj";
        private const string TEMPLATE_EXTENSION = ".cs.txt";

        [MenuItem("SilkTouch/Deploy Project")]
        public static void DeployProject()
        {
            string templatesRoot = FindTemplatesRoot();
            if (templatesRoot == null)
            {
                return;
            }

            string destinationRoot = Path.Combine(Application.dataPath, DESTINATION_RELATIVE_PATH);

            List<string> created = new List<string>();
            List<string> skipped = new List<string>();

            string[] templateFiles = Directory.GetFiles(templatesRoot, "*" + TEMPLATE_EXTENSION, SearchOption.AllDirectories);
            foreach (string templateFile in templateFiles)
            {
                string relativePath = templateFile.Substring(templatesRoot.Length + 1).Replace('\\', '/');
                string targetRelativePath = relativePath.Substring(0, relativePath.Length - ".txt".Length);
                string targetFullPath = Path.Combine(destinationRoot, targetRelativePath);

                if (File.Exists(targetFullPath))
                {
                    skipped.Add(targetRelativePath);
                    continue;
                }

                Directory.CreateDirectory(Path.GetDirectoryName(targetFullPath));
                File.Copy(templateFile, targetFullPath);
                created.Add(targetRelativePath);
            }

            AssetDatabase.Refresh();

            LogResult(created, skipped);
            ShowResultDialog(created, skipped);
        }

        private static string FindTemplatesRoot()
        {
            string[] matches = Directory.GetDirectories(Application.dataPath, TEMPLATES_FOLDER_NAME, SearchOption.AllDirectories);

            if (matches.Length == 0)
            {
                Debug.LogError($"[Silk Touch] Deploy Project: templates folder '{TEMPLATES_FOLDER_NAME}' not found anywhere under Assets.");
                return null;
            }

            if (matches.Length > 1)
            {
                Debug.LogWarning($"[Silk Touch] Deploy Project: multiple '{TEMPLATES_FOLDER_NAME}' folders found under Assets, using the first one: {matches[0]}");
            }

            return matches[0];
        }

        private static void LogResult(List<string> created, List<string> skipped)
        {
            string message = $"[Silk Touch] Deploy Project finished. Created: {created.Count}, skipped (already exist): {skipped.Count}.";

            if (created.Count > 0)
            {
                message += "\nCreated:\n" + string.Join("\n", created);
            }

            if (skipped.Count > 0)
            {
                message += "\nSkipped:\n" + string.Join("\n", skipped);
            }

            Debug.Log(message);
        }

        private static void ShowResultDialog(List<string> created, List<string> skipped)
        {
            string message = $"Created: {created.Count}\nSkipped (already exist): {skipped.Count}";
            EditorUtility.DisplayDialog("Deploy Project", message, "OK");
        }
    }
}

#endif
