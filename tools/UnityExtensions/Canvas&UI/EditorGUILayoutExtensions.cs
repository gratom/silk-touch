#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace SilkTouch.Tools
{
    public class EditorGUILayoutExtensions
    {
        public static float LogSlider(string label, float value, float min = 0.1f, float max = 10f)
        {
            return LogSlider(new GUIContent(label), value, min, max);
        }

        public static float LogSlider(GUIContent label, float value, float min = 0.1f, float max = 10f)
        {
            float logMin = Mathf.Log10(min);
            float logMax = Mathf.Log10(max);
            float logCurrent = Mathf.Log10(Mathf.Clamp(value, min, max));

            EditorGUILayout.BeginHorizontal();

            if (label != null && !string.IsNullOrEmpty(label.text))
            {
                EditorGUILayout.PrefixLabel(label);
            }

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
#endif