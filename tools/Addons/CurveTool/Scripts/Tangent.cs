using UnityEngine;

namespace Tools.InGameCurveComponents
{
    [RequireComponent(typeof(Drag))]
    public class Tangent : MonoBehaviour
    {
        public enum TangentType
        {
            tIN = 0,
            tOUT = 2
        }

        #region Inspector Variables

#pragma warning disable
        [SerializeField] private Tangent mirrored;
        [SerializeField] private TangentType tangentType;
#pragma warning restore

        #endregion Inspector Variables

        #region Properties

        public Drag Drag { get; private set; }

        private int tangentSign => (int)tangentType - 1;

        #endregion Properties

        #region Unity Methods

        private void Awake()
        {
            Drag = GetComponent<Drag>();
        }

        #endregion Unity Methods

        #region Public Methods

        public float GetTangent()
        {
            return transform.localPosition.y / transform.localPosition.x;
        }

        public void SetTangent(float tangent)
        {
            transform.localPosition = new Vector3(tangentSign, tangent * tangentSign, transform.localPosition.z);
        }

        public void Mirror(Vector2 position)
        {
            transform.localPosition = -position;
        }

        #endregion Public Methods
    }
}