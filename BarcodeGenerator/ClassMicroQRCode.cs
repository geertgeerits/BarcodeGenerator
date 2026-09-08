using SkiaSharp.QrCode;
using SkiaSharp.QrCode.Image;

namespace BarcodeGenerator
{
    internal class ClassMicroQRCode
    {
        /// <summary>
        /// Generates a Micro QR code image from the specified text asynchronously.
        /// </summary>
        /// <remarks>The generated Micro QR code uses a fixed error correction level and version. The
        /// returned ImageSource can be used directly in UI elements that support image sources. The generated PNG and SVG image
        /// is also saved to disk for sharing or further processing.</remarks>
        /// <param name="text">The text to encode in the Micro QR code. Cannot be null or empty.</param>
        /// <param name="nVersion">The requested Micro QR code version (-1 to -4). Default is -4 (M4), which supports the largest data capacity. If the specified version cannot accommodate the text, a smaller version may be generated if possible.</param>
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
                byte[] pngBytes = MicroQRCodeImageBuilder.GetPngBytes(text, MicroQREccLevel.M, size: ClassBarcodes.nQRCodeSizePixels);

                // Save the byte array 'qrCodeImage' as a PNG file
                await ClassFileUtilities.SavePngFromStreamAsync(new MemoryStream(pngBytes), ClassBarcodes.cFileBarcodePng);

                // Generate the Micro QR code as an SVG string and save it to disk for sharing or other purposes
                string qrCodeAsSvg = MicroQRCodeImageBuilder.GetSvgString(text, MicroQREccLevel.M, size: ClassBarcodes.nQRCodeSizePixels);

                // Save the string 'qrCodeAsSvg' as a SVG file
                ClassFileUtilities.SaveStringAsFileSvg(qrCodeAsSvg, ClassBarcodes.cFileBarcodeSvg);

                // Return an ImageSource that opens a fresh stream when needed
                return ImageSource.FromStream(() => new MemoryStream(pngBytes));
            }
            catch (Exception ex)
            {
                await Application.Current!.Windows[0].Page!.DisplayAlertAsync(CodeLang.Barcode_MICRO_QR_CODE_Text, ex.Message, CodeLang.ButtonClose_Text);
                return null;
            }
        }
    }
}
