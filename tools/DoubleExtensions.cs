using System;

namespace Tools
{
    public static class DoubleExtensions
    {
        public static bool HasFraction(this double number)
        {
            const double tolerance = 0.00001d;
            return Math.Abs(number - Math.Truncate(number)) > tolerance;
        }
    }
}