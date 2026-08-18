using UnityEngine;

namespace Tools
{
    public static class ColorExtensions
    {
        public static Color Plus(this Color color, float plusPercent)
        {
            return new Color(color.a * plusPercent, color.g * plusPercent, color.b * plusPercent, color.a);
        }

        public static Color WithR(this Color color, float r)
        {
            return new Color(r, color.g, color.b, color.a);
        }

        public static Color WithG(this Color color, float g)
        {
            return new Color(color.r, g, color.b, color.a);
        }

        public static Color WithB(this Color color, float b)
        {
            return new Color(color.r, color.g, b, color.a);
        }

        public static Color WithAlpha(this Color color, float a)
        {
            return new Color(color.r, color.g, color.b, a);
        }

        public static Texture2D ToSolidTexture(this Color color)
        {
            Texture2D texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, color);
            texture.Apply();
            return texture;
        }

        public static string ToHtmlStringRGBA(this Color color)
        {
            int r = (int)(color.r * 255);
            int g = (int)(color.g * 255);
            int b = (int)(color.b * 255);
            int a = (int)(color.a * 255);

            return string.Format("#{0:X2}{1:X2}{2:X2}{3:X2}", r, g, b, a);
        }

        public static Color HexToColor(this string hex)
        {
            if (ColorUtility.TryParseHtmlString(hex, out Color color))
            {
                return color;
            }
            return Color.white;
        }

        private static readonly System.Random _rand = new System.Random(50);

        public static Color RandomColor(float minBrightness = 0.6f, float maxBrightness = 0.85f)
        {
            float r = Mathf.Lerp(minBrightness, maxBrightness, (float)_rand.NextDouble());
            float g = Mathf.Lerp(minBrightness, maxBrightness, (float)_rand.NextDouble());
            float b = Mathf.Lerp(minBrightness, maxBrightness, (float)_rand.NextDouble());

            Color color = new Color(r, g, b);
            return color;
        }

        public static Color RandomHSV(
            float hMin = 0f, float hMax = 1f,
            float sMin = 0.4f, float sMax = 0.95f,
            float vMin = 0.65f, float vMax = 0.95f)
        {
            float h = Mathf.Lerp(hMin, hMax, (float)_rand.NextDouble());
            float s = Mathf.Lerp(sMin, sMax, (float)_rand.NextDouble());
            float v = Mathf.Lerp(vMin, vMax, (float)_rand.NextDouble());
            return Color.HSVToRGB(h, s, v);
        }

    }
}