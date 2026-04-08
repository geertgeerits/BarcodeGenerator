using CommunityToolkit.Maui.Views;

namespace BarcodeGenerator
{
    public partial class PopupSettingsArtQRCode : Popup
    {
        public PopupSettingsArtQRCode(string cMessage = "")
        {
            InitializeComponent();

            // Set the message text
            lblPopupTitle.Text = cMessage;

            // Set the initial states of the switches
            swtGradientColors.IsToggled = ClassBarcodes.bQRCodeGradientColor;
            swtForegroundImage.IsToggled = ClassBarcodes.bQRCodeForegroundImage;
            swtBackgroundImage.IsToggled = ClassBarcodes.bQRCodeBackgroundImage;

            Globals.bPopupCanceled = false;
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
        /// Switch QR code gradient color toggled event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SwtGradientColors_Toggled(object sender, ToggledEventArgs e)
        {
            ClassBarcodes.bQRCodeGradientColor = e.Value;
        }

        /// <summary>
        /// Switch QR code foreground image toggled event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SwtForegroundImage_Toggled(object sender, ToggledEventArgs e)
        {
            ClassBarcodes.bQRCodeForegroundImage = e.Value;
        }

        /// <summary>
        /// Switch QR code background image toggled event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SwtBackgroundImage_Toggled(object sender, ToggledEventArgs e)
        {
            ClassBarcodes.bQRCodeBackgroundImage = e.Value;
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
            await CloseAsync();
        }
    }
}
