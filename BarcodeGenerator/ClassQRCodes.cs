using QRCoder;
using SkiaSharp;
using SkiaSharp.QrCode;
using SkiaSharp.QrCode.Image;

namespace BarcodeGenerator
{
    internal class ClassQRCodes
    {
        /// <summary>
        /// Generates a QR code image from the specified text asynchronously.
        /// </summary>
        /// <remarks>The generated QR code uses a fixed error correction level and version. The
        /// returned ImageSource can be used directly in UI elements that support image sources. The generated PNG and SVG image
        /// is also saved to disk for sharing or further processing.</remarks>
        /// <param name="text">The text to encode in the QR code. Cannot be null or empty.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an ImageSource representing the
        /// generated QR code, or null if the code could not be generated.</returns>

        public static async Task<ImageSource?> GenerateQrCodeAsync(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return null;
            }

            try
            {
                // Create QR code data first so we can reuse it for both PNG and SVG and compute pixels per module
                using QRCoder.QRCodeGenerator qrGenerator = new();
                using QRCoder.QRCodeData qrData = qrGenerator.CreateQrCode(text, QRCoder.QRCodeGenerator.ECCLevel.M);

                // Compute pixels per module so final image is approximately ClassBarcodes.nQRCodeSizePixels
                int moduleCount = qrData.ModuleMatrix.Count;
                int pixelsPerModule = Math.Max(1, ClassBarcodes.nQRCodeSizePixels / moduleCount);

                // Generate the QR code PNG with custom foreground and background colors
                PngByteQRCode pngQr = new(qrData);
                byte[] pngBytes = pngQr.GetGraphic(
                    pixelsPerModule,
                    System.Drawing.Color.FromArgb(Convert.ToInt32(ClassBarcodes.cCodeColorFg, 16)),    // foreground (dark) color
                    System.Drawing.Color.FromArgb(Convert.ToInt32(ClassBarcodes.cCodeColorBg, 16)), // background (light) color
                    drawQuietZones: true);

                // Save the byte array as a PNG file
                await ClassFileUtilities.SavePngFromStreamAsync(new MemoryStream(pngBytes), ClassBarcodes.cFileBarcodePng);

                // Generate the QR code as an SVG string with the same colors and save it to disk for sharing or other purposes
                SvgQRCode svgQr = new(qrData);
                string qrCodeAsSvg = svgQr.GetGraphic(
                    pixelsPerModule,
                    System.Drawing.Color.FromArgb(Convert.ToInt32(ClassBarcodes.cCodeColorFg, 16)),
                    System.Drawing.Color.FromArgb(Convert.ToInt32(ClassBarcodes.cCodeColorBg, 16)),
                    drawQuietZones: true);

                // Save the string 'qrCodeAsSvg' as a SVG file
                ClassFileUtilities.SaveStringAsFileSvg(qrCodeAsSvg, ClassBarcodes.cFileBarcodeSvg);

                // Return an ImageSource that opens a fresh stream when needed
                return ImageSource.FromStream(() => new MemoryStream(pngBytes));
            }
            catch (Exception ex)
            {
#if DEBUG
                await Application.Current!.Windows[0].Page!.DisplayAlertAsync(CodeLang.Barcode_MICRO_QR_CODE_Text, ex.Message, CodeLang.ButtonClose_Text);
#endif
                return null;
            }
        }

        /// <summary>
        /// Generates a Micro QR code image from the specified text asynchronously.
        /// </summary>
        /// <remarks>The generated Micro QR code uses a fixed error correction level and version. The
        /// returned ImageSource can be used directly in UI elements that support image sources. The generated PNG and SVG image
        /// is also saved to disk for sharing or further processing.</remarks>
        /// <param name="text">The text to encode in the Micro QR code. Cannot be null or empty.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an ImageSource representing the
        /// generated Micro QR code, or null if the code could not be generated.</returns>
        public static async Task<ImageSource?> GenerateMicroQrCodeAsync(string text, int nVersion = -4)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return null;
            }

            try
            {
                // Generate the Micro QR code as a PNG byte array
                // Fix the symbol height; the width is selected automatically
                byte[] pngBytes = new MicroQRCodeImageBuilder(text)
                    .WithErrorCorrection(MicroQREccLevel.M)
                    .WithModulePixelSize(12)
                    .WithColors(codeColor: SkColorFromHex(ClassBarcodes.cCodeColorFg), backgroundColor: SkColorFromHex(ClassBarcodes.cCodeColorBg))
                    .ToByteArray();

                // Save the byte array 'qrCodeImage' as a PNG file
                await ClassFileUtilities.SavePngFromStreamAsync(new MemoryStream(pngBytes), ClassBarcodes.cFileBarcodePng);

                // Generate the Micro QR code as an SVG string and save it to disk for sharing or other purposes
                string qrCodeAsSvg = new MicroQRCodeImageBuilder(text)
                    .WithErrorCorrection(MicroQREccLevel.M)
                    .WithModulePixelSize(12)
                    .WithColors(codeColor: SkColorFromHex(ClassBarcodes.cCodeColorFg), backgroundColor: SkColorFromHex(ClassBarcodes.cCodeColorBg))
                    .ToSvgString();

                // Save the string 'qrCodeAsSvg' as a SVG file
                ClassFileUtilities.SaveStringAsFileSvg(qrCodeAsSvg, ClassBarcodes.cFileBarcodeSvg);

                // Return an ImageSource that opens a fresh stream when needed
                return ImageSource.FromStream(() => new MemoryStream(pngBytes));
            }
            catch (Exception ex)
            {
#if DEBUG
                await Application.Current!.Windows[0].Page!.DisplayAlertAsync(CodeLang.Barcode_MICRO_QR_CODE_Text, ex.Message, CodeLang.ButtonClose_Text);
#endif
                return null;
            }
        }

        /// <summary>
        /// Generates a Rectangular Micro QR code image from the specified text asynchronously.
        /// </summary>
        /// <remarks>The generated Rectangular Micro QR code uses a fixed error correction level and version. The
        /// returned ImageSource can be used directly in UI elements that support image sources. The generated PNG and SVG image
        /// is also saved to disk for sharing or further processing.</remarks>
        /// <param name="text">The text to encode in the Rectangular Micro QR code. Cannot be null or empty.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an ImageSource representing the
        /// generated Rectangular Micro QR code, or null if the code could not be generated.</returns>
        public static async Task<ImageSource?> GenerateRMQRCodeAsync(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return null;
            }

            try
            {
                // Generate the rMQR code as a PNG byte array
                // Fix the symbol height; the width is selected automatically
                byte[] pngBytes = new RmQRCodeImageBuilder(text)
                    .WithHeight(RmQRHeight.H9)
                    .WithErrorCorrection(RmQREccLevel.M)
                    .WithModulePixelSize(12)
                    .WithColors(codeColor: SkColorFromHex(ClassBarcodes.cCodeColorFg), backgroundColor: SkColorFromHex(ClassBarcodes.cCodeColorBg))
                    .ToByteArray();

                // Save the byte array 'qrCodeImage' as a PNG file
                await ClassFileUtilities.SavePngFromStreamAsync(new MemoryStream(pngBytes), ClassBarcodes.cFileBarcodePng);

                // Generate the rMQR code as an SVG string and save it to disk for sharing or other purposes
                string qrCodeAsSvg = new RmQRCodeImageBuilder(text)
                    .WithHeight(RmQRHeight.H9)
                    .WithErrorCorrection(RmQREccLevel.M)
                    .WithModulePixelSize(12)
                    .WithColors(codeColor: SkColorFromHex(ClassBarcodes.cCodeColorFg), backgroundColor: SkColorFromHex(ClassBarcodes.cCodeColorBg))
                    .ToSvgString();

                // Save the string 'qrCodeAsSvg' as a SVG file
                ClassFileUtilities.SaveStringAsFileSvg(qrCodeAsSvg, ClassBarcodes.cFileBarcodeSvg);

                // Return an ImageSource that opens a fresh stream when needed
                return ImageSource.FromStream(() => new MemoryStream(pngBytes));
            }
            catch (Exception ex)
            {
#if DEBUG                
                await Application.Current!.Windows[0].Page!.DisplayAlertAsync(CodeLang.Barcode_RMQR_CODE_Text, ex.Message, CodeLang.ButtonClose_Text);
#endif
                return null;
            }
        }

        // Convert stored hex color strings (ARGB or RRGGBB) to SKColor
        private static SkiaSharp.SKColor SkColorFromHex(string hex)
        {
            // Allow hex with or without leading '#'
            if (hex.StartsWith("#")) hex = hex[1..];
            
            // If only RRGGBB provided, assume opaque alpha = 0xFF
            if (hex.Length == 6) hex = "FF" + hex;
            
            int argb = Convert.ToInt32(hex, 16);
            
            byte a = (byte)((argb >> 24) & 0xFF);
            byte r = (byte)((argb >> 16) & 0xFF);
            byte g = (byte)((argb >> 8) & 0xFF);
            byte b = (byte)(argb & 0xFF);
            
            return new SkiaSharp.SKColor(r, g, b, a);
        }
    }
}
