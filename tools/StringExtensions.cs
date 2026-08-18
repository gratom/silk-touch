using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.Android;

namespace Tools
{
    public static class StringExtensions
    {
        public const string LettersAndNumbers = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        public const string SpecialCharacters = "!@#$%^&*()-_=+[]{}|;:'\",.<>?/";

        public static void OpenLink(this string link)
        {
            Application.OpenURL(link);
        }

        public static string ToCrossed(this string s)
        {
            return $"<s>{s}</s>";
        }

        public static string Clamp(this string str, int lenght)
        {
            return str.Substring(0, lenght.ClampMax(str.Length));
        }

        public static string FirstRowTruncated(this string str)
        {
            int nextLineIndex = str.IndexOf('\n');
            if (nextLineIndex != -1)
            {
                str = str.Substring(0, nextLineIndex);
            }
            return str;
        }

        public static string FromCamelCase(this string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return str;
            }

            StringBuilder sb = new StringBuilder();
            sb.Append(char.ToUpper(str[0]));

            for (int i = 1; i < str.Length; i++)
            {
                if (char.IsUpper(str[i]) && !char.IsUpper(str[i - 1]))
                {
                    sb.Append(' ');
                }
                sb.Append(str[i]);
            }

            return sb.ToString();
        }

        public static string GetRandomString(this int length, bool includeSpecialCharacters = false)
        {
            if (length <= 0)
            {
                throw new ArgumentException("Length must be greater than zero.", nameof(length));
            }

            string characterSet = includeSpecialCharacters ? LettersAndNumbers + SpecialCharacters : LettersAndNumbers;
            StringBuilder randomString = new StringBuilder(length);
            byte[] randomBytes = new byte[length];

            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }

            for (int i = 0; i < length; i++)
            {
                randomString.Append(characterSet[randomBytes[i] % characterSet.Length]);
            }

            return randomString.ToString();
        }

        public static string FillRight(this string source, int totalLength, char fillChar = ' ')
        {
            if (source == null)
            {
                return new string(fillChar, totalLength);
            }

            int missing = totalLength - source.Length;
            if (missing <= 0)
            {
                return source;
            }

            return source + new string(fillChar, missing);
        }

        public static string FillLeft(this string source, int totalLength, char fillChar = ' ')
        {
            if (source == null)
            {
                return new string(fillChar, totalLength);
            }

            int missing = totalLength - source.Length;
            if (missing <= 0)
            {
                return source;
            }

            return new string(fillChar, missing) + source;
        }

        public static List<string> SearchSimilar(this List<string> list, string request, int countMax = 5)
        {
            if (string.IsNullOrEmpty(request))
            {
                return new List<string>();
            }

            string lowerRequest = request.ToLowerInvariant();

            List<(string value, int index)> scored = new List<(string, int)>();

            foreach (string item in list)
            {
                if (item == null)
                {
                    continue;
                }

                string lowerItem = item.ToLowerInvariant();

                int index = lowerItem.IndexOf(lowerRequest, StringComparison.Ordinal);
                if (index >= 0)
                {
                    scored.Add((item, index));
                }
            }

            return scored
                .OrderBy(s => s.index)
                .ThenBy(s => s.value.Length)
                .Select(s => s.value)
                .Take(countMax)
                .ToList();
        }

        public static List<string> SearchSimilarLevenshtein(this List<string> list, string request, int countMax = 5)
        {
            if (string.IsNullOrEmpty(request))
            {
                return new List<string>();
            }

            string lowerRequest = request.ToLowerInvariant();

            List<(string value, int distance)> scored = new List<(string, int)>();

            foreach (string item in list)
            {
                if (item == null)
                {
                    continue;
                }

                string lowerItem = item.ToLowerInvariant();
                int dist = LevenshteinDistance(lowerRequest, lowerItem);

                scored.Add((item, dist));
            }

            return scored
                .OrderBy(s => s.distance)
                .Take(countMax)
                .Select(s => s.value)
                .ToList();
        }

        public static int LevenshteinDistance(string source, string target)
        {
            int n = source.Length;
            int m = target.Length;

            int[,] d = new int[n + 1, m + 1];

            for (int i = 0; i <= n; i++)
            {
                d[i, 0] = i;
            }

            for (int j = 0; j <= m; j++)
            {
                d[0, j] = j;
            }

            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    int cost = source[i - 1] == target[j - 1] ? 0 : 1;

                    d[i, j] = Math.Min(
                        Math.Min(
                            d[i - 1, j] + 1, // удаление
                            d[i, j - 1] + 1 // вставка
                        ),
                        d[i - 1, j - 1] + cost // замена
                    );
                }
            }

            return d[n, m];
        }

    }
}