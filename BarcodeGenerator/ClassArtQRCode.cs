using SkiaSharp;
using SkiaSharp.QrCode;
using SkiaSharp.QrCode.Image;

namespace BarcodeGenerator
{
    internal class ClassArtQRCode
    {
        /// <summary>
        /// Generates an artistic QR code image from the provided text and saves it as a PNG file.
        /// The QR code is styled with custom colors, circular modules, and high error correction level to ensure it remains
        /// scannable even with the artistic modifications.
        /// </summary>
        /// <param name="text">The text to encode in the QR code.</param>
        /// <returns>An ImageSource pointing to the generated QR code PNG file, or null if the input text is invalid.</returns>
        public static async Task<ImageSource?> GenerateArtQrCodeAsync(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return null;
            }

            // Create QR code with custom styling
            QRCodeImageBuilder qrData = new QRCodeImageBuilder(text)
                .WithSize(ClassQRCodeImage.nQRCodeSizePixels, ClassQRCodeImage.nQRCodeSizePixels)
                .WithModuleShape(CircleModuleShape.Default, sizePercent: 0.95f)
                .WithColors(
                    codeColor: SKColor.Parse(Globals.cCodeColorFg),
                    backgroundColor: SKColor.Parse(Globals.cCodeColorBg),
                    clearColor: SKColors.Transparent)
                .WithErrorCorrection(ECCLevel.H)
                .WithModuleShape(CircleModuleShape.Default);

            // Generate PNG bytes
            byte[] pngBytes = qrData.ToByteArray();
            if (pngBytes == null)
            {
                return null;
            }

            // Save PNG to file
            await File.WriteAllBytesAsync(Globals.cFileBarcodePng, pngBytes);

            // Return ImageSource for the generated PNG file
            return ImageSource.FromFile(Globals.cFileBarcodePng);
        }
    }
}
