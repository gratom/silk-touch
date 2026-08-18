using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tools.InGameCurveComponents
{
    public class ClampPosition : MonoBehaviour
    {
        [Serializable]
        public class ClampDatas
        {
            public Vector2 min;
            public Vector2 max;
            public float radius;

            public ClampDatas(Vector2 min, Vector2 max)
            {
                this.min = min;
                this.max = max;
                radius = 0;
            }

            public ClampDatas(float radius)
            {
                min = Vector2.zero;
                max = Vector2.zero;
                this.radius = radius;
            }
        }

        public enum ClampType
        {
            borders,
            radius
        }

        #region Inspector Variables

#pragma warning disable
        [SerializeField] private ClampType clampType;
        [SerializeField] private ClampDatas clampDatas;
#pragma warning restore

        #endregion Inspector Variables

        public ClampDatas clampData => clampDatas;

        private static Dictionary<ClampType, Action<RectTransform, ClampDatas>> actionsDictionary = new Dictionary<ClampType, Action<RectTransform, ClampDatas>>()
        {
            { ClampType.borders, ClampByBorders },
            { ClampType.radius, ClampByRadius }
        };

        private RectTransform rectTransform;

        #region Unity Methods

        private void Start()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        private void Update()
        {
            actionsDictionary[clampType](rectTransform, clampDatas);
        }

        #endregion Unity Methods

        #region private static functions

        private static void ClampByRadius(RectTransform rectTransform, ClampDatas clampData)
        {
            rectTransform.SetPosOnRadiusLocal(clampData.radius);
        }

        private static void ClampByBorders(RectTransform rectTransform, ClampDatas clampData)
        {
            rectTransform.TruncateLocalPositionMin(clampData.min);
            rectTransform.TruncateLocalPositionMax(clampData.max);
        }

        #endregion private static functions
    }
}