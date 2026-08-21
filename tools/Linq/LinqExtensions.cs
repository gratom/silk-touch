using System.Collections.Generic;
using System.Linq;
using Tools;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace System
{
    public static class LinqExtensions
    {
        public static bool TryRemoveAt<T>(this List<T> list, int index)
        {
            if (index >= 0 && index < list.Count)
            {
                list.RemoveAt(index);
                return true;
            }
            return false;
        }

        public static int IndexOf(this string[] arr, string value)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i] == value)
                {
                    return i;
                }
            }
            return -1;
        }

        public static bool TrySetValueAt<T>(this List<T> list, T value, int index, bool ignoreOrder = false, bool isFillDefault = false)
        {
            if (index < 0)
            {
                return false;
            }
            if (index < list.Count)
            {
                list[index] = value;
                return true;
            }
            if (index == list.Count)
            {
                list.Add(value);
                return true;
            }
            if (ignoreOrder)
            {
                if (isFillDefault)
                {
                    while (list.Count < index)
                    {
                        list.Add(default);
                    }
                }
                list.Add(value);
                return true;
            }
            return false;
        }

        public static T ElementWithMin<T>(this IEnumerable<T> source, Func<T, float> selector)
        {
            if (source == null || !source.Any())
            {
                throw new ArgumentException("The source collection is null or empty.");
            }

            float min = float.MaxValue;
            T element = default;

            foreach (T item in source)
            {
                float value = selector(item);
                if (value < min)
                {
                    min = value;
                    element = item;
                }
            }

            return element;
        }

        public static T ElementWithMax<T>(this IEnumerable<T> source, Func<T, float> selector)
        {
            if (source == null || !source.Any())
            {
                throw new ArgumentException("The source collection is null or empty.");
            }

            float max = float.MinValue;
            T element = default;

            foreach (T item in source)
            {
                float value = selector(item);
                if (value > max)
                {
                    max = value;
                    element = item;
                }
            }

            return element;
        }

        public static T RandomWeightedElement<T>(this IEnumerable<T> elements, Func<T, float> weightFunc, out int index, T def = default)
        {
            double bestKey = double.PositiveInfinity;
            T bestItem = def;
            int bestIndex = -1;

            int i = 0;
            foreach (T t in elements)
            {
                float w = weightFunc(t);
                if (w > 0)
                {
                    Random rnd = new Random();
                    double u = 1.0 - rnd.NextDouble();
                    double key = -Math.Log(u) / w;
                    if (key < bestKey)
                    {
                        bestKey = key;
                        bestItem = t;
                        bestIndex = i;
                    }
                }
                i++;
            }

            index = bestIndex;
            return bestItem;
        }

        public static string SDebug<T>(this IEnumerable<T> source, Func<T, string> selector = null, string prefix = "", bool autolog = true, string tag = "def")
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (selector == null)
            {
                selector = element => element.ToString();
            }
            string result = prefix + "\n" + string.Join("\n", source.Select(selector));
            if (autolog)
            {
                Tools.SDebug.Log(result, tag);
            }
            return result;
        }

        public static string SDebug<T>(this IEnumerable<T> source, Func<T, string> selector = null, string prefix = "", bool autolog = true, SDebugTags tag = SDebugTags.def)
        {
            return source.SDebug(selector, prefix, autolog, tag.ToString());
        }

        public static string Debug<T>(this IEnumerable<T> source, Func<T, string> selector = null, string prefix = "", bool autolog = true)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (selector == null)
            {
                selector = element => element.ToString();
            }
            string result = prefix + "\n" + string.Join("\n", source.Select(selector));
            if (autolog)
            {
                UnityEngine.Debug.Log(result);
            }
            return result;
        }

#if UNITY_EDITOR
        public static List<string> GetClipboardLines()
        {
            string clipboardText = EditorGUIUtility.systemCopyBuffer;

            if (string.IsNullOrEmpty(clipboardText))
            {
                return new List<string>();
            }

            string[] lines = clipboardText.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            return new List<string>(lines);
        }

        public static void SetClipboardLines(List<string> lines)
        {
            if (lines == null || lines.Count == 0)
            {
                EditorGUIUtility.systemCopyBuffer = string.Empty;
                return;
            }

            EditorGUIUtility.systemCopyBuffer = string.Join(Environment.NewLine, lines);
        }
#endif

    }
}