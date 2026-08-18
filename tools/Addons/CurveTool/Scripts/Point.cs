using UnityEngine;

namespace Tools.InGameCurveComponents
{
    public class Point : MonoBehaviour
    {
        #region Inspector Variables

#pragma warning disable
        [SerializeField] private Tangent handleIn;
        [SerializeField] private Tangent handleOut;
        [SerializeField] private TangentLine line;
        [SerializeField][HideInInspector] private RectTransform rectTransform;
#pragma warning restore

        #endregion Inspector Variables

        #region Properties

        public Vector2 Position
        {
            get => rectTransform.localPosition;
            set => rectTransform.localPosition = value;
        }

        public float Angle
        {
            get => line.Angle;
            set
            {
                handleIn.SetTangent(value);
                handleOut.SetTangent(value);
                line.Angle = value;
            }
        }

        #endregion Properties

        #region Unity Methods

        private void OnValidate()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        private void Start()
        {
            handleIn.Drag.OnPositionChanged += handleOut.Mirror;
            handleOut.Drag.OnPositionChanged += handleIn.Mirror;
            handleIn.Drag.OnPositionChanged += (position) => line.Angle = handleIn.GetTangent();
            handleOut.Drag.OnPositionChanged += (position) => line.Angle = handleOut.GetTangent();
        }

        #endregion Unity Methods
    }
}