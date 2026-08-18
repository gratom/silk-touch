using System.Collections.Generic;
using UnityEngine;

namespace Tools.InGameCurveComponents
{
    public class CurveEditor : MonoBehaviour
    {
        #region Inspector Variables

#pragma warning disable
        [SerializeField] private List<Point> points = new List<Point>();
        [SerializeField] private AnimationCurve curve;
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private int segments = 100;

        [SerializeField] private Vector2 curveMin = new Vector2(0, -10);
        [SerializeField] private Vector2 curveMax = new Vector2(1, 10);
        [SerializeField] private Vector2 factMin = new Vector2(-500, -500);
        [SerializeField] private Vector2 factMax = new Vector2(500, 500);
#pragma warning restore

        #endregion Inspector Variables

        #region Properties

        public AnimationCurve Curve => curve;

        private Vector3[] positions;

        #endregion Properties

        private Keyframe[] keyframes
        {
            get => curve.keys;
            set => curve.keys = value;
        }

        private float curveScale;
        private float factScale;

        #region Unity Methods

        private void Start()
        {
            positions = new Vector3[segments];
            curveScale = (curveMax.y - curveMin.y) / (curveMax.x - curveMin.x);
            factScale = (factMax.x - factMin.x) / (factMax.y - factMin.y);
            FirstSet();
            lineRenderer.positionCount = segments;
        }

        private void FixedUpdate()
        {
            Keyframe[] keys = new Keyframe[points.Count];

            for (int i = 0; i < points.Count; i++)
            {
                keys[i].time = Mathf.Lerp(curveMin.x, curveMax.x, Mathf.InverseLerp(factMin.x, factMax.x, points[i].Position.x));
                keys[i].value = Mathf.Lerp(curveMin.y, curveMax.y, Mathf.InverseLerp(factMin.y, factMax.y, points[i].Position.y));
                keys[i].inTangent = keys[i].outTangent = points[i].Angle * curveScale * factScale;
            }

            keyframes = keys;

            float koef = (curveMax.x - curveMin.x) / segments;
            for (int i = 0; i < positions.Length; i++)
            {
                positions[i].x = Mathf.Lerp(factMin.x, factMax.x, Mathf.InverseLerp(0, segments, i));
                positions[i].y = Mathf.Lerp(factMin.y, factMax.y, Mathf.InverseLerp(curveMin.y, curveMax.y, curve.Evaluate(curveMin.x + koef * i)));
            }
            positions[positions.Length - 1].x = factMax.x;
            positions[positions.Length - 1].y = Mathf.Lerp(factMin.y, factMax.y, Mathf.InverseLerp(curveMin.y, curveMax.y, curve.Evaluate(curveMax.x)));
            lineRenderer.SetPositions(positions);
        }

        #endregion Unity Methods

        #region private functions

        private void FirstSet()
        {
            points[0].Position = new Vector2(points[0].Position.x, Mathf.Lerp(factMin.y, factMax.y, Mathf.InverseLerp(curveMin.y, curveMax.y, curve.Evaluate(curveMin.x))));
            points[0].Angle = keyframes[0].outTangent / (curveScale * factScale);
            points[1].Position = new Vector2(points[1].Position.x, Mathf.Lerp(factMin.y, factMax.y, Mathf.InverseLerp(curveMin.y, curveMax.y, keyframes[1].value)));
            points[1].Angle = keyframes[1].inTangent / (curveScale * factScale);
            points[2].Position = new Vector2(points[2].Position.x, Mathf.Lerp(factMin.y, factMax.y, Mathf.InverseLerp(curveMin.y, curveMax.y, curve.Evaluate(curveMax.x))));
            points[2].Angle = keyframes[2].inTangent / (curveScale * factScale);
        }

        #endregion private functions
    }
}