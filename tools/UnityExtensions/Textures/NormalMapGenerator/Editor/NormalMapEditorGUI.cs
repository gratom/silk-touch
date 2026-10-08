using UnityEditor;
using UnityEngine;

namespace SilkTouch.Tools.Textures.Editor
{
    // Same log-scale slider approach as SilkTouch.Tools.EditorGUILayoutExtensions.LogSlider.
    public static class NormalMapEditorGUI
    {
        public static float LogSlider(string label, float value, float min, float max)
        {
            float logMin = Mathf.Log10(min);
            float logMax = Mathf.Log10(max);
            float logCurrent = Mathf.Log10(Mathf.Clamp(value, min, max));

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(label);

            EditorGUI.BeginChangeCheck();
            float newLogScale = GUILayout.HorizontalSlider(logCurrent, logMin, logMax);
            bool sliderChanged = EditorGUI.EndChangeCheck();

            EditorGUI.BeginChangeCheck();
            float inputScale = EditorGUILayout.FloatField(value, GUILayout.Width(50));
            bool inputChanged = EditorGUI.EndChangeCheck();

            EditorGUILayout.EndHorizontal();

            if (sliderChanged)
            {
                return Mathf.Pow(10f, newLogScale);
            }

            if (inputChanged)
            {
                return Mathf.Clamp(inputScale, min, max);
            }

            return value;
        }
    }
}
