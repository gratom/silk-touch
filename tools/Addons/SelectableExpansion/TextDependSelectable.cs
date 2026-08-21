using System;
using UnityEngine;
using UnityEngine.UI;

namespace Tools
{
    [RequireComponent(typeof(Text))]
    public class TextDependSelectable : MonoBehaviour
    {
        [SerializeField] private Text text;
        [SerializeField] private SelectableExpansion selectable;

        [SerializeField] private Color normal;
        [SerializeField] private Color highlighted;
        [SerializeField] private Color pressed;
        [SerializeField] private Color selected;
        [SerializeField] private Color disabled;

        private void OnValidate()
        {
            if (text == null)
            {
                text = GetComponent<Text>();
            }
            if (selectable == null)
            {
                selectable = transform.parent.GetComponent<SelectableExpansion>();
            }
            if (selectable != null)
            {
                text.color = selectable.interactable ? normal : disabled;
            }
        }

        private void Start()
        {
            selectable.ONStateChange += ChangeDependsColor;
            text.color = selectable.interactable ? normal : disabled;
        }

        private void ChangeDependsColor(SelectableExpansion.SelectableState selectableState)
        {
            text.color = selectableState switch
            {
                SelectableExpansion.SelectableState.Normal => normal,
                SelectableExpansion.SelectableState.Highlighted => highlighted,
                SelectableExpansion.SelectableState.Pressed => pressed,
                SelectableExpansion.SelectableState.Selected => selected,
                SelectableExpansion.SelectableState.Disabled => disabled,
                _ => Color.black
            };
        }
    }
}