using BarcodeGenerator.Resources.Languages;
using System.Diagnostics;
using System.Globalization;

namespace BarcodeGenerator;

public partial class PageSettings : ContentPage
{
    // Local variables.
    private readonly string cHexCharacters = "0123456789ABCDEFabcdef";
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

        // Workaround for !!!BUG!!! in iOS.
#if IOS
        //Grid ColumnDefinitions="40, 110, 190*"
        // The last column with a star (*) is too wide (is beyond the right margin of the device). 
        Grid grid = new()
        {
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = new GridLength(40) },
                new ColumnDefinition { Width = new GridLength(110) },
                new ColumnDefinition { Width = new GridLength(188) }
            }
        };
        grdSettings.ColumnDefinitions = grid.ColumnDefinitions;

        // Correction of the the Slider right margin.
        //Slider slider = new Slider
        //{
        //    Margin = new Thickness(0, 0, 6, 0)
        //};

        //sldOpacityFg.Margin = slider.Margin;
        //sldColorFgRed.Margin = slider.Margin;
        //sldColorFgGreen.Margin = slider.Margin;
        //sldColorFgBlue.Margin = slider.Margin;
        //sldOpacityBg.Margin = slider.Margin;
        //sldColorBgRed.Margin = slider.Margin;
        //sldColorBgGreen.Margin = slider.Margin;
        //sldColorBgBlue.Margin = slider.Margin;
#endif

        // Put text in the chosen language in the controls and variables.
        SetLanguage();

        // Set the current language in the picker.
        pckLanguage.SelectedIndex = MainPage.cLanguage switch
        {
            // Čeština - Czech.
            "cs" => 0,

            // Dansk - Danish.
            "da" => 1,

            // Deutsch - German.
            "de" => 2,

            // Español - Spanish.
            "es" => 4,

            // Français - French.
            "fr" => 5,

            // Italiano - Italian.
            "it" => 6,

            // Magyar - Hungarian.
            "hu" => 7,

            // Nederlands - Dutch.
            "nl" => 8,

            // Norsk Bokmål - Norwegian Bokmål.
            "nb" => 9,

            // Polski - Polish.
            "pl" => 10,

            // Português - Portuguese.
            "pt" => 11,

            // Română - Romanian.
            "ro" => 12,

            // Suomi - Finnish.
            "fi" => 13,

            // Svenska - Swedish.
            "sv" => 14,

            // English.
            _ => 3,
        };

        // Fill the picker with the speech languages and set the saved language in the picker.
        FillPickerWithSpeechLanguages();

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

        // Set the barcode list and the current default barcode format in the picker for the barcode generator.
        pckFormatCodeGenerator.ItemsSource = MainPage.GetFormatCodeListGenerator();
        pckFormatCodeGenerator.SelectedIndex = MainPage.nFormatGeneratorIndex;

        // Set the barcode list and the current default barcode format in the picker for the barcode scanner.
        pckFormatCodeScanner.ItemsSource = MainPage.GetFormatCodeListScanner();
        pckFormatCodeScanner.SelectedIndex = MainPage.nFormatScannerIndex;

        // Set the current color in the entry and on the sliders.
        int nOpacity = 0;
        int nRed = 0;
        int nGreen = 0;
        int nBlue = 0;

        entHexColorFg.Text = MainPage.cCodeColorFg;

        HexToRgbColor(MainPage.cCodeColorFg, ref nOpacity, ref nRed, ref nGreen, ref nBlue);

        sldOpacityFg.Value = nOpacity;
        sldColorFgRed.Value = nRed;
        sldColorFgGreen.Value = nGreen;
        sldColorFgBlue.Value = nBlue;

        entHexColorBg.Text = MainPage.cCodeColorBg;

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
        string cLanguageOld = MainPage.cLanguage;

        var picker = (Picker)sender;
        int selectedIndex = picker.SelectedIndex;

        if (selectedIndex != -1)
        {
            MainPage.cLanguage = selectedIndex switch
            {
                // Čeština - Czech.
                0 => "cs",

                // Dansk - Danish.
                1 => "da",

                // Deutsch - German.
                2 => "de",

                // Español - Spanish.
                4 => "es",

                // Français - French.
                5 => "fr",

                // Italiano - Italian.
                6 => "it",

                // Magyar - Hungarian.
                7 => "hu",

                // Nederlands - Dutch.
                8 => "nl",

                // Norsk Bokmål - Norwegian Bokmål.
                9 => "nb",

                // Polski - Polish.
                10 => "pl",

                // Português - Portuguese.
                11 => "pt",

                // Română - Romanian.
                12 => "ro",

                // Suomi - Finnish.
                13 => "fi",

                // Svenska - Swedish.
                14 => "sv",

                // English.
                _ => "en",
            };
        }

        if (cLanguageOld != MainPage.cLanguage)
        {
            MainPage.bLanguageChanged = true;

            // Set the current UI culture of the selected language.
            MainPage.SetCultureSelectedLanguage();

            // Put text in the chosen language in the controls and variables.
            SetLanguage();

            // Search the new language in the cLanguageLocales array and select the new speech language.
            int nTotalItems = MainPage.cLanguageLocales.Length;

            for (int nItem = 0; nItem < nTotalItems; nItem++)
            {
                if (MainPage.cLanguageLocales[nItem].StartsWith(MainPage.cLanguage))
                {
                    pckLanguageSpeech.SelectedIndex = nItem;
                    break;
                }
            }
        }
    }

    // Put text in the chosen language in the controls and variables.
    private void SetLanguage()
    {
        // Set the barcode list and the current default barcode format in the picker for the barcode scanner.
        pckFormatCodeScanner.ItemsSource = MainPage.GetFormatCodeListScanner();
        pckFormatCodeScanner.SelectedIndex = MainPage.nFormatScannerIndex;

        // Set the current theme in the picker.
        var ThemeList = new List<string>
        {
            CodeLang.ThemeSystem_Text,
            CodeLang.ThemeLight_Text,
            CodeLang.ThemeDark_Text
        };
        pckTheme.ItemsSource = ThemeList;

        pckTheme.SelectedIndex = MainPage.cTheme switch
        {
            // Light.
            "Light" => 1,

            // Dark.
            "Dark" => 2,

            // System.
            _ => 0,
        };

        //lblExplanation.Text = CodeLang.SettingsSaved_Text + "\n";  // Workaround for !!!BUG!!! auto sizing label for small screens.  Add a new line to solve the bug.
    }

    // Fill the picker with the speech languages from the array.
    // .Country = KR ; .Id = ''  ; .Language = ko ; .Name = Korean (South Korea) ; 
    private void FillPickerWithSpeechLanguages()
    {
        // If there are no locales then return.
        bool bIsSetSelectedIndex = false;

        if (!MainPage.bLanguageLocalesExist)
        {
            pckLanguageSpeech.IsEnabled = false;           
            return;
        }

        // Put the sorted locales from the array in the picker and select the saved language.
        int nTotalItems = MainPage.cLanguageLocales.Length;

        for (int nItem = 0; nItem < nTotalItems; nItem++)
        {
            pckLanguageSpeech.Items.Add(MainPage.cLanguageLocales[nItem]);

            if (MainPage.cLanguageSpeech == MainPage.cLanguageLocales[nItem])
            {
                pckLanguageSpeech.SelectedIndex = nItem;
                bIsSetSelectedIndex = true;
            }
        }

        // If the language is not found set the picker to the first item.
        if (!bIsSetSelectedIndex)
        {
            pckLanguageSpeech.SelectedIndex = 0;
        }
    }

    // Picker speech language clicked event.
    private void OnPickerLanguageSpeechChanged(object sender, EventArgs e)
    {
        var picker = (Picker)sender;
        int selectedIndex = picker.SelectedIndex;

        if (selectedIndex != -1)
        {
            MainPage.cLanguageSpeech = picker.Items[selectedIndex];
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

    // Picker format generator clicked event.
    private void OnPickerFormatCodeGeneratorChanged(object sender, EventArgs e)
    {
        var picker = (Picker)sender;
        int selectedIndex = picker.SelectedIndex;

        if (selectedIndex != -1)
        {
            MainPage.nFormatGeneratorIndex = selectedIndex;
        }
    }

    // Picker format scanner clicked event.
    private void OnPickerFormatCodeScannerChanged(object sender, EventArgs e)
    {
        var picker = (Picker)sender;
        int selectedIndex = picker.SelectedIndex;

        if (selectedIndex != -1)
        {
            MainPage.nFormatScannerIndex = selectedIndex;
        }
    }

    // On entry HexColor text changed event.
    private void EntryHexColorTextChanged(object sender, EventArgs e)
    {
        var entry = (Entry)sender;

        if (TestAllowedCharacters(cHexCharacters, entry.Text) == false)
        {
            entry.Focus();
        }
    }

    // Display help for Hex color.
    private async void OnSettingsHexColorClicked(object sender, EventArgs e)
    {
        await DisplayAlert("?", CodeLang.HexColorCodes_Text, CodeLang.ButtonClose_Text);
    }

    // Entry HexColor Unfocused event.
    private void EntryHexColorUnfocused(object sender, EventArgs e)
    {
        var entry = (Entry)sender;

        //Test for allowed characters.
        if (TestAllowedCharacters(cHexCharacters, entry.Text) == false)
        {
            entry.Focus();
            return;
        }

        // Add the opacity if length = 6 characters.
        if (entry.Text.Length == 6)
        {
            entry.Text = "FF" + entry.Text;
        }

        // Length must be 8 characters.
        if (entry.Text.Length != 8)
        {
            entry.Focus();
            return;
        }

        // Set the sliders position.
        int nOpacity = 0;
        int nRed = 0;
        int nGreen = 0;
        int nBlue = 0;

        if (entry == entHexColorFg)
        {
            MainPage.cCodeColorFg = entHexColorFg.Text;

            HexToRgbColor(MainPage.cCodeColorFg, ref nOpacity, ref nRed, ref nGreen, ref nBlue);

            sldOpacityFg.Value = nOpacity;
            sldColorFgRed.Value = nRed;
            sldColorFgGreen.Value = nGreen;
            sldColorFgBlue.Value = nBlue;
        }
        else
        {
            MainPage.cCodeColorBg = entHexColorBg.Text;

            HexToRgbColor(MainPage.cCodeColorBg, ref nOpacity, ref nRed, ref nGreen, ref nBlue);

            sldOpacityBg.Value = nOpacity;
            sldColorBgRed.Value = nRed;
            sldColorBgGreen.Value = nGreen;
            sldColorBgBlue.Value = nBlue;
        }

        // Set focus to the next or save button.
        if (sender.Equals(entHexColorFg))
        {
            entHexColorBg.Focus();
        }
        else
        {
            // Hide the keyboard.
            entry.IsEnabled = false;
            entry.IsEnabled = true;

            _ = btnSettingsSave.Focus();
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
                DisplayAlert(CodeLang.ErrorTitle_Text, CodeLang.AllowedChar_Text + "\n" + cAllowedCharacters + "\n\n" + CodeLang.AllowedCharNot_Text + " " + cChar, CodeLang.ButtonClose_Text);
                return false;
            }
        }

        return true;
    }

    // Slider color barcode Foreground value change.
    private void OnSliderColorForegroundValueChanged(object sender, ValueChangedEventArgs args)
    {
        int nAmountOpacity = 0;
        int nColorRed = 0;
        int nColorGreen = 0;
        int nColorBlue = 0;

        var slider = (Slider)sender;

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

        string cColorFgHex = nAmountOpacity.ToString("X2") + nColorRed.ToString("X2") + nColorGreen.ToString("X2") + nColorBlue.ToString("X2");
        entHexColorFg.Text = cColorFgHex;
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
        entHexColorBg.Text = cColorBgHex;
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
    private static void OnSettingsSaveClicked(object sender, EventArgs e)
    {
        Preferences.Default.Set("SettingTheme", MainPage.cTheme);
        Preferences.Default.Set("SettingFormatGeneratorIndex", MainPage.nFormatGeneratorIndex);
        Preferences.Default.Set("SettingFormatScannerIndex", MainPage.nFormatScannerIndex);
        Preferences.Default.Set("SettingCodeColorFg", MainPage.cCodeColorFg);
        Preferences.Default.Set("SettingCodeColorBg", MainPage.cCodeColorBg);
        Preferences.Default.Set("SettingLanguage", MainPage.cLanguage);
        Preferences.Default.Set("SettingLanguageSpeech", MainPage.cLanguageSpeech);

        // Wait 500 milliseconds otherwise the settings are not saved in Android.
        Task.Delay(500).Wait();

        // Restart the application.
        //Application.Current.MainPage = new AppShell();
        Application.Current.MainPage = new NavigationPage(new MainPage());
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
            Preferences.Default.Remove("SettingFormatGeneratorIndex");
            Preferences.Default.Remove("SettingFormatScannerIndex");
            Preferences.Default.Remove("SettingCodeColorFg");
            Preferences.Default.Remove("SettingCodeColorBg");
            Preferences.Default.Remove("SettingLanguage");
            Preferences.Default.Remove("SettingLanguageSpeech");
        }

        // Wait 500 milliseconds otherwise the settings are not saved in Android.
        Task.Delay(500).Wait();

        // Restart the application.
        //Application.Current.MainPage = new AppShell();
        Application.Current.MainPage = new NavigationPage(new MainPage());
    }
}
