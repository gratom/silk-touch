using System;
using System.Collections.Generic;
using System.Linq;

namespace Tools
{
    public static class EnumEnumerator
    {
        public static T Next<T>(this T src) where T : Enum
        {
            T[] array = (T[])Enum.GetValues(src.GetType());
            int j = Array.IndexOf(array, src) + 1;
            return array.Length == j ? array[0] : array[j];
        }

        public static IEnumerable<Enum> GetFlags(this Enum e)
        {
            return Enum.GetValues(e.GetType()).Cast<Enum>().Where(e.HasFlag);
        }
    }
}