using System;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace Tools
{
    public static class TransformTool
    {
        #region set position

        public static Transform SetPosition(this Transform transform, Vector3 Pos)
        {
            transform.position = Pos;
            return transform;
        }

        public static Transform SetPositionX(this Transform transform, float XPos)
        {
            transform.position = new Vector3(XPos, transform.position.y, transform.position.z);
            return transform;
        }

        public static Transform SetPositionY(this Transform transform, float YPos)
        {
            transform.position = new Vector3(transform.position.x, YPos, transform.position.z);
            return transform;
        }

        public static Transform SetPositionZ(this Transform transform, float ZPos)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, ZPos);
            return transform;
        }

        #endregion set position

        #region offset position

        public static Transform OffsetPosition(this Transform transform, Vector3 Pos)
        {
            transform.position = transform.position + Pos;
            return transform;
        }

        public static Transform OffsetPositionX(this Transform transform, float XPos)
        {
            transform.position = new Vector3(transform.position.x + XPos, transform.position.y, transform.position.z);
            return transform;
        }

        public static Transform OffsetPositionY(this Transform transform, float YPos)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + YPos, transform.position.z);
            return transform;
        }

        public static Transform OffsetPositionZ(this Transform transform, float ZPos)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + ZPos);
            return transform;
        }

        #endregion offset position

        #region set local position

        public static Transform SetLocalPosition(this Transform transform, Vector3 Pos)
        {
            transform.localPosition = Pos;
            return transform;
        }

        public static Transform SetLocalPositionX(this Transform transform, float XPos)
        {
            transform.localPosition = new Vector3(XPos, transform.localPosition.y, transform.localPosition.z);
            return transform;
        }

        public static Transform SetLocalPositionY(this Transform transform, float YPos)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, YPos, transform.localPosition.z);
            return transform;
        }

        public static Transform SetLocalPositionZ(this Transform transform, float ZPos)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, ZPos);
            return transform;
        }

        #endregion set local position

        #region offset local position

        public static Transform OffsetLocalPosition(this Transform transform, Vector3 Pos)
        {
            transform.localPosition = transform.localPosition + Pos;
            return transform;
        }

        public static Transform OffsetLocalPositionX(this Transform transform, float XPos)
        {
            transform.localPosition = new Vector3(transform.localPosition.x + XPos, transform.localPosition.y, transform.localPosition.z);
            return transform;
        }

        public static Transform OffsetLocalPositionY(this Transform transform, float YPos)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + YPos, transform.localPosition.z);
            return transform;
        }

        public static Transform OffsetLocalPositionZ(this Transform transform, float ZPos)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z + ZPos);
            return transform;
        }

        #endregion offset local position

        #region truncate position

        public static Transform TruncatePositionMax(this Transform transform, Vector3 Pos)
        {
            Vector3 v = transform.position;
            v.x = v.x > Pos.x ? Pos.x : v.x;
            v.y = v.y > Pos.y ? Pos.y : v.y;
            v.z = v.z > Pos.z ? Pos.z : v.z;
            transform.position = v;
            return transform;
        }

        public static bool TryTruncatePositionMax(this Transform transform, Vector3 pos)
        {
            bool isTruncated = false;
            Vector3 v = transform.position;

            if (v.x > pos.x)
            {
                v.x = pos.x;
                isTruncated = true;
            }
            if (v.y > pos.y)
            {
                v.y = pos.y;
                isTruncated = true;
            }
            if (v.z > pos.z)
            {
                v.z = pos.z;
                isTruncated = true;
            }

            transform.position = v;
            return isTruncated;
        }

        public static Transform TruncatePositionMin(this Transform transform, Vector3 pos)
        {
            Vector3 v = transform.position;
            v.x = v.x < pos.x ? pos.x : v.x;
            v.y = v.y < pos.y ? pos.y : v.y;
            v.z = v.z < pos.z ? pos.z : v.z;
            transform.position = v;
            return transform;
        }

        public static bool TryTruncatePositionMin(this Transform transform, Vector3 pos)
        {
            bool isTruncated = false;
            Vector3 v = transform.position;

            if (v.x < pos.x)
            {
                v.x = pos.x;
                isTruncated = true;
            }
            if (v.y < pos.y)
            {
                v.y = pos.y;
                isTruncated = true;
            }
            if (v.z < pos.z)
            {
                v.z = pos.z;
                isTruncated = true;
            }

            transform.position = v;
            return isTruncated;
        }

        public static Transform TruncateLocalPositionMax(this Transform transform, Vector3 Pos)
        {
            Vector3 v = transform.localPosition;
            v.x = v.x > Pos.x ? Pos.x : v.x;
            v.y = v.y > Pos.y ? Pos.y : v.y;
            v.z = v.z > Pos.z ? Pos.z : v.z;
            transform.localPosition = v;
            return transform;
        }

        public static Transform TruncateLocalPositionMin(this Transform transform, Vector3 Pos)
        {
            Vector3 v = transform.localPosition;
            v.x = v.x < Pos.x ? Pos.x : v.x;
            v.y = v.y < Pos.y ? Pos.y : v.y;
            v.z = v.z < Pos.z ? Pos.z : v.z;
            transform.localPosition = v;
            return transform;
        }

        public static Transform TruncateByRadius(this Transform transform, Vector3 center, float radius)
        {
            if ((transform.position - center).magnitude > radius)
            {
                transform.position = center + (transform.position - center).normalized * radius;
            }

            return transform;
        }

        public static Transform TruncateByRadiusLocal(this Transform transform, float radius)
        {
            if (transform.localPosition.magnitude > radius)
            {
                transform.localPosition = transform.localPosition.normalized * radius;
            }

            return transform;
        }

        public static Transform SetPosOnRadius(this Transform transform, Vector3 center, float radius)
        {
            transform.position = center + (transform.position - center).normalized * radius;
            return transform;
        }

        public static Transform SetPosOnRadiusLocal(this Transform transform, float radius)
        {
            transform.localPosition = transform.localPosition.normalized * radius;
            return transform;
        }

        #endregion truncate position

        public static Transform SetRotation(this Transform transform, Vector3 Rot)
        {
            transform.rotation = Quaternion.Euler(Rot);
            return transform;
        }

        public static Transform SetRotationX(this Transform transform, float XRot)
        {
            transform.rotation = Quaternion.Euler(new Vector3(XRot, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z));
            return transform;
        }

        public static Transform SetRotationY(this Transform transform, float YRot)
        {
            transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.eulerAngles.x, YRot, transform.rotation.eulerAngles.z));
            return transform;
        }

        public static Transform SetRotationZ(this Transform transform, float ZRot)
        {
            transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, ZRot));
            return transform;
        }

        public static RectTransform CopyValuesFrom(this RectTransform transform, RectTransform other)
        {
            transform.anchorMin = other.anchorMin;
            transform.anchorMax = other.anchorMax;
            transform.anchoredPosition = other.anchoredPosition;
            transform.sizeDelta = other.sizeDelta;
            transform.pivot = other.pivot;
            return transform;
        }

        public static string GetHashPos(this Transform transform, string accuracy = "0.000")
        {
            Vector3 pos = transform.position;
            string basic = pos.x.ToString(accuracy) + pos.y.ToString(accuracy) + pos.z.ToString(accuracy);
            using SHA256 sha256 = SHA256.Create();
            byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(basic));
            return BitConverter.ToString(hashBytes, 0).Replace("-", string.Empty);
        }

        public static void SetActiveSafe(this GameObject g, bool state)
        {
            if (g.activeSelf != state)
            {
                g.SetActive(state);
            }
        }

        public static Vector3 GetWorldSize(this GameObject obj)
        {
            Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
            if (renderers.Length == 0)
            {
                return Vector3.zero;
            }

            Bounds bounds = renderers[0].bounds;
            for (int i = 1; i < renderers.Length; i++)
            {
                bounds.Encapsulate(renderers[i].bounds);
            }

            return bounds.size;
        }

        public static void SetLossyScale(this Transform transform, Vector3 desiredWorldScale)
        {
            if (transform.parent == null)
            {
                transform.localScale = desiredWorldScale;
            }
            else
            {
                Vector3 parentScale = transform.parent.lossyScale;
                transform.localScale = new Vector3(
                    desiredWorldScale.x / parentScale.x,
                    desiredWorldScale.y / parentScale.y,
                    desiredWorldScale.z / parentScale.z
                );
            }
        }

        #region geometric center

        public static Vector3 TopExtent(this Transform transform)
        {
            return GetExtent(transform, Vector3.up);
        }

        public static Vector3 BottomExtent(this Transform transform)
        {
            return GetExtent(transform, Vector3.down);
        }

        public static Vector3 LeftExtent(this Transform transform)
        {
            return GetExtent(transform, Vector3.left);
        }

        public static Vector3 RightExtent(this Transform transform)
        {
            return GetExtent(transform, Vector3.right);
        }

        public static Vector3 FrontExtent(this Transform transform)
        {
            return GetExtent(transform, Vector3.forward);
        }

        public static Vector3 BackExtent(this Transform transform)
        {
            return GetExtent(transform, Vector3.back);
        }

        #endregion

        #region pivot

        public static Vector3 TopExtentPivot(this Transform transform)
        {
            Vector3 v = GetExtent(transform, Vector3.up);
            return transform.position.WithY(v.y);
        }

        public static Vector3 BottomExtentPivot(this Transform transform)
        {
            Vector3 v = GetExtent(transform, Vector3.down);
            return transform.position.WithY(v.y);
        }

        public static Vector3 LeftExtentPivot(this Transform transform)
        {
            Vector3 v = GetExtent(transform, Vector3.left);
            return transform.position.WithX(v.x);
        }

        public static Vector3 RightExtentPivot(this Transform transform)
        {
            Vector3 v = GetExtent(transform, Vector3.right);
            return transform.position.WithX(v.x);
        }

        public static Vector3 FrontExtentPivot(this Transform transform)
        {
            Vector3 v = GetExtent(transform, Vector3.forward);
            return transform.position.WithZ(v.z);
        }

        public static Vector3 BackExtentPivot(this Transform transform)
        {
            Vector3 v = GetExtent(transform, Vector3.back);
            return transform.position.WithZ(v.z);
        }

        #endregion

        public static Vector3 GetExtent(this Transform transform, Vector3 localDir)
        {
            Renderer renderer = transform.GetComponent<Renderer>();
            return renderer == null ? Vector3.zero : WorldProjectToBounds(renderer.bounds, localDir);
        }

        private static Vector3 WorldProjectToBounds(Bounds bounds, Vector3 worldDir)
        {
            return bounds.center + Vector3.Scale(worldDir, bounds.extents);
        }

        public static Vector3 PivotToBoundsCenter(this Transform transform)
        {
            Renderer renderer = transform.GetComponent<Renderer>();
            return renderer == null ? Vector3.zero : renderer.bounds.center - transform.position;
        }

        public static Quaternion ToQuaternionR(this Plane plane, float radianAngle)
        {
            Vector3 axis = plane.normal.normalized;
            float degrees = radianAngle * Mathf.Rad2Deg;
            return Quaternion.AngleAxis(degrees, axis);
        }

        public static Quaternion ToQuaternion(this Plane plane, float angle)
        {
            Vector3 axis = plane.normal.normalized;
            return Quaternion.AngleAxis(angle, axis);
        }

        public static Plane ToPlane(this Quaternion q, out float angle)
        {
            if (q == Quaternion.identity)
            {
                angle = 0f;
                return new Plane(Vector3.up, 0f);
            }

            q.ToAngleAxis(out float angleDeg, out Vector3 axis);

            if (axis.sqrMagnitude < 1e-12f)
            {
                angle = 0f;
                return new Plane(Vector3.up, 0f);
            }

            axis = axis.normalized;

            if (angleDeg > 180f)
            {
                angleDeg = 360f - angleDeg;
                axis = -axis;
            }

            angle = angleDeg;
            return new Plane(axis, 0f);
        }

        public static Plane ToPlaneR(this Quaternion q, out float radianAngle)
        {
            if (q == Quaternion.identity)
            {
                radianAngle = 0f;
                return new Plane(Vector3.up, 0f);
            }

            q.ToAngleAxis(out float angleDeg, out Vector3 axis);

            if (axis.sqrMagnitude < 1e-12f)
            {
                radianAngle = 0f;
                return new Plane(Vector3.up, 0f);
            }

            axis = axis.normalized;

            if (angleDeg > 180f)
            {
                angleDeg = 360f - angleDeg;
                axis = -axis;
            }

            radianAngle = angleDeg * Mathf.Deg2Rad;
            return new Plane(axis, 0f);
        }

    }
}