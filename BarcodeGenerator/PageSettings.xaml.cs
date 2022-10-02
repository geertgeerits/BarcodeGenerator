using System.Diagnostics;
using System.Globalization;

namespace BarcodeGenerator;

public partial class PageSettings : ContentPage
{
    // Local variables.
    private Stopwatch stopWatch = new();
    private int nNoClickedEvents = 1;
    private readonly string cThemeCurrent = MainPage.cTheme;

    public PageSettings()
	{
        //InitializeComponent();
        try
        {
            InitializeComponent();
        }
        catch (Exception ex)
        {
            DisplayAlert("InitializeComponent PageSettings", ex.Message, "OK");
            return;
        }

        // Set radiobutton to the used theme.
        string cCurrentTheme;

        if (MainPage.cTheme == "")
        {
            AppTheme currentTheme = Application.Current.RequestedTheme;
            cCurrentTheme = Convert.ToString(currentTheme);
        }
        else
        {
            cCurrentTheme = MainPage.cTheme;
        }
        //DisplayAlert("cCurrentTheme", cCurrentTheme, "OK");  // For testing

        if (cCurrentTheme == "Light")
        {
            rbnThemeLight.IsChecked = true;
        }
        else if (cCurrentTheme == "Dark")
        {
            rbnThemeDark.IsChecked = true;
        }
        else
        {
            rbnThemeSystem.IsChecked = true;
        }

        // Set the barcode format in the picker.
        PckFormatCode.SelectedIndex = MainPage.nFormatIndex;

        // Set the color sliders.
        int nRed = 0;
        int nGreen = 0;
        int nBlue = 0;

        HexToRgbColor(MainPage.cCodeColorFg, ref nRed, ref nGreen, ref nBlue);

        sldColorFgRed.Value = nRed;
        sldColorFgGreen.Value = nGreen;
        sldColorFgBlue.Value = nBlue;

        HexToRgbColor(MainPage.cCodeColorBg, ref nRed, ref nGreen, ref nBlue);

        sldColorBgRed.Value = nRed;
        sldColorBgGreen.Value = nGreen;
        sldColorBgBlue.Value = nBlue;

        // Start the stopWatch for clearing all the settings.
        stopWatch.Start();
    }

    // Radio button themes clicked event.
    private void OnThemesRadioButtonCheckedChanged(object sender, EventArgs e)
    {
        if (rbnThemeSystem.IsChecked)
        {
            MainPage.cTheme = "System";
        }
        else if (rbnThemeLight.IsChecked)
        {
            MainPage.cTheme = "Light";
        }
        else if (rbnThemeDark.IsChecked)
        {
            MainPage.cTheme = "Dark";
        }
    }

    // Picker format clicked event.
    private void OnPickerFormatCodeChanged(object sender, EventArgs e)
    {
        var picker = (Picker)sender;
        int selectedIndex = picker.SelectedIndex;

        if (selectedIndex != -1)
        {
            MainPage.nFormatIndex = selectedIndex;
        }
    }

    // Slider color barcode forground value change.
    private void OnSliderColorForgroundValueChanged(object sender, ValueChangedEventArgs args)
    {
        int nColorRed = 0;
        int nColorGreen = 0;
        int nColorBlue = 0;

        var slider = (Slider)sender;

        if (slider == sldColorFgRed)
        {
            nColorRed = (int)args.NewValue;
            nColorGreen = (int)sldColorFgGreen.Value;
            nColorBlue = (int)sldColorFgBlue.Value;
        }
        else if (slider == sldColorFgGreen)
        {
            nColorRed = (int)sldColorFgRed.Value;
            nColorGreen = (int)args.NewValue;
            nColorBlue = (int)sldColorFgBlue.Value;
        }
        else if (slider == sldColorFgBlue)
        {
            nColorRed = (int)sldColorFgRed.Value;
            nColorGreen = (int)sldColorFgGreen.Value;
            nColorBlue = (int)args.NewValue;
        }

        string cColorFgHex = nColorRed.ToString("X2") + nColorGreen.ToString("X2") + nColorBlue.ToString("X2");
        bxvColorFg.Color = Color.FromArgb(cColorFgHex);
        //bxvColorFg.Color = Color.FromRgb(nColorRed, nColorGreen, nColorBlue);

        MainPage.cCodeColorFg = cColorFgHex;
    }

    // Slider color barcode background value change.
    private void OnSliderColorBackgroundValueChanged(object sender, ValueChangedEventArgs args)
    {
        int nColorRed = 0;
        int nColorGreen = 0;
        int nColorBlue = 0;

        var slider = (Slider)sender;

        if (slider == sldColorBgRed)
        {
            nColorRed = (int)args.NewValue;
            nColorGreen = (int)sldColorBgGreen.Value;
            nColorBlue = (int)sldColorBgBlue.Value;
        }
        else if (slider == sldColorBgGreen)
        {
            nColorRed = (int)sldColorBgRed.Value;
            nColorGreen = (int)args.NewValue;
            nColorBlue = (int)sldColorBgBlue.Value;
        }
        else if (slider == sldColorBgBlue)
        {
            nColorRed = (int)sldColorBgRed.Value;
            nColorGreen = (int)sldColorBgGreen.Value;
            nColorBlue = (int)args.NewValue;
        }

        string cColorBgHex = nColorRed.ToString("X2") + nColorGreen.ToString("X2") + nColorBlue.ToString("X2");
        bxvColorBg.Color = Color.FromArgb(cColorBgHex);
        //bxvColorBg.Color = Color.FromRgb(nColorRed, nColorGreen, nColorBlue);

        MainPage.cCodeColorBg = cColorBgHex;
    }

    // Convert RRGGBB Hex color to RGB color.
    public static void HexToRgbColor(string cHexColor, ref int nRed, ref int nGreen, ref int nBlue)
    {
        // Remove # if present.
        if (cHexColor.IndexOf('#') != -1)
        {
            cHexColor = cHexColor.Replace("#", "");
        }

        nRed = int.Parse(cHexColor.Substring(0, 2), NumberStyles.AllowHexSpecifier);
        nGreen = int.Parse(cHexColor.Substring(2, 2), NumberStyles.AllowHexSpecifier);
        nBlue = int.Parse(cHexColor.Substring(4, 2), NumberStyles.AllowHexSpecifier);
    }

    // Button save settings clicked event.
    private async void OnSettingsSaveClicked(object sender, EventArgs e)
    {
        Preferences.Default.Set("SettingTheme", MainPage.cTheme);
        Preferences.Default.Set("SettingFormatIndex", MainPage.nFormatIndex);
        Preferences.Default.Set("SettingCodeColorFg", MainPage.cCodeColorFg);
        Preferences.Default.Set("SettingCodeColorBg", MainPage.cCodeColorBg);

        if (cThemeCurrent != MainPage.cTheme)
        {
            bool bAnswer = await Application.Current.MainPage.DisplayAlert("Theme", "The theme will not be applied until the application is closed.\nClose now?", "Yes", "No");

            if (bAnswer)
            {
                // Wait 500 milliseconds otherwise the settings are not saved in Android.
                Task.Delay(500).Wait();

                // Close the application.
                Application.Current.Quit();
            }
        }
    }

    // Button clear settings clicked event.
    private void OnSettingsClearClicked(object sender, EventArgs e)
    {
        // Clear some settings.
        if (nNoClickedEvents != 4)
        {
            Preferences.Remove("SettingTheme");
            Preferences.Remove("SettingFormatIndex");
            Preferences.Remove("SettingCodeColorFg");
            Preferences.Remove("SettingCodeColorBg");

            nNoClickedEvents++;
        }
        // Clear all settings and close the application after the 4 first clicked events in the first 5 seconds after opening the page.
        else
        {
            // Get the elapsed time in milli seconds.
            stopWatch.Stop();

            if (stopWatch.ElapsedMilliseconds < 5001)
            {
                // Clear all settings.
                Preferences.Clear();

                // Wait 500 milliseconds otherwise the Preferences.Clear() is not executed in Android.
                Task.Delay(500).Wait();

                // Close the application.
                Application.Current.Quit();
            }
        }
    }
}