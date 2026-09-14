#if UNITY_EDITOR

//TODO: Delete this shit when unity fix original problem with default editors

using System;
using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(ScriptableObject), true, isFallback = true)]
public class SafeScriptableObjectEditor : Editor
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