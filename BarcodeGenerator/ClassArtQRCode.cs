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

            // The 2 characters of cArtCodeOpacityBg and the last 6 characters of cCodeColorBg to create a valid hex color string for SkiaSharp
            string cBackgroundColor = string.Concat(Globals.cArtCodeOpacityBg, Globals.cCodeColorBg.AsSpan(2, 6));

            // Create QR code with custom styling
            QRCodeImageBuilder qrData = new QRCodeImageBuilder(text)
                .WithSize(ClassQRCodeImage.nQRCodeSizePixels, ClassQRCodeImage.nQRCodeSizePixels)
                .WithModuleShape(CircleModuleShape.Default)
                .WithErrorCorrection(ECCLevel.H)
                .WithColors(codeColor: SKColor.Parse(Globals.cCodeColorFg),
                            backgroundColor: SKColor.Parse(cBackgroundColor),
                            clearColor: SKColors.Transparent);

            // Generate PNG bytes off the UI thread (QRCode creation can be heavy)
            byte[]? pngBytes = null;
            try
            {
                pngBytes = await Task.Run(() => qrData.ToByteArray());
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error generating QR bytes: {ex.Message}");
                return null;
            }

            if (pngBytes == null || pngBytes.Length == 0)
            {
                return null;
            }

            // If a background image was selected, composite the QR code on top of the background on a background thread
            if (cFile != null)
            {
                try
                {
                    // Read background image into memory first (safe to pass bytes into Task.Run)
                    byte[] bgBytes;
                    await using (Stream bgStream = await cFile.OpenReadAsync())
                    {
                        using MemoryStream ms = new MemoryStream();
                        await bgStream.CopyToAsync(ms);
                        bgBytes = ms.ToArray();
                    }

                    // Offload decoding + scaling + compositing to a background thread
                    byte[] composedBytes = await Task.Run(() =>
                    {
                        try
                        {
                            using SKBitmap? bgBitmap = SKBitmap.Decode(bgBytes);
                            using SKBitmap? qrBitmap = SKBitmap.Decode(pngBytes);

                            if (bgBitmap == null || qrBitmap == null)
                            {
                                Debug.WriteLine("Decode failed for bgBitmap or qrBitmap.");
                                return pngBytes;
                            }

                            int targetWidth = qrBitmap.Width;
                            int targetHeight = qrBitmap.Height;

                            // Create a scaled background with the same size as the QR code
                            SKImageInfo info = new SKImageInfo(targetWidth, targetHeight, bgBitmap.ColorType, bgBitmap.AlphaType);
                            using SKBitmap scaledBg = new SKBitmap(info);
                            using (SKCanvas bgCanvas = new SKCanvas(scaledBg))
                            {
                                bgCanvas.Clear(SKColors.Transparent);

                                // Draw bgBitmap stretched to the target size (preserves no aspect ratio).
                                using SKPaint paint = new() { IsAntialias = true, FilterQuality = SKFilterQuality.High };
                                SKRect srcRect = new SKRect(0, 0, bgBitmap.Width, bgBitmap.Height);
                                SKRect dstRect = new SKRect(0, 0, targetWidth, targetHeight);
                                bgCanvas.DrawBitmap(bgBitmap, srcRect, dstRect, paint);
                            }

                            // Compose scaled background and QR onto an SKSurface (CPU-backed)
                            SKImageInfo infoSurface = new SKImageInfo(targetWidth, targetHeight, SKColorType.Rgba8888, SKAlphaType.Premul);
                            using SKSurface surface = SKSurface.Create(infoSurface);
                            if (surface == null)
                            {
                                Debug.WriteLine("SKSurface.Create returned null.");
                                return pngBytes;
                            }

                            SKCanvas canvas = surface.Canvas;
                            canvas.Clear(SKColors.Transparent);

                            // Draw scaled background then QR
                            canvas.DrawBitmap(scaledBg, 0, 0);
                            canvas.DrawBitmap(qrBitmap, 0, 0);

                            // Snapshot and encode to PNG
                            canvas.Flush();
                            using SKImage image = surface.Snapshot();
                            using SKData pngData = image.Encode(SKEncodedImageFormat.Png, 100);

                            if (pngData != null)
                            {
                                return pngData.ToArray();
                            }

                            return pngBytes;
                        }
                        catch (Exception innerEx)
                        {
                            Debug.WriteLine($"Error composing art QR on background thread: {innerEx.Message}");
                            return pngBytes;
                        }
                    }).ConfigureAwait(false);

                    // Replace pngBytes with the composed bytes
                    if (composedBytes != null && composedBytes.Length > 0)
                    {
                        pngBytes = composedBytes;
                    }

                    Debug.WriteLine("Art QR: composite finished (background thread)");
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"Error processing background image: {e.Message}");
                }
            }

            // Save PNG to file
            try
            {
                await File.WriteAllBytesAsync(Globals.cFileBarcodePng, pngBytes);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error saving PNG file: {ex.Message}");
                return null;
            }

            // Return ImageSource for the generated PNG file
            return ImageSource.FromStream(() => new MemoryStream(pngBytes, writable: false));
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

https://mono.github.io/SkiaSharp/docs/basics/bitmaps.html
*/