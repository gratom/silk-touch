using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Tools.InGameCurveComponents
{
    public class Drag : EventTrigger
    {
        #region Properties

        [HideInInspector] public event Action<Vector2> OnPositionChanged;

        #endregion Properties

        #region Public Methods

        public override void OnDrag(PointerEventData eventData)
        {
            SetPosition(eventData);
        }

        public void SetPosition(PointerEventData data)
        {
            transform.localPosition += new Vector3(data.delta.x, data.delta.y, 0);
            OnPositionChanged?.Invoke(transform.localPosition);
        }

        #endregion Public Methods
    }
}