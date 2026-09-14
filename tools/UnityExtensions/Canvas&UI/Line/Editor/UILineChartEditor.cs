#if UNITY_EDITOR && UI_TMP
using UnityEditor;
using UnityEngine;

namespace Tools
{
    [CustomEditor(typeof(UILineChart))]
    [CanEditMultipleObjects]
    public class UILineChartEditor : Editor
    {
        private SerializedProperty pointsProp;
        private SerializedProperty thicknessProp;
        private SerializedProperty joinTypeProp;
        private SerializedProperty miterLimitProp;
        private SerializedProperty bevelAngleThresholdProp;
        private SerializedProperty colorProp;
        private SerializedProperty raycastTargetProp;

        private UILineChart lineChart;

        private bool isCreatingPoint;
        private int draggedNewPointIndex = -1;

        private void OnEnable()
        {
            lineChart = (UILineChart)target;

            pointsProp = serializedObject.FindProperty("points");
            thicknessProp = serializedObject.FindProperty("thickness");
            joinTypeProp = serializedObject.FindProperty("joinType");
            miterLimitProp = serializedObject.FindProperty("miterLimit");
            bevelAngleThresholdProp = serializedObject.FindProperty("bevelAngleThreshold");

            colorProp = serializedObject.FindProperty("m_Color");
            raycastTargetProp = serializedObject.FindProperty("m_RaycastTarget");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(colorProp);
            EditorGUILayout.PropertyField(raycastTargetProp);
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(pointsProp, true);
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(thicknessProp);
            EditorGUILayout.PropertyField(joinTypeProp);

            UILineChart.JoinType currentJoinType = (UILineChart.JoinType)joinTypeProp.enumValueIndex;

            switch (currentJoinType)
            {
                case UILineChart.JoinType.Miter:
                    EditorGUILayout.PropertyField(miterLimitProp);
                    break;

                case UILineChart.JoinType.Bevel:
                    EditorGUILayout.PropertyField(miterLimitProp);
                    EditorGUILayout.PropertyField(bevelAngleThresholdProp);
                    break;

                case UILineChart.JoinType.Simple:
                default:
                    break;
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void OnSceneGUI()
        {
            if (lineChart == null || lineChart.points == null)
            {
                return;
            }

            RectTransform rectTransform = lineChart.rectTransform;
            Rect rect = rectTransform.rect;

            Handles.color = Color.cyan;

            for (int i = 0; i < lineChart.points.Count; i++)
            {
                Vector3 worldPos = GetWorldPos(lineChart.points[i], rectTransform, rect);
                float handleSize = HandleUtility.GetHandleSize(worldPos) * 0.1f;

                EditorGUI.BeginChangeCheck();

                Vector3 newWorldPos = Handles.FreeMoveHandle(
                    worldPos,
                    handleSize,
                    Vector3.zero,
                    Handles.DotHandleCap
                );

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(lineChart, "Move Chart Point");
                    lineChart.points[i] = GetNormalizedPos(newWorldPos, rectTransform, rect);
                    EditorUtility.SetDirty(lineChart);
                    lineChart.Refresh();
                }
            }

            // Plus buttons on end of line, both sides
            if (lineChart.points.Count > 0)
            {
                DrawAddButton(0, true, rectTransform, rect);
                DrawAddButton(lineChart.points.Count - 1, false, rectTransform, rect);
            }
        }

        private void DrawAddButton(int pointIndex, bool atStart, RectTransform rectTransform, Rect rect)
        {
            Vector2 normPoint = lineChart.points[pointIndex];
            Vector3 worldPos = GetWorldPos(normPoint, rectTransform, rect);
            float handleSize = HandleUtility.GetHandleSize(worldPos);

            Vector3 offset = atStart ? new Vector3(-handleSize * 0.25f, 0, 0) : new Vector3(handleSize * 0.25f, 0, 0);
            Vector3 buttonPos = worldPos + offset;

            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 12,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };

            Handles.BeginGUI();
            Vector2 guiPoint = HandleUtility.WorldToGUIPoint(buttonPos);
            Rect buttonRect = new Rect(guiPoint.x - 10, guiPoint.y - 10, 20, 20);

            Event e = Event.current;

            if (buttonRect.Contains(e.mousePosition))
            {
                if (e.type == EventType.MouseDown && e.button == 0)
                {
                    Undo.RecordObject(lineChart, "Add Chart Point");

                    // new dots creating
                    Vector2 newNormPoint = GetNormalizedPos(buttonPos, rectTransform, rect);

                    if (atStart)
                    {
                        lineChart.points.Insert(0, newNormPoint);
                        draggedNewPointIndex = 0;
                    }
                    else
                    {
                        lineChart.points.Add(newNormPoint);
                        draggedNewPointIndex = lineChart.points.Count - 1;
                    }

                    isCreatingPoint = true;

                    EditorUtility.SetDirty(lineChart);
                    lineChart.Refresh();

                    e.Use();
                }
            }

            if (isCreatingPoint && draggedNewPointIndex >= 0 && draggedNewPointIndex < lineChart.points.Count)
            {
                if (e.type == EventType.MouseDrag)
                {
                    Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
                    Plane plane = new Plane(rectTransform.forward, rectTransform.position);

                    if (plane.Raycast(ray, out float enter))
                    {
                        Vector3 dragWorldPos = ray.GetPoint(enter);
                        lineChart.points[draggedNewPointIndex] = GetNormalizedPos(dragWorldPos, rectTransform, rect);
                        EditorUtility.SetDirty(lineChart);
                        lineChart.Refresh();
                    }

                    e.Use();
                }
                else if (e.type == EventType.MouseUp)
                {
                    isCreatingPoint = false;
                    draggedNewPointIndex = -1;
                    e.Use();
                }
            }

            GUI.Button(buttonRect, "+", buttonStyle);
            Handles.EndGUI();
        }

        private Vector3 GetWorldPos(Vector2 normPoint, RectTransform rectTransform, Rect rect)
        {
            float localX = rect.xMin + normPoint.x * rect.width;
            float localY = rect.yMin + normPoint.y * rect.height;
            return rectTransform.TransformPoint(new Vector3(localX, localY, 0f));
        }

        private Vector2 GetNormalizedPos(Vector3 worldPos, RectTransform rectTransform, Rect rect)
        {
            Vector3 localPos = rectTransform.InverseTransformPoint(worldPos);
            float newNormX = (localPos.x - rect.xMin) / rect.width;
            float newNormY = (localPos.y - rect.yMin) / rect.height;
            return new Vector2(newNormX, newNormY);
        }
    }
}
#endif