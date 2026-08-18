using UnityEngine;
using System;
using Cysharp.Threading.Tasks;

namespace Tools.Swipes
{
    public static class SwipeTracker
    {
        public enum SwipeDirection
        {
            Left,
            Right,
            Up,
            Down
        }

        public struct Swipe
        {
            public bool IsOverUI;
            public Vector2 Start;
            public Vector2 End;
            public float DeltaTime;
            public Vector2 Delta => End - Start;
            public Vector2 Impulse => DeltaTime > 0 ? Delta / DeltaTime : Vector2.zero;

            public SwipeDirection Direction => Mathf.Abs(Impulse.x) > Mathf.Abs(Impulse.y)
                ? Impulse.x > 0 ? SwipeDirection.Right : SwipeDirection.Left
                : Impulse.y > 0
                    ? SwipeDirection.Up
                    : SwipeDirection.Down;

            public SwipeDirection AxisXDirection => Impulse.x >= 0 ? SwipeDirection.Right : SwipeDirection.Left;
            public SwipeDirection AxisYDirection => Impulse.y >= 0 ? SwipeDirection.Up : SwipeDirection.Down;
        }

        private const float SWIPE_THRESHOLD_SCREEN_PERCENT = 0.03f;
        private static readonly float screenMinAxisPixelSize = Mathf.Min(Screen.width, Screen.height);

        public static event Action<Swipe> OnSwipeAny
        {
            add
            {
                onSwipeAny += value;
                EnsureTracking();
            }
            remove => onSwipeAny -= value;
        }

        public static event Action<Swipe> OnSwipeUI
        {
            add
            {
                onSwipeUI += value;
                EnsureTracking();
            }
            remove => onSwipeUI -= value;
        }

        public static event Action<Swipe> OnSwipeWorld
        {
            add
            {
                onSwipeWorld += value;
                EnsureTracking();
            }
            remove => onSwipeWorld -= value;
        }

        private static Action<Swipe> onSwipeAny;
        private static Action<Swipe> onSwipeUI;
        private static Action<Swipe> onSwipeWorld;

        private static Swipe currentSwipe;
        private static bool isSwiping = false;
        private static bool trackingActive = false;
        private static float swipeStartTime;

        private static async void EnsureTracking()
        {
            if (trackingActive)
            {
                return;
            }

            trackingActive = true;
            while (onSwipeAny != null || onSwipeUI != null || onSwipeWorld != null)
            {
#if UNITY_EDITOR
                TrackMouseSwipe();
#else
                TrackTouchSwipe();
#endif
                await UniTask.Yield();
            }
            trackingActive = false;
        }

        private static void TrackMouseSwipe()
        {
            if (Input.GetMouseButtonDown(0))
            {
                InitSwipe(Input.mousePosition);
                return;
            }

            if (Input.GetMouseButtonUp(0))
            {
                TryFinishSwipe(Input.mousePosition);
            }
        }

        private static void TrackTouchSwipe()
        {
            if (Input.touchCount == 0)
            {
                return;
            }

            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    InitSwipe(touch.position);
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    TryFinishSwipe(touch.position);
                    break;
            }
        }

        private static void InitSwipe(Vector2 startPos)
        {
            currentSwipe = new Swipe
            {
                Start = startPos,
                IsOverUI = UnityEngine.EventSystems.EventSystem.current != null && UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()
            };
            swipeStartTime = Time.time;
            isSwiping = true;
        }

        private static void TryFinishSwipe(Vector2 endPos)
        {
            if (isSwiping)
            {
                currentSwipe.End = endPos;
                currentSwipe.DeltaTime = Time.time - swipeStartTime;
                EvaluateSwipe(currentSwipe);
                isSwiping = false;
            }
        }

        private static void EvaluateSwipe(Swipe swipe)
        {
            float minDistance = SWIPE_THRESHOLD_SCREEN_PERCENT * screenMinAxisPixelSize;
            if (swipe.Delta.magnitude < minDistance)
            {
                return;
            }
            onSwipeAny?.Invoke(swipe);
            if (swipe.IsOverUI)
            {
                onSwipeUI?.Invoke(swipe);
            }
            else
            {
                onSwipeWorld?.Invoke(swipe);
            }
        }
    }
}