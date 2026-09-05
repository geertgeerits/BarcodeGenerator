using QRCoder;
using System;
using System.Collections.Generic;
using System.Text;
using SkiaSharp.QrCode;
using SkiaSharp.QrCode.Image;

namespace BarcodeGenerator
{
    internal class ClassRMQRCode
    {
        /// <summary>
        /// Generates a Micro QR code image from the specified text asynchronously.
        /// </summary>
        /// <remarks>The generated Micro QR code uses a fixed error correction level and version. The
        /// returned ImageSource can be used directly in UI elements that support image sources. The generated PNG and SVG image
        /// is also saved to disk for sharing or further processing.</remarks>
        /// <param name="text">The text to encode in the Micro QR code. Cannot be null or empty.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an ImageSource representing the
        /// generated Micro QR code, or null if the code could not be generated.</returns>
        public static async Task<ImageSource?> GenerateRMQRCodeAsync(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return null;
            }

            try
            {
                //RmQRCodeData rmqr = RmQRCodeGenerator.CreateRmQRCode("content", RmQREccLevel.M, new RmQRCodeGeneratorOptions
                //{
                //    Height = RmQRHeight.H9,
                //    Segmentation = RmQRSegmentation.Optimal,
                //});

                byte[] pngBytes = RmQRCodeImageBuilder.GetPngBytes(text, RmQREccLevel.M, size: 512);
                //File.WriteAllBytes("rmqr.png", pngBytes);

                // Generate the QR code as an SVG string and save it to disk for sharing or other purposes
                //using QRCodeData qrDataSvg = QRCodeGenerator.GenerateMicroQrCode(text, QRCodeGenerator.ECCLevel.L, requestedVersion: nVersion);
                //using SvgQRCode qrCodeSvg = new(qrDataSvg);
                //string qrCodeAsSvg = qrCodeSvg.GetGraphic(20, System.Drawing.Color.FromArgb(Convert.ToInt32(ClassBarcodes.cCodeColorFg, 16)), System.Drawing.Color.FromArgb(Convert.ToInt32(ClassBarcodes.cCodeColorBg, 16)));

                // Save the string 'qrCodeAsSvg' as a SVG file
                //ClassFileUtilities.SaveStringAsFileSvg(qrCodeAsSvg, ClassBarcodes.cFileBarcodeSvg);

                // Generate the Micro QR code as PNG file with the specified version and error correction level
                //using QRCodeData qrDataPng = QRCodeGenerator.GenerateMicroQrCode(text, QRCodeGenerator.ECCLevel.L, requestedVersion: nVersion);
                //using PngByteQRCode qrCodePng = new(qrDataPng);
                //byte[] qrCodeImage = qrCodePng.GetGraphic(20, System.Drawing.Color.FromArgb(Convert.ToInt32(ClassBarcodes.cCodeColorFg, 16)), System.Drawing.Color.FromArgb(Convert.ToInt32(ClassBarcodes.cCodeColorBg, 16)));

                // Save the byte array 'qrCodeImage' as a PNG file
                await ClassFileUtilities.SavePngFromStreamAsync(new MemoryStream(pngBytes), ClassBarcodes.cFileBarcodePng);

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
