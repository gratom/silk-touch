using System.Collections.Generic;
using UnityEditor;

namespace Tools
{

    public enum SDebugTags
    {
        fight,
        def,
        inventory,
        temp,
        triggerRandom,
        modifiers
    }

    public static class SDebug
    {
        public static Dictionary<string, List<SDebugObject>> DebugObjects = new Dictionary<string, List<SDebugObject>>();

        public static void Log(string message, string tag = "def")
        {
            DebugObjects.InvokeIfContainOrSetAndInvoke(tag, new List<SDebugObject>(), list => list.Add(new SDebugObject { stringData = message }));
        }

        public static void Log(string message, SDebugTags tag = SDebugTags.def)
        {
            Log(message, tag.ToString());
        }

        public static void Clear()
        {
            DebugObjects.Clear();
        }
    }
}