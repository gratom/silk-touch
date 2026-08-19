using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Tools
{
    [RequireComponent(typeof(Image))]
    public class TouchPanelInput : RectComponent, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private Image raycastTarget;

        public event Action<Vector2> OnTouchStart;
        public event Action<Vector2, float> OnTouching;
        public event Action<Vector2> OnTouchEnd;

        private bool isTouching = false;

        protected override void OnValidate()
        {
            base.OnValidate();
            raycastTarget = GetComponent<Image>();
            raycastTarget.raycastTarget = true;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!isTouching)
            {
                OnTouchStart?.Invoke(World2Pivot(eventData.position).normalized);
            }
            isTouching = true;
            Vector2 localPos = World2Pivot(eventData.position);
            OnTouching?.Invoke(localPos.normalized, Mathf.Clamp01(localPos.magnitude / (Height / 2)));
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            isTouching = false;
            OnTouchEnd?.Invoke(World2Pivot(eventData.position));
        }

        public void Update()
        {
            if (isTouching)
            {
                Vector2 localPos = World2Pivot(Input.mousePosition);
                OnTouching?.Invoke(localPos.normalized, Mathf.Clamp01(localPos.magnitude / (Height / 2)));
            }
        }

    }
}