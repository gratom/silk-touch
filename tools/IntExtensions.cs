namespace Tools
{
    public static class IntExtensions
    {
        public static bool IsInRange(this int number, int minValue, int maxValue)
        {
            return number >= minValue && number <= maxValue;
        }
    }

}