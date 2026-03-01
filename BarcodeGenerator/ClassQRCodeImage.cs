using CommunityToolkit.Maui.Extensions;
using QRCoder;
using SkiaSharp;

namespace BarcodeGenerator
{
    public static class ClassQRCodeImage
    {
        // Global variable to control the size of the image as a percentage of the QR code size.
        public static float nQRCodeImageSizePercent;

        /// <summary>
        /// Generates a QR code image from the specified text, optionally overlaying a centered logo image.
        /// </summary>
        /// <remarks>The QR code is generated with a fixed pixel size per module. If a logo is provided,
        /// it is scaled to 20% of the QR code's size and centered, with a border for improved visibility. The
        /// returned ImageSource is suitable for use in UI frameworks that support image sources.
        /// The ECC (Error Correction Code) levels for QR codes are as follows:
        /// Level L: Corrects up to 7% of data damage.
        /// Level M: Corrects up to 15% of data damage.
        /// Level Q: Corrects up to 25% of data damage.
        /// Level H: Corrects up to 30% of data damage.
        /// The higher the ECC level, the more data can be recovered, but it also increases the size of the QR code.
        /// </remarks>
        /// <param name="text">The text to encode within the generated QR code.
        /// without a logo. The stream must be positioned at the beginning.</param>
        /// <returns>An ImageSource representing the generated QR code image, including the logo overlay if provided.</returns>
        public static async Task<ImageSource> GenerateQrWithLogo(string text)
        {
            // Generate QR code data with high error correction level to allow for logo overlay
            var generator = new QRCodeGenerator();
            var qrData = generator.CreateQrCode(text, QRCodeGenerator.ECCLevel.H);

            var modules = qrData.ModuleMatrix;
            int moduleCount = modules.Count;
            int pixelsPerModule = 20;
            int size = moduleCount * pixelsPerModule;
            Debug.WriteLine($"QR code generated with module count: {moduleCount}, size: {size}x{size}, pixels per module: {pixelsPerModule}");

            // Calculate the recommended image size based on the QR code size and the configured percentage
            int nImageRecommendedSize = (int)(size * nQRCodeImageSizePercent / 100f);

            // Show a DisplayAlertAsync to inform the user about the recommended image size before opening the file picker
            //await Application.Current!.Windows[0].Page!.DisplayAlertAsync(CodeLang.QRCodeRecommendedImageSize_Text, $"{nImageRecommendedSize} {CodeLang.Pixels_Text}", CodeLang.ButtonClose_Text);

            // Show a modal popup to inform the user about the recommended image size before opening the file picker
            var currentPage = Application.Current?.Windows.Count > 0 ? Application.Current.Windows[0]?.Page : null;
            if (currentPage != null)
            {
                MainPage.bIsPopupMessage = true;
                _ = await currentPage.ShowPopupAsync(new PopupMessage(4, $"{CodeLang.QRCodeRecommendedImageSize_Text}:\n\n{nImageRecommendedSize} {CodeLang.Pixels_Text}"));
            }

            // Open the file picker to select an image file
            FileResult? cFile = await ClassFileOperations.PickImage();
            MainPage.bIsPopupMessage = false;

            // Read the selected file as a stream
            Stream? logoStream = null;
            if (cFile != null)
            {
                logoStream = await cFile.OpenReadAsync();
            }

            // Calculate the maximum image size in pixels based on the QR code size and the configured percentage
            using var bitmap = new SKBitmap(width: size, height: size);
            using var canvas = new SKCanvas(bitmap);
            canvas.Clear(SKColor.Parse(Globals.cCodeColorBg));

            var paint = new SKPaint { Style = SKPaintStyle.Fill, Color = SKColor.Parse(Globals.cCodeColorFg), IsAntialias = false };

            // Draw QR code modules
            for (int y = 0; y < moduleCount; y++)
            {
                for (int x = 0; x < moduleCount; x++)
                {
                    bool moduleOn = modules[y][x];

                    if (moduleOn)
                    {
                        var rect = new SKRect(x * pixelsPerModule, y * pixelsPerModule,
                                              (x + 1) * pixelsPerModule, (y + 1) * pixelsPerModule);
                        canvas.DrawRect(rect, paint);
                    }
                }
            }

            // Draw logo if provided
            if (logoStream != null)
            {
                logoStream.Position = 0;

                // Use SKCodec to read encoded origin (EXIF orientation) so we can correct rotation
                using var codec = SKCodec.Create(logoStream);

                SKBitmap? logoBitmap = null;
                if (codec != null)
                {
                    var info = codec.Info;
                    logoBitmap = new SKBitmap(info.Width, info.Height, info.ColorType, info.AlphaType);
                    codec.GetPixels(logoBitmap.Info, logoBitmap.GetPixels());

                    // Fix orientation according to EXIF
                    var oriented = FixOrientation(logoBitmap, codec.EncodedOrigin);
                    if (!ReferenceEquals(oriented, logoBitmap))
                    {
                        logoBitmap.Dispose();
                        logoBitmap = oriented;
                    }
                }
                else
                {
                    // Fallback to simple decoding if codec not available
                    logoStream.Position = 0;
                    logoBitmap = SKBitmap.Decode(logoStream);
                }

                if (logoBitmap != null && logoBitmap.Width > 0 && logoBitmap.Height > 0)
                {
                    // Maximum box for the logo (as a fraction of the QR size)
                    float iconMaxSize = size * nQRCodeImageSizePercent / 100f;
                    Debug.WriteLine($"Original logo size: {logoBitmap.Width}x{logoBitmap.Height}, QR code size: {size}x{size}, icon max size: {iconMaxSize}x{iconMaxSize}");

                    // Compute scale to fit the logo inside a square of iconMaxSize while preserving aspect ratio.
                    float scale = Math.Min(iconMaxSize / logoBitmap.Width, iconMaxSize / logoBitmap.Height);

                    // Optionally avoid upscaling to reduce pixelation
                    // If you want to allow upscaling, remove the scale = Math.Min(1f, scale) clamp or increase iconMaxSize.
                    //scale = Math.Min(1f, scale);

                    float destWidth = logoBitmap.Width * scale;
                    float destHeight = logoBitmap.Height * scale;

                    float left = (size - destWidth) / 2f;
                    float top = (size - destHeight) / 2f;
                    var dest = new SKRect(left, top, left + destWidth, top + destHeight);

                    // Draw a background/border behind the logo for contrast
                    var borderPaint = new SKPaint
                    {
                        Style = SKPaintStyle.Fill,
                        Color = SKColor.Parse(Globals.cCodeColorBg),
                        IsAntialias = true
                    };

                    float borderPadding = Math.Max(4f, iconMaxSize * 0.06f);
                    var borderRect = new SKRect(dest.Left - borderPadding, dest.Top - borderPadding,
                                                 dest.Right + borderPadding, dest.Bottom + borderPadding);
                    canvas.DrawRoundRect(borderRect, 6f, 6f, borderPaint);

                    // Draw the logo scaled, centered, and preserving aspect ratio
                    canvas.DrawBitmap(logoBitmap, dest);
                }

                logoBitmap?.Dispose();
            }

            canvas.Flush();

            // Convert SKBitmap to ImageSource
            using var image = SKImage.FromBitmap(bitmap);
            using var encoded = image.Encode(SKEncodedImageFormat.Png, 100);
            var ms = new MemoryStream();
            encoded.SaveTo(ms);
            ms.Position = 0;

            // Save the generated PNG to disk with the original pixel size for sharing or other purposes
            using MemoryStream memoryStream = new(ms.ToArray());
            string cFileName = Path.Combine(FileSystem.Current.CacheDirectory, "qr_code_image.png");
            _ = ClassFileOperations.SavePngFromStreamAsync(memoryStream, cFileName);

            // Return the ImageSource for use in the UI
            return ImageSource.FromStream(() => ms);
        }

        /// <summary>
        /// Adjusts the orientation of the specified bitmap according to the provided EXIF encoded origin.
        /// </summary>
        /// <remarks>This method supports all standard EXIF orientation values, including rotations and
        /// flips, to ensure the bitmap is displayed as intended. The original bitmap is not modified.</remarks>
        /// <param name="src">The source bitmap to be reoriented.</param>
        /// <param name="origin">The EXIF encoded origin value that specifies the intended orientation of the bitmap.</param>
        /// <returns>A new SKBitmap instance with the orientation corrected based on the specified origin.</returns>
        private static SKBitmap FixOrientation(SKBitmap src, SKEncodedOrigin origin)
        {
            switch (origin)
            {
                case SKEncodedOrigin.TopLeft:
                    return src;
                case SKEncodedOrigin.TopRight: // flip horizontal
                    return FlipBitmap(src, horizontal: true);
                case SKEncodedOrigin.BottomRight: // rotate 180
                    return RotateBitmap(src, 180);
                case SKEncodedOrigin.BottomLeft: // flip vertical
                    return FlipBitmap(src, horizontal: false);
                case SKEncodedOrigin.LeftTop: // transpose (rotate 90 and flip horizontal)
                    {
                        var r = RotateBitmap(src, 90);
                        var f = FlipBitmap(r, horizontal: true);
                        r.Dispose();
                        return f;
                    }
                case SKEncodedOrigin.RightTop: // rotate 90
                    return RotateBitmap(src, 90);
                case SKEncodedOrigin.RightBottom: // transverse (rotate 270 and flip horizontal)
                    {
                        var r = RotateBitmap(src, 270);
                        var f = FlipBitmap(r, horizontal: true);
                        r.Dispose();
                        return f;
                    }
                case SKEncodedOrigin.LeftBottom: // rotate 270
                    return RotateBitmap(src, 270);
                default:
                    return src;
            }
        }

        /// <summary>
        /// Rotates the specified bitmap by the given angle in degrees and returns a new bitmap containing the rotated
        /// image.
        /// </summary>
        /// <remarks>If the rotation angle is a multiple of 180 degrees, the returned bitmap has the same
        /// width and height as the source. For angles of 90 or 270 degrees, the width and height are swapped in the
        /// result. The returned bitmap uses the same color and alpha types as the source.</remarks>
        /// <param name="src">The source bitmap to rotate. Cannot be null.</param>
        /// <param name="degrees">The angle, in degrees, by which to rotate the bitmap. Valid values are typically 90, 180, or 270.</param>
        /// <returns>A new SKBitmap instance containing the rotated image. The original bitmap is not modified.</returns>
        private static SKBitmap RotateBitmap(SKBitmap src, float degrees)
        {
            if (degrees % 180 == 0)
            {
                int w = src.Width;
                int h = src.Height;
                var dest = new SKBitmap(new SKImageInfo(w, h, src.ColorType, src.AlphaType));
                using (var canvas = new SKCanvas(dest))
                {
                    canvas.Clear(SKColors.Transparent);
                    canvas.Translate(w / 2f, h / 2f);
                    canvas.RotateDegrees(degrees);
                    canvas.Translate(-src.Width / 2f, -src.Height / 2f);
                    canvas.DrawBitmap(src, 0, 0);
                    canvas.Flush();
                }
                return dest;
            }
            else
            {
                // swap width/height for 90/270
                int w = src.Height;
                int h = src.Width;
                var dest = new SKBitmap(new SKImageInfo(w, h, src.ColorType, src.AlphaType));
                using (var canvas = new SKCanvas(dest))
                {
                    canvas.Clear(SKColors.Transparent);
                    canvas.Translate(w / 2f, h / 2f);
                    canvas.RotateDegrees(degrees);
                    canvas.Translate(-src.Width / 2f, -src.Height / 2f);
                    canvas.DrawBitmap(src, 0, 0);
                    canvas.Flush();
                }
                return dest;
            }
        }

        /// <summary>
        /// Creates a new bitmap that is a flipped version of the specified source bitmap, either horizontally or
        /// vertically.
        /// </summary>
        /// <remarks>The returned bitmap has the same dimensions and color settings as the source. The
        /// destination bitmap is cleared to transparent before the flip is applied. Ensure that the source bitmap is
        /// valid and properly initialized before calling this method.</remarks>
        /// <param name="src">The source bitmap to flip. This parameter must not be null.</param>
        /// <param name="horizontal">A value indicating the direction of the flip. If <see langword="true"/>, the bitmap is flipped horizontally;
        /// otherwise, it is flipped vertically.</param>
        /// <returns>A new <see cref="SKBitmap"/> instance containing the flipped image. The original bitmap is not modified.</returns>
        private static SKBitmap FlipBitmap(SKBitmap src, bool horizontal)
        {
            var dest = new SKBitmap(new SKImageInfo(src.Width, src.Height, src.ColorType, src.AlphaType));
            using (var canvas = new SKCanvas(dest))
            {
                canvas.Clear(SKColors.Transparent);
                if (horizontal)
                {
                    canvas.Translate(src.Width, 0);
                    canvas.Scale(-1, 1);
                }
                else
                {
                    canvas.Translate(0, src.Height);
                    canvas.Scale(1, -1);
                }

                canvas.DrawBitmap(src, 0, 0);
                canvas.Flush();
            }
            return dest;
        }
    }
}
