using SkiaSharp.QrCode;
using SkiaSharp.QrCode.Image;

namespace BarcodeGenerator
{
    internal class ClassRMQRCode
    {
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
                byte[] pngBytes = RmQRCodeImageBuilder.GetPngBytes(text, RmQREccLevel.M, size: ClassBarcodes.nQRCodeSizePixels);

                // Save the byte array 'qrCodeImage' as a PNG file
                await ClassFileUtilities.SavePngFromStreamAsync(new MemoryStream(pngBytes), ClassBarcodes.cFileBarcodePng);

                // Generate the rMQR code as an SVG string and save it to disk for sharing or other purposes
                string qrCodeAsSvg = RmQRCodeImageBuilder.GetSvgString(text, RmQREccLevel.M, size: ClassBarcodes.nQRCodeSizePixels);

                // Save the string 'qrCodeAsSvg' as a SVG file
                ClassFileUtilities.SaveStringAsFileSvg(qrCodeAsSvg, ClassBarcodes.cFileBarcodeSvg);

                // Return an ImageSource that opens a fresh stream when needed
                return ImageSource.FromStream(() => new MemoryStream(pngBytes));
            }
            catch (Exception ex)
            {
                await Application.Current!.Windows[0].Page!.DisplayAlertAsync(CodeLang.Barcode_RMQR_CODE_Text, ex.Message, CodeLang.ButtonClose_Text);
                return null;
            }
        }
    }
}
