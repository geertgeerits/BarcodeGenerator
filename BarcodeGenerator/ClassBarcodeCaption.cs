// Detailed plan / pseudocode:
// 1. Create a static helper class `ClassBarcodeCaption` that can take an image stream (barcode) and a caption (the number).
// 2. Decode the incoming stream into a SkiaSharp bitmap (SKBitmap).
// 3. Measure the caption with an SKPaint to compute required caption height. If the caption is wider than the image,
//    reduce the font size iteratively until it fits (with a reasonable minimum).
// 4. Create a new SKBitmap whose height is original image height + captionHeight + padding.
// 5. Draw background color (from ClassBarcodes.cCodeColorBg) onto the canvas.
// 6. Draw the barcode bitmap on top area (keeping its width; scale if needed to fit width).
// 7. Draw the caption centered horizontally below the barcode with foreground color (ClassBarcodes.cCodeColorFg).
// 8. Encode the composed image as PNG and save to FileSystem.Current.CacheDirectory (or return a stream/path).
// 9. Provide two public helpers:
//    - `SaveBarcodeWithCaptionAsync(Stream barcodeStream, string caption, string fileName = ...)`
//    - `SaveBarcodeWithCaptionFromScreenshotAsync(IScreenshotResult screen, string caption, string fileName = ...)`
// 10. Make methods asynchronous and perform CPU-heavy work on a background thread using Task.Run.
// 11. Handle nulls and throw meaningful exceptions; ensure stream is seeked before decode.

using CommunityToolkit.Maui.Extensions;
using SkiaSharp;
using ZXing.Net.Maui.Controls;

namespace BarcodeGenerator
{
    public static class ClassBarcodeCaption
    {
        /// <summary>
        /// Prompt the user for a caption and save the barcode with caption to a file.
        /// </summary>
        /// <param name="bgvBarcode">The barcode generator view.</param>
        /// <param name="image">The image to which the caption will be added.</param>
        /// <param name="fileBarcodeCaptionPng">The file path to save the barcode with caption.</param>
        /// <param name="caption">The caption text.</param>
        /// <param name="barcodeType">The type of the barcode.</param>
        /// <returns></returns>
        public static async Task AddBarcodeCaptionFileAsync(BarcodeGeneratorView bgvBarcode, Image image, string fileBarcodeCaptionPng, string caption, string barcodeType)
        {
            if (ClassBarcodes.bBarcodeWithCaption)
            {
                // Check if the popup 'PopupSettingsArtQRCode' was canceled before proceeding
                if (Globals.bPopupCanceled)
                {
                    Debug.WriteLine("ClassBarcodeCaption.AddBarcodeCaptionFileAsync: Popup canceled.");
                    return;
                }

                // Prompt the user for a caption
                caption = await PromptForCaptionAsync(caption);

                // Exit if caption is null or whitespace
                if (string.IsNullOrWhiteSpace(caption))
                {
                    Debug.WriteLine("ClassBarcodeCaption.AddBarcodeCaptionFileAsync: Caption is null or whitespace.");
                    return;
                }

                // Save the barcode with caption to a file
                _ = await SaveBarcodeWithCaptionFromFileAsync(ClassBarcodes.cFileBarcodePng, caption, ClassBarcodes.cFileBarcodePng, 12, barcodeType);

                // Set the image source to the saved file to display it in the Image control
                await SetImageSourceAsync(bgvBarcode, image, fileBarcodeCaptionPng);
            }
        }

        /// <summary>
        /// Capture the barcode as a screenshot and save the barcode with caption to a file.
        /// </summary>
        /// <param name="bgvBarcode"></param>
        /// <param name="image"></param>
        /// <param name="caption"></param>
        /// <param name="barcodeType"></param>
        /// <returns></returns>
        public static async Task AddBarcodeCaptionScreenAsync(BarcodeGeneratorView bgvBarcode, Image image, string caption, string barcodeType)
        {
            // Wait a short time to ensure the barcode is generated and displayed before saving it to a file
            await Task.Delay(400);

            // Save the barcode as a file by capturing the barcode view using the ZXing.Net.MAUI library
            if (Screenshot.Default.IsCaptureSupported)
            {
                // Capture the barcode view as a screenshot
                IScreenshotResult? screen = await bgvBarcode.CaptureAsync();
                await using Stream stream = await screen!.OpenReadAsync();

                // Barcode with caption
                if (ClassBarcodes.bBarcodeWithCaption)
                {
                    // Prompt the user for a caption
                    caption = await PromptForCaptionAsync(caption);

                    if (string.IsNullOrWhiteSpace(caption))
                    {
                        Debug.WriteLine("ClassBarcodeCaption.AddBarcodeCaptionScreenAsync: Caption is null or whitespace.");

                        ClassFileUtilities.SaveStreamAsFilePng(stream, ClassBarcodes.cFileBarcodePng);

                        return;     // Exit if caption is null or whitespace
                    }

                    // Save the barcode with caption to a file
                    await SaveBarcodeWithCaptionAsync(stream, caption, ClassBarcodes.cFileBarcodePng, 12, barcodeType);

                    // Set the image source to the saved file to display it in the Image control
                    await SetImageSourceAsync(bgvBarcode, image, ClassBarcodes.cFileBarcodePng);
                }

                // Barcode without caption
                else
                {
                    ClassFileUtilities.SaveStreamAsFilePng(stream, ClassBarcodes.cFileBarcodePng);
                }
            }
        }

        /// <summary>
        /// Prompt the user for a caption if it is null or whitespace, and return the entered caption.
        /// </summary>
        /// <param name="caption">Text to draw under the barcode (usually the numeric string)</param>
        /// <returns>The entered caption or the original caption if it was not null or whitespace</returns>
        private static async Task<string> PromptForCaptionAsync(string caption)
        {
            // Prompt the user for a caption if it is null or whitespace
            if (string.IsNullOrWhiteSpace(caption))
            {
                //caption = await Application.Current!.Windows[0].Page!.DisplayPromptAsync(CodeLang.ButtonCaption_Text, "");

                PopupEntry popup = await OpenPopupCaptionAsync();   // Returns the instance
                return popup.entCaption?.Text ?? string.Empty;
            }

            // Return the original caption if it was not null or whitespace
            else
            {
                return caption;
            }
        }

        /// <summary>
        /// Set the image source to the saved file to display it in the Image control
        /// </summary>
        /// <param name="bgvBarcode"></param>
        /// <param name="image"></param>
        /// <param name="fileBarcodeCaptionPng"></param>
        /// <returns></returns>
        private static async Task SetImageSourceAsync(BarcodeGeneratorView bgvBarcode, Image image, string fileBarcodeCaptionPng)
        {
            // Set the image source to the saved file to display it in the Image control
#if ANDROID
            // !!!BUG!!! in Android: returns always the first generated barcode, even when a new barcode is
            // generated and saved to the same file name. This does not happen on Windows and iOS.
            // Create a unique file name for the copied barcode PNG file to avoid caching issues on Android
            string cFileBarcodeCaptionPngUnique = Path.Combine(FileSystem.Current.CacheDirectory, $@"{DateTime.Now.Ticks}.png");
            File.Copy(fileBarcodeCaptionPng, cFileBarcodeCaptionPngUnique);
#endif
            bgvBarcode.Value = string.Empty;    // Clear the BarcodeView value to avoid displaying the barcode twice
            image.IsVisible = true;
#if ANDROID
            // Set the Image control source to the saved file
            image.Source = ImageSource.FromFile(cFileBarcodeCaptionPngUnique);

            // Delete the unique file with caption after a short delay to ensure it is not cached and displayed again on Android
            await Task.Delay(400);
            ClassFileUtilities.DeleteFileInCache(cFileBarcodeCaptionPngUnique);
#else
            // Set the Image control source to the saved file
            image.Source = ImageSource.FromFile(fileBarcodeCaptionPng);
#endif
        }

        /// <summary>
        /// Convenience helper to get a file stream and call SaveBarcodeWithCaptionAsync
        /// </summary>
        /// <param name="filePathIn">Path to the input file containing the barcode image</param>
        /// <param name="caption">Text to draw under the barcode (usually the numeric string)</param>
        /// <param name="filePathOut">Optional output file name. Defaults to "barcode_generator.png"</param>
        /// <param name="padding">Padding in pixels between barcode and caption and edges</param>
        /// <returns>Full path to the saved PNG file</returns>
        public static async Task<string> SaveBarcodeWithCaptionFromFileAsync(string filePathIn, string caption, string filePathOut = "barcode_generator.png", int padding = 12, string barcodeType = "Normal")
        {
            if (filePathIn is null || string.IsNullOrWhiteSpace(caption))
            {
                Debug.WriteLine("ClassBarcodeCaption.SaveBarcodeWithCaptionFromFileAsync: filePath or caption is null.");
                return string.Empty;
            }

            FileStream fileStream = new(filePathIn, FileMode.Open, FileAccess.Read, FileShare.Read);
            return await SaveBarcodeWithCaptionAsync(fileStream, caption, filePathOut, padding, barcodeType);
        }

        /// <summary>
        /// Create an image that contains the barcode image with the caption (number) placed under it,
        /// save the resulting PNG to the cache directory and return the full file path.
        /// </summary>
        /// <param name="barcodeStream">Stream containing the barcode image (PNG/JPEG/etc)</param>
        /// <param name="caption">Text to draw under the barcode (usually the numeric string)</param>
        /// <param name="fileName">Optional output file name. Defaults to "barcode_generator.png"</param>
        /// <param name="padding">Padding in pixels between barcode and caption and edges</param>
        /// <returns>Full path to the saved PNG file</returns>
        private static async Task<string> SaveBarcodeWithCaptionAsync(Stream barcodeStream, string caption, string fileName = "barcode_generator.png", int padding = 12, string barcodeType = "Normal")
        {
            if (barcodeStream is null)
            {
                Debug.WriteLine("ClassBarcodeCaption.SaveBarcodeWithCaptionAsync: barcodeStream is null.");
                return string.Empty;    // Return empty string on failure instead of throwing, to avoid crashing the app
            }

            caption ??= string.Empty;
            //caption = "AB123W5M5IJ";  // For testing only

            fileName = string.IsNullOrWhiteSpace(fileName) ? "barcode_generator.png" : fileName;

            // Do the CPU-bound image work on a background thread
            return await Task.Run(() =>
            {
                try
                {
                    // Ensure stream is readable from beginning
                    if (barcodeStream.CanSeek)
                    {
                        barcodeStream.Seek(0, SeekOrigin.Begin);
                    }

                    // Decode input image to SKBitmap
                    using Stream codecStream = barcodeStream.CanSeek ? barcodeStream : CopyToMemoryStream(barcodeStream);
                    using SKBitmap? skBitmap = SKBitmap.Decode(codecStream);
                    
                    if (skBitmap is null)
                    {
                        Debug.WriteLine("ClassBarcodeCaption.SaveBarcodeWithCaptionAsync: Failed to decode barcode image stream.");
                        return string.Empty;    // Return empty string on failure instead of throwing, to avoid crashing the app
                    }

                    int srcWidth = skBitmap.Width;
                    int srcHeight = skBitmap.Height;

                    // Determine colors (Globals stores as "AARRGGBB" without '#')
                    SKColor fgColor;
                    SKColor bgColor;
                    string fontFamily1;      // Default font family
                    string fontFamily2;      // Fallback font family

                    if (barcodeType == "ArtQRcode" || barcodeType == "ArtQRcode2")
                    {
                        // Set specific colors and font families for Art QR codes
                        fgColor = TryParseSkColor(ClassBarcodes.cCodeColorFgArtQRCode, SKColors.Black);
                        bgColor = TryParseSkColor(ClassBarcodes.cCodeColorBgArtQRCode, SKColors.White);
                        fontFamily1 = "OpenSansRegular";
                        fontFamily2 = "serif";
                    }

                    else if (barcodeType == "QRcode" || barcodeType == "QRcode2" || barcodeType == "2D")
                    {
                        // Set specific colors and font families for QR codes and other 2D barcodes
                        fgColor = TryParseSkColor(ClassBarcodes.cCodeColorFg, SKColors.Black);
                        bgColor = TryParseSkColor(ClassBarcodes.cCodeColorBg, SKColors.White);
                        fontFamily1 = "OpenSansRegular";
                        fontFamily2 = "serif";
                    }

                    else
                    {
                        // Set default colors and font families for the other barcodes like 1D barcodes
                        fgColor = TryParseSkColor(ClassBarcodes.cCodeColorFg, SKColors.Black);
                        bgColor = TryParseSkColor(ClassBarcodes.cCodeColorBg, SKColors.White);
                        fontFamily1 = "Courier New";
                        fontFamily2 = "monospace";
                    }

                    // Determine font size relative to image width if not provided
                    float fontSize = Math.Max(14f, srcWidth / 16f);

                    // Select a font Typeface - Try a common Windows name, a generic monospace, then default
                    using SKTypeface typeface = SKTypeface.FromFamilyName(fontFamily1, SKFontStyle.Normal)
                                     ?? SKTypeface.FromFamilyName(fontFamily2, SKFontStyle.Normal)
                                     ?? SKTypeface.Default;
                    using SKFont font = new(typeface, fontSize);

                    // Prepare paint for text drawing (color, antialias)
                    using SKPaint textPaint = new()
                    {
                        IsAntialias = true,
                        Color = fgColor,
                        Style = SKPaintStyle.Fill
                    };

                    // Ensure caption fits horizontally; reduce font size if necessary
                    float maxTextWidth = srcWidth - (padding * 4);
                    const float minFontSize = 9f;
                    int attempts = 0;
                    SKRect textBounds = new();
                    float measuredWidth = font.MeasureText(caption, out textBounds);
                    
                    while (measuredWidth > maxTextWidth && font.Size > minFontSize && attempts++ < 60)
                    {
                        font.Size -= 1.5f;
                        measuredWidth = font.MeasureText(caption, out textBounds);
                    }

                    // Reduce font size slightly more to ensure it fits well within the width
                    font.Size -= 1.0f;
                    
                    Debug.WriteLine($"ClassBarcodeCaption.SaveBarcodeWithCaptionAsync: Final font size for caption '{caption}' is {font.Size} after {attempts} attempts to fit within {maxTextWidth}px width.");

                    // Compute caption height (approximate line height) from font metrics
                    SKFontMetrics metrics = font.Metrics;
                    float textHeight = metrics.Descent - metrics.Ascent;
                    int captionHeight = (int)Math.Ceiling(textHeight) + padding * 2;

                    // Special handling for Art QR codes and QR codes with quiet zone size > 2
                    if ((barcodeType == "ArtQRcode" || barcodeType == "QRcode") && ClassBarcodes.nQRCodeQuietZoneSize > 2)
                    {
                        captionHeight /= 2;
                    }
                    
                    else if ((barcodeType == "ArtQRcode2" || barcodeType == "QRcode2") && ClassBarcodes.nQRCodeQuietZoneSize2 > 2)
                    {
                        captionHeight /= 2;
                    }

                    // Create new bitmap with extra space for caption
                    int outWidth = srcWidth;
                    int outHeight = srcHeight + captionHeight;

                    using SKBitmap outBitmap = new(outWidth, outHeight, SKColorType.Bgra8888, SKAlphaType.Premul);
                    using SKCanvas canvas = new(outBitmap);

                    // Draw background
                    canvas.Clear(bgColor);

                    // Draw the barcode bitmap centered horizontally at top
                    // If source width differs from outWidth, scale proportionally to outWidth
                    float scaleX = (float)outWidth / srcWidth;
                    float scaleY = scaleX; // keep aspect ratio
                    SKRect destRect = SKRect.Create(0, 0, srcWidth * scaleX, srcHeight * scaleY);
                    
                    // Center horizontally if destRect.Width < outWidth due to rounding
                    destRect.Left = (outWidth - destRect.Width) / 2f;
                    destRect.Top = 0;

                    // Use an SKPaint when calling DrawBitmap overload that accepts SKRect; avoid obsolete FilterQuality
                    using SKPaint paintImage = new() { IsAntialias = true };
                    canvas.DrawBitmap(skBitmap, destRect, SKSamplingOptions.Default, paintImage);

                    // Draw caption centered horizontally below the image
                    float textX = outWidth / 2f;

                    // baseline Y: top of caption area + padding + absolute ascent
                    float textY = srcHeight + padding - metrics.Ascent; // ascent is negative

                    // Special handling for Art QR codes and QR codes with quiet zone size > 2
                    if ((barcodeType == "ArtQRcode" || barcodeType == "QRcode") && ClassBarcodes.nQRCodeQuietZoneSize > 2)
                    {
                        textY -= captionHeight;
                    }

                    else if ((barcodeType == "ArtQRcode2" || barcodeType == "QRcode2") && ClassBarcodes.nQRCodeQuietZoneSize2 > 2)
                    {
                        textY -= captionHeight;
                    }

                    canvas.DrawText(caption, textX, textY, SKTextAlign.Center, font, textPaint);

                    // Encode to PNG
                    using SKImage image = SKImage.FromBitmap(outBitmap);
                    using SKData data = image.Encode(SKEncodedImageFormat.Png, 100);

                    // Write to file
                    using (FileStream file = File.Open(fileName, FileMode.Create, FileAccess.Write))
                    {
                        data.SaveTo(file);
                    }

                    return fileName;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"ClassBarcodeCaption.SaveBarcodeWithCaptionAsync error: {ex}");
                    return string.Empty;    // Return empty string on failure instead of throwing, to avoid crashing the app
                }
            });
        }

        // Helper: copy to MemoryStream when original stream is not seekable
        private static MemoryStream CopyToMemoryStream(Stream src)
        {
            MemoryStream ms = new();
            src.CopyTo(ms);
            ms.Seek(0, SeekOrigin.Begin);
            return ms;
        }

        // Helper: parse "AARRGGBB" or "#AARRGGBB" or "RRGGBB" into SKColor, fallback to defaultColor on failure
        private static SKColor TryParseSkColor(string? colorString, SKColor defaultColor)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(colorString))
                {
                    return defaultColor;
                }

                string s = colorString.Trim();
                if (!s.StartsWith('#'))
                {
                    s = "#" + s;
                }

                return SKColor.Parse(s);
            }
            catch
            {
                return defaultColor;
            }
        }

        /// <summary>
        /// Show a modal popup to inform the user about the recommended image size before opening the file picker
        /// </summary>
        /// <returns></returns>
        private static async Task<PopupEntry> OpenPopupCaptionAsync()
        {
            Page? currentPage = Application.Current?.Windows.Count > 0 ? Application.Current.Windows[0]?.Page : null;

            if (currentPage != null)
            {
                // Create/show the PopupEntry and await user input, then return the instance
                var popup = new PopupEntry();
                await currentPage.ShowPopupAsync(popup);

                return popup;
            }
            
            return null!;
        }
    }
}
