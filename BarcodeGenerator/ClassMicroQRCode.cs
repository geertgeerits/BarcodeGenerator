/*
QRCoder supports Micro QR codes, which are smaller versions of standard QR codes suitable for applications
with limited space. Micro QR codes have significantly limited storage capacity—as few as 5 numeric digits (M1)
or as many as 35 numeric digits (M4), with alphanumeric and byte data storing considerably less.
Micro QR codes have limitations on data capacity and error correction levels.
They support versions M1 through M4 (specified as -1 to -4), and not all ECC levels are available for all versions.
M1 only supports detection (no ECC), M2 and M3 support L and M levels, and M4 supports L, M, and Q levels.
For detailed capacity tables, see the Micro QR Code specification.
https://www.nuget.org/packages/QRCoder/1.7.0#show-readme-container
https://www.qrcode.com/en/codes/microqr.html  
*/

using QRCoder;

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
                // Generate the QR code as an SVG string and save it to disk for sharing or other purposes
                using QRCodeData qrDataSvg = QRCodeGenerator.GenerateMicroQrCode(text, QRCodeGenerator.ECCLevel.L, requestedVersion: nVersion);
                using SvgQRCode qrCodeSvg = new(qrDataSvg);
                string qrCodeAsSvg = qrCodeSvg.GetGraphic(20, System.Drawing.Color.FromArgb(Convert.ToInt32(Globals.cCodeColorFg, 16)), System.Drawing.Color.FromArgb(Convert.ToInt32(Globals.cCodeColorBg, 16)));

                // Save the string 'qrCodeAsSvg' as a SVG file
                ClassFileOperations.SaveStringAsFileSvg(qrCodeAsSvg, Globals.cFileBarcodeSvg);

                // Generate the Micro QR code as PNG file with the specified version and error correction level
                using QRCodeData qrDataPng = QRCodeGenerator.GenerateMicroQrCode(text, QRCodeGenerator.ECCLevel.L, requestedVersion: nVersion);
                using PngByteQRCode qrCodePng = new(qrDataPng);
                byte[] qrCodeImage = qrCodePng.GetGraphic(20, System.Drawing.Color.FromArgb(Convert.ToInt32(Globals.cCodeColorFg, 16)), System.Drawing.Color.FromArgb(Convert.ToInt32(Globals.cCodeColorBg, 16)));

                // Save the byte array 'qrCodeImage' as a PNG file
                await ClassFileOperations.SavePngFromStreamAsync(new MemoryStream(qrCodeImage), Globals.cFileBarcodePng);

                // Return an ImageSource that opens a fresh stream when needed
                return ImageSource.FromStream(() => new MemoryStream(qrCodeImage));
            }
            catch (Exception ex)
            {
                await Application.Current!.Windows[0].Page!.DisplayAlertAsync(CodeLang.Barcode_MICRO_QR_CODE_Text, ex.Message, CodeLang.ButtonClose_Text);
                return null;
            }
        }
    }
}
