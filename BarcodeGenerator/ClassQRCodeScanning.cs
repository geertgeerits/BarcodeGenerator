// https://www.nuget.org/packages/SkiaSharp.QrCode/1.2.0#decoders

using SkiaSharp;
using SkiaSharp.QrCode;

namespace BarcodeGenerator
{
    internal class ClassQRCodeScanning
    {
        public static string QRCodeDecoderText(string text)
        {
            QRCodeData qrData = QRCodeGenerator.CreateQrCode(text, ECCLevel.M);
            
            if (QRCodeDecoder.TryDecode(qrData, out var decodedText))
            {
                Debug.WriteLine(decodedText);
                return decodedText;
            }

            return string.Empty;
        }

        /// <summary>
        /// Decodes a QR code from an image file and returns the decoded text.
        /// </summary>
        /// <param name="cImageFile"></param>
        /// <returns></returns>
        /// <remarks>Works for standard QR code images but not for Micro QR codes and rectangular QR codes.</remarks>
        public static string QRCodeDecoderImage(string cImageFile)
        {
            using SKBitmap bitmap = SKBitmap.Decode(cImageFile);
            
            if (QRCodeDecoder.TryDecode(bitmap, out var decodedText, out var info))
            {
                Debug.WriteLine($"Result decoder image: {decodedText} (version {info.Version}, ECC {info.EccLevel})");
                return decodedText;
            }

            return string.Empty;
        }
    }
}
