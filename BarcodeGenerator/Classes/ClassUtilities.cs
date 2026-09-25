using System.Text.RegularExpressions;
using ZXing.Net.Maui.Controls;

namespace BarcodeGenerator
{
    internal class ClassUtilities
    {
        /// <summary>
        /// Set the image source to the saved file to display it in the Image control
        /// </summary>
        /// <param name="bgvBarcode"></param>
        /// <param name="image"></param>
        /// <param name="fileBarcodePng"></param>
        /// <returns></returns>
        public static async Task SetImageSourceAsync(BarcodeGeneratorView bgvBarcode, Image image, string fileBarcodePng)
        {
            // Check if the barcode with caption or circle is enabled, otherwise return
            if (!ClassBarcodes.bBarcodeWithCaption && !ClassBarcodes.bBarcodeWithCircle)
            {
                return;
            }

            // Wait for a short delay to ensure the file is fully written before setting the image source
            await Task.Delay(400);

            // Set the image source to the saved file to display it in the Image control
#if ANDROID
            // !!!BUG!!! in Android: returns always the first generated barcode, even when a new barcode is
            // generated and saved to the same file name. This does not happen on Windows and iOS.

            // Create a unique file name for the copied barcode PNG file to avoid caching issues on Android
            string cFileBarcodePngUnique = Path.Combine(FileSystem.Current.CacheDirectory, $@"{DateTime.Now.Ticks}.png");
            File.Copy(fileBarcodePng, cFileBarcodePngUnique);
#endif
            bgvBarcode.Value = string.Empty;    // Clear the BarcodeView value to avoid displaying the barcode twice
            image.IsVisible = true;
#if ANDROID
            // Set the Image control source to the saved file
            image.Source = ImageSource.FromFile(cFileBarcodePngUnique);

            // Delete the unique file with caption after a short delay to ensure it is not cached and displayed again on Android
            await Task.Delay(400);
            ClassFileUtilities.DeleteFileInCache(cFileBarcodePngUnique);
#else
            // Set the Image control source to the saved file
            image.Source = ImageSource.FromFile(fileBarcodePng);
#endif
        }

        /// <summary>
        /// Button share event: share the barcode result
        /// </summary>
        /// <param name="cText"></param>
        /// <returns></returns>
        public static async Task ShareBarcodeResultAsync(string cText)
        {
            if (string.IsNullOrEmpty(cText))
            {
                return;
            }

            // For testing
            //cText = "http://www.google.com";
            //cText = "url http://www.google.com, visit website url https://www.microsoft.com, www.yahou.com and WWW.MODEGEERITS.BE and geertgeerits@gmail.com address";
            //cText = "Share text from barcode scanner";

            try
            {
                // Extract URLs from the text and confirm/open them
                List<string> cUrls = ParseUrlsFromText(cText);

                if (cUrls.Count > 0)
                {
                    await ConfirmAndOpenLinksAsync(cUrls);
                }
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
#if DEBUG
                await Application.Current!.Windows[0].Page!.DisplayAlertAsync("ShareBarcodeResultAsync", ex.Message, CodeLang.ButtonClose_Text);
#endif
            }

            // Wait 700 milliseconds otherwise the ShareText() is not executed after the last opened link
            await Task.Delay(700);

            // Open share interface
            await ShareTextAsync(cText);
        }

        /// <summary>
        /// Parse URLs from a block of text using the same regex as before
        /// </summary>
        /// <param name="cText"></param>
        /// <returns>List of matched URL strings</returns>
        private static List<string> ParseUrlsFromText(string cText)
        {
            List<string> urls = [];

            if (string.IsNullOrEmpty(cText))
            {
                return urls;
            }

            // Set the pattern for the URL matching
            //string cPattern = @"(http|ftp|https):\/\/([\w\-_]+(?:(?:\.[\w\-_]+)+))([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?";
            string cPattern = @"((https?|ftp|file)\://|www.)[A-Za-z0-9\.\-]+(/[A-Za-z0-9\?\&\=;\+!'\(\)\*\-\._~%]*)*";

            foreach (Match match in Regex.Matches(cText, cPattern, RegexOptions.IgnoreCase).Cast<Match>())
            {
                if (match.Success)
                {
                    urls.Add(match.Value);
                }
            }

            return urls;
        }

        /// <summary>
        /// Confirm each URL with the user and open it when confirmed
        /// </summary>
        /// <param name="cUrls"></param>
        private static async Task ConfirmAndOpenLinksAsync(List<string> cUrls)
        {
            if (cUrls is null || cUrls.Count == 0)
            {
                return;
            }

            foreach (string cUrl in cUrls)
            {
                bool bAnswer = await Application.Current!.Windows[0].Page!.DisplayAlertAsync(
                    CodeLang.OpenLinkTitle_Text,
                    $"{cUrl}\n\n{CodeLang.OpenLinkText_Text}",
                    CodeLang.Yes_Text,
                    CodeLang.No_Text);

                if (bAnswer)
                {
                    await OpenWebsiteLinkAsync(cUrl);
                }
            }
        }

        /// <summary>
        /// Open the website link
        /// </summary>
        /// <param name="cUrl"></param>
        /// <returns></returns>
        public static async Task OpenWebsiteLinkAsync(string cUrl)
        {
            if (cUrl[..4] is "www." or "WWW.")
            {
                cUrl = $"http://{cUrl}";
            }

            try
            {
                Uri uri = new(cUrl);
#if IOS
                // !!!BUG!!! in iOS. The camera is disabled after opening the website link
                // using the BrowserLaunchMode.SystemPreferred or the new PageWebsite(cUrl)
                await Browser.Default.OpenAsync(uri, BrowserLaunchMode.External);
#else
                await Browser.Default.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
#endif
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
#if DEBUG
                await Application.Current!.Windows[0].Page!.DisplayAlertAsync("OpenWebsiteLinkAsync", ex.Message, CodeLang.ButtonClose_Text);
#endif
            }
        }

        /// <summary>
        /// Open the share interface
        /// </summary>
        /// <param name="cText"></param>
        /// <returns></returns>
        public static async Task ShareTextAsync(string cText)
        {
            try
            {
                await Share.Default.RequestAsync(new ShareTextRequest
                {
                    Text = cText,
                    Title = "Barcode Scanner"
                });
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
#if DEBUG
                await Application.Current!.Windows[0].Page!.DisplayAlertAsync("ShareTextAsync", ex.Message, CodeLang.ButtonClose_Text);
#endif
            }
        }

        /// <summary>
        /// Paste text from the clipboard into the editor at the current cursor position, replacing any selected text
        /// </summary>
        /// <param name="editor"></param>
        /// <returns></returns>
        public static async Task PasteFromClipboardAsync(Editor editor)
        {
            if (!Clipboard.Default.HasText || editor == null)
            {
                return;
            }

            try
            {
                string cTextToPaste = await Clipboard.Default.GetTextAsync() ?? string.Empty;
                int cursor = editor.CursorPosition;
                int length = editor.SelectionLength;
                string original = editor.Text ?? string.Empty;

                if (length > 0)
                {
                    original = original.Remove(cursor, length);
                }

                editor.Text = original.Insert(cursor, cTextToPaste);
                editor.CursorPosition = cursor + cTextToPaste.Length;
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
#if DEBUG
                await Application.Current!.Windows[0].Page!.DisplayAlertAsync("OnPasteFromClipboard_Clicked", ex.Message, CodeLang.ButtonClose_Text);
#endif
            }
        }
    }
}
