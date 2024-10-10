//// Global usings
global using BarcodeGenerator.Resources.Languages;
global using System.Globalization;
global using System.Diagnostics;
global using Sentry;

//// Local usings
using System.Text.RegularExpressions;

namespace BarcodeGenerator
{
    //// Global variables and methods
    internal static class Globals
    {
        //// Global variables
        public static string cTheme = "";
        public static int nFormatGeneratorIndex;
        public static int nFormatScannerIndex;
        public static string cCodeColorFg = "";
        public static string cCodeColorBg = "";
        public static string cLanguage= "";
        public static bool bLanguageChanged;
        public static string cLanguageSpeech = "";
        public static string[]? cLanguageLocales;
        public static bool bLanguageLocalesExist;
        public static bool bTextToSpeechIsBusy;
        public static IEnumerable<Locale>? locales;
        public static CancellationTokenSource? cts;
        public static string cImageTextToSpeech = "speaker_64p_blue_green.png";
        public static string cImageTextToSpeechCancel = "speaker_cancel_64p_blue_red.png";
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
        public static void SetCultureSelectedLanguage()
        {
            try
            {
                //CodeLang.Culture = new CultureInfo(cLanguage);
                CultureInfo switchToCulture = new(cLanguage);
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
                cLanguageIso = cLanguageIso.Remove(cLanguageIso.Length - 1, 1);
            }

            return cLanguageIso;
        }

        /// <summary>
        /// Put the generator barcode formats in a List string
        /// </summary>
        /// <returns></returns>
        public static List<string> GetFormatCodeListGenerator()
        {
            return
            [
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
        }

        ///// <summary>
        ///// Put the scanner barcode formats in a List string
        ///// </summary>
        ///// <returns></returns>
        //public static List<string> GetFormatCodeListScanner()
        //{
        //    return
        //    [
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
        //}

        /// <summary>
        /// Put the scanner barcode formats in a List string for the Native scanner for Android
        /// https://developers.google.com/ml-kit/vision/barcode-scanning/android
        /// </summary>
        /// <returns></returns>
        public static List<string> GetFormatCodeListScannerNativeAndroid()
        {
            return
            [
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
        }

        /// <summary>
        /// Put the scanner barcode formats in a List string for the Native scanner for iOS
        /// https://developer.apple.com/documentation/avfoundation/avmetadataobject/objecttype
        /// </summary>
        /// <returns></returns>
        public static List<string> GetFormatCodeListScannerNativeIOS()
        {
            return
            [
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
        }

        /// <summary>
        /// Button share event: share the barcode result
        /// </summary>
        /// <param name="cText"></param>
        /// <returns></returns>
        public static async Task ShareBarcodeResultAsync(string cText)
        {
            if (cText is null or "")
            {
                return;
            }

            // For testing
            //cText = "http://www.google.com";
            //cText = "url http://www.google.com, visit website url https://www.microsoft.com, www.yahou.com and WWW.MODEGEERITS.BE and geertgeerits@gmail.com address";
            //cText = "Share text from barcode scanner";

            //string cPattern = @"(http|ftp|https):\/\/([\w\-_]+(?:(?:\.[\w\-_]+)+))([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?";
            string cPattern = @"((https?|ftp|file)\://|www.)[A-Za-z0-9\.\-]+(/[A-Za-z0-9\?\&\=;\+!'\(\)\*\-\._~%]*)*";

            // Call
            // Matches method for case-insensitive matching
            try
            {
                foreach (Match match in Regex.Matches(cText, cPattern, RegexOptions.IgnoreCase).Cast<Match>())
                {
                    if (match.Success)
                    {
                        bool bAnswer = await Application.Current!.MainPage!.DisplayAlert(CodeLang.OpenLinkTitle_Text, $"{match.Value}\n\n{CodeLang.OpenLinkText_Text}", CodeLang.Yes_Text, CodeLang.No_Text);

                        // Open link website
                        if (bAnswer)
                        {
                            await OpenWebsiteLinkAsync(match.Value);
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
#if DEBUG
                await Application.Current!.MainPage!.DisplayAlert(CodeLang.ErrorTitle_Text, ex.Message, CodeLang.ButtonClose_Text);
#endif
            }

            // Wait 700 milliseconds otherwise the ShareText() is not executed after the last opened link
            Task.Delay(700).Wait();

            // Open share interface
            await ShareTextAsync(cText);
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
                await Application.Current!.MainPage!.DisplayAlert(CodeLang.ErrorTitle_Text, ex.Message, CodeLang.ButtonClose_Text);
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
                await Application.Current!.MainPage!.DisplayAlert(CodeLang.ErrorTitle_Text, ex.Message, CodeLang.ButtonClose_Text);
#endif
            }
        }

        /// <summary>
        /// Initialize text to speech for the barcode scanner
        /// </summary>
        /// <param name="cPageName"></param>
        /// <returns></returns>
        public static async Task<bool> InitializeTextToSpeechScannerAsync(string cPageName)
        {
            if (!bLanguageLocalesExist)
            {
                return false;
            }

            try
            {
                locales = await TextToSpeech.Default.GetLocalesAsync();
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
#if DEBUG
                await Application.Current!.MainPage!.DisplayAlert(CodeLang.ErrorTitle_Text, ex.Message, CodeLang.ButtonClose_Text);
#endif
                return false;
            }

            return true;
        }

        /// <summary>
        /// Button text to speech event - Convert text to speech
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="cText"></param>
        /// <returns></returns>
        public static async Task ConvertTextToSpeechAsync(object sender, string cText)
        {
            var imageButton = (ImageButton)sender;

            // Start with the text to speech
            if (cText != null && cText != "")
            {
                bTextToSpeechIsBusy = true;
                imageButton.Source = cImageTextToSpeechCancel;

                try
                {
                    cts = new CancellationTokenSource();

                    SpeechOptions options = new()
                    {
                        Locale = locales?.Single(l => $"{l.Language}-{l.Country} {l.Name}" == cLanguageSpeech)
                    };

                    await TextToSpeech.Default.SpeakAsync(cText, options, cancelToken: cts.Token);
                    bTextToSpeechIsBusy = false;
                }
                catch (Exception ex)
                {
                    SentrySdk.CaptureException(ex);
#if DEBUG
                    await Application.Current!.MainPage!.DisplayAlert(CodeLang.ErrorTitle_Text, ex.Message, CodeLang.ButtonClose_Text);
#endif
                }

                imageButton.Source = cImageTextToSpeech;
            }
        }

        /// <summary>
        /// Cancel the text to speech
        /// </summary>
        /// <returns></returns>
        public static string CancelTextToSpeech()
        {
            // Cancel speech if a cancellation token exists & hasn't been already requested
            if (bTextToSpeechIsBusy)
            {
                if (cts?.IsCancellationRequested ?? true)
                    return cImageTextToSpeechCancel;

                cts.Cancel();
                bTextToSpeechIsBusy = false;
            }
        
            return cImageTextToSpeech;
        }
    }
}