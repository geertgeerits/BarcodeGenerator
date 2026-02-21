namespace BarcodeGenerator;

public partial class PopupMessage : ContentPage
{
    public PopupMessage(int secondsToClose = 3, int nPixels = 0)
    {
        InitializeComponent();

        // Set the message text with the recommended image size
        lblPopupMessage.Text = $"{CodeLang.QRCodeRecommendedImageSize_Text}:\n\n{nPixels} {CodeLang.Pixels_Text}";

        // Start a timer to close the popup after the specified number of seconds
        Task.Delay(secondsToClose * 1000).ContinueWith(async _ =>
        {
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                await Navigation.PopModalAsync();
            });
        });
    }
}
