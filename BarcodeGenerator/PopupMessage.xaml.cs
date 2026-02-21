using CommunityToolkit.Maui.Views;

namespace BarcodeGenerator;

public partial class PopupMessage : Popup
{
    public PopupMessage(int seconds = 3, string cMessage = "")
    {
        InitializeComponent();

        // Set the message text with the recommended image size
        lblPopupMessage.Text = cMessage;

        CloseAfter(seconds);
    }

    private async void CloseAfter(int seconds)
    {
        await Task.Delay(seconds * 1000);
        await CloseAsync();
    }
}
