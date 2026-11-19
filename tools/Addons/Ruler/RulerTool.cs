#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace SilkTouch.Tools.Ruler
{
    [InitializeOnLoad]
    public static class RulerTool
    {
        public const string AlwaysVisibleTag = "alwaysVisible";
        private const string EnabledPrefKey = "SilkTouch.Ruler.Enabled";
        private const string ToggleMenuPath = "Tools/Editors/Toggle Ruler Tool";

        private static readonly Color[] axisColors =
        {
            new Color(0.93f, 0.25f, 0.25f),
            new Color(0.35f, 0.9f, 0.3f),
            new Color(0.3f, 0.55f, 1f)
        };

        private static readonly Vector3[] axisDirs = { Vector3.right, Vector3.up, Vector3.forward };

        private static readonly HashSet<GameObject> processed = new HashSet<GameObject>();

        private const string GridTopTag = "gridTop";
        private const string GridBottomTag = "gridBottom";
        private const string GridLeftTag = "gridLeft";
        private const string GridRightTag = "gridRight";
        private const string GridFrontTag = "gridFront";
        private const string GridBackTag = "gridBack";

        private struct GridSide
        {
            public string tag;
            public int axis;
            public float sign;
        }

        // axis: 0 = X, 1 = Y, 2 = Z. sign selects which face along that axis (max or min), matching
        // the Top/Bottom/Left/Right/Front/Back convention already used by BoundsVisualizer.
        private static readonly GridSide[] gridSides =
        {
            new GridSide { tag = GridTopTag,    axis = 1, sign =  1f },
            new GridSide { tag = GridBottomTag, axis = 1, sign = -1f },
            new GridSide { tag = GridLeftTag,   axis = 0, sign = -1f },
            new GridSide { tag = GridRightTag,  axis = 0, sign =  1f },
            new GridSide { tag = GridFrontTag,  axis = 2, sign =  1f },
            new GridSide { tag = GridBackTag,   axis = 2, sign = -1f }
        };

        private static readonly Color GridMajorColor = new Color(0.92f, 0.92f, 0.92f, 0.95f);
        private static readonly Color GridMinorColor = new Color(0.92f, 0.92f, 0.92f, 0.22f);
        private const int MaxGridLinesPerAxis = 400;

        // Free two-point measurement (Shift+R): point A is fixed once placed, point B tracks
        // the cursor live until any mouse click ends the measurement.
        private static bool measuring;
        private static Vector3 pointA;
        private static readonly Color MeasureColor = new Color(1f, 0.85f, 0.2f);

        private static GUIStyle labelStyle;
        private static Texture2D labelBackgroundTexture;

        // Plain white-on-dark backing so measurement numbers stay readable over any
        // scene background, light or dark, instead of relying on the default label style.
        private static GUIStyle LabelStyle
        {
            get
            {
                if (labelStyle == null || labelBackgroundTexture == null)
                {
                    labelBackgroundTexture = new Texture2D(1, 1, TextureFormat.RGBA32, false)
                    {
                        hideFlags = HideFlags.HideAndDontSave
                    };
                    labelBackgroundTexture.SetPixel(0, 0, new Color(0f, 0f, 0f, 0.6f));
                    labelBackgroundTexture.Apply();

                    labelStyle = new GUIStyle(EditorStyles.label)
                    {
                        alignment = TextAnchor.MiddleCenter,
                        fontStyle = FontStyle.Bold,
                        padding = new RectOffset(3, 3, 1, 1)
                    };
                    labelStyle.normal.textColor = Color.white;
                    labelStyle.normal.background = labelBackgroundTexture;
                }
                return labelStyle;
            }
        }

        public static bool Enabled
        {
            get => EditorPrefs.GetBool(EnabledPrefKey, true);
            set => EditorPrefs.SetBool(EnabledPrefKey, value);
        }

        static RulerTool()
        {
            EnsureTagExists();
            SceneView.duringSceneGui += OnSceneGUI;
        }

        private static void EnsureTagExists()
        {
            EnsureTag(AlwaysVisibleTag);
            for (int i = 0; i < gridSides.Length; i++)
            {
                EnsureTag(gridSides[i].tag);
            }
        }

        private static void EnsureTag(string tagName)
        {
            string[] tags = InternalEditorUtility.tags;
            for (int i = 0; i < tags.Length; i++)
            {
                if (tags[i] == tagName)
                {
                    return;
                }
            }
            InternalEditorUtility.AddTag(tagName);
        }

        #region menu

        [MenuItem("GameObject/Ruler/Mark Always Visible", false, 0)]
        private static void MarkAlwaysVisible()
        {
            GameObject go = Selection.activeGameObject;
            if (go == null)
            {
                return;
            }
            EnsureTagExists();
            Undo.RecordObject(go, "Mark Always Visible");
            go.tag = AlwaysVisibleTag;
            EditorUtility.SetDirty(go);
            SceneView.RepaintAll();
        }

        [MenuItem("GameObject/Ruler/Mark Always Visible", true)]
        private static bool ValidateMarkAlwaysVisible()
        {
            GameObject go = Selection.activeGameObject;
            return go != null && !go.CompareTag(AlwaysVisibleTag);
        }

        [MenuItem("GameObject/Ruler/Unmark Always Visible", false, 0)]
        private static void UnmarkAlwaysVisible()
        {
            GameObject go = Selection.activeGameObject;
            if (go == null)
            {
                return;
            }
            Undo.RecordObject(go, "Unmark Always Visible");
            go.tag = "Untagged";
            EditorUtility.SetDirty(go);
            SceneView.RepaintAll();
        }

        [MenuItem("GameObject/Ruler/Unmark Always Visible", true)]
        private static bool ValidateUnmarkAlwaysVisible()
        {
            GameObject go = Selection.activeGameObject;
            return go != null && go.CompareTag(AlwaysVisibleTag);
        }

        private static void ToggleGridTag(string tagName)
        {
            GameObject go = Selection.activeGameObject;
            if (go == null)
            {
                return;
            }
            EnsureTagExists();
            Undo.RecordObject(go, "Toggle Grid Tag");
            go.tag = go.CompareTag(tagName) ? "Untagged" : tagName;
            EditorUtility.SetDirty(go);
            SceneView.RepaintAll();
        }

        private static bool ValidateGridMenu()
        {
            return Selection.activeGameObject != null;
        }

        [MenuItem("GameObject/Ruler/Grid/Top", false, 10)]
        private static void MarkGridTop() => ToggleGridTag(GridTopTag);
        [MenuItem("GameObject/Ruler/Grid/Top", true)]
        private static bool ValidateMarkGridTop() => ValidateGridMenu();

        [MenuItem("GameObject/Ruler/Grid/Bottom", false, 10)]
        private static void MarkGridBottom() => ToggleGridTag(GridBottomTag);
        [MenuItem("GameObject/Ruler/Grid/Bottom", true)]
        private static bool ValidateMarkGridBottom() => ValidateGridMenu();

        [MenuItem("GameObject/Ruler/Grid/Left", false, 10)]
        private static void MarkGridLeft() => ToggleGridTag(GridLeftTag);
        [MenuItem("GameObject/Ruler/Grid/Left", true)]
        private static bool ValidateMarkGridLeft() => ValidateGridMenu();

        [MenuItem("GameObject/Ruler/Grid/Right", false, 10)]
        private static void MarkGridRight() => ToggleGridTag(GridRightTag);
        [MenuItem("GameObject/Ruler/Grid/Right", true)]
        private static bool ValidateMarkGridRight() => ValidateGridMenu();

        [MenuItem("GameObject/Ruler/Grid/Front", false, 10)]
        private static void MarkGridFront() => ToggleGridTag(GridFrontTag);
        [MenuItem("GameObject/Ruler/Grid/Front", true)]
        private static bool ValidateMarkGridFront() => ValidateGridMenu();

        [MenuItem("GameObject/Ruler/Grid/Back", false, 10)]
        private static void MarkGridBack() => ToggleGridTag(GridBackTag);
        [MenuItem("GameObject/Ruler/Grid/Back", true)]
        private static bool ValidateMarkGridBack() => ValidateGridMenu();

        [MenuItem(ToggleMenuPath)]
        private static void ToggleEnabledMenu()
        {
            Enabled = !Enabled;
            SceneView.RepaintAll();
        }

        [MenuItem(ToggleMenuPath, true)]
        private static bool ValidateToggleEnabledMenu()
        {
            Menu.SetChecked(ToggleMenuPath, Enabled);
            return true;
        }

        #endregion

        private static void OnSceneGUI(SceneView sceneView)
        {
            EnsureTagExists();

            if (!Enabled)
            {
                return;
            }

            HandleFreeMeasurementInput(sceneView);

            processed.Clear();

            foreach (GameObject go in Selection.gameObjects)
            {
                if (IsMeasurable(go) && processed.Add(go))
                {
                    DrawRulerFor(go);
                }
            }

            GameObject[] tagged = null;
            try
            {
                tagged = GameObject.FindGameObjectsWithTag(AlwaysVisibleTag);
            }
            catch (UnityException)
            {
                tagged = null;
            }

            if (tagged != null)
            {
                foreach (GameObject go in tagged)
                {
                    if (IsMeasurable(go) && processed.Add(go))
                    {
                        DrawRulerFor(go);
                    }
                }
            }

            DrawAllGrids();
            DrawFreeMeasurement(sceneView);
        }

        #region free measurement (shift+r)

        private static void HandleFreeMeasurementInput(SceneView sceneView)
        {
            Event e = Event.current;

            if (e.type == EventType.KeyDown && e.keyCode == KeyCode.R && e.shift)
            {
                // No anchor fallback for the first point: if nothing is hit and the camera
                // faces away from the origin planes, we simply ignore the key press.
                if (TryPickPoint(sceneView.camera, e.mousePosition, null, out Vector3 point))
                {
                    pointA = point;
                    measuring = true;
                    sceneView.Repaint();
                }
                e.Use();
            }
            else if (measuring && e.type == EventType.MouseDown && (e.button == 0 || e.button == 1))
            {
                // Any click ends the measurement; the click itself still reaches normal
                // scene input (object selection, camera-look drag start) unaffected.
                measuring = false;
                sceneView.Repaint();
            }
        }

        private static void DrawFreeMeasurement(SceneView sceneView)
        {
            if (!measuring)
            {
                return;
            }

            Camera cam = sceneView.camera;
            bool hasB = TryPickPoint(cam, Event.current.mousePosition, pointA, out Vector3 pointB);

            Handles.color = MeasureColor;
            float markerSizeA = HandleUtility.GetHandleSize(pointA) * 0.08f;
            Handles.SphereHandleCap(0, pointA, Quaternion.identity, markerSizeA, Event.current.type);

            if (hasB)
            {
                Handles.DrawLine(pointA, pointB);

                float markerSizeB = HandleUtility.GetHandleSize(pointB) * 0.08f;
                Handles.SphereHandleCap(0, pointB, Quaternion.identity, markerSizeB, Event.current.type);

                float length = Vector3.Distance(pointA, pointB);
                if (length > 0.0001f)
                {
                    Vector3 dir = (pointB - pointA) / length;
                    Vector3 camForward = cam != null ? cam.transform.forward : Vector3.forward;
                    Vector3 tickDir = Vector3.Cross(dir, camForward);
                    if (tickDir.sqrMagnitude < 0.0001f)
                    {
                        tickDir = Vector3.Cross(dir, Vector3.up);
                    }
                    if (tickDir.sqrMagnitude < 0.0001f)
                    {
                        tickDir = Vector3.Cross(dir, Vector3.right);
                    }
                    tickDir.Normalize();

                    DrawTicks(pointA, pointB, length, tickDir);

                    float handleSize = HandleUtility.GetHandleSize(pointB);
                    float labelOffset = handleSize * (DragHandleSizeMultiplier + 0.6f);
                    Handles.Label(pointB + Vector3.up * labelOffset, length.ToString("0.00") + "m", LabelStyle);
                }
            }

            // Keep repainting while measuring so point B tracks the mouse without needing a drag.
            sceneView.Repaint();
        }

        // Point pick order: (1) RaySnap - the same surface pick Unity's own snap-to-surface
        // (V / Ctrl-drag) uses, hits any renderer with no collider required; (2) a plain physics
        // raycast against colliders, covering collider-only objects; (3) an origin-centered axis
        // plane chosen to face the ray most head-on; (4) for the second point only, a plane
        // through the anchor facing the camera, which is guaranteed to intersect.
        private static bool TryPickPoint(Camera cam, Vector2 guiPosition, Vector3? anchorFallback, out Vector3 worldPoint)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(guiPosition);

            object snapHit = HandleUtility.RaySnap(ray);
            if (snapHit is RaycastHit hit)
            {
                worldPoint = hit.point;
                return true;
            }

            if (Physics.Raycast(ray, out RaycastHit physicsHit))
            {
                worldPoint = physicsHit.point;
                return true;
            }

            if (TryIntersectZeroPlane(ray, out worldPoint))
            {
                return true;
            }

            if (anchorFallback.HasValue && TryIntersectCameraFacingPlane(ray, cam, anchorFallback.Value, out worldPoint))
            {
                return true;
            }

            worldPoint = default;
            return false;
        }

        private static bool TryIntersectZeroPlane(Ray ray, out Vector3 point)
        {
            Vector3 dir = ray.direction;
            float ax = Mathf.Abs(dir.x);
            float ay = Mathf.Abs(dir.y);
            float az = Mathf.Abs(dir.z);

            Vector3 normal = (ay >= ax && ay >= az) ? Vector3.up : (ax >= az ? Vector3.right : Vector3.forward);

            return TryIntersectPlane(ray, normal, Vector3.zero, out point);
        }

        private static bool TryIntersectCameraFacingPlane(Ray ray, Camera cam, Vector3 anchor, out Vector3 point)
        {
            Vector3 normal = cam != null ? cam.transform.forward : ray.direction;
            return TryIntersectPlane(ray, normal, anchor, out point);
        }

        private static bool TryIntersectPlane(Ray ray, Vector3 normal, Vector3 pointOnPlane, out Vector3 point)
        {
            float denom = Vector3.Dot(ray.direction, normal);
            if (Mathf.Abs(denom) > 1e-6f)
            {
                float t = Vector3.Dot(pointOnPlane - ray.origin, normal) / denom;
                if (t >= 0f)
                {
                    point = ray.origin + ray.direction * t;
                    return true;
                }
            }
            point = default;
            return false;
        }

        #endregion

        private static void DrawAllGrids()
        {
            for (int i = 0; i < gridSides.Length; i++)
            {
                GridSide side = gridSides[i];
                GameObject[] sideTagged;
                try
                {
                    sideTagged = GameObject.FindGameObjectsWithTag(side.tag);
                }
                catch (UnityException)
                {
                    sideTagged = null;
                }

                if (sideTagged == null)
                {
                    continue;
                }

                foreach (GameObject go in sideTagged)
                {
                    if (go != null)
                    {
                        DrawGridFor(go, side);
                    }
                }
            }
        }

        private static bool IsMeasurable(GameObject go)
        {
            if (go == null)
            {
                return false;
            }
            return TryGetLocalBounds(go, out _);
        }

        // Falls back from the render mesh to whatever collider shape is present, so the ruler
        // also works on colliders-only objects (no MeshFilter/MeshRenderer at all).
        private static bool TryGetLocalBounds(GameObject go, out Bounds bounds)
        {
            MeshFilter meshFilter = go.GetComponent<MeshFilter>();
            if (meshFilter != null && meshFilter.sharedMesh != null)
            {
                bounds = meshFilter.sharedMesh.bounds;
                return true;
            }

            BoxCollider box = go.GetComponent<BoxCollider>();
            if (box != null)
            {
                bounds = new Bounds(box.center, box.size);
                return true;
            }

            SphereCollider sphere = go.GetComponent<SphereCollider>();
            if (sphere != null)
            {
                float diameter = sphere.radius * 2f;
                bounds = new Bounds(sphere.center, new Vector3(diameter, diameter, diameter));
                return true;
            }

            CapsuleCollider capsule = go.GetComponent<CapsuleCollider>();
            if (capsule != null)
            {
                float diameter = capsule.radius * 2f;
                float height = Mathf.Max(capsule.height, diameter);
                Vector3 size = new Vector3(diameter, diameter, diameter);
                SetComponent(ref size, capsule.direction, height);
                bounds = new Bounds(capsule.center, size);
                return true;
            }

            MeshCollider meshCollider = go.GetComponent<MeshCollider>();
            if (meshCollider != null && meshCollider.sharedMesh != null)
            {
                bounds = meshCollider.sharedMesh.bounds;
                return true;
            }

            bounds = default;
            return false;
        }

        private static void DrawGridFor(GameObject go, GridSide side)
        {
            if (!TryGetLocalBounds(go, out Bounds bounds))
            {
                return;
            }

            Transform t = go.transform;

            int axis = side.axis;
            int u = (axis + 1) % 3;
            int v = (axis + 2) % 3;

            float facePos = side.sign > 0f ? GetComponent(bounds.max, axis) : GetComponent(bounds.min, axis);
            float uMin = GetComponent(bounds.min, u);
            float uMax = GetComponent(bounds.max, u);
            float vMin = GetComponent(bounds.min, v);
            float vMax = GetComponent(bounds.max, v);

            float scaleU = Mathf.Abs(GetComponent(t.lossyScale, u));
            float scaleV = Mathf.Abs(GetComponent(t.lossyScale, v));
            if (scaleU < 0.0001f || scaleV < 0.0001f)
            {
                return;
            }

            // Grid spacing is derived from world-space meters (1m major / 0.1m minor),
            // converted into local units per axis so it stays accurate under non-uniform scale.
            float deciStepU = 0.1f / scaleU;
            float deciStepV = 0.1f / scaleV;

            int uLines = Mathf.FloorToInt((uMax - uMin) / deciStepU + 0.0001f);
            int vLines = Mathf.FloorToInt((vMax - vMin) / deciStepV + 0.0001f);

            bool drawMinorU = uLines <= MaxGridLinesPerAxis;
            bool drawMinorV = vLines <= MaxGridLinesPerAxis;

            for (int i = 0; i <= uLines; i++)
            {
                bool isMajor = i % 10 == 0;
                if (!isMajor && !drawMinorU)
                {
                    continue;
                }
                float uVal = uMin + i * deciStepU;
                if (uVal > uMax + 0.0001f)
                {
                    break;
                }
                Vector3 a = BuildFacePoint(axis, u, v, facePos, uVal, vMin);
                Vector3 b = BuildFacePoint(axis, u, v, facePos, uVal, vMax);
                Handles.color = isMajor ? GridMajorColor : GridMinorColor;
                Handles.DrawLine(t.TransformPoint(a), t.TransformPoint(b));
            }

            for (int j = 0; j <= vLines; j++)
            {
                bool isMajor = j % 10 == 0;
                if (!isMajor && !drawMinorV)
                {
                    continue;
                }
                float vVal = vMin + j * deciStepV;
                if (vVal > vMax + 0.0001f)
                {
                    break;
                }
                Vector3 a = BuildFacePoint(axis, u, v, facePos, uMin, vVal);
                Vector3 b = BuildFacePoint(axis, u, v, facePos, uMax, vVal);
                Handles.color = isMajor ? GridMajorColor : GridMinorColor;
                Handles.DrawLine(t.TransformPoint(a), t.TransformPoint(b));
            }
        }

        private static Vector3 BuildFacePoint(int axis, int u, int v, float facePos, float uVal, float vVal)
        {
            Vector3 p = Vector3.zero;
            SetComponent(ref p, axis, facePos);
            SetComponent(ref p, u, uVal);
            SetComponent(ref p, v, vVal);
            return p;
        }

        private static void DrawRulerFor(GameObject go)
        {
            if (!TryGetLocalBounds(go, out Bounds localBounds))
            {
                return;
            }

            Transform t = go.transform;
            Vector3 cornerLocal = localBounds.min;

            for (int axis = 0; axis < 3; axis++)
            {
                DrawAxis(t, localBounds, axis, cornerLocal);
            }
        }

        private static void DrawAxis(Transform t, Bounds localBounds, int axis, Vector3 cornerLocal)
        {
            // Lines run along the box's edges (from one shared corner), not through the center,
            // so the measurement sits on the object's boundary instead of cutting through it.
            Vector3 farLocal = cornerLocal;
            SetComponent(ref farLocal, axis, GetComponent(localBounds.max, axis));

            Vector3 worldMin = t.TransformPoint(cornerLocal);
            Vector3 worldMax = t.TransformPoint(farLocal);

            float length = Vector3.Distance(worldMin, worldMax);
            Color color = axisColors[axis];

            Handles.color = color;
            Handles.DrawLine(worldMin, worldMax);

            if (length > 0.0001f)
            {
                Vector3 worldAxisDir = (worldMax - worldMin) / length;
                Camera cam = SceneView.currentDrawingSceneView != null ? SceneView.currentDrawingSceneView.camera : null;
                Vector3 camForward = cam != null ? cam.transform.forward : Vector3.forward;

                Vector3 tickDir = Vector3.Cross(worldAxisDir, camForward);
                if (tickDir.sqrMagnitude < 0.0001f)
                {
                    tickDir = Vector3.Cross(worldAxisDir, Vector3.up);
                }
                if (tickDir.sqrMagnitude < 0.0001f)
                {
                    tickDir = Vector3.Cross(worldAxisDir, Vector3.right);
                }
                tickDir.Normalize();

                DrawTicks(worldMin, worldMax, length, tickDir);

                float handleSize = HandleUtility.GetHandleSize(worldMax);
                float labelOffset = handleSize * (DragHandleSizeMultiplier + 0.6f);
                Handles.Label(worldMax + Vector3.up * labelOffset, length.ToString("0.00") + "m", LabelStyle);
            }

            DrawDragHandle(t, localBounds, axis, worldMin, worldMax, true, color);
            DrawDragHandle(t, localBounds, axis, worldMin, worldMax, false, color);
        }

        private static void DrawTicks(Vector3 worldMin, Vector3 worldMax, float length, Vector3 tickDir)
        {
            int totalTenths = Mathf.FloorToInt(length * 10f + 0.001f);
            for (int j = 1; j <= totalTenths; j++)
            {
                float dist = j * 0.1f;
                if (dist > length)
                {
                    break;
                }

                Vector3 p = Vector3.Lerp(worldMin, worldMax, dist / length);
                float handleSize = HandleUtility.GetHandleSize(p);

                bool isMeter = j % 10 == 0;
                bool isHalfMeter = !isMeter && j % 5 == 0;
                float tickSize = handleSize * (isMeter ? 0.18f : isHalfMeter ? 0.11f : 0.06f);

                Handles.DrawLine(p - tickDir * tickSize, p + tickDir * tickSize);

                if (isMeter)
                {
                    Handles.Label(p + tickDir * tickSize * 1.3f, Mathf.RoundToInt(dist).ToString(), LabelStyle);
                }
            }
        }

        private const float DragHandleSizeMultiplier = 0.45f;

        private static void DrawDragHandle(Transform t, Bounds localBounds, int axis, Vector3 worldMin, Vector3 worldMax, bool isMaxEnd, Color color)
        {
            Vector3 point = isMaxEnd ? worldMax : worldMin;
            Vector3 lineDir = worldMax - worldMin;
            if (lineDir.sqrMagnitude < 0.0001f)
            {
                lineDir = axisDirs[axis];
            }
            Vector3 outward = isMaxEnd ? lineDir.normalized : -lineDir.normalized;

            float size = HandleUtility.GetHandleSize(point) * DragHandleSizeMultiplier;

            Handles.color = color;
            EditorGUI.BeginChangeCheck();
            Vector3 newPoint = Handles.Slider(point, outward, size, Handles.ArrowHandleCap, 0f);
            if (EditorGUI.EndChangeCheck())
            {
                ApplyResize(t, localBounds, axis, isMaxEnd, point, newPoint);
            }
        }

        private static void ApplyResize(Transform t, Bounds localBounds, int axis, bool isMaxEnd, Vector3 oldWorldPoint, Vector3 newWorldPoint)
        {
            Vector3 worldDelta = newWorldPoint - oldWorldPoint;
            Vector3 localDelta = t.InverseTransformVector(worldDelta);
            float deltaAlongAxis = GetComponent(localDelta, axis);

            float oldExtent = GetComponent(localBounds.extents, axis);
            float oldLength = oldExtent * 2f;
            if (Mathf.Approximately(oldLength, 0f))
            {
                return;
            }

            float newLength = isMaxEnd ? oldLength + deltaAlongAxis : oldLength - deltaAlongAxis;
            newLength = Mathf.Max(newLength, 0.001f);

            float oldScale = GetComponent(t.localScale, axis);
            float newScale = oldScale * (newLength / oldLength);

            Vector3 axisDir = axisDirs[axis];
            Vector3 center = localBounds.center;
            Vector3 anchorLocalPoint = isMaxEnd
                ? center - axisDir * oldExtent
                : center + axisDir * oldExtent;

            Vector3 anchorWorldBefore = t.TransformPoint(anchorLocalPoint);

            Undo.RecordObject(t, "Resize via Ruler");

            Vector3 newLocalScale = t.localScale;
            SetComponent(ref newLocalScale, axis, newScale);
            t.localScale = newLocalScale;

            Vector3 anchorWorldAfter = t.TransformPoint(anchorLocalPoint);
            t.position += anchorWorldBefore - anchorWorldAfter;

            EditorUtility.SetDirty(t);
        }

        private static float GetComponent(Vector3 v, int axis)
        {
            switch (axis)
            {
                case 0: return v.x;
                case 1: return v.y;
                default: return v.z;
            }
        }

        private static void SetComponent(ref Vector3 v, int axis, float value)
        {
            switch (axis)
            {
                case 0: v.x = value; break;
                case 1: v.y = value; break;
                default: v.z = value; break;
            }
        }
    }
}

#endif
