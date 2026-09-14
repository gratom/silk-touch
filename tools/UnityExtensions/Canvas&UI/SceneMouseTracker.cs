#if UNITY_EDITOR && UI_TMP
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

namespace Tools
{
    [InitializeOnLoad]
    public class SceneMouseTracker
    {
        private static GameObject lastPickedObject;
        private static bool altPressed;

        static SceneMouseTracker()
        {
            SceneView.duringSceneGui += OnSceneGui;
            GlobalKeyEventHandler.OnKeyEvent += GlobalKeyEvent;
        }

        private static void GlobalKeyEvent(Event evn)
        {
            if (evn != null && evn.keyCode == KeyCode.LeftAlt)
            {
                altPressed = evn.alt;
            }
            else
            {
                altPressed = false;
            }
        }

        private static void OnSceneGui(SceneView sceneView)
        {
            Event currentEvent = Event.current;
            if (currentEvent.isScrollWheel)
            {
                TryScroll(currentEvent);
            }

            if (currentEvent.type == EventType.MouseMove)
            {
                GameObject pickedObject = HandleUtility.PickGameObject(currentEvent.mousePosition, false);
                if (pickedObject != null)
                {
                    if (pickedObject.GetComponent<RectTransform>() != null)
                    {
                        lastPickedObject = pickedObject;
                        return;
                    }
                }
                lastPickedObject = null;
            }
        }

        private static void TryScroll(Event evn)
        {
            if (altPressed && lastPickedObject != null)
            {
                PerformAction(evn.delta);
                evn.Use();
            }
        }

        private static void PerformAction(Vector2 evnDelta)
        {

            ScrollRect scrollRect = lastPickedObject.GetComponent<ScrollRect>() ?? lastPickedObject.GetComponentInParent<ScrollRect>() ?? lastPickedObject.GetComponentInChildren<ScrollRect>();

            if (scrollRect == null)
            {
                return;
            }

            float sensitivity = 0.01f;
            float deltaValue = evnDelta.y * sensitivity;

            if (scrollRect.verticalScrollbar != null)
            {
                scrollRect.verticalScrollbar.value = Mathf.Clamp01(scrollRect.verticalScrollbar.value - deltaValue);
            }
            else
            {
                scrollRect.verticalNormalizedPosition = Mathf.Clamp01(scrollRect.verticalNormalizedPosition - deltaValue);
            }
        }
    }
}
#endif