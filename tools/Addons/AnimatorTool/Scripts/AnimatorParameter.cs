using UnityEngine;
using System;

namespace Tools.Anim
{
    /// <summary>
    /// Tool for convenient management of animator parameters
    /// </summary>
    [Serializable]
    public class AnimatorParameter
    {
#pragma warning disable
        [SerializeField] private int hash;
        [SerializeField] private string name;
        [SerializeField] private AnimatorControllerParameterType parameterType = AnimatorControllerParameterType.Bool;
        [SerializeField] private Animator animator;
#pragma warning restore

        /// <summary>
        /// Equivalent to <a href="https://docs.unity3d.com/ScriptReference/AnimatorControllerParameter-nameHash.html">this</a> parameter
        /// in the standard <a href="https://docs.unity3d.com/ScriptReference/AnimatorControllerParameter.html">AnimatorControllerParameter</a> module
        /// </summary>
        public int Hash => hash;

        /// <summary>
        /// Equivalent to <a href="https://docs.unity3d.com/ScriptReference/AnimatorControllerParameter-name.html">this</a> parameter
        /// in the standard <a href="https://docs.unity3d.com/ScriptReference/AnimatorControllerParameter.html">AnimatorControllerParameter</a> module
        /// </summary>
        public string Name => name;

        /// <summary>
        /// Equivalent to <a href="https://docs.unity3d.com/ScriptReference/AnimatorControllerParameter-type.html">this</a> parameter
        /// in the standard <a href="https://docs.unity3d.com/ScriptReference/AnimatorControllerParameter.html">AnimatorControllerParameter</a> unit
        /// </summary>
        public AnimatorControllerParameterType ParameterType => parameterType;
    }
}