using BarcodeGenerator.Resources.Languages;

namespace BarcodeGenerator
{
    public sealed partial class PageSettings : ContentPage
    {
        //// Local variables
        private const string cHexCharacters = "0123456789ABCDEFabcdef";
        private readonly Stopwatch stopWatch = new();

        public PageSettings()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
#if DEBUG            
                DisplayAlert("InitializeComponent: PageSettings", ex.Message, "OK");
#endif
                return;
            }
#if ANDROID
            // !!!BUG!!! in Android - Return typ 'Done' is not working on Android
            entHexColorBg.ReturnType = ReturnType.Next;
#endif
            //// Put text in the chosen language in the controls and variables
            SetLanguage();

            //// Set the current language in the picker
            pckLanguage.SelectedIndex = Globals.cLanguage switch
            {
                "cs" => 0,      // Čeština - Czech
                "da" => 1,      // Dansk - Danish
                "de" => 2,      // Deutsch - German
                "es" => 4,      // Español - Spanish
                "fr" => 5,      // Français - French
                "it" => 6,      // Italiano - Italian
                "hu" => 7,      // Magyar - Hungarian
                "nl" => 8,      // Nederlands - Dutch
                "nb" => 9,      // Norsk Bokmål - Norwegian Bokmål
                "pl" => 10,     // Polski - Polish
                "pt" => 11,     // Português - Portuguese
                "ro" => 12,     // Română - Romanian
                "fi" => 13,     // Suomi - Finnish
                "sv" => 14,     // Svenska - Swedish
                _ => 3,         // English
            };

            //// Fill the picker with the speech languages and set the saved language in the picker
            FillPickerWithSpeechLanguages();

            //// Set the current theme in the picker
            pckTheme.SelectedIndex = Globals.cTheme switch
            {
                "Light" => 1,   // Light
                "Dark" => 2,    // Dark
                _ => 0,         // System
            };

            //// Set the barcode list and the current default barcode format in the picker for the barcode generator
            pckFormatCodeGenerator.ItemsSource = Globals.GetFormatCodeListGenerator();
            pckFormatCodeGenerator.SelectedIndex = Globals.nFormatGeneratorIndex;

            //// Set the barcode list and the current default barcode format in the picker for the barcode scanner
#if ANDROID
            pckFormatCodeScanner.ItemsSource = Globals.GetFormatCodeListScannerNativeAndroid();
#elif IOS
            pckFormatCodeScanner.ItemsSource = Globals.GetFormatCodeListScannerNativeIOS();
#endif
            pckFormatCodeScanner.SelectedIndex = Globals.nFormatScannerIndex;

            //// Set the current color in the entry and on the sliders
            int nOpacity = 0;
            int nRed = 0;
            int nGreen = 0;
            int nBlue = 0;

            entHexColorFg.Text = Globals.cCodeColorFg;

            HexToRgbColor(Globals.cCodeColorFg, ref nOpacity, ref nRed, ref nGreen, ref nBlue);

            sldOpacityFg.Value = nOpacity;
            sldColorFgRed.Value = nRed;
            sldColorFgGreen.Value = nGreen;
            sldColorFgBlue.Value = nBlue;

            entHexColorBg.Text = Globals.cCodeColorBg;

            HexToRgbColor(Globals.cCodeColorBg, ref nOpacity, ref nRed, ref nGreen, ref nBlue);

            sldOpacityBg.Value = nOpacity;
            sldColorBgRed.Value = nRed;
            sldColorBgGreen.Value = nGreen;
            sldColorBgBlue.Value = nBlue;

            //// Start the stopWatch for resetting all the settings
            stopWatch.Start();
        }

        /// <summary>
        /// Picker language clicked event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPickerLanguageChanged(object sender, EventArgs e)
        {
            string cLanguageOld = Globals.cLanguage;

            Picker picker = (Picker)sender;
            int selectedIndex = picker.SelectedIndex;

            if (selectedIndex != -1)
            {
                Globals.cLanguage = selectedIndex switch
                {
                    0 => "cs",      // Čeština - Czech
                    1 => "da",      // Dansk - Danish
                    2 => "de",      // Deutsch - German
                    4 => "es",      // Español - Spanish
                    5 => "fr",      // Français - French
                    6 => "it",      // Italiano - Italian
                    7 => "hu",      // Magyar - Hungarian
                    8 => "nl",      // Nederlands - Dutch
                    9 => "nb",      // Norsk Bokmål - Norwegian Bokmål
                    10 => "pl",     // Polski - Polish
                    11 => "pt",     // Português - Portuguese
                    12 => "ro",     // Română - Romanian
                    13 => "fi",     // Suomi - Finnish
                    14 => "sv",     // Svenska - Swedish
                    _ => "en",      // English
                };
            }

            if (cLanguageOld != Globals.cLanguage)
            {
                Globals.bLanguageChanged = true;

                // Set the current UI culture of the selected language
                Globals.SetCultureSelectedLanguage();

                // Put text in the chosen language in the controls and variables
                SetLanguage();

                // Search the new language in the cLanguageLocales array and select the new speech language
                if (Globals.cLanguageLocales is not null)
                {
                    int nTotalItems = Globals.cLanguageLocales.Length;

                    for (int nItem = 0; nItem < nTotalItems; nItem++)
                    {
                        if (Globals.cLanguageLocales[nItem].StartsWith(Globals.cLanguage))
                        {
                            pckLanguageSpeech.SelectedIndex = nItem;
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Put text in the chosen language in the controls and variables
        /// </summary>
        private void SetLanguage()
        {
            // Set the barcode list and the current default barcode format in the picker for the barcode scanner
#if ANDROID
            pckFormatCodeScanner.ItemsSource = Globals.GetFormatCodeListScannerNativeAndroid();
#elif IOS
            pckFormatCodeScanner.ItemsSource = Globals.GetFormatCodeListScannerNativeIOS();
#endif
            pckFormatCodeScanner.SelectedIndex = Globals.nFormatScannerIndex;

            // Set the current theme in the picker
            List<string> ThemeList =
            [
                CodeLang.ThemeSystem_Text,
                CodeLang.ThemeLight_Text,
                CodeLang.ThemeDark_Text
            ];
            pckTheme.ItemsSource = ThemeList;

            pckTheme.SelectedIndex = Globals.cTheme switch
            {
                "Light" => 1,   // Light
                "Dark" => 2,    // Dark
                _ => 0,         // System
            };
        }

        /// <summary>
        /// Fill the picker with the speech languages from the array
        /// .Country = KR ; .Id = ''  ; .Language = ko ; .Name = Korean (South Korea) ;
        /// </summary>
        private void FillPickerWithSpeechLanguages()
        {
            // If there are no locales then return
            bool bIsSetSelectedIndex = false;

            if (!Globals.bLanguageLocalesExist)
            {
                pckLanguageSpeech.IsEnabled = false;           
                return;
            }

            // Put the sorted locales from the array in the picker and select the saved language
            if (Globals.cLanguageLocales is not null)
            {
                int nTotalItems = Globals.cLanguageLocales.Length;

                for (int nItem = 0; nItem < nTotalItems; nItem++)
                {
                    pckLanguageSpeech.Items.Add(Globals.cLanguageLocales[nItem]);

                    if (Globals.cLanguageSpeech == Globals.cLanguageLocales[nItem])
                    {
                        pckLanguageSpeech.SelectedIndex = nItem;
                        bIsSetSelectedIndex = true;
                    }
                }
            }

            // If the language is not found set the picker to the first item
            if (!bIsSetSelectedIndex)
            {
                pckLanguageSpeech.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Picker speech language clicked event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPickerLanguageSpeechChanged(object sender, EventArgs e)
        {
            Picker picker = (Picker)sender;
            int selectedIndex = picker.SelectedIndex;

            if (selectedIndex != -1)
            {
                Globals.cLanguageSpeech = picker.Items[selectedIndex];
            }
        }

        /// <summary>
        /// Picker theme clicked event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPickerThemeChanged(object sender, EventArgs e)
        {
            Picker picker = (Picker)sender;
            int selectedIndex = picker.SelectedIndex;

            if (selectedIndex != -1)
            {
                Globals.cTheme = selectedIndex switch
                {
                    1 => "Light",   // Light
                    2 => "Dark",    // Dark
                    _ => "System",  // System
                };

                // Set the theme
                Globals.SetTheme();
            }
        }

        /// <summary>
        /// Picker format generator clicked event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPickerFormatCodeGeneratorChanged(object sender, EventArgs e)
        {
            Picker picker = (Picker)sender;
            int selectedIndex = picker.SelectedIndex;

            if (selectedIndex != -1)
            {
                Globals.nFormatGeneratorIndex = selectedIndex;
            }
        }

        /// <summary>
        /// Picker format scanner clicked event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPickerFormatCodeScannerChanged(object sender, EventArgs e)
        {
            Picker picker = (Picker)sender;
            int selectedIndex = picker.SelectedIndex;

            if (selectedIndex != -1)
            {
                Globals.nFormatScannerIndex = selectedIndex;
            }
        }

        /// <summary>
        /// On entry HexColor text changed event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EntryHexColorTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!IsHex(e.NewTextValue))
            {
                ((Entry)sender).Text = e.OldTextValue;
            }
        }

        /// <summary>
        /// Test if the text is a hex value
        /// </summary>
        /// <param name="cText"></param>
        /// <returns></returns>
        private static bool IsHex(string cText)
        {
            foreach (char c in cText)
            {
                if (!cHexCharacters.Contains(c))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Display help for Hex color
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnSettingsHexColorHelpClicked(object sender, EventArgs e)
        {
            await DisplayAlert("?", $"{CodeLang.HexColorCodes_Text}\n\n{CodeLang.AllowedChar_Text}\n{cHexCharacters}", CodeLang.ButtonClose_Text);
        }

        /// <summary>
        /// Entry HexColor Unfocused event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EntryHexColorUnfocused(object sender, EventArgs e)
        {
            Entry entry = (Entry)sender;

            // Add the opacity if length = 6 characters
            if (entry.Text.Length == 6)
            {
                entry.Text = $"FF{entry.Text}";
            }

            // Length must be 8 characters
            if (entry.Text.Length != 8)
            {
                _ = entry.Focus();
                return;
            }

            // Set the sliders position
            int nOpacity = 0;
            int nRed = 0;
            int nGreen = 0;
            int nBlue = 0;

            if (entry == entHexColorFg)
            {
                Globals.cCodeColorFg = entHexColorFg.Text;

                HexToRgbColor(Globals.cCodeColorFg, ref nOpacity, ref nRed, ref nGreen, ref nBlue);

                sldOpacityFg.Value = nOpacity;
                sldColorFgRed.Value = nRed;
                sldColorFgGreen.Value = nGreen;
                sldColorFgBlue.Value = nBlue;
            }
            else
            {
                Globals.cCodeColorBg = entHexColorBg.Text;

                HexToRgbColor(Globals.cCodeColorBg, ref nOpacity, ref nRed, ref nGreen, ref nBlue);

                sldOpacityBg.Value = nOpacity;
                sldColorBgRed.Value = nRed;
                sldColorBgGreen.Value = nGreen;
                sldColorBgBlue.Value = nBlue;
            }

            // Set focus to the next button
            if (sender.Equals(entHexColorFg))
            {
                _ = entHexColorBg.Focus();
            }
            else
            {
                _ = entDummy.Focus();
            }
        }

        /// <summary>
        /// Hide the keyboard when the dummy entry is focused
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EntDummyFocused(object sender, FocusEventArgs e)
        {
            entDummy.IsEnabled = false;
            entDummy.IsEnabled = true;
        }

        /// <summary>
        /// Slider color barcode Foreground value change
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnSliderColorForegroundValueChanged(object sender, ValueChangedEventArgs args)
        {
            int nAmountOpacity = 0;
            int nColorRed = 0;
            int nColorGreen = 0;
            int nColorBlue = 0;

            Slider slider = (Slider)sender;

            if (slider == sldOpacityFg)
            {
                nAmountOpacity = (int)args.NewValue;
                nColorRed = (int)sldColorFgRed.Value;
                nColorGreen = (int)sldColorFgGreen.Value;
                nColorBlue = (int)sldColorFgBlue.Value;
            }
            else if (slider == sldColorFgRed)
            {
                nAmountOpacity = (int)sldOpacityFg.Value;
                nColorRed = (int)args.NewValue;
                nColorGreen = (int)sldColorFgGreen.Value;
                nColorBlue = (int)sldColorFgBlue.Value;
            }
            else if (slider == sldColorFgGreen)
            {
                nAmountOpacity = (int)sldOpacityFg.Value;
                nColorRed = (int)sldColorFgRed.Value;
                nColorGreen = (int)args.NewValue;
                nColorBlue = (int)sldColorFgBlue.Value;
            }
            else if (slider == sldColorFgBlue)
            {
                nAmountOpacity = (int)sldOpacityFg.Value;
                nColorRed = (int)sldColorFgRed.Value;
                nColorGreen = (int)sldColorFgGreen.Value;
                nColorBlue = (int)args.NewValue;
            }

            string cColorFgHex = $"{nAmountOpacity:X2}{nColorRed:X2}{nColorGreen:X2}{nColorBlue:X2}";
            entHexColorFg.Text = cColorFgHex;
            bxvColorFg.Color = Color.FromArgb(cColorFgHex);

            Globals.cCodeColorFg = cColorFgHex;

            /* Explanation of the line below
            string cColorFgHex = $"{nAmountOpacity:X2}{nColorRed:X2}{nColorGreen:X2}{nColorBlue:X2}";

            The code you provided is using string interpolation in C#
            The $ sign is used to indicate that the string is an interpolated string
            The X2 format specifier formats the number as a hexadecimal value with a minimum width of 2 digits
            The nAmountOpacity, nColorRed, nColorGreen, and nColorBlue are variables that are being formatted into a single string
            So, the resulting string will be a concatenation of the hexadecimal values of nAmountOpacity, nColorRed, nColorGreen, and nColorBlue
            The resulting string will have a length of 8 characters (2 characters for each variable)
            */
        }

        /// <summary>
        /// Slider color barcode background value change
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnSliderColorBackgroundValueChanged(object sender, ValueChangedEventArgs args)
        {
            int nAmountOpacity = 0;
            int nColorRed = 0;
            int nColorGreen = 0;
            int nColorBlue = 0;

            Slider slider = (Slider)sender;

            if (slider == sldOpacityBg)
            {
                nAmountOpacity = (int)args.NewValue;
                nColorRed = (int)sldColorBgRed.Value;
                nColorGreen = (int)sldColorBgGreen.Value;
                nColorBlue = (int)sldColorBgBlue.Value;
            }
            else if (slider == sldColorBgRed)
            {
                nAmountOpacity = (int)sldOpacityBg.Value;
                nColorRed = (int)args.NewValue;
                nColorGreen = (int)sldColorBgGreen.Value;
                nColorBlue = (int)sldColorBgBlue.Value;
            }
            else if (slider == sldColorBgGreen)
            {
                nAmountOpacity = (int)sldOpacityBg.Value;
                nColorRed = (int)sldColorBgRed.Value;
                nColorGreen = (int)args.NewValue;
                nColorBlue = (int)sldColorBgBlue.Value;
            }
            else if (slider == sldColorBgBlue)
            {
                nAmountOpacity = (int)sldOpacityBg.Value;
                nColorRed = (int)sldColorBgRed.Value;
                nColorGreen = (int)sldColorBgGreen.Value;
                nColorBlue = (int)args.NewValue;
            }

            string cColorBgHex = $"{nAmountOpacity:X2}{nColorRed:X2}{nColorGreen:X2}{nColorBlue:X2}";
            entHexColorBg.Text = cColorBgHex;
            bxvColorBg.Color = Color.FromArgb(cColorBgHex);

            Globals.cCodeColorBg = cColorBgHex;
        }

        /// <summary>
        /// Convert OORRGGBB Hex color to RGB color
        /// </summary>
        /// <param name="cHexColor"></param>
        /// <param name="nOpacity"></param>
        /// <param name="nRed"></param>
        /// <param name="nGreen"></param>
        /// <param name="nBlue"></param>
        private static void HexToRgbColor(string cHexColor, ref int nOpacity, ref int nRed, ref int nGreen, ref int nBlue)
        {
            // Remove leading # if present
            if (cHexColor[..1] == "#")
            {
                cHexColor = cHexColor[1..];
            }

            nOpacity = int.Parse(cHexColor[..2], NumberStyles.AllowHexSpecifier);
            nRed = int.Parse(cHexColor.Substring(2, 2), NumberStyles.AllowHexSpecifier);
            nGreen = int.Parse(cHexColor.Substring(4, 2), NumberStyles.AllowHexSpecifier);
            nBlue = int.Parse(cHexColor.Substring(6, 2), NumberStyles.AllowHexSpecifier);
        }

        /// <summary>
        /// Button save settings clicked event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnSettingsSaveClicked(object sender, EventArgs e)
        {
            Preferences.Default.Set("SettingTheme", Globals.cTheme);
            Preferences.Default.Set("SettingFormatGeneratorIndex", Globals.nFormatGeneratorIndex);
            Preferences.Default.Set("SettingFormatScannerIndex", Globals.nFormatScannerIndex);
            Preferences.Default.Set("SettingCodeColorFg", Globals.cCodeColorFg);
            Preferences.Default.Set("SettingCodeColorBg", Globals.cCodeColorBg);
            Preferences.Default.Set("SettingLanguage", Globals.cLanguage);
            Preferences.Default.Set("SettingLanguageSpeech", Globals.cLanguageSpeech);

            // Give it some time to save the settings
            Task.Delay(400).Wait();

            // Restart the application
            Application.Current!.Windows[0].Page = new AppShell();
        }

        /// <summary>
        /// Button reset settings clicked event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSettingsResetClicked(object sender, EventArgs e)
        {
            // Get the elapsed time in milli seconds
            stopWatch.Stop();

            if (stopWatch.ElapsedMilliseconds < 2001)
            {
                // Clear all settings after the first clicked event within the first 2 seconds after opening the setting page
                Preferences.Default.Clear();
            }
            else
            {
                // Reset some settings
                Preferences.Default.Remove("SettingTheme");
                Preferences.Default.Remove("SettingFormatGeneratorIndex");
                Preferences.Default.Remove("SettingFormatScannerIndex");
                Preferences.Default.Remove("SettingCodeColorFg");
                Preferences.Default.Remove("SettingCodeColorBg");
                Preferences.Default.Remove("SettingLanguage");
                Preferences.Default.Remove("SettingLanguageSpeech");
                Preferences.Default.Remove("SettingQualityCameraBack");
                Preferences.Default.Remove("SettingQualityCameraFront");
            }

            // Give it some time to remove the settings
            Task.Delay(400).Wait();

            // Restart the application
            Application.Current!.Windows[0].Page = new AppShell();
        }
    }
}
