using System;
using UnityEngine;

namespace SilkTouch.Tools
{
    public static class CursorGuard
    {
        private static bool isLocked;

        public static event Action<bool> OnLockChange;

        public static bool IsLocked
        {
            get => isLocked;
            set
            {
                if (isLocked == value)
                {
                    return;
                }

                isLocked = value;
                ApplyCursorState();
                OnLockChange?.Invoke(isLocked);

                if (isLocked)
                {
                    CursorGuardWatchdog.EnsureExists();
                }
            }
        }

        public static void Lock()
        {
            IsLocked = true;
        }

        public static void Unlock()
        {
            IsLocked = false;
        }

        private static void ApplyCursorState()
        {
            Cursor.lockState = isLocked ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !isLocked;
        }

        [DefaultExecutionOrder(-1000)]
        private class CursorGuardWatchdog : MonoBehaviour
        {
            private static CursorGuardWatchdog instance;

            public static void EnsureExists()
            {
                if (instance != null)
                {
                    return;
                }

                GameObject go = new GameObject("CursorGuardWatchdog");
                instance = go.AddComponent<CursorGuardWatchdog>();
                go.hideFlags = HideFlags.HideAndDontSave;
                DontDestroyOnLoad(go);
            }

            private void Update()
            {
                EnsureCorrectCursorState();
            }

            private void LateUpdate()
            {
                EnsureCorrectCursorState();
            }

            private void OnApplicationFocus(bool hasFocus)
            {
                if (hasFocus)
                {
                    EnsureCorrectCursorState();
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

            private static void EnsureCorrectCursorState()
            {
                CursorLockMode correctLockState = isLocked ? CursorLockMode.Locked : CursorLockMode.None;
                if (Cursor.lockState != correctLockState)
                {
                    Cursor.lockState = correctLockState;
                }

                bool correctVisible = !isLocked;
                if (Cursor.visible != correctVisible)
                {
                    Cursor.visible = correctVisible;
                }
            }
        }
    }
}
