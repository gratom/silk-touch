using UnityEngine;

namespace Tools.Img
{
    [RequireComponent(typeof(ImageStateSetter))]
    public class ButtonImageStateSetter : MonoBehaviour
    {
        [ImageStateTag] private const string ACTIVE = "active";
        [ImageStateTag] private const string INACTIVE = "inactive";

        [HideInInspector][SerializeField] private ImageStateSetter imageStateSetter;

        public bool IsActiveState
        {
            set => imageStateSetter.SetState(value ? ACTIVE : INACTIVE);
        }

        private void OnValidate()
        {
            imageStateSetter = GetComponent<ImageStateSetter>();
        }
    }
}