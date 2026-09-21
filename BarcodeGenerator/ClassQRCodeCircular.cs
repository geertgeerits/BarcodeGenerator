using SkiaSharp;

namespace BarcodeGenerator
{
    internal class ClassQRCodeCircular
    {
        /// <summary>
        /// Generates a circular QR code from an existing image file and saves it as a PNG in the cache directory.
        /// </summary>
        /// <param name="filePath">The path to the existing image file.</param>
        public static void GenerateCircularCodeFromFile(string filePath)
        {
            string outputPath = filePath;

            // Crop to circular footprint using SkiaSharp
            using SKBitmap finalCircular = CropToCircle(SKBitmap.Decode(filePath));

            // Encode and save PNG file
            using SKImage image = SKImage.FromBitmap(finalCircular);
            using SKData encoded = image.Encode(SKEncodedImageFormat.Png, 100);
            using FileStream outStream = File.OpenWrite(outputPath);
            encoded.SaveTo(outStream);
        }

        private static SKBitmap CropToCircle(SKBitmap source)
        {
            SKColor fill = ClassColors.TryParseSkColor(ClassBarcodes.cCodeColorBgArtQRCode, SKColors.White);
            SKColor stroke = ClassColors.ToSKColor(Microsoft.Maui.Graphics.Colors.White);

            return DrawOuterCircleAroundBitmapWithFill(source, fill, stroke, 1);
        }

        private static SKBitmap DrawOuterCircleAroundBitmapWithFill(
            SKBitmap source,
            SKColor fillColor,
            SKColor strokeColor,
            float strokeWidth)
        {
            int srcW = source.Width;
            int srcH = source.Height;

            // Circle radius = half the diagonal of the bitmap
            float radius = (float)Math.Sqrt(srcW * srcW + srcH * srcH) / 2f;

            // Output bitmap must be large enough to contain the circle
            int diameter = (int)Math.Ceiling(radius * 2f);

            SKBitmap output = new(diameter, diameter);

            using (var canvas = new SKCanvas(output))
            {
                canvas.Clear(SKColors.Transparent);

                float cx = diameter / 2f;
                float cy = diameter / 2f;

                // 1️⃣ Draw filled circle (inside color)
                using (var fillPaint = new SKPaint
                {
                    Color = fillColor,
                    IsAntialias = true,
                    Style = SKPaintStyle.Fill
                })
                {
                    canvas.DrawCircle(cx, cy, radius, fillPaint);
                }

                // 2️⃣ Draw bitmap centered on top of the filled circle
                float left = (diameter - srcW) / 2f;
                float top = (diameter - srcH) / 2f;

                canvas.DrawBitmap(source, left, top, new SKSamplingOptions(SKFilterMode.Linear), null);

                // 3️⃣ Draw circle outline
                using (var strokePaint = new SKPaint
                {
                    Color = strokeColor,
                    StrokeWidth = strokeWidth,
                    IsAntialias = true,
                    Style = SKPaintStyle.Stroke
                })
                {
                    canvas.DrawCircle(cx, cy, radius, strokePaint);
                }
            }

            return output;
        }
    }
}
