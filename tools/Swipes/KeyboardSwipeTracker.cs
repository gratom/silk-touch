using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Tools.Swipes
{
    public static class KeyboardSwipeTracker
    {
        public static event Action<SwipeTracker.Swipe> OnSwipe
        {
            add
            {
                onSwipe += value;
                EnsureTracking();
            }
            remove => onSwipe -= value;
        }

        private static Action<SwipeTracker.Swipe> onSwipe;
        private static bool trackingActive;

        private static async void EnsureTracking()
        {
            if (trackingActive)
            {
                return;
            }

            trackingActive = true;

            while (onSwipe != null)
            {
                CheckArrowKey(KeyCode.UpArrow, Vector2.up);
                CheckArrowKey(KeyCode.DownArrow, Vector2.down);
                CheckArrowKey(KeyCode.LeftArrow, Vector2.left);
                CheckArrowKey(KeyCode.RightArrow, Vector2.right);

                await UniTask.Yield();
            }

            trackingActive = false;
        }

        private static void CheckArrowKey(KeyCode key, Vector2 direction)
        {
            if (Input.GetKeyDown(key))
            {
                SwipeTracker.Swipe fakeSwipe = new SwipeTracker.Swipe
                {
                    IsOverUI = false,
                    Start = Vector2.zero,
                    End = direction * 100f,
                    DeltaTime = 0.1f
                };

                onSwipe?.Invoke(fakeSwipe);
            }
        }
    }
}