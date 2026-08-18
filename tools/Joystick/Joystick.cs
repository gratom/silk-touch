using Tools;
using UnityEngine;

namespace Tools
{
    public class Joystick : MonoBehaviour
    {
        [SerializeField] private RectComponent arrow;
        [SerializeField] private TouchPanelInput input;

        [SerializeField] private float minSize = 30;
        [SerializeField] private float maxSize = 200;

        public Vector2 Value => value;
        private Vector2 value;

        private void Awake()
        {
            input.OnTouchStart += _ => arrow.gameObject.SetActive(true);
            input.OnTouching += Touch;
            input.OnTouchEnd += EndTouch;
        }

        private void Touch(Vector2 pos, float force)
        {
            float rot = Mathf.Atan2(pos.y, pos.x) * Mathf.Rad2Deg;
            arrow.RectTransform.rotation = Quaternion.Euler(new Vector3(0, 0, rot));

            value = pos * force;
            arrow.Width = Mathf.Clamp(maxSize * value.magnitude, minSize, maxSize);
        }

        private void EndTouch(Vector2 pos)
        {
            arrow.RectTransform.rotation = Quaternion.identity;
            arrow.gameObject.SetActive(false);
            value = Vector2.zero;
        }
    }
}