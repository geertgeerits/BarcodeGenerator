using SkiaSharp;

namespace BarcodeGenerator
{
    internal class ClassBarcodeCircle
    {
        /// <summary>
        /// Add a circle to a QR code from an existing image file and saves it as a PNG in the cache directory.
        /// </summary>
        /// <param name="filePath">The path to the existing image file.</param>
        public static void DrawOuterCircleFromFile(string filePath)
        {
            // Check if the barcode with circle is enabled, otherwise return
            if (!ClassBarcodes.bBarcodeWithCircle)
            {
                return;
            }

            string outputPath = filePath;

            // Draw outer circle around the QR code image
            using SKBitmap finalCircular = DrawOuterCircle(SKBitmap.Decode(filePath));

            // Encode and save PNG file
            using SKImage image = SKImage.FromBitmap(finalCircular);
            using SKData encoded = image.Encode(SKEncodedImageFormat.Png, 100);
            using FileStream outStream = File.OpenWrite(outputPath);
            encoded.SaveTo(outStream);
        }

        /// <summary>
        /// Draws an outer circle around the given bitmap with specified fill and stroke colors.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private static SKBitmap DrawOuterCircle(SKBitmap source)
        {
            SKColor fill = ClassColors.TryParseSkColor(ClassBarcodes.cCodeColorBgArtQRCode, SKColors.White);
            SKColor stroke = ClassColors.TryParseSkColor(ClassBarcodes.cCodeColorFgArtQRCode, SKColors.Black);

            return DrawOuterCircleAroundBitmapWithFill(source, fill, stroke, 2);
        }

        /// <summary>
        /// Draws an outer circle around the given bitmap with specified fill and stroke colors.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="fillColor"></param>
        /// <param name="strokeColor"></param>
        /// <param name="strokeWidth"></param>
        /// <returns></returns>
        private static SKBitmap DrawOuterCircleAroundBitmapWithFill(SKBitmap source, SKColor fillColor, SKColor strokeColor, float strokeWidth)
        {
            try
            {
                int srcW = source.Width;
                int srcH = source.Height;

                // Circle radius = half the diagonal of the bitmap
                float radius = (float)Math.Sqrt((srcW * srcW) + (srcH * srcH)) / 2f;

                // Output bitmap must be large enough to contain the circle
                int diameter = (int)Math.Ceiling(radius * 2f);

                SKBitmap output = new(diameter, diameter);

                using (SKCanvas canvas = new(output))
                {
                    canvas.Clear(SKColors.Transparent);

                    float cx = diameter / 2f;
                    float cy = diameter / 2f;

                    // Draw filled circle (inside color)
                    using (SKPaint fillPaint = new()
                    {
                        Color = fillColor,
                        IsAntialias = true,
                        Style = SKPaintStyle.Fill
                    })
                    {
                        canvas.DrawCircle(cx, cy, radius, fillPaint);
                    }

                    // Draw bitmap centered on top of the filled circle
                    float left = (diameter - srcW) / 2f;
                    float top = (diameter - srcH) / 2f;

                    canvas.DrawBitmap(source, left, top, new SKSamplingOptions(SKFilterMode.Linear), null);

                    // Draw circle outline - commented out for now, as it may not be needed
                    //using SKPaint strokePaint = new()
                    //{
                    //    Color = strokeColor,
                    //    StrokeWidth = strokeWidth,
                    //    IsAntialias = true,
                    //    Style = SKPaintStyle.Stroke
                    //};
                    //canvas.DrawCircle(cx, cy, radius, strokePaint);
                }

                return output;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in ClassQRCodeCircular.DrawOuterCircleAroundBitmapWithFill: {ex.Message}");
                return null!;
            }
        }
    }
}
