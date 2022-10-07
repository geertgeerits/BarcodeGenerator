using System.Diagnostics;
using System.Globalization;

namespace BarcodeGenerator;

public partial class PageSettings : ContentPage
{
    // Local variables.
    private Stopwatch stopWatch = new();

    public PageSettings()
	{
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
        int nOpacity = 0;
        int nRed = 0;
        int nGreen = 0;
        int nBlue = 0;

        HexToRgbColor(MainPage.cCodeColorFg, ref nOpacity, ref nRed, ref nGreen, ref nBlue);

        sldColorFgRed.Value = nRed;
        sldColorFgGreen.Value = nGreen;
        sldColorFgBlue.Value = nBlue;

        HexToRgbColor(MainPage.cCodeColorBg, ref nOpacity, ref nRed, ref nGreen, ref nBlue);

        sldOpacityBg.Value = nOpacity;
        sldColorBgRed.Value = nRed;
        sldColorBgGreen.Value = nGreen;
        sldColorBgBlue.Value = nBlue;

        // Start the stopWatch for resetting all the settings.
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

        string cColorFgHex = "FF" + nColorRed.ToString("X2") + nColorGreen.ToString("X2") + nColorBlue.ToString("X2");
        bxvColorFg.Color = Color.FromArgb(cColorFgHex);

        MainPage.cCodeColorFg = cColorFgHex;
    }

    // Slider color barcode background value change.
    private void OnSliderColorBackgroundValueChanged(object sender, ValueChangedEventArgs args)
    {
        int nAmountOpacity = 0;
        int nColorRed = 0;
        int nColorGreen = 0;
        int nColorBlue = 0;

        var slider = (Slider)sender;

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

        string cColorBgHex = nAmountOpacity.ToString("X2") + nColorRed.ToString("X2") + nColorGreen.ToString("X2") + nColorBlue.ToString("X2");
        bxvColorBg.Color = Color.FromArgb(cColorBgHex);

        MainPage.cCodeColorBg = cColorBgHex;
    }

    // Convert OORRGGBB Hex color to RGB color.
    private static void HexToRgbColor(string cHexColor, ref int nOpacity, ref int nRed, ref int nGreen, ref int nBlue)
    {
        // Remove leading # if present.
        if (cHexColor.Substring(0, 1) == "#")
        {
            cHexColor = cHexColor.Substring(1);
        }

        nOpacity = int.Parse(cHexColor.Substring(0, 2), NumberStyles.AllowHexSpecifier);
        nRed = int.Parse(cHexColor.Substring(2, 2), NumberStyles.AllowHexSpecifier);
        nGreen = int.Parse(cHexColor.Substring(4, 2), NumberStyles.AllowHexSpecifier);
        nBlue = int.Parse(cHexColor.Substring(6, 2), NumberStyles.AllowHexSpecifier);
    }

    // Button save settings clicked event.
    private void OnSettingsSaveClicked(object sender, EventArgs e)
    {
        Preferences.Default.Set("SettingTheme", MainPage.cTheme);
        Preferences.Default.Set("SettingFormatIndex", MainPage.nFormatIndex);
        Preferences.Default.Set("SettingCodeColorFg", MainPage.cCodeColorFg);
        Preferences.Default.Set("SettingCodeColorBg", MainPage.cCodeColorBg);

        // Wait 500 milliseconds otherwise the settings are not saved in Android.
        Task.Delay(500).Wait();

        // Restart the application.
        Application.Current.MainPage = new AppShell();
    }

    // Button reset settings clicked event.
    private void OnSettingsResetClicked(object sender, EventArgs e)
    {
        // Get the elapsed time in milli seconds.
        stopWatch.Stop();

        if (stopWatch.ElapsedMilliseconds < 2001)
        {
            // Clear all settings after the first clicked event within the first 2 seconds after opening the setting page.
            Preferences.Default.Clear();
        }
        else
        {
            // Reset some settings.
            Preferences.Default.Remove("SettingTheme");
            Preferences.Default.Remove("SettingFormatIndex");
            Preferences.Default.Remove("SettingCodeColorFg");
            Preferences.Default.Remove("SettingCodeColorBg");
        }

        // Wait 500 milliseconds otherwise the settings are not saved in Android.
        Task.Delay(500).Wait();

        // Restart the application.
        Application.Current.MainPage = new AppShell();
    }
}