#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Tools
{
    public static class SelectionDebugger
    {
        [MenuItem("GameObject/Objects Count", priority = 0)]
        private static void PrintSelectedCount()
        {
            int count = Selection.objects.Length;
            Debug.Log($"Selected objects count: {count}");
        }
    }
}
#endif