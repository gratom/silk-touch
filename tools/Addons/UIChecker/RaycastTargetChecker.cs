#if UNITY_EDITOR && UI_TMP

using System;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq;

namespace Tools.UIChecker
{
    [InitializeOnLoad]
    public static class RaycastTargetChecker
    {
        #region editor stuff

        [MenuItem("GameObject/Disable all raycastTargets here", false, 0)]
        private static void DisableAllRaycastTargets()
        {
            GameObject selected = Selection.activeGameObject;
            if (selected == null)
            {
                return;
            }

            Graphic[] graphics = selected.GetComponentsInChildren<Graphic>(true);
            foreach (Graphic g in graphics)
            {
                if (g.raycastTarget)
                {
                    g.raycastTarget = false;
                    EditorUtility.SetDirty(g);
                }
            }
            Debug.Log("[RTCheck] Disabled raycastTargets on " + graphics.Length + " elements.");
        }

        [MenuItem("GameObject/Disable all raycastTargets here", true)]
        private static bool ValidateDisableAllRaycastTargets()
        {
            GameObject selected = Selection.activeGameObject;
            return selected != null && selected.GetComponentInChildren<Graphic>(true) != null;
        }

        [MenuItem("Tools/Editors/Open RT Checker", false, 1000)]
        [MenuItem("GameObject/Open RT Checker", false, 0)]
        private static void OpenCheckerWindow()
        {
            EditorWindow.GetWindow<RTCheckerWindow>(false, "RT Checker");
        }

        [MenuItem("Tools/Editors/Open RT Checker", true)]
        private static bool ValidateOpenCheckerWindowFromEditorTab()
        {
            return true;
        }

        [MenuItem("GameObject/Open RT Checker", true)]
        private static bool ValidateOpenCheckerWindow()
        {
            return potentiallyProblematic.Count > 0;
        }

        #endregion

        public static HashSet<Graphic> potentiallyProblematic = new HashSet<Graphic>();

        static RaycastTargetChecker()
        {
            EditorSceneManager.sceneSaving += OnSceneSaving;
            PrefabStage.prefabSaving += OnPrefabSaving;
            PrefabStage.prefabStageOpened += OnPrefabOpen;
            EditorApplication.focusChanged += b =>
            {
                if (b)
                {
                    ProcessOpened();
                }
            };
        }

        private static void OnSceneSaving(UnityEngine.SceneManagement.Scene scene, string path)
        {
            ProcessOpened();
        }

        private static void OnPrefabSaving(GameObject prefab)
        {
            ProcessOpened();
        }

        private static void OnPrefabOpen(PrefabStage obj)
        {
            ProcessOpened();
        }

        public static void ProcessOpened(bool ignoreOpenedEditor = false)
        {
            if (RTCheckerWindow.IsOpen && !ignoreOpenedEditor)
            {
                return;
            }

            HashSet<Graphic> tempBackup = potentiallyProblematic.Copy();
            potentiallyProblematic.Clear();

            foreach (GameObject root in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
            {
                ProcessRoot(root.transform);
            }
            if (PrefabStageUtility.GetCurrentPrefabStage() is PrefabStage stage && stage.prefabContentsRoot != null)
            {
                ProcessRoot(stage.prefabContentsRoot.transform);
            }

            if (potentiallyProblematic.Count == 0)
            {
                potentiallyProblematic = tempBackup;
            }
        }

        private static void ProcessRoot(Transform root)
        {
            if (RTCheckerWindow.IsOpen)
            {
                return;
            }
            Graphic[] allGraphics = root.GetComponentsInChildren<Graphic>(true);

            foreach (Graphic graphic in allGraphics.Where(g => !HasAnyEventHandler(g.gameObject)))
            {
                bool isTarget = graphic.raycastTarget;
                bool hasTag = graphic.name.EndsWith("(-RT⚠️)");

                if (isTarget && !hasTag)
                {
                    graphic.name += "(-RT⚠️)";
                    EditorUtility.SetDirty(graphic.gameObject);
                }
                else if (!isTarget && hasTag)
                {
                    graphic.name = graphic.name.Replace("(-RT⚠️)", "").Trim();
                    EditorUtility.SetDirty(graphic.gameObject);
                }

                if (isTarget)
                {
                    potentiallyProblematic.Add(graphic);
                }
            }
            if (potentiallyProblematic.Count > 0)
            {
                RTCheckerOverlay.RTButton.UpdateCount();

                //Debug.Log($"[RTCheck] Marked as non-raycast:\n{ string.Join("\n", potentiallyProblematic)}".AsEditorEvent());
            }
        }

        private static bool HasAnyEventHandler(GameObject go)
        {
            return go.GetComponent<Button>() != null ||
                   go.GetComponent<Toggle>() != null ||
                   go.GetComponent<Slider>() != null ||
                   go.GetComponent<Scrollbar>() != null ||
                   go.GetComponent<Dropdown>() != null ||
                   go.GetComponent<InputField>() != null ||
                   go.GetComponent<TMP_InputField>() != null ||
                   go.GetComponent<EventTrigger>() != null ||
                   HasInterface<IEventSystemHandler>(go);
        }

        private static bool HasInterface<T>(GameObject go)
        {
            return go.GetComponents<MonoBehaviour>().OfType<T>().Any();
        }
    }

}
#endif