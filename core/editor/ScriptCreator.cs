#if UNITY_EDITOR

using System.Diagnostics;
using System.IO;
using Unity.CodeEditor;
using UnityEditor;
using UnityEngine;

namespace SilkTouch.EditorScripts
{
    public class ScriptCreator : Editor
    {
        private const string TEMPLATE_FILENAME_MANAGER = "managerTemplate.cs.txt";
        private const string TEMPLATE_FILENAME_UI = "uiTemplate.cs.txt";

        [MenuItem("Assets/Create/Architecture/Service manager", priority = 51)]
        public static void CreateManager()
        {
            string templatePath = FindTemplatePath(TEMPLATE_FILENAME_MANAGER);
            if (templatePath == null)
            {
                return;
            }

            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(templatePath, "DefaultManager.cs");
        }

        [MenuItem("Assets/Create/Architecture/UI panel", priority = 51)]
        private static void CreateUI()
        {
            string templatePath = FindTemplatePath(TEMPLATE_FILENAME_UI);
            if (templatePath == null)
            {
                return;
            }

            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(templatePath, "NewUIPanel.cs");
        }

        private static string FindTemplatePath(string templateFileName)
        {
            string[] matches = Directory.GetFiles(Application.dataPath, templateFileName, SearchOption.AllDirectories);

            if (matches.Length == 0)
            {
                UnityEngine.Debug.LogError($"Template '{templateFileName}' not found anywhere under Assets.");
                return null;
            }

            if (matches.Length > 1)
            {
                UnityEngine.Debug.LogWarning($"Multiple templates named '{templateFileName}' found under Assets, using the first one: {matches[0]}");
            }

            string fullPath = matches[0].Replace('\\', '/');
            string projectPath = Path.GetDirectoryName(Application.dataPath)!.Replace('\\', '/');
            return fullPath.Substring(projectPath.Length + 1);
        }
    }

    public static class HotKeyScriptEditor
    {
        //[MenuItem("Scripts/Edit active script &e")]
        private static void EditCurrentScript()
        {
            Object active = Selection.activeObject;

            if (active == null)
            {
                return;
            }

            if (active is GameObject go)
            {
                Component[] components = go.GetComponents<Component>();
                for (int i = 0; i < components.Length; i++)
                {
                    if (components[i] == null || components[i] is Transform)
                    {
                        continue;
                    }

                    MonoScript script = MonoScript.FromMonoBehaviour(components[i] as MonoBehaviour);
                    if (script != null)
                    {
                        string ns = components[i].GetType().Namespace;
                        if (string.IsNullOrEmpty(ns) || !(ns.StartsWith("UnityEngine") || ns.Contains("TMPro")))
                        {
                            AssetDatabase.OpenAsset(script);
                            return;
                        }
                    }
                }
            }
            else if (active is MonoScript monoScript)
            {
                AssetDatabase.OpenAsset(monoScript);
            }
            else if (active is ScriptableObject so)
            {
                MonoScript script = MonoScript.FromScriptableObject(so);
                if (script != null)
                {
                    AssetDatabase.OpenAsset(script);
                }
            }
        }

        [MenuItem("Scripts/Edit active script &e")]
        private static void EditCurrentScriptViaRider()
        {
            Object active = Selection.activeObject;

            if (active == null)
            {
                return;
            }

            if (active is GameObject go)
            {
                Component[] components = go.GetComponents<Component>();
                for (int i = 0; i < components.Length; i++)
                {
                    if (components[i] == null || components[i] is Transform)
                    {
                        continue;
                    }

                    MonoScript script = MonoScript.FromMonoBehaviour(components[i] as MonoBehaviour);
                    if (script != null)
                    {
                        string ns = components[i].GetType().Namespace;
                        if (string.IsNullOrEmpty(ns) || !(ns.StartsWith("UnityEngine") || ns.Contains("TMPro")))
                        {
                            OpenInRider(script);
                            return;
                        }
                    }
                }
            }
            else if (active is MonoScript monoScript)
            {
                OpenInRider(monoScript);
            }
            else if (active is ScriptableObject so)
            {
                MonoScript script = MonoScript.FromScriptableObject(so);
                if (script != null)
                {
                    OpenInRider(script);
                }
            }
        }

        private static void OpenInRider(MonoScript script)
        {
            // 1. Получаем путь к IDE из настроек Unity
            string riderPath = CodeEditor.CurrentEditorInstallation;

            if (string.IsNullOrEmpty(riderPath) || !riderPath.ToLower().Contains("rider"))
            {
                UnityEngine.Debug.LogWarning("Rider не выбран в External Tools или путь пуст. Используем стандартный OpenAsset.");
                AssetDatabase.OpenAsset(script);
                return;
            }

            // 2. Получаем абсолютный путь к файлу скрипта
            string assetPath = AssetDatabase.GetAssetPath(script);
            string fullFilePath = Path.GetFullPath(assetPath);

            // 3. Находим .sln файл проекта в корне Unity
            string projectDir = Path.GetDirectoryName(Application.dataPath);
            string projectName = Path.GetFileName(projectDir);
            string slnPath = Path.Combine(projectDir, $"{projectName}.sln");

            // 4. Формируем аргументы (передаем .sln, чтобы не плодить новые окна Rider)
            string arguments = $"\"{slnPath}\" --line 1 \"{fullFilePath}\"";

            // 5. Запускаем процесс
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = riderPath,
                Arguments = arguments,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            Process.Start(startInfo);
        }
    }
}

#endif