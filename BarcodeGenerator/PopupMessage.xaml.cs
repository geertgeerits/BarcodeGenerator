using CommunityToolkit.Maui.Views;

namespace BarcodeGenerator;

public partial class PopupMessage : Popup
{
    public PopupMessage(int nSeconds = 3, string cMessage = "")
    {
        InitializeComponent();

        // Set the message text
        lblPopupMessage.Text = cMessage;

        CloseAfter(nSeconds);
    }

    private async void CloseAfter(int nSeconds)
    {
        await Task.Delay(nSeconds * 1000);
        
        try
        {
            await CloseAsync();
        }
        catch
        {
            Debug.WriteLine("Error closing popup.");
        }
    }
}
