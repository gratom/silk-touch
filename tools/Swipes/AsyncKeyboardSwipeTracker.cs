#if UNITASK

using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Tools.Swipes
{
    public static class AsyncKeyboardSwipeTracker
    {
        public static event Action<AsyncSwipeTracker.Swipe> OnSwipe
        {
            add
            {
                onSwipe += value;
                EnsureTracking();
            }
            remove => onSwipe -= value;
        }

        private static Action<AsyncSwipeTracker.Swipe> onSwipe;
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
                AsyncSwipeTracker.Swipe fakeSwipe = new AsyncSwipeTracker.Swipe
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

#endif