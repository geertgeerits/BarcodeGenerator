using CommunityToolkit.Maui.Extensions;
using SkiaSharp;
using SkiaSharp.QrCode;
using SkiaSharp.QrCode.Image;
using System.IO.Compression;

namespace BarcodeGenerator
{
    internal class ClassArtQRCode
    {
        private static bool bImageStretched = false;  // Whether to stretch the background image to fill the QR code dimensions (true) or fit within while preserving aspect ratio (false)

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

            // Check input text length against QR code limits based on detected mode and error correction level (H),
            // and show an alert if it exceeds the limits            
            if (!await QrModeDetector.CheckInputTextAsync(text))
            {
                return null;
            }

            // Show a modal popup to inform the user about the recommended image size before opening the file picker
            Page? currentPage = Application.Current?.Windows.Count > 0 ? Application.Current.Windows[0]?.Page : null;
            if (currentPage != null)
            {
                Globals.bIsPopupMessage = true;
                _ = await currentPage.ShowPopupAsync(new PopupMessage(5, $"{CodeLang.QRCodeRecommendedImageSize_Text}:\n\n{ClassQRCodeImage.nQRCodeSizePixels:N0} x {ClassQRCodeImage.nQRCodeSizePixels:N0} {CodeLang.Pixels_Text}"));

                // Check if the popup was canceled by the user before proceeding to open the file picker
                if (Globals.bPopupCanceled)
                {
                    Globals.bPopupCanceled = false;
                    return null;
                }
            }

            // Open the file picker to select an image file for the background of the Art QR code
            FileResult? cFile = await ClassFileOperations.PickImage();

            // Compress then encode (gzip->base64) to reduce bytes and allow encoding larger text content than the QR code
            // would normally allow, at the cost of requiring a custom decoder on the scanning side
            //text = CompressToBase64(text);

            // Create QR code with custom styling and non-compressed text
            // If no size is given then the default size = 512 x 512 pixels
            QRCodeImageBuilder qrData = new QRCodeImageBuilder(text)
                .WithSize(ClassQRCodeImage.nQRCodeSizePixels, ClassQRCodeImage.nQRCodeSizePixels)
                .WithModuleShape(CircleModuleShape.Default)
                .WithErrorCorrection(ECCLevel.H)
                .WithColors(codeColor: SKColor.Parse(Globals.cCodeColorFg),
                            backgroundColor: SKColor.Parse(Globals.cCodeColorBgArtQRCode),
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
                            using SKBitmap? bgBitmap = DecodeAndOrientBitmap(bgBytes);
                            using SKBitmap? qrBitmap = SKBitmap.Decode(pngBytes);

                            if (bgBitmap == null || qrBitmap == null)
                            {
                                Debug.WriteLine("Decode failed for bgBitmap or qrBitmap.");
                                return pngBytes;
                            }

                            int targetWidth = qrBitmap.Width;
                            int targetHeight = qrBitmap.Height;

                            // Create a scaled background with the same size as the QR code
                            SKImageInfo info = new(targetWidth, targetHeight, bgBitmap.ColorType, bgBitmap.AlphaType);
                            using SKBitmap scaledBg = new(info);
                            using (SKCanvas bgCanvas = new(scaledBg))
                            {
                                bgCanvas.Clear(SKColors.Transparent);

                                if (bImageStretched)
                                {
                                    // Draw bgBitmap stretched to the target size (preserves no aspect ratio)
                                    using SKPaint paint = new() { IsAntialias = true, FilterQuality = SKFilterQuality.High };
                                    SKRect srcRect = new SKRect(0, 0, bgBitmap.Width, bgBitmap.Height);
                                    SKRect dstRect = new SKRect(0, 0, targetWidth, targetHeight);
                                    bgCanvas.DrawBitmap(bgBitmap, srcRect, dstRect, paint);
                                }
                                else
                                {
                                    // Draw bgBitmap scaled to fit within the target size while preserving aspect ratio, centered on the canvas
                                    float scale = Math.Min((float)targetWidth / bgBitmap.Width, (float)targetHeight / bgBitmap.Height);
                                    float scaledWidth = bgBitmap.Width * scale;
                                    float scaledHeight = bgBitmap.Height * scale;
                                    float left = (targetWidth - scaledWidth) / 2;
                                    float top = (targetHeight - scaledHeight) / 2;
                                    using SKPaint paint = new() { IsAntialias = true, FilterQuality = SKFilterQuality.High };
                                    SKRect srcRect = new SKRect(0, 0, bgBitmap.Width, bgBitmap.Height);
                                    SKRect dstRect = new SKRect(left, top, left + scaledWidth, top + scaledHeight);
                                    bgCanvas.DrawBitmap(bgBitmap, srcRect, dstRect, paint);
                                }
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

        /// <summary>
        /// Decode image bytes and apply EXIF orientation (if any) so the returned bitmap is oriented "top-left"
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        private static SKBitmap? DecodeAndOrientBitmap(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
                return null;

            SKEncodedOrigin origin = SKEncodedOrigin.TopLeft;
            try
            {
                using SKMemoryStream codecStream = new(bytes);
                using SKCodec codec = SKCodec.Create(codecStream);
                if (codec != null)
                {
                    origin = codec.EncodedOrigin;
                }
            }
            catch
            {
                origin = SKEncodedOrigin.TopLeft;
            }

            // Decode original bitmap
            SKBitmap? srcBitmap = SKBitmap.Decode(bytes);
            if (srcBitmap == null)
                return null;

            // If already top-left, return as-is
            if (origin == SKEncodedOrigin.TopLeft)
                return srcBitmap;

            int w = srcBitmap.Width;
            int h = srcBitmap.Height;

            // Determine result size for rotations that swap dims
            int resultW = (origin == SKEncodedOrigin.RightTop || origin == SKEncodedOrigin.RightBottom || origin == SKEncodedOrigin.LeftTop || origin == SKEncodedOrigin.LeftBottom) ? h : w;
            int resultH = (origin == SKEncodedOrigin.RightTop || origin == SKEncodedOrigin.RightBottom || origin == SKEncodedOrigin.LeftTop || origin == SKEncodedOrigin.LeftBottom) ? w : h;

            SKImageInfo info = new(resultW, resultH, srcBitmap.ColorType, srcBitmap.AlphaType);
            SKBitmap result = new(info);

            using (SKCanvas canvas = new(result))
            {
                canvas.Clear(SKColors.Transparent);

                switch (origin)
                {
                    case SKEncodedOrigin.TopRight:
                        // Flip horizontal
                        canvas.Scale(-1, 1);
                        canvas.Translate(-w, 0);
                        canvas.DrawBitmap(srcBitmap, 0, 0);
                        break;

                    case SKEncodedOrigin.BottomRight:
                        // Rotate 180
                        canvas.RotateDegrees(180);
                        canvas.Translate(-w, -h);
                        canvas.DrawBitmap(srcBitmap, 0, 0);
                        break;

                    case SKEncodedOrigin.BottomLeft:
                        // Flip vertical
                        canvas.Scale(1, -1);
                        canvas.Translate(0, -h);
                        canvas.DrawBitmap(srcBitmap, 0, 0);
                        break;

                    case SKEncodedOrigin.RightTop:
                        // Rotate 90 CW
                        canvas.Translate(resultW, 0);
                        canvas.RotateDegrees(90);
                        canvas.DrawBitmap(srcBitmap, 0, 0);
                        break;

                    case SKEncodedOrigin.LeftBottom:
                        // Rotate 270 CW (or 90 CCW)
                        canvas.Translate(0, resultH);
                        canvas.RotateDegrees(270);
                        canvas.DrawBitmap(srcBitmap, 0, 0);
                        break;

                    case SKEncodedOrigin.LeftTop:
                        // Transpose (mirror across top-left to bottom-right axis)
                        // Implement as rotate 90 CW then flip horizontal
                        canvas.Translate(resultW, 0);
                        canvas.RotateDegrees(90);
                        canvas.Scale(-1, 1);
                        canvas.DrawBitmap(srcBitmap, 0, 0);
                        break;

                    case SKEncodedOrigin.RightBottom:
                        // Transverse (mirror across top-right to bottom-left axis)
                        // Implement as rotate 270 CW then flip horizontal
                        canvas.Translate(0, resultH);
                        canvas.RotateDegrees(270);
                        canvas.Scale(-1, 1);
                        canvas.DrawBitmap(srcBitmap, 0, 0);
                        break;

                    default:
                        canvas.DrawBitmap(srcBitmap, 0, 0);
                        break;
                }
            }

            srcBitmap.Dispose();
            return result;
        }

        /// <summary>
        /// Compresses the specified string using GZip compression and encodes the result as a Base64 string.
        /// </summary>
        /// <remarks>The input string is first encoded as UTF-8 before compression. The returned Base64
        /// string can be decoded and decompressed to retrieve the original input.</remarks>
        /// <param name="s">The string to compress. Cannot be null.</param>
        /// <returns>A Base64-encoded string representing the compressed input.</returns>
        static string CompressToBase64(string s)
        {
            byte[] input = System.Text.Encoding.UTF8.GetBytes(s);
            using MemoryStream ms = new();
            using (GZipStream gz = new(ms, System.IO.Compression.CompressionLevel.Optimal, leaveOpen: true))
            {
                gz.Write(input, 0, input.Length);
            }

            ms.Position = 0;
            
            return Convert.ToBase64String(ms.ToArray());
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