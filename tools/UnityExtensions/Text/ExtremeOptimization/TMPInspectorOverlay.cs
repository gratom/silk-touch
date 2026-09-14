#if UNITY_EDITOR && UI_TMP
using UnityEngine;
using UnityEditor;
using TMPro;

namespace SilkTouch.Tools
{
    [InitializeOnLoad]
    public static class TMPInspectorOverlay
    {
        static TMPInspectorOverlay()
        {
            Editor.finishedDefaultHeaderGUI += DrawButton;
        }

        private static void DrawButton(Editor editor)
        {
            GameObject go = editor.target as GameObject;
            if (go == null)
            {
                return;
            }

            TMP_Text txt = go.GetComponent<TMP_Text>();
            if (txt == null)
            {
                return;
            }

            EditorGUILayout.Space(2);
            if (GUILayout.Button("Open in Advanced Editor", GUILayout.Height(25)))
            {
                OptimizedTextWindow.Open(txt);
            }
            EditorGUILayout.Space(2);
        }
    }
}
#endif