// https://github.com/guitarrapc/FeatherQR

using SkiaSharp;
using SkiaSharp.QrCode;
using SkiaSharp.QrCode.Image;
//using FeatherQR;
//using FeatherQR.SkiaSharp;

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
        /// <remarks>!!! Do not use '.WithSize(ClassBarcodes.nQRCodeSizePixels, ClassBarcodes.nQRCodeSizePixels)' for Rectangular Micro QR codes.!!!</remarks>

        public static async Task<ImageSource?> GenerateQrCodeAsync(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return null;
            }

            try
            {
                // Generate the QR code
                QRCodeImageBuilder QrData = new QRCodeImageBuilder(text)
                    .WithErrorCorrection(ECCLevel.M)
                    .WithColors(codeColor: SkColorFromHex(ClassBarcodes.cCodeColorFg), backgroundColor: SkColorFromHex(ClassBarcodes.cCodeColorBg), clearColor: SKColors.Transparent)
                    .WithQuietZone(ClassBarcodes.nQRCodeQuietZoneSize);

                // Only apply size if the user has not selected a variable size (to avoid overriding the variable size setting)
                if (!ClassBarcodes.bQRCodeSizeVariable)
                {
                    QrData = QrData.WithSize(ClassBarcodes.nQRCodeSizePixels, ClassBarcodes.nQRCodeSizePixels);
                }

                // Generate the QR code as a PNG byte array and SVG string
                byte[] pngBytes = QrData.ToByteArray();
                string qrCodeAsSvg = QrData.ToSvgString();

                // Save the byte array 'qrCodeImage' as a PNG file and SVG file
                await ClassFileUtilities.SavePngFromStreamAsync(new MemoryStream(pngBytes), ClassBarcodes.cFileBarcodePng);
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
                // Generate the Micro QR code
                MicroQRCodeImageBuilder QrData = new MicroQRCodeImageBuilder(text)
                    .WithErrorCorrection(MicroQREccLevel.M)
                    .WithColors(codeColor: SkColorFromHex(ClassBarcodes.cCodeColorFg), backgroundColor: SkColorFromHex(ClassBarcodes.cCodeColorBg), clearColor: SKColors.Transparent)
                    .WithQuietZone(ClassBarcodes.nQRCodeQuietZoneSize2);

                // Only apply size if the user has not selected a variable size (to avoid overriding the variable size setting)
                if (!ClassBarcodes.bQRCodeSizeVariable)
                {
                    QrData = QrData.WithSize(ClassBarcodes.nQRCodeSizePixels, ClassBarcodes.nQRCodeSizePixels);
                }

                // Generate the QR code as a PNG byte array and SVG string
                byte[] pngBytes = QrData.ToByteArray();
                string qrCodeAsSvg = QrData.ToSvgString();

                // Save the byte array 'qrCodeImage' as a PNG file and SVG file
                await ClassFileUtilities.SavePngFromStreamAsync(new MemoryStream(pngBytes), ClassBarcodes.cFileBarcodePng);
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
                RmQRCodeImageBuilder QrData = new RmQRCodeImageBuilder(text)
                    .WithErrorCorrection(RmQREccLevel.M)
                    .WithColors(codeColor: SkColorFromHex(ClassBarcodes.cCodeColorFg), backgroundColor: SkColorFromHex(ClassBarcodes.cCodeColorBg), clearColor: SKColors.Transparent)
                    .WithQuietZone(ClassBarcodes.nQRCodeQuietZoneSize2);

                // Only apply size if the user has not selected a variable size (to avoid overriding the variable size setting)
                if (!ClassBarcodes.bQRCodeSizeVariable)
                {
                    QrData = ClassBarcodes.nQRCodeSizeModulesHeight switch
                    {
                        7 => QrData.WithHeight(RmQRHeight.H7),
                        11 => QrData.WithHeight(RmQRHeight.H11),
                        13 => QrData.WithHeight(RmQRHeight.H13),
                        15 => QrData.WithHeight(RmQRHeight.H15),
                        17 => QrData.WithHeight(RmQRHeight.H17),
                        _ => QrData.WithHeight(RmQRHeight.H9),
                    };
                }

                // Generate the QR code as a PNG byte array and SVG string
                byte[] pngBytes = QrData.ToByteArray();
                string qrCodeAsSvg = QrData.ToSvgString();

                // Save the byte array 'qrCodeImage' as a PNG file and SVG file
                await ClassFileUtilities.SavePngFromStreamAsync(new MemoryStream(pngBytes), ClassBarcodes.cFileBarcodePng);
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
        public static SkiaSharp.SKColor SkColorFromHex(string hex)
        {
            // Allow hex with or without leading '#'
            if (hex.StartsWith('#')) hex = hex[1..];
            
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
