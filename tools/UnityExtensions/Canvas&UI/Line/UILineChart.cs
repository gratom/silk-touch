using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Tools
{
    [ExecuteAlways]
    [RequireComponent(typeof(CanvasRenderer))]
    public class UILineChart : Graphic
    {
        public enum JoinType
        {
            Simple,
            Miter,
            Bevel
        }

        public List<Vector2> points = new List<Vector2>();
        public float thickness = 5f;
        public JoinType joinType = JoinType.Bevel;
        public float miterLimit = 2f;
        public float bevelAngleThreshold = 60f;

        public void Refresh()
        {
            SetVerticesDirty();
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();

            if (points == null || points.Count < 2)
            {
                return;
            }

            Rect rect = rectTransform.rect;

            switch (joinType)
            {
                case JoinType.Simple:
                    BuildSimpleMesh(vh, rect);
                    break;
                case JoinType.Miter:
                    BuildMiterMesh(vh, rect);
                    break;
                case JoinType.Bevel:
                    BuildBevelMesh(vh, rect);
                    break;
            }
        }

        private void BuildSimpleMesh(VertexHelper vh, Rect rect)
        {
            float halfThickness = thickness / 2f;

            for (int i = 0; i < points.Count - 1; i++)
            {
                Vector2 start = NormalizeToPoint(points[i], rect);
                Vector2 end = NormalizeToPoint(points[i + 1], rect);

                Vector2 dir = (end - start).normalized;
                if (dir == Vector2.zero)
                {
                    continue;
                }

                Vector2 normal = new Vector2(-dir.y, dir.x) * halfThickness;

                AddQuad(vh, start - normal, start + normal, end + normal, end - normal);
            }
        }

        private void BuildMiterMesh(VertexHelper vh, Rect rect)
        {
            float halfThickness = thickness / 2f;

            for (int i = 0; i < points.Count; i++)
            {
                Vector2 current = NormalizeToPoint(points[i], rect);
                Vector2 normal;
                float scale = 1f;

                if (i == 0)
                {
                    Vector2 dir = (NormalizeToPoint(points[1], rect) - current).normalized;
                    normal = new Vector2(-dir.y, dir.x);
                }
                else if (i == points.Count - 1)
                {
                    Vector2 dir = (current - NormalizeToPoint(points[i - 1], rect)).normalized;
                    normal = new Vector2(-dir.y, dir.x);
                }
                else
                {
                    Vector2 dir1 = (current - NormalizeToPoint(points[i - 1], rect)).normalized;
                    Vector2 dir2 = (NormalizeToPoint(points[i + 1], rect) - current).normalized;

                    Vector2 tangent = (dir1 + dir2).normalized;
                    normal = new Vector2(-tangent.y, tangent.x);

                    Vector2 miterNormal = new Vector2(-dir1.y, dir1.x);
                    float dot = Vector2.Dot(normal, miterNormal);
                    if (Mathf.Abs(dot) > 0.001f)
                    {
                        scale = Mathf.Min(1f / dot, miterLimit);
                    }
                }

                UIVertex v1 = UIVertex.simpleVert;
                v1.color = color;
                v1.position = current - normal * (halfThickness * scale);

                UIVertex v2 = UIVertex.simpleVert;
                v2.color = color;
                v2.position = current + normal * (halfThickness * scale);

                vh.AddVert(v1);
                vh.AddVert(v2);
            }

            for (int i = 0; i < points.Count - 1; i++)
            {
                int index = i * 2;
                vh.AddTriangle(index, index + 1, index + 3);
                vh.AddTriangle(index + 3, index + 2, index);
            }
        }

        private void BuildBevelMesh(VertexHelper vh, Rect rect)
        {
            for (int i = 0; i < points.Count - 1; i++)
            {
                Vector2 start = NormalizeToPoint(points[i], rect);
                Vector2 end = NormalizeToPoint(points[i + 1], rect);

                Vector2 dirCurrent = (end - start).normalized;
                if (dirCurrent == Vector2.zero)
                {
                    continue;
                }

                Vector2 startNormal = GetSegmentNormal(i, dirCurrent, rect, true);
                Vector2 endNormal = GetSegmentNormal(i + 1, dirCurrent, rect, false);

                AddQuad(vh, start - startNormal, start + startNormal, end + endNormal, end - endNormal);
            }

            for (int i = 1; i < points.Count - 1; i++)
            {
                Vector2 prevStart = NormalizeToPoint(points[i - 1], rect);
                Vector2 currentPos = NormalizeToPoint(points[i], rect);
                Vector2 nextEnd = NormalizeToPoint(points[i + 1], rect);

                Vector2 dir1 = (currentPos - prevStart).normalized;
                Vector2 dir2 = (nextEnd - currentPos).normalized;

                float angle = Vector2.Angle(dir1, dir2);

                if (angle >= bevelAngleThreshold)
                {
                    int seg1EndIdx = (i - 1) * 4;
                    int seg2StartIdx = i * 4;

                    float cross = dir1.x * dir2.y - dir1.y * dir2.x;

                    if (cross > 0)
                    {
                        vh.AddTriangle(seg1EndIdx + 3, seg2StartIdx + 0, seg1EndIdx + 2);
                    }
                    else
                    {
                        vh.AddTriangle(seg1EndIdx + 2, seg2StartIdx + 1, seg1EndIdx + 3);
                    }
                }
            }
        }

        private Vector2 GetSegmentNormal(int index, Vector2 currentDir, Rect rect, bool isStartOfSegment)
        {
            float halfThickness = thickness / 2f;

            if (index == 0 || index == points.Count - 1)
            {
                return new Vector2(-currentDir.y, currentDir.x) * halfThickness;
            }

            Vector2 prevPos = NormalizeToPoint(points[index - 1], rect);
            Vector2 currentPos = NormalizeToPoint(points[index], rect);
            Vector2 nextPos = NormalizeToPoint(points[index + 1], rect);

            Vector2 dir1 = (currentPos - prevPos).normalized;
            Vector2 dir2 = (nextPos - currentPos).normalized;

            float angle = Vector2.Angle(dir1, dir2);

            if (angle >= bevelAngleThreshold)
            {
                return new Vector2(-currentDir.y, currentDir.x) * halfThickness;
            }

            Vector2 tangent = (dir1 + dir2).normalized;
            Vector2 normal = new Vector2(-tangent.y, tangent.x);
            Vector2 miterNormal = new Vector2(-dir1.y, dir1.x);

            float scale = 1f;
            float dot = Vector2.Dot(normal, miterNormal);
            if (Mathf.Abs(dot) > 0.001f)
            {
                scale = Mathf.Min(1f / dot, miterLimit);
            }

            return normal * (halfThickness * scale);
        }

        private void AddQuad(VertexHelper vh, Vector2 v0, Vector2 v1, Vector2 v2, Vector2 v3)
        {
            int baseIdx = vh.currentVertCount;

            UIVertex v = UIVertex.simpleVert;
            v.color = color;

            v.position = v0;
            vh.AddVert(v);
            v.position = v1;
            vh.AddVert(v);
            v.position = v2;
            vh.AddVert(v);
            v.position = v3;
            vh.AddVert(v);

            vh.AddTriangle(baseIdx + 0, baseIdx + 1, baseIdx + 2);
            vh.AddTriangle(baseIdx + 2, baseIdx + 3, baseIdx + 0);
        }

        private Vector2 NormalizeToPoint(Vector2 normalizedPoint, Rect rect)
        {
            float x = rect.xMin + normalizedPoint.x * rect.width;
            float y = rect.yMin + normalizedPoint.y * rect.height;
            return new Vector2(x, y);
        }
    }
}