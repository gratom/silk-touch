#if UNITY_EDITOR

using UnityEditor;
using UnityEditor.Overlays;
using UnityEditor.Toolbars;
using UnityEngine.UIElements;

namespace SilkTouch.Tools.Ruler
{
    [Overlay(typeof(SceneView), "Ruler Overlay", defaultDisplay = true)]
    public class RulerOverlay : ToolbarOverlay
    {
        protected RulerOverlay() : base(EnabledToggle.id) { }

        [EditorToolbarElement(id, typeof(SceneView))]
        public class EnabledToggle : EditorToolbarToggle
        {
            public const string id = "RulerEnabledToggle";

            public EnabledToggle()
            {
                text = "Ruler";
                tooltip = "Show/hide measurement lines on selected and always-visible primitives";
                value = RulerTool.Enabled;
                this.RegisterValueChangedCallback(evt =>
                {
                    RulerTool.Enabled = evt.newValue;
                    SceneView.RepaintAll();
                });
            }
        }
    }
}

#endif
