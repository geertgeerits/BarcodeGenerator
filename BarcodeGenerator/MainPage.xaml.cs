/* Program .....: BarcodeGenerator.sln
 * Author ......: Geert Geerits - E-mail: geertgeerits@gmail.com
 * Copyright ...: (C) 2022-2026
 * Version .....: 1.0.45
 * Date ........: 2026-01-07 (YYYY-MM-DD)
 * Language ....: Microsoft Visual Studio 2026: .NET 10.0 MAUI C# 14.0
 * Description .: Barcode Generator: ZXing - Barcode Scanner: Native Android and iOS
 * Note ........: Only portrait mode is supported for iOS (!!!BUG!!! problems with the editor in iOS when turning from landscape to portrait)
 *                zxing:CameraBarcodeReaderView -> ex. WidthRequest="300" -> Grid RowDefinitions="400" (300 x 1.3333) = 3:4 aspect ratio
 *                Google Vision: https://developers.google.com/android/reference/com/google/android/gms/vision/CameraSource.Builder
 *                Google ML Kit: https://developers.google.com/ml-kit
 * Dependencies : NuGet Package: ZXing.Net.Maui by Redth version 0.7.3 ; https://github.com/redth/ZXing.Net.Maui
 *                NuGet Package: ZXing.Net.Maui.Controls by Redth version 0.7.3
 *                NuGet Package: BarcodeScanner.Native.Maui by Alen Friščić version 3.0.0-beta4 for Android & iOS; https://github.com/afriscic/BarcodeScanning.Native.Maui
 *                NuGet Package: Sentry.Maui version 6.0.0 ; https://sentry.io ; https://geerits.sentry.io/issues/ ; https://www.youtube.com/watch?v=9-50zH8fqYA
 * Thanks to ...: Gerald Versluis, Alen Friščić, Redth, Jimmy Pun */

using ZXing.Net.Maui;

namespace BarcodeGenerator
{
    public sealed partial class MainPage : ContentPage
    {
        //// Local variables
        private string cLicense = "";

        public MainPage()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
#if DEBUG
                DisplayAlertAsync("InitializeComponent: MainPage", ex.Message, "OK");
#endif
                return;
            }

            //// Disable Sentry for testing - https://github.com/getsentry/sentry-dotnet/discussions/3325
            //SentrySdk.Close();
#if IOS
            //// AutoSize has to be disabled for iOS
            edtTextToCode.AutoSize = EditorAutoSizeOption.Disabled;
#endif
            //// Get the saved settings
            Globals.cTheme = Preferences.Default.Get("SettingTheme", "System");
            Globals.nFormatGeneratorIndex = Preferences.Default.Get("SettingFormatGeneratorIndex", 12);
            Globals.nFormatScannerIndex = Preferences.Default.Get("SettingFormatScannerIndex", 0);
            Globals.cCodeColorFg = Preferences.Default.Get("SettingCodeColorFg", "FF000000");
            Globals.cCodeColorBg = Preferences.Default.Get("SettingCodeColorBg", "FFFFFFFF");
            Globals.cLanguage = Preferences.Default.Get("SettingLanguage", "");
            Globals.cLanguageSpeech = Preferences.Default.Get("SettingLanguageSpeech", "");
            Globals.bLicense = Preferences.Default.Get("SettingLicense", false);

            //// The height of the title bar is lower when an iPhone is in horizontal position
            if (DeviceInfo.Platform == DevicePlatform.iOS && DeviceInfo.Idiom == DeviceIdiom.Phone)
            {
                imgbtnAbout.VerticalOptions = LayoutOptions.Start;
                lblTitle.VerticalOptions = LayoutOptions.Start;
                lblTitle.VerticalTextAlignment = TextAlignment.Start;
                imgbtnScanNT.VerticalOptions = LayoutOptions.Start;
                imgbtnSettings.VerticalOptions = LayoutOptions.Start;
            }

            //// Set the tooltips for the scanner buttons
            ToolTipProperties.SetText(imgbtnScanNT, CodeLang.ToolTipBarcodeScanner_Text + " (Native)");

            //// Set the theme
            Globals.SetTheme();

            //// Set the barcode list and the current default barcode format in the picker for the barcode generator
            pckFormatCodeGenerator.ItemsSource = Globals.GetFormatCodeListGenerator();

            if (Globals.nFormatGeneratorIndex < 0 || Globals.nFormatGeneratorIndex > 14)
            {
                // Default format code = QrCode
                pckFormatCodeGenerator.SelectedIndex = 12;
            }
            else
            {
                // Set the format barcode to the saved code
                pckFormatCodeGenerator.SelectedIndex = Globals.nFormatGeneratorIndex;
            }

            //// Get and set the user interface language after a first start or reset of the application
            try
            {
                if (string.IsNullOrEmpty(Globals.cLanguage))
                {
                    Globals.cLanguage = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName;

                    // Chinese needs the language code as zh-CN and zh-TW
                    if (Globals.cLanguage == "zh")
                    {
                        Globals.cLanguage = Thread.CurrentThread.CurrentUICulture.Name;
                    }
                }
            }
            catch (Exception)
            {
                Globals.cLanguage = "en";
            }
            finally
            {
                // Save the UI language
                Preferences.Default.Set("SettingLanguage", Globals.cLanguage);
                Debug.WriteLine("MainPage - Globals.cLanguage: " + Globals.cLanguage);
            }

            //// Set the text language
            SetTextLanguage();

            //// Initialize text to speech and get and set the speech language
            InitializeTextToSpeechAsync();

            //// Clear the clipboard
            //Clipboard.Default.SetTextAsync(null);  // For testing

            //// Set focus to the editor
            edtTextToCode.Focus();

            //// Test for crashes Sentry
            //SentrySdk.CaptureMessage("Hello Sentry");
            //throw new Exception("This is a test exception");
        }

        /// <summary>
        /// Initialize text to speech and get and set the speech language
        /// Must be called in the constructor of the MainPage and not in the ClassSpeech.cs
        /// The InitializeTextToSpeechAsync method is called asynchronously after the UI components are initialized
        /// Once the asynchronous operation completes, the Globals.bTextToSpeechAvailable value is checked, and the UI is updated accordingly
        /// </summary>
        private async void InitializeTextToSpeechAsync()
        {
            try
            {
                if (string.IsNullOrEmpty(Globals.cLanguageSpeech))
                {
                    Globals.cLanguageSpeech = Thread.CurrentThread.CurrentUICulture.Name;
                }
            }
            catch (Exception)
            {
                Globals.cLanguageSpeech = "en-US";
            }

            // Initialize text to speech
            Globals.bTextToSpeechAvailable = await ClassSpeech.InitializeTextToSpeechAsync();

            if (Globals.bTextToSpeechAvailable)
            {
                lblTextToSpeech.IsVisible = true;
                imgbtnTextToSpeech.IsVisible = true;
                lblTextToSpeech.Text = Globals.GetIsoLanguageCode();

                // Search the selected language in the cLanguageLocales array
                ClassSpeech.SearchArrayWithSpeechLanguages(Globals.cLanguageSpeech);

                // Save the speech language
                Preferences.Default.Set("SettingLanguageSpeech", Globals.cLanguageSpeech);
            }
            
            Debug.WriteLine("MainPage - Globals.bTextToSpeechAvailable: " + Globals.bTextToSpeechAvailable);
            Debug.WriteLine("MainPage - Globals.cLanguageSpeech: " + Globals.cLanguageSpeech);
        }

        //// TitleView buttons clicked events
        private async void OnPageAboutClicked(object sender, EventArgs e)
        {
            imgbtnTextToSpeech.Source = ClassSpeech.CancelTextToSpeech();
            await Navigation.PushAsync(new PageAbout());
        }

        private async void OnPageScanClickedNT(object sender, EventArgs e)
        {
            imgbtnTextToSpeech.Source = ClassSpeech.CancelTextToSpeech();
            await Navigation.PushAsync(new PageScanNT());
        }

        private async void OnPageSettingsClicked(object sender, EventArgs e)
        {
            imgbtnTextToSpeech.Source = ClassSpeech.CancelTextToSpeech();
            await Navigation.PushAsync(new PageSettings());
        }

        /// <summary>
        /// Set the editor properties for the selected format code
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPickerFormatCodeChanged(object sender, EventArgs e)
        {
            var picker = (Picker)sender;
            int selectedIndex = picker.SelectedIndex;

            if (selectedIndex != -1)
            {
                bgvBarcode.Value = "";
                bgvBarcode.HeightRequest = 160;
                bgvBarcode.WidthRequest = -1;
                bgvBarcode.HorizontalOptions = LayoutOptions.Fill;

                btnShare.Text = CodeLang.ButtonShare_Text;
                btnShare.IsEnabled = false;

                switch (selectedIndex)
                {
                    // Aztec
                    case 0:
                        edtTextToCode.MaxLength = 1900;
                        edtTextToCode.Keyboard = Keyboard.Default;
                        bgvBarcode.HeightRequest = 250;
                        bgvBarcode.WidthRequest = 250;
                        bgvBarcode.BarcodeMargin = 2;
                        bgvBarcode.Format = BarcodeFormat.Aztec;
                        break;

                    // Codabar
                    case 1:
                        edtTextToCode.MaxLength = 43;
                        edtTextToCode.Keyboard = Keyboard.Default;
                        bgvBarcode.BarcodeMargin = 4;
                        bgvBarcode.Format = BarcodeFormat.Codabar;
                        break;

                    // Code39
                    case 2:
                        edtTextToCode.MaxLength = 48;
                        edtTextToCode.Keyboard = Keyboard.Default;
                        bgvBarcode.BarcodeMargin = 4;
                        bgvBarcode.Format = BarcodeFormat.Code39;
                        break;

                    // Code93
                    case 3:
                        edtTextToCode.MaxLength = 48;
                        edtTextToCode.Keyboard = Keyboard.Default;
                        bgvBarcode.BarcodeMargin = 4;
                        bgvBarcode.Format = BarcodeFormat.Code93;
                        break;

                    // Code128
                    case 4:
                        edtTextToCode.MaxLength = 48;
                        edtTextToCode.Keyboard = Keyboard.Default;
                        bgvBarcode.BarcodeMargin = 20;
                        bgvBarcode.Format = BarcodeFormat.Code128;
                        break;

                    // DataMatrix
                    case 5:
                        edtTextToCode.MaxLength = 1500;
                        edtTextToCode.Keyboard = Keyboard.Default;
                        bgvBarcode.HeightRequest = 250;
                        bgvBarcode.WidthRequest = 250;
                        bgvBarcode.BarcodeMargin = 2;
                        bgvBarcode.Format = BarcodeFormat.DataMatrix;
                        break;

                    // Ean8
                    case 6:
                        edtTextToCode.MaxLength = 8;
                        edtTextToCode.Keyboard = Keyboard.Numeric;
                        bgvBarcode.BarcodeMargin = 4;
                        bgvBarcode.Format = BarcodeFormat.Ean8;
                        break;

                    // Ean13
                    case 7:
                        edtTextToCode.MaxLength = 13;
                        edtTextToCode.Keyboard = Keyboard.Numeric;
                        bgvBarcode.BarcodeMargin = 4;
                        bgvBarcode.Format = BarcodeFormat.Ean13;
                        break;

                    // Itf
                    case 8:
                        edtTextToCode.MaxLength = 30;
                        edtTextToCode.Keyboard = Keyboard.Numeric;
                        bgvBarcode.BarcodeMargin = 8;
                        bgvBarcode.Format = BarcodeFormat.Itf;
                        break;

                    // Msi
                    case 9:
                        edtTextToCode.MaxLength = 255;
                        edtTextToCode.Keyboard = Keyboard.Numeric;
                        bgvBarcode.BarcodeMargin = 10;
                        bgvBarcode.Format = BarcodeFormat.Msi;
                        break;

                    // Pdf417
                    case 10:
                        edtTextToCode.MaxLength = 1100;
                        edtTextToCode.Keyboard = Keyboard.Default;
                        bgvBarcode.BarcodeMargin = 10;
                        bgvBarcode.Format = BarcodeFormat.Pdf417;
                        break;

                    // Plessey
                    case 11:
                        edtTextToCode.MaxLength = 16;
                        edtTextToCode.Keyboard = Keyboard.Default;
                        bgvBarcode.BarcodeMargin = 8;
                        bgvBarcode.Format = BarcodeFormat.Plessey;
                        break;

                    // QrCode
                    case 12:
                        edtTextToCode.MaxLength = 1800;
                        edtTextToCode.Keyboard = Keyboard.Default;
                        bgvBarcode.HeightRequest = 250;
                        bgvBarcode.WidthRequest = 250;
                        bgvBarcode.BarcodeMargin = 1;
                        bgvBarcode.Format = BarcodeFormat.QrCode;
                        break;

                    // UpcA
                    case 13:
                        edtTextToCode.MaxLength = 12;
                        edtTextToCode.Keyboard = Keyboard.Numeric;
                        bgvBarcode.BarcodeMargin = 0;
                        bgvBarcode.Format = BarcodeFormat.UpcA;
                        break;

                    // UpcE
                    case 14:
                        edtTextToCode.MaxLength = 8;
                        edtTextToCode.Keyboard = Keyboard.Numeric;
                        bgvBarcode.BarcodeMargin = 8;
                        bgvBarcode.Format = BarcodeFormat.UpcE;
                        break;
                }
            }
        }

        /// <summary>
        /// OnGenerateCodeClicked event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnGenerateCodeClicked(object sender, EventArgs e)
        {
            // Hide the keyboard
            edtTextToCode.IsEnabled = false;
            edtTextToCode.IsEnabled = true;

            // Set the barcode colors
            bgvBarcode.ForegroundColor = Color.FromArgb(Globals.cCodeColorFg);
            bgvBarcode.BackgroundColor = Color.FromArgb(Globals.cCodeColorBg);

            // Miscellaneous
            bgvBarcode.Value = "";
            string cChecksum = "";

            // Validate the input
            if (string.IsNullOrEmpty(edtTextToCode.Text))
            {
                _ = edtTextToCode.Focus();
                return;
            }

            string cTextToCode = edtTextToCode.Text.Trim();
            int nLenTextToCode = cTextToCode.Length;
        
            if (nLenTextToCode == 0)
            {
                return;
            }

            // Validate the text input and set the format
            int selectedIndex = pckFormatCodeGenerator.SelectedIndex;

            if (selectedIndex != -1)
            {
                try
                {
                    switch (selectedIndex)
                    {
                        // Aztec
                        case 0:
                            cTextToCode = ReplaceCharacters(cTextToCode);
                            edtTextToCode.Text = cTextToCode;

                            if (TestAllowedAsciiValues(1, 255, cTextToCode) == false)
                            {
                                return;
                            }

                            break;

                        // Codabar
                        case 1:
                            cTextToCode = cTextToCode.ToUpper();

                            if (TestAllowedCharacters("0123456789-$:/.+ABCD", cTextToCode) == false)
                            {
                                return;
                            }

                            if (TestStartEndGuards("ABCD", cTextToCode) == false)
                            {
                                return;
                            }

                            break;

                        // Code39
                        case 2:
                            cTextToCode = cTextToCode.ToUpper();

                            if (TestAllowedCharacters("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789 -.$/+%*", cTextToCode) == false)
                            {
                                return;
                            }

                            if (TestStartEndGuards("*", cTextToCode) == false)
                            {
                                return;
                            }

                            break;

                        // Code93
                        case 3:
                            cTextToCode = cTextToCode.ToUpper();

                            if (TestAllowedCharacters("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789 -.$/+%*", cTextToCode) == false)
                            {
                                return;
                            }

                            if (TestStartEndGuards("*", cTextToCode) == false)
                            {
                                return;
                            }

                            break;

                        // Code128
                        case 4:
                            cTextToCode = ReplaceCharacters(cTextToCode);
                            edtTextToCode.Text = cTextToCode;

                            if (TestAllowedAsciiValues(1, 127, cTextToCode) == false)
                            {
                                return;
                            }

                            break;

                        // DataMatrix
                        case 5:
                            cTextToCode = ReplaceCharacters(cTextToCode);
                            edtTextToCode.Text = cTextToCode;

                            if (TestAllowedAsciiValues(1, 255, cTextToCode) == false)
                            {
                                return;
                            }

                            break;

                        // Ean8
                        case 6:
                            if (TestAllowedCharacters("0123456789", cTextToCode) == false)
                            {
                                return;
                            }

                            if (nLenTextToCode < 7 || nLenTextToCode > 8)
                            {
                                DisplayErrorMessageLength("7", "8");
                                return;
                            }

                            // Calculate, (correct) and add the checksum
                            if (nLenTextToCode == 8)
                            {
                                cChecksum = cTextToCode.Substring(7, 1);
                            }

                            cTextToCode = cTextToCode[..7];
                            cTextToCode += CalculateChecksumEanUpcA(ReverseString(cTextToCode));

                            if (nLenTextToCode == 8 && cChecksum != cTextToCode.Substring(7, 1))
                            {
                                DisplayErrorMessage(CodeLang.CheckDigitError_Text);
                            }

                            edtTextToCode.Text = cTextToCode;
                            //cTextCode = string.Concat(cTextToCode[..4], " ", cTextToCode.Substring(4, 4));

                            break;

                        // Ean13
                        case 7:
                            if (TestAllowedCharacters("0123456789", cTextToCode) == false)
                            {
                                return;
                            }

                            if (nLenTextToCode < 12 || nLenTextToCode > 13)
                            {
                                DisplayErrorMessageLength("12", "13");
                                return;
                            }

                            // Calculate, (correct) and add the checksum
                            if (nLenTextToCode == 13)
                            {
                                cChecksum = cTextToCode.Substring(12, 1);
                            }

                            cTextToCode = cTextToCode[..12];
                            cTextToCode += CalculateChecksumEanUpcA(ReverseString(cTextToCode));

                            if (nLenTextToCode == 13 && cChecksum != cTextToCode.Substring(12, 1))
                            {
                                DisplayErrorMessage(CodeLang.CheckDigitError_Text);
                            }

                            edtTextToCode.Text = cTextToCode;
                            //cTextCode = string.Concat(cTextToCode[..1], " ", cTextToCode.Substring(1, 6), " ", cTextToCode.Substring(7, 6));

                            break;

                        // Itf
                        case 8:
                            if (TestAllowedCharacters("0123456789", cTextToCode) == false)
                            {
                                return;
                            }

                            if (nLenTextToCode % 2 != 0)
                            {
                                DisplayErrorMessage(CodeLang.LengthInputEven_Text);
                                return;
                            }

                            break;

                        // Msi
                        case 9:
                            if (TestAllowedCharacters("0123456789", cTextToCode) == false)
                            {
                                return;
                            }

                            break;

                        // Pdf417
                        case 10:
                            cTextToCode = ReplaceCharacters(cTextToCode);
                            edtTextToCode.Text = cTextToCode;

                            if (TestAllowedAsciiValues(1, 255, cTextToCode) == false)
                            {
                                return;
                            }

                            break;

                        // Plessey
                        case 11:
                            cTextToCode = cTextToCode.ToUpper();

                            if (TestAllowedCharacters("0123456789ABCDEF", cTextToCode) == false)
                            {
                                return;
                            }

                            break;

                        // QrCode
                        case 12:
                            break;

                        // UpcA
                        case 13:
                            if (TestAllowedCharacters("0123456789", cTextToCode) == false)
                            {
                                return;
                            }

                            if (nLenTextToCode < 11 || nLenTextToCode > 12)
                            {
                                DisplayErrorMessageLength("11", "12");
                                return;
                            }

                            // Calculate, (correct) and add the checksum
                            if (nLenTextToCode == 12)
                            {
                                cChecksum = cTextToCode.Substring(11, 1);
                            }

                            cTextToCode = cTextToCode[..11];
                            cTextToCode += CalculateChecksumEanUpcA(cTextToCode);

                            if (nLenTextToCode == 12 && cChecksum != cTextToCode.Substring(11, 1))
                            {
                                DisplayErrorMessage(CodeLang.CheckDigitError_Text);
                            }

                            edtTextToCode.Text = cTextToCode;
                            //cTextCode = string.Concat(cTextToCode[..1], " ", cTextToCode.Substring(1, 5), " ", cTextToCode.Substring(6, 5), " ", cTextToCode.Substring(11, 1));

                            break;

                        // UpcE
                        case 14:
                            if (TestAllowedCharacters("0123456789", cTextToCode) == false)
                            {
                                return;
                            }

                            if (nLenTextToCode < 7 || nLenTextToCode > 8)
                            {
                                DisplayErrorMessageLength("7", "8");
                                return;
                            }

                            if (cTextToCode[..1] != "0")
                            {
                                DisplayErrorMessage(CodeLang.FirstNumber0_Text);
                                return;
                            }

                            // Convert UPC-E to UPC-A code
                            if (nLenTextToCode == 8)
                            {
                                cChecksum = cTextToCode.Substring(7, 1);
                            }

                            string cUpcE = cTextToCode.Substring(1, 6);
                            string cLastDigit = cUpcE.Substring(cUpcE.Length - 1, 1);
                            int nLastDigit = Convert.ToInt32(cLastDigit);

                            string cUpcA;

                            if (nLastDigit < 3)
                            {
                                cUpcA = string.Concat(cUpcE.AsSpan(0, 2), cLastDigit, "0000", cUpcE.AsSpan(2, 3));
                            }
                            else if (nLastDigit == 3)
                            {
                                cUpcA = string.Concat(cUpcE.AsSpan(0, 3), "00000", cUpcE.AsSpan(3, 2));
                            }
                            else if (nLastDigit == 4)
                            {
                                cUpcA = string.Concat(cUpcE.AsSpan(0, 4), "00000", cUpcE.AsSpan(4, 1));
                            }
                            else
                            {
                                cUpcA = string.Concat(cUpcE.AsSpan(0, 5), "0000", cLastDigit);
                            }

                            cUpcA = $"0{cUpcA}";

                            // Calculate and add the checksum of the UPC-A code
                            cUpcA += CalculateChecksumEanUpcA(cUpcA);

                            // Add the checksum from the UPC-A code to the UPC-E code
                            cTextToCode = string.Concat(cTextToCode.AsSpan(0, 7), cUpcA.AsSpan(11, 1));

                            if (nLenTextToCode == 8 && cChecksum != cTextToCode.Substring(7, 1))
                            {
                                DisplayErrorMessage(CodeLang.CheckDigitError_Text);
                            }

                            edtTextToCode.Text = cTextToCode;
                            //cTextCode = string.Concat(cTextToCode[..1], " ", cTextToCode.Substring(1, 6), " ", cTextToCode.Substring(7, 1));

                            break;
                    }
                }
                catch (Exception)
                {
                    bgvBarcode.Value = "";
                    return;
                }
            }

            try
            {
                // For testing crashes - DivideByZeroException
                //int divByZero = 51 / int.Parse("0");

                bgvBarcode.Value = cTextToCode;
            
                btnShare.Text = $"{CodeLang.ButtonShare_Text} {pckFormatCodeGenerator.Items[selectedIndex]}";
                btnShare.IsEnabled = true;
            }
            catch (Exception ex)
            {
                _ = SentrySdk.CaptureException(ex);

                bgvBarcode.Value = "";

                RestartApplication(ex.Message);
            }
        }

        /// <summary>
        /// Test for allowed characters
        /// </summary>
        /// <param name="cAllowedCharacters"></param>
        /// <param name="cTextToCode"></param>
        /// <returns></returns>
        private bool TestAllowedCharacters(string cAllowedCharacters, string cTextToCode)
        {
            foreach (char cChar in cTextToCode)
            {
                bool bResult = cAllowedCharacters.Contains(cChar);

                if (bResult == false)
                {
                    _ = DisplayAlertAsync(CodeLang.ErrorTitle_Text, $"{CodeLang.AllowedChar_Text}\n{cAllowedCharacters}\n\n{CodeLang.AllowedCharNot_Text} {cChar}", CodeLang.ButtonClose_Text);

                    _ = edtTextToCode.Focus();
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Test for allowed minimum and maximum ASCII values
        /// </summary>
        /// <param name="nMinAsciiValue"></param>
        /// <param name="nMaxAsciiValue"></param>
        /// <param name="cTextToCode"></param>
        /// <returns></returns>
        private bool TestAllowedAsciiValues(int nMinAsciiValue, int nMaxAsciiValue, string cTextToCode)
        {
            // Test for allowed minimum and maximum ASCII values
            foreach (char cChar in cTextToCode)
            {
                //Console.WriteLine($"{"ASCII value: "} {(int)cChar}");  // For testing

                if ((int)cChar < nMinAsciiValue || (int)cChar > nMaxAsciiValue)
                {
                    _ = DisplayAlertAsync(CodeLang.ErrorTitle_Text, $"{CodeLang.TextContainsChar_Text} {cChar}", CodeLang.ButtonClose_Text);

                    _ = edtTextToCode.Focus();
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Replace special characters in strings for ASCII output (iOS keyboard marks are curved instead of straight)
        /// </summary>
        /// <param name="cText"></param>
        /// <returns></returns>
        private static string ReplaceCharacters(string cText)
        {
            // Convert characters from UTF-8 or ASCII extended to characters that are supported in ASCII
            cText = cText.Replace('‘', '\'');       // Left single quotation mark replaced with apostrophe
            cText = cText.Replace('’', '\'');       // Right single quotation mark replaced with apostrophe
            cText = cText.Replace('“', '"');        // Left double quotation mark replaced with quotation mark
            cText = cText.Replace('”', '"');        // Right double quotation mark replaced with quotation mark

            return cText;
        }

        /// <summary>
        /// Test start and end guards
        /// </summary>
        /// <param name="cStartEndGuards"></param>
        /// <param name="cTextToCode"></param>
        /// <returns></returns>
        private bool TestStartEndGuards(string cStartEndGuards, string cTextToCode)
        {
            int nPos;
            char cChar;

            // Control of start and end guards in the wrong place
            for (nPos = 0; nPos < cTextToCode.Length; nPos++)
            {
                cChar = cTextToCode[nPos];

                if (cStartEndGuards.Contains(cChar) && nPos > 0 && nPos < cTextToCode.Length - 1)
                {
                    _ = DisplayAlertAsync(CodeLang.ErrorTitle_Text, $"{CodeLang.GuardInvalidStartEnd_Text} {cChar}", CodeLang.ButtonClose_Text);

                    _ = edtTextToCode.Focus();
                    return false;
                }
            }

            // Control of missing start or end guard
            if (cStartEndGuards.Contains(cTextToCode[..1]) && cStartEndGuards.Contains(cTextToCode.Substring(cTextToCode.Length - 1, 1)) == false)
            {
                _ = DisplayAlertAsync(CodeLang.ErrorTitle_Text, CodeLang.GuardMissingEnd_Text, CodeLang.ButtonClose_Text);

                _ = edtTextToCode.Focus();
                return false;
            }
            else if (cStartEndGuards.Contains(cTextToCode[..1]) == false && cStartEndGuards.Contains(cTextToCode.Substring(cTextToCode.Length - 1, 1)))
            {
                _ = DisplayAlertAsync(CodeLang.ErrorTitle_Text, CodeLang.GuardMissingStart_Text, CodeLang.ButtonClose_Text);

                _ = edtTextToCode.Focus();
                return false;
            }

            return true;
        }

        /// <summary>
        /// Reverse the characters in a string
        /// </summary>
        /// <param name="cText"></param>
        /// <returns></returns>
        private static string ReverseString(string cText)
        {
            char[] charArray = cText.ToCharArray();
            Array.Reverse(charArray);

            return string.Concat(charArray);
        }

        /// <summary>
        /// Calculate the checksum of an EAN-13, EAN-8 and UPC-A code
        /// </summary>
        /// <param name="cTextToCode"></param>
        /// <returns></returns>
        private static string CalculateChecksumEanUpcA(string cTextToCode)
        {
            int nPos;
            int nPartialSum;
            int nChecksum = 0;
            int nCheckDigit = 0;

            // Loop over string
            for (nPos = 0; nPos < cTextToCode.Length; nPos++)
            {
                if ((nPos + 1) % 2 == 0)
                {
                    nPartialSum = (int)char.GetNumericValue(cTextToCode[nPos]);
                }
                else
                {
                    nPartialSum = (int)char.GetNumericValue(cTextToCode[nPos]) * 3;
                }
                Debug.WriteLine($"{"nPartialSum: "} {nPartialSum}");  // For testing

                nChecksum += nPartialSum;
            }
            Debug.WriteLine($"{"nChecksum: "} {nChecksum}");  // For testing

            int nRemainder = nChecksum % 10;
            if (nRemainder != 0)
            {
                nCheckDigit = 10 - nRemainder;
            }
            Debug.WriteLine($"{"nCheckDigit: "} {nCheckDigit}");  // For testing

            return Convert.ToString(nCheckDigit);
        }

        /// <summary>
        /// OnClearCodeClicked event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClearCodeClicked(object sender, EventArgs e)
        {
            edtTextToCode.Text = "";
            bgvBarcode.Value = "";
            btnShare.Text = CodeLang.ButtonShare_Text;
            btnShare.IsEnabled = false;

            _ = edtTextToCode.Focus();
        }

        /// <summary>
        /// Display an error message
        /// </summary>
        /// <param name="cMessage"></param>
        private void DisplayErrorMessage(string cMessage)
        {
            _ = DisplayAlertAsync(CodeLang.ErrorTitle_Text, cMessage, CodeLang.ButtonClose_Text);

            _ = edtTextToCode.Focus();
        }

        /// <summary>
        /// Display an error message with minimum and maximum length
        /// </summary>
        /// <param name="cMinLength"></param>
        /// <param name="cMaxLength"></param>
        private void DisplayErrorMessageLength(string cMinLength, string cMaxLength)
        {
            _ = DisplayAlertAsync(CodeLang.ErrorTitle_Text, $"{CodeLang.CodeLengthPart1_Text} {cMinLength} {CodeLang.CodeLengthPart2_Text} {cMaxLength} {CodeLang.CodeLengthPart3_Text}", CodeLang.ButtonClose_Text);

            _ = edtTextToCode.Focus();
        }

        /// <summary>
        /// Display an error message and restart the application
        /// </summary>
        /// <param name="cErrorMessage"></param>
        private async void RestartApplication(string cErrorMessage)
        {
            await DisplayAlertAsync(CodeLang.ErrorTitle_Text, $"{cErrorMessage}\n{CodeLang.RestartApp_Text}", CodeLang.ButtonClose_Text);

            Application.Current!.Windows[0].Page = new AppShell();
        }

        /// <summary>
        /// Show license using the Loaded event of the MainPage.xaml
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnPageLoaded(object sender, EventArgs e)
        {
            // Show license
            if (Globals.bLicense == false)
            {
                Globals.bLicense = await Application.Current!.Windows[0].Page!.DisplayAlertAsync(CodeLang.LicenseTitle_Text, cLicense, CodeLang.Agree_Text, CodeLang.Disagree_Text);

                if (Globals.bLicense)
                {
                    Preferences.Default.Set("SettingLicense", true);
                }
                else
                {
#if IOS
                    //Thread.CurrentThread.Abort();  // Not allowed in iOS
                    imgbtnAbout.IsEnabled = false;
                    imgbtnScanNT.IsEnabled = false;
                    imgbtnSettings.IsEnabled = false;
                    btnGenerateCode.IsEnabled = false;

                    await DisplayAlertAsync(CodeLang.LicenseTitle_Text, CodeLang.CloseApplication_Text, CodeLang.ButtonClose_Text);
#else
                    Application.Current.Quit();
#endif
                }
            }
        }

        /// <summary>
        /// Set text and speech language and the generator format using the Appearing event of the MainPage.xaml
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPageAppearing(object sender, EventArgs e)
        {
            // Set the text language
            if (Globals.bLanguageChanged)
            {
                SetTextLanguage();
                Globals.bLanguageChanged = false;
            }

            // Set the speech language
            lblTextToSpeech.Text = Globals.GetIsoLanguageCode();

            // Set the generator format in the picker
            pckFormatCodeGenerator.SelectedIndex = Globals.nFormatGeneratorIndex;
        }

        /// <summary>
        ///  On page disappearing event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnPageDisappearing(object sender, EventArgs e)
        {
            // Hide the soft keyboard when the page disappears
            if (edtTextToCode.IsSoftInputShowing())
            {
                await edtTextToCode.HideSoftInputAsync(System.Threading.CancellationToken.None);
            }
        }

        /// <summary>
        /// Button share event - make screenshot of the barcode
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnShareClicked(object sender, EventArgs e)
        {
            // Sometimes an InvalidCastException when running on a MacBook Air M1:
            // System.InvalidCastException: Unable to cast object of type 'Foundation.NSString' to type 'Foundation.NSExtensionItem'.
            try
            {
                if (Screenshot.Default.IsCaptureSupported)
                {
                    IScreenshotResult? screen = await bgvBarcode.CaptureAsync();
                    Stream stream = await screen!.OpenReadAsync();

                    SaveStreamAsFile(stream);
                }
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
#if DEBUG
                _ = DisplayAlertAsync("OnShareClicked", ex.Message, CodeLang.ButtonClose_Text);
#endif
            }
        }

        /// <summary>
        /// Save the barcode as an image file
        /// </summary>
        /// <param name="inputStream"></param>
        private static async void SaveStreamAsFile(Stream inputStream)
        {
            // Save the image file
            string cFileName = Path.Combine(FileSystem.CacheDirectory, "BarcodeGenerator.png");

            using (FileStream outputFileStream = new(cFileName, FileMode.Create))
            {
                inputStream.CopyTo(outputFileStream);
            }

            inputStream.Dispose();

            // Open the share interface to share the file
            await OpenShareInterfaceAsync(cFileName);
        }

        /// <summary>
        /// Open the share interface
        /// </summary>
        /// <param name="cFile"></param>
        /// <returns></returns>
        private static async Task OpenShareInterfaceAsync(string cFile)
        {
            await Share.Default.RequestAsync(new ShareFileRequest
            {
                Title = "Barcode Generator",
                File = new ShareFile(cFile)
            });
        }

        /// <summary>
        /// Put text in the chosen language in the controls
        /// </summary>
        private void SetTextLanguage()
        {
            // Set the current UI culture of the selected language
            Globals.SetCultureSelectedLanguage(Globals.cLanguage);

            cLicense = $"{CodeLang.License_Text}\n\n{CodeLang.LicenseMit2_Text}";
            btnShare.Text = $"{CodeLang.ButtonShare_Text} {pckFormatCodeGenerator.Items[pckFormatCodeGenerator.SelectedIndex]}";
        }

        /// <summary>
        /// Button text to speech event - Convert text to speech
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTextToSpeechClicked(object sender, EventArgs e)
        {
            // Cancel the text to speech
            if (Globals.bTextToSpeechIsBusy)
            {
                imgbtnTextToSpeech.Source = ClassSpeech.CancelTextToSpeech();
                return;
            }

            // Convert the text to speech
            _ = ClassSpeech.ConvertTextToSpeechAsync(imgbtnTextToSpeech, edtTextToCode.Text);
        }

        /// <summary>
        /// Paste text from the clipboard clicked event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnPasteFromClipboardClicked(object sender, EventArgs e)
        {
            if (Clipboard.Default.HasText)
            {
                try
                {
                    string cTextToPaste = await Clipboard.Default.GetTextAsync() ?? string.Empty;

                    if (string.IsNullOrEmpty(cTextToPaste))
                    {
                        return;
                    }

                    if (cTextToPaste.Length > edtTextToCode.MaxLength)
                    {
                        cTextToPaste = cTextToPaste[..edtTextToCode.MaxLength];
                    }

                    edtTextToCode.Text = cTextToPaste;
                }
                catch (Exception ex)
                {
                    SentrySdk.CaptureException(ex);
#if DEBUG
                    await DisplayAlertAsync("OnPasteFromClipboardClicked", ex.Message, CodeLang.ButtonClose_Text);
#endif
                }
            }
        }
    }
}

/* How to convert a UPC-E code back to UPC-A ?
   A 6-digit UPC-E code is derived from a UPC-A 12-digit code.
   You can convert a UPC-E code back to its UPC-A format using the following scenarios.

   If the UPC-E code ends in 0, 1, or 2:
   Convert the UPC-E back to UPC-A code by picking the first two digits in the UPC-E code.
   Add the last digit (still of using the UPC-E code) and then four zeros(0).
   Complete the conversion by adding characters 3 -5 of your UPC-E code.

   If the UPC-E code ends in 3:
   Determine what the UPC-A code is by picking the first three digits used in your UPC-E code.
   Add five zeros (0), and then characters 4 and 5 of the UPC-E.

   Where the UPC-E code ends in 4:
   Determine the UPC-A code in this way: take the UPC-E code and write out the first four digits.
   Add five zeros (0), then the fifth character of the UPC-E code.

   If the UPC-E code ends in any of 5, 6, 7, 8, or 9:
   Convert the UPC-E code to UPC-A by first picking the leading five digits in the UPC-E code.
   Add four 0 digits and the last character of the UPC-E code.

   Samples:
   UPC-E: 01326901 -> UPC-A: 013000002691
   UPC-E: 01810905 -> UPC-A: 018000001095

   Source: https://bytescout.com/blog/2013/10/upc-and-upc-e-purpose-advantages.html
   _____________________________________________________________________________________________ */

/* Barcode formats and maximum length of the text to encode.
   edtTextToCode.MaxLength     Numeric     AlphaNumeric    Binary 8-bits   Kanji/Kana JIS X 0208
   -----------------------     -------     ------------    -------------   ---------------------

   Aztec              1900     3832        3067            1914
   Codabar              43       43          43
   Code39               48       48          48
   Code93               48       48          48
   Code128              48       48          48
   DataMatrix         1500     3116        2335            1555                     
   Ean8                  8        8
   Ean13                13       13
   GS1 DataBar
   Imb                  31       31
   Itf                  30       30
   MaxiCode             93       93
   MicroPDF417                  366         250             150
   MicroQR                       35          21              15
   Msi                  255     255
   Pdf417              1100    2710        1850            1108
   PharmaCode             6       6
   Plessey               16      16          16
   QrCode              1800    7089        4296            2953            1817
   Rss14                 14      14
   RssExpanded           74      74
   UpcA                  12      12
   UpcE                   8       8
   UpcEanExtension        2       2
   _____________________________________________________________________________________________ */
