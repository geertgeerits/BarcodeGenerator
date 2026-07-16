using CommunityToolkit.Maui.Views;

namespace BarcodeGenerator
{
    public partial class PopupSettingsArtQRCode : Popup
    {
        public PopupSettingsArtQRCode(string cMessage = "")
        {
            // Name of the current page
            Globals.cCurrentPage = "PopupSettingsArtQRCode";

            InitializeComponent();

            // Indicate that the popup has been opened, which can be used to prevent certain actions in the MainPage OnAppearing event
            Globals.bPopupOpened = true;

            // Set the message text
            lblPopupTitle.Text = cMessage;
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
            // ???!!!BUG!!!??? in NuGet package SkiaSharp.QrCode
            // The background color of the finder pattern shape is not being applied correctly when the opacity is less than 255,
            // so we need to manually adjust the opacity of the background color to ensure it is fully opaque when using Rounded or Circle shapes
            if (ClassBarcodes.cQRCodeFinderPatternShape == "Rounded" || ClassBarcodes.cQRCodeFinderPatternShape == "Circle")
            {
                if (ClassBarcodes.cCodeColorBgArtQRCode.Length == 8)
                {
                    ClassBarcodes.cCodeColorBgArtQRCode = $"FF{ClassBarcodes.cCodeColorBgArtQRCode.Substring(2, 6)}";
                }
            }

            await CloseAsync();
        }
    }
}
