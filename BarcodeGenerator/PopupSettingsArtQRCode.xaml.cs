using CommunityToolkit.Maui.Views;
//using Microsoft.Maui.Controls;
//using BarcodeGenerator.Pages; // <-- add the namespace where PageSettingsMain is defined
//using System.Reflection;

namespace BarcodeGenerator
{
    public partial class PopupSettingsArtQRCode : Popup
    {
        public PopupSettingsArtQRCode()
        {
            // Name of the current page
            Globals.cCurrentPage = "PopupSettingsArtQRCode";

            InitializeComponent();

            // Set the flow direction of the text elements
            Globals.SetFlowDirection(this);

            // Reset the popup canceled flag to false when the popup is opened
            Globals.bPopupCanceled = false;

            // Disable certain options for Artistic Micro QR Codes and Artistic Rectangular Micro QR Codes
            if (ClassBarcodes.cQRCodeType == ClassBarcodes.cBarcode_ART_MICRO_QR_CODE || ClassBarcodes.cQRCodeType == ClassBarcodes.cBarcode_ART_RMQR_CODE)
            {
                artQRCodeSettings.swtForegroundImage.IsToggled = false;
                artQRCodeSettings.swtBackgroundImage.IsToggled = false;
                artQRCodeSettings.swtForegroundImage.IsEnabled = false;
                artQRCodeSettings.swtBackgroundImage.IsEnabled = false;

                artQRCodeSettings.rbtQRCodeFinderPatternShapeSquare.IsEnabled = false;
                artQRCodeSettings.rbtQRCodeFinderPatternShapeRounded.IsEnabled = false;
                artQRCodeSettings.rbtQRCodeFinderPatternShapeCircle.IsEnabled = false;
            }

            //else if (ClassBarcodes.cQRCodeType == ClassBarcodes.cBarcode_ART_CIRCULAR_QR_CODE)
            //{
            //    //// Try to find the running PageSettingsMain instance and disable its caption toggle.
            //    //if (Application.Current?.MainPage is PageSettingsMain settingsPage)
            //    //{
            //    //    settingsPage.swtBarcodeWithCaption.IsToggled = false;
            //    //    settingsPage.swtBarcodeWithCaption.IsEnabled = false;
            //    //}
            //    //else if (Application.Current?.MainPage is NavigationPage nav && nav.CurrentPage is PageSettingsMain navSettingsPage)
            //    //{
            //    //    navSettingsPage.swtBarcodeWithCaption.IsToggled = false;
            //    //    navSettingsPage.swtBarcodeWithCaption.IsEnabled = false;
            //    //}

            //    var mainPage = Application.Current?.MainPage;
            //    void DisableCaption(object page)
            //    {
            //        if (page == null) return;
            //        // try property first
            //        var field = page.GetType().GetProperty("swtBarcodeWithCaption", BindingFlags.Public | BindingFlags.Instance);
            //        if (field != null)
            //        {
            //            var control = field.GetValue(page);
            //            if (control != null)
            //            {
            //                var isToggled = control.GetType().GetProperty("IsToggled");
            //                var isEnabled = control.GetType().GetProperty("IsEnabled");
            //                isToggled?.SetValue(control, false);
            //                isEnabled?.SetValue(control, false);
            //            }
            //        }
            //    }

            //    DisableCaption(mainPage);

            //    if (mainPage is NavigationPage nav)
            //    {
            //        DisableCaption(nav.CurrentPage);
            //    }
            //}

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
