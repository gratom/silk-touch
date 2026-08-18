using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace Tools
{
    [RequireComponent(typeof(Button))]
    public abstract class ButtonContainer : MonoBehaviour
    {
        [SerializeField][HideInInspector][NotNull] protected Button button;

        protected void OnValidate()
        {
            if (GetComponent<Button>() == null)
            {
                gameObject.AddComponent<Button>();
            }

            button = GetComponent<Button>();
            if (button.targetGraphic == null)
            {
                if (GetComponent<Image>() == null)
                {
                    gameObject.AddComponent<Image>();
                }

                button.targetGraphic = GetComponent<Image>();
            }

            if (button == null)
            {
                Debug.LogError("button container is null!");
            }
        }

        private void Awake() { button.onClick.AddListener(OnClickFunction); }

        protected abstract void OnClickFunction();
    }

}