#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public static class DebugSelection
{
    [MenuItem("Assets/Debug Selected Object Names")]
    private static void DebugSelectedNames()
    {
        Object[] selectedObjects = Selection.objects;
        string names = "Selected Object Names: \n";
        foreach (Object obj in selectedObjects)
        {
            names += obj.name + ",\n";
        }
        Debug.Log(names);
    }
}

#endif