// Program .....: BarcodeGenerator.sln
// Author ......: Geert Geerits - E-mail: geertgeerits@gmail.com
// Copyright ...: (C) 2022-2022
// Version .....: 1.0.24
// Date ........: 2022-12-22 (YYYY-MM-DD)
// Language ....: Microsoft Visual Studio 2022: .NET MAUI C# 11.0
// Description .: Barcode Generator
// Note ........: zxing:CameraBarcodeReaderView -> ex. WidthRequest="300" -> Grid RowDefinitions="400" (300 x 1.3333) = 3:4 aspect ratio
// Dependencies : NuGet Package: ZXing.Net.Maui by Redth version 0.3.0-preview.1 ; https://github.com/redth/ZXing.Net.Maui
//                NuGet Package: ZXing.Net.Maui.Controls by Redth version 0.3.0-preview.1

using BarcodeGenerator.Resources.Languages;
using System.Globalization;
using ZXing.Net.Maui;

namespace BarcodeGenerator;

public partial class MainPage : ContentPage
{
    // Global variables for all pages part of Barcode Generator.
    public static string cTheme;
    public static int nFormatGeneratorIndex;
    public static int nFormatScannerIndex;
    public static string cCodeColorFg;
    public static string cCodeColorBg;
    public static string cLanguage;
    public static bool bLanguageChanged = false;

    // Local variables.
    private string cButtonShare;
    private string cButtonClose;
    private string cErrorTitle;
    private string cAllowedChar;
    private string cAllowedCharNot;
    private string cTextContainsChar;
    private string cGuardInvalidStartEnd;
    private string cGuardMissingEnd;
    private string cGuardMissingStart;
    private string cFirstNumber0;
    private string cCheckDigitError;
    private string cLengthInputEven;
    private string cFormatTitle;
    private string cFormatNotSupported;
    private string cCodeLengthPart1;
    private string cCodeLengthPart2;
    private string cCodeLengthPart3;
    private string cRestartApp;
    private string cLicenseTitle;
    private string cLicense;
    private string cAgree;
    private string cDisagree;
    private readonly bool bLicense;
    private string cCloseApplication;

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
        cTheme = Preferences.Default.Get("SettingTheme", "System");
        nFormatGeneratorIndex = Preferences.Default.Get("SettingFormatGeneratorIndex", 15);
        nFormatScannerIndex = Preferences.Default.Get("SettingFormatScannerIndex", 21);
        cCodeColorFg = Preferences.Default.Get("SettingCodeColorFg", "FF000000");
        cCodeColorBg = Preferences.Default.Get("SettingCodeColorBg", "FFFFFFFF");
        cLanguage = Preferences.Default.Get("SettingLanguage", "");
        bLicense = Preferences.Default.Get("SettingLicense", false);

        // Set the theme.
        if (cTheme == "Light")
        {
            Application.Current.UserAppTheme = AppTheme.Light;
        }
        else if (cTheme == "Dark")
        {
            Application.Current.UserAppTheme = AppTheme.Dark;
        }
        else
        {
            Application.Current.UserAppTheme = AppTheme.Unspecified;
        }

        // Set the barcode list and the current default barcode format in the picker for the barcode generator.
        pckFormatCodeGenerator.ItemsSource = MainPage.GetFormatCodeListGenerator();

        if (nFormatGeneratorIndex < 0 || nFormatGeneratorIndex > 20)
        {
            // Default format code = QrCode.
            pckFormatCodeGenerator.SelectedIndex = 15;
        }
        else
        {
            // Set the format barcode to the saved code.
            pckFormatCodeGenerator.SelectedIndex = nFormatGeneratorIndex;
        }

        // Get and set the system OS user language.
        try
        {
            if (cLanguage == "")
            {
                cLanguage = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
            }
        }
        catch (Exception)
        {
            cLanguage = "en";
        }

        SetTextLanguage();

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
            bgvBarcode.MaximumWidthRequest = 700;
            bgvBarcode.HorizontalOptions = LayoutOptions.Fill;

            lblTextCode.Text = "";
            btnShare.Text = cButtonShare;
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
                    break;

                // Codabar.
                case 1:
                    edtTextToCode.MaxLength = 43;
                    edtTextToCode.Keyboard = Keyboard.Default;
                    bgvBarcode.BarcodeMargin = 4;
                    break;

                // Code128.
                case 2:
                    edtTextToCode.MaxLength = 48;
                    edtTextToCode.Keyboard = Keyboard.Default;
                    bgvBarcode.BarcodeMargin = 20;
                    break;

                // Code39.
                case 3:
                    edtTextToCode.MaxLength = 48;
                    edtTextToCode.Keyboard = Keyboard.Default;
                    bgvBarcode.BarcodeMargin = 4;
                    break;

                // Code93.
                case 4:
                    edtTextToCode.MaxLength = 48;
                    edtTextToCode.Keyboard = Keyboard.Default;
                    bgvBarcode.BarcodeMargin = 4;
                    break;

                // DataMatrix.
                case 5:
                    edtTextToCode.MaxLength = 2335;
                    edtTextToCode.Keyboard = Keyboard.Default;
                    bgvBarcode.HeightRequest = 250;
                    bgvBarcode.WidthRequest = 250;
                    bgvBarcode.BarcodeMargin = 2;
                    break;

                // Ean13.
                case 6:
                    edtTextToCode.MaxLength = 13;
                    edtTextToCode.Keyboard = Keyboard.Numeric;
                    bgvBarcode.BarcodeMargin = 4;
                    break;

                // Ean8.
                case 7:
                    edtTextToCode.MaxLength = 8;
                    edtTextToCode.Keyboard = Keyboard.Numeric;
                    bgvBarcode.BarcodeMargin = 4;
                    break;

                // Imb.
                case 8:
                    edtTextToCode.MaxLength = 2730;
                    edtTextToCode.Keyboard = Keyboard.Default;
                    bgvBarcode.BarcodeMargin = 0;
                    break;

                // Itf.
                case 9:
                    edtTextToCode.MaxLength = 30;
                    edtTextToCode.Keyboard = Keyboard.Numeric;
                    bgvBarcode.BarcodeMargin = 8;
                    break;

                // MaxiCode.
                case 10:
                    edtTextToCode.MaxLength = 93;
                    edtTextToCode.Keyboard = Keyboard.Default;
                    bgvBarcode.HeightRequest = 250;
                    bgvBarcode.WidthRequest = 250;
                    bgvBarcode.BarcodeMargin = 8;
                    break;

                // Msi.
                case 11:
                    edtTextToCode.MaxLength = 255;
                    edtTextToCode.Keyboard = Keyboard.Numeric;
                    bgvBarcode.BarcodeMargin = 10;
                    break;

                // Pdf417.
                case 12:
                    edtTextToCode.MaxLength = 2710;
                    edtTextToCode.Keyboard = Keyboard.Default;
                    bgvBarcode.BarcodeMargin = 10;
                    break;

                // PharmaCode.
                case 13:
                    edtTextToCode.MaxLength = 6;
                    edtTextToCode.Keyboard = Keyboard.Numeric;
                    bgvBarcode.BarcodeMargin = 0;
                    break;

                // Plessey.
                case 14:
                    edtTextToCode.MaxLength = 16;
                    edtTextToCode.Keyboard = Keyboard.Default;
                    bgvBarcode.BarcodeMargin = 8;
                    break;

                // QrCode.
                case 15:
                    edtTextToCode.MaxLength = 7089;
                    edtTextToCode.Keyboard = Keyboard.Default;
                    bgvBarcode.HeightRequest = 250;
                    bgvBarcode.WidthRequest = 250;
                    bgvBarcode.BarcodeMargin = 1;
                    break;

                // Rss14.
                case 16:
                    edtTextToCode.MaxLength = 14;
                    edtTextToCode.Keyboard = Keyboard.Numeric;
                    bgvBarcode.BarcodeMargin = 0;
                    break;

                // RssExpanded.
                case 17:
                    edtTextToCode.MaxLength = 74;
                    edtTextToCode.Keyboard = Keyboard.Default;
                    bgvBarcode.BarcodeMargin = 0;
                    break;

                // UpcA.
                case 18:
                    edtTextToCode.MaxLength = 12;
                    edtTextToCode.Keyboard = Keyboard.Numeric;
                    bgvBarcode.BarcodeMargin = 0;
                    break;

                // UpcE.
                case 19:
                    edtTextToCode.MaxLength = 8;
                    edtTextToCode.Keyboard = Keyboard.Numeric;
                    bgvBarcode.BarcodeMargin = 8;
                    break;

                // UpcEanExtension.
                case 20:
                    edtTextToCode.MaxLength = 2;
                    edtTextToCode.Keyboard = Keyboard.Numeric;
                    bgvBarcode.BarcodeMargin = 4;
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
        bgvBarcode.ForegroundColor = Color.FromArgb(cCodeColorFg);
        bgvBarcode.BackgroundColor = Color.FromArgb(cCodeColorBg);

        // Miscellaneous.
        bgvBarcode.Value = "";
        string cChecksum = "";
        string cTextCode = "";

        if (edtTextToCode.Text == null)
        {
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

                        bgvBarcode.Format = BarcodeFormat.Aztec;
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

                        bgvBarcode.Format = BarcodeFormat.Codabar;
                        break;

                    // Code128.
                    case 2:
                        if (TestAllowedAsciiValues(1, 127, cTextToCode) == false)
                        {
                            return;
                        }

                        bgvBarcode.Format = BarcodeFormat.Code128;
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

                        bgvBarcode.Format = BarcodeFormat.Code39;
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

                        bgvBarcode.Format = BarcodeFormat.Code93;
                        break;

                    // DataMatrix.
                    case 5:
                        if (TestAllowedAsciiValues(1, 255, cTextToCode) == false)
                        {
                            return;
                        }

                        bgvBarcode.Format = BarcodeFormat.DataMatrix;
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
                            DisplayErrorMessage(cCheckDigitError);
                        }

                        edtTextToCode.Text = cTextToCode;
                        cTextCode = string.Concat(cTextToCode[..1], " ", cTextToCode.Substring(1, 6), " ", cTextToCode.Substring(7, 6));

                        bgvBarcode.Format = BarcodeFormat.Ean13;
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
                            DisplayErrorMessage(cCheckDigitError);
                        }

                        edtTextToCode.Text = cTextToCode;
                        cTextCode = string.Concat(cTextToCode[..4], " ", cTextToCode.Substring(4, 4));

                        bgvBarcode.Format = BarcodeFormat.Ean8;
                        break;

                    // Imb.
                    case 8:
                        DisplayMessageFormat("IMb");
                        return;

                        //bgvBarcode.Format = BarcodeFormat.Imb;
                        //break;

                    // Itf.
                    case 9:
                        if (TestAllowedCharacters("0123456789", cTextToCode) == false)
                        {
                            return;
                        }

                        if (nLenTextToCode % 2 != 0)
                        {
                            DisplayErrorMessage(cLengthInputEven);
                            return;
                        }

                        bgvBarcode.Format = BarcodeFormat.Itf;
                        break;

                    // MaxiCode.
                    case 10:
                        DisplayMessageFormat("MaxiCode");
                        return;

                        //bgvBarcode.Format = BarcodeFormat.MaxiCode;
                        //break;

                    // Msi.
                    case 11:
                        if (TestAllowedCharacters("0123456789", cTextToCode) == false)
                        {
                            return;
                        }

                        bgvBarcode.Format = BarcodeFormat.Msi;
                        break;

                    // Pdf417.
                    case 12:
                        if (TestAllowedAsciiValues(1, 255, cTextToCode) == false)
                        {
                            return;
                        }

                        bgvBarcode.Format = BarcodeFormat.Pdf417;
                        break;

                    // PharmaCode.
                    case 13:
                        DisplayMessageFormat("Pharmacode");
                        return;

                        //bgvBarcode.Format = BarcodeFormat.PharmaCode;
                        //break;

                    // Plessey.
                    case 14:
                        cTextToCode = cTextToCode.ToUpper();

                        if (TestAllowedCharacters("0123456789ABCDEF", cTextToCode) == false)
                        {
                            return;
                        }

                        bgvBarcode.Format = BarcodeFormat.Plessey;
                        break;

                    // QrCode.
                    case 15:
                        bgvBarcode.Format = BarcodeFormat.QrCode;
                        break;

                    // Rss14.
                    case 16:
                        DisplayMessageFormat("RSS 14");
                        return;

                        //bgvBarcode.Format = BarcodeFormat.Rss14;
                        //break;

                    // RssExpanded.
                    case 17:
                        DisplayMessageFormat("RSS Expanded");
                        return;

                        //bgvBarcode.Format = BarcodeFormat.RssExpanded;
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
                            DisplayErrorMessage(cCheckDigitError);
                        }

                        edtTextToCode.Text = cTextToCode;
                        cTextCode = string.Concat(cTextToCode[..1], " ", cTextToCode.Substring(1, 5), " ", cTextToCode.Substring(6, 5), " ", cTextToCode.Substring(11, 1));

                        bgvBarcode.Format = BarcodeFormat.UpcA;
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
                            DisplayErrorMessage(cFirstNumber0);
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

                        cUpcA = "0" + cUpcA;

                        // Calculate and add the checksum of the UPC-A code.
                        cUpcA += CalculateChecksumEanUpcA(cUpcA);

                        // Add the checksum from the UPC-A code to the UPC-E code.
                        cTextToCode = string.Concat(cTextToCode.AsSpan(0, 7), cUpcA.AsSpan(11, 1));

                        if (nLenTextToCode == 8 && cChecksum != cTextToCode.Substring(7, 1))
                        {
                            DisplayErrorMessage(cCheckDigitError);
                        }

                        edtTextToCode.Text = cTextToCode;
                        cTextCode = string.Concat(cTextToCode[..1], " ", cTextToCode.Substring(1, 6), " ", cTextToCode.Substring(7, 1));

                        bgvBarcode.Format = BarcodeFormat.UpcE;
                        break;

                    // UpcEanExtension.
                    case 20:
                        DisplayMessageFormat("UPC EAN Extension");
                        return;

                        //bgvBarcode.Format = BarcodeFormat.UpcEanExtension;
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
            bgvBarcode.Value = cTextToCode;
            
            btnShare.Text = cButtonShare + " " + pckFormatCodeGenerator.Items[selectedIndex];
            btnShare.IsEnabled = true;

            if (cTextCode == "")
            {
                lblTextCode.Text = cTextToCode;
            }
            else
            {
                lblTextCode.Text = cTextCode;
            }
        }

        catch (Exception ex)
        {
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
                DisplayAlert(cErrorTitle, cAllowedChar + "\n" + cAllowedCharacters + "\n\n" + cAllowedCharNot + " " + cChar, cButtonClose);
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
                DisplayAlert(cErrorTitle, cTextContainsChar + " " + cChar, cButtonClose);
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
                DisplayAlert(cErrorTitle, cGuardInvalidStartEnd + " " + cChar, cButtonClose);
                edtTextToCode.Focus();

                return false;
            }
        }

        // Control of missing start or end guard.
        if (cStartEndGuards.Contains(cTextToCode[..1]) && cStartEndGuards.Contains(cTextToCode.Substring(cTextToCode.Length - 1, 1)) == false)
            {
            DisplayAlert(cErrorTitle, cGuardMissingEnd, cButtonClose);
            edtTextToCode.Focus();

            return false;
        }
        else if (cStartEndGuards.Contains(cTextToCode[..1]) == false && cStartEndGuards.Contains(cTextToCode.Substring(cTextToCode.Length - 1, 1)))
            {
            DisplayAlert(cErrorTitle, cGuardMissingStart, cButtonClose);
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
        lblTextCode.Text = "";
        btnShare.Text = cButtonShare;
        btnShare.IsEnabled = false;

        edtTextToCode.Focus();
    }

    // Display a message with no encoder available for format.
    private void DisplayMessageFormat(string cFormat)
    {
        DisplayAlert(cFormatTitle, cFormat + " " + cFormatNotSupported, cButtonClose);

        edtTextToCode.Focus();
    }

    // Display an error message.
    private void DisplayErrorMessage(string cMessage)
    {
        DisplayAlert(cErrorTitle, cMessage, cButtonClose);

        edtTextToCode.Focus();
    }

    // Display an error message with minimum and maximum length.
    private void DisplayErrorMessageLength(string cMinLength, string cMaxLength)
    {
        DisplayAlert(cErrorTitle, cCodeLengthPart1 + " " + cMinLength + " " + cCodeLengthPart2 + " " + cMaxLength + " " + cCodeLengthPart3 , cButtonClose);

        edtTextToCode.Focus();
    }

    // Display an error message and restart the application.
    private async void RestartApplication(string cErrorMessage)
    {
        await DisplayAlert(cErrorTitle, cErrorMessage + "\n" + cRestartApp, cButtonClose);

        //Application.Current.MainPage = new AppShell();
        Application.Current.MainPage = new NavigationPage(new MainPage());
    }

    // Show license using the Loaded event of the MainPage.xaml.
    private async void OnPageLoad(object sender, EventArgs e)
    {
        // Show license.
        if (bLicense == false)
        {
            bool bAnswer = await Application.Current.MainPage.DisplayAlert(cLicenseTitle, cLicense, cAgree, cDisagree);

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
                imgbtnSettings.IsEnabled= false;
                btnGenerateCode.IsEnabled = false;

                await DisplayAlert(cLicenseTitle, cCloseApplication, cButtonClose);
#else
                Application.Current.Quit();
#endif
            }
        }
    }

    // Set language using the Appearing event of the MainPage.xaml.
    private void OnPageAppearing(object sender, EventArgs e)
    {
        if (bLanguageChanged)
        {
            SetTextLanguage();
            bLanguageChanged = false;

            //DisplayAlert("bLanguageChanged", "true", "OK");  // For testing.
        }
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

        using (FileStream outputFileStream = new FileStream(cFileName, FileMode.Create))
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
        //cLanguage = "es";  // For testing.
        //App.Current.MainPage.DisplayAlert("cLanguage", cLanguage, "OK");  // For testing.

        // Set the current UI culture of the selected language.
        SetCultureSelectedLanguage();

        lblTitle.Text = CodeLang.BarcodeGenerator_Text;
        lblFormatCode.Text = CodeLang.FormatCode_Text;
        lblTextToEncode.Text = CodeLang.TextToEncode_Text;
        btnGenerateCode.Text = CodeLang.GenerateCode_Text;
        btnClearCode.Text = CodeLang.ClearCode_Text;
        btnShare.Text = CodeLang.ButtonShare_Text;

        cButtonShare = CodeLang.ButtonShare_Text;
        cButtonClose = CodeLang.ButtonClose_Text;
        cErrorTitle = CodeLang.ErrorTitle_Text;
        cAllowedChar = CodeLang.AllowedChar_Text;
        cAllowedCharNot = CodeLang.AllowedCharNot_Text;
        cTextContainsChar = CodeLang.TextContainsChar_Text;
        cGuardInvalidStartEnd = CodeLang.GuardInvalidStartEnd_Text;
        cGuardMissingEnd = CodeLang.GuardMissingEnd_Text;
        cGuardMissingStart = CodeLang.GuardMissingStart_Text;
        cFirstNumber0 = CodeLang.FirstNumber0_Text;
        cCheckDigitError = CodeLang.CheckDigitError_Text;
        cLengthInputEven = CodeLang.LengthInputEven_Text;
        cFormatTitle = CodeLang.FormatTitle_Text;
        cFormatNotSupported = CodeLang.FormatNotSupported_Text;
        cCodeLengthPart1 = CodeLang.CodeLengthPart1_Text;
        cCodeLengthPart2 = CodeLang.CodeLengthPart2_Text;
        cCodeLengthPart3 = CodeLang.CodeLengthPart3_Text;
        cRestartApp = CodeLang.RestartApp_Text;
        cLicenseTitle = CodeLang.LicenseTitle_Text;
        cLicense = CodeLang.License_Text + "\n\n" + CodeLang.LicenseMit2_Text;
        cAgree = CodeLang.Agree_Text;
        cDisagree = CodeLang.Disagree_Text;
        cCloseApplication = CodeLang.CloseApplication_Text;

        //App.Current.MainPage.DisplayAlert(cErrorTitleText, cLanguage, cButtonCloseText);  // For testing.
    }

    // Set the current UI culture of the selected language.
    public static void SetCultureSelectedLanguage()
    {
        try
        {
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(cLanguage);
        }
        catch
        {
            // Do nothing.
        }
    }

    // Put the generator barcode formats in a List string.
    public static List<string> GetFormatCodeListGenerator()
    {
        return new List<string>
        {
            "Aztec",
            "Codabar",
            "Code 128",
            "Code 39",
            "Code 93",
            "Data Matrix",
            "EAN-13",
            "EAN-8",
            "(IMb (Intelligent Mail))",
            "ITF (Interleaved 2 of 5)",
            "(MaxiCode)",
            "MSI (Modified Plessey)",
            "PDF417",
            "(Pharmacode)",
            "Plessey",
            "QR Code",
            "(RSS 14)",
            "(RSS Expanded)",
            "UPC-A",
            "UPC-E",
            "(UPC EAN Extension)"
        };
    }

    // Put the scanner barcode formats in a List string.
    public static List<string> GetFormatCodeListScanner()
    {
        return new List<string>
        {
            "Aztec",
            "Codabar",
            "Code 128",
            "Code 39",
            "Code 93",
            "Data Matrix",
            "EAN-13",
            "EAN-8",
            "(IMb (Intelligent Mail))",
            "ITF (Interleaved 2 of 5)",
            "(MaxiCode)",
            "MSI (Modified Plessey)",
            "PDF417",
            "(Pharmacode)",
            "Plessey",
            "QR Code",
            "RSS 14",
            "RSS Expanded",
            "UPC-A",
            "UPC-E",
            "(UPC EAN Extension)",
            CodeLang.AllCodes_Text
        };
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
