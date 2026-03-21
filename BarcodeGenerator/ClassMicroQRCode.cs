/*
QRCoder supports Micro QR codes, which are smaller versions of standard QR codes suitable for applications
with limited space. Micro QR codes have significantly limited storage capacity—as few as 5 numeric digits (M1)
or as many as 35 numeric digits (M4), with alphanumeric and byte data storing considerably less.
Micro QR codes have limitations on data capacity and error correction levels.
They support versions M1 through M4 (specified as -1 to -4), and not all ECC levels are available for all versions.
M1 only supports detection (no ECC), M2 and M3 support L and M levels, and M4 supports L, M, and Q levels.
For detailed capacity tables, see the Micro QR Code specification.
https://www.qrcode.com/en/codes/microqr.html  
*/  

using QRCoder;
using System.Runtime.Versioning;

namespace BarcodeGenerator
{
    internal class ClassMicroQRCode
    {
        /// <summary>
        /// Generates a Micro QR code image from the specified text asynchronously.
        /// </summary>
        /// <remarks>The generated Micro QR code uses a fixed error correction level and version. The
        /// returned ImageSource can be used directly in UI elements that support image sources. The generated PNG image
        /// is also saved to disk for sharing or further processing.</remarks>
        /// <param name="text">The text to encode in the Micro QR code. Cannot be null or empty.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an ImageSource representing the
        /// generated Micro QR code, or null if the code could not be generated.</returns>
        [SupportedOSPlatform("ios13.0")]
        [SupportedOSPlatform("maccatalyst13.0")]
        [SupportedOSPlatform("windows10.0.17763.0")]
        public static async Task<ImageSource?> GenerateMicroQrCode(string text, int nVersion = -4)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return null;
            }

            using QRCodeGenerator generator = new();
            using QRCodeData qrCodeData = QRCodeGenerator.GenerateMicroQrCode(text, QRCodeGenerator.ECCLevel.L, requestedVersion: nVersion);
            using PngByteQRCode qrCode = new(qrCodeData);
            byte[] qrCodeImage = qrCode.GetGraphic(20);

            // Save a copy to disk (await the async save)
            await ClassFileOperations.SavePngFromStreamAsync(new MemoryStream(qrCodeImage), Globals.cFileBarcode);

            // Return an ImageSource that opens a fresh stream when needed
            return ImageSource.FromStream(() => new MemoryStream(qrCodeImage));
        }
    }
}
