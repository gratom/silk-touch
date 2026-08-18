using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Tools
{
    public class LongPressButton : UIBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        [SerializeField] private bool interactable;
        [SerializeField] private Color enabledColor;
        [SerializeField] private Color disabledColor;
        [SerializeField] private Text text;

        public bool Interactable
        {
            get => interactable;
            set
            {
                interactable = value;
                if (text != null)
                {
                    text.color = interactable ? enabledColor : disabledColor;
                }
            }
        }

        [Tooltip("How long must pointer be down on this object to trigger a long press")]
        public float durationThreshold = 1.0f;

        public UnityEvent onLongPressComplete = new UnityEvent();
        public UnityEvent onLongPressStart = new UnityEvent();
        public UnityEvent onLongPressAbort = new UnityEvent();
        public UnityEvent onDisabled = new UnityEvent();

        private Coroutine holderCoroutineInstance;

        public UnityEvent<float> onLongPressProcess = new UnityEvent<float>();

        protected void OnValidate()
        {
            if (text != null)
            {
                text.color = interactable ? enabledColor : disabledColor;
            }
        }

        private IEnumerator HoldingCoroutine(float holdTime)
        {
            float timeEnd = Time.time + holdTime;
            while (timeEnd > Time.time)
            {
                yield return new WaitForEndOfFrame();
                onLongPressProcess?.Invoke(1 - (timeEnd - Time.time) / holdTime);
            }
            onLongPressComplete?.Invoke();
            EndCoroutine();
        }

        private void EndCoroutine()
        {
            if (holderCoroutineInstance != null)
            {
                StopCoroutine(holderCoroutineInstance);
                holderCoroutineInstance = null;
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (Interactable)
            {
                onLongPressStart?.Invoke();
                if (holderCoroutineInstance == null)
                {
                    holderCoroutineInstance = StartCoroutine(HoldingCoroutine(durationThreshold));
                }
            }
            else
            {
                onDisabled?.Invoke();
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            EndCoroutine();
            onLongPressAbort?.Invoke();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            EndCoroutine();
            onLongPressAbort?.Invoke();
        }
    }
}