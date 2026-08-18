#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Tools
{
    public class HotkeySubscription
    {
        public KeyCode Key { get; }
        public Action Callback { get; }
        public double LastPressTime { get; set; }

        // Флаг, контролирующий, что между нажатиями был физический подъем клавиши
        public bool IsKeyReleased { get; set; } = true;

        public HotkeySubscription(KeyCode key, Action callback)
        {
            Key = key;
            Callback = callback;
        }
    }

    [InitializeOnLoad]
    public static class CustomDoubleClickHotkeysTrigger
    {
        private static List<HotkeySubscription> subscriptions = new List<HotkeySubscription>();
        private static float doubleClickTimeMax = 0.25f;

        static CustomDoubleClickHotkeysTrigger()
        {
            FieldInfo field = typeof(EditorApplication).GetField("globalEventHandler", BindingFlags.Static | BindingFlags.NonPublic);
            if (field != null)
            {
                EditorApplication.CallbackFunction callback = (EditorApplication.CallbackFunction)field.GetValue(null);
                callback += CheckEditorKeyPress;
                field.SetValue(null, callback);
            }
        }

        public static void Register(KeyCode key, Action callback)
        {
            subscriptions.Add(new HotkeySubscription(key, callback));
        }

        private static void CheckEditorKeyPress()
        {
            Event currentEvent = Event.current;
            if (currentEvent == null || !currentEvent.isKey)
            {
                return;
            }

            double currentTime = EditorApplication.timeSinceStartup;

            for (int i = 0; i < subscriptions.Count; i++)
            {
                HotkeySubscription sub = subscriptions[i];

                if (currentEvent.keyCode != sub.Key)
                {
                    continue;
                }

                // 1. Если клавишу отпустили, фиксируем это состояние
                if (currentEvent.type == EventType.KeyUp)
                {
                    sub.IsKeyReleased = true;
                    continue;
                }

                // 2. Если это нажатие, проверяем логику двойного клика
                if (currentEvent.type == EventType.KeyDown)
                {
                    // Если это зажатие (автоповтор) без предварительного KeyUp, то просто игнорируем его
                    if (!sub.IsKeyReleased)
                    {
                        continue;
                    }

                    if (currentTime - sub.LastPressTime < doubleClickTimeMax)
                    {
                        sub.LastPressTime = 0;
                        sub.IsKeyReleased = false; // Сбрасываем, чтобы текущее нажатие не дублировалось
                        sub.Callback?.Invoke();
                        currentEvent.Use();
                    }
                    else
                    {
                        sub.LastPressTime = currentTime;
                        sub.IsKeyReleased = false; // Кнопка нажата, теперь ждем её поднятия (KeyUp)
                    }
                }
            }
        }
    }
}
#endif