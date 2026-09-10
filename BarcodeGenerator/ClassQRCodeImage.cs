using CommunityToolkit.Maui.Extensions;
using SkiaSharp;
using SkiaSharp.QrCode;
using SkiaSharp.QrCode.Image;

namespace BarcodeGenerator
{
    internal class ClassQRCodeImage
    {
        /// <summary>
        /// Generates a QR code image from the specified text, optionally overlaying a centered image
        /// </summary>
        /// <remarks>The QR code is generated with a fixed pixel size per module.
        /// The fixed pixel size defines how many pixels in the generated SKBitmap correspond to one QR 'module'
        /// (one dark/light square in the QR matrix).
        /// Together with moduleCount it determines the output bitmap size: size = moduleCount * pixelsPerModule.
        /// If an image is provided, it is scaled from 10% to 35% (value set in the settings of the app)
        /// of the QR code's size and centered, with a border for improved visibility.
        /// The returned ImageSource is suitable for use in UI frameworks that support image sources.
        /// The ECC (Error Correction Code) levels for QR codes are as follows:
        /// Level L (Low): Corrects up to 7% of data damage.
        /// Level M (Medium): Corrects up to 15% of data damage.
        /// Level Q (Quartile): Corrects up to 25% of data damage.
        /// Level H (High): Corrects up to 30% of data damage.
        /// The higher the ECC level, the more data can be recovered, but it also increases the size of the QR code.
        /// </remarks>
        /// <param name="text">The text to encode within the generated QR code.
        /// without a logo. The stream must be positioned at the beginning.</param>
        /// <returns>An ImageSource representing the generated QR code image, including the logo overlay if provided.</returns>
        public static async Task<ImageSource?> GenerateQrCodeImageAsync(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return null;
            }

            QRCodeImageBuilder? standardQrData = null;
            byte[] pngBytes;

            try
            {
                standardQrData = new QRCodeImageBuilder(text)
                    .WithErrorCorrection(ECCLevel.H)
                    .WithColors(codeColor: ClassQRCodes.SkColorFromHex(ClassBarcodes.cCodeColorFg), backgroundColor: ClassQRCodes.SkColorFromHex(ClassBarcodes.cCodeColorBg), clearColor: SKColors.Transparent)
                    .WithQuietZone(ClassBarcodes.nQRCodeQuietZoneSize);

                // Only apply size if the user has not selected a variable size (to avoid overriding the variable size setting)
                if (!ClassBarcodes.bQRCodeSizeVariable)
                {
                    standardQrData = standardQrData.WithSize(ClassBarcodes.nQRCodeSizePixels, ClassBarcodes.nQRCodeSizePixels);
                }

                pngBytes = standardQrData.ToByteArray();
            }
            catch (Exception ex)
            {
#if DEBUG                
                await Application.Current!.Windows[0].Page!.DisplayAlertAsync(CodeLang.Barcode_QR_CODE_IMAGE_Text, ex.Message, CodeLang.ButtonClose_Text);
#endif
                return null;
            }

            // Decode generated PNG into a bitmap and draw that bitmap instead of trying to build a BitArray module matrix.
            using SKBitmap? sourceQrBitmap = SKBitmap.Decode(pngBytes);
            if (sourceQrBitmap == null || sourceQrBitmap.Width == 0)
            {
                await Application.Current!.Windows[0].Page!.DisplayAlertAsync(CodeLang.Barcode_QR_CODE_IMAGE_Text, "Failed to decode generated QR image.", CodeLang.ButtonClose_Text);
                return null;
            }

            // Determine final drawing size. Use generated bitmap size unless user requested a fixed output size.
            int size = sourceQrBitmap.Width;
            if (!ClassBarcodes.bQRCodeSizeVariable)
            {
                // If non-variable requested, scale the QR to the configured pixel size.
                // We will render into a bitmap of that pixel size.
                size = ClassBarcodes.nQRCodeSizePixels;
            }

            Debug.WriteLine($"QR code generated (decoded) with source size: {sourceQrBitmap.Width}, drawing size: {size}x{size}");

            // Calculate the recommended image size based on the QR code drawing size and the configured percentage
            int nImageRecommendedSize = (int)(size * ClassBarcodes.nQRCodeImageSizePercent / 100.0f);

            Stream? logoStream = null;

            if (ClassBarcodes.cQRCodeType == ClassBarcodes.cBarcode_QR_CODE_IMAGE)
            {
                Page? currentPage = Application.Current?.Windows.Count > 0 ? Application.Current.Windows[0]?.Page : null;
                if (currentPage != null)
                {
                    await currentPage.ShowPopupAsync(new PopupMessage(20, CodeLang.QRCodeImageForegroundTitle_Text, $"{CodeLang.QRCodeRecommendedImageSize_Text}:\n\n{nImageRecommendedSize:N0} x {nImageRecommendedSize:N0} {CodeLang.Pixels_Text}"));

                    if (Globals.bPopupCanceled)
                    {
                        Globals.bPopupCanceled = false;
                        return null;
                    }
                }

                FileResult? cFileForeground = await ClassFileUtilities.PickImage();
                if (cFileForeground != null)
                {
                    logoStream = await cFileForeground.OpenReadAsync();
                }
            }

            // Create destination bitmap with final size and draw the decoded QR bitmap into it (scaling if necessary)
            using SKBitmap bitmap = new(width: size, height: size);
            using SKCanvas canvas = new(bitmap);
            canvas.Clear(SKColor.Parse(ClassBarcodes.cCodeColorBg));

            SKPaint paint = new() { Style = SKPaintStyle.Fill, Color = SKColor.Parse(ClassBarcodes.cCodeColorFg), IsAntialias = false };

            // Draw the generated QR bitmap into the canvas. Use nearest filtering to keep modules crisp when scaling.
            SKRect destRect = new SKRect(0, 0, size, size);
            canvas.DrawBitmap(sourceQrBitmap, destRect, new SKSamplingOptions(SKFilterMode.Nearest, SKMipmapMode.None));

            // Draw logo if provided (rest of your existing logic unchanged)...
            if (logoStream != null)
            {
                logoStream.Position = 0;
                using SKCodec codec = SKCodec.Create(logoStream);

                SKBitmap? logoBitmap = null;
                if (codec != null)
                {
                    SKImageInfo info = codec.Info;
                    logoBitmap = new SKBitmap(info.Width, info.Height, info.ColorType, info.AlphaType);
                    codec.GetPixels(logoBitmap.Info, logoBitmap.GetPixels());

                    SKBitmap oriented = ClassImageUtilities.FixOrientation(logoBitmap, codec.EncodedOrigin);
                    if (!ReferenceEquals(oriented, logoBitmap))
                    {
                        logoBitmap.Dispose();
                        logoBitmap = oriented;
                    }
                }
                else
                {
                    logoStream.Position = 0;
                    logoBitmap = SKBitmap.Decode(logoStream);
                }

                if (logoBitmap != null && logoBitmap.Width > 0 && logoBitmap.Height > 0)
                {
                    float iconMaxSize = size * ClassBarcodes.nQRCodeImageSizePercent / 100.0f;
                    float scale = Math.Min(iconMaxSize / logoBitmap.Width, iconMaxSize / logoBitmap.Height);
                    float destWidth = logoBitmap.Width * scale;
                    float destHeight = logoBitmap.Height * scale;
                    float left = (size - destWidth) / 2f;
                    float top = (size - destHeight) / 2f;
                    SKRect dest = new(left, top, left + destWidth, top + destHeight);

                    SKPaint borderPaint = new()
                    {
                        Style = SKPaintStyle.Fill,
                        Color = SKColor.Parse(ClassBarcodes.cCodeColorBg),
                        IsAntialias = true
                    };

                    float borderPadding = ClassBarcodes.nQRCodeImageSizeBorder;
                    SKRect borderRect = new(dest.Left - borderPadding, dest.Top - borderPadding,
                                                 dest.Right + borderPadding, dest.Bottom + borderPadding);
                    canvas.DrawRoundRect(borderRect, 6f, 6f, borderPaint);

                    canvas.DrawBitmap(logoBitmap, dest, new SKSamplingOptions(SKFilterMode.Linear, SKMipmapMode.Linear));
                }

                logoBitmap?.Dispose();
            }

            canvas.Flush();

            using SKImage image = SKImage.FromBitmap(bitmap);
            using SKData encoded = image.Encode(SKEncodedImageFormat.Png, 100);
            MemoryStream ms = new();
            encoded.SaveTo(ms);
            ms.Position = 0;

            using MemoryStream memoryStream = new(ms.ToArray());
            _ = ClassFileUtilities.SavePngFromStreamAsync(memoryStream, ClassBarcodes.cFileBarcodePng);

            return ImageSource.FromStream(() => ms);
        }
    }
}
