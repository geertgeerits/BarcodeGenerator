using System.Text.RegularExpressions;

namespace BarcodeGenerator
{
    //// Global variables and methods
    internal static class Globals
    {
        //// Global variables
        public static string cTheme = string.Empty;
        public static int nFormatGeneratorIndex;
        public static int nFormatScannerIndex;
        public static string cCodeColorFg = string.Empty;
        public static string cCodeColorBg = string.Empty;
        public static string cLanguage = string.Empty;
        public static bool bLanguageChanged;
        public static string cLanguageSpeech = string.Empty;
        public static bool bTextToSpeechAvailable;
        public static bool bTextToSpeechIsBusy;
        public static readonly string cImageTextToSpeech = "speaker_64p_blue_green.png";
        public static readonly string cImageTextToSpeechCancel = "speaker_cancel_64p_blue_red.png";
        public static bool bLicense;

        //// Global methods

        /// <summary>
        /// Set the theme
        /// </summary>
        public static void SetTheme()
        {
            Application.Current!.UserAppTheme = cTheme switch
            {
                "Light" => AppTheme.Light,
                "Dark" => AppTheme.Dark,
                _ => AppTheme.Unspecified,
            };
        }

        /// <summary>
        /// Set the current UI culture of the selected language
        /// </summary>
        public static void SetCultureSelectedLanguage(string cCultureName)
        {
            try
            {
                CultureInfo switchToCulture = new(cCultureName);
                LocalizationResourceManager.Instance.SetCulture(switchToCulture);
            }
            catch
            {
                // Do nothing
            }
        }

        /// <summary>
        /// Get ISO language (and country) code from locales
        /// </summary>
        /// <returns></returns>
        public static string GetIsoLanguageCode()
        {
            // Split before first space and remove last character '-' if there
            string cLanguageIso = cLanguageSpeech.Split(' ').First();

            if (cLanguageIso.EndsWith('-'))
            {
                cLanguageIso = cLanguageIso[..^1];
            }

            return cLanguageIso;
        }

        /// <summary>
        /// Put the generator barcode formats in a List string
        /// </summary>
        /// <returns></returns>
        public static List<string> GetFormatCodeListGenerator() => [
                "Aztec",
                "Codabar",
                "Code 39",
                "Code 93",
                "Code 128",
                "Data Matrix",
                "EAN-8",
                "EAN-13",
                "ITF (Interleaved 2 of 5)",
                "MSI (Modified Plessey)",
                "PDF417",
                "Plessey",
                "QR Code",
                "UPC-A",
                "UPC-E"
            ];

        ///// <summary>
        ///// Put the scanner barcode formats in a List string
        ///// </summary>
        ///// <returns></returns>
        //public static List<string> GetFormatCodeListScanner() => [
        //        "Aztec",
        //        "Codabar",
        //        "Code 128",
        //        "Code 39",
        //        "Code 93",
        //        "Data Matrix",
        //        "EAN-13",
        //        "EAN-8",
        //        "(IMb (Intelligent Mail))",
        //        "ITF (Interleaved 2 of 5)",
        //        "(MaxiCode)",
        //        "MSI (Modified Plessey)",
        //        "PDF417",
        //        "(Pharmacode)",
        //        "Plessey",
        //        "QR Code",
        //        "RSS 14",
        //        "RSS Expanded",
        //        "UPC-A",
        //        "UPC-E",
        //        "(UPC EAN Extension)",
        //        CodeLang.AllCodes_Text
        //    ];

        /// <summary>
        /// Put the scanner barcode formats in a List string for the Native scanner for Android
        /// https://developers.google.com/ml-kit/vision/barcode-scanning/android
        /// </summary>
        /// <returns></returns>
        public static List<string> GetFormatCodeListScannerNativeAndroid() => [
                CodeLang.AllCodes_Text,
                "Aztec",
                "Codabar",
                "Code 39",
                "Code 93",
                "Code 128",
                "Data Matrix",
                "EAN-8",
                "EAN-13",
                "ITF",
                "PDF417",
                "QR Code",
                "UPC-A",
                "UPC-E"
            ];

        /// <summary>
        /// Put the scanner barcode formats in a List string for the Native scanner for iOS
        /// https://developer.apple.com/documentation/avfoundation/avmetadataobject/objecttype
        /// </summary>
        /// <returns></returns>
        public static List<string> GetFormatCodeListScannerNativeIOS() => [
                CodeLang.AllCodes_Text,
                "Aztec",
                "Codabar",
                "Code 39",
                "Code 93",
                "Code 128",
                "Data Matrix",
                "EAN-8",
                "EAN-13",
                "GS1 DataBar",
                "ITF",
                "Micro PDF417",
                "Micro QR Code",
                "PDF417",
                "QR Code",
                "UPC-A",
                "UPC-E"
            ];

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
            var urls = new List<string>();

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
        private static async Task ConfirmAndOpenLinksAsync(IReadOnlyList<string> cUrls)
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
    }
}