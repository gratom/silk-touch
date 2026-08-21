using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tools
{

    public static class MathExtensions
    {
        public static bool IsInRange(this int number, int minValue, int maxValue)
        {
            return number >= minValue && number <= maxValue;
        }
        
        public static bool HasFraction(this double number)
        {
            const double tolerance = 0.00001d;
            return Math.Abs(number - Math.Truncate(number)) > tolerance;
        }
        
        public static float Sign(this float f)
        {
            return Mathf.Sign(f);
        }

        public static float Abs(this float f)
        {
            return Mathf.Abs(f);
        }

        public static float Clamp01(this float f)
        {
            return Mathf.Clamp01(f);
        }

        public static double Clamp01(this double d)
        {
            return Math.Clamp(d, 0d, 1d);
        }

        public static double Sign(this double d)
        {
            return Math.Sign(d);
        }

        public static double Abs(this double d)
        {
            return Math.Abs(d);
        }

        public static int RoundBy(this int value, int baseValue)
        {
            if (baseValue <= 0)
            {
                throw new ArgumentException("Base value must be greater than zero.");
            }

            int remainder = value % baseValue;
            return remainder == 0 ? value : value + (baseValue - remainder);
        }

        public static int Floor(this float value)
        {
            return Mathf.FloorToInt(value);
        }

        public static int Ceil(this float value)
        {
            return Mathf.CeilToInt(value);
        }

        public static int[] SplitToDigits(this int value)
        {
            if (value == 0)
            {
                return new[] { 0 };
            }

            value = Math.Abs(value);

            List<int> digits = new List<int>();
            while (value > 0)
            {
                digits.Add(value % 10);
                value /= 10;
            }

            digits.Reverse();
            return digits.ToArray();
        }

        public static int Clamp0(this int val)
        {
            return Mathf.Clamp(val, 0, int.MaxValue);
        }

        public static float Clamp0(this float val)
        {
            return Mathf.Clamp(val, 0, float.MaxValue);
        }

        public static int Clamp0ToMax(this int val, int max)
        {
            return Mathf.Clamp(val, 0, max);
        }

        public static int Clamp(this int val, int min, int max)
        {
            return Mathf.Clamp(val, min, max);
        }

        public static int ClampMax(this int val, int max)
        {
            return Mathf.Min(val, max);
        }

        public static int ClampMin(this int val, int min)
        {
            return Mathf.Max(val, min);
        }

        public static float Clamp0ToMax(this float val, float max)
        {
            return Mathf.Clamp(val, 0f, max);
        }

        public static float Clamp(this float val, float min, float max)
        {
            return Mathf.Clamp(val, min, max);
        }

        public static float ClampMax(this float val, float max)
        {
            return Mathf.Min(val, max);
        }

        public static float ClampMin(this float val, float min)
        {
            return Mathf.Max(val, min);
        }

        public static float ClampMinTo0(this float val, float min)
        {
            return Mathf.Clamp(val, min, 0f);
        }

        public static int ClampMinTo0(this int val, int min)
        {
            return Mathf.Clamp(val, min, 0);
        }

        public static float LerpFrom0(this float f, float value)
        {
            return Mathf.Lerp(0f, f, value);
        }

        public static float LerpFrom0(this int i, float value)
        {
            return Mathf.Lerp(0f, i, value);
        }

        public static int Round(this float f)
        {
            return Mathf.RoundToInt(f);
        }

        public static float Fract(this float value)
        {
            return value - Mathf.Floor(value);
        }

        public static float Inv(this float value)
        {
            return value != 0 ? 1f / value : 0f;
        }

        public static double Inv(this double value)
        {
            return value != 0 ? 1.0 / value : 0.0;
        }

        public static float RemapTo01(this float f, float min, float max)
        {
            if (Mathf.Approximately(min, max))
            {
                return 0f;
            }
            return Mathf.Clamp01((f - min) / (max - min));
        }

        public static float RemapTo01Unbound(this float f, float min, float max)
        {
            if (Mathf.Approximately(min, max))
            {
                return 0f;
            }
            return (f - min) / (max - min);
        }

        public static float RemapFrom01(this float t, float min, float max)
        {
            return min + (max - min) * Mathf.Clamp01(t);
        }

        public static float RemapFrom01Unbound(this float t, float min, float max)
        {
            return min + (max - min) * t;
        }

        public static float RemapTo01(this int value, int min, int max)
        {
            if (min == max)
            {
                return 0f;
            }

            return Mathf.Clamp01((float)(value - min) / (max - min));
        }

        public static int RemapFrom01(this float t, int min, int max)
        {
            return (int)(min + (max - min) * Mathf.Clamp01(t));
        }

        public static Vector2 ToCirclePoint(this float t, float r = 1f)
        {
            float angle = t * Mathf.PI * 2f;

            float x = Mathf.Cos(angle) * r;
            float y = Mathf.Sin(angle) * r;

            return new Vector2(x, y);
        }
    }
}