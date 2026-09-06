// https://www.nuget.org/packages/SkiaSharp.QrCode/1.2.0#decoders

using SkiaSharp;
using SkiaSharp.QrCode;

namespace BarcodeGenerator
{
    internal class ClassQRCodeScanning
    {
        public static string QRCodeDecoderMain(string cImageFile)
        {
            if (string.IsNullOrEmpty(cImageFile))
            {
                return string.Empty;
            }

            string cText;

            // Attempt to decode the QR code from the image file
            cText = QRCodeDecoderImage(cImageFile);
            if (!string.IsNullOrEmpty(cText))
            {
                return $"{CodeLang.Barcode_QR_CODE_Text}:\n{cText}";
            }

            // If the QR code decoding fails, attempt to decode a Micro QR code
            cText = MicroQRCodeDecoderImage(cImageFile);
            if (!string.IsNullOrEmpty(cText))
            {
                return $"{CodeLang.Barcode_MICRO_QR_CODE_Text}:\n{cText}";
            }

            // If both QR code and Micro QR code decoding fail, attempt to decode a Rectangular QR code
            cText = RectangularQRCodeDecoderImage(cImageFile);
            if (!string.IsNullOrEmpty(cText))
            {
                return $"{CodeLang.Barcode_RMQR_CODE_Text}:\n{cText}";
            }

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
