#if UNITY_EDITOR && UI_TMP

using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEditor;
using TMPro;

namespace SilkTouch.Tools
{

    public class OptimizedTextWindow : EditorWindow
    {
        private TMP_Text targetText;
        private string buffer;
        private Vector2 scrollPosition;

        private const float SCROLL_SPEED = 15f;
        private const int CHARS_PER_VIEW = 1000;

        private static GUIStyle textAreaStyle;
        public static void Open(TMP_Text target)
        {
            OptimizedTextWindow window = GetWindow<OptimizedTextWindow>("Text Editor");
            window.targetText = target;
            window.buffer = target.text;

            textAreaStyle = new GUIStyle(EditorStyles.textArea)
            {
                wordWrap = true,
                font = EditorStyles.textField.font
            };
        }

        // TextArea chunk lenght
        private int activeStartIndex = -1;
        private string currentChunk;

        private static bool scrollManuallyUpdated;

        private void OnGUI()
        {
            if (targetText == null)
            {
                Close();
                return;
            }

            Event e = Event.current;
            int maxScroll = Mathf.Max(0, buffer.Length - CHARS_PER_VIEW);

            if (e.type == EventType.ScrollWheel)
            {
                scrollPosition.y += e.delta.y * SCROLL_SPEED;
                scrollPosition.y = Mathf.Clamp(scrollPosition.y, 0, maxScroll);
                GUI.FocusControl(null);
                e.Use();
                scrollManuallyUpdated = true;
            }

            EditorGUILayout.LabelField($"Editing: {targetText.name}", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();

            int targetStartIndex = Mathf.Clamp(Mathf.FloorToInt(scrollPosition.y), 0, maxScroll);

            if (activeStartIndex != targetStartIndex || currentChunk == null)
            {
                activeStartIndex = targetStartIndex;
                int length = Mathf.Min(CHARS_PER_VIEW, buffer.Length - activeStartIndex);
                currentChunk = buffer.Substring(activeStartIndex, length);
                scrollManuallyUpdated = false;
            }

            string newChunk = EditorGUILayout.TextArea(currentChunk, textAreaStyle, GUILayout.Height(position.height - 100), GUILayout.ExpandWidth(true));

            if (newChunk != currentChunk)
            {
                buffer = buffer.Remove(activeStartIndex, currentChunk.Length).Insert(activeStartIndex, newChunk);
                currentChunk = newChunk;
                TryPushChanges();
            }

            float newY = GUILayout.VerticalScrollbar(scrollPosition.y, CHARS_PER_VIEW, 0, buffer.Length, GUILayout.Height(position.height - 100));
            if (!Mathf.Approximately(newY, scrollPosition.y))
            {
                scrollPosition.y = newY;
                scrollManuallyUpdated = true;
            }

            EditorGUILayout.EndHorizontal();
        }

        private bool isPushScheduled;
        private DateTime timeToPushChanges;

        private void TryPushChanges()
        {
            timeToPushChanges = DateTime.Now.AddSeconds(0.8);
            if (!isPushScheduled)
            {
                isPushScheduled = true;
                RunPushLoop().Forget();
            }
        }

        private async UniTaskVoid RunPushLoop()
        {
            while (DateTime.Now < timeToPushChanges)
            {
                TimeSpan waitTime = timeToPushChanges - DateTime.Now;

                if (waitTime > TimeSpan.Zero)
                {
                    await UniTask.Delay(waitTime, true);
                }
            }

            if (targetText != null)
            {
                Undo.RecordObject(targetText, "Update TMP Text");
                targetText.text = buffer;
                EditorUtility.SetDirty(targetText);
            }

            isPushScheduled = false;
        }
    }
}
#endif