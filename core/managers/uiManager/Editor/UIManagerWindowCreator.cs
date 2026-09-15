#if UNITY_EDITOR && UI_TMP
using UnityEngine;
using UnityEditor;

namespace SilkTouch.UI
{
    [InitializeOnLoad]
    public static class UIManagerWindowCreator
    {
        static UIManagerWindowCreator()
        {
#if UNITY_6000_6_OR_NEWER
            EditorApplication.hierarchyWindowItemByEntityIdOnGUI += OnHierarchyGUI;
#else
            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI;
#endif
        }

        private static void OnHierarchyGUI(
#if UNITY_6000_6_OR_NEWER
            EntityId entityID
#else
            int instanceID
#endif
            , Rect selectionRect)
        {
            Event currentEvent = Event.current;

            if (currentEvent.type == EventType.DragUpdated || currentEvent.type == EventType.DragPerform)
            {
                if (selectionRect.Contains(currentEvent.mousePosition))
                {
#if UNITY_6000_6_OR_NEWER
                    GameObject targetGO = EditorUtility.EntityIdToObject(entityID) as GameObject;
#else
                    GameObject targetGO = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
#endif
                    if (targetGO != null && targetGO.GetComponent<UIManager>() != null && ContainsBaseWindowScript())
                    {
                        ProcessDrag(currentEvent, targetGO);
                    }
                }
            }
        }

        private static bool ContainsBaseWindowScript()
        {
            if (DragAndDrop.objectReferences.Length == 0)
            {
                return false;
            }

            for (int i = 0; i < DragAndDrop.objectReferences.Length; i++)
            {
                if (DragAndDrop.objectReferences[i] is MonoScript script)
                {
                    System.Type scriptType = script.GetClass();

                    if (scriptType != null && scriptType.IsSubclassOf(typeof(BaseWindow)))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static void ProcessDrag(Event currentEvent, GameObject targetGO)
        {
            DragAndDrop.visualMode = DragAndDropVisualMode.Link;

            if (currentEvent.type == EventType.DragPerform)
            {
                DragAndDrop.AcceptDrag();

                for (int i = 0; i < DragAndDrop.objectReferences.Length; i++)
                {
                    if (DragAndDrop.objectReferences[i] is MonoScript script)
                    {
                        System.Type scriptType = script.GetClass();
                        if (scriptType != null && scriptType.IsSubclassOf(typeof(BaseWindow)))
                        {
                            Debug.Log($"[Hierarchy Drop] Скрипт {script.name} успешно сброшен на {targetGO.name} с UIManager!");
                            TryAddNewWindow(targetGO, script);
                        }
                    }
                }

                currentEvent.Use();
            }
        }

        private static void TryAddNewWindow(GameObject targetGo, MonoScript script)
        {
            UIManager uiManager = targetGo.GetComponent<UIManager>();
            if (uiManager == null)
            {
                Debug.LogError("Can not add script to UIManager, not found UIManager by some reason");
                return;
            }

            System.Type scriptType = script.GetClass();
            if (scriptType == null)
            {
                Debug.LogError("Can not add script to UIManager, script is null by some reason");
                return;
            }

            SerializedObject serializedManager = new SerializedObject(uiManager);
            SerializedProperty listProperty = serializedManager.FindProperty("windowContainers");

            if (listProperty == null)
            {
                Debug.LogError("Not found field [windowContainers] in UIManager. Check the field name.");
                return;
            }

            bool alreadyExists = false;
            for (int i = 0; i < listProperty.arraySize; i++)
            {
                SerializedProperty element = listProperty.GetArrayElementAtIndex(i);
                if (element.objectReferenceValue != null && element.objectReferenceValue.GetType() == scriptType)
                {
                    alreadyExists = true;
                    break;
                }
            }

            if (alreadyExists)
            {
                Debug.LogWarning($"The window of type {scriptType.Name} already registered in UIManager!");
                return;
            }

            Undo.IncrementCurrentGroup();
            int undoGroup = Undo.GetCurrentGroup();

            GameObject newWindowGo = new GameObject(script.name);
            Undo.RegisterCreatedObjectUndo(newWindowGo, "Create New Window GameObject");
            Undo.SetTransformParent(newWindowGo.transform, targetGo.transform, "Parent Window to UIManager");

            newWindowGo.transform.localPosition = Vector3.zero;
            newWindowGo.transform.localRotation = Quaternion.identity;
            newWindowGo.transform.localScale = Vector3.one;

            Component windowComponent = Undo.AddComponent(newWindowGo, scriptType);
            if (windowComponent is BaseWindow baseWindow)
            {
                serializedManager.Update();
                int newIndex = listProperty.arraySize;
                listProperty.InsertArrayElementAtIndex(newIndex);
                SerializedProperty newElement = listProperty.GetArrayElementAtIndex(newIndex);
                newElement.objectReferenceValue = baseWindow;
                serializedManager.ApplyModifiedProperties();
                Selection.activeGameObject = newWindowGo;
            }

            Undo.CollapseUndoOperations(undoGroup);
        }
    }
}
#endif