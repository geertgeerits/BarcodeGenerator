// https://github.com/Shane32/QRCoder/wiki/Advanced-usage---QR-Code-renderers#211-artqrcode-renderer-in-detail

using QRCoder;
using System.Drawing;
using static QRCoder.ArtQRCode;

namespace BarcodeGenerator
{
    internal class ClassArtQRCode
    {
        public static async Task<ImageSource?> GenerateArtQrCodeAsync(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return null;
            }

            try
            {
                // Open the file picker to select an image file for the background of the Art QR code
                FileResult? cFile = await ClassFileOperations.PickImage();

                // Create background bitmap if file picked
                Bitmap? bgBitmap = null;
                if (cFile != null)
                {
                    using var s = await cFile.OpenReadAsync();
                    bgBitmap = new Bitmap(s);
                }

                // Generate the Art QR code as PNG file with the specified version and error correction level
                QRCodeGenerator qrGenerator = new();
                using QRCodeData qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
                using ArtQRCode qrCode = new(qrCodeData);

                Bitmap qrCodeImage = qrCode.GetGraphic(10,
                                        darkColor: System.Drawing.Color.FromArgb(ParseHexColor(Globals.cCodeColorFg)),
                                        lightColor: System.Drawing.Color.FromArgb(ParseHexColor(Globals.cCodeColorBg)),
                                        backgroundColor: System.Drawing.Color.FromArgb(ParseHexColor(Globals.cCodeColorBg)),
                                        backgroundImage: bgBitmap,
                                        backgroundImageStyle: BackgroundImageStyle.DataAreaOnly);

                // dispose bgBitmap when no longer needed
                bgBitmap?.Dispose();

                // Convert bitmap to byte array
                byte[] imageBytes;
                using (MemoryStream ms = new())
                {
                    qrCodeImage.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    imageBytes = ms.ToArray();
                }

                // Save the byte array as a PNG file
                await ClassFileOperations.SavePngFromStreamAsync(new MemoryStream(imageBytes), Globals.cFileBarcodePng);

                // Return an ImageSource that opens a fresh stream when needed
                return ImageSource.FromStream(() => new MemoryStream(imageBytes));
            }

            catch (Exception ex)
            {
                await Application.Current!.Windows[0].Page!.DisplayAlertAsync(CodeLang.Barcode_ART_QR_CODE_Text, ex.Message, CodeLang.ButtonClose_Text);
                return null;
            }
        }

        /// <summary>
        /// Parses a hex color string (e.g., "#RRGGBB" or "#AARRGGBB") and returns the corresponding ARGB integer value.
        /// </summary>
        /// <param name="hex">The hex color string.</param>
        /// <returns>The ARGB integer value.</returns>
        private static int ParseHexColor(string hex)
        {
            hex = hex.Replace("#", "");
            int val = int.Parse(hex, NumberStyles.HexNumber);
            return hex.Length == 6 ? (unchecked((int)0xFF000000) | val) : val;
        }
    }
}
