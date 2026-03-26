/*
https://github.com/Shane32/QRCoder/wiki/Advanced-usage---QR-Code-renderers#211-artqrcode-renderer-in-detail
Only supported in .NET 6.0+ due to System.Drawing.Common dependency, which is not supported in .NET 5.0
and earlier on non-Windows platforms.
.NET 6.0+ supports System.Drawing.Common on all platforms, but it is still recommended to use it only on Windows
for production applications due to potential performance and reliability issues on other platforms.
The QRCode and ArtQRCode renderers depend on System.Drawing.Common, which Microsoft has removed cross-platform support
for in .NET 6+. You may encounter one of the following build or runtime errors:
- CA1416: This call site is reachable on all platforms. 'QRCode.QRCode(QRCodeData)' is only supported on: 'windows'
- System.TypeInitializationException: The type initializer for 'Gdip' threw an exception.
- System.PlatformNotSupportedException: System.Drawing.Common is not supported on this platform.
*/

using System.Drawing;
using System.Runtime.Versioning;
using CommunityToolkit.Maui.Extensions;
using QRCoder;
using static QRCoder.ArtQRCode;

namespace BarcodeGenerator
{
    internal class ClassArtQRCode
    {
        [SupportedOSPlatform("windows")]
        public static async Task<ImageSource?> GenerateArtQrCodeAsync(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return null;
            }

            try
            {
                // Calculate the recommended image size based on the Art QR code size
                // Generate the Art QR code data with the appropriate error correction level without border and quiet zones to get the actual size of the Art QR code without any padding
                QRCodeGenerator qrGenerator = new();
                using QRCodeData qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
                using ArtQRCode qrCode = new(qrCodeData);

                using Bitmap qrCodeImageTemp = qrCode.GetGraphic(pixelsPerModule:10,
                                                   darkColor: System.Drawing.Color.FromArgb(ParseHexColor(Globals.cCodeColorFg)),
                                                   lightColor: System.Drawing.Color.FromArgb(ParseHexColor(Globals.cCodeColorBg)),
                                                   backgroundColor: System.Drawing.Color.FromArgb(ParseHexColor(Globals.cCodeColorBg)),
                                                   drawQuietZones: false);

                // Retrieve the width and height of the generated Art QR code
                int qrCodeWidth = qrCodeImageTemp.Width;
                int qrCodeHeight = qrCodeImageTemp.Height;

                // Show a modal popup to inform the user about the recommended image size before opening the file picker
                Page? currentPage = Application.Current?.Windows.Count > 0 ? Application.Current.Windows[0]?.Page : null;
                if (currentPage != null)
                {
                    Globals.bIsPopupMessage = true;
                    await currentPage.ShowPopupAsync(new PopupMessage(5, $"{CodeLang.QRCodeRecommendedImageSize_Text}:\n\n{qrCodeWidth} x {qrCodeHeight} {CodeLang.Pixels_Text}"));

                    // Check if the popup was canceled by the user before proceeding to open the file picker
                    if (Globals.bPopupCanceled)
                    {
                        Globals.bPopupCanceled = false;
                        return null;
                    }
                }

                // Open the file picker to select an image file for the background of the Art QR code
                FileResult? cFile = await ClassFileOperations.PickImage();

                // Create background bitmap if file picked
                Bitmap? bgBitmap = null;
                if (cFile != null)
                {
                    using Stream s2 = await cFile.OpenReadAsync();
                    bgBitmap = new Bitmap(s2);
                }

                // Generate the Art QR code with the specified version and error correction level and the selected background image (if any)
                using Bitmap qrCodeImage = qrCode.GetGraphic(pixelsPerModule:10,
                                                darkColor: System.Drawing.Color.FromArgb(ParseHexColor(Globals.cCodeColorFg)),
                                                lightColor: System.Drawing.Color.FromArgb(ParseHexColor(Globals.cCodeColorBg)),
                                                backgroundColor: System.Drawing.Color.FromArgb(ParseHexColor(Globals.cCodeColorBg)),
                                                backgroundImage: bgBitmap,
                                                backgroundImageStyle: BackgroundImageStyle.DataAreaOnly,
                                                drawQuietZones: true);

                // Dispose bgBitmap when no longer needed
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
