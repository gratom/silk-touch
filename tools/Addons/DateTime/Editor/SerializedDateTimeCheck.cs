#if UNITY_EDITOR
using System;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Tools
{
    public class SerializedDateTimeCheck : EditorWindow
    {
        private string inputText = "";
        private string resultText = "Enter JSON or Ticks to parse";

        [MenuItem("Tools/Editors/SerializedDateTime check", false, 1000)]
        public static void ShowWindow()
        {
            SerializedDateTimeCheck window = GetWindow<SerializedDateTimeCheck>("DateTime Checker");

            // Задаем фиксированный размер окна
            Vector2 size = new Vector2(350f, 260f);
            window.minSize = size;
            window.maxSize = size;
        }

        private void OnGUI()
        {
            GUILayout.Label("Paste JSON or raw Ticks below:", EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();
            inputText = EditorGUILayout.TextArea(inputText, GUILayout.Height(100));

            if (EditorGUI.EndChangeCheck())
            {
                ParseInput();
            }

            GUILayout.Space(10);
            GUILayout.Label("Result:", EditorStyles.boldLabel);

            EditorGUILayout.SelectableLabel(resultText, EditorStyles.wordWrappedLabel, GUILayout.Height(40));

            GUILayout.Space(10);
            if (GUILayout.Button("Clear", GUILayout.Height(25)))
            {
                inputText = "";
                resultText = "Enter JSON or Ticks to parse";
            }
        }

        private void ParseInput()
        {
            if (string.IsNullOrWhiteSpace(inputText))
            {
                resultText = "Input is empty";
                return;
            }

            Match match = Regex.Match(inputText, @"\d+");

            if (match.Success && long.TryParse(match.Value, out long ticks))
            {
                try
                {
                    DateTime utcTime = new DateTime(ticks, DateTimeKind.Utc);
                    resultText = $"UTC (Saved): {utcTime:yyyy-MM-dd HH:mm:ss}\nLocal (Your PC): {utcTime.ToLocalTime():yyyy-MM-dd HH:mm:ss}";
                }
                catch (ArgumentOutOfRangeException)
                {
                    resultText = "Ticks value is out of valid DateTime range.";
                }
            }
            else
            {
                resultText = "Could not find any valid ticks/numbers in the input.";
            }
        }
    }
}
#endif