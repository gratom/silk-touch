#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace SilkTouch.Tools
{
    public class AspectRatioTool : EditorWindow
    {
        private Texture2D selectedTexture;
        private int originalWidth;
        private int originalHeight;

        private float scaleMultiplier = 1f;
        private string outputWidthStr = "0";
        private string outputHeightStr = "0";

        [MenuItem("Assets/Aspect Ratio..")]
        private static void OpenWindow()
        {
            GetWindow<AspectRatioTool>("Aspect Ratio Calculator");
        }

        [MenuItem("Assets/Aspect Ratio..", true)]
        private static bool ValidateOpenWindow()
        {
            return Selection.activeObject is Texture2D;
        }

        private void OnSelectionChange()
        {
            UpdateSelectedTexture();
            Repaint();
        }

        private void OnEnable()
        {
            UpdateSelectedTexture();
        }

        private void UpdateSelectedTexture()
        {
            selectedTexture = Selection.activeObject as Texture2D;

            if (selectedTexture != null)
            {
                originalWidth = selectedTexture.width;
                originalHeight = selectedTexture.height;
                RecalculateFromMultiplier();
            }
        }

        private void OnGUI()
        {
            if (selectedTexture == null)
            {
                EditorGUILayout.HelpBox("Select a Texture2D asset in the Project window to use this tool.", MessageType.Info);
                return;
            }

            EditorGUILayout.LabelField($"Texture: {selectedTexture.name}", EditorStyles.boldLabel);
            EditorGUILayout.LabelField($"Original Size: {originalWidth} x {originalHeight}");

            EditorGUILayout.Space();

            EditorGUI.BeginChangeCheck();
            scaleMultiplier = EditorGUILayoutExtensions.LogSlider("Multiplier", scaleMultiplier, 0.01f, 10f);
            if (EditorGUI.EndChangeCheck())
            {
                RecalculateFromMultiplier();
            }

            EditorGUILayout.Space();

            EditorGUI.BeginChangeCheck();
            outputWidthStr = EditorGUILayout.TextField("Output Width", outputWidthStr);
            if (EditorGUI.EndChangeCheck())
            {
                if (float.TryParse(outputWidthStr, out float newWidth) && originalWidth > 0)
                {
                    scaleMultiplier = newWidth / originalWidth;
                    outputHeightStr = Mathf.RoundToInt(originalHeight * scaleMultiplier).ToString();
                }
            }

            EditorGUI.BeginChangeCheck();
            outputHeightStr = EditorGUILayout.TextField("Output Height", outputHeightStr);
            if (EditorGUI.EndChangeCheck())
            {
                if (float.TryParse(outputHeightStr, out float newHeight) && originalHeight > 0)
                {
                    scaleMultiplier = newHeight / originalHeight;
                    outputWidthStr = Mathf.RoundToInt(originalWidth * scaleMultiplier).ToString();
                }
            }

            if (GUILayout.Button("Reset Scale"))
            {
                scaleMultiplier = 1f;
                RecalculateFromMultiplier();
            }
        }

        private void RecalculateFromMultiplier()
        {
            outputWidthStr = Mathf.RoundToInt(originalWidth * scaleMultiplier).ToString();
            outputHeightStr = Mathf.RoundToInt(originalHeight * scaleMultiplier).ToString();
        }
    }
}
#endif