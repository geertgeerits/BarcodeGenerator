namespace BarcodeGenerator
{
    // Global variables and methods
    internal static class Globals
    {
        // Languages where the flow direction is right-to-left:
        /* ISO 639-1: ar:Arabic, dv:Divehi/Maldivian, fa:Dari/Persian (Farsi), ff:Fula, he:Hebrew, iw:Hebrew, kd:Kurdish (Sorani), pk:Panjabi-Shahmuki,
                      ps:Pushto/Pashto, ug:Uighur/Uyghur, ur:Urdu, yi:Yiddish
           ISO 639-2: arc:Aramaic, nqo:N'ko, rhg:Rohingya, syr:Syriac
           az:Azeri: when written in Latin or Cyrillic scripts, Azeri is left-to-right (LTR), in Arabic script, it is right-to-left (RTL) */
        //private static readonly HashSet<string> cRightToLeftLanguages_ISO639_1 =
        //[
        //    "ar", "dv", "fa", "ff", "he", "iw", "kd", "pk", "ps", "ug", "ur", "yi"
        //];

        // Global variables
        public static string cTheme = string.Empty;                 // Theme: Light, Dark or System
        public static double nFontSize;                             // Font size for the UI
        public static string cLanguage = string.Empty;              // Language for the UI (culture name, e.g. "en-US")
        public static bool bLanguageChanged;                        // Flag to indicate if the language has been changed
        public static string cLanguageSpeech = string.Empty;        // Language for text-to-speech
        public static bool bTextToSpeechAvailable;                  // Flag to indicate if text-to-speech is available
        public static bool bTextToSpeechIsBusy;                     // Flag to indicate if text-to-speech is currently busy
        public static readonly string cImageTextToSpeech = "speaker_64p_blue_green.png";  // Image for text-to-speech button
        public static readonly string cImageTextToSpeechCancel = "speaker_cancel_64p_blue_red.png";  // Image for text-to-speech cancel button
        public static bool bPopupOpened;                            // Popup has been opened, used to prevent the OnAppearing event to change the pckFormatCodeGenerator index on the MainPage
        public static bool bPopupCanceled;                          // Popup was canceled by the user
        public static bool bLicense;                                // Flag to indicate if the user has accepted the license agreement
        public static string cCurrentPage = string.Empty;           // Name of the current page

        // Global methods

        /// <summary>
        /// Set the current UI culture of the selected language
        /// </summary>
        public static void SetCultureSelectedLanguage(string cCultureName)
        {
            try
            {
                CultureInfo culture = new(cCultureName);
                LocalizationResourceManager.Instance.SetCulture(culture);

                // Necessary for the determination of the flow direction
                CultureInfo.CurrentUICulture = new CultureInfo(cCultureName);
            }
            catch
            {
                // Do nothing
            }
        }

        // Set the flow direction of the text elements
        public static void SetFlowDirection(VisualElement element)
        {
            // Get the flow direction of the current UI culture
            bool bIsRightToLeft = CultureInfo.CurrentUICulture.TextInfo.IsRightToLeft;
            Debug.WriteLine($"CurrentUICulture: {CultureInfo.CurrentUICulture.Name}, IsRightToLeft: {bIsRightToLeft}");

            // Set the flow direction to right-to-left
            //if (cRightToLeftLanguages_ISO639_1.Contains(cLanguage))
            if (bIsRightToLeft)
            {
                if (element.FlowDirection != FlowDirection.RightToLeft)
                {
                    element.FlowDirection = FlowDirection.RightToLeft;
                }
            }
            // Set the flow direction to left-to-right
            else
            {
                if (element.FlowDirection != FlowDirection.LeftToRight)
                {
                    element.FlowDirection = FlowDirection.LeftToRight;
                }
            }
        }

        /// <summary>
        /// Set the global font size
        /// </summary>
        public static void SetGlobalFontSize()
        {
            Application.Current?.Resources["GlobalFontSize"] = nFontSize;
        }

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
    }
}