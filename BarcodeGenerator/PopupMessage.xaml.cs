using CommunityToolkit.Maui.Views;

namespace BarcodeGenerator;

public partial class PopupMessage : Popup
{
    private CancellationTokenSource _closeCts = new();
    private int _closing;                                // 0 == not closing, 1 == closing

    public PopupMessage(int nSeconds = 3, string cMessage = "")
    {
        InitializeComponent();

        // Set the message text
        lblPopupMessage.Text = cMessage;

        // Start the asynchronous process to close the popup after the specified number of seconds
        _ = CloseAfterSecondsAsync(nSeconds, _closeCts.Token);
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
            await TryCloseAsync();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"OnButtonCloseClicked: Error closing popup - {ex.Message}");
        }
    }

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
            await TryCloseAsync();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"OnButtonCloseClicked: Error closing popup - {ex.Message}");
        }

        ClassQRCodeImage.bPopupCanceled = true;
    }
}
