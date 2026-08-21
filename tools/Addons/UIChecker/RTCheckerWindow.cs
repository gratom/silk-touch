#if UNITY_EDITOR && UI_TMP

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Tools.UIChecker
{
    public class RTCheckerWindow : EditorWindow
    {
        private Vector2 scroll;

        private void OnDestroy()
        {
            RaycastTargetChecker.ProcessOpened(true);
        }

        public static bool IsOpen => HasOpenInstances<RTCheckerWindow>();

        private void OnGUI()
        {
            GUILayout.Label("Graphics with potentially unnecessary enabled raycast:", EditorStyles.boldLabel);
            if (GUILayout.Button("Fix All"))
            {
                foreach (Graphic g in RaycastTargetChecker.potentiallyProblematic)
                {
                    if (g != null)
                    {
                        g.raycastTarget = false;
                        EditorUtility.SetDirty(g);
                    }
                }
                RaycastTargetChecker.potentiallyProblematic.Clear();
                Debug.Log("[RTCheck] Fixed all raycastTarget mismatches.".AsEditorEvent());
            }

            scroll = GUILayout.BeginScrollView(scroll);
            foreach (Graphic g in RaycastTargetChecker.potentiallyProblematic)
            {
                if (g == null || g.raycastTarget == false)
                {
                    continue;
                }
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Select", GUILayout.Width(60)))
                {
                    Selection.activeObject = g.gameObject;
                    EditorGUIUtility.PingObject(g);
                }
                if (GUILayout.Button("Fix", GUILayout.Width(30)))
                {
                    Selection.activeObject = g.gameObject;
                    g.raycastTarget = false;
                    EditorUtility.SetDirty(g);
                    Debug.Log($"[RTCheck] Fixed {g.name}".AsEditorEvent());
                }
                EditorGUILayout.ObjectField(g.name, g, typeof(Graphic), true);
                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
        }
    }
}

#endif