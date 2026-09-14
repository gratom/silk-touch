#if UNITY_EDITOR && UI_TMP

using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using UnityEditor.Toolbars;

namespace SilkTouch.Tools.UIChecker
{
    [InitializeOnLoad]
    [Overlay(typeof(SceneView), "RT Checker Overlay", defaultDisplay = true)]
    public class RTCheckerOverlay : ToolbarOverlay
    {
        protected RTCheckerOverlay() : base(RTButton.s_ElementName) { }

        [EditorToolbarElement(s_ElementName, typeof(SceneView))]
        public class RTButton : Button
        {
            public const string s_ElementName = "RTCheckSceneButton";
            private static RTButton s_Instance;

            private static readonly StyleLength s_FixedWidth = new Length(110, LengthUnit.Pixel);

            public RTButton()
            {
                s_Instance = this;
                style.width = s_FixedWidth;
                UpdateCount();
                clicked += OpenWindow;
            }

            private void OpenWindow()
            {
                EditorWindow.GetWindow<RTCheckerWindow>(false, "RT Checker");
            }

            public static void UpdateCount()
            {
                if (s_Instance == null)
                {
                    return;
                }

                int count = RaycastTargetChecker.potentiallyProblematic.Count;
                s_Instance.text = $"RTcheck-{count}";
            }
        }
    }

}
#endif