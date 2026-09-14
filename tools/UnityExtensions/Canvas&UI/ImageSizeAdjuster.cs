#if UNITY_EDITOR && UI_TMP
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Tools
{
    [CustomEditor(typeof(Image))]
    [CanEditMultipleObjects]
    public class CustomImageEditor : ImageEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            Image image = (Image)target;
            if (image.sprite == null)
            {
                return;
            }

            EditorGUILayout.Space(5);

            float currentWidth = image.rectTransform.rect.width;
            float nativeWidth = image.sprite.rect.width;
            float currentScale = nativeWidth > 0 ? currentWidth / nativeWidth : 1f;

            EditorGUI.BeginChangeCheck();
            float newScale = EditorGUILayoutExtensions.LogSlider("Aspect scaler", currentScale, 0.1f, 10f);

            if (EditorGUI.EndChangeCheck())
            {
                ScaleSprite(newScale);
            }
        }

        private void ScaleSprite(float factor)
        {
            for (int i = 0; i < targets.Length; i++)
            {
                Image image = targets[i] as Image;
                if (image == null || image.sprite == null)
                {
                    continue;
                }

                Undo.RecordObject(image.rectTransform, "Resize Image");

                Vector2 newSize = image.sprite.rect.size * factor;
                GridLayoutGroup grid = image.GetComponent<GridLayoutGroup>() ?? image.GetComponentInParent<GridLayoutGroup>();

                if (grid != null)
                {
                    Undo.RecordObject(grid, "Resize Grid Cell");
                    grid.cellSize = newSize;
                    EditorUtility.SetDirty(grid);
                }
                else
                {
                    image.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newSize.x);
                    image.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newSize.y);
                    EditorUtility.SetDirty(image.rectTransform);
                }
            }
        }
    }
}
#endif