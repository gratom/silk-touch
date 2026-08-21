#if UI_TMP
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Tools.ScrollComponent
{
    public class DragComponent : RectComponent, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerClickHandler, IScrollHandler
    {
        public event Action<PointerEventData, Vector2> OnDragEvent;
        public event Action<PointerEventData, Vector2> OnBeginDragEvent;
        public event Action<PointerEventData, Vector2> OnEndDragEvent;
        public event Action<PointerEventData, Vector2> OnClickEvent;
        public event Action<PointerEventData, Vector2> OnScrollEvent;

        private AverageVector2 averageImpulse = new AverageVector2();
        private Vector2 startPoint;
        private bool isDrag = false;

        public Vector2 StartPoint => startPoint;

        protected virtual void Awake()
        {
            if (!TryGetComponent(out Graphic graphic))
            {
                Image raycastCatcher = gameObject.AddComponent<Image>();
                raycastCatcher.color = Color.clear;
                raycastCatcher.raycastTarget = true;
            }
            else
            {
                graphic.raycastTarget = true;
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            averageImpulse.AddNext(eventData.delta);
            OnDragEvent?.Invoke(eventData, averageImpulse.Average);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            isDrag = true;
            startPoint = eventData.position;
            averageImpulse.Clear();
#if UNITY_EDITOR
            if (OnBeginDragEvent == null)
            {
                //Debug.Log($"OnBeginDrag [x:{startPoint.x}, y:{startPoint.y}]");
            }
            else
            {
                OnBeginDragEvent.Invoke(eventData, startPoint);
            }
#else
            OnBeginDragEvent?.Invoke(eventData, startPoint);
#endif
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            isDrag = false;
            averageImpulse.AddNext(eventData.delta);

#if UNITY_EDITOR
            if (OnEndDragEvent == null)
            {
                Debug.Log($"OnEndDrag [x:{eventData.position.x}, y:{eventData.position.y}]");
            }
            else
            {
                OnEndDragEvent.Invoke(eventData, averageImpulse.Average);
            }
#else
            OnEndDragEvent?.Invoke(eventData, averageImpulse.Average);
#endif
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!isDrag)
            {
                OnClickEvent?.Invoke(eventData, eventData.position);
            }
        }

        public void OnScroll(PointerEventData eventData)
        {
            OnScrollEvent?.Invoke(eventData, eventData.scrollDelta);
        }
    }
}
#endif