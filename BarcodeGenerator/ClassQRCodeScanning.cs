// https://www.nuget.org/packages/SkiaSharp.QrCode/1.2.0#decoders

using SkiaSharp;
using SkiaSharp.QrCode;

namespace BarcodeGenerator
{
    internal class ClassQRCodeScanning
    {
        public static string QRCodeDecoderText(string text)
        {
            //QRCodeData qrData = QRCodeGenerator.CreateQrCode(text, ECCLevel.M);
            
            //if (QRCodeDecoder.TryDecode(qrData, out var decodedText))
            //{
            //    Debug.WriteLine(decodedText);
            //    return decodedText;
            //}

            return string.Empty;
        }

        /// <summary>
        /// Decodes a QR code from an image file and returns the decoded text.
        /// </summary>
        /// <param name="cImageFile"></param>
        /// <returns></returns>
        public static string QRCodeDecoderImage(string cImageFile)
        {
            using SKBitmap bitmap = SKBitmap.Decode(cImageFile);
            
            if (QRCodeDecoder.TryDecode(bitmap, out var decodedText, out var info))
            {
                Debug.WriteLine($"Result QR Code image: {decodedText} (version {info.Version}, ECC {info.EccLevel})");
                return decodedText;
            }

            return string.Empty;
        }

        /// <summary>
        /// Decodes a Micro QR code from an image file and returns the decoded text.
        /// </summary>
        /// <param name="cImageFile"></param>
        /// <returns></returns>
        public static string MicroQRCodeDecoderImage(string cImageFile)
        {
            using SKBitmap bitmap = SKBitmap.Decode(cImageFile);

            if (MicroQRCodeDecoder.TryDecode(bitmap, out var decodedText, out var info))
            {
                Debug.WriteLine($"Result Micro QR Code image: {decodedText} (version {info.Version}, ECC {info.EccLevel})");
                return decodedText;
            }

            return string.Empty;
        }

        /// <summary>
        /// Decodes a Rectangular QR code from an image file and returns the decoded text.
        /// </summary>
        /// <param name="cImageFile"></param>
        /// <returns></returns>
        public static string RectangularQRCodeDecoderImage(string cImageFile)
        {
            using SKBitmap bitmap = SKBitmap.Decode(cImageFile);

            if (RmQRCodeDecoder.TryDecode(bitmap, out var decodedText, out var info))
            {
                Debug.WriteLine($"Result rMQR Code image: {decodedText} (version {info.Version}, ECC {info.EccLevel})");
                return decodedText;
            }

            return string.Empty;
        }
    }
}
