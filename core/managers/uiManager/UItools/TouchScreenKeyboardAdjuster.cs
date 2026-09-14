#if UI_TMP
using Crystal;
using UnityEngine;
using UnityEngine.UI;

namespace SilkTouch.UI
{
    using Tools;

    public class TouchScreenKeyboardAdjuster : MonoBehaviour
    {
        public enum ConstraintPriority
        {
            Bottom,
            Top
        }

        [SerializeField]
        private ConstraintPriority priority = ConstraintPriority.Bottom;
        [SerializeField]
        private float smoothness = 12f;
        [SerializeField]
        private float extraOffset = 50f;
        [SerializeField]
        private float maxTopMargin = 100f;
        public float virtualKeyBoard = 1000;

        private RectTransform rct;
        private RectTransform canvasRect;
        private Canvas canvas;
        private Vector2 initialPosition;
        private float targetY;

        private AndroidJavaObject view;
        private AndroidJavaObject rect;
        private int screenHeight;

        private void Start()
        {
            rct = GetComponent<RectTransform>();
            canvas = GetComponentInParent<Canvas>();
            canvasRect = canvas.GetComponent<RectTransform>();
            initialPosition = rct.anchoredPosition;

            if (Application.platform == RuntimePlatform.Android)
            {
                using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                {
                    AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                    view = activity.Get<AndroidJavaObject>("mUnityPlayer").Call<AndroidJavaObject>("getView");
                    rect = new AndroidJavaObject("android.graphics.Rect");
                    screenHeight = view.Call<AndroidJavaObject>("getRootView").Call<int>("getHeight");
                }
            }
        }

#if UNITY_EDITOR
        private GameObject keyboardEmulator;
        private RectTransform keyboardEmulatorRT;
#endif

        private void Update()
        {
#if UNITY_EDITOR
            if (Input.GetKey(KeyCode.KeypadMinus))
            {
                if (keyboardEmulator == null)
                {
                    keyboardEmulator = new GameObject("KeyboardEmulator");
                    keyboardEmulator.transform.parent = canvas.transform;
                    keyboardEmulatorRT = keyboardEmulator.GetOrAddComponent<RectTransform>();
                    RectComponent rc = keyboardEmulator.GetOrAddComponent<RectComponent>();
                    keyboardEmulator.AddComponent<Image>().color = Color.gray;
                    rc.SetAnchorsPreset(RectComponent.PresetType.BottomCenter);
                }
                keyboardEmulatorRT.anchoredPosition = new Vector2(0f, 0f);
                keyboardEmulatorRT.sizeDelta = new Vector2(canvasRect.rect.width, virtualKeyBoard);
            }
            else
            {
                if (keyboardEmulatorRT != null)
                {
                    keyboardEmulatorRT.sizeDelta = new Vector2(canvasRect.rect.width, 0);
                }
            }
#endif

            float keyboardHeight = GetKeyboardHeight();

            if (keyboardHeight > 250)
            {
                targetY = BottomToPivot(keyboardHeight) + extraOffset - rct.rect.y;
            }
            else
            {
                targetY = initialPosition.y;
            }

            float currentY = Mathf.Lerp(rct.anchoredPosition.y, targetY, Time.deltaTime * smoothness);
            rct.anchoredPosition = new Vector2(initialPosition.x, currentY);
        }

        private float GetKeyboardHeight()
        {
#if UNITY_EDITOR
            return Input.GetKey(KeyCode.KeypadMinus) ? virtualKeyBoard : 0f;
#else
        view.Call("getWindowVisibleDisplayFrame", rect);
        int visibleHeight = rect.Call<int>("height");
        return (screenHeight - visibleHeight) / (Screen.height / canvasRect.rect.height);
#endif
        }

        private float zero => canvasRect.rect.height - canvasRect.rect.height * rct.anchorMin.y;

        public float PivotToBottom(float pivotValue)
        {
            return pivotValue + zero;
        }

        public float BottomToPivot(float bottomValue)
        {
            return bottomValue - zero;
        }

    }
}

#endif