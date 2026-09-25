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
        private static SKBitmap DrawOuterCircleAroundBitmapWithFillOLD(SKBitmap source, SKColor fillColor, SKColor strokeColor, float strokeWidth)
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

        private static SKBitmap DrawOuterCircleAroundBitmapWithFill(SKBitmap source, SKColor fillColor, SKColor strokeColor, float strokeWidth)
        {
            try
            {
                int originalWidth = source.Width;
                int originalHeight = source.Height;

                // 1. Define the circle radius and diameter
                float radius = ((float)Math.Sqrt((originalWidth * originalWidth) + (originalHeight * originalHeight)) / 2f) + ClassBarcodes.nQRCodeQuietZoneSizeCircle;
                int circleDiameter = (int)Math.Ceiling(radius * 2f);

                // 2. Setup the canvas matching the circle size
                var info = new SKImageInfo(circleDiameter, circleDiameter);
                using (var surface = SKSurface.Create(info))
                {
                    var canvas = surface.Canvas;
                    canvas.Clear(SKColors.Transparent); // Keep background outside the circle clear

                    // 3. Create the circular clip path
                    using (var clipPath = new SKPath())
                    {
                        clipPath.AddCircle(radius, radius, radius);
                        canvas.ClipPath(clipPath, antialias: true);
                    }

                    // 4. Fill the circle background color
                    using (var paint = new SKPaint { Color = fillColor, IsAntialias = true })
                    {
                        canvas.DrawCircle(radius, radius, radius, paint);
                    }

                    // 5. Calculate source and destination rectangles to center the image WITHOUT scaling
                    // We find the center points of both the canvas and the image
                    float canvasCenter = radius;
                    float imageCenterWidth = originalWidth / 2f;
                    float imageCenterHeight = originalHeight / 2f;

                    // Define the bounding box on the canvas where the image will sit
                    float destLeft = canvasCenter - imageCenterWidth;
                    float destTop = canvasCenter - imageCenterHeight;

                    SKRect destRect = new SKRect(
                        destLeft,
                        destTop,
                        destLeft + originalWidth,
                        destTop + originalHeight
                    );

                    // Grab the full original image dimensions 
                    SKRect sourceRect = new SKRect(0, 0, originalWidth, originalHeight);

                    // 6. Draw the bitmap. Because sourceRect and destRect dimensions match perfectly, 
                    // SkiaSharp will draw the pixels 1:1 with zero scaling.
                    canvas.DrawBitmap(source, sourceRect, destRect);

                    SKBitmap output = new SKBitmap(circleDiameter, circleDiameter);
                    using (var snapshot = surface.Snapshot())
                    {
                        snapshot.ReadPixels(output.Info, output.GetPixels(), output.RowBytes, 0, 0);
                    }

                    return output;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in ClassQRCodeCircular.DrawOuterCircleAroundBitmapWithFill: {ex.Message}");
                return null!;
            }
        }
    }
}
