using CommunityToolkit.Maui.Extensions;
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

            // Show a modal popup to inform the user about the recommended image size before opening the file picker
            Page? currentPage = Application.Current?.Windows.Count > 0 ? Application.Current.Windows[0]?.Page : null;
            if (currentPage != null)
            {
                Globals.bIsPopupMessage = true;
                await currentPage.ShowPopupAsync(new PopupMessage(5, $"{CodeLang.QRCodeRecommendedImageSize_Text}:\n\n{ClassQRCodeImage.nQRCodeSizePixels} x {ClassQRCodeImage.nQRCodeSizePixels} {CodeLang.Pixels_Text}"));

                // Check if the popup was canceled by the user before proceeding to open the file picker
                if (Globals.bPopupCanceled)
                {
                    Globals.bPopupCanceled = false;
                    return null;
                }
            }

            // Open the file picker to select an image file for the background of the Art QR code
            FileResult? cFile = await ClassFileOperations.PickImage();

            // Create QR code with custom styling
            QRCodeImageBuilder qrData = new QRCodeImageBuilder(text)
                .WithSize(ClassQRCodeImage.nQRCodeSizePixels, ClassQRCodeImage.nQRCodeSizePixels)
                .WithModuleShape(CircleModuleShape.Default, sizePercent: 0.95f)
                .WithColors(codeColor: SKColor.Parse(Globals.cCodeColorFg),
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

            // If a background image was selected, composite the QR code on top of the background
            if (cFile != null)
            {
                using Stream bgStream = await cFile.OpenReadAsync();
                using SKBitmap bgBitmap = SKBitmap.Decode(bgStream);
                using SKBitmap qrBitmap = SKBitmap.Decode(pngBytes);

                int targetWidth = qrBitmap.Width;
                int targetHeight = qrBitmap.Height;

                // Create a scaled background with the same size as the QR code
                var info = new SKImageInfo(targetWidth, targetHeight, bgBitmap.ColorType, bgBitmap.AlphaType);
                using SKBitmap scaledBg = new SKBitmap(info);
                using (SKCanvas bgCanvas = new SKCanvas(scaledBg))
                {
                    bgCanvas.Clear(SKColors.Transparent);

                    // Draw bgBitmap stretched to the target size (preserves no aspect ratio).
                    using SKPaint paint = new() { IsAntialias = true, FilterQuality = SKFilterQuality.High };
                    var srcRect = new SKRect(0, 0, bgBitmap.Width, bgBitmap.Height);
                    var dstRect = new SKRect(0, 0, targetWidth, targetHeight);
                    bgCanvas.DrawBitmap(bgBitmap, srcRect, dstRect, paint);
                }

                // Composite QR on top of the scaled background
                using SKBitmap compositeBitmap = new SKBitmap(info);
                using SKCanvas canvas = new SKCanvas(compositeBitmap);
                canvas.Clear(SKColors.Transparent);

                // Draw scaled background then QR code (QR is same size and will be centered — top-left 0,0)
                canvas.DrawBitmap(scaledBg, 0, 0);
                canvas.DrawBitmap(qrBitmap, 0, 0);

                // Encode the composited image back to PNG
                using SKData pngData = compositeBitmap.Encode(SKEncodedImageFormat.Png, 100);
                pngBytes = pngData.ToArray();
            }

            // Save PNG to file
            await File.WriteAllBytesAsync(Globals.cFileBarcodePng, pngBytes);

            // Return ImageSource for the generated PNG file
            return ImageSource.FromFile(Globals.cFileBarcodePng);
        }
    }
}

/*
SKPaint' does not contain a definition for 'SamplingOptions'
The SamplingOptions property isn't available on your SKPaint type in the SkiaSharp version you're using. Two options:
1. Use the older FilterQuality property (no package change required). Replace the line with:
   using SKPaint paint = new() { IsAntialias = true, FilterQuality = SKFilterQuality.High };
2. Or upgrade your SkiaSharp package to a newer version that exposes SamplingOptions and then your original code will compile.
   using SKPaint paint = new() { SamplingOptions = new SKSamplingOptions(SKFilterMode.Linear, SKMipmapMode.Linear) };
Option 1 is the simplest and works with existing SkiaSharp releases.
*/