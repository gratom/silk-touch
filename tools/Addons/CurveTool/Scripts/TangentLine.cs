using UnityEngine;

namespace Tools.InGameCurveComponents
{
    public class TangentLine : MonoBehaviour
    {
        public float Angle
        {
            get => angle;
            set
            {
                angle = value;
                transform.localEulerAngles = new Vector3(0, 0, Mathf.Atan(angle) * Mathf.Rad2Deg);
            }
        }

        private float angle;
    }
}