using UnityEngine;

namespace Tools
{
    public static class DebugColoredOut
    {
        private const string GAME_EVENT_COLOR = "#ffc4f7";
        private const string GAME_DEBUG_COLOR = "#7aa398";
        private const string SYSTEM_EVENT_COLOR = "#28b594";
        private const string UI_EVENT_COLOR = "#d4ca72";
        private const string PING_COLOR = "#9c1ee6";
        private const string ERROR = "#eb5e34";
        private const string BATTLE = "#d84c74";
        private const string EDITOR_COLOR = "#F0B61E";

        public static string AsGameEvent(this string str)
        {
            return $"<color={GAME_EVENT_COLOR}>[GAME EVENT] {str}</color>";
        }

        public static string AsPing(this string str)
        {
            return $"<color={PING_COLOR}>[PING] {str}</color>";
        }

        public static string AsError(this string str)
        {
            return $"<color={ERROR}>[ERROR] {str}</color>";
        }

        public static string AsBattle(this string str)
        {
            return $"<color={BATTLE}>[BATTLE] {str}</color>";
        }

        public static string AsGameDebug(this string str)
        {
            return $"<color={GAME_DEBUG_COLOR}>[GAME DEBUG] {str}</color>";
        }

        public static string AsSystemEvent(this string str)
        {
            return $"<color={SYSTEM_EVENT_COLOR}>[SYSTEM] {str}</color>";
        }

        public static string AsUIEvent(this string str)
        {
            return $"<color={UI_EVENT_COLOR}>[UI] {str}</color>";
        }

        public static string AsEditorEvent(this string str)
        {
            return $"<color={EDITOR_COLOR}>[EDITOR] {str}</color>";
        }
        
        public static void Debug(this float f, string name = "float value")
        {
            UnityEngine.Debug.Log($"{name}:{f:0.000}".AsGameDebug());
        }

        public static void Debug(this int i, string name = "int value")
        {
            UnityEngine.Debug.Log($"{name}:{i}".AsGameDebug());
        }

        public static void Debug(this string s, string name = "string value")
        {
            UnityEngine.Debug.Log($"{name}:{s}".AsGameDebug());
        }

        public static void Debug(this long l, string name = "long value")
        {
            UnityEngine.Debug.Log($"{name}:{l}".AsGameDebug());
        }
        
        public static void Debug(this Vector2 v, string name = "vector2 value")
        {
            UnityEngine.Debug.Log($"{name}:[x={v.x:0.00}; y={v.y:0.00}]".AsGameDebug());
        }

        public static void Debug(this Vector3 v, string name = "vector3 value")
        {
            UnityEngine.Debug.Log($"{name}:[x={v.x:0.00}; y={v.y:0.00}; z={v.z:0.00}]".AsGameDebug());
        }
        
        public static string WithColor(this string str, string color)
        {
            return $"<color={color}>{str}</color>";
        }
        
        public static string WithColor(this string str, Color color)
        {
            return $"<color={color.ToHtmlStringRGBA()}>{str}</color>";
        }
    }
}