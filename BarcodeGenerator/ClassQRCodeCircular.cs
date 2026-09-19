using SkiaSharp;
using QRCoder;

namespace BarcodeGenerator
{
    internal class ClassQRCodeCircular
    {
        public static void GenerateCircularQRCode()
        {
            Debug.WriteLine("Generating circular QR code");

            string payloadData = "https://www.example.com";
            string outputPath = Path.Combine(FileSystem.Current.CacheDirectory, "circular_footprint_qr.png");

            // 1. Generate QR data with ECC Level H
            using QRCodeGenerator qrGenerator = new();
            using QRCodeData qrCodeData = qrGenerator.CreateQrCode(payloadData, QRCodeGenerator.ECCLevel.H);

            // 2. Render PNG bytes directly (avoids System.Drawing)
            var pngQr = new PngByteQRCode(qrCodeData);
            byte[] pngBytes = pngQr.GetGraphic(20);

            // 3. Decode into a Skia bitmap
            SKBitmap standardBitmap;
            using (var ms = new MemoryStream(pngBytes))
            {
                standardBitmap = SKBitmap.Decode(ms);
            }

            // 4. Crop to circular footprint using SkiaSharp
            using SKBitmap finalCircular = CropToCircle(standardBitmap);

            // 5. Encode and save PNG
            using SKImage image = SKImage.FromBitmap(finalCircular);
            using SKData encoded = image.Encode(SKEncodedImageFormat.Png, 100);
            using var outStream = File.OpenWrite(outputPath);
            encoded.SaveTo(outStream);

            Debug.WriteLine($"Circular QR Code footprint saved to {outputPath}");
        }

        /// <summary>
        /// Crops a square/rectangular SKBitmap into a circular SKBitmap by applying a circular clip and scaling the source to fit.
        /// </summary>
        private static SKBitmap CropToCircle(SKBitmap src)
        {
            int diameter = Math.Min(src.Width, src.Height);
            var dst = new SKBitmap(diameter, diameter, SKColorType.Rgba8888, SKAlphaType.Premul);

            using var canvas = new SKCanvas(dst);
            canvas.Clear(SKColors.Transparent);

            using var path = new SKPath();
            float r = diameter / 2f;
            path.AddCircle(r, r, r); // Use AddCircle to create a circular path

            // Use clip to restrict drawing to circle and enable antialias
            canvas.ClipPath(path, SKClipOperation.Intersect, true);

            // Use ClipOval (non-obsolete) instead of the obsolete SKPath.AddCircle
            //var oval = SKRect.Create(0, 0, diameter, diameter);
            //canvas.ClipOval(oval, SKClipOperation.Intersect, true);

            // Draw the source bitmap scaled to the destination diameter (centers and fits)
            var srcRect = new SKRect(0, 0, src.Width, src.Height);
            var destRect = new SKRect(0, 0, diameter, diameter);

            // Create a paint object with antialiasing enabled
            var paint = new SKPaint { IsAntialias = true };

            // Draw the source image onto the canvas, scaling it to fit the circular area
            using var srcImage = SKImage.FromBitmap(src);
            canvas.DrawImage(srcImage, srcRect, destRect, SKSamplingOptions.Default, paint);

            canvas.Flush();
            return dst;
        }
    }
}
