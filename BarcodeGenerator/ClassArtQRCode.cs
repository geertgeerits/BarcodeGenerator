using CommunityToolkit.Maui.Extensions;
using SkiaSharp;
using SkiaSharp.QrCode;
using SkiaSharp.QrCode.Image;
using System.IO;

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
                .WithErrorCorrection(ECCLevel.H)
                .WithColors(codeColor: SKColor.Parse(Globals.cCodeColorFg),
                            backgroundColor: SKColor.Parse(Globals.cCodeColorBg),
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
                        using var ms = new MemoryStream();
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

                            // Compose scaled background and QR onto an SKSurface (CPU-backed)
                            var infoSurface = new SKImageInfo(targetWidth, targetHeight, SKColorType.Rgba8888, SKAlphaType.Premul);
                            using var surface = SKSurface.Create(infoSurface);
                            if (surface == null)
                            {
                                Debug.WriteLine("SKSurface.Create returned null.");
                                return pngBytes;
                            }

                            var canvas = surface.Canvas;
                            canvas.Clear(SKColors.Transparent);

                            // Draw scaled background then QR
                            canvas.DrawBitmap(scaledBg, 0, 0);
                            canvas.DrawBitmap(qrBitmap, 0, 0);

                            // Snapshot and encode to PNG
                            canvas.Flush();
                            using var image = surface.Snapshot();
                            using var pngData = image.Encode(SKEncodedImageFormat.Png, 100);

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
            return ImageSource.FromFile(Globals.cFileBarcodePng);


            //CombineBitmaps(cFile, "overlay.png", Globals.cFileBarcodePng);
            
            //return ImageSource.FromFile(Globals.cFileBarcodePng);

        }

        public static void CombineBitmaps(string basePath, string overlayPath, string outputPath)
        {
            // Load the base image
            using SKBitmap baseBitmap = SKBitmap.Decode(basePath);

            // Load the overlay image (with transparency)
            using SKBitmap overlayBitmap = SKBitmap.Decode(overlayPath);

            // Create a surface to draw on
            using SKSurface surface = SKSurface.Create(new SKImageInfo(baseBitmap.Width, baseBitmap.Height));
            SKCanvas canvas = surface.Canvas;

            // Clear canvas (optional)
            canvas.Clear(SKColors.Transparent);

            // Draw the base image
            canvas.DrawBitmap(baseBitmap, 0, 0);

            // Draw the overlay image on top
            // You can change the position (x, y)
            canvas.DrawBitmap(overlayBitmap, new SKPoint(0, 0));

            // Save the result
            using var image = surface.Snapshot();
            using var data = image.Encode(SKEncodedImageFormat.Png, 100);

            using var stream = File.OpenWrite(outputPath);
            data.SaveTo(stream);
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