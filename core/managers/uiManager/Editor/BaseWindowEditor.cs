#if UNITY_EDITOR && UI_TMP
using UnityEditor;
using UnityEngine;

namespace SilkTouch.UI
{
    using Crystal;
    using Tools;
    
    [CustomEditor(typeof(BaseWindow), true)]
    public class BaseWindowEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            Rect rect = GUILayoutUtility.GetRect(0, 0);

            Rect buttonRect = new Rect(rect.x - 17, rect.y + 1, 18, 15);
            GUIStyle buttonStyle = new GUIStyle(EditorStyles.miniButton);

            buttonStyle.padding = new RectOffset(0, 0, 0, 0);
            buttonStyle.fontSize = 14;
            buttonStyle.contentOffset = new Vector2(0, -1);

            if (GUI.Button(buttonRect, "⛶", buttonStyle))
            {
                AddChildWithComponent();
            }

            DrawDefaultInspector();
        }

        private void AddChildWithComponent()
        {
            BaseWindow window = (BaseWindow)target;
            GameObject child = new GameObject("backgroundWithSafeArea");
            child.transform.SetParent(window.transform, false);
            RectTransform rectTransform = child.AddComponent<RectTransform>();
            rectTransform.SetAnchorsPreset(RectComponent.PresetType.StretchAll);
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
            child.AddComponent<SafeArea>();
            Undo.RegisterCreatedObjectUndo(child, "Create Child Object");
            Selection.activeGameObject = child;
        }
    }
}
#endif