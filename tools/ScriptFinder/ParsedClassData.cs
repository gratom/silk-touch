#if UNITY_EDITOR
using UnityEditor;

namespace Tools
{
    public class ParsedClassData
    {
        public string ClassName { get; }
        public string BaseClassName { get; }
        public MonoScript ScriptAsset { get; }

        // Кэшированная строка для вывода, чтобы не заниматься конкатенацией в OnGUI
        public string DisplayLayout { get; }

        public ParsedClassData(string className, string baseClassName, MonoScript scriptAsset)
        {
            ClassName = className;
            BaseClassName = baseClassName;
            ScriptAsset = scriptAsset;
            DisplayLayout = $"{className} <color=#808080ff>: {baseClassName}</color>";
        }
    }
}
#endif