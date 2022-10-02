using System.Diagnostics;

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

        // Set the barcode format.
        PckFormatCode.SelectedIndex = MainPage.nFormatIndex;

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
    void OnPickerFormatCodeChanged(object sender, EventArgs e)
    {
        var picker = (Picker)sender;
        int selectedIndex = picker.SelectedIndex;

        if (selectedIndex != -1)
        {
            MainPage.nFormatIndex = selectedIndex;
        }
    }

    // Button save settings clicked event.
    private async void OnSettingsSaveClicked(object sender, EventArgs e)
    {
        Preferences.Default.Set("SettingTheme", MainPage.cTheme);
        Preferences.Default.Set("SettingFormatIndex", MainPage.nFormatIndex);

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