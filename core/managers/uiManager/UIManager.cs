#if UI_TMP
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace SilkTouch.UI
{
    using Tools;
    using Managers;

    public class UIManager : BaseManager
    {
        public override Type ManagerType => typeof(UIManager);

        public override bool IsFunctional => true;
#pragma warning disable
        [SerializeField] private List<BaseWindow> windowContainers;
#pragma warning restore

        private Dictionary<Type, BaseWindow> windowsDictionary = new Dictionary<Type, BaseWindow>();
        public event Action<BaseWindow> OnWindowShow;

#pragma warning disable

        protected override async Task<bool> OnInit()
        {
            InitDictionary();
            return true;
        }

#pragma warning restore

        private void InitDictionary()
        {
            foreach (BaseWindow window in windowContainers)
            {
                windowsDictionary.Add(window.GetType(), window);
            }
        }

        public T ShowWindow<T>(Action<T> beforeOpen = null) where T : BaseWindow
        {
            Debug.Log($"Showing window {typeof(T)}".AsUIEvent());
            T window = GetWindow<T>();
            if (window != null)
            {
                beforeOpen?.Invoke(window);
                SetTopOrder(window);
                window.Show();
                OnWindowShow?.Invoke(window);
            }
#if UNITY_EDITOR
            else
            {
                Debug.LogError($"{typeof(T).Name} window is null");
            }
#endif
            return window;
        }

        public T EnsureWindowShow<T>(Action<T> beforeOpen = null) where T : BaseWindow
        {
            Debug.Log($"Ensure Showing window {typeof(T)}".AsUIEvent());
            T window = GetWindow<T>();
            if (window != null)
            {
                if (!window.IsShowing)
                {
                    beforeOpen?.Invoke(window);
                    SetTopOrder(window);
                    window.Show();
                    OnWindowShow?.Invoke(window);
                }
            }
#if UNITY_EDITOR
            else
            {
                Debug.LogError($"{typeof(T).Name} window is null");
            }
#endif
            return window;
        }

        public BaseWindow ShowWindow(Type t, Action beforeOpen = null)
        {
            Debug.Log($"Showing window {t?.Name}".AsUIEvent());
            BaseWindow window = GetWindow(t);
            if (window != null)
            {
                beforeOpen?.Invoke();
                SetTopOrder(window);
                window.Show();
                OnWindowShow?.Invoke(window);
            }
#if UNITY_EDITOR
            else
            {
                Debug.LogError($"{t?.Name} window is null");
            }
#endif
            return window;
        }

        public BaseWindow EnsureWindowShow(Type t, Action beforeOpen = null)
        {
            Debug.Log($"Ensure Showing window {t?.Name}".AsUIEvent());
            BaseWindow window = GetWindow(t);
            if (window != null)
            {
                if (!window.IsShowing)
                {
                    beforeOpen?.Invoke();
                    SetTopOrder(window);
                    window.Show();
                    OnWindowShow?.Invoke(window);
                }
            }
#if UNITY_EDITOR
            else
            {
                Debug.LogError($"{t?.Name} window is null");
            }
#endif
            return window;
        }

        public T HideWindow<T>() where T : BaseWindow
        {
            Debug.Log($"Hiding window {typeof(T)}".AsUIEvent());

            T window = GetWindow<T>();
            if (window != null)
            {
                SetLowestOrder(window);
                window.Hide();
            }
#if UNITY_EDITOR
            else
            {
                Debug.LogError($"{typeof(T).Name} window is null");
            }
#endif
            return window;
        }

        public BaseWindow HideWindow(Type t)
        {
            Debug.Log($"Hiding window {t?.Name}".AsUIEvent());
            BaseWindow window = GetWindow(t);
            if (window != null)
            {
                SetLowestOrder(window);
                window.Hide();
            }
#if UNITY_EDITOR
            else
            {
                Debug.LogError($"{t?.Name} window is null");
            }
#endif
            return window;
        }

        public T EnsureWindowHidden<T>() where T : BaseWindow
        {
            Debug.Log($"Ensure Hiding window {typeof(T)}".AsUIEvent());
            T window = GetWindow<T>();
            if (window != null)
            {
                if (window.IsShowing)
                {
                    SetLowestOrder(window);
                    window.Hide();
                }
            }
#if UNITY_EDITOR
            else
            {
                Debug.LogError($"{typeof(T).Name} window is null");
            }
#endif
            return window;
        }

        public BaseWindow EnsureWindowHidden(Type t)
        {
            Debug.Log($"Ensure Hiding window {t?.Name}".AsUIEvent());
            BaseWindow window = GetWindow(t);
            if (window != null)
            {
                if (window.IsShowing)
                {
                    SetLowestOrder(window);
                    window.Hide();
                }
            }
#if UNITY_EDITOR
            else
            {
                Debug.LogError($"{t?.Name} window is null");
            }
#endif
            return window;
        }


        public T GetWindow<T>() where T : BaseWindow
        {
            if (windowsDictionary.ContainsKey(typeof(T)))
            {
                return (T)windowsDictionary[typeof(T)];
            }

            return null;
        }

        public BaseWindow GetWindow(Type t)
        {
            return t == null ? null : windowsDictionary.GetValueOrDefault(t);
        }

        public void SetTopOrder(BaseWindow window)
        {
            if (window == null)
            {
#if UNITY_EDITOR
                Debug.LogError("window is null");
#endif
                return;
            }

            int maxOrder = -1;
            for (int i = 0; i < windowContainers.Count; i++)
            {
                if (windowContainers[i].Order > maxOrder)
                {
                    maxOrder = windowContainers[i].Order;
                }
            }

            window.Order = maxOrder + 1;
            NormalizeOrders();
        }

        public void SetTopOrder<T>() where T : BaseWindow
        {
            T window = GetWindow<T>();
            SetTopOrder(window);
        }

        public void NormalizeOrders()
        {
            if (windowContainers.Count == 0)
            {
                return;
            }

            windowContainers.Sort(CompareWindowsByOrder);

            for (int i = 0; i < windowContainers.Count; i++)
            {
                windowContainers[i].Order = i;
            }
        }

        private static int CompareWindowsByOrder(BaseWindow x, BaseWindow y)
        {
            if (ReferenceEquals(x, y))
            {
                return 0;
            }
            if (ReferenceEquals(x, null))
            {
                return -1;
            }
            if (ReferenceEquals(y, null))
            {
                return 1;
            }
            return x.Order.CompareTo(y.Order);
        }

        public void SetLowestOrder(BaseWindow window)
        {
            window.Order = -1;
        }
    }
}
#endif