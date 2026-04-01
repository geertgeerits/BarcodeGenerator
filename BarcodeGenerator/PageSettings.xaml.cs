using CommunityToolkit.Maui.Extensions;

namespace BarcodeGenerator
{
    public sealed partial class PageSettings : ContentPage
    {
        // Local variables
        private const string cHexCharacters = "0123456789ABCDEFabcdef";
        private const string cDecimalCharacters = "0123456789";
        private readonly Stopwatch stopWatch = new();
        private string searchKeyGenerator = string.Empty;
        private string searchKeyScanner = string.Empty;
        //private bool hasWorkaroundRow;

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
                DisplayAlertAsync("InitializeComponent: PageSettings", ex.Message, "OK");
#endif
                return;
            }

#if ANDROID
            // Android !!!BUG!!! SafeAreaEdges not behaving as expected #33922 - https://github.com/dotnet/maui/issues/33922
            // A padding with a height of 50 at the bottom is added to the grid to workaround this issue and push the content
            // above the navigation bar on Android when the entry is unfocused and the soft input keyboard is hidden.
            // Happens when the 'Entry ReturnType' is set to 'Done' and the soft keyboard is hiding after pressing 'Done'.
            // Workaround: use always 'Next' and handle the focus in the GoToNextField method to go to the next field or unfocus the last field.
            // Happens most with the Microsoft SwiftKey keyboard, the Samsung and Google keyboards have it less or not at all.
            entHexColorBg.ReturnType = ReturnType.Next;
#endif

            // Put text in the chosen language in the controls and variables
            SetLanguage();

            // Select the current language in the picker
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
                _ => 3          // English
            };

            // Initialize the barcode formats in the ClassBarcodes class to update the format names in the selected language
            ClassBarcodes.InitializeBarcodeSearchFormats();

            // Fill the picker with the speech languages and select the current language in the picker
            ClassSpeech.FillPickerWithSpeechLanguages(pckLanguageSpeech);

            // Select the current theme in the picker
            pckTheme.SelectedIndex = Globals.cTheme switch
            {
                "Light" => 1,   // Light
                "Dark" => 2,    // Dark
                _ => 0          // System
            };

            // Set the current color in the entry and on the sliders
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

            // Set the QR code image size to update the switch and entry
            swtQRCodeSizeVariable.IsToggled = ClassQRCodeImage.bQRCodeSizeVariable;
            entQRCodeSizePixels.Text = ClassQRCodeImage.nQRCodeSizePixels.ToString();

            // Set the barcode with caption variable to update the switch
            swtBarcodeWithCaption.IsToggled = Globals.bBarcodeWithCaption;

            // Start the stopWatch for resetting all the settings
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
                    _ => "en"       // English
                };
            }

            if (cLanguageOld != Globals.cLanguage)
            {
                Globals.bLanguageChanged = true;

                // Search the barcode generator and scanner name with the search key to get the new name in the selected language
                searchKeyGenerator = ClassBarcodes.SearchValueInDictionary(ClassBarcodes.cBarcodeGeneratorName);
                searchKeyScanner = ClassBarcodes.SearchValueInDictionary(ClassBarcodes.cBarcodeScannerName);

                // Set the current UI culture of the selected language
                Globals.SetCultureSelectedLanguage(Globals.cLanguage);

                // Put text in the chosen language in the controls and variables
                SetLanguage();

                // Search the selected language in the cLanguageLocales array and select the new speech language
                pckLanguageSpeech.SelectedIndex = ClassSpeech.SearchArrayWithSpeechLanguages(Globals.cLanguage);
                Debug.WriteLine("pckLanguageSpeech.SelectedIndex OUT: " + pckLanguageSpeech.SelectedIndex);
            }
        }

        /// <summary>
        /// Put text in the chosen language in the controls and variables
        /// The pickers 'pckTheme', 'pckFormatCodeGenerator' and 'pckFormatCodeScanner' are set in this method
        /// because some values in the pickers are different depending on the language
        /// </summary>
        private void SetLanguage()
        {
            // Initialize the barcode formats in the ClassBarcodes class to update the format names in the selected language
            ClassBarcodes.InitializeBarcodeFormats();

            // Set the generator barcode formats in the picker
#if WINDOWS
            pckFormatCodeGenerator.ItemsSource = ClassBarcodes.GetFormatCodeListGenerator_ZX_Windows();
#else
            pckFormatCodeGenerator.ItemsSource = ClassBarcodes.GetFormatCodeListGenerator_ZX();
#endif

            // Set the scanner barcode formats in the picker
#if ANDROID
            pckFormatCodeScanner.ItemsSource = ClassBarcodes.GetFormatCodeListScanner_NT_Android();
#elif IOS
            pckFormatCodeScanner.ItemsSource = ClassBarcodes.GetFormatCodeListScanner_NT_IOS();
#elif WINDOWS
            //pckFormatCodeScanner.ItemsSource = ClassBarcodes.GetFormatCodeListScanner_NT_Windows();
            pckFormatCodeScanner.ItemsSource = ClassBarcodes.GetFormatCodeListScanner_ZX();
#endif

            // Initialize the barcode formats in the ClassBarcodes class to update the format names in the selected language and get the new barcode generator name with the search key
            ClassBarcodes.InitializeBarcodeSearchFormats();

            // Search the barcode generator and scanner name with the search key to get the new name in the selected language
            if (Globals.bLanguageChanged)
            {
                ClassBarcodes.cBarcodeGeneratorName = ClassBarcodes.SearchKeyInDictionary(searchKeyGenerator);
                ClassBarcodes.cBarcodeScannerName = ClassBarcodes.SearchKeyInDictionary(searchKeyScanner);
            }

            // Select the current barcode format in the picker for the barcode generator and scanner
            ClassBarcodes.SelectBarcodeGeneratorNameIndex(pckFormatCodeGenerator);
            ClassBarcodes.SelectBarcodeScannerNameIndex(pckFormatCodeScanner);

            // Set the QR code image size percent in the label
            lblQRCodeImageSize.Text = $"{string.Format(CodeLang.QRCodeImageSize_Text, ClassQRCodeImage.nQRCodeImageSizePercent)}";
            sldQRCodeImageSize.Value = ClassQRCodeImage.nQRCodeImageSizePercent;

            // Set the Art QR code background opacity value on the slider and label
            sldArtQRCodeOpacityBg.Value = int.Parse(Globals.cArtCodeOpacityBg.AsSpan(0, 2), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture);
            lblArtQRCodeOpacityBg.Text = $"{CodeLang.BackgroundOpacityArt_Text} {Globals.cArtCodeOpacityBg}";

            // Set the theme in the picker
            List<string> ThemeList =
            [
                CodeLang.ThemeSystem_Text,
                CodeLang.ThemeLight_Text,
                CodeLang.ThemeDark_Text
            ];
            pckTheme.ItemsSource = ThemeList;
        }

        /// <summary>
        /// Picker speech language clicked event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPickerLanguageSpeechChanged(object sender, EventArgs e)
        {
            ClassSpeech.PickerLanguageSpeechChanged(sender, e);
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
                    _ => "System"   // System
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
                ClassBarcodes.nBarcodeGeneratorIndex = selectedIndex;
                ClassBarcodes.cBarcodeGeneratorName = pckFormatCodeGenerator.Items[ClassBarcodes.nBarcodeGeneratorIndex];
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
                ClassBarcodes.nBarcodeScannerIndex = selectedIndex;
                ClassBarcodes.cBarcodeScannerName = pckFormatCodeScanner.Items[ClassBarcodes.nBarcodeScannerIndex];
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
            await DisplayAlertAsync("?", $"{CodeLang.HexColorCodes_Text}\n\n{CodeLang.AllowedChar_Text}\n{cHexCharacters}", CodeLang.ButtonClose_Text);
        }

        /// <summary>
        /// Go to the next field when the next/done key have been pressed 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        private async void GoToNextField(object sender, EventArgs e)
        {
            // Go to the next field when the next/done key have been pressed on the soft input keyboard
            if (sender == entHexColorFg)
            {
                _ = entHexColorBg.Focus();
            }
            else if (sender == entHexColorBg)
            {
                entHexColorBg.Unfocus();
            }
        }

        /// <summary>
        /// Entry HexColor Unfocused event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EntryHexColorUnfocused(object sender, EventArgs e)
        {
            Entry entry = (Entry)sender;

#if ANDROID
            // Android !!!BUG!!! SafeAreaEdges not behaving as expected #33922 - https://github.com/dotnet/maui/issues/33922
            // A padding with a height of 50 at the bottom is added to the grid to workaround this issue and push the content
            // above the navigation bar on Android when the entry is unfocused and the soft input keyboard is hidden.
            // Happens when the 'Entry ReturnType' is set to 'Done' and the soft keyboard is hiding after pressing 'Done'.
            // Workaround: use always 'Next' and handle the focus in the GoToNextField method to go to the next field or unfocus the last field.
            // Happens most with the Microsoft SwiftKey keyboard, the Samsung and Google keyboards have it less or not at all.
            //if (!hasWorkaroundRow)
            //{
            //    grdMainFixed.Padding = new Thickness(0, 0, 0, 50);
            //    hasWorkaroundRow = true;
            //}
#endif

#if IOS
            // https://github.com/dotnet/maui/issues/33316 and https://github.com/dotnet/maui/issues/32016
            // Workaround for iOS !!!BUG!!! The MaxLength property of an Entry control is not respected on iOS,
            // so we have to set the text again to get the correct length of the text (Microsoft.Maui.Controls Version 10.0.41)
            // Re-assign text to enforce MaxLength on iOS
            string cTemp = entry.Text;
            entry.Text = string.Empty;
            entry.Text = cTemp;
#endif
            // Add the opacity if length = 6 characters
            if (entry.Text?.Length == 6)
            {
                entry.Text = $"FF{entry.Text}";
            }

            // Length must be 8 characters
            if (entry.Text == null || entry.Text.Length != 8)
            {
                _ = entry.Focus();
                return;
            }

            // Set the sliders position based on the entry content
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

            // The X2 format specifier formats the number as a hexadecimal value with a minimum width of 2 digits
            string cColorFgHex = $"{nAmountOpacity:X2}{nColorRed:X2}{nColorGreen:X2}{nColorBlue:X2}";
            entHexColorFg.Text = cColorFgHex;
            bxvColorFg.Color = Color.FromArgb(cColorFgHex);

            Globals.cCodeColorFg = cColorFgHex;
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

            nOpacity = int.Parse(cHexColor.AsSpan(0, 2), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture);
            nRed = int.Parse(cHexColor.AsSpan(2, 2), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture);
            nGreen = int.Parse(cHexColor.AsSpan(4, 2), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture);
            nBlue = int.Parse(cHexColor.AsSpan(6, 2), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Switch QR code image size variable toggled event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SwtQRCodeSizeVariable_Toggled(object sender, ToggledEventArgs e)
        {
            ClassQRCodeImage.bQRCodeSizeVariable = e.Value;
        }

        /// <summary>
        /// Switch barcode with caption toggled event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SwtBarcodeWithCaption_Toggled(object sender, ToggledEventArgs e)
        {
            Globals.bBarcodeWithCaption = e.Value;
        }

        /// <summary>
        /// Handles the TextChanged event for the QR code image size entry, ensuring that only valid decimal values are
        /// accepted.
        /// </summary>
        /// <remarks>If the new text value is not a valid decimal, the text is reverted to the previous
        /// value to prevent invalid input.</remarks>
        /// <param name="sender">The source of the event, typically the Entry control that triggered the TextChanged event.</param>
        /// <param name="e">The event data containing information about the text change, including the new and old text values.</param>
        private void EntQRCodeSizePixels_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!IsDecimal(e.NewTextValue))
            {
                ((Entry)sender).Text = e.OldTextValue;
            }
        }

        /// <summary>
        /// Determines whether the specified string consists exclusively of valid decimal characters.
        /// </summary>
        /// <remarks>This method checks each character in the input string against a predefined set of
        /// valid decimal characters. It is case-sensitive and returns false if any character is not recognized as a
        /// valid decimal character.</remarks>
        /// <param name="cText">The string to evaluate for valid decimal characters. This parameter cannot be null or empty.</param>
        /// <returns>true if all characters in the string are valid decimal characters; otherwise, false.</returns>
        private static bool IsDecimal(string cText)
        {
            foreach (char c in cText)
            {
                if (!cDecimalCharacters.Contains(c))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EntQRCodeSizePixels_Unfocused(object sender, FocusEventArgs e)
        {
            string cText = ((Entry)sender).Text;

            if (!int.TryParse(cText, out int nValue))
            {
                entQRCodeSizePixels.Focus();
                return;
            }

            if (nValue < 500 || nValue > 10000)
            {
                entQRCodeSizePixels.Focus();
                return;
            }

            ClassQRCodeImage.nQRCodeSizePixels = nValue;
        }

        /// <summary>
        /// Handles the ValueChanged event for the QR code image size slider, updating the QR code image size percentage
        /// and pixel dimensions based on the new slider value.
        /// </summary>
        /// <remarks>This method recalculates the QR code image size in pixels and updates the associated
        /// label to reflect the current size selection. It should be connected to the slider's ValueChanged event to
        /// ensure the UI remains in sync with user input.</remarks>
        /// <param name="sender">The source of the event, typically the slider control whose value has changed.</param>
        /// <param name="e">An object that contains the event data, including the new value of the slider representing the desired QR
        /// code image size percentage.</param>
        private void OnSliderQRCodeImageSizeValueChanged(object sender, ValueChangedEventArgs e)
        {
            ClassQRCodeImage.nQRCodeImageSizePercent = MathF.Round((float)e.NewValue, 1);
            sldQRCodeImageSize.Value = ClassQRCodeImage.nQRCodeImageSizePercent;
            lblQRCodeImageSize.Text = $"{string.Format(CodeLang.QRCodeImageSize_Text, ClassQRCodeImage.nQRCodeImageSizePercent)}";
        }

        /// <summary>
        /// Handles the ValueChanged event for the QR code background opacity slider, updating the background color's opacity
        /// </summary>
        /// <param name="sender">The source of the event, typically the slider control whose value has changed.</param>
        /// <param name="e">An object that contains the event data, including the new value of the slider representing the desired QR
        /// code background opacity.</param>
        private void SldArtQRCodeOpacityBg_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            int nAmountOpacity = (int)e.NewValue;
            Globals.cArtCodeOpacityBg = $"{nAmountOpacity:X2}";

            lblArtQRCodeOpacityBg.Text = $"{CodeLang.BackgroundOpacityArt_Text} {Globals.cArtCodeOpacityBg}";
        }

        /// <summary>
        /// Button save settings clicked event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnSettingsSaveClicked(object sender, EventArgs e)
        {
            Preferences.Default.Set("SettingBarcodeGeneratorName", ClassBarcodes.cBarcodeGeneratorName);
            Preferences.Default.Set("SettingBarcodeScannerName", ClassBarcodes.cBarcodeScannerName);
            Preferences.Default.Set("SettingQRCodeSizeVariable", ClassQRCodeImage.bQRCodeSizeVariable);
            Preferences.Default.Set("SettingQRCodeSizePixels", ClassQRCodeImage.nQRCodeSizePixels);
            Preferences.Default.Set("SettingQRCodeImageSizePercent", ClassQRCodeImage.nQRCodeImageSizePercent);
            Preferences.Default.Set("SettingBarcodeWithCaption", Globals.bBarcodeWithCaption);
            Preferences.Default.Set("SettingTheme", Globals.cTheme);
            Preferences.Default.Set("SettingCodeColorFg", Globals.cCodeColorFg);
            Preferences.Default.Set("SettingCodeColorBg", Globals.cCodeColorBg);
            Preferences.Default.Set("SettingArtCodeOpacityBg", Globals.cArtCodeOpacityBg);
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
                Preferences.Default.Remove("SettingBarcodeGeneratorName");
                Preferences.Default.Remove("SettingBarcodeScannerName");
                Preferences.Default.Remove("SettingQRCodeSizeVariable");
                Preferences.Default.Remove("SettingQRCodeSizePixels");
                Preferences.Default.Remove("SettingQRCodeImageSizePercent");
                Preferences.Default.Remove("SettingBarcodeWithCaption");
                Preferences.Default.Remove("SettingTheme");
                Preferences.Default.Remove("SettingCodeColorFg");
                Preferences.Default.Remove("SettingCodeColorBg");
                Preferences.Default.Remove("SettingArtCodeOpacityBg");
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

        private async void OnButtonColorForgroundClicked(object sender, EventArgs e)
        {
            Globals.cCodeColor = Globals.cCodeColorFg;
            await OpenPopupColorPickerAsync();
            Globals.cCodeColorFg = Globals.cCodeColor;
            entHexColorFg.Text = Globals.cCodeColorFg;
            bxvColorFg.Color = Color.FromArgb(Globals.cCodeColorFg);
        }

        /// <summary>
        /// Show a modal popup to inform the user about the recommended image size before opening the file picker
        /// </summary>
        private async Task OpenPopupColorPickerAsync()
        {
            Page? currentPage = Application.Current?.Windows.Count > 0 ? Application.Current.Windows[0]?.Page : null;
            if (currentPage != null)
            {
                Globals.bIsPopupMessage = true;
                _ = await currentPage.ShowPopupAsync(new PopupColorPicker(60, $"{CodeLang.QRCodeRecommendedImageSize_Text}:\n\n{ClassQRCodeImage.nQRCodeSizePixels:N0} x {ClassQRCodeImage.nQRCodeSizePixels:N0} {CodeLang.Pixels_Text}"));

                // Check if the popup was canceled by the user before proceeding to open the file picker
                if (Globals.bPopupCanceled)
                {
                    Globals.bPopupCanceled = false;
                }
            }
        }
    }
}
