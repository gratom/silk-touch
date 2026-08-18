using UnityEngine;
using UnityEngine.UI;

namespace Tools
{

    [RequireComponent(typeof(Image))]
    public class ImageDependSelectable : MonoBehaviour
    {
        [SerializeField] private Image image;
        [SerializeField] private SelectableExpansion buttonParent;

        [SerializeField] private Color normal;
        [SerializeField] private Color highlighted;
        [SerializeField] private Color pressed;
        [SerializeField] private Color selected;
        [SerializeField] private Color disabled;

        private void OnValidate()
        {
            if (image == null)
            {
                image = GetComponent<Image>();
            }
            if (buttonParent == null)
            {
                buttonParent = transform.parent.GetComponent<SelectableExpansion>();
            }
            if (buttonParent != null)
            {
                image.color = buttonParent.interactable ? normal : disabled;
            }
        }

        private void Start()
        {
            buttonParent.ONStateChange += ChangeDependsColor;
            image.color = buttonParent.interactable ? normal : disabled;
        }

        private void ChangeDependsColor(SelectableExpansion.SelectableState selectableState)
        {
            image.color = selectableState switch
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