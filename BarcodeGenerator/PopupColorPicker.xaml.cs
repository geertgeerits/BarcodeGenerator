using CommunityToolkit.Maui.Views;

namespace BarcodeGenerator;

public partial class PopupColorPicker : Popup
{
    // Local variables
    private const string cHexCharacters = "0123456789ABCDEFabcdef";
    private CancellationTokenSource _closeCts = new();
    private int _closing;                                // 0 == not closing, 1 == closing

    public PopupColorPicker(int nSeconds = 3, string cMessage = "")
    {
        InitializeComponent();

        // Set the message text
        //lblPopupMessage.Text = cMessage;

        // Start the asynchronous process to close the popup after the specified number of seconds
        _ = CloseAfterSecondsAsync(nSeconds, _closeCts.Token);

        // Set the current color in the entry and on the sliders
        int nOpacity = 0;
        int nRed = 0;
        int nGreen = 0;
        int nBlue = 0;

        entHexColor.Text = Globals.cCodeColorFg;

        HexToRgbColor(Globals.cCodeColorFg, ref nOpacity, ref nRed, ref nGreen, ref nBlue);

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

#if ANDROID
        // Android !!!BUG!!! SafeAreaEdges not behaving as expected #33922 - https://github.com/dotnet/maui/issues/33922
        // A padding with a height of 50 at the bottom is added to the grid to workaround this issue and push the content
        // above the navigation bar on Android when the entry is unfocused and the soft input keyboard is hidden.
        // Happens when the 'Entry ReturnType' is set to 'Done' and the soft keyboard is hiding after pressing 'Done'.
        // Workaround: use always 'Next' and handle the focus in the GoToNextField method to go to the next field or unfocus the last field.
        // Happens most with the Microsoft SwiftKey keyboard, the Samsung and Google keyboards have it less or not at all.
        //if (!hasWorkaroundRow)
        //{
        //    grdMainFixed.Padding = new Thickness(0, 0, 0, 50);
        //    hasWorkaroundRow = true;
        //}
#endif

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

        if (entry == entHexColor)
        {
            Globals.cCodeColor = entHexColor.Text;

            HexToRgbColor(Globals.cCodeColor, ref nOpacity, ref nRed, ref nGreen, ref nBlue);

            sldOpacity.Value = nOpacity;
            sldColorRed.Value = nRed;
            sldColorGreen.Value = nGreen;
            sldColorBlue.Value = nBlue;
        }
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
        string cColorFgHex = $"{nAmountOpacity:X2}{nColorRed:X2}{nColorGreen:X2}{nColorBlue:X2}";
        entHexColor.Text = cColorFgHex;
        bxvColor.Color = Color.FromArgb(cColorFgHex);

        Globals.cCodeColor = cColorFgHex;
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
    /// Initiates an asynchronous operation to close the popup after a specified delay, unless canceled.
    /// </summary>
    /// <remarks>If the operation is canceled via the provided token, the popup is not closed. Any exceptions
    /// encountered during the close attempt are logged and do not propagate to the caller.</remarks>
    /// <param name="nSeconds">The number of seconds to wait before attempting to close the popup. Must be a positive integer.</param>
    /// <param name="token">A cancellation token that can be used to cancel the close operation before the delay elapses.</param>
    /// <returns>A task that represents the asynchronous close operation.</returns>
    private async Task CloseAfterSecondsAsync(int nSeconds, CancellationToken token)
    {
        try
        {
            await Task.Delay(nSeconds * 1000, token);
            await TryCloseAsync();
        }
        catch (OperationCanceledException)
        {
            // Canceled by manual close - nothing to do
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"CloseAfterAsync: Error closing popup - {ex.Message}");
        }
    }

    /// <summary>
    /// Attempts to close the popup asynchronously, ensuring that only one caller can proceed with the closing
    /// operation.
    /// </summary>
    /// <remarks>This method uses a thread-safe mechanism to prevent multiple concurrent calls to the closing
    /// operation. If an error occurs during the closing process, it is logged for debugging purposes. The method also
    /// ensures that any associated cancellation token source is properly disposed of after the operation.</remarks>
    /// <returns>A task that represents the asynchronous close operation. The task completes when the popup has been closed or if
    /// the operation was already in progress.</returns>
    private async Task TryCloseAsync()
    {
        // Ensure only one caller proceeds to call CloseAsync
        if (Interlocked.CompareExchange(ref _closing, 1, 0) != 0)
            return;

        try
        {
            await CloseAsync();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"TryCloseAsync: Error closing popup - {ex.Message}");
        }
        finally
        {
            // cleanup token source
            try { _closeCts.Dispose(); } catch { }
        }
    }

    /// <summary>
    /// Handles the click event for the close button by cancelling any pending auto-close operations and initiating an
    /// asynchronous attempt to close the popup.
    /// </summary>
    /// <remarks>This method ensures that any ongoing auto-close operation is cancelled before attempting to
    /// close the popup. Errors encountered during the closing process are logged for diagnostic purposes.</remarks>
    /// <param name="sender">The source of the event, typically the close button that was clicked.</param>
    /// <param name="e">The event data associated with the button click.</param>
    private async void OnButtonClose_Clicked(object sender, EventArgs e)
    {
        // Cancel the pending auto-close and attempt a single close
        try
        {
            _closeCts.Cancel();
        }
        catch { }

        try
        {
            Globals.cCodeColor = entHexColor.Text;
            await TryCloseAsync();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"OnButtonCloseClicked: Error closing popup - {ex.Message}");
        }
    }

    /// <summary>
    /// On click event for the cancel button, which cancels any pending auto-close operations and attempts to close the popup
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void OnButtonCancel_Clicked(object sender, EventArgs e)
    {
        // Cancel the pending auto-close and attempt a single close
        try
        {
            _closeCts.Cancel();
        }
        catch { }

        try
        {
            Globals.bPopupCanceled = true;

            await TryCloseAsync();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"OnButtonCloseClicked: Error closing popup - {ex.Message}");
        }
    }
}
