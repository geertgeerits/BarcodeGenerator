using CommunityToolkit.Maui.Views;

namespace BarcodeGenerator
{
    public partial class PopupSettingsArtQRCode : Popup
    {
        public PopupSettingsArtQRCode()
        {
            // Name of the current page
            Globals.cCurrentPage = "PopupSettingsArtQRCode";

            InitializeComponent();

            if (ClassBarcodes.cQRCodeType == ClassBarcodes.cBarcode_ART_MICRO_QR_CODE)
            {
                // Disable the foreground image option for Micro QR Codes, as they do not support this feature
                artQRCodeSettings.swtForegroundImage.IsToggled = false;
                artQRCodeSettings.swtBackgroundImage.IsToggled = false;
                artQRCodeSettings.swtForegroundImage.IsEnabled = false;
                artQRCodeSettings.swtBackgroundImage.IsEnabled = false;
            }

            // Indicate that the popup has been opened, which can be used to prevent certain actions in the MainPage OnAppearing event
            Globals.bPopupOpened = true;
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
