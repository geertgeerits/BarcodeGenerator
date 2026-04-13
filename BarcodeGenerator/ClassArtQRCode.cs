using CommunityToolkit.Maui.Extensions;
using SkiaSharp;
using SkiaSharp.QrCode;
using SkiaSharp.QrCode.Image;
using System.IO.Compression;

namespace BarcodeGenerator
{
    internal class ClassArtQRCode
    {
        private static readonly bool bImageStretched = false;  // Whether to stretch the background image to fill the QR code dimensions (true) or fit within while preserving aspect ratio (false)

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

            // Show a modal popup with information about the Art QR code features before opening the file pickers
            Page? currentPage = Application.Current?.Windows.Count > 0 ? Application.Current.Windows[0]?.Page : null;
            if (currentPage != null)
            {
                _ = await currentPage.ShowPopupAsync(new PopupSettingsArtQRCode(CodeLang.Settings_Text));

                // Check if the popup was canceled by the user before proceeding to open the file picker
                if (Globals.bPopupCanceled)
                {
                    return null;
                }
            }

            // Create a gradient for the QR code if enabled in settings
            GradientOptions? gradient = null;

            if (ClassBarcodes.bQRCodeGradientColor)
            {
                gradient = new([SKColor.Parse(ClassBarcodes.cQRCodeGradientColor1), SKColor.Parse(ClassBarcodes.cQRCodeGradientColor2), SKColor.Parse(ClassBarcodes.cQRCodeGradientColor3)],
                    Enum.Parse<GradientDirection>(ClassBarcodes.cQRCodeGradientDirection),
                    [0f, 0.5f, 1f]);
            }

            // Show a modal popup to inform the user about the recommended foreground image size before opening the file picker
            FileResult? cFileForeground = null;

            if (ClassBarcodes.bQRCodeForegroundImage)
            {
                // Calculate the recommended foreground image size based on the QR code size and a percentage defined in ClassBarcodes
                int nRecommendedImageSize = (int)(ClassBarcodes.nQRCodeSizePixels * ClassBarcodes.nQRCodeImageSizePercent / 100f);

                currentPage = Application.Current?.Windows.Count > 0 ? Application.Current.Windows[0]?.Page : null;
                if (currentPage != null)
                {
                    _ = await currentPage.ShowPopupAsync(new PopupMessage(20, CodeLang.QRCodeImageForegroundTitle_Text, $"{CodeLang.QRCodeRecommendedImageSize_Text}:\n\n{nRecommendedImageSize:N0} x {nRecommendedImageSize:N0} {CodeLang.Pixels_Text}"));

                    // Check if the popup was canceled by the user before proceeding to open the file picker
                    if (Globals.bPopupCanceled)
                    {
                        Globals.bPopupCanceled = false;
                    }
                    else
                    {
                        // Open the file picker to select an image file for the foreground of the Art QR code
                        cFileForeground = await ClassFileOperations.PickImage();
                    }
                }
            }

            // Show a modal popup to inform the user about the recommended background image size before opening the file picker
            FileResult? cFileBackground = null;

            if (ClassBarcodes.bQRCodeBackgroundImage)
            {
                currentPage = Application.Current?.Windows.Count > 0 ? Application.Current.Windows[0]?.Page : null;
                if (currentPage != null)
                {
                    _ = await currentPage.ShowPopupAsync(new PopupMessage(20, CodeLang.QRCodeImageBackgroundTitle_Text, $"{CodeLang.QRCodeRecommendedImageSize_Text}:\n\n{ClassBarcodes.nQRCodeSizePixels:N0} x {ClassBarcodes.nQRCodeSizePixels:N0} {CodeLang.Pixels_Text}"));

                    // Check if the popup was canceled by the user before proceeding to open the file picker
                    if (Globals.bPopupCanceled)
                    {
                        Globals.bPopupCanceled = false;
                    }
                    else
                    {
                        // Open the file picker to select an image file for the background of the Art QR code
                        cFileBackground = await ClassFileOperations.PickImage();
                    }
                }
            }

            // Compress then encode (gzip->base64) to reduce bytes and allow encoding larger text content than the QR code
            // would normally allow, at the cost of requiring a custom decoder on the scanning side
            if (ClassBarcodes.bCompressionEnabled)
            {
                text = ClassCompression.CompressToBase64(text);
            }

            // Load the foreground image (if any) into an SKBitmap and create an IconData for the QR code builder.
            // The IconData will be embedded in the center of the QR code and should be sized appropriately (e.g. 20-30% of the QR code size)
            // to ensure the QR code remains scannable.
            SKBitmap? logo = null;
            IconData? icon = null;

            try
            {
                if (cFileForeground != null)
                {
                    // Read foreground image bytes (async) and apply EXIF orientation
                    byte[] fgBytes;
                    await using (Stream fgStream = await cFileForeground.OpenReadAsync())
                    {
                        using MemoryStream ms = new();
                        await fgStream.CopyToAsync(ms);
                        fgBytes = ms.ToArray();
                    }

                    logo = DecodeAndOrientBitmap(fgBytes);
                    if (logo != null)
                    {
                        icon = IconData.FromImage(logo, iconSizePercent: (int)ClassBarcodes.nQRCodeImageSizePercent, iconBorderWidth: 10);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ClassArtQRCode.GenerateArtQrCodeAsync (foreground): {ex.Message}");
            }

            // Create QR code with custom styling and non-compressed text
            // If no size is given then the default size = 512 x 512 pixels
            QRCodeImageBuilder? qrData = null;

            if (ClassBarcodes.cQRCodeModuleShape == "Square")
            {

                qrData = new QRCodeImageBuilder(text)
                    .WithSize(ClassBarcodes.nQRCodeSizePixels, ClassBarcodes.nQRCodeSizePixels)
                    .WithErrorCorrection(ECCLevel.H)
                    .WithColors(codeColor: SKColor.Parse(ClassBarcodes.cCodeColorFgArtQRCode),
                                backgroundColor: SKColor.Parse(ClassBarcodes.cCodeColorBgArtQRCode),
                                clearColor: SKColors.Transparent)
                    .WithGradient(gradient)
                    .WithIcon(icon);
            }
            else if (ClassBarcodes.cQRCodeModuleShape == "Rounded")
            {
                qrData = new QRCodeImageBuilder(text)
                    .WithSize(ClassBarcodes.nQRCodeSizePixels, ClassBarcodes.nQRCodeSizePixels)
                    .WithModuleShape(RoundedRectangleModuleShape.Default)
                    .WithErrorCorrection(ECCLevel.H)
                    .WithColors(codeColor: SKColor.Parse(ClassBarcodes.cCodeColorFgArtQRCode),
                                backgroundColor: SKColor.Parse(ClassBarcodes.cCodeColorBgArtQRCode),
                                clearColor: SKColors.Transparent)
                    .WithFinderPatternShape(RoundedRectangleFinderPatternShape.Default)
                    .WithGradient(gradient)
                    .WithIcon(icon);
            }
            else if (ClassBarcodes.cQRCodeModuleShape == "Circle")
            {
                qrData = new QRCodeImageBuilder(text)
                    .WithSize(ClassBarcodes.nQRCodeSizePixels, ClassBarcodes.nQRCodeSizePixels)
                    .WithModuleShape(CircleModuleShape.Default)
                    .WithErrorCorrection(ECCLevel.H)
                    .WithColors(codeColor: SKColor.Parse(ClassBarcodes.cCodeColorFgArtQRCode),
                                backgroundColor: SKColor.Parse(ClassBarcodes.cCodeColorBgArtQRCode),
                                clearColor: SKColors.Transparent)
                    .WithFinderPatternShape(CircleFinderPatternShape.Default)
                    .WithGradient(gradient)
                    .WithIcon(icon);
            }

            // Add a null check before using qrData
            if (qrData == null)
            {
                Debug.WriteLine("Invalid QR code module shape");
                return null;
            }

            // Generate PNG bytes off the UI thread (QRCode creation can be heavy)
            byte[]? pngBytes = null;
            try
            {
                pngBytes = await Task.Run(() => qrData.ToByteArray());
                
                logo?.Dispose();
                if (icon is IDisposable d) d.Dispose();
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
            if (cFileBackground != null)
            {
                try
                {
                    // Read background image into memory first (safe to pass bytes into Task.Run)
                    byte[] bgBytes;
                    await using (Stream bgStream = await cFileBackground.OpenReadAsync())
                    {
                        using MemoryStream ms = new();
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
                                    SKRect srcRect = new(0, 0, bgBitmap.Width, bgBitmap.Height);
                                    SKRect dstRect = new(0, 0, targetWidth, targetHeight);
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
                                    SKRect srcRect = new(0, 0, bgBitmap.Width, bgBitmap.Height);
                                    SKRect dstRect = new(left, top, left + scaledWidth, top + scaledHeight);
                                    bgCanvas.DrawBitmap(bgBitmap, srcRect, dstRect, paint);
                                }
                            }

                            // Compose scaled background and QR onto an SKSurface (CPU-backed)
                            SKImageInfo infoSurface = new(targetWidth, targetHeight, SKColorType.Rgba8888, SKAlphaType.Premul);
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
                await File.WriteAllBytesAsync(ClassBarcodes.cFileBarcodePng, pngBytes);
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