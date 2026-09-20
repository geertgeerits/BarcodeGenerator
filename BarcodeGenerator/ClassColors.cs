using SkiaSharp;

namespace BarcodeGenerator
{
    internal class ClassColors
    {
        /// <summary>
        /// Converts a Microsoft.Maui.Graphics.Color to a SkiaSharp.SKColor.
        /// </summary>
        /// <param name="c">The Microsoft.Maui.Graphics.Color to convert.</param>
        /// <returns>The equivalent SkiaSharp.SKColor.</returns>
        public static SKColor ToSKColor(Microsoft.Maui.Graphics.Color c)
        {
            return new SKColor(
                (byte)Math.Round(c.Red * 255.0),
                (byte)Math.Round(c.Green * 255.0),
                (byte)Math.Round(c.Blue * 255.0),
                (byte)Math.Round(c.Alpha * 255.0));
        }

        /// <summary>
        /// Attempts to parse a color string into an SKColor. If parsing fails, returns the provided default color.
        /// Helper: parse "AARRGGBB" or "#AARRGGBB" or "RRGGBB" into SKColor, fallback to defaultColor on failure
        /// </summary>
        /// <param name="colorString">The color string to parse.</param>
        /// <param name="defaultColor">The default SKColor to return if parsing fails.</param>
        /// <returns>The parsed SKColor, or the default color if parsing fails.</returns>
        public static SKColor TryParseSkColor(string? colorString, SKColor defaultColor)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(colorString))
                {
                    return defaultColor;
                }

                string s = colorString.Trim();
                if (!s.StartsWith('#'))
                {
                    s = "#" + s;
                }

                return SKColor.Parse(s);
            }
            catch
            {
                return defaultColor;
            }
        }
    }
}
