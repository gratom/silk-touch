#if UNITY_EDITOR

using System;
using UnityEditor;
using UnityEngine;

namespace SilkTouch.Managers.Data
{
    [CustomEditor(typeof(StaticData))]
    public class StaticDataEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            try
            {
                base.OnInspectorGUI();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
    }

}

#endif