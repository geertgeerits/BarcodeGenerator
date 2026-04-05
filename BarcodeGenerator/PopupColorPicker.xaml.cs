using CommunityToolkit.Maui.Views;

namespace BarcodeGenerator;

public partial class PopupColorPicker : Popup
{
    // Local variables
    private const string cHexCharacters = "0123456789ABCDEFabcdef";

    public PopupColorPicker(string cMessage = "")
    {
        InitializeComponent();

        // Set the message text
        lblPopupMessage.Text = cMessage;
        
        Globals.bPopupCanceled = false;

        // Set the current color in the entry and on the sliders
        int nOpacity = 0;
        int nRed = 0;
        int nGreen = 0;
        int nBlue = 0;

        entHexColor.Text = Globals.cCodeColor;

        HexToRgbColor(Globals.cCodeColor, ref nOpacity, ref nRed, ref nGreen, ref nBlue);

        sldOpacity.Value = nOpacity;
        sldColorRed.Value = nRed;
        sldColorGreen.Value = nGreen;
        sldColorBlue.Value = nBlue;
    }

    /// <summary>
    /// Set focus to the close button when the popup is loaded, so that pressing Enter will close it immediately.
    /// </summary>
    /// <param name="sender">The source of the event, typically the popup that was loaded.</param>
    /// <param name="e">An EventArgs object that contains the event data.</param>
    private void Popup_Loaded(object sender, EventArgs e)
    {
        btnButtonClose.Focus();
    }
   
    /// <summary>
    /// Entry HexColor Unfocused event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void EntryHexColorUnfocused(object sender, EventArgs e)
    {
        Entry entry = (Entry)sender;

#if IOS
        // https://github.com/dotnet/maui/issues/33316 and https://github.com/dotnet/maui/issues/32016
        // Workaround for iOS !!!BUG!!! The MaxLength property of an Entry control is not respected on iOS,
        // so we have to set the text again to get the correct length of the text (Microsoft.Maui.Controls Version 10.0.41)
        // Re-assign text to enforce MaxLength on iOS
        string cTemp = entry.Text;
        entry.Text = string.Empty;
        entry.Text = cTemp;
#endif
        // Add the opacity if length = 6 characters
        if (entry.Text?.Length == 6)
        {
            entry.Text = $"FF{entry.Text}";
        }

        // Length must be 8 characters
        if (entry.Text == null || entry.Text.Length != 8)
        {
            _ = entry.Focus();
            return;
        }

        // Set the sliders position based on the entry content
        int nOpacity = 0;
        int nRed = 0;
        int nGreen = 0;
        int nBlue = 0;

        Globals.cCodeColor = entHexColor.Text;

        HexToRgbColor(Globals.cCodeColor, ref nOpacity, ref nRed, ref nGreen, ref nBlue);

        sldOpacity.Value = nOpacity;
        sldColorRed.Value = nRed;
        sldColorGreen.Value = nGreen;
        sldColorBlue.Value = nBlue;
    }

    /// <summary>
    /// Slider color barcode value change
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private void OnSliderColorValueChanged(object sender, ValueChangedEventArgs args)
    {
        int nAmountOpacity = 0;
        int nColorRed = 0;
        int nColorGreen = 0;
        int nColorBlue = 0;

        Slider slider = (Slider)sender;

        try
        {
            if (slider == sldOpacity)
            {
                nAmountOpacity = (int)args.NewValue;
                nColorRed = (int)sldColorRed.Value;
                nColorGreen = (int)sldColorGreen.Value;
                nColorBlue = (int)sldColorBlue.Value;
            }
            else if (slider == sldColorRed)
            {
                nAmountOpacity = (int)sldOpacity.Value;
                nColorRed = (int)args.NewValue;
                nColorGreen = (int)sldColorGreen.Value;
                nColorBlue = (int)sldColorBlue.Value;
            }
            else if (slider == sldColorGreen)
            {
                nAmountOpacity = (int)sldOpacity.Value;
                nColorRed = (int)sldColorRed.Value;
                nColorGreen = (int)args.NewValue;
                nColorBlue = (int)sldColorBlue.Value;
            }
            else if (slider == sldColorBlue)
            {
                nAmountOpacity = (int)sldOpacity.Value;
                nColorRed = (int)sldColorRed.Value;
                nColorGreen = (int)sldColorGreen.Value;
                nColorBlue = (int)args.NewValue;
            }

            // The X2 format specifier formats the number as a hexadecimal value with a minimum width of 2 digits
            string cColorHex = $"{nAmountOpacity:X2}{nColorRed:X2}{nColorGreen:X2}{nColorBlue:X2}";
            entHexColor.Text = cColorHex;
            bxvColor.Color = Color.FromArgb(cColorHex);

            Globals.cCodeColor = cColorHex;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"PopupColorPicker.OnSliderColorValueChanged: Error converting slider value to hex - {ex.Message}");
        }
    }

    /// <summary>
    /// Convert OORRGGBB Hex color to RGB color
    /// </summary>
    /// <param name="cHexColor"></param>
    /// <param name="nOpacity"></param>
    /// <param name="nRed"></param>
    /// <param name="nGreen"></param>
    /// <param name="nBlue"></param>
    private static void HexToRgbColor(string cHexColor, ref int nOpacity, ref int nRed, ref int nGreen, ref int nBlue)
    {
        try
        {
            // Remove leading # if present
            if (cHexColor[..1] == "#")
            {
                cHexColor = cHexColor[1..];
            }

            nOpacity = int.Parse(cHexColor.AsSpan(0, 2), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture);
            nRed = int.Parse(cHexColor.AsSpan(2, 2), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture);
            nGreen = int.Parse(cHexColor.AsSpan(4, 2), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture);
            nBlue = int.Parse(cHexColor.AsSpan(6, 2), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture);

        }
        catch (Exception ex)
        {
            Debug.WriteLine($"PopupColorPicker.HexToRgbColor: Error converting hex to RGB - {ex.Message}");
        }
    }

    /// <summary>
    /// On entry HexColor text changed event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void EntryHexColorTextChanged(object sender, TextChangedEventArgs e)
    {
        if (!IsHex(e.NewTextValue))
        {
            ((Entry)sender).Text = e.OldTextValue;
        }
    }

    /// <summary>
    /// Test if the text is a hex value
    /// </summary>
    /// <param name="cText"></param>
    /// <returns></returns>
    private static bool IsHex(string cText)
    {
        foreach (char c in cText)
        {
            if (!cHexCharacters.Contains(c))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Display help for Hex color
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void OnSettingsHexColorHelpClicked(object sender, EventArgs e)
    {
        await Application.Current!.Windows[0].Page!.DisplayAlertAsync("?", $"{CodeLang.HexColorCodes_Text}\n\n{CodeLang.AllowedChar_Text}\n{cHexCharacters}", CodeLang.ButtonClose_Text);
    }

    /// <summary>
    /// On click event for the cancel button, which cancels attempts to close the popup
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void OnButtonCancel_Clicked(object sender, EventArgs e)
    {
        Globals.bPopupCanceled = true;
        await CloseAsync();
    }

    /// <summary>
    /// Handles the click event for the close button by initiating an asynchronous attempt to close the popup
    /// </summary>
    /// <param name="sender">The source of the event, typically the close button that was clicked.</param>
    /// <param name="e">The event data associated with the button click.</param>
    private async void OnButtonClose_Clicked(object sender, EventArgs e)
    {
        Globals.cCodeColor = entHexColor.Text;
        await CloseAsync();
    }
}
