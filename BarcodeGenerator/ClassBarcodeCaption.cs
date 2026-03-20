// Detailed plan / pseudocode:
// 1. Create a static helper class `ClassBarcodeCaption` that can take an image stream (barcode) and a caption (the number).
// 2. Decode the incoming stream into a SkiaSharp bitmap (SKBitmap).
// 3. Measure the caption with an SKPaint to compute required caption height. If the caption is wider than the image,
//    reduce the font size iteratively until it fits (with a reasonable minimum).
// 4. Create a new SKBitmap whose height is original image height + captionHeight + padding.
// 5. Draw background color (from Globals.cCodeColorBg) onto the canvas.
// 6. Draw the barcode bitmap on top area (keeping its width; scale if needed to fit width).
// 7. Draw the caption centered horizontally below the barcode with foreground color (Globals.cCodeColorFg).
// 8. Encode the composed image as PNG and save to FileSystem.Current.CacheDirectory (or return a stream/path).
// 9. Provide two public helpers:
//    - `SaveBarcodeWithCaptionAsync(Stream barcodeStream, string caption, string fileName = ...)`
//    - `SaveBarcodeWithCaptionFromScreenshotAsync(IScreenshotResult screen, string caption, string fileName = ...)`
// 10. Make methods asynchronous and perform CPU-heavy work on a background thread using Task.Run.
// 11. Handle nulls and throw meaningful exceptions; ensure stream is seeked before decode.

using SkiaSharp;

namespace BarcodeGenerator
{
    public static class ClassBarcodeCaption
    {
        /// <summary>
        /// Convenience helper to get a screenshot's stream and call SaveBarcodeWithCaptionAsync
        /// </summary>
        public static async Task<string> SaveBarcodeWithCaptionFromScreenshotAsync(IScreenshotResult screen, string caption, string fileName = "barcode_generator.png", int padding = 12)
        {
            if (screen is null)
            {
                Debug.WriteLine("ClassBarcodeCaption.SaveBarcodeWithCaptionFromScreenshotAsync: screen is null.");
                return string.Empty;    // Return empty string on failure instead of throwing, to avoid crashing the app
            }

            await using Stream stream = await screen.OpenReadAsync();
            return await SaveBarcodeWithCaptionAsync(stream, caption, fileName, padding);
        }

        /// <summary>
        /// Create an image that contains the barcode image with the caption (number) placed under it,
        /// save the resulting PNG to the cache directory and return the full file path.
        /// </summary>
        /// <param name="barcodeStream">Stream containing the barcode image (PNG/JPEG/etc)</param>
        /// <param name="caption">Text to draw under the barcode (usually the numeric string)</param>
        /// <param name="fileName">Optional output file name. Defaults to "barcode_generator.png"</param>
        /// <param name="padding">Padding in pixels between barcode and caption and edges</param>
        /// <returns>Full path to the saved PNG file</returns>
        public static async Task<string> SaveBarcodeWithCaptionAsync(Stream barcodeStream, string caption, string fileName = "barcode_generator.png", int padding = 12)
        {
            if (barcodeStream is null)
            {
                Debug.WriteLine("ClassBarcodeCaption.SaveBarcodeWithCaptionAsync: barcodeStream is null.");
                return string.Empty;    // Return empty string on failure instead of throwing, to avoid crashing the app
            }

            caption ??= string.Empty;
            //caption = "AB123W5M5IJ";  // For testing only

            fileName = string.IsNullOrWhiteSpace(fileName) ? "barcode_generator.png" : fileName;

            // Do the CPU-bound image work on a background thread
            return await Task.Run(() =>
            {
                try
                {
                    // Ensure stream is readable from beginning
                    if (barcodeStream.CanSeek)
                    {
                        barcodeStream.Seek(0, SeekOrigin.Begin);
                    }

                    // Decode input image to SKBitmap
                    using Stream codecStream = barcodeStream.CanSeek ? barcodeStream : CopyToMemoryStream(barcodeStream);
                    using SKBitmap? skBitmap = SKBitmap.Decode(codecStream);
                    
                    if (skBitmap is null)
                    {
                        Debug.WriteLine("ClassBarcodeCaption.SaveBarcodeWithCaptionAsync: Failed to decode barcode image stream.");
                        return string.Empty;    // Return empty string on failure instead of throwing, to avoid crashing the app
                    }

                    int srcWidth = skBitmap.Width;
                    int srcHeight = skBitmap.Height;

                    // Determine colors (Globals stores as "AARRGGBB" without '#')
                    SKColor fgColor = TryParseSkColor(Globals.cCodeColorFg, SKColors.Black);
                    SKColor bgColor = TryParseSkColor(Globals.cCodeColorBg, SKColors.White);

                    // Determine font size relative to image width if not provided
                    float fontSize = Math.Max(14f, srcWidth / 16f);

                    // Select a font Typeface (default system font)
                    using SKTypeface typeface = SKTypeface.FromFamilyName("CourierNew", SKFontStyle.Normal)
                                     ?? SKTypeface.FromFamilyName("monospace")
                                     ?? SKTypeface.Default;
                    using SKFont font = new(typeface, fontSize);

                    // Prepare paint for text drawing (color, antialias)
                    using SKPaint textPaint = new()
                    {
                        IsAntialias = true,
                        Color = fgColor,
                        Style = SKPaintStyle.Fill
                    };

                    // Ensure caption fits horizontally; reduce font size if necessary
                    float maxTextWidth = srcWidth - padding * 2;
                    const float minFontSize = 6f;
                    int attempts = 0;
                    SKRect textBounds = new();
                    float measuredWidth = font.MeasureText(caption, out textBounds);
                    
                    while (measuredWidth > maxTextWidth && font.Size > minFontSize && attempts++ < 20)
                    {
                        font.Size -= 1.5f;
                        measuredWidth = font.MeasureText(caption, out textBounds);
                    }

                    // Compute caption height (approximate line height) from font metrics
                    SKFontMetrics metrics = font.Metrics;
                    float textHeight = metrics.Descent - metrics.Ascent;
                    int captionHeight = (int)Math.Ceiling(textHeight) + padding * 2;

                    // Create new bitmap with extra space for caption
                    int outWidth = srcWidth;
                    int outHeight = srcHeight + captionHeight;

                    using SKBitmap outBitmap = new(outWidth, outHeight, SKColorType.Bgra8888, SKAlphaType.Premul);
                    using SKCanvas canvas = new(outBitmap);

                    // Draw background
                    canvas.Clear(bgColor);

                    // Draw the barcode bitmap centered horizontally at top
                    // If source width differs from outWidth, scale proportionally to outWidth
                    float scaleX = (float)outWidth / srcWidth;
                    float scaleY = scaleX; // keep aspect ratio
                    SKRect destRect = SKRect.Create(0, 0, srcWidth * scaleX, srcHeight * scaleY);
                    
                    // Center horizontally if destRect.Width < outWidth due to rounding
                    destRect.Left = (outWidth - destRect.Width) / 2f;
                    destRect.Top = 0;

                    // Use an SKPaint when calling DrawBitmap overload that accepts SKRect; avoid obsolete FilterQuality
                    using SKPaint paintImage = new() { IsAntialias = true };
                    canvas.DrawBitmap(skBitmap, destRect, paintImage);

                    // Draw caption centered horizontally below the image
                    float textX = outWidth / 2f;
                    
                    // baseline Y: top of caption area + padding + absolute ascent
                    float textY = srcHeight + padding - metrics.Ascent; // ascent is negative
                    canvas.DrawText(caption, textX, textY, SKTextAlign.Center, font, textPaint);

                    // Encode to PNG
                    using SKImage image = SKImage.FromBitmap(outBitmap);
                    using SKData data = image.Encode(SKEncodedImageFormat.Png, 100);

                    // Write to file
                    using (FileStream file = File.Open(fileName, FileMode.Create, FileAccess.Write))
                    {
                        data.SaveTo(file);
                    }

                    return fileName;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"ClassBarcodeCaption.SaveBarcodeWithCaptionAsync error: {ex}");
                    return string.Empty;    // Return empty string on failure instead of throwing, to avoid crashing the app
                }
            });
        }

        // Helper: copy to MemoryStream when original stream is not seekable
        private static MemoryStream CopyToMemoryStream(Stream src)
        {
            MemoryStream ms = new();
            src.CopyTo(ms);
            ms.Seek(0, SeekOrigin.Begin);
            return ms;
        }

        // Helper: parse "AARRGGBB" or "#AARRGGBB" or "RRGGBB" into SKColor, fallback to defaultColor on failure
        private static SKColor TryParseSkColor(string? colorString, SKColor defaultColor)
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

        // Minor helper to obtain SKPixmap from SKBitmap (unused but kept for future adjustments)
        private static SKPixmap SKBitmapToPixmap(SKBitmap bmp)
        {
            return new SKPixmap(new SKImageInfo(bmp.Width, bmp.Height, bmp.ColorType, bmp.AlphaType), bmp.GetPixels());
        }
    }
}
