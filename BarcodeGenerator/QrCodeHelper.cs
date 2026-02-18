using QRCoder;
using SkiaSharp;

namespace BarcodeGenerator
{
    public static class QrCodeHelper
    {
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
        /// <param name="text">The text to encode within the generated QR code.</param>
        /// <param name="logoStream">A stream containing the logo image to overlay at the center of the QR code, or null to generate a QR code
        /// without a logo. The stream must be positioned at the beginning.</param>
        /// <returns>An ImageSource representing the generated QR code image, including the logo overlay if provided.</returns>
        public static ImageSource GenerateQrWithLogo(string text, Stream? logoStream)
        {
            // Generate QR code data with high error correction level to allow for logo overlay
            var generator = new QRCodeGenerator();
            var qrData = generator.CreateQrCode(text, QRCodeGenerator.ECCLevel.H);

            var modules = qrData.ModuleMatrix;
            int moduleCount = modules.Count;
            int pixelsPerModule = 20;
            int size = moduleCount * pixelsPerModule;

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
                using var logoBitmap = SKBitmap.Decode(logoStream);
                
                if (logoBitmap != null)
                {
                    float iconSize = size * 0.18f;                  // Scale logo to 18% of QR code size
                    float left = (size - iconSize) / 2f;
                    float top = (size - iconSize) / 2f;
                    var dest = new SKRect(left: left, top: top, right: left + iconSize, bottom: top + iconSize);

                    // Optional: draw a border around the icon
                    var borderPaint = new SKPaint { Style = SKPaintStyle.Fill, Color = SKColor.Parse(Globals.cCodeColorBg) };
                    float borderPadding = 4;
                    var borderRect = new SKRect(dest.Left - borderPadding, dest.Top - borderPadding,
                                                 dest.Right + borderPadding, dest.Bottom + borderPadding);
                    canvas.DrawRoundRect(rect: borderRect, rx: 4, ry: 4, paint: borderPaint);

                    // Draw the image centered on the QR code
                    canvas.DrawBitmap(logoBitmap, dest);
                }
            }

            canvas.Flush();

            // Convert SKBitmap to ImageSource
            using var image = SKImage.FromBitmap(bitmap);
            using var encoded = image.Encode(SKEncodedImageFormat.Png, 100);
            var ms = new MemoryStream();
            encoded.SaveTo(ms);
            ms.Position = 0;
            
            return ImageSource.FromStream(() => ms);
        }

        /// <summary>
        /// Opens a file picker dialog and returns the selected file.
        /// </summary>
        /// <param name="options">The options for the file picker.</param>
        /// <returns>The selected file result, or null if no file was selected.</returns>
        public static async Task<FileResult?> PickImage(PickOptions options)
        {
            try
            {
                var result = await FilePicker.Default.PickAsync(options);
                
                if (result != null)
                {
                    if (result.FileName.EndsWith("jpg", StringComparison.OrdinalIgnoreCase) ||
                        result.FileName.EndsWith("png", StringComparison.OrdinalIgnoreCase))
                    {
                        using var stream = await result.OpenReadAsync();
                        var image = ImageSource.FromStream(() => stream);
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                // The user canceled or something went wrong
                Debug.WriteLine($"File picking error: {ex.Message}");
            }

            return null;
        }
    }
}
