using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using SkiaSharp;
using QRCoder;
using System.Text;

namespace BarcodeGenerator
{
    internal class ClassQRCodeCircular
    {
        public static void GenerateCircularQRCode()
        {
            // Implementation for generating a circular QR code
            // This is a placeholder for the actual QR code generation logic
            Debug.WriteLine($"Generating circular QR code");
            
            string payloadData = "https://www.example.com";
            string outputPath = Path.Combine(FileSystem.Current.CacheDirectory, "circular_footprint_qr.png");

            //ClassBarcodes.cFileBarcodePng = outputPath;

            // 1. Generate the standard QR data using ECC Level H.
            // Level H lets the reader recover up to 30% of data—essential since we are shaving off the corners.
            using QRCodeGenerator qrGenerator = new();
            using QRCodeData qrCodeData = qrGenerator.CreateQrCode(payloadData, QRCodeGenerator.ECCLevel.H);
            
            // 2. Generate a standard square QR image
            using QRCode qrCode = new(qrCodeData);

            // Parameters: pixelsPerModule (adjust 20 for resolution)
            using Bitmap standardSquareQr = qrCode.GetGraphic(20);

            // 3. Cut the square into a strict circular layout
            using Bitmap finalCircularQr = CropToCircle(standardSquareQr);
            finalCircularQr.Save(outputPath, System.Drawing.Imaging.ImageFormat.Png);
            Debug.WriteLine($"Circular QR Code footprint saved to {outputPath}");
        }

        /// <summary>
        /// Crops a square bitmap image into a circular shape by applying a circular mask.
        /// </summary>
        /// <param name="srcImage">The source square bitmap image to be cropped.</param>
        /// <returns>A new bitmap image cropped into a circular shape.</returns>
        private static Bitmap CropToCircle(Bitmap srcImage)
        {
            // Find the smallest dimension to ensure a perfect 1:1 circular ratio
            int diameter = Math.Min(srcImage.Width, srcImage.Height);
            Bitmap dstImage = new(diameter, diameter);

            using (Graphics g = Graphics.FromImage(dstImage))
            {
                // Smooth out the clipped edges so they don't look pixelated
                g.SmoothingMode = SmoothingMode.AntiAlias;

                // Fill background with transparent (or Color.White if preferred)
                g.Clear(System.Drawing.Color.Transparent);

                // Create a perfect circular mask path
                using GraphicsPath path = new();
                path.AddEllipse(0, 0, diameter, diameter);

                // Set the canvas to only allow drawing within this circle
                g.SetClip(path);

                // Draw the square QR code over it—the corners falling outside the circle are discarded
                g.DrawImage(srcImage, 0, 0, diameter, diameter);
            }
            
            return dstImage;
        }
    }
}
