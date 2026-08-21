#if UI_TMP
using System;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Tools
{

    [ExecuteAlways]
    [RequireComponent(typeof(CanvasScaler))]
    public class DynamicCanvasScaler : MonoBehaviour
    {
        public AnimationCurve matchCurve = AnimationCurve.Linear(0.5f, 0, 2.5f, 1);
        private CanvasScaler _scaler;

        public CanvasScaler Scaler
        {
            get
            {
                if (_scaler == null)
                {
                    _scaler = GetComponent<CanvasScaler>();
                }
                return _scaler;
            }
        }

        public float CurrentAspect => (float)Screen.width / Screen.height;

        private void Update()
        {
            ApplyMatch();
        }

        public void ApplyMatch()
        {
            CanvasScaler s = Scaler;
            if (s == null || s.uiScaleMode != CanvasScaler.ScaleMode.ScaleWithScreenSize)
            {
                return;
            }

            float matchValue = matchCurve.Evaluate(CurrentAspect);
            s.matchWidthOrHeight = Mathf.Clamp01(matchValue);

#if UNITY_EDITOR
            editorCashScreenWidth = Screen.width;
            editorCashScreenHeight = Screen.height;
            editorCashScreenCurrentAspect = CurrentAspect;
#endif
        }

#if UNITY_EDITOR
        [NonSerialized] public int editorCashScreenWidth;
        [NonSerialized] public int editorCashScreenHeight;
        [NonSerialized] public float editorCashScreenCurrentAspect;
#endif
    }

#if UNITY_EDITOR

    [CustomEditor(typeof(DynamicCanvasScaler))]
    public class DynamicCanvasScalerEditor : Editor
    {
        public override bool RequiresConstantRepaint()
        {
            return true;
        }

        public override void OnInspectorGUI()
        {
            DynamicCanvasScaler script = (DynamicCanvasScaler)target;

            EditorGUI.BeginChangeCheck();
            DrawDefaultInspector();
            if (EditorGUI.EndChangeCheck())
            {
                script.ApplyMatch();
            }

            EditorGUILayout.Space(10);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            float aspect = script.editorCashScreenCurrentAspect;
            int w = script.editorCashScreenWidth;
            int h = script.editorCashScreenHeight;

            EditorGUILayout.LabelField("Live Screen Info", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Resolution:", $"{w} x {h}");
            EditorGUILayout.LabelField("Aspect Ratio:", $"[{aspect:F2}:1]      [1:{1 / aspect:F2}]");

            float currentMatch = script.Scaler != null ? script.Scaler.matchWidthOrHeight : 0;
            EditorGUILayout.LabelField("Current Match:", $"{currentMatch:F3}");

            EditorGUILayout.EndVertical();
        }
    }

#endif

}
#endif