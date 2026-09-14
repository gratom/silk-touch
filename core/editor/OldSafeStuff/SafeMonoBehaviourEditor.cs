#if UNITY_EDITOR

//TODO: Delete this shit when unity fix original problem with default editors

using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MonoBehaviour), true, isFallback = true)]
public class SafeMonoBehaviourEditor : Editor
{
    public override void OnInspectorGUI()
    {
        try { base.OnInspectorGUI(); }
        catch (Exception e)
        {
            //Debug.LogError(e);
        }
    }
}

#endif