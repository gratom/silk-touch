#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Tools
{

    [InitializeOnLoad]
    public static class Debug3d
    {
        private struct DotData
        {
            public Vector3 position;
            public Color color;
            public string label;
        }

        private struct LineData
        {
            public Vector3 start;
            public Vector3 end;
            public Color color;
            public string label;
        }

        private static readonly List<DotData> dots = new List<DotData>();
        private static readonly List<LineData> lines = new List<LineData>();

        static Debug3d()
        {
            SceneView.duringSceneGui += OnSceneGUI;
        }

        public static void Dot(Vector3 point, Color? color = null, string label = null)
        {
            dots.Add(new DotData
            {
                position = point,
                color = color ?? Color.yellow,
                label = label
            });
            SceneView.RepaintAll();
        }

        public static void DebugOnScene(this Vector3 point, Color? color = null, string label = null)
        {
            Dot(point, color, label);
        }

        public static void Line(Vector3 start, Vector3 end, Color? color = null, string label = null)
        {
            lines.Add(new LineData
            {
                start = start,
                end = end,
                color = color ?? Color.cyan,
                label = label
            });
            SceneView.RepaintAll();
        }

        public static void Clear()
        {
            dots.Clear();
            lines.Clear();
            SceneView.RepaintAll();
        }

        private const float ANGULAR_SIZE = 0.013f;

        private static void OnSceneGUI(SceneView sceneView)
        {
            if (SceneView.currentDrawingSceneView?.camera == null)
            {
                return;
            }
            Camera cam = SceneView.currentDrawingSceneView.camera;

            Handles.BeginGUI();
            GUILayout.BeginArea(new Rect(10, 10, 120, 40));
            if ((dots != null && dots.Count > 0) || (lines != null && lines.Count > 0))
            {
                if (GUILayout.Button("Clear All"))
                {
                    Clear();
                }
            }
            GUILayout.EndArea();
            Handles.EndGUI();

            foreach (DotData dot in dots)
            {
                float distance = Vector3.Distance(cam.transform.position, dot.position);
                float size = distance * ANGULAR_SIZE;

                Handles.color = dot.color;
                Handles.SphereHandleCap(0, dot.position, Quaternion.identity, size, EventType.Repaint);

                if (!string.IsNullOrEmpty(dot.label))
                {
                    Handles.Label(dot.position + Vector3.up * size * 1.2f, dot.label);
                }
            }

            foreach (LineData line in lines)
            {
                Handles.color = line.color;
                Handles.DrawLine(line.start, line.end);

                if (!string.IsNullOrEmpty(line.label))
                {
                    Vector3 mid = (line.start + line.end) * 0.5f;
                    float distance = Vector3.Distance(cam.transform.position, mid);
                    float offset = distance * ANGULAR_SIZE * 1.5f;

                    Handles.Label(mid + Vector3.up * offset, line.label);
                }
            }
        }
    }

}
#endif