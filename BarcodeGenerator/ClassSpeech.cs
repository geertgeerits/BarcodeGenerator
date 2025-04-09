﻿using System.Diagnostics;

namespace BarcodeGenerator
{
    internal sealed class ClassSpeech
    {
        private static string[]? cLanguageLocales;
        private static IEnumerable<Locale>? locales;
        private static CancellationTokenSource? cts;

        /// <summary>
        /// Initialize text to speech
        /// </summary>
        public static async void InitializeTextToSpeechZZZ()
        {
            Globals.bTextToSpeechAvailable = await InitializeTextToSpeechAsyncZZZ();
        }

        /// <summary>
        /// Initialize text to speech and fill the the array with the speech languages
        /// <para>.Country = KR ; .Id = ''  ; .Language = ko ; .Name = Korean (South Korea) ;</para>
        /// </summary>
        public static async Task<bool> InitializeTextToSpeechAsyncZZZ()
        {
            // Initialize text to speech
            int nTotalItems;

            try
            {
                locales = await TextToSpeech.Default.GetLocalesAsync();

                nTotalItems = locales.Count();

                if (nTotalItems == 0)
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                // Text to speech is not supported on this device
                //SentrySdk.CaptureException(ex);
#if DEBUG
                await Application.Current!.Windows[0].Page!.DisplayAlert(CodeLang.ErrorTitle_Text, ex.Message + "\n\n" + CodeLang.TextToSpeechError_Text, CodeLang.ButtonClose_Text);
#endif
                return false;
            }

            // Put the locales in the array and sort the array
            cLanguageLocales = new string[nTotalItems];
            int nItem = 0;

            foreach (var l in locales)
            {
                cLanguageLocales[nItem] = $"{l.Language}-{l.Country} {l.Name}";
                nItem++;
            }

            Array.Sort(cLanguageLocales);

            return true;
        }

        /// <summary>
        /// Fill the picker with the speech languages
        /// </summary>
        /// <param name="picker"></param>
        public static void FillPickerWithSpeechLanguages(Picker picker)
        {
            // If there are no locales, disable the picker and return
            if (cLanguageLocales is null)
            {
                picker.IsEnabled = false;
                return;
            }

            // Populate the picker with sorted locales
            foreach (var locale in cLanguageLocales)
            {
                picker.Items.Add(locale);
            }

            // Select the saved language
            picker.SelectedIndex = SearchArrayWithSpeechLanguages(Globals.cLanguageSpeech);

            Debug.WriteLine("FillPickerWithSpeechLanguages - Globals.cLanguageSpeech: " + Globals.cLanguageSpeech);
        }

        /// <summary>
        /// Search the selected language in the cLanguageLocales array
        /// </summary>
        /// <param name="cCultureName"></param>
        public static int SearchArrayWithSpeechLanguages(string cCultureName)
        {
            Debug.WriteLine("SearchArrayWithSpeechLanguages - cCultureName IN: " + cCultureName);

            try
            {
                int nTotalItems = cLanguageLocales?.Length ?? 0;

                if (cLanguageLocales is not null)
                {
                    if (!string.IsNullOrEmpty(cCultureName))
                    {
                        // Search for the Indonesian or Hebrew or Yiddish language code, if not found search for the old language code
                        // Android generating old/wrong language codes - https://stackoverflow.com/questions/44245959/android-generating-wrong-language-code-for-indonesia
                        if (cCultureName.StartsWith("id") || cCultureName.StartsWith("he") || cCultureName.StartsWith("yi"))
                        {
                            for (int nItem = 0; nItem < nTotalItems; nItem++)
                            {
                                if (cLanguageLocales[nItem].StartsWith(cCultureName))
                                {
                                    Globals.cLanguageSpeech = cLanguageLocales[nItem];
                                    return nItem;
                                }
                            }

                            // Map new language codes to old codes
                            cCultureName = GetCurrentLanguageTag(cCultureName);
                            Debug.WriteLine("SearchArrayWithSpeechLanguages - cCultureName NEW to OLD: " + cCultureName);
                        }

                        // Search for the speech language as 'en-US'
                        for (int nItem = 0; nItem < nTotalItems; nItem++)
                        {
                            if (cLanguageLocales[nItem].StartsWith(cCultureName))
                            {
                                Globals.cLanguageSpeech = cLanguageLocales[nItem];
                                return nItem;
                            }
                        }

                        // Select the characters before the first hyphen if there is a hyphen in the string
                        cCultureName = cCultureName.Split('-')[0];

                        // Search for the speech language as 'en'
                        for (int nItem = 0; nTotalItems > nItem; nItem++)
                        {
                            if (cLanguageLocales[nItem].StartsWith(cCultureName))
                            {
                                Globals.cLanguageSpeech = cLanguageLocales[nItem];
                                return nItem;
                            }
                        }
                    }
                }

                // If the language is not found use the first language in the array
                if (string.IsNullOrEmpty(Globals.cLanguageSpeech) && nTotalItems > 0)
                {
                    Globals.cLanguageSpeech = cLanguageLocales![0];
                    return 0;
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Application.Current!.Windows[0].Page!.DisplayAlert(CodeLang.ErrorTitle_Text, ex.Message, CodeLang.ButtonClose_Text);
#endif
            }

            return 0;
        }

        /// <summary>
        /// Map new language codes to old codes
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentLanguageTag(string languageTag)
        {
            // Map new language codes to old ones
            return languageTag switch
            {
                "id" => "in",           // Indonesian - Changed in 1989 to 'id'
                "id-ID" => "in-ID",
                "he" => "iw",           // Hebrew - Changed in 1989 to 'he'
                "he-IL" => "iw-IL",
                "yi" => "ji",           // Yiddish - Changed in 1989 to 'yi'
                _ => languageTag
            };
        }

        /// <summary>
        /// Convert text to speech
        /// </summary>
        /// <param name="cText"></param>
        /// <returns></returns>
        public static async Task ConvertTextToSpeechAsync(object sender, string cText)
        {
            /* If you do not wait long enough to press the arrow key in the Task 'MakeExplainTurnAsync()',
               an error message will sometimes appear: 'The operation was canceled'.
               This only occurs if the 'Explained by speech' setting is enabled.
               The error occurs in the method 'ConvertTextToSpeechAsync()'. */

            // Cancel the text to speech
            if (Globals.bTextToSpeechIsBusy)
            {
                if (cts?.IsCancellationRequested ?? true)
                {
                    return;
                }

                cts.Cancel();
            }

            var imageButton = (ImageButton)sender;

            // Start with the text to speech
            Debug.WriteLine("ConvertTextToSpeechAsync + cText: " + cText);
            Debug.WriteLine("ConvertTextToSpeechAsync + Globals.cLanguageSpeech: " + Globals.cLanguageSpeech);

            if (cText is not null and not "")
            {
                Globals.bTextToSpeechIsBusy = true;
                imageButton.Source = Globals.cImageTextToSpeechCancel;

                try
                {
                    cts = new CancellationTokenSource();

                    SpeechOptions options = new()
                    {
                        Locale = locales?.Single(static l => $"{l.Language}-{l.Country} {l.Name}" == Globals.cLanguageSpeech)
                    };

                    await TextToSpeech.Default.SpeakAsync(cText, options, cancelToken: cts.Token);
                    Globals.bTextToSpeechIsBusy = false;
                }
                catch (Exception ex)
                {
                    //SentrySdk.CaptureException(ex);
#if DEBUG
                    await Application.Current!.Windows[0].Page!.DisplayAlert(CodeLang.ErrorTitle_Text, $"{ex.Message}\n{ex.StackTrace}", CodeLang.ButtonClose_Text);
#endif
                }

                imageButton.Source = Globals.cImageTextToSpeech;
            }
        }

        /// <summary>
        /// Cancel speech if a cancellation token exists and hasn't been already requested
        /// </summary>
        public static string CancelTextToSpeech()
        {
            if (Globals.bTextToSpeechIsBusy)
            {
                if (cts?.IsCancellationRequested ?? true)
                {
                    return Globals.cImageTextToSpeechCancel;
                }

                cts.Cancel();
                Globals.bTextToSpeechIsBusy = false;
            }

            return Globals.cImageTextToSpeech;
        }

        /// <summary>
        /// Initialize text to speech and fill the the array with the speech languages
        /// .Country = KR ; .Id = ''  ; .Language = ko ; .Name = Korean (South Korea) ; 
        /// </summary>
        /// <param name="cCultureName"></param>
        public static async void InitializeTextToSpeech(string cCultureName)
        {
            // Initialize text to speech
            Globals.bTextToSpeechAvailable = false;
            int nTotalItems;

            try
            {
                locales = await TextToSpeech.Default.GetLocalesAsync();

                nTotalItems = locales.Count();

                if (nTotalItems == 0)
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
#if DEBUG
                await Application.Current!.Windows[0].Page!.DisplayAlert(CodeLang.ErrorTitle_Text, $"{ex.Message}\n\n{CodeLang.TextToSpeechError_Text}", CodeLang.ButtonClose_Text);
#endif
                return;
            }

            Globals.bLanguageLocalesExist = true;

            // Put the locales in the array and sort the array
            cLanguageLocales = new string[nTotalItems];
            int nItem = 0;

            foreach (var l in locales)
            {
                cLanguageLocales[nItem] = $"{l.Language}-{l.Country} {l.Name}";
                nItem++;
            }

            Array.Sort(cLanguageLocales);

            Globals.bTextToSpeechAvailable = true;

            // Search for the language after a first start or reset of the application
            if (string.IsNullOrEmpty(Globals.cLanguageSpeech))
            {
                SearchArrayWithSpeechLanguages(cCultureName);
            }
        }
    }
}
