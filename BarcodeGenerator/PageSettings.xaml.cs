using BarcodeGenerator.Resources.Languages;
using System.Diagnostics;
using System.Globalization;

namespace BarcodeGenerator;

public partial class PageSettings : ContentPage
{
    // Local variables.
    private readonly Stopwatch stopWatch = new();

    public PageSettings()
	{
        try
        {
            InitializeComponent();
        }
        catch (Exception ex)
        {
            DisplayAlert("InitializeComponent: PageSettings", ex.Message, "OK");
            return;
        }

        // Put text in the chosen language in the controls.
        lblTitle.Text = CodeLang.Settings_Text;

        lblLanguage.Text = CodeLang.Language_Text;
        lblTheme.Text = CodeLang.Theme_Text;
        lblDefaultFormat.Text = CodeLang.DefaultFormat_Text;
        lblBarcodeColor.Text = CodeLang.BarcodeColor_Text;
        lblBackgroundColor.Text = CodeLang.BackgroundColor_Text;
        lblBackgroundOpacity.Text = CodeLang.BackgroundOpacity_Text;
        btnSettingsSave.Text = CodeLang.SettingsSave_Text;
        btnSettingsReset.Text = CodeLang.SettingsReset_Text;

        var ThemeList = new List<string>
        {
            CodeLang.ThemeSystem_Text,
            CodeLang.ThemeLight_Text,
            CodeLang.ThemeDark_Text
        };
        pckTheme.ItemsSource = ThemeList;

        // Set the current language in the picker.
        pckLanguage.SelectedIndex = MainPage.cLanguage switch
        {
            // German (Deutsch).
            "de" => 0,
            
            // Spanish (Español).
            "es" => 2,
            
            // French (Français).
            "fr" => 3,
            
            // Italian (Italiano).
            "it" => 4,
            
            // Dutch (Nederlands).
            "nl" => 5,
            
            // Portuguese (Português).
            "pt" => 6,
            
            // English.
            _ => 1,
        };

        // Set the current theme in the picker.
        pckTheme.SelectedIndex = MainPage.cTheme switch
        {
            // Light.
            "Light" => 1,

            // Dark.
            "Dark" => 2,

            // System.
            _ => 0,
        };

        // Set the current default barcode format in the picker.
        pckFormatCode.SelectedIndex = MainPage.nFormatIndex;

        // Set the current color on the sliders.
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

    // Picker language clicked event.
    private void OnPickerLanguageChanged(object sender, EventArgs e)
    {
        var picker = (Picker)sender;
        int selectedIndex = picker.SelectedIndex;

        if (selectedIndex != -1)
        {
            MainPage.cLanguage = selectedIndex switch
            {
                // German (Deutsch).
                0 => "de",
                
                // Spanish (Español).
                2 => "es",
                
                // French (Français).
                3 => "fr",
                
                // Italian (Italiano).
                4 => "it",
                
                // Dutch (Nederlands).
                5 => "nl",
                
                // Portuguese (Português).
                6 => "pt",
                
                // English.
                _ => "en",
            };
        }
    }

    // Picker theme clicked event.
    private void OnPickerThemeChanged(object sender, EventArgs e)
    {
        var picker = (Picker)sender;
        int selectedIndex = picker.SelectedIndex;

        if (selectedIndex != -1)
        {
            MainPage.cTheme = selectedIndex switch
            {
                // Light.
                1 => "Light",

                // Dark.
                2 => "Dark",

                // System.
                _ => "System",
            };
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
        if (cHexColor[..1] == "#")
        {
            cHexColor = cHexColor[1..];
        }

        nOpacity = int.Parse(cHexColor[..2], NumberStyles.AllowHexSpecifier);
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
        Preferences.Default.Set("SettingLanguage", MainPage.cLanguage);

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
            Preferences.Default.Remove("SettingLanguage");
        }

        // Wait 500 milliseconds otherwise the settings are not saved in Android.
        Task.Delay(500).Wait();

        // Restart the application.
        Application.Current.MainPage = new AppShell();
    }
}