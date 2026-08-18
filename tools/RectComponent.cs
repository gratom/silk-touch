//developer -> gratomov@gmail.com

using System;
using UnityEngine;
using UnityEngine.UI;

namespace Tools
{
    public enum StartCoordinateType
    {
        BottomLeft, //default
        BottomRight,
        TopLeft,
        TopRight
    }

    [RequireComponent(typeof(RectTransform))]
    public class RectComponent : MonoBehaviour
    {
        [SerializeField][HideInInspector] protected RectTransform rectTransform;

        public RectTransform RectTransform => rectTransform;

        public Vector2 Size
        {
            get => rectTransform.rect.size;
            set
            {
                Vector2 currentSize = rectTransform.rect.size;
                Vector2 delta = value - currentSize;
                rectTransform.offsetMin -= new Vector2(delta.x * rectTransform.pivot.x, delta.y * rectTransform.pivot.y);
                rectTransform.offsetMax += new Vector2(delta.x * (1 - rectTransform.pivot.x), delta.y * (1 - rectTransform.pivot.y));
            }
        }

        public float XRight => CornerTopRight.x;
        public float XLeft => CornerTopLeft.x;
        public float YTop => CornerTopRight.y;
        public float YBottom => CornerBottomRight.y;

        public Vector2 CornerTopLeft => GetCorners()[1];
        public Vector2 CornerTopRight => GetCorners()[2];
        public Vector2 CornerBottomLeft => GetCorners()[0];
        public Vector2 CornerBottomRight => GetCorners()[3];

        private Vector3[] GetCorners()
        {
            Vector3[] corners = new Vector3[4];
            rectTransform.GetWorldCorners(corners);
            return corners;
        }

        public float Width
        {
            get => Size.x;
            set => Size = new Vector2(value, Size.y);
        }

        public float Height
        {
            get => Size.y;
            set => Size = new Vector2(Size.x, value);
        }

        public float Rotation
        {
            get => rectTransform.rotation.eulerAngles.z;
            set => rectTransform.rotation = rectTransform.rotation.eulerAngles.WithZ(value).ToQuaternion();
        }

        public float LocalRotation
        {
            get => rectTransform.localRotation.eulerAngles.z;
            set => rectTransform.localRotation = rectTransform.localRotation.eulerAngles.WithZ(value).ToQuaternion();
        }

        public Vector2 AnchoredPosition
        {
            get => rectTransform.anchoredPosition;
            set => rectTransform.anchoredPosition = value;
        }

        public float AnchoredY
        {
            get => rectTransform.anchoredPosition.y;
            set => rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, value);
        }

        public float AnchoredX
        {
            get => rectTransform.anchoredPosition.x;
            set => rectTransform.anchoredPosition = new Vector2(value, rectTransform.anchoredPosition.y);
        }

        public Vector2 WorldPosition
        {
            get => rectTransform.position;
            set => rectTransform.position = value;
        }

        public float OffsetLocalLeft
        {
            get => rectTransform.offsetMin.x;
            set => rectTransform.offsetMin = new Vector2(value, rectTransform.offsetMin.y);
        }

        public float OffsetLocalRight
        {
            get => rectTransform.offsetMax.x;
            set => rectTransform.offsetMax = new Vector2(-value, rectTransform.offsetMax.y);
        }

        public float OffsetLocalBottom
        {
            get => rectTransform.offsetMin.y;
            set => rectTransform.offsetMin = new Vector2(rectTransform.offsetMin.x, value);
        }

        public float OffsetLocalTop
        {
            get => rectTransform.offsetMax.y;
            set => rectTransform.offsetMax = new Vector2(rectTransform.offsetMax.x, -value);
        }

        public float OffsetGlobalLeft => WorldPosition.x - Size.x * rectTransform.pivot.x;

        public float OffsetGlobalRight => Screen.width - WorldPosition.x - Size.x * (1 - rectTransform.pivot.x);

        public float OffsetGlobalBottom => WorldPosition.y - Size.y * rectTransform.pivot.y;

        public float OffsetGlobalTop => Screen.height - WorldPosition.y - Size.y * (1 - rectTransform.pivot.y);

        public Vector2 World2Local(Vector2 worldPos)
        {
            Vector2 ret = new Vector2(worldPos.x - OffsetGlobalLeft, worldPos.y - OffsetGlobalBottom);
            return ret;
        }

        public Vector2 World2Pivot(Vector2 worldPos)
        {
            Vector2 ret = new Vector2(worldPos.x - OffsetGlobalLeft, worldPos.y - OffsetGlobalBottom);
            ret -= Size * rectTransform.pivot;
            return ret;
        }

        public Vector2 Local2World(Vector2 localPos)
        {
            Vector2 ret = new Vector2(localPos.x + OffsetGlobalLeft, localPos.y + OffsetGlobalBottom);
            return ret;
        }

        #region coordinate transformation by corner

        public Vector2 ConvertCoordinateTo(StartCoordinateType cornerType, Vector2 origin)
        {
            switch (cornerType)
            {
                case StartCoordinateType.BottomLeft:
                    return new Vector2(origin.x, origin.y);
                case StartCoordinateType.BottomRight:
                    return new Vector2(Width - origin.x, origin.y);
                case StartCoordinateType.TopLeft:
                    return new Vector2(origin.x, Height - origin.y);
                case StartCoordinateType.TopRight:
                    return new Vector2(Width - origin.x, Height - origin.y);
            }

            return default;
        }

        #endregion

        #region Unity functions

        private void Awake()
        {
            Init();
        }

        protected virtual void OnValidate()
        {
            Init();
        }

        private void Init()
        {
            if (rectTransform == null)
            {
                rectTransform = GetComponent<RectTransform>();
            }
        }

        #endregion

        #region anchors presets

        public enum PresetType
        {
            TopLeft,
            TopCenter,
            TopRight,
            MiddleLeft,
            MiddleCenter,
            MiddleRight,
            BottomLeft,
            BottomCenter,
            BottomRight,

            StretchTop, // anchors.x: 0..1, y: 1
            StretchMiddle, // anchors.x: 0..1, y: 0.5
            StretchBottom, // anchors.x: 0..1, y: 0

            StretchLeft, // x: 0, y: 0..1
            StretchCenter, // x: 0.5, y: 0..1
            StretchRight, // x: 1, y: 0..1

            StretchAll
        }

        public void SetAnchorsPreset(PresetType preset)
        {
            RectTransform rt = rectTransform != null ? rectTransform : GetComponent<RectTransform>();
            RectTransform parent = rt != null ? rt.parent as RectTransform : null;

            if (rt == null) { return; }

            Vector2 oldAnchorMin = rt.anchorMin;
            Vector2 oldAnchorMax = rt.anchorMax;
            Vector2 oldOffsetMin = rt.offsetMin;
            Vector2 oldOffsetMax = rt.offsetMax;
            Vector2 oldPivot = rt.pivot;
            Vector2 parentSize = parent != null ? parent.rect.size : Vector2.zero;
            float oldWidth = rt.rect.width;
            float oldHeight = rt.rect.height;

            GetPresetValues(preset, out Vector2 newAnchorMin, out Vector2 newAnchorMax, out Vector2 newPivot);

            rt.anchorMin = newAnchorMin;
            rt.anchorMax = newAnchorMax;

            Vector2 deltaMin = new Vector2(
                (oldAnchorMin.x - newAnchorMin.x) * parentSize.x,
                (oldAnchorMin.y - newAnchorMin.y) * parentSize.y
            );
            Vector2 deltaMax = new Vector2(
                (oldAnchorMax.x - newAnchorMax.x) * parentSize.x,
                (oldAnchorMax.y - newAnchorMax.y) * parentSize.y
            );

            rt.offsetMin = oldOffsetMin + deltaMin;
            rt.offsetMax = oldOffsetMax + deltaMax;

            rt.pivot = newPivot;

            float w = rt.rect.width;
            float h = rt.rect.height;
            Vector2 pivotDelta = new Vector2(
                (newPivot.x - oldPivot.x) * w,
                (newPivot.y - oldPivot.y) * h
            );
            rt.anchoredPosition += pivotDelta;
        }

        public static void GetPresetValues(PresetType preset, out Vector2 aMin, out Vector2 aMax, out Vector2 pivot)
        {
            Vector2 TL = new Vector2(0f, 1f);
            Vector2 TC = new Vector2(0.5f, 1f);
            Vector2 TR = new Vector2(1f, 1f);
            Vector2 ML = new Vector2(0f, 0.5f);
            Vector2 MC = new Vector2(0.5f, 0.5f);
            Vector2 MR = new Vector2(1f, 0.5f);
            Vector2 BL = new Vector2(0f, 0f);
            Vector2 BC = new Vector2(0.5f, 0f);
            Vector2 BR = new Vector2(1f, 0f);

            switch (preset)
            {
                case PresetType.TopLeft:
                    aMin = TL;
                    aMax = TL;
                    pivot = TL;
                    return;
                case PresetType.TopCenter:
                    aMin = TC;
                    aMax = TC;
                    pivot = TC;
                    return;
                case PresetType.TopRight:
                    aMin = TR;
                    aMax = TR;
                    pivot = TR;
                    return;
                case PresetType.MiddleLeft:
                    aMin = ML;
                    aMax = ML;
                    pivot = ML;
                    return;
                case PresetType.MiddleCenter:
                    aMin = MC;
                    aMax = MC;
                    pivot = MC;
                    return;
                case PresetType.MiddleRight:
                    aMin = MR;
                    aMax = MR;
                    pivot = MR;
                    return;
                case PresetType.BottomLeft:
                    aMin = BL;
                    aMax = BL;
                    pivot = BL;
                    return;
                case PresetType.BottomCenter:
                    aMin = BC;
                    aMax = BC;
                    pivot = BC;
                    return;
                case PresetType.BottomRight:
                    aMin = BR;
                    aMax = BR;
                    pivot = BR;
                    return;

                case PresetType.StretchTop:
                    aMin = new Vector2(0f, 1f);
                    aMax = new Vector2(1f, 1f);
                    pivot = new Vector2(0.5f, 1f);
                    return;
                case PresetType.StretchMiddle:
                    aMin = new Vector2(0f, 0.5f);
                    aMax = new Vector2(1f, 0.5f);
                    pivot = new Vector2(0.5f, 0.5f);
                    return;
                case PresetType.StretchBottom:
                    aMin = new Vector2(0f, 0f);
                    aMax = new Vector2(1f, 0f);
                    pivot = new Vector2(0.5f, 0f);
                    return;

                case PresetType.StretchLeft:
                    aMin = new Vector2(0f, 0f);
                    aMax = new Vector2(0f, 1f);
                    pivot = new Vector2(0f, 0.5f);
                    return;
                case PresetType.StretchCenter:
                    aMin = new Vector2(0.5f, 0f);
                    aMax = new Vector2(0.5f, 1f);
                    pivot = new Vector2(0.5f, 0.5f);
                    return;
                case PresetType.StretchRight:
                    aMin = new Vector2(1f, 0f);
                    aMax = new Vector2(1f, 1f);
                    pivot = new Vector2(1f, 0.5f);
                    return;

                case PresetType.StretchAll:
                    aMin = new Vector2(0f, 0f);
                    aMax = new Vector2(1f, 1f);
                    pivot = new Vector2(0.5f, 0.5f);
                    return;
            }

            //fallback
            aMin = new Vector2(0.5f, 0.5f);
            aMax = new Vector2(0.5f, 0.5f);
            pivot = new Vector2(0.5f, 0.5f);
        }

        #endregion
    }

    public static class RectExtensions
    {
        public static void SetAnchorsPreset(this RectTransform rt, RectComponent.PresetType preset)
        {
            RectTransform parent = rt != null ? rt.parent as RectTransform : null;

            if (rt == null) { return; }

            Vector2 oldAnchorMin = rt.anchorMin;
            Vector2 oldAnchorMax = rt.anchorMax;
            Vector2 oldOffsetMin = rt.offsetMin;
            Vector2 oldOffsetMax = rt.offsetMax;
            Vector2 oldPivot = rt.pivot;
            Vector2 parentSize = parent != null ? parent.rect.size : Vector2.zero;

            RectComponent.GetPresetValues(preset, out Vector2 newAnchorMin, out Vector2 newAnchorMax, out Vector2 newPivot);

            rt.anchorMin = newAnchorMin;
            rt.anchorMax = newAnchorMax;

            Vector2 deltaMin = new Vector2(
                (oldAnchorMin.x - newAnchorMin.x) * parentSize.x,
                (oldAnchorMin.y - newAnchorMin.y) * parentSize.y
            );
            Vector2 deltaMax = new Vector2(
                (oldAnchorMax.x - newAnchorMax.x) * parentSize.x,
                (oldAnchorMax.y - newAnchorMax.y) * parentSize.y
            );

            rt.offsetMin = oldOffsetMin + deltaMin;
            rt.offsetMax = oldOffsetMax + deltaMax;

            rt.pivot = newPivot;

            float w = rt.rect.width;
            float h = rt.rect.height;
            Vector2 pivotDelta = new Vector2(
                (newPivot.x - oldPivot.x) * w,
                (newPivot.y - oldPivot.y) * h
            );
            rt.anchoredPosition += pivotDelta;
        }

        private static RectComponent AddRectComponent(GameObject gameObject)
        {
            if (gameObject == null)
            {
#if UNITY_EDITOR
                Debug.LogError("You try to add RectComponent, but GameObject reference is null");
#endif
                return null;
            }

            if (gameObject.GetComponent<RectTransform>() != null)
            {
                return gameObject.GetOrAddComponent<RectComponent>();
            }

#if UNITY_EDITOR
            Debug.LogError($"You try to add RectComponent to '{gameObject.name}', but it is missing a RectTransform");
#endif
            return null;
        }

        public static RectComponent AsRectComponent(this Button button)
        {
            return AddRectComponent(button?.gameObject);
        }

        public static RectComponent AsRectComponent(this GameObject gameObject)
        {
            return AddRectComponent(gameObject);
        }

        public static RectComponent AsRectComponent(this RectTransform rectTransform)
        {
            return AddRectComponent(rectTransform?.gameObject);
        }

        public static RectComponent AsRectComponent(this Image image)
        {
            return AddRectComponent(image?.gameObject);
        }

        public static Canvas GetCanvas(this RectTransform component)
        {
            if (component == null)
            {
                return null;
            }
            return component.GetComponentInParent<Canvas>();
        }

        public static Rect GetCanvasScaledSafeArea(this Canvas canvas)
        {
            if (canvas == null)
            {
                return Rect.zero;
            }

            Rect safeArea = Screen.safeArea;
            float scale = canvas.scaleFactor;
            Debug.Log($"Scale {scale}");
            return new Rect(
                safeArea.x / scale,
                safeArea.y / scale,
                safeArea.width / scale,
                safeArea.height / scale
            );
        }

        public static void SetAsLineBetween2Points(this Image img, Vector2 startPoint, Vector2 endPoint, float thickness = 10)
        {
            if (img == null)
            {
                return;
            }

            RectTransform rect = img.rectTransform;

            float scale = img.GetComponent<RectTransform>().GetCanvas().scaleFactor;
            Vector2 difference = endPoint - startPoint;
            float distance = difference.magnitude / scale;
            float angle = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;

            rect.pivot = new Vector2(0, 0.5f);
            rect.position = startPoint;
            rect.sizeDelta = new Vector2(distance, thickness);
            rect.rotation = Quaternion.Euler(0, 0, angle);
        }

        // public static void SetAsArrow(this Image img, Vector2 start, Vector2 target)
        // {
        //     RectTransform rectTransform = img.rectTransform;
        //
        //     Vector2 oldPivot = rectTransform.pivot;
        //     Vector2 newPivot = new Vector2(0.5f, 0);
        //
        //     if (oldPivot != newPivot)
        //     {
        //         Vector2 size = rectTransform.rect.size;
        //         Vector2 deltaPivot = newPivot - oldPivot;
        //         rectTransform.pivot = newPivot;
        //         rectTransform.anchoredPosition += new Vector2(deltaPivot.x * size.x, deltaPivot.y * size.y);
        //     }
        //
        //     rectTransform.position = start;
        //     Vector2 direction = target - start;
        //     float distance = direction.magnitude;
        //
        //     rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, distance);
        //     float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        //     rectTransform.rotation = Quaternion.Euler(0, 0, angle - 90f);
        // }

        public static void SetAsLineBetween2PointsLocal(this Image img, Vector2 startAnchoredPos, Vector2 endAnchoredPos, float thickness = 10)
        {
            if (img == null)
            {
                return;
            }

            RectTransform rect = img.rectTransform;
            Vector2 difference = endAnchoredPos - startAnchoredPos;
            float distance = difference.magnitude;
            float angle = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;

            rect.pivot = new Vector2(0, 0.5f);
            rect.anchoredPosition = startAnchoredPos;
            rect.sizeDelta = new Vector2(distance, thickness);
            rect.localRotation = Quaternion.Euler(0, 0, angle);
        }

        public static void ForceRebuild(this LayoutGroup layoutGroup, bool disableAfterRebuild = true)
        {
            layoutGroup.gameObject.SetActiveSafe(true);
            layoutGroup.enabled = true;
            LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroup.GetComponent<RectTransform>());
            if (disableAfterRebuild)
            {
                layoutGroup.enabled = false;
            }
        }
    }

}