using UnityEngine;

namespace Tools
{
    public interface ISwitch
    {
        void On();
        void Off();
        void Switch();
        bool IsOn { get; set; }
    }

    public class SwitchGameObject : MonoBehaviour, ISwitch
    {
        public void On()
        {
            gameObject.SetActive(true);
        }

        public void Off()
        {
            gameObject.SetActive(false);
        }

        public void Switch()
        {
            IsOn = !IsOn;
        }

        public bool IsOn
        {
            get => gameObject.activeSelf;
            set => gameObject.SetActive(value);
        }
    }
}