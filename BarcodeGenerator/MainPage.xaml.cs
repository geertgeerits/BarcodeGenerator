// Program .....: CodeGenerator.sln
// Author ......: Geert Geerits - E-mail: geertgeerits@gmail.com
// Copyright ...: (C) 2022-2022
// Version .....: 1.0.15 Beta
// Date ........: 2022-10-02 (YYYY-MM-DD)
// Language ....: Microsoft Visual Studio 2022: .NET MAUI C# 10.0
// Description .: Code generator for barcordes
// Dependencies : NuGet Package: ZXing.Net.Maui by Redth v0.1.0-preview.7 ; https://github.com/redth/ZXing.Net.Maui

using ZXing.Net.Maui;

namespace BarcodeGenerator;

public partial class MainPage : ContentPage
{
    // Global variables for all pages part of Barcode Generator.
    public static string cTheme;
    public static int nFormatIndex;

    // Local variables.
    private bool bLicense;
    private readonly string cMessageErrorInCode = "There is an error in the code you entered.\nThe check digit was modified.";

    public MainPage()
    {
        try
        {
            InitializeComponent();
        }
        catch (Exception ex)
        {
            DisplayAlert("InitializeComponent MainPage", ex.Message, "OK");
            return;
        }

        // Get the saved settings.
        cTheme = Preferences.Get("SettingTheme", "System");
        nFormatIndex = Preferences.Get("SettingFormatIndex", 14);
        bLicense = Preferences.Get("SettingLicense", false);
        //bLicense = false;  // For testing.

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

        // Set the format barcode.
        if (nFormatIndex < 1 || nFormatIndex > 19)
        {
            // Default format code = QrCode.
            PckFormatCode.SelectedIndex = 14;
        }
        else
        {
            // Set the format barcode to the saved code.
            PckFormatCode.SelectedIndex = nFormatIndex;
        }

        edtTextToCode.Focus();
    }

    // TitleView buttons clicked events.
    private async void OnPageAboutClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new PageAbout());
    }

    private async void OnPageSettingsClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new PageSettings());
    }

    // Set the editor properties for the selected format code.
    void OnPickerFormatCodeChanged(object sender, EventArgs e)
    {
        //bgvCode.IsVisible = false;

        var picker = (Picker)sender;
        int selectedIndex = picker.SelectedIndex;

        if (selectedIndex != -1)
        {
            bgvCode.Value = "";
            lblTextCode.Text = "";
            btnShare.Text = "Share";
            btnShare.IsEnabled = false;

            switch (selectedIndex)
            {
                // Aztec.
                case 0:
                    edtTextToCode.MaxLength = 3832;
                    edtTextToCode.Keyboard = Keyboard.Default;
                    bgvCode.BarcodeMargin = 0;
                    break;

                // Codabar.
                case 1:
                    edtTextToCode.MaxLength = 43;
                    edtTextToCode.Keyboard = Keyboard.Default;
                    bgvCode.BarcodeMargin = 4;
                    break;

                // Code128.
                case 2:
                    edtTextToCode.MaxLength = 48;
                    edtTextToCode.Keyboard = Keyboard.Default;
                    bgvCode.BarcodeMargin = 20;
                    break;

                // Code39.
                case 3:
                    edtTextToCode.MaxLength = 48;
                    edtTextToCode.Keyboard = Keyboard.Default;
                    bgvCode.BarcodeMargin = 4;
                    break;

                // Code93.
                case 4:
                    edtTextToCode.MaxLength = 48;
                    edtTextToCode.Keyboard = Keyboard.Default;
                    bgvCode.BarcodeMargin = 4;
                    break;

                // DataMatrix.
                case 5:
                    edtTextToCode.MaxLength = 3116;
                    edtTextToCode.Keyboard = Keyboard.Default;
                    bgvCode.BarcodeMargin = 4;
                    break;

                // Ean13.
                case 6:
                    edtTextToCode.MaxLength = 13;
                    edtTextToCode.Keyboard = Keyboard.Numeric;
                    bgvCode.BarcodeMargin = 4;
                    break;

                // Ean8.
                case 7:
                    edtTextToCode.MaxLength = 8;
                    edtTextToCode.Keyboard = Keyboard.Numeric;
                    bgvCode.BarcodeMargin = 4;
                    break;

                // Imb.
                case 8:
                    edtTextToCode.MaxLength = 2730;
                    edtTextToCode.Keyboard = Keyboard.Default;
                    bgvCode.BarcodeMargin = 0;
                    break;

                // Itf.
                case 9:
                    edtTextToCode.MaxLength = 30;
                    edtTextToCode.Keyboard = Keyboard.Numeric;
                    bgvCode.BarcodeMargin = 8;
                    break;

                // Msi.
                case 10:
                    edtTextToCode.MaxLength = 255;
                    edtTextToCode.Keyboard = Keyboard.Numeric;
                    bgvCode.BarcodeMargin = 10;
                    break;

                // Pdf417.
                case 11:
                    edtTextToCode.MaxLength = 2710;
                    edtTextToCode.Keyboard = Keyboard.Default;
                    bgvCode.BarcodeMargin = 10;
                    break;

                // PharmaCode.
                case 12:
                    edtTextToCode.MaxLength = 6;
                    edtTextToCode.Keyboard = Keyboard.Numeric;
                    bgvCode.BarcodeMargin = 0;
                    break;

                // Plessey.
                case 13:
                    edtTextToCode.MaxLength = 16;
                    edtTextToCode.Keyboard = Keyboard.Default;
                    bgvCode.BarcodeMargin = 8;
                    break;

                // QrCode.
                case 14:
                    edtTextToCode.MaxLength = 7089;
                    edtTextToCode.Keyboard = Keyboard.Default;
                    bgvCode.BarcodeMargin = 0;
                    break;

                // Rss14.
                case 15:
                    edtTextToCode.MaxLength = 14;
                    edtTextToCode.Keyboard = Keyboard.Numeric;
                    bgvCode.BarcodeMargin = 0;
                    break;

                // RssExpanded.
                case 16:
                    edtTextToCode.MaxLength = 74;
                    edtTextToCode.Keyboard = Keyboard.Default;
                    bgvCode.BarcodeMargin = 0;
                    break;

                // UpcA.
                case 17:
                    edtTextToCode.MaxLength = 12;
                    edtTextToCode.Keyboard = Keyboard.Numeric;
                    bgvCode.BarcodeMargin = 0;
                    break;

                // UpcE.
                case 18:
                    edtTextToCode.MaxLength = 8;
                    edtTextToCode.Keyboard = Keyboard.Numeric;
                    bgvCode.BarcodeMargin = 8;
                    break;

                // UpcEanExtension.
                case 19:
                    edtTextToCode.MaxLength = 2;
                    edtTextToCode.Keyboard = Keyboard.Numeric;
                    bgvCode.BarcodeMargin = 4;
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

        //bgvCode.IsVisible = true;  //Does not work !!!

        bgvCode.Value = "";
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

        int selectedIndex = PckFormatCode.SelectedIndex;

        if (selectedIndex != -1)
        {
            try
            {
                switch (selectedIndex)
                {
                    // Aztec.
                    case 0:
                        bgvCode.Format = BarcodeFormat.Aztec;
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

                        bgvCode.Format = BarcodeFormat.Codabar;
                        break;

                    // Code128.
                    case 2:
                        bgvCode.Format = BarcodeFormat.Code128;
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

                        bgvCode.Format = BarcodeFormat.Code39;
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

                        bgvCode.Format = BarcodeFormat.Code93;
                        break;

                    // DataMatrix.
                    case 5:
                        bgvCode.Format = BarcodeFormat.DataMatrix;
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

                        cTextToCode = cTextToCode.Substring(0, 12);
                        cTextToCode += CalculateChecksumEanUpcA(ReverseString(cTextToCode));

                        if (nLenTextToCode == 13 && cChecksum != cTextToCode.Substring(12, 1))
                        {
                            DisplayErrorMessage(cMessageErrorInCode);
                        }

                        edtTextToCode.Text = cTextToCode;
                        cTextCode = string.Concat(cTextToCode.Substring(0, 1), " ", cTextToCode.Substring(1, 6), " ", cTextToCode.Substring(7, 6));

                        bgvCode.Format = BarcodeFormat.Ean13;
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

                        cTextToCode = cTextToCode.Substring(0, 7);
                        cTextToCode += CalculateChecksumEanUpcA(ReverseString(cTextToCode));

                        if (nLenTextToCode == 8 && cChecksum != cTextToCode.Substring(7, 1))
                        {
                            DisplayErrorMessage(cMessageErrorInCode);
                        }

                        edtTextToCode.Text = cTextToCode;
                        cTextCode = string.Concat(cTextToCode.Substring(0, 4), " ", cTextToCode.Substring(4, 4));

                        bgvCode.Format = BarcodeFormat.Ean8;
                        break;

                    // Imb.
                    case 8:
                        DisplayMessageFormat("IMb");
                        return;

                    //bgvCode.Format = BarcodeFormat.Imb;
                    //break;

                    // Itf.
                    case 9:
                        if (TestAllowedCharacters("0123456789", cTextToCode) == false)
                        {
                            return;
                        }

                        if (nLenTextToCode % 2 != 0)
                        {
                            DisplayErrorMessage("The length of the input should be even.");
                            return;
                        }

                        bgvCode.Format = BarcodeFormat.Itf;
                        break;

                    // Msi.
                    case 10:
                        if (TestAllowedCharacters("0123456789", cTextToCode) == false)
                        {
                            return;
                        }

                        bgvCode.Format = BarcodeFormat.Msi;
                        break;

                    // Pdf417.
                    case 11:
                        bgvCode.Format = BarcodeFormat.Pdf417;
                        break;

                    // PharmaCode.
                    case 12:
                        DisplayMessageFormat("PharmaCode");
                        return;

                    //bgvCode.Format = BarcodeFormat.PharmaCode;
                    //break;

                    // Plessey.
                    case 13:
                        cTextToCode = cTextToCode.ToUpper();

                        if (TestAllowedCharacters("0123456789ABCDEF", cTextToCode) == false)
                        {
                            return;
                        }

                        bgvCode.Format = BarcodeFormat.Plessey;
                        break;

                    // QrCode.
                    case 14:
                        bgvCode.Format = BarcodeFormat.QrCode;
                        break;

                    // Rss14.
                    case 15:
                        DisplayMessageFormat("RSS 14");
                        return;

                    //bgvCode.Format = BarcodeFormat.Rss14;
                    //break;

                    // RssExpanded.
                    case 16:
                        DisplayMessageFormat("RSS Expanded");
                        return;

                    //bgvCode.Format = BarcodeFormat.RssExpanded;
                    //break;

                    // UpcA.
                    case 17:
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

                        cTextToCode = cTextToCode.Substring(0, 11);
                        cTextToCode += CalculateChecksumEanUpcA(cTextToCode);

                        if (nLenTextToCode == 12 && cChecksum != cTextToCode.Substring(11, 1))
                        {
                            DisplayErrorMessage(cMessageErrorInCode);
                        }

                        edtTextToCode.Text = cTextToCode;
                        cTextCode = string.Concat(cTextToCode.Substring(0, 1), " ", cTextToCode.Substring(1, 5), " ", cTextToCode.Substring(6, 5), " ", cTextToCode.Substring(11, 1));

                        bgvCode.Format = BarcodeFormat.UpcA;
                        break;

                    // UpcE.
                    case 18:
                        if (TestAllowedCharacters("0123456789", cTextToCode) == false)
                        {
                            return;
                        }

                        if (nLenTextToCode < 7 || nLenTextToCode > 8)
                        {
                            DisplayErrorMessageLength("7", "8");
                            return;
                        }

                        if (cTextToCode.Substring(0, 1) != "0")
                        {
                            DisplayErrorMessage("First number chould be 0.");
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
                            DisplayErrorMessage(cMessageErrorInCode);
                        }

                        edtTextToCode.Text = cTextToCode;
                        cTextCode = string.Concat(cTextToCode.Substring(0, 1), " ", cTextToCode.Substring(1, 6), " ", cTextToCode.Substring(7, 1));

                        bgvCode.Format = BarcodeFormat.UpcE;
                        break;

                    // UpcEanExtension.
                    case 19:
                        DisplayMessageFormat("UPC EAN Extension");
                        return;

                        //bgvCode.Format = BarcodeFormat.UpcEanExtension;
                        //break;
                }
            }

            catch (Exception)
            {
                bgvCode.Value = "";

                return;
            }
        }

        try
        {
            bgvCode.Value = cTextToCode;
            btnShare.Text = "Share " + PckFormatCode.Items[selectedIndex];
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
            DisplayAlert("Format code", ex.Message, "OK");
            bgvCode.Value = "";
            CloseApplication();
        }
    }

    // Test allowed characters.
    private bool TestAllowedCharacters(string cAllowedCharacters, string cTextToCode)
    {
        foreach (char cChar in cTextToCode)
        {
            bool bResult = cAllowedCharacters.Contains(cChar);

            if (bResult == false)
            {
                DisplayAlert("Error", "Allowed characters:\n" + cAllowedCharacters, "OK");
                edtTextToCode.Focus();

                return false;
            }
        }

        return true;
    }

    // Test start/end guards.
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
                DisplayAlert("Error", "Invalid start/end guards: " + cChar, "OK");
                edtTextToCode.Focus();

                return false;
            }
        }

        // Control of missing start or end guard.
        if (cStartEndGuards.Contains(cTextToCode.Substring(0, 1)) && cStartEndGuards.Contains(cTextToCode.Substring(cTextToCode.Length - 1, 1)) == false)
        {
            DisplayAlert("Error", "Missing end guard.", "OK");
            edtTextToCode.Focus();

            return false;
        }
        else if (cStartEndGuards.Contains(cTextToCode.Substring(0, 1)) == false && cStartEndGuards.Contains(cTextToCode.Substring(cTextToCode.Length - 1, 1)))
        {
            DisplayAlert("Error", "Missing start guard.", "OK");
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

        return String.Concat(charArray);
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
        bgvCode.Value = "";
        lblTextCode.Text = "";
        btnShare.Text = "Share";
        btnShare.IsEnabled = false;

        edtTextToCode.Focus();
    }

    // Display an error message.
    private void DisplayErrorMessage(string cMessage)
    {
        DisplayAlert("Error", cMessage, "OK");

        edtTextToCode.Focus();
    }

    // Display an error message with minimum and maximum length.
    private void DisplayErrorMessageLength(string cMinLength, string cMaxLength)
    {
        DisplayAlert("Error", "The code chould be " + cMinLength + " (without checksum digit) or " + cMaxLength + " digits long.", "OK");

        edtTextToCode.Focus();
    }

    // Display a message with no encoder available for format.
    private void DisplayMessageFormat(string cFormat)
    {
        DisplayAlert("Error", "No encoder available for format " + cFormat + ".", "OK");

        edtTextToCode.Focus();
    }

    // Show license for Android and Windows = Workaround !!!
    // Solution is using the Loaded event of the MainPage.xaml.
    private async void OnLoad(object sender, EventArgs e)
    {
        if (bLicense == false)
        {
            bool bAnswer = await Application.Current.MainPage.DisplayAlert("License", "Code Generator" + "\n" + "Copyright © 2022-2022 Geert Geerits" + "\n\n" + "This application may be used freely for non-commercial purposes.\r\nUse this program entirely at your own risk.", "Agree", "Disagree");

            if (bAnswer)
            {
                Preferences.Default.Set("SettingLicense", true);
            }
            else
            {
                Application.Current.Quit();
            }
        }
    }

    // Button share event.
    private async void OnShareClicked(object sender, EventArgs e)
    {
        imgScreenshot.Source = await TakeScreenshotAsync();
    }

    // Capture a screenshot.
    public static async Task<ImageSource> TakeScreenshotAsync()
    {
        if (Screenshot.Default.IsCaptureSupported)
        {
            IScreenshotResult screen = await Screenshot.Default.CaptureAsync();

            Stream stream = await screen.OpenReadAsync();
            
            SaveStreamAsFile(stream);

            return ImageSource.FromStream(() => stream);
        }

        return null;
    }

    // Save screenshot as image file.
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
    public static async Task OpenShareInterface(string cFile)
    {
        await Share.Default.RequestAsync(new ShareFileRequest
        {
            Title = "Barcode Generator",
            File = new ShareFile(cFile)
        });
    }

    // Close the application after an error.
    private static async void CloseApplication()
    {
        bool bAnswer = await Application.Current.MainPage.DisplayAlert("Error", "The application needs to be restarted due to an error.\nClose now?", "Yes", "No");

        if (bAnswer)
        {
            Application.Current.Quit();
        }
    }

    // Close the application.
    private static void OnCloseApplicationClicked(object sender, EventArgs e)
    {
        Application.Current.Quit();
    }
}

/*
How to convert a UPC-E code back UPC-A ?
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
