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
                // Generate the QR code as a PNG byte array
                // Fix the symbol height; the width is selected automatically
                byte[] pngBytes = new QRCodeImageBuilder(text)
                    .WithModulePixelSize(12)
                    .WithErrorCorrection(ECCLevel.M)
                    .WithColors(codeColor: SkColorFromHex(ClassBarcodes.cCodeColorFg), backgroundColor: SkColorFromHex(ClassBarcodes.cCodeColorBg))
                    .WithQuietZone(ClassBarcodes.nQRCodeQuietZoneSize)
                    .ToByteArray();

                // Save the byte array 'qrCodeImage' as a PNG file
                await ClassFileUtilities.SavePngFromStreamAsync(new MemoryStream(pngBytes), ClassBarcodes.cFileBarcodePng);

                // Generate the QR code as an SVG string and save it to disk for sharing or other purposes
                string qrCodeAsSvg = new QRCodeImageBuilder(text)
                    .WithModulePixelSize(12)
                    .WithErrorCorrection(ECCLevel.M)
                    .WithColors(codeColor: SkColorFromHex(ClassBarcodes.cCodeColorFg), backgroundColor: SkColorFromHex(ClassBarcodes.cCodeColorBg))
                    .WithQuietZone(ClassBarcodes.nQRCodeQuietZoneSize)
                    .ToSvgString();

                // Save the string 'qrCodeAsSvg' as a SVG file
                ClassFileUtilities.SaveStringAsFileSvg(qrCodeAsSvg, ClassBarcodes.cFileBarcodeSvg);

                // Return an ImageSource that opens a fresh stream when needed
                return ImageSource.FromStream(() => new MemoryStream(pngBytes));
            }
            catch (Exception ex)
            {
#if DEBUG
                await Application.Current!.Windows[0].Page!.DisplayAlertAsync(CodeLang.Barcode_QR_CODE_Text, ex.Message, CodeLang.ButtonClose_Text);
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
        public static async Task<ImageSource?> GenerateMicroQrCodeAsync(string text)
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
                    .WithModulePixelSize(12)
                    .WithErrorCorrection(MicroQREccLevel.M)
                    .WithColors(codeColor: SkColorFromHex(ClassBarcodes.cCodeColorFg), backgroundColor: SkColorFromHex(ClassBarcodes.cCodeColorBg))
                    .WithQuietZone(ClassBarcodes.nQRCodeQuietZoneSize)
                    .ToByteArray();

                // Save the byte array 'qrCodeImage' as a PNG file
                await ClassFileUtilities.SavePngFromStreamAsync(new MemoryStream(pngBytes), ClassBarcodes.cFileBarcodePng);

                // Generate the Micro QR code as an SVG string and save it to disk for sharing or other purposes
                string qrCodeAsSvg = new MicroQRCodeImageBuilder(text)
                    .WithModulePixelSize(12)
                    .WithErrorCorrection(MicroQREccLevel.M)
                    .WithColors(codeColor: SkColorFromHex(ClassBarcodes.cCodeColorFg), backgroundColor: SkColorFromHex(ClassBarcodes.cCodeColorBg))
                    .WithQuietZone(ClassBarcodes.nQRCodeQuietZoneSize)
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
                    .WithModulePixelSize(12)
                    .WithHeight(RmQRHeight.H9)
                    .WithErrorCorrection(RmQREccLevel.M)
                    .WithColors(codeColor: SkColorFromHex(ClassBarcodes.cCodeColorFg), backgroundColor: SkColorFromHex(ClassBarcodes.cCodeColorBg))
                    .WithQuietZone(ClassBarcodes.nQRCodeQuietZoneSize)
                    .ToByteArray();

                // Save the byte array 'qrCodeImage' as a PNG file
                await ClassFileUtilities.SavePngFromStreamAsync(new MemoryStream(pngBytes), ClassBarcodes.cFileBarcodePng);

                // Generate the rMQR code as an SVG string and save it to disk for sharing or other purposes
                string qrCodeAsSvg = new RmQRCodeImageBuilder(text)
                    .WithModulePixelSize(12)
                    .WithHeight(RmQRHeight.H9)
                    .WithErrorCorrection(RmQREccLevel.M)
                    .WithColors(codeColor: SkColorFromHex(ClassBarcodes.cCodeColorFg), backgroundColor: SkColorFromHex(ClassBarcodes.cCodeColorBg))
                    .WithQuietZone(ClassBarcodes.nQRCodeQuietZoneSize)
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
