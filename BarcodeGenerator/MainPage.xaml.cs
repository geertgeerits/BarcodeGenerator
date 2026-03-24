/* Program .....: BarcodeGenerator.sln
 * Author ......: Geert Geerits - E-mail: geertgeerits@gmail.com
 * Copyright ...: (C) 2022-2026
 * Version .....: 1.0.50
 * Date ........: 2026-03-24 (YYYY-MM-DD)
 * Language ....: Microsoft Visual Studio 2026: .NET 10.0 MAUI C# 14.0
 * Description .: Barcode Generator: ZXing - Barcode Scanner: Native Android and iOS
 * Note ........: zxing:CameraBarcodeReaderView -> ex. WidthRequest="300" -> Grid RowDefinitions="400" (300 x 1.3333) = 3:4 aspect ratio
 *                Apple Vision framework: https://developer.apple.com/documentation/vision
 *                Google Vision: https://developers.google.com/android/reference/com/google/android/gms/vision/CameraSource.Builder
 *                Google ML Kit: https://developers.google.com/ml-kit
 *                zxing-cpp: https://github.com/zxing-cpp/zxing-cpp
 * Dependencies : NuGet Package: CommunityToolkit.Maui
 *                NuGet Package: ZXing.Net.Maui by Redth - https://github.com/redth/ZXing.Net.Maui
 *                NuGet Package: ZXing.Net.Maui.Controls by Redth
 *                NuGet Package: QRCoder by Raffael Herrmann, Shane Krueger - https://github.com/Shane32/QRCoder - https://github.com/Shane32/QRCoder/wiki
 *                NuGet Package: SkiaSharp by Microsoft - https://github.com/mono/SkiaSharp
 *                NuGet Package: BarcodeScanner.Native.Maui by Alen Friščić - https://github.com/afriscic/BarcodeScanning.Native.Maui
 *                NuGet Package: Sentry.Maui - https://sentry.io ; https://geerits.sentry.io/issues/ ; https://www.youtube.com/watch?v=9-50zH8fqYA
 * Thanks to ...: Gerald Versluis, Alen Friščić, Redth, Jimmy Pun, Raffael Herrmann, Shane Krueger, Copilot */

using ZXing.Net.Maui;

namespace BarcodeGenerator
{
    public sealed partial class MainPage : ContentPage
    {
        // Local variables
        private string cLicense = string.Empty;
        private const string cAllowedCharactersDecimal = "0123456789";
        private const string cAllowedCharactersHex = "0123456789ABCDEF";
        private const string cAllowedCharactersCode39_93 = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789 -.$/+%*";
        private static string cBarcodeCaption = string.Empty;

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

            // Disable Sentry for testing - https://github.com/getsentry/sentry-dotnet/discussions/3325
            //SentrySdk.Close();
#if IOS
            // AutoSize has to be disabled for iOS
            edtTextToCode.AutoSize = EditorAutoSizeOption.Disabled;
#endif
            // Get the saved settings
            ClassBarcodes.cBarcodeGeneratorName = Preferences.Default.Get("SettingBarcodeGeneratorName", ClassBarcodes.cBarcodeGeneratorDefault);
            ClassBarcodes.cBarcodeScannerName = Preferences.Default.Get("SettingBarcodeScannerName", ClassBarcodes.cBarcodeScannerDefault);
            ClassQRCodeImage.bQRCodeSizeVariable = Preferences.Default.Get("SettingQRCodeSizeVariable", true);
            ClassQRCodeImage.nQRCodeSizePixels = Preferences.Default.Get("SettingQRCodeSizePixels", 800);
            ClassQRCodeImage.nQRCodeImageSizePercent = Preferences.Default.Get("SettingQRCodeImageSizePercent", 25.0f);
            Globals.bBarcodeWithCaption = Preferences.Default.Get("SettingBarcodeWithCaption", true);
            Globals.cTheme = Preferences.Default.Get("SettingTheme", "System");
            Globals.cCodeColorFg = Preferences.Default.Get("SettingCodeColorFg", "FF000000");
            Globals.cCodeColorBg = Preferences.Default.Get("SettingCodeColorBg", "FFFFFFFF");
            Globals.cLanguage = Preferences.Default.Get("SettingLanguage", "");
            Globals.cLanguageSpeech = Preferences.Default.Get("SettingLanguageSpeech", "");
            Globals.bLicense = Preferences.Default.Get("SettingLicense", false);

            // The height of the title bar is lower when an iPhone is in horizontal position
            if (DeviceInfo.Platform == DevicePlatform.iOS && DeviceInfo.Idiom == DeviceIdiom.Phone)
            {
                imgbtnAbout.VerticalOptions = LayoutOptions.Start;
                lblTitle.VerticalOptions = LayoutOptions.Start;
                lblTitle.VerticalTextAlignment = TextAlignment.Start;
                imgbtnScanNT.VerticalOptions = LayoutOptions.Start;
                imgbtnSettings.VerticalOptions = LayoutOptions.Start;
            }

            // Set the tooltips for the scanner buttons
            ToolTipProperties.SetText(imgbtnScanNT, CodeLang.ToolTipBarcodeScanner_Text + " (Native)");

            // Set the theme
            Globals.SetTheme();

            // Get and set the user interface language after a first start or reset of the application
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

            // Set the text language
            SetTextLanguage();

            // Select the name and index in the barcode list and save the name
            Preferences.Default.Set("SettingBarcodeGeneratorName", ClassBarcodes.cBarcodeGeneratorName);

            // Initialize text to speech and get and set the speech language
            InitializeTextToSpeechAsync();
            
            // Path and file name of the saved barcode image
            Globals.cFileBarcodePng = Path.Combine(FileSystem.Current.CacheDirectory, "barcode_generator.png");
            Globals.cFileBarcodeSvg = Path.Combine(FileSystem.Current.CacheDirectory, "barcode_generator.svg");

            // Clear the clipboard
            //Clipboard.Default.SetTextAsync(null);  // For testing

            // Set focus to the editor
            edtTextToCode.Focus();

            // Test for crashes Sentry
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

        /// <summary>
        /// Handles the click event for the About page navigation button by canceling any ongoing text-to-speech
        /// operation and navigating asynchronously to the About page.
        /// </summary>
        /// <remarks>This method ensures that any active text-to-speech process is stopped before
        /// navigating to the About page. It performs navigation asynchronously to maintain UI responsiveness.</remarks>
        /// <param name="sender">The source of the event, typically the button that was clicked.</param>
        /// <param name="e">The event data associated with the click event.</param>
        private async void OnPageAboutClicked(object sender, EventArgs e)
        {
            imgbtnTextToSpeech.Source = ClassSpeech.CancelTextToSpeech();
            await Navigation.PushAsync(new PageAbout());
        }

        /// <summary>
        /// Handles the click event for the page scan button and navigates to the appropriate scanning page based on the
        /// platform.
        /// </summary>
        /// <remarks>Cancels any ongoing text-to-speech operation before navigating. On Windows platforms,
        /// navigates to a ZX scanning page due to a known issue with the native scanner.</remarks>
        /// <param name="sender">The source of the event, typically the button that was clicked.</param>
        /// <param name="e">An object that contains the event data.</param>
        private async void OnPageScanClickedNT(object sender, EventArgs e)
        {
            imgbtnTextToSpeech.Source = ClassSpeech.CancelTextToSpeech();
#if WINDOWS
            //await Navigation.PushAsync(new PageScanNT());  // !!!BUG!!! Error in NuGet package when using the native Windows scanner ???
            await Navigation.PushAsync(new PageScanZX());
#else
            await Navigation.PushAsync(new PageScanNT());
#endif
        }

        /// <summary>
        /// On page settings clicked event - navigate to the settings page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
            const int nHeightBarcode1D = 160;
            const int nHeightBarcode2D = 280;
            const int nWidthBarcode2D = 280;

            var picker = (Picker)sender;
            int selectedIndex = picker.SelectedIndex;

            if (selectedIndex != -1)
            {
                var itemsSource = picker.ItemsSource;
                string? item = itemsSource is not null && itemsSource.Count > selectedIndex
                    ? itemsSource[selectedIndex] as string : null;

                string? selectedName = item is not null
                    ? picker.ItemsSource[selectedIndex] as string : string.Empty;

                ClassQRCodeImage.cQRCodeType = string.Empty;

                brdQrCodeImage.IsVisible = false;
                imgQrCodeImage.IsVisible = false;
                brdBarcode.IsVisible = true;
                bgvBarcode.IsVisible = true;

                imgQrCodeImage.Source = null;
                bgvBarcode.Value = string.Empty;
                bgvBarcode.HeightRequest = nHeightBarcode1D;
                bgvBarcode.WidthRequest = -1;
                bgvBarcode.MaximumHeightRequest = nHeightBarcode2D;
                bgvBarcode.MaximumWidthRequest = 600;
                bgvBarcode.HorizontalOptions = LayoutOptions.Fill;
                edtTextToCode.Placeholder = string.Empty;

                btnShare.Text = CodeLang.ButtonShare_Text;
                btnShare.IsEnabled = false;

                if (selectedName == ClassBarcodes.cBarcode_AZTEC)
                {
                    edtTextToCode.Placeholder = $"{CodeLang.MaximumCharacters_Text} 1900";
                    edtTextToCode.MaxLength = 1900;
                    edtTextToCode.Keyboard = Keyboard.Default;
                    bgvBarcode.HeightRequest = nHeightBarcode2D;
                    bgvBarcode.WidthRequest = nWidthBarcode2D;
                    bgvBarcode.BarcodeMargin = 2;
                    bgvBarcode.Format = BarcodeFormat.Aztec;
                }
                else if (selectedName == ClassBarcodes.cBarcode_CODABAR)
                {
                    edtTextToCode.Placeholder = $"{CodeLang.MaximumCharacters_Text} 43";
                    edtTextToCode.MaxLength = 43;
                    edtTextToCode.Keyboard = Keyboard.Default;
                    bgvBarcode.BarcodeMargin = 4;
                    bgvBarcode.Format = BarcodeFormat.Codabar;
                }
                else if (selectedName == ClassBarcodes.cBarcode_CODE_39)
                {
                    edtTextToCode.Placeholder = $"{CodeLang.MaximumCharacters_Text} 48\n{CodeLang.AllowedChar_Text} {cAllowedCharactersCode39_93}";
                    edtTextToCode.MaxLength = 48;
                    edtTextToCode.Keyboard = Keyboard.Default;
                    bgvBarcode.BarcodeMargin = 4;
                    bgvBarcode.Format = BarcodeFormat.Code39;
                }
                else if (selectedName == ClassBarcodes.cBarcode_CODE_93)
                {
                    edtTextToCode.Placeholder = $"{CodeLang.MaximumCharacters_Text} 48\n{CodeLang.AllowedChar_Text} {cAllowedCharactersCode39_93}";
                    edtTextToCode.MaxLength = 48;
                    edtTextToCode.Keyboard = Keyboard.Default;
                    bgvBarcode.BarcodeMargin = 4;
                    bgvBarcode.Format = BarcodeFormat.Code93;
                }
                else if (selectedName == ClassBarcodes.cBarcode_CODE_128)
                {
                    edtTextToCode.Placeholder = $"{CodeLang.MaximumCharacters_Text} 48";
                    edtTextToCode.MaxLength = 48;
                    edtTextToCode.Keyboard = Keyboard.Default;
                    bgvBarcode.BarcodeMargin = 20;
                    bgvBarcode.Format = BarcodeFormat.Code128;
                }
                else if (selectedName == ClassBarcodes.cBarcode_DATA_MATRIX)
                {
                    edtTextToCode.Placeholder = $"{CodeLang.MaximumCharacters_Text} 1500";
                    edtTextToCode.MaxLength = 1500;
                    edtTextToCode.Keyboard = Keyboard.Default;
                    bgvBarcode.HeightRequest = nHeightBarcode2D;
                    bgvBarcode.WidthRequest = nWidthBarcode2D;
                    bgvBarcode.BarcodeMargin = 2;
                    bgvBarcode.Format = BarcodeFormat.DataMatrix;
                }
                else if (selectedName == ClassBarcodes.cBarcode_EAN_8)
                {
                    edtTextToCode.Placeholder = $"{CodeLang.NumberOfDigits_Text} 8\n{CodeLang.AllowedChar_Text} {cAllowedCharactersDecimal}";
                    edtTextToCode.MaxLength = 8;
                    edtTextToCode.Keyboard = Keyboard.Numeric;
                    bgvBarcode.BarcodeMargin = 4;
                    bgvBarcode.Format = BarcodeFormat.Ean8;
                }
                else if (selectedName == ClassBarcodes.cBarcode_EAN_13)
                {
                    edtTextToCode.Placeholder = $"{CodeLang.NumberOfDigits_Text} 13\n{CodeLang.AllowedChar_Text} {cAllowedCharactersDecimal}";
                    edtTextToCode.MaxLength = 13;
                    edtTextToCode.Keyboard = Keyboard.Numeric;
                    bgvBarcode.BarcodeMargin = 4;
                    bgvBarcode.Format = BarcodeFormat.Ean13;
                }
                else if (selectedName == ClassBarcodes.cBarcode_ITF)
                {
                    edtTextToCode.Placeholder = $"{CodeLang.MaximumCharacters_Text} 30\n{CodeLang.AllowedChar_Text} {cAllowedCharactersDecimal}";
                    edtTextToCode.MaxLength = 30;
                    edtTextToCode.Keyboard = Keyboard.Numeric;
                    bgvBarcode.BarcodeMargin = 8;
                    bgvBarcode.Format = BarcodeFormat.Itf;
                }
                else if (selectedName == ClassBarcodes.cBarcode_MSI)
                {
                    edtTextToCode.Placeholder = $"{CodeLang.MaximumCharacters_Text} 255\n{CodeLang.AllowedChar_Text} {cAllowedCharactersDecimal}";
                    edtTextToCode.MaxLength = 255;
                    edtTextToCode.Keyboard = Keyboard.Numeric;
                    bgvBarcode.BarcodeMargin = 10;
                    bgvBarcode.Format = BarcodeFormat.Msi;
                }
                else if (selectedName == ClassBarcodes.cBarcode_PDF_417)
                {
                    edtTextToCode.Placeholder = $"{CodeLang.MaximumCharacters_Text} 1100";
                    edtTextToCode.MaxLength = 1100;
                    edtTextToCode.Keyboard = Keyboard.Default;
                    bgvBarcode.HeightRequest = nHeightBarcode2D;
#if WINDOWS
                    bgvBarcode.BarcodeMargin = 25;
#else
                    bgvBarcode.BarcodeMargin = 10;
#endif              
                    bgvBarcode.Format = BarcodeFormat.Pdf417;
                }
                else if (selectedName == ClassBarcodes.cBarcode_PLESSEY)
                {
                    edtTextToCode.Placeholder = $"{CodeLang.MaximumCharacters_Text} 16\n{CodeLang.AllowedChar_Text} {cAllowedCharactersHex}";
                    edtTextToCode.MaxLength = 16;
                    edtTextToCode.Keyboard = Keyboard.Default;
                    bgvBarcode.BarcodeMargin = 4;
                    bgvBarcode.Format = BarcodeFormat.Plessey;
                }
                else if (selectedName == ClassBarcodes.cBarcode_QR_CODE)        // Model 2 - ECCLevel.Quartile
                {
                    edtTextToCode.Placeholder = $"{CodeLang.MaximumCharacters_Text} {string.Format(CodeLang.MaximumCharactersNABK_Text, 3993, 2420, 1663, 1024)}";
                    edtTextToCode.MaxLength = 3993;
                    edtTextToCode.Keyboard = Keyboard.Default;
                    imgQrCodeImage.HeightRequest = nHeightBarcode2D;
                    imgQrCodeImage.WidthRequest = nWidthBarcode2D;
                    brdBarcode.IsVisible = false;
                    bgvBarcode.IsVisible = false;
                    brdQrCodeImage.IsVisible = true;
                    imgQrCodeImage.IsVisible = true;
                }
                else if (selectedName == ClassBarcodes.cBarcode_ART_QR_CODE)  // Model 2 - ECCLevel.Quartile
                {
                    edtTextToCode.Placeholder = $"{CodeLang.MaximumCharacters_Text} {string.Format(CodeLang.MaximumCharactersNABK_Text, 3993, 2420, 1663, 1024)}";
                    edtTextToCode.MaxLength = 3057;
                    edtTextToCode.Keyboard = Keyboard.Default;
                    imgQrCodeImage.HeightRequest = nHeightBarcode2D;
                    imgQrCodeImage.WidthRequest = nWidthBarcode2D;
                    brdBarcode.IsVisible = false;
                    bgvBarcode.IsVisible = false;
                    brdQrCodeImage.IsVisible = true;
                    imgQrCodeImage.IsVisible = true;
                }
                else if (selectedName == ClassBarcodes.cBarcode_QR_CODE_IMAGE)  // Model 2 - ECCLevel.High
                {
                    edtTextToCode.Placeholder = $"{CodeLang.MaximumCharacters_Text} {string.Format(CodeLang.MaximumCharactersNABK_Text, 3057, 1852, 1273, 784)}";
                    edtTextToCode.MaxLength = 3057;
                    edtTextToCode.Keyboard = Keyboard.Default;
                    imgQrCodeImage.HeightRequest = nHeightBarcode2D;
                    imgQrCodeImage.WidthRequest = nWidthBarcode2D;
                    brdBarcode.IsVisible = false;
                    bgvBarcode.IsVisible = false;
                    brdQrCodeImage.IsVisible = true;
                    imgQrCodeImage.IsVisible = true;
                }
                else if (selectedName == ClassBarcodes.cBarcode_MICRO_QR_CODE)  // Version M4 - ECCLevel.Low
                {
                    edtTextToCode.Placeholder = $"{CodeLang.MaximumCharacters_Text} {string.Format(CodeLang.MaximumCharactersNABK_Text, 35, 21, 15, 9)}";
                    edtTextToCode.MaxLength = 35;
                    edtTextToCode.Keyboard = Keyboard.Default;
                    imgQrCodeImage.HeightRequest = nHeightBarcode2D;
                    imgQrCodeImage.WidthRequest = nWidthBarcode2D;
                    brdBarcode.IsVisible = false;
                    bgvBarcode.IsVisible = false;
                    brdQrCodeImage.IsVisible = true;
                    imgQrCodeImage.IsVisible = true;
                }
                else if (selectedName == ClassBarcodes.cBarcode_UPC_A)
                {
                    edtTextToCode.Placeholder = $"{CodeLang.NumberOfDigits_Text} 12\n{CodeLang.AllowedChar_Text} {cAllowedCharactersDecimal}";
                    edtTextToCode.MaxLength = 12;
                    edtTextToCode.Keyboard = Keyboard.Numeric;
                    bgvBarcode.BarcodeMargin = 0;
                    bgvBarcode.Format = BarcodeFormat.UpcA;
                }
                else if (selectedName == ClassBarcodes.cBarcode_UPC_E)
                {
                    edtTextToCode.Placeholder = $"{CodeLang.NumberOfDigits_Text} 8\n{CodeLang.AllowedChar_Text} {cAllowedCharactersDecimal}";
                    edtTextToCode.MaxLength = 8;
                    edtTextToCode.Keyboard = Keyboard.Numeric;
                    bgvBarcode.BarcodeMargin = 8;
                    bgvBarcode.Format = BarcodeFormat.UpcE;
                }

                edtTextToCode.Focus();
            }
        }

        /// <summary>
        /// OnGenerateCodeClicked event
        /// Validate the text input and set the format for the selected format code and generate the barcode
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnGenerateCodeClicked(object sender, EventArgs e)
        {
            // Hide the keyboard
            edtTextToCode.IsEnabled = false;
            edtTextToCode.IsEnabled = true;

            // Ensure any existing barcode files are deleted before generating new ones to avoid confusion and manage storage
            ClassFileOperations.DeleteFileIfExists(Globals.cFileBarcodePng);
            ClassFileOperations.DeleteFileIfExists(Globals.cFileBarcodeSvg);

            // Set the barcode colors
            bgvBarcode.ForegroundColor = Color.FromArgb(Globals.cCodeColorFg);
            bgvBarcode.BackgroundColor = Color.FromArgb(Globals.cCodeColorBg);

            // Miscellaneous
            bgvBarcode.Value = string.Empty;
            cBarcodeCaption = string.Empty;
            string cChecksum = string.Empty;

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

            int selectedIndex = pckFormatCodeGenerator.SelectedIndex;

            var itemsSource = pckFormatCodeGenerator.ItemsSource;
            string? item = itemsSource is not null && itemsSource.Count > selectedIndex
                ? itemsSource[selectedIndex] as string : null;

            string? selectedName = item is not null
                ? pckFormatCodeGenerator.ItemsSource[selectedIndex] as string : string.Empty;

            // Validate the text input and set the format
            if (selectedIndex != -1)
            {
                try
                {
                    if (selectedName == ClassBarcodes.cBarcode_AZTEC)
                    {
                        cTextToCode = ClassValidateBarcodes.ReplaceCharacters(cTextToCode);
                        edtTextToCode.Text = cTextToCode;

                        if (await ClassValidateBarcodes.TestAllowedAsciiValues(1, 255, cTextToCode) == false)
                        {
                            _ = edtTextToCode.Focus();
                            return;
                        }
                    }
                    else if (selectedName == ClassBarcodes.cBarcode_CODABAR)
                    {
                        cTextToCode = cTextToCode.ToUpper();

                        if (await ClassValidateBarcodes.TestAllowedCharacters("0123456789-$:/.+ABCD", cTextToCode) == false)
                        {
                            _ = edtTextToCode.Focus();
                            return;
                        }

                        if (await ClassValidateBarcodes.TestStartEndGuards("ABCD", cTextToCode) == false)
                        {
                            _ = edtTextToCode.Focus();
                            return;
                        }

                        cBarcodeCaption = cTextToCode;
                    }
                    else if (selectedName == ClassBarcodes.cBarcode_CODE_39)
                    {
                        cTextToCode = cTextToCode.ToUpper();

                        if (await ClassValidateBarcodes.TestAllowedCharacters(cAllowedCharactersCode39_93, cTextToCode) == false)
                        {
                            _ = edtTextToCode.Focus();
                            return;
                        }

                        if (await ClassValidateBarcodes.TestStartEndGuards("*", cTextToCode) == false)
                        {
                            _ = edtTextToCode.Focus();
                            return;
                        }

                        cBarcodeCaption = cTextToCode;
                    }
                    else if (selectedName == ClassBarcodes.cBarcode_CODE_93)
                    {
                        cTextToCode = cTextToCode.ToUpper();

                        if (await ClassValidateBarcodes.TestAllowedCharacters(cAllowedCharactersCode39_93, cTextToCode) == false)
                        {
                            _ = edtTextToCode.Focus();
                            return;
                        }

                        if (await ClassValidateBarcodes.TestStartEndGuards("*", cTextToCode) == false)
                        {
                            _ = edtTextToCode.Focus();
                            return;
                        }

                        cBarcodeCaption = cTextToCode;
                    }
                    else if (selectedName == ClassBarcodes.cBarcode_CODE_128)
                    {
                        cTextToCode = ClassValidateBarcodes.ReplaceCharacters(cTextToCode);
                        edtTextToCode.Text = cTextToCode;

                        if (await ClassValidateBarcodes.TestAllowedAsciiValues(1, 127, cTextToCode) == false)
                        {
                            _ = edtTextToCode.Focus();
                            return;
                        }

                        cBarcodeCaption = cTextToCode;
                    }
                    else if (selectedName == ClassBarcodes.cBarcode_DATA_MATRIX)
                    {
                        cTextToCode = ClassValidateBarcodes.ReplaceCharacters(cTextToCode);
                        edtTextToCode.Text = cTextToCode;

                        if (await ClassValidateBarcodes.TestAllowedAsciiValues(1, 255, cTextToCode) == false)
                        {
                            _ = edtTextToCode.Focus();
                            return;
                        }
                    }
                    else if (selectedName == ClassBarcodes.cBarcode_EAN_8)
                    {
                        if (await ClassValidateBarcodes.TestAllowedCharacters(cAllowedCharactersDecimal, cTextToCode) == false)
                        {
                            _ = edtTextToCode.Focus();
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
                        cTextToCode += ClassValidateBarcodes.CalculateChecksumEanUpcA(ClassValidateBarcodes.ReverseString(cTextToCode));

                        if (nLenTextToCode == 8 && cChecksum != cTextToCode.Substring(7, 1))
                        {
                            DisplayErrorMessage(CodeLang.CheckDigitError_Text);
                        }

                        edtTextToCode.Text = cTextToCode;
                        cBarcodeCaption = InsertCharacterInCaption(cTextToCode, 4);
                    }
                    else if (selectedName == ClassBarcodes.cBarcode_EAN_13)
                    {
                        if (await ClassValidateBarcodes.TestAllowedCharacters(cAllowedCharactersDecimal, cTextToCode) == false)
                        {
                            _ = edtTextToCode.Focus();
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
                        cTextToCode += ClassValidateBarcodes.CalculateChecksumEanUpcA(ClassValidateBarcodes.ReverseString(cTextToCode));

                        if (nLenTextToCode == 13 && cChecksum != cTextToCode.Substring(12, 1))
                        {
                            DisplayErrorMessage(CodeLang.CheckDigitError_Text);
                        }

                        edtTextToCode.Text = cTextToCode;
                        cBarcodeCaption = InsertCharacterInCaption(cTextToCode, 7);
                        cBarcodeCaption = InsertCharacterInCaption(cBarcodeCaption, 1);
                    }
                    else if (selectedName == ClassBarcodes.cBarcode_ITF)
                    {
                        if (await ClassValidateBarcodes.TestAllowedCharacters(cAllowedCharactersDecimal, cTextToCode) == false)
                        {
                            _ = edtTextToCode.Focus();
                            return;
                        }

                        if (nLenTextToCode % 2 != 0)
                        {
                            DisplayErrorMessage(CodeLang.LengthInputEven_Text);
                            return;
                        }

                        cBarcodeCaption = cTextToCode;
                    }
                    else if (selectedName == ClassBarcodes.cBarcode_MSI)
                    {
                        if (await ClassValidateBarcodes.TestAllowedCharacters(cAllowedCharactersDecimal, cTextToCode) == false)
                        {
                            _ = edtTextToCode.Focus();
                            return;
                        }

                        cBarcodeCaption = cTextToCode;
                    }
                    else if (selectedName == ClassBarcodes.cBarcode_PDF_417)
                    {
                        cTextToCode = ClassValidateBarcodes.ReplaceCharacters(cTextToCode);
                        edtTextToCode.Text = cTextToCode;

                        if (await ClassValidateBarcodes.TestAllowedAsciiValues(1, 255, cTextToCode) == false)
                        {
                            _ = edtTextToCode.Focus();
                            return;
                        }
                    }
                    else if (selectedName == ClassBarcodes.cBarcode_PLESSEY)
                    {
                        cTextToCode = cTextToCode.ToUpper();

                        if (await ClassValidateBarcodes.TestAllowedCharacters(cAllowedCharactersHex, cTextToCode) == false)
                        {
                            _ = edtTextToCode.Focus();
                            return;
                        }

                        cBarcodeCaption = cTextToCode;
                    }
                    else if (selectedName == ClassBarcodes.cBarcode_QR_CODE)
                    {
                        // no validation here
                    }
                    else if (selectedName == ClassBarcodes.cBarcode_ART_QR_CODE)
                    {
                        // no validation here
                    }
                    else if (selectedName == ClassBarcodes.cBarcode_QR_CODE_IMAGE)
                    {
                        // no validation here
                    }
                    else if (selectedName == ClassBarcodes.cBarcode_MICRO_QR_CODE)
                    {
                        // no validation here
                    }
                    else if (selectedName == ClassBarcodes.cBarcode_UPC_A)
                    {
                        if (await ClassValidateBarcodes.TestAllowedCharacters(cAllowedCharactersDecimal, cTextToCode) == false)
                        {
                            _ = edtTextToCode.Focus();
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
                        cTextToCode += ClassValidateBarcodes.CalculateChecksumEanUpcA(cTextToCode);

                        if (nLenTextToCode == 12 && cChecksum != cTextToCode.Substring(11, 1))
                        {
                            DisplayErrorMessage(CodeLang.CheckDigitError_Text);
                        }

                        edtTextToCode.Text = cTextToCode;
                        cBarcodeCaption = InsertCharacterInCaption(cTextToCode, 11);
                        cBarcodeCaption = InsertCharacterInCaption(cBarcodeCaption, 6);
                        cBarcodeCaption = InsertCharacterInCaption(cBarcodeCaption, 1);
                    }
                    else if (selectedName == ClassBarcodes.cBarcode_UPC_E)
                    {
                        if (await ClassValidateBarcodes.TestAllowedCharacters(cAllowedCharactersDecimal, cTextToCode) == false)
                        {
                            _ = edtTextToCode.Focus();
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

                        // Store the checksum character if it exists to compare it later with the calculated checksum character from the UPC-A code, because the checksum character of the UPC-E code is the same as the checksum character of the corresponding UPC-A code
                        if (nLenTextToCode == 8)
                        {
                            cChecksum = cTextToCode.Substring(7, 1);
                        }
                        
                        // Convert UPC-E to UPC-A code
                        string cUpcA = ClassValidateBarcodes.ConvertUpcEToUpcA(cTextToCode, nLenTextToCode);

                        // Calculate and add the checksum of the UPC-A code
                        cUpcA += ClassValidateBarcodes.CalculateChecksumEanUpcA(cUpcA);

                        // Add the checksum from the UPC-A code to the UPC-E code
                        cTextToCode = string.Concat(cTextToCode.AsSpan(0, 7), cUpcA.AsSpan(11, 1));

                        if (nLenTextToCode == 8 && cChecksum != cTextToCode.Substring(7, 1))
                        {
                            DisplayErrorMessage(CodeLang.CheckDigitError_Text);
                        }

                        edtTextToCode.Text = cTextToCode;
                        cBarcodeCaption = InsertCharacterInCaption(cTextToCode, 7);
                        cBarcodeCaption = InsertCharacterInCaption(cBarcodeCaption, 1);
                    }
                }
                catch (Exception)
                {
                    bgvBarcode.Value = string.Empty;
                    return;
                }
            }

            // Generate the barcode
            _ = GenerateBarcode(selectedIndex, selectedName!, cTextToCode);
        }

        /// <summary>
        /// Generate the barcode based on the selected format and the input text
        /// </summary>
        /// <param name="selectedIndex">The index of the selected barcode format.</param>
        /// <param name="selectedName">The name of the selected barcode format.</param>
        /// <param name="cTextToCode">The text to encode into the barcode.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task GenerateBarcode(int selectedIndex, string selectedName, string cTextToCode)
        {
            try
            {
                // For testing crashes - DivideByZeroException
                //int divByZero = 51 / int.Parse("0");

                if (OperatingSystem.IsWindows() && selectedName == ClassBarcodes.cBarcode_ART_QR_CODE)
                {
                    ClassQRCodeImage.cQRCodeType = selectedName;

                    ImageSource? qrImage = await ClassArtQRCode.GenerateArtQrCodeAsync(cTextToCode);
                    imgQrCodeImage.Source = qrImage;
                }
                // Generate the QR code with or without an image using QRCoder (and SkiaSharp)
                else if (selectedName == ClassBarcodes.cBarcode_QR_CODE || selectedName == ClassBarcodes.cBarcode_QR_CODE_IMAGE)
                {
                    ClassQRCodeImage.cQRCodeType = selectedName;

                    ImageSource? qrImage = await ClassQRCodeImage.GenerateQrCodeAsync(cTextToCode);
                    imgQrCodeImage.Source = qrImage;
                }
                // Generate the Micro QR code using QRCoder
                else if (selectedName == ClassBarcodes.cBarcode_MICRO_QR_CODE)
                {
                    ClassQRCodeImage.cQRCodeType = selectedName;

                    ImageSource? qrImage = await ClassMicroQRCode.GenerateMicroQrCodeAsync(cTextToCode, -4);
                    imgQrCodeImage.Source = qrImage;
                }
                // Generate the other barcodes using the BarcodeView control from the ZXing.Net.MAUI library
                else
                {
                    bgvBarcode.Value = cTextToCode;
                }

                btnShare.Text = $"{CodeLang.ButtonShare_Text} {pckFormatCodeGenerator.Items[selectedIndex]}";
                btnShare.IsEnabled = true;
            }
            catch (Exception ex)
            {
                _ = SentrySdk.CaptureException(ex);

                bgvBarcode.Value = string.Empty;

                RestartApplication(ex.Message);
            }
        }

        /// <summary>
        /// Inserts a specified character or string into the caption at the given position.
        /// Start with the last position to insert to avoid affecting the positions of the remaining characters to insert.
        /// This method is used to insert spaces in the caption of EAN and UPC barcodes for better readability.
        /// </summary>
        /// <param name="cCaption">The original caption string in which to insert the character or string. If null, the method returns the
        /// original value.</param>
        /// <param name="nPosition">The zero-based position in the caption at which to insert the character or string. Must be between 0 and the
        /// length of the caption, inclusive.</param>
        /// <param name="cCharacter">The character or string to insert. If not specified, a single space character is used.</param>
        /// <returns>A new string with the specified character or string inserted at the given position. If the caption is null
        /// or the position is out of range, returns the original caption.</returns>
        private static string InsertCharacterInCaption(string cCaption, int nPosition, string cCharacter = " ")
        {
            if (cCaption == null || nPosition < 0 || nPosition > cCaption.Length)
            {
                return cCaption!;
            }

            return cCaption.Insert(nPosition, cCharacter);
        }

        /// <summary>
        /// OnClearCodeClicked event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClearCodeClicked(object sender, EventArgs e)
        {
            edtTextToCode.Text = string.Empty;
            bgvBarcode.Value = string.Empty;
            imgQrCodeImage.Source = null;
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
            // If the page is appearing after a popup message, then do nothing
            if (Globals.bIsPopupMessage)
            {
                return;
            }

            // Set the text language
            if (Globals.bLanguageChanged)
            {
                SetTextLanguage();
                Globals.bLanguageChanged = false;
            }

            // Set the speech language
            lblTextToSpeech.Text = Globals.GetIsoLanguageCode();

            // Set the generator format in the picker
            pckFormatCodeGenerator.SelectedIndex = ClassBarcodes.nBarcodeGeneratorIndex;
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
            try
            {
                // Share the QR code or the Micro QR code as an image file using the Share API
                if (ClassQRCodeImage.cQRCodeType == ClassBarcodes.cBarcode_QR_CODE || ClassQRCodeImage.cQRCodeType == ClassBarcodes.cBarcode_MICRO_QR_CODE)
                {
                    if (!await ClassFileOperations.ShareMultipleFilesAsync())
                    {
                        await Share.Default.RequestAsync(new ShareFileRequest
                        {
                            Title = "Barcode Generator",
                            File = new ShareFile(Globals.cFileBarcodePng)
                        });
                    }
                }
                // Share the QR code with the image as an image file using the Share API
                else if (ClassQRCodeImage.cQRCodeType == ClassBarcodes.cBarcode_ART_QR_CODE || ClassQRCodeImage.cQRCodeType == ClassBarcodes.cBarcode_QR_CODE_IMAGE)
                {
                    await Share.Default.RequestAsync(new ShareFileRequest
                    {
                        Title = "Barcode Generator",
                        File = new ShareFile(Globals.cFileBarcodePng)
                    });
                }
                // Share the barcode by capturing the barcode view and saving it as a file using the Share API
                else if (Screenshot.Default.IsCaptureSupported)
                {
                    // Capture the barcode view as a screenshot
                    IScreenshotResult? screen = await bgvBarcode.CaptureAsync();

                    // Barcode with caption
                    if (Globals.bBarcodeWithCaption && !string.IsNullOrEmpty(cBarcodeCaption))
                    {
                        string cFile = await ClassBarcodeCaption.SaveBarcodeWithCaptionFromScreenshotAsync(screen!, cBarcodeCaption, Globals.cFileBarcodePng);

                        // Open the share interface to share the file
                        await ClassFileOperations.OpenShareInterfaceAsync(cFile);
                    }
                    // Barcode without caption
                    else
                    {
                        Stream stream = await screen!.OpenReadAsync();

                        // Save the barcode as a file
                        ClassFileOperations.SaveStreamAsFilePng(stream, Globals.cFileBarcodePng);

                        // Open the share interface to share the file
                        await ClassFileOperations.OpenShareInterfaceAsync(Globals.cFileBarcodePng);
                    }
                }
            }
            catch (Exception ex)
            {
                // Sometimes an InvalidCastException when running on a MacBook Air M1:
                // System.InvalidCastException: Unable to cast object of type 'Foundation.NSString' to type 'Foundation.NSExtensionItem'.
                SentrySdk.CaptureException(ex);
#if DEBUG
                _ = DisplayAlertAsync("OnShareClicked", ex.Message, CodeLang.ButtonClose_Text);
#endif
            }
        }

        /// <summary>
        /// Put text in the chosen language in the controls
        /// </summary>
        private void SetTextLanguage()
        {
            // Set the current UI culture of the selected language
            Globals.SetCultureSelectedLanguage(Globals.cLanguage);

            // Initialize the barcode formats in the ClassBarcodes class to update the format names in the selected language
            ClassBarcodes.InitializeBarcodeFormats();
#if WINDOWS
            pckFormatCodeGenerator.ItemsSource = ClassBarcodes.GetFormatCodeListGenerator_ZX_Windows();
#else
            pckFormatCodeGenerator.ItemsSource = ClassBarcodes.GetFormatCodeListGenerator_ZX();
#endif
            // Select the name and index in the barcode list and save the name
            ClassBarcodes.SelectBarcodeGeneratorNameIndex(pckFormatCodeGenerator);

            cLicense = $"{CodeLang.License_Text}\n\n{CodeLang.LicenseMit2_Text}";
            
            if (pckFormatCodeGenerator.SelectedIndex >= 0)
            {
                btnShare.Text = $"{CodeLang.ButtonShare_Text} {pckFormatCodeGenerator.Items[pckFormatCodeGenerator.SelectedIndex]}";
            }
            else
            {
                btnShare.Text = CodeLang.ButtonShare_Text;
            }
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
