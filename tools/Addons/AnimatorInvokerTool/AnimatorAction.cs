using System;
using UnityEngine;

namespace Tools.Anim
{
    [Serializable]
    public class AnimatorAction
    {
#pragma warning disable

        [SerializeField] private AnimatorValue value;
        [SerializeField] private float delayTime;
        [SerializeField] private AnimatorActionType actionType;
        [SerializeField] private AnimatorControllerParameterTypeFunc parameterTypeFunc;

#pragma warning restore

        public AnimatorValue Value => value;
        public float DelayTime => delayTime;
        public AnimatorActionType ActionType => actionType;
        public AnimatorControllerParameterTypeFunc ParameterTypeFunc => parameterTypeFunc;
    }
}