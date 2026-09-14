#if UNITY_EDITOR
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace SilkTouch.Tools
{
    public static class DynamicMenuGenerator
    {
        private const string GENERATED_FILE_PATH = ProjectLinksData.DIRECTORY_PATH + "/GeneratedProjectLinks.cs";

        public static void GenerateMenu(ProjectLinksData data)
        {
            if (data == null)
            {
                Debug.LogError("ProjectLinksData is null!");
                return;
            }

            StringBuilder builder = new StringBuilder();

            builder.AppendLine("// AUTO-GENERATED CODE. DO NOT MODIFY MANUALLY.");
            builder.AppendLine("#if UNITY_EDITOR");
            builder.AppendLine("using UnityEditor;");
            builder.AppendLine("using UnityEngine;");
            builder.AppendLine();
            builder.AppendLine("namespace SilkTouch.Tools.Generated");
            builder.AppendLine("{");
            builder.AppendLine("    public static class GeneratedProjectLinks");
            builder.AppendLine("    {");

            for (int i = 0; i < data.links.Count; i++)
            {
                ProjectLinksData.LinkItem item = data.links[i];

                if (string.IsNullOrWhiteSpace(item.menuPath) || string.IsNullOrWhiteSpace(item.url))
                {
                    continue;
                }

                string shortcutSuffix = string.IsNullOrWhiteSpace(item.shortcut) ? "" : $" {item.shortcut.Trim()}";
                string fullMenuPath = $"{item.menuPath.Trim()}{shortcutSuffix}";

                builder.AppendLine($"        [MenuItem(\"{fullMenuPath}\", false, {100 + i})]");
                builder.AppendLine($"        private static void OpenLink_{i}()");
                builder.AppendLine("        {");
                builder.AppendLine($"            Application.OpenURL(\"{item.url.Trim()}\");");
                builder.AppendLine("        }");
                builder.AppendLine();
            }

            builder.AppendLine("    }");
            builder.AppendLine("}");
            builder.AppendLine("#endif");

            string directory = Path.GetDirectoryName(GENERATED_FILE_PATH);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllText(GENERATED_FILE_PATH, builder.ToString());
            AssetDatabase.Refresh();

            Debug.Log($"Menu code generated successfully at: {GENERATED_FILE_PATH}");
        }

        public static void RemoveGeneratedMenu()
        {
            if (File.Exists(GENERATED_FILE_PATH))
            {
                File.Delete(GENERATED_FILE_PATH);

                string metaPath = GENERATED_FILE_PATH + ".meta";
                if (File.Exists(metaPath))
                {
                    File.Delete(metaPath);
                }

                AssetDatabase.Refresh();
                Debug.Log("Generated menu file removed.");
            }
        }
    }
}
#endif