using System;
using System.Collections.Generic;

namespace Tools
{
    public static class DictionaryExtensions
    {
        public static bool InvokeIfContain<T1, T2>(this Dictionary<T1, T2> dictionary, T1 key, Action<T2> act)
        {
            if (dictionary.TryGetValue(key, out T2 value))
            {
                act(value);
                return true;
            }

            return false;
        }

        public static void InvokeIfContainOrSetAndInvoke<T1, T2>(this Dictionary<T1, T2> dictionary, T1 key, T2 fallBackElement, Action<T2> act)
        {
            if (dictionary.TryGetValue(key, out T2 value))
            {
                act(value);
            }
            else
            {
                dictionary.Add(key, fallBackElement);
                act(dictionary[key]);
            }
        }

        public static void GetOrDefaultAndSet<T1, T2>(this Dictionary<T1, T2> dictionary, T1 key, Func<T2, T2> func, T2 defValue = default)
        {
            dictionary[key] = func(dictionary.GetValueOrDefault(key, defValue));
        }

        public static T2 GetOrDefault<T1, T2>(this Dictionary<T1, T2> dictionary, T1 key)
        {
            return dictionary.TryGetValue(key, out T2 value) ? value : default;
        }

        public static bool SetOrCreate<T1, T2>(this Dictionary<T1, T2> dictionary, T1 key, T2 value)
        {
            if (dictionary.TryGetValue(key, out T2 _))
            {
                dictionary[key] = value;
                return true;
            }

            dictionary.Add(key, value);

            return false;
        }
    }
}