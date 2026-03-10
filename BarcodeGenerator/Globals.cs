using System.Text.RegularExpressions;

namespace BarcodeGenerator
{
    // Global variables and methods
    internal static class Globals
    {
        // Global variables
        public static string cTheme = string.Empty;                         // Theme: Light, Dark or System
        public static string cCodeColorFg = string.Empty;                   // Foreground color of the barcode (text and bars)
        public static string cCodeColorBg = string.Empty;                   // Background color of the barcode
        public static string cLanguage = string.Empty;                      // Language for the UI (culture name, e.g. "en-US")
        public static bool bLanguageChanged;                                // Flag to indicate if the language has been changed
        public static string cLanguageSpeech = string.Empty;                // Language for text-to-speech
        public static bool bTextToSpeechAvailable;                          // Flag to indicate if text-to-speech is available
        public static bool bTextToSpeechIsBusy;                             // Flag to indicate if text-to-speech is currently busy
        public static readonly string cImageTextToSpeech = "speaker_64p_blue_green.png";  // Image for text-to-speech button
        public static readonly string cImageTextToSpeechCancel = "speaker_cancel_64p_blue_red.png";  // Image for text-to-speech cancel button
        public static bool bIsPopupMessage;                                 // Popup message has been displayed
        public static bool bPopupCanceled;                                  // Popup message was canceled by the user
        public static bool bLicense;                                        // Flag to indicate if the user has accepted the license agreement
        public static string cFileBarcode = string.Empty;                   // Path and file name of the saved barcode image

        // Global methods

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
    }
}