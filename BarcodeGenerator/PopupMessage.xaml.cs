using CommunityToolkit.Maui.Views;

namespace BarcodeGenerator;

public partial class PopupMessage : Popup
{
    public PopupMessage(int seconds = 3, int nPixels = 0)
    {
        InitializeComponent();

        // Set the message text with the recommended image size
        lblPopupMessage.Text = $"{CodeLang.QRCodeRecommendedImageSize_Text}:\n\n{nPixels} {CodeLang.Pixels_Text}";

        CloseAfter(seconds);
    }

    private async void CloseAfter(int seconds)
    {
        await Task.Delay(seconds * 1000);
        await CloseAsync();
    }
}
