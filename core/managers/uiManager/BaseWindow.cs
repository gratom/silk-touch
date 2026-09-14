#if UI_TMP
using System;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SilkTouch.UI
{
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(CanvasScaler))]
    [RequireComponent(typeof(GraphicRaycaster))]
    public abstract class BaseWindow : MonoBehaviour
    {
        [SerializeField] protected Canvas canvas;

        public abstract Type WindowType { get; }

        public int Order
        {
            get => canvas.sortingOrder;
            set => canvas.sortingOrder = value;
        }

        public bool IsShowing => isShowing;
        private bool isShowing;

#if UNITY_EDITOR
        [ContextMenu("Refresh")]
        private void RefreshEditorData()
        {
            canvas = null;
            OnValidate();
        }

        private void OnValidate()
        {
            if (canvas == null)
            {

                canvas = GetComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                CanvasScaler scaler = GetComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
#if UNITY_ANDROID
                scaler.referenceResolution = new Vector2(1080, 1920); //portrait Android
#endif
#if UNITY_STANDALONE_WIN
                scaler.referenceResolution = new Vector2(1920, 1080); //landscape for win
#endif
                scaler.matchWidthOrHeight = 0.0f;
                EditorUtility.SetDirty(gameObject);
            }
        }
#endif
        public void Show()
        {
            gameObject.SetActive(true);
            isShowing = true;
            OnShow();
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            isShowing = false;
            OnHide();
        }

        protected abstract void OnHide();
        protected abstract void OnShow();
    }
}
#endif