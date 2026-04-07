using CommunityToolkit.Maui.Extensions;

namespace BarcodeGenerator
{
    public sealed partial class PageSettings : ContentPage
    {
        // Local variables
        private const string cDecimalCharacters = "0123456789";
        private readonly Stopwatch stopWatch = new();
        private string searchKeyGenerator = string.Empty;
        private string searchKeyScanner = string.Empty;
        private readonly string cGradientDirectionBackgroundColor = "000099";  // navy blue

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
            //entHexColorBg.ReturnType = ReturnType.Next;
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

            // Select the current QR Code module shape in the radio buttons
            rbtQRCodeModuleShapeSquare.IsChecked = ClassBarcodes.cQRCodeModuleShape == "Square";
            rbtQRCodeModuleShapeRounded.IsChecked = ClassBarcodes.cQRCodeModuleShape == "Rounded";
            rbtQRCodeModuleShapeCircle.IsChecked = ClassBarcodes.cQRCodeModuleShape == "Circle";

            // Set the QR code image size to update the switch and entry
            swtQRCodeSizeVariable.IsToggled = ClassBarcodes.bQRCodeSizeVariable;
            entQRCodeSizePixels.Text = ClassBarcodes.nQRCodeSizePixels.ToString();

            // Set the QR code gradient variable to update the switch
            swtQRCodeGradient.IsToggled = ClassBarcodes.bQRCodeGradientColor;

            // Set the current color in the box view
            bxvColorFg.Color = Color.FromArgb(Globals.cCodeColorFg);
            bxvColorBg.Color = Color.FromArgb(Globals.cCodeColorBg);
            bxvColorBgArtQRCode.Color = Color.FromArgb(Globals.cCodeColorBgArtQRCode);
            bxvGradientColor1.Color = Color.FromArgb(ClassBarcodes.cQRCodeGradientColor1);
            bxvGradientColor2.Color = Color.FromArgb(ClassBarcodes.cQRCodeGradientColor2);
            bxvGradientColor3.Color = Color.FromArgb(ClassBarcodes.cQRCodeGradientColor3);

            // Set the QR code gradient direction to update the button background color
            switch (ClassBarcodes.cQRCodeGradientDirection)
            {
                case "BottomToTop":
                    btnQRCodeGradientDirection1.BackgroundColor = Color.FromArgb(cGradientDirectionBackgroundColor);
                    break;
                case "BottomLeftToTopRight":
                    btnQRCodeGradientDirection2.BackgroundColor = Color.FromArgb(cGradientDirectionBackgroundColor);
                    break;
                case "LeftToRight":
                    btnQRCodeGradientDirection3.BackgroundColor = Color.FromArgb(cGradientDirectionBackgroundColor);
                    break;
                case "TopLeftToBottomRight":
                    btnQRCodeGradientDirection4.BackgroundColor = Color.FromArgb(cGradientDirectionBackgroundColor);
                    break;
                case "TopToBottom":
                    btnQRCodeGradientDirection5.BackgroundColor = Color.FromArgb(cGradientDirectionBackgroundColor);
                    break;
                case "TopRightToBottomLeft":
                    btnQRCodeGradientDirection6.BackgroundColor = Color.FromArgb(cGradientDirectionBackgroundColor);
                    break;
                case "RightToLeft":
                    btnQRCodeGradientDirection7.BackgroundColor = Color.FromArgb(cGradientDirectionBackgroundColor);
                    break;
                case "BottomRightToTopLeft":
                    btnQRCodeGradientDirection8.BackgroundColor = Color.FromArgb(cGradientDirectionBackgroundColor);
                    break;
            }

            // Set the barcode with caption variable to update the switch
            swtBarcodeWithCaption.IsToggled = ClassBarcodes.bBarcodeWithCaption;

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
            lblQRCodeImageSize.Text = $"{string.Format(CodeLang.QRCodeImageSize_Text, ClassBarcodes.nQRCodeImageSizePercent)}";
            sldQRCodeImageSize.Value = ClassBarcodes.nQRCodeImageSizePercent;

            // Set the theme in the picker
            List<string> ThemeList =
            [
                CodeLang.ThemeSystem_Text,
                CodeLang.ThemeLight_Text,
                CodeLang.ThemeDark_Text
            ];
            pckTheme.ItemsSource = ThemeList;

            // Set the QR code module shapes in the picker
            List<string> QRCodeModuleShape =
            [
                CodeLang.QRCodeModuleShapeCircle_Text,
                CodeLang.QRCodeModuleShapeRounded_Text,
                CodeLang.QRCodeModuleShapeSquare_Text
            ];
            //pckQRCodeModuleShape.ItemsSource = QRCodeModuleShape;
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
        /// Switch QR code image size variable toggled event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SwtQRCodeSizeVariable_Toggled(object sender, ToggledEventArgs e)
        {
            ClassBarcodes.bQRCodeSizeVariable = e.Value;
        }

        /// <summary>
        /// Switch QR code gradient toggled event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SwtQRCodeGradient_Toggled(object sender, ToggledEventArgs e)
        {
            ClassBarcodes.bQRCodeGradientColor = e.Value;
        }

        /// <summary>
        /// Switch barcode with caption toggled event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SwtBarcodeWithCaption_Toggled(object sender, ToggledEventArgs e)
        {
            ClassBarcodes.bBarcodeWithCaption = e.Value;
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
        /// Handles the Unfocused event for the QR code image size entry, ensuring that the entered value is within the valid range.
        /// </summary>
        /// <param name="sender">The source of the event, typically the Entry control that triggered the Unfocused event.</param>
        /// <param name="e">The event data containing information about the focus change.</param>
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

            ClassBarcodes.nQRCodeSizePixels = nValue;
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
            ClassBarcodes.nQRCodeImageSizePercent = MathF.Round((float)e.NewValue, 1);
            sldQRCodeImageSize.Value = ClassBarcodes.nQRCodeImageSizePercent;
            lblQRCodeImageSize.Text = $"{string.Format(CodeLang.QRCodeImageSize_Text, ClassBarcodes.nQRCodeImageSizePercent)}";
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
            Preferences.Default.Set("SettingQRCodeSizeVariable", ClassBarcodes.bQRCodeSizeVariable);
            Preferences.Default.Set("SettingQRCodeSizePixels", ClassBarcodes.nQRCodeSizePixels);
            Preferences.Default.Set("SettingQRCodeImageSizePercent", ClassBarcodes.nQRCodeImageSizePercent);
            Preferences.Default.Set("SettingQRCodeModuleShape", ClassBarcodes.cQRCodeModuleShape);
            Preferences.Default.Set("SettingQRCodeGradientColor", ClassBarcodes.bQRCodeGradientColor);
            Preferences.Default.Set("SettingQRCodeGradientColor1", ClassBarcodes.cQRCodeGradientColor1);
            Preferences.Default.Set("SettingQRCodeGradientColor2", ClassBarcodes.cQRCodeGradientColor2);
            Preferences.Default.Set("SettingQRCodeGradientColor3", ClassBarcodes.cQRCodeGradientColor3);
            Preferences.Default.Set("SettingQRCodeGradientDirection", ClassBarcodes.cQRCodeGradientDirection);
            Preferences.Default.Set("SettingBarcodeWithCaption", ClassBarcodes.bBarcodeWithCaption);
            Preferences.Default.Set("SettingTheme", Globals.cTheme);
            Preferences.Default.Set("SettingCodeColorFg", Globals.cCodeColorFg);
            Preferences.Default.Set("SettingCodeColorBg", Globals.cCodeColorBg);
            Preferences.Default.Set("SettingCodeColorBgArtQRCode", Globals.cCodeColorBgArtQRCode);
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
                Preferences.Default.Remove("SettingQRCodeModuleShape");
                Preferences.Default.Remove("SettingQRCodeGradientColor");
                Preferences.Default.Remove("SettingQRCodeGradientColor1");
                Preferences.Default.Remove("SettingQRCodeGradientColor2");
                Preferences.Default.Remove("SettingQRCodeGradientColor3");
                Preferences.Default.Remove("SettingQRCodeGradientDirection");
                Preferences.Default.Remove("SettingBarcodeWithCaption");
                Preferences.Default.Remove("SettingTheme");
                Preferences.Default.Remove("SettingCodeColorFg");
                Preferences.Default.Remove("SettingCodeColorBg");
                Preferences.Default.Remove("SettingCodeColorBgArtQRCode");
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

        /// <summary>
        /// On button color forground clicked event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnButtonColorForgroundClicked(object sender, EventArgs e)
        {
            Globals.cCodeColor = Globals.cCodeColorFg;
            await OpenPopupColorPickerAsync(CodeLang.ForegroundColor_Text);

            if (!Globals.bPopupCanceled)
            {
                Globals.cCodeColorFg = Globals.cCodeColor;
                bxvColorFg.Color = Color.FromArgb(Globals.cCodeColorFg);
            }
        }

        /// <summary>
        /// On button color background clicked event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnButtonColorBackgroundClicked(object sender, EventArgs e)
        {
            Globals.cCodeColor = Globals.cCodeColorBg;
            await OpenPopupColorPickerAsync(CodeLang.BackgroundColor_Text);

            if (!Globals.bPopupCanceled)
            {
                Globals.cCodeColorBg = Globals.cCodeColor;
                bxvColorBg.Color = Color.FromArgb(Globals.cCodeColorBg);
            }
        }

        /// <summary>
        /// On button color background Art QR code clicked event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnButtonColorBackgroundArtQRCodeClicked(object sender, EventArgs e)
        {
            Globals.cCodeColor = Globals.cCodeColorBgArtQRCode;
            await OpenPopupColorPickerAsync(CodeLang.BackgroundColorArtQRCode_Text);

            if (!Globals.bPopupCanceled)
            {
                Globals.cCodeColorBgArtQRCode = Globals.cCodeColor;
                bxvColorBgArtQRCode.Color = Color.FromArgb(Globals.cCodeColorBgArtQRCode);
            }
        }

        /// <summary>
        ///  On button color gradient 1 clicked event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnButtonGradientColor1Clicked(object sender, EventArgs e)
        {
            Globals.cCodeColor = ClassBarcodes.cQRCodeGradientColor1;
            await OpenPopupColorPickerAsync(CodeLang.QRCodeGradientColor1_Text);
    
            if (!Globals.bPopupCanceled)
            {
                ClassBarcodes.cQRCodeGradientColor1 = Globals.cCodeColor;
                bxvGradientColor1.Color = Color.FromArgb(ClassBarcodes.cQRCodeGradientColor1);
            }
        }

        /// <summary>
        /// On button color gradient 2 clicked event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnButtonGradientColor2Clicked(object sender, EventArgs e)
        {
            Globals.cCodeColor = ClassBarcodes.cQRCodeGradientColor2;
            await OpenPopupColorPickerAsync(CodeLang.QRCodeGradientColor2_Text);

            if (!Globals.bPopupCanceled)
            {
                ClassBarcodes.cQRCodeGradientColor2 = Globals.cCodeColor;
                bxvGradientColor2.Color = Color.FromArgb(ClassBarcodes.cQRCodeGradientColor2);
            }
        }

        /// <summary>
        /// On button color gradient 3 clicked event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnButtonGradientColor3Clicked(object sender, EventArgs e)
        {
            Globals.cCodeColor = ClassBarcodes.cQRCodeGradientColor3;
            await OpenPopupColorPickerAsync(CodeLang.QRCodeGradientColor3_Text);

            if (!Globals.bPopupCanceled)
            {
                ClassBarcodes.cQRCodeGradientColor3 = Globals.cCodeColor;
                bxvGradientColor3.Color = Color.FromArgb(ClassBarcodes.cQRCodeGradientColor3);
            }
        }

        /// <summary>
        /// On button QR code gradient direction clicked event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnButtonQRCodeGradientDirectionClicked(object sender, EventArgs e)
        {
            btnQRCodeGradientDirection1.BackgroundColor = Colors.Transparent;
            btnQRCodeGradientDirection2.BackgroundColor = Colors.Transparent;
            btnQRCodeGradientDirection3.BackgroundColor = Colors.Transparent;
            btnQRCodeGradientDirection4.BackgroundColor = Colors.Transparent;
            btnQRCodeGradientDirection5.BackgroundColor = Colors.Transparent;
            btnQRCodeGradientDirection6.BackgroundColor = Colors.Transparent;
            btnQRCodeGradientDirection7.BackgroundColor = Colors.Transparent;
            btnQRCodeGradientDirection8.BackgroundColor = Colors.Transparent;

            if (sender == btnQRCodeGradientDirection1)
            {
                ClassBarcodes.cQRCodeGradientDirection = "BottomToTop";
                btnQRCodeGradientDirection1.BackgroundColor = Color.FromArgb(cGradientDirectionBackgroundColor);
            }
            else if (sender == btnQRCodeGradientDirection2)
            {
                ClassBarcodes.cQRCodeGradientDirection = "BottomLeftToTopRight";
                btnQRCodeGradientDirection2.BackgroundColor = Color.FromArgb(cGradientDirectionBackgroundColor);
            }
            else if (sender == btnQRCodeGradientDirection3)
            {
                ClassBarcodes.cQRCodeGradientDirection = "LeftToRight";
                btnQRCodeGradientDirection3.BackgroundColor = Color.FromArgb(cGradientDirectionBackgroundColor);
            }
            else if (sender == btnQRCodeGradientDirection4)
            {
                ClassBarcodes.cQRCodeGradientDirection = "TopLeftToBottomRight";
                btnQRCodeGradientDirection4.BackgroundColor = Color.FromArgb(cGradientDirectionBackgroundColor);
            }
            else if (sender == btnQRCodeGradientDirection5)
            {
                ClassBarcodes.cQRCodeGradientDirection = "TopToBottom";
                btnQRCodeGradientDirection5.BackgroundColor = Color.FromArgb(cGradientDirectionBackgroundColor);
            }
            else if (sender == btnQRCodeGradientDirection6)
            {
                ClassBarcodes.cQRCodeGradientDirection = "TopRightToBottomLeft";
                btnQRCodeGradientDirection6.BackgroundColor = Color.FromArgb(cGradientDirectionBackgroundColor);
            }
            else if (sender == btnQRCodeGradientDirection7)
            {
                ClassBarcodes.cQRCodeGradientDirection = "RightToLeft";
                btnQRCodeGradientDirection7.BackgroundColor = Color.FromArgb(cGradientDirectionBackgroundColor);
            }
            else if (sender == btnQRCodeGradientDirection8)
            {
                ClassBarcodes.cQRCodeGradientDirection = "BottomRightToTopLeft";
                btnQRCodeGradientDirection8.BackgroundColor = Color.FromArgb(cGradientDirectionBackgroundColor);
            }
        }

        /// <summary>
        /// Radio button QR code module shape checked changed event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RbtQRCodeModuleShape_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (rbtQRCodeModuleShapeSquare.IsChecked)
            {
                ClassBarcodes.cQRCodeModuleShape = "Square";
            }
            else if (rbtQRCodeModuleShapeRounded.IsChecked)
            {
                ClassBarcodes.cQRCodeModuleShape = "Rounded";
            }
            else if (rbtQRCodeModuleShapeCircle.IsChecked)
            {
                ClassBarcodes.cQRCodeModuleShape = "Circle";
            }
        }

        /// <summary>
        /// Show a modal popup to inform the user about the recommended image size before opening the file picker
        /// </summary>
        private async Task OpenPopupColorPickerAsync(string cMessage)
        {
            // If the last character of the message is a : then remove it
            if (cMessage.EndsWith(':'))
            {
                cMessage = cMessage[..^1];
            }

            Page? currentPage = Application.Current?.Windows.Count > 0 ? Application.Current.Windows[0]?.Page : null;
            if (currentPage != null)
            {
                Globals.bIsPopupMessage = true;
                _ = await currentPage.ShowPopupAsync(new PopupColorPicker(cMessage));
            }
        }
    }
}
