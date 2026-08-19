using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Tools
{
    public static class Pause
    {
        private static bool isPaused = false;
        private static bool timeScaleFollowPause;

        public static event Action OnPaused;
        public static event Action OnUnpaused;
        public static event Action<bool> OnPauseChange;

        public static Task Paused => pausedAwaiter.Task;
        private static TaskCompletionSource<bool> pausedAwaiter = new TaskCompletionSource<bool>();
        public static Task UnPaused => unpausedAwaiter.Task;
        private static TaskCompletionSource<bool> unpausedAwaiter;

        public static bool IsPaused
        {
            get => isPaused;
            set
            {
                if (isPaused == value)
                {
                    return;
                }
                isPaused = value;
                ApplyTimeScale();
                NotifyState();

                if (isPaused)
                {
                    PauseWatchdog.EnsureExists();
                }
            }
        }

        public static bool TimeScaleFollowPause
        {
            get => timeScaleFollowPause;
            set
            {
                if (value == timeScaleFollowPause)
                {
                    return;
                }
                timeScaleFollowPause = value;
                ApplyTimeScale();
            }
        }

        private static void ApplyTimeScale()
        {
            if (timeScaleFollowPause)
            {
                Time.timeScale = isPaused ? 0 : 1;
            }
        }

        private static void NotifyState()
        {
            OnPauseChange?.Invoke(isPaused);
            if (isPaused)
            {
                OnPaused?.Invoke();
                unpausedAwaiter = new TaskCompletionSource<bool>();
                pausedAwaiter.TrySetResult(true);
            }
            else
            {
                OnUnpaused?.Invoke();
                pausedAwaiter = new TaskCompletionSource<bool>();
                unpausedAwaiter.TrySetResult(true);
            }
        }

        static Pause()
        {
            unpausedAwaiter = new TaskCompletionSource<bool>();
            unpausedAwaiter.TrySetResult(true);
        }

        public static void PauseOn()
        {
            IsPaused = true;
        }
        public static void PauseOff()
        {
            IsPaused = false;
        }
        public static void Toggle()
        {
            IsPaused = !isPaused;
        }

        [DefaultExecutionOrder(-1000)]
        private class PauseWatchdog : MonoBehaviour
        {
            private static PauseWatchdog instance;

            public static void EnsureExists()
            {
                if (instance != null)
                {
                    return;
                }

                GameObject go = new GameObject("PauseWatchdog");
                instance = go.AddComponent<PauseWatchdog>();
                go.hideFlags = HideFlags.HideAndDontSave;
                DontDestroyOnLoad(go);
            }

            private void Update()
            {
                if (timeScaleFollowPause)
                {
                    EnsureCorrectTimeScale();
                }
            }

            private void LateUpdate()
            {
                if (timeScaleFollowPause)
                {
                    EnsureCorrectTimeScale();
                }
            }

            private void OnApplicationFocus(bool hasFocus)
            {
                if (timeScaleFollowPause && hasFocus)
                {
                    EnsureCorrectTimeScale();
                }
            }

            private void OnApplicationPause(bool pauseStatus)
            {
                if (timeScaleFollowPause && !pauseStatus)
                {
                    EnsureCorrectTimeScale();
                }
            }

            private void OnDestroy()
            {
                instance = null;
                EnsureExists();
            }

            private void OnDisable()
            {
                enabled = true;
            }

            private static void EnsureCorrectTimeScale()
            {
                float correctTimeScale = isPaused ? 0 : 1;
                if (!Mathf.Approximately(Time.timeScale, correctTimeScale))
                {
                    Time.timeScale = correctTimeScale;
                }
            }
        }
    }
}