using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tools.Anim
{
    [Serializable]
    public class AnimatorValue
    {
        private static readonly Dictionary<AnimatorControllerParameterType, Func<AnimatorValue, object>> CaseDictionary =
            new Dictionary<AnimatorControllerParameterType, Func<AnimatorValue, object>>()
            {
                { AnimatorControllerParameterType.Bool, value => value.boolValue },
                { AnimatorControllerParameterType.Float, value => value.floatValue },
                { AnimatorControllerParameterType.Int, value => value.intValue },
                { AnimatorControllerParameterType.Trigger, value => null }
            };

#pragma warning disable

        [SerializeField] private AnimatorParameter parameter;
        [SerializeField] private bool boolValue;
        [SerializeField] private int intValue;
        [SerializeField] private float floatValue;

#pragma warning restore

        public object Value => CaseDictionary[parameter.ParameterType](this);
        public AnimatorParameter Parameter => parameter;
    }
}