// Program .....: BarcodeGenerator.sln
// Author ......: Geert Geerits - E-mail: geertgeerits@gmail.com
// Copyright ...: (C) 2022-2023
// Version .....: 1.0.34
// Date ........: 2023-07-24 (YYYY-MM-DD)
// Language ....: Microsoft Visual Studio 2022: .NET 7.0 MAUI C# 11.0
// Description .: Barcode Generator using ZXing
// Note ........: zxing:CameraBarcodeReaderView -> ex. WidthRequest="300" -> Grid RowDefinitions="400" (300 x 1.3333) = 3:4 aspect ratio
// Dependencies : NuGet Package: ZXing.Net.Maui by Redth version 0.3.0-preview.1 ; https://github.com/redth/ZXing.Net.Maui
//                NuGet Package: ZXing.Net.Maui.Controls by Redth version 0.3.0-preview.1
//                NuGet Package: Microsoft.AppCenter version 5.0.2 ; https://appcenter.ms/apps ; https://azure.microsoft.com/en-us/products/app-center/
//                NuGet Package: Microsoft.AppCenter.Crashes version 5.0.2 
// Thanks to ...: Gerald Versluis

using System;
using ZXing.Net.Maui;

namespace BarcodeGenerator;

public partial class MainPage : ContentPage
{
    // Local variables.
    private string cLicense;
    private readonly bool bLicense;
    private readonly bool bLogAlwaysSend;
    private IEnumerable<Locale> locales;
    private CancellationTokenSource cts;
    private bool bTextToSpeechIsBusy = false;

    public MainPage()
    {
        try
        {
            InitializeComponent();
        }
        catch (Exception ex)
        {
            DisplayAlert("InitializeComponent: MainPage", ex.Message, "OK");
            return;
        }

        // Get the saved settings.
        Globals.cTheme = Preferences.Default.Get("SettingTheme", "System");
        Globals.nFormatGeneratorIndex = Preferences.Default.Get("SettingFormatGeneratorIndex", 15);
        Globals.nFormatScannerIndex = Preferences.Default.Get("SettingFormatScannerIndex", 21);
        Globals.cCodeColorFg = Preferences.Default.Get("SettingCodeColorFg", "FF000000");
        Globals.cCodeColorBg = Preferences.Default.Get("SettingCodeColorBg", "FFFFFFFF");
        Globals.cLanguage = Preferences.Default.Get("SettingLanguage", "");
        Globals.cLanguageSpeech = Preferences.Default.Get("SettingLanguageSpeech", "");
        bLicense = Preferences.Default.Get("SettingLicense", false);
        bLogAlwaysSend = Preferences.Default.Get("SettingLogAlwaysSend", false);

        // For testing crashes - DivideByZeroException.
        //int divByZero = 51 / int.Parse("0");

        // Crash log confirmation.
        if (!bLogAlwaysSend)
        {
            Crashes.ShouldAwaitUserConfirmation = () =>
            {
                // Return true if you built a UI for user consent and are waiting for user input on that custom UI, otherwise false.
                ConfirmationSendCrashLog();
                return true;
            };
        }

        // Set the theme.
        if (Globals.cTheme == "Light")
        {
            Application.Current.UserAppTheme = AppTheme.Light;
        }
        else if (Globals.cTheme == "Dark")
        {
            Application.Current.UserAppTheme = AppTheme.Dark;
        }
        else
        {
            Application.Current.UserAppTheme = AppTheme.Unspecified;
        }

        // Set the barcode list and the current default barcode format in the picker for the barcode generator.
        pckFormatCodeGenerator.ItemsSource = Globals.GetFormatCodeListGenerator();

        if (Globals.nFormatGeneratorIndex < 0 || Globals.nFormatGeneratorIndex > 20)
        {
            // Default format code = QrCode.
            pckFormatCodeGenerator.SelectedIndex = 15;
        }
        else
        {
            // Set the format barcode to the saved code.
            pckFormatCodeGenerator.SelectedIndex = Globals.nFormatGeneratorIndex;
        }

        // Get and set the system OS user language.
        try
        {
            if (string.IsNullOrEmpty(Globals.cLanguage))
            {
                Globals.cLanguage = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
            }
        }
        catch (Exception)
        {
            Globals.cLanguage = "en";
        }

        SetTextLanguage();

        // Initialize text to speech and get and set the speech language.
        string cCultureName = "";

        try
        {
            if (string.IsNullOrEmpty(Globals.cLanguageSpeech))
            {
                cCultureName = Thread.CurrentThread.CurrentCulture.Name;
            }
        }
        catch (Exception)
        {
            cCultureName = "en-US";
        }
        //DisplayAlert("cCultureName", $"*{cCultureName}*", "OK");  // For testing.

        InitializeTextToSpeech(cCultureName);

        // Set focus to the editor.
        edtTextToCode.Focus();
    }

    // TitleView buttons clicked events.
    private async void OnPageAboutClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new PageAbout());
    }

    private async void OnPageScanClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new PageScan());
    }

    private async void OnPageSettingsClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new PageSettings());
    }

    // Set the editor properties for the selected format code.
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
                // Aztec.
                case 0:
                    edtTextToCode.MaxLength = 3832;
                    edtTextToCode.Keyboard = Keyboard.Default;
                    bgvBarcode.HeightRequest = 250;
                    bgvBarcode.WidthRequest = 250;
                    bgvBarcode.BarcodeMargin = 2;
                    bgvBarcode.Format = BarcodeFormat.Aztec;
                    break;

                // Codabar.
                case 1:
                    edtTextToCode.MaxLength = 43;
                    edtTextToCode.Keyboard = Keyboard.Default;
                    bgvBarcode.BarcodeMargin = 4;
                    bgvBarcode.Format = BarcodeFormat.Codabar;
                    break;

                // Code128.
                case 2:
                    edtTextToCode.MaxLength = 48;
                    edtTextToCode.Keyboard = Keyboard.Default;
                    bgvBarcode.BarcodeMargin = 20;
                    bgvBarcode.Format = BarcodeFormat.Code128;
                    break;

                // Code39.
                case 3:
                    edtTextToCode.MaxLength = 48;
                    edtTextToCode.Keyboard = Keyboard.Default;
                    bgvBarcode.BarcodeMargin = 4;
                    bgvBarcode.Format = BarcodeFormat.Code39;
                    break;

                // Code93.
                case 4:
                    edtTextToCode.MaxLength = 48;
                    edtTextToCode.Keyboard = Keyboard.Default;
                    bgvBarcode.BarcodeMargin = 4;
                    bgvBarcode.Format = BarcodeFormat.Code93;
                    break;

                // DataMatrix.
                case 5:
                    edtTextToCode.MaxLength = 2335;
                    edtTextToCode.Keyboard = Keyboard.Default;
                    bgvBarcode.HeightRequest = 250;
                    bgvBarcode.WidthRequest = 250;
                    bgvBarcode.BarcodeMargin = 2;
                    bgvBarcode.Format = BarcodeFormat.DataMatrix;
                    break;

                // Ean13.
                case 6:
                    edtTextToCode.MaxLength = 13;
                    edtTextToCode.Keyboard = Keyboard.Numeric;
                    bgvBarcode.BarcodeMargin = 4;
                    bgvBarcode.Format = BarcodeFormat.Ean13;
                    break;

                // Ean8.
                case 7:
                    edtTextToCode.MaxLength = 8;
                    edtTextToCode.Keyboard = Keyboard.Numeric;
                    bgvBarcode.BarcodeMargin = 4;
                    bgvBarcode.Format = BarcodeFormat.Ean8;
                    break;

                // Imb.
                case 8:
                    edtTextToCode.MaxLength = 2730;
                    edtTextToCode.Keyboard = Keyboard.Default;
                    bgvBarcode.BarcodeMargin = 0;
                    bgvBarcode.Format = BarcodeFormat.Imb;
                    break;

                // Itf.
                case 9:
                    edtTextToCode.MaxLength = 30;
                    edtTextToCode.Keyboard = Keyboard.Numeric;
                    bgvBarcode.BarcodeMargin = 8;
                    bgvBarcode.Format = BarcodeFormat.Itf;
                    break;

                // MaxiCode.
                case 10:
                    edtTextToCode.MaxLength = 93;
                    edtTextToCode.Keyboard = Keyboard.Default;
                    bgvBarcode.HeightRequest = 250;
                    bgvBarcode.WidthRequest = 250;
                    bgvBarcode.BarcodeMargin = 8;
                    bgvBarcode.Format = BarcodeFormat.MaxiCode;
                    break;

                // Msi.
                case 11:
                    edtTextToCode.MaxLength = 255;
                    edtTextToCode.Keyboard = Keyboard.Numeric;
                    bgvBarcode.BarcodeMargin = 10;
                    bgvBarcode.Format = BarcodeFormat.Msi;
                    break;

                // Pdf417.
                case 12:
                    edtTextToCode.MaxLength = 2710;
                    edtTextToCode.Keyboard = Keyboard.Default;
                    bgvBarcode.BarcodeMargin = 10;
                    bgvBarcode.Format = BarcodeFormat.Pdf417;
                    break;

                // PharmaCode.
                case 13:
                    edtTextToCode.MaxLength = 6;
                    edtTextToCode.Keyboard = Keyboard.Numeric;
                    bgvBarcode.BarcodeMargin = 0;
                    bgvBarcode.Format = BarcodeFormat.PharmaCode;
                    break;

                // Plessey.
                case 14:
                    edtTextToCode.MaxLength = 16;
                    edtTextToCode.Keyboard = Keyboard.Default;
                    bgvBarcode.BarcodeMargin = 8;
                    bgvBarcode.Format = BarcodeFormat.Plessey;
                    break;

                // QrCode.
                case 15:
                    edtTextToCode.MaxLength = 7089;
                    edtTextToCode.Keyboard = Keyboard.Default;
                    bgvBarcode.HeightRequest = 250;
                    bgvBarcode.WidthRequest = 250;
                    bgvBarcode.BarcodeMargin = 1;
                    bgvBarcode.Format = BarcodeFormat.QrCode;
                    break;

                // Rss14.
                case 16:
                    edtTextToCode.MaxLength = 14;
                    edtTextToCode.Keyboard = Keyboard.Numeric;
                    bgvBarcode.BarcodeMargin = 0;
                    bgvBarcode.Format = BarcodeFormat.Rss14;
                    break;

                // RssExpanded.
                case 17:
                    edtTextToCode.MaxLength = 74;
                    edtTextToCode.Keyboard = Keyboard.Default;
                    bgvBarcode.BarcodeMargin = 0;
                    bgvBarcode.Format = BarcodeFormat.RssExpanded;
                    break;

                // UpcA.
                case 18:
                    edtTextToCode.MaxLength = 12;
                    edtTextToCode.Keyboard = Keyboard.Numeric;
                    bgvBarcode.BarcodeMargin = 0;
                    bgvBarcode.Format = BarcodeFormat.UpcA;
                    break;

                // UpcE.
                case 19:
                    edtTextToCode.MaxLength = 8;
                    edtTextToCode.Keyboard = Keyboard.Numeric;
                    bgvBarcode.BarcodeMargin = 8;
                    bgvBarcode.Format = BarcodeFormat.UpcE;
                    break;

                // UpcEanExtension.
                case 20:
                    edtTextToCode.MaxLength = 2;
                    edtTextToCode.Keyboard = Keyboard.Numeric;
                    bgvBarcode.BarcodeMargin = 4;
                    bgvBarcode.Format = BarcodeFormat.UpcEanExtension;
                    break;
            }
        }
    }

    // OnGenerateCodeClicked event.
    private void OnGenerateCodeClicked(object sender, EventArgs e)
    {
        // Hide the keyboard.
        edtTextToCode.IsEnabled = false;
        edtTextToCode.IsEnabled = true;

        // Set the barcode colors.
        bgvBarcode.ForegroundColor = Color.FromArgb(Globals.cCodeColorFg);
        bgvBarcode.BackgroundColor = Color.FromArgb(Globals.cCodeColorBg);

        // Miscellaneous.
        bgvBarcode.Value = "";
        string cChecksum = "";
        //string cTextCode = "";

        if (string.IsNullOrEmpty(edtTextToCode.Text))
        {
            edtTextToCode.Focus();
            return;
        }

        string cTextToCode = edtTextToCode.Text.Trim();

        int nLenTextToCode = cTextToCode.Length;
        if (nLenTextToCode == 0)
        {
            return;
        }

        // Validate the text input and set the format.
        int selectedIndex = pckFormatCodeGenerator.SelectedIndex;

        if (selectedIndex != -1)
        {
            try
            {
                switch (selectedIndex)
                {
                    // Aztec.
                    case 0:
                        if (TestAllowedAsciiValues(1, 255, cTextToCode) == false)
                        {
                            return;
                        }

                        break;

                    // Codabar.
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

                    // Code128.
                    case 2:
                        if (TestAllowedAsciiValues(1, 127, cTextToCode) == false)
                        {
                            return;
                        }

                        break;

                    // Code39.
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

                    // Code93.
                    case 4:
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

                    // DataMatrix.
                    case 5:
                        if (TestAllowedAsciiValues(1, 255, cTextToCode) == false)
                        {
                            return;
                        }

                        break;

                    // Ean13.
                    case 6:
                        if (TestAllowedCharacters("0123456789", cTextToCode) == false)
                        {
                            return;
                        }

                        if (nLenTextToCode < 12 || nLenTextToCode > 13)
                        {
                            DisplayErrorMessageLength("12", "13");
                            return;
                        }

                        // Calculate, (correct) and add the checksum.
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

                    // Ean8.
                    case 7:
                        if (TestAllowedCharacters("0123456789", cTextToCode) == false)
                        {
                            return;
                        }

                        if (nLenTextToCode < 7 || nLenTextToCode > 8)
                        {
                            DisplayErrorMessageLength("7", "8");
                            return;
                        }

                        // Calculate, (correct) and add the checksum.
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

                    // Imb.
                    case 8:
                        DisplayMessageFormat("IMb");
                        return;

                        //break;

                    // Itf.
                    case 9:
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

                    // MaxiCode.
                    case 10:
                        DisplayMessageFormat("MaxiCode");
                        return;

                        //break;

                    // Msi.
                    case 11:
                        if (TestAllowedCharacters("0123456789", cTextToCode) == false)
                        {
                            return;
                        }

                        break;

                    // Pdf417.
                    case 12:
                        if (TestAllowedAsciiValues(1, 255, cTextToCode) == false)
                        {
                            return;
                        }

                        break;

                    // PharmaCode.
                    case 13:
                        DisplayMessageFormat("Pharmacode");
                        return;

                        //break;

                    // Plessey.
                    case 14:
                        cTextToCode = cTextToCode.ToUpper();

                        if (TestAllowedCharacters("0123456789ABCDEF", cTextToCode) == false)
                        {
                            return;
                        }

                        break;

                    // QrCode.
                    case 15:
                        break;

                    // Rss14.
                    case 16:
                        DisplayMessageFormat("RSS 14");
                        return;

                        //break;

                    // RssExpanded.
                    case 17:
                        DisplayMessageFormat("RSS Expanded");
                        return;

                        //break;

                    // UpcA.
                    case 18:
                        if (TestAllowedCharacters("0123456789", cTextToCode) == false)
                        {
                            return;
                        }

                        if (nLenTextToCode < 11 || nLenTextToCode > 12)
                        {
                            DisplayErrorMessageLength("11", "12");
                            return;
                        }

                        // Calculate, (correct) and add the checksum.
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

                    // UpcE.
                    case 19:
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

                        // Convert UPC-E to UPC-A code.
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

                        // Calculate and add the checksum of the UPC-A code.
                        cUpcA += CalculateChecksumEanUpcA(cUpcA);

                        // Add the checksum from the UPC-A code to the UPC-E code.
                        cTextToCode = string.Concat(cTextToCode.AsSpan(0, 7), cUpcA.AsSpan(11, 1));

                        if (nLenTextToCode == 8 && cChecksum != cTextToCode.Substring(7, 1))
                        {
                            DisplayErrorMessage(CodeLang.CheckDigitError_Text);
                        }

                        edtTextToCode.Text = cTextToCode;
                        //cTextCode = string.Concat(cTextToCode[..1], " ", cTextToCode.Substring(1, 6), " ", cTextToCode.Substring(7, 1));

                        break;

                    // UpcEanExtension.
                    case 20:
                        DisplayMessageFormat("UPC EAN Extension");
                        return;

                        //if (TestAllowedCharacters("0123456789", cTextToCode) == false)
                        //{
                        //    return;
                        //}

                        //break;
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
            // For testing crashes - DivideByZeroException.
            //int divByZero = 51 / int.Parse("0");

            bgvBarcode.Value = cTextToCode;
            
            btnShare.Text = $"{CodeLang.ButtonShare_Text} {pckFormatCodeGenerator.Items[selectedIndex]}";
            btnShare.IsEnabled = true;
        }
        catch (Exception ex)
        {
            var properties = new Dictionary<string, string> {
                { "File:", "MainPage.xaml.cs" },
                { "Method:", "OnGenerateCodeClicked" },
                { "BarcodeFormat:", Convert.ToString(bgvBarcode.Format) },
                { "BarcodeValue:", cTextToCode }
            };
            Crashes.TrackError(ex, properties);

            //if (Crashes.ShouldAwaitUserConfirmation())
            //{
            //    Crashes.TrackError(ex, properties);
            //}

            //Crashes.ShouldProcessErrorReport = (ErrorReport report) =>
            //{
            //    //DataLogger.Error("AppCenter process error");
            //    // Return true if the crash report should be processed, otherwise false.
            //    return true; 
            //};

            //if (bLogAlwaysSend)
            //{
            //    Crashes.TrackError(ex, properties);
            //}
            //else
            //{
            //    ConfirmationSendCrashLog();
            //}

            bgvBarcode.Value = "";

            RestartApplication(ex.Message);
        }
    }

    // Test for allowed characters.
    private bool TestAllowedCharacters(string cAllowedCharacters, string cTextToCode)
    {
        foreach (char cChar in cTextToCode)
        {
            bool bResult = cAllowedCharacters.Contains(cChar);

            if (bResult == false)
            {
                DisplayAlert(CodeLang.ErrorTitle_Text, $"{CodeLang.AllowedChar_Text}\n{cAllowedCharacters}\n\n{CodeLang.AllowedCharNot_Text} {cChar}", CodeLang.ButtonClose_Text);
                
                edtTextToCode.Focus();
                return false;
            }
        }

        return true;
    }

    // Test for allowed minimum and maximum ASCII values.
    private bool TestAllowedAsciiValues(int nMinAsciiValue, int nMaxAsciiValue, string cTextToCode)
    {
        foreach (char cChar in cTextToCode)
        {
            if (cChar < nMinAsciiValue || cChar > nMaxAsciiValue)
            {
                DisplayAlert(CodeLang.ErrorTitle_Text, $"{CodeLang.TextContainsChar_Text} {cChar}", CodeLang.ButtonClose_Text);
                
                edtTextToCode.Focus();
                return false;
            }
        }

        return true;
    }

    // Test start & end guards.
    private bool TestStartEndGuards(string cStartEndGuards, string cTextToCode)
    {
        int nPos;
        char cChar;

        // Control of start and end guards in the wrong place.
        for (nPos = 0; nPos < cTextToCode.Length; nPos++)
        {
            cChar = cTextToCode[nPos];

            if (cStartEndGuards.Contains(cChar) && nPos > 0 && nPos < cTextToCode.Length - 1)
            {
                DisplayAlert(CodeLang.ErrorTitle_Text, $"{CodeLang.GuardInvalidStartEnd_Text} {cChar}", CodeLang.ButtonClose_Text);
                
                edtTextToCode.Focus();
                return false;
            }
        }

        // Control of missing start or end guard.
        if (cStartEndGuards.Contains(cTextToCode[..1]) && cStartEndGuards.Contains(cTextToCode.Substring(cTextToCode.Length - 1, 1)) == false)
        {
            DisplayAlert(CodeLang.ErrorTitle_Text, CodeLang.GuardMissingEnd_Text, CodeLang.ButtonClose_Text);
            
            edtTextToCode.Focus();
            return false;
        }
        else if (cStartEndGuards.Contains(cTextToCode[..1]) == false && cStartEndGuards.Contains(cTextToCode.Substring(cTextToCode.Length - 1, 1)))
        {
            DisplayAlert(CodeLang.ErrorTitle_Text, CodeLang.GuardMissingStart_Text, CodeLang.ButtonClose_Text);
            
            edtTextToCode.Focus();
            return false;
        }

        return true;
    }

    // Reverse the characters in a string.
    private static string ReverseString(string cText)
    {
        char[] charArray = cText.ToCharArray();
        Array.Reverse(charArray);

        return string.Concat(charArray);
    }

    // Calculate the checksum of an EAN-13, EAN-8 and UPC-A code.
    private static string CalculateChecksumEanUpcA(string cTextToCode)
    {
        int nPos;
        int nPartialSum;
        int nChecksum = 0;
        int nCheckDigit = 0;

        // Loop over string.
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
            //DisplayAlert("nPartialSum", Convert.ToString(nPartialSum), "OK");

            nChecksum += nPartialSum;
        }
        //DisplayAlert("nChecksum", Convert.ToString(nChecksum), "OK");

        int nRemainder = nChecksum % 10;
        if (nRemainder != 0)
        {
            nCheckDigit = 10 - nRemainder;
        }
        //DisplayAlert("nCheckDigit", Convert.ToString(nCheckDigit), "OK");

        return Convert.ToString(nCheckDigit);
    }

    // OnClearCodeClicked event.
    private void OnClearCodeClicked(object sender, EventArgs e)
    {
        edtTextToCode.Text = "";
        bgvBarcode.Value = "";
        btnShare.Text = CodeLang.ButtonShare_Text;
        btnShare.IsEnabled = false;

        edtTextToCode.Focus();
    }

    // Display a message with no encoder available for format.
    private void DisplayMessageFormat(string cFormat)
    {
        DisplayAlert(CodeLang.FormatTitle_Text, $"{cFormat} {CodeLang.FormatNotSupported_Text}", CodeLang.ButtonClose_Text);

        edtTextToCode.Focus();
    }

    // Display an error message.
    private void DisplayErrorMessage(string cMessage)
    {
        DisplayAlert(CodeLang.ErrorTitle_Text, cMessage, CodeLang.ButtonClose_Text);

        edtTextToCode.Focus();
    }

    // Display an error message with minimum and maximum length.
    private void DisplayErrorMessageLength(string cMinLength, string cMaxLength)
    {
        DisplayAlert(CodeLang.ErrorTitle_Text, $"{CodeLang.CodeLengthPart1_Text} {cMinLength} {CodeLang.CodeLengthPart2_Text} {cMaxLength} {CodeLang.CodeLengthPart3_Text}", CodeLang.ButtonClose_Text);

        edtTextToCode.Focus();
    }

    // Display an error message and restart the application.
    private async void RestartApplication(string cErrorMessage)
    {
        await DisplayAlert(CodeLang.ErrorTitle_Text, $"{cErrorMessage}\n{CodeLang.RestartApp_Text}", CodeLang.ButtonClose_Text);

        //Application.Current.MainPage = new AppShell();
        Application.Current.MainPage = new NavigationPage(new MainPage());
    }

    // Show license using the Loaded event of the MainPage.xaml.
    private async void OnPageLoad(object sender, EventArgs e)
    {
        // Show license.
        if (bLicense == false)
        {
            bool bAnswer = await Application.Current.MainPage.DisplayAlert(CodeLang.LicenseTitle_Text, cLicense, CodeLang.Agree_Text, CodeLang.Disagree_Text);

            if (bAnswer)
            {
                Preferences.Default.Set("SettingLicense", true);
            }
            else
            {
#if IOS
                //Thread.CurrentThread.Abort();  // Not allowed in iOS.
                imgbtnAbout.IsEnabled = false;
                imgbtnScan.IsEnabled = false;
                imgbtnSettings.IsEnabled = false;
                btnGenerateCode.IsEnabled = false;

                await DisplayAlert(CodeLang.LicenseTitle_Text, CodeLang.CloseApplication_Text, CodeLang.ButtonClose_Text);
#else
                Application.Current.Quit();
#endif
            }
        }
    }

    // Set language using the Appearing event of the MainPage.xaml.
    private void OnPageAppearing(object sender, EventArgs e)
    {
        if (Globals.bLanguageChanged)
        {
            SetTextLanguage();
            Globals.bLanguageChanged = false;

            //DisplayAlert("Globals.bLanguageChanged", "true", "OK");  // For testing.
        }

        lblTextToSpeech.Text = Globals.GetIsoLanguageCode();
    }

    // Button share event - make screenshot of the barcode.
    private async void OnShareClicked(object sender, EventArgs e)
    {
        if (Screenshot.Default.IsCaptureSupported)
        {
            IScreenshotResult screen = await bgvBarcode.CaptureAsync();
            Stream stream = await screen.OpenReadAsync();

            SaveStreamAsFile(stream);
        }
    }

    // Save the barcode as an image file.
    private static async void SaveStreamAsFile(Stream inputStream)
    {
        // Save the image file.
        string cFileName = Path.Combine(FileSystem.CacheDirectory, "BarcodeGenerator.png");

        using (FileStream outputFileStream = new(cFileName, FileMode.Create))
        {
            inputStream.CopyTo(outputFileStream);
        }

        inputStream.Dispose();

        // Open the share interface to share the file.
        await OpenShareInterface(cFileName);
    }

    // Open the share interface.
    private static async Task OpenShareInterface(string cFile)
    {
        await Share.Default.RequestAsync(new ShareFileRequest
        {
            Title = "Barcode Generator",
            File = new ShareFile(cFile)
        });
    }

    // Put text in the chosen language in the controls.
    private void SetTextLanguage()
    {
        //Globals.cLanguage = "es";  // For testing.
        //App.Current.MainPage.DisplayAlert("Globals.cLanguage", Globals.cLanguage, "OK");  // For testing.

        // Set the current UI culture of the selected language.
        Globals.SetCultureSelectedLanguage();

        cLicense = $"{CodeLang.License_Text}\n\n{CodeLang.LicenseMit2_Text}";
        btnShare.Text = $"{CodeLang.ButtonShare_Text} {pckFormatCodeGenerator.Items[pckFormatCodeGenerator.SelectedIndex]}";

        //App.Current.MainPage.DisplayAlert(CodeLang.ErrorTitle_Text, Globals.cLanguage, "OK");  // For testing.
    }

    // Initialize text to speech and fill the the array with the speech languages.
    // .Country = KR ; .Id = ''  ; .Language = ko ; .Name = Korean (South Korea) ; 
    private async void InitializeTextToSpeech(string cCultureName)
    {
        // Initialize text to speech.
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
            await DisplayAlert(CodeLang.ErrorTitle_Text, $"{ex.Message}\n\n{CodeLang.TextToSpeechError_Text}", CodeLang.ButtonClose_Text);
            return;
        }
       
        lblTextToSpeech.IsVisible = true;
        imgbtnTextToSpeech.IsVisible = true;
        Globals.bLanguageLocalesExist = true;

        // Put the locales in the array and sort the array.
        Globals.cLanguageLocales = new string[nTotalItems];
        int nItem = 0;

        foreach (var l in locales)
        {
            Globals.cLanguageLocales[nItem] = $"{l.Language}-{l.Country} {l.Name}";
            nItem++;
        }

        Array.Sort(Globals.cLanguageLocales);

        // Search for the language after a first start or reset of the application.
        if (string.IsNullOrEmpty(Globals.cLanguageSpeech))
        {
            SearchArrayWithSpeechLanguages(cCultureName);
        }
        //await DisplayAlert("Globals.cLanguageSpeech", Globals.cLanguageSpeech, "OK");  // For testing.
        
        lblTextToSpeech.Text = Globals.GetIsoLanguageCode();
    }
    
    // Search for the language after a first start or reset of the application.
    private void SearchArrayWithSpeechLanguages(string cCultureName)
    {
        try
        {
            int nTotalItems = Globals.cLanguageLocales.Length;

            for (int nItem = 0; nItem < nTotalItems; nItem++)
            {
                if (Globals.cLanguageLocales[nItem].StartsWith(cCultureName))
                {
                    Globals.cLanguageSpeech = Globals.cLanguageLocales[nItem];
                    break;
                }
            }

            // If the language is not found try it with the language (Globals.cLanguage) of the user setting for this app.
            if (string.IsNullOrEmpty(Globals.cLanguageSpeech))
            {
                for (int nItem = 0; nItem < nTotalItems; nItem++)
                {
                    if (Globals.cLanguageLocales[nItem].StartsWith(Globals.cLanguage))
                    {
                        Globals.cLanguageSpeech = Globals.cLanguageLocales[nItem];
                        break;
                    }
                }
            }

            // If the language is still not found use the first language in the array.
            if (string.IsNullOrEmpty(Globals.cLanguageSpeech))
            {
                Globals.cLanguageSpeech = Globals.cLanguageLocales[0];
            }
        }
        catch (Exception ex)
        {
            DisplayAlert(CodeLang.ErrorTitle_Text, ex.Message, CodeLang.ButtonClose_Text);
        }
    }

    // Search for the language after a first start or reset of the application.
    //private void SearchArrayWithSpeechLanguages(string cCultureName)
    //{
    //    // Does not works on Android 13 (works on Android 8) - App is closing after starting.
    //    // There is a problem with the StartsWith->cCultureName:
    //    // string value = Array.Find(cLanguageLocales, element => element.StartsWith(cCultureName, StringComparison.OrdinalIgnoreCase));

    //    if (string.IsNullOrEmpty(Globals.cLanguageSpeech))
    //    {
    //        try
    //        {
    //            string value = Array.Find(cLanguageLocales, element => element.StartsWith(cCultureName, StringComparison.OrdinalIgnoreCase));
    //            cLanguageSpeech = value;
    //        }
    //        catch (Exception ex)
    //        {
    //            DisplayAlert(CodeLang.ErrorTitle_Text, ex.Message, CodeLang.ButtonClose_Text);
    //            cLanguageSpeech = cLanguageLocales[0];
    //        }
    //    }
    //}

    // Button text to speech event.
    private async void OnTextToSpeechClicked(object sender, EventArgs e)
    {
        // Cancel the text to speech.
        if (bTextToSpeechIsBusy)
        {
            if (cts?.IsCancellationRequested ?? true)
                return;

            cts.Cancel();
            imgbtnTextToSpeech.Source = Globals.cImageTextToSpeech;
            return;
        }

        // Start with the text to speech.
        if (edtTextToCode.Text != null && edtTextToCode.Text != "")
        {
            bTextToSpeechIsBusy = true;
            imgbtnTextToSpeech.Source = Globals.cImageTextToSpeechCancel;

            try
            {
                cts = new CancellationTokenSource();

                SpeechOptions options = new()
                {
                    Locale = locales.Single(l => $"{l.Language}-{l.Country} {l.Name}" == Globals.cLanguageSpeech)
                };

                await TextToSpeech.Default.SpeakAsync(edtTextToCode.Text, options, cancelToken: cts.Token);
                bTextToSpeechIsBusy = false;
            }
            catch (Exception ex)
            {
                await DisplayAlert(CodeLang.ErrorTitle_Text, ex.Message, CodeLang.ButtonClose_Text);
            }

            imgbtnTextToSpeech.Source = Globals.cImageTextToSpeech;
        }
    }

    // Paste text from the clipboard clicked event.
    private async void OnPasteFromClipboardClicked(object sender, EventArgs e)
    {
        if (Clipboard.Default.HasText)
        {
            try
            {
                string cTextToPaste = await Clipboard.Default.GetTextAsync();

                if (cTextToPaste.Length > edtTextToCode.MaxLength)
                {
                    cTextToPaste = cTextToPaste[..edtTextToCode.MaxLength];
                }

                edtTextToCode.Text = cTextToPaste;
            }
            catch (Exception ex)
            {
                var properties = new Dictionary<string, string> {
                    { "File:", "MainPage.xaml.cs" },
                    { "Method:", "OnPasteFromClipboardClicked" },
                    { "BarcodeFormat:", Convert.ToString(bgvBarcode.Format) }
                };
                Crashes.TrackError(ex, properties);

                await DisplayAlert(CodeLang.ErrorTitle_Text, ex.Message, CodeLang.ButtonClose_Text);
            }          
        }
    }

    // Crash log confirmation.
    private async void ConfirmationSendCrashLog()
    {
        // Using the DisplayActionSheet with 3 choices.
        string cAction = await DisplayActionSheet(CodeLang.LogTitle_Text, null, null, CodeLang.LogSend_Text, CodeLang.LogAlwaysSend_Text, CodeLang.LogDontSend_Text);

        if (cAction == CodeLang.LogSend_Text)
        {
            Crashes.NotifyUserConfirmation(UserConfirmation.Send);
        }
        else if (cAction == CodeLang.LogAlwaysSend_Text)
        {
            Crashes.NotifyUserConfirmation(UserConfirmation.AlwaysSend);
            Preferences.Default.Set("SettingLogAlwaysSend", true);
        }
        else if (cAction == CodeLang.LogDontSend_Text)
        {
            Crashes.NotifyUserConfirmation(UserConfirmation.DontSend);
        }

        // Using the DisplayAlert with 2 choices.
        //bool bAction = await DisplayAlert(CodeLang.LogTitle_Text, CodeLang.LogMessage_Text, CodeLang.LogSend_Text, CodeLang.LogDontSend_Text);

        //if (bAction)
        //{
        //    Crashes.NotifyUserConfirmation(UserConfirmation.Send);
        //}
        //else
        //{
        //    Crashes.NotifyUserConfirmation(UserConfirmation.DontSend);
        //}
    }
}

/*
How to convert a UPC-E code back to UPC-A ?
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
*/
