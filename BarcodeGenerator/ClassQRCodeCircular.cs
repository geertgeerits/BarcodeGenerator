using SkiaSharp;
using QRCoder;

namespace BarcodeGenerator
{
    internal class ClassQRCodeCircular
    {
        /// <summary>
        /// Generates a circular QR code from a payload string and saves it as a PNG in the cache directory.
        /// </summary>
        /// <remarks>For testing</remarks>
        public static void GenerateCircularQRCode()
        {
            Debug.WriteLine("Generating circular QR code");

            string payloadData = "https://www.example.com";
            string outputPath = Path.Combine(FileSystem.Current.CacheDirectory, "circular_footprint_qr.png");

            // 1. Generate QR data with ECC Level H
            using QRCodeGenerator qrGenerator = new();
            using QRCodeData qrCodeData = qrGenerator.CreateQrCode(payloadData, QRCodeGenerator.ECCLevel.H);

            // 2. Render PNG bytes directly (avoids System.Drawing)
            PngByteQRCode pngQr = new(qrCodeData);
            byte[] pngBytes = pngQr.GetGraphic(20);

            // 3. Decode into a Skia bitmap
            SKBitmap standardBitmap;
            using (MemoryStream ms = new(pngBytes))
            {
                standardBitmap = SKBitmap.Decode(ms);
            }

            // 4. Crop to circular footprint using SkiaSharp
            using SKBitmap finalCircular = CropToCircle(standardBitmap);

            // 5. Encode and save PNG
            using SKImage image = SKImage.FromBitmap(finalCircular);
            using SKData encoded = image.Encode(SKEncodedImageFormat.Png, 100);
            using FileStream outStream = File.OpenWrite(outputPath);
            encoded.SaveTo(outStream);

            Debug.WriteLine($"Circular QR Code footprint saved to {outputPath}");
        }

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

        /// <summary>
        /// load PNG from the cached file path into an SKBitmap and call CropToCircle to crop it into a circular shape.
        /// </summary>
        public static void LoadAndCropPngToCircle()
        {
            string path = ClassBarcodes.cFileBarcodePng;
            if (!File.Exists(path))
            {
                return;
            }

            SKBitmap? bmp;
            using (FileStream fs = File.OpenRead(path))
            {
                bmp = SKBitmap.Decode(fs);
            }

            if (bmp is null)
            {
                return;
            }

            // If CropToCircle accepts SKBitmap:
            SKBitmap circBitmap = CropToCircle(bmp);

            // If CropToCircle expects SKImage, convert:
            //SKBitmap circImage = CropToCircle(SKImage.FromBitmap(bmp));
        }

        /// <summary>
        /// Crops a square/rectangular SKBitmap into a circular SKBitmap by applying a circular clip and scaling the source to fit.
        /// </summary>
        /// <param name="src">The source SKBitmap to be cropped.</param>
        /// <returns>A new SKBitmap cropped to a circular shape.</returns>
        private static SKBitmap CropToCircleOLD(SKBitmap src)
        {
            // Determine the diameter of the circle based on the smaller dimension of the source bitmap
            //int diameter = Math.Min(src.Width, src.Height);
            int diameter = (int)(src.Width * Math.Sqrt(2));
            SKBitmap dst = new(diameter, diameter, SKColorType.Rgba8888, SKAlphaType.Premul);

            // Create a new canvas to draw on the destination bitmap
            using SKCanvas canvas = new(dst);
            canvas.Clear(SKColors.Transparent);

            // Calculate the radius for the circular clipping path
            float r = diameter / 2f;
            using SKPathBuilder builder = new();
            builder.AddCircle(r, r, r);

            // Detach() builds and returns the final immutable SKPath object
            using SKPath path = builder.Detach();

            // Use clip to restrict drawing to circle and enable antialias
            canvas.ClipPath(path, SKClipOperation.Intersect, antialias: true);

            // Draw the source bitmap scaled to the destination diameter (centers and fits)
            SKRect srcRect = new(0, 0, src.Width, src.Height);
            SKRect destRect = new(0, 0, diameter, diameter);

            // Draw the source bitmap centered without scaling
            //SKRect srcRect = new SKRect(0, 0, src.Width, src.Height);
            //float left = (diameter - src.Width) / 2f;
            //float top = (diameter - src.Height) / 2f;
            //SKRect destRect = new SKRect(left, top, left + src.Width, top + src.Height);

            // Create a paint object with antialiasing enabled
            SKPaint paint = new() { IsAntialias = true };

            // Draw the source image onto the canvas, scaling it to fit the circular area
            using SKImage srcImage = SKImage.FromBitmap(src);
            canvas.DrawImage(srcImage, srcRect, destRect, SKSamplingOptions.Default, paint);

            // Flush the canvas to ensure all drawing operations are completed
            canvas.Flush();

            return dst;
        }

        public static SKBitmap CropToCircle(SKBitmap source)
        {
            SKColor fill = ToSKColor(Microsoft.Maui.Graphics.Colors.Red);
            SKColor stroke = ToSKColor(Microsoft.Maui.Graphics.Colors.White);
            return DrawOuterCircleAroundBitmapWithFill(source, fill, stroke, 2);
        }

        public static SKBitmap DrawOuterCircleAroundBitmapWithFill(
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

            SKBitmap output = new SKBitmap(diameter, diameter);

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

                canvas.DrawBitmap(source, left, top);

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
        //public static SKBitmap CropToCircle(SKBitmap source)
        //{
        //    // Convert MAUI Colors to Skia SKColor and pass into the SK-based drawing method
        //    SKColor fill = ToSKColor(Microsoft.Maui.Graphics.Colors.Red);
        //    SKColor stroke = ToSKColor(Microsoft.Maui.Graphics.Colors.White);
        //    return DrawOuterCircleAroundBitmapWithFill(source, fill, stroke, 2);
        //}

        private static SKColor ToSKColor(Microsoft.Maui.Graphics.Color c)
        {
            return new SKColor(
                (byte)Math.Round(c.Red * 255.0),
                (byte)Math.Round(c.Green * 255.0),
                (byte)Math.Round(c.Blue * 255.0),
                (byte)Math.Round(c.Alpha * 255.0));
        }
    }
}
