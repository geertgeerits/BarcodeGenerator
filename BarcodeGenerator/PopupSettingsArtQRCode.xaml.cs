using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Maui.Views;

namespace BarcodeGenerator
{
    public partial class PopupSettingsArtQRCode : Popup
    {
        private readonly string cGradientDirectionBackgroundColor = "000099";  // navy blue

        public PopupSettingsArtQRCode(string cMessage = "")
        {
            InitializeComponent();

            // Set the message text
            lblPopupTitle.Text = cMessage;

            // Select the current QR Code module shape in the radio buttons
            rbtQRCodeModuleShapeSquare.IsChecked = ClassBarcodes.cQRCodeModuleShape == "Square";
            rbtQRCodeModuleShapeRounded.IsChecked = ClassBarcodes.cQRCodeModuleShape == "Rounded";
            rbtQRCodeModuleShapeCircle.IsChecked = ClassBarcodes.cQRCodeModuleShape == "Circle";

            // Set the initial states of the switches
            swtQRCodeGradient.IsToggled = ClassBarcodes.bQRCodeGradientColor;
            swtForegroundImage.IsToggled = ClassBarcodes.bQRCodeForegroundImage;
            swtBackgroundImage.IsToggled = ClassBarcodes.bQRCodeBackgroundImage;

            // Set the current color in the box view
            bxvColorFg.Color = Color.FromArgb(Globals.cCodeColorFg);
            bxvColorBgArtQRCode.Color = Color.FromArgb(Globals.cCodeColorBgArtQRCode);
            bxvGradientColor1.Color = Color.FromArgb(ClassBarcodes.cQRCodeGradientColor1);
            bxvGradientColor2.Color = Color.FromArgb(ClassBarcodes.cQRCodeGradientColor2);
            bxvGradientColor3.Color = Color.FromArgb(ClassBarcodes.cQRCodeGradientColor3);

            // Set the QR code gradient direction to update the button background color
            switch (ClassBarcodes.cQRCodeGradientDirection)
            {
                case "BottomToTop":
                    btnQRCodeGradientDirection1.BackgroundColor = Color.FromArgb(cGradientDirectionBackgroundColor);
                    break;
                case "BottomLeftToTopRight":
                    btnQRCodeGradientDirection2.BackgroundColor = Color.FromArgb(cGradientDirectionBackgroundColor);
                    break;
                case "LeftToRight":
                    btnQRCodeGradientDirection3.BackgroundColor = Color.FromArgb(cGradientDirectionBackgroundColor);
                    break;
                case "TopLeftToBottomRight":
                    btnQRCodeGradientDirection4.BackgroundColor = Color.FromArgb(cGradientDirectionBackgroundColor);
                    break;
                case "TopToBottom":
                    btnQRCodeGradientDirection5.BackgroundColor = Color.FromArgb(cGradientDirectionBackgroundColor);
                    break;
                case "TopRightToBottomLeft":
                    btnQRCodeGradientDirection6.BackgroundColor = Color.FromArgb(cGradientDirectionBackgroundColor);
                    break;
                case "RightToLeft":
                    btnQRCodeGradientDirection7.BackgroundColor = Color.FromArgb(cGradientDirectionBackgroundColor);
                    break;
                case "BottomRightToTopLeft":
                    btnQRCodeGradientDirection8.BackgroundColor = Color.FromArgb(cGradientDirectionBackgroundColor);
                    break;
            }

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
        /// On button color forground clicked event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnButtonColorForgroundClicked(object sender, EventArgs e)
        {
            Globals.cCodeColor = Globals.cCodeColorFg;
            await OpenPopupColorPickerAsync(CodeLang.ForegroundColor_Text);

            if (!Globals.bPopupCanceled)
            {
                Globals.cCodeColorFg = Globals.cCodeColor;
                bxvColorFg.Color = Color.FromArgb(Globals.cCodeColorFg);
            }
        }

        /// <summary>
        /// On button color background Art QR code clicked event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnButtonColorBackgroundArtQRCodeClicked(object sender, EventArgs e)
        {
            Globals.cCodeColor = Globals.cCodeColorBgArtQRCode;
            await OpenPopupColorPickerAsync(CodeLang.BackgroundColorArtQRCode_Text);

            if (!Globals.bPopupCanceled)
            {
                Globals.cCodeColorBgArtQRCode = Globals.cCodeColor;
                bxvColorBgArtQRCode.Color = Color.FromArgb(Globals.cCodeColorBgArtQRCode);
            }
        }

        /// <summary>
        /// Radio button QR code module shape checked changed event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RbtQRCodeModuleShape_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (rbtQRCodeModuleShapeSquare.IsChecked)
            {
                ClassBarcodes.cQRCodeModuleShape = "Square";
            }
            else if (rbtQRCodeModuleShapeRounded.IsChecked)
            {
                ClassBarcodes.cQRCodeModuleShape = "Rounded";
            }
            else if (rbtQRCodeModuleShapeCircle.IsChecked)
            {
                ClassBarcodes.cQRCodeModuleShape = "Circle";
            }
        }

        /// <summary>
        /// Switch QR code gradient toggled event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SwtQRCodeGradient_Toggled(object sender, ToggledEventArgs e)
        {
            ClassBarcodes.bQRCodeGradientColor = e.Value;
        }

        /// <summary>
        ///  On button color gradient 1 clicked event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnButtonGradientColor1Clicked(object sender, EventArgs e)
        {
            Globals.cCodeColor = ClassBarcodes.cQRCodeGradientColor1;
            await OpenPopupColorPickerAsync(CodeLang.QRCodeGradientColor1_Text);

            if (!Globals.bPopupCanceled)
            {
                ClassBarcodes.cQRCodeGradientColor1 = Globals.cCodeColor;
                bxvGradientColor1.Color = Color.FromArgb(ClassBarcodes.cQRCodeGradientColor1);
            }
        }

        /// <summary>
        /// On button color gradient 2 clicked event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnButtonGradientColor2Clicked(object sender, EventArgs e)
        {
            Globals.cCodeColor = ClassBarcodes.cQRCodeGradientColor2;
            await OpenPopupColorPickerAsync(CodeLang.QRCodeGradientColor2_Text);

            if (!Globals.bPopupCanceled)
            {
                ClassBarcodes.cQRCodeGradientColor2 = Globals.cCodeColor;
                bxvGradientColor2.Color = Color.FromArgb(ClassBarcodes.cQRCodeGradientColor2);
            }
        }

        /// <summary>
        /// On button color gradient 3 clicked event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnButtonGradientColor3Clicked(object sender, EventArgs e)
        {
            Globals.cCodeColor = ClassBarcodes.cQRCodeGradientColor3;
            await OpenPopupColorPickerAsync(CodeLang.QRCodeGradientColor3_Text);

            if (!Globals.bPopupCanceled)
            {
                ClassBarcodes.cQRCodeGradientColor3 = Globals.cCodeColor;
                bxvGradientColor3.Color = Color.FromArgb(ClassBarcodes.cQRCodeGradientColor3);
            }
        }

        /// <summary>
        /// On button QR code gradient direction clicked event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnButtonQRCodeGradientDirectionClicked(object sender, EventArgs e)
        {
            btnQRCodeGradientDirection1.BackgroundColor = Colors.Transparent;
            btnQRCodeGradientDirection2.BackgroundColor = Colors.Transparent;
            btnQRCodeGradientDirection3.BackgroundColor = Colors.Transparent;
            btnQRCodeGradientDirection4.BackgroundColor = Colors.Transparent;
            btnQRCodeGradientDirection5.BackgroundColor = Colors.Transparent;
            btnQRCodeGradientDirection6.BackgroundColor = Colors.Transparent;
            btnQRCodeGradientDirection7.BackgroundColor = Colors.Transparent;
            btnQRCodeGradientDirection8.BackgroundColor = Colors.Transparent;

            if (sender == btnQRCodeGradientDirection1)
            {
                ClassBarcodes.cQRCodeGradientDirection = "BottomToTop";
                btnQRCodeGradientDirection1.BackgroundColor = Color.FromArgb(cGradientDirectionBackgroundColor);
            }
            else if (sender == btnQRCodeGradientDirection2)
            {
                ClassBarcodes.cQRCodeGradientDirection = "BottomLeftToTopRight";
                btnQRCodeGradientDirection2.BackgroundColor = Color.FromArgb(cGradientDirectionBackgroundColor);
            }
            else if (sender == btnQRCodeGradientDirection3)
            {
                ClassBarcodes.cQRCodeGradientDirection = "LeftToRight";
                btnQRCodeGradientDirection3.BackgroundColor = Color.FromArgb(cGradientDirectionBackgroundColor);
            }
            else if (sender == btnQRCodeGradientDirection4)
            {
                ClassBarcodes.cQRCodeGradientDirection = "TopLeftToBottomRight";
                btnQRCodeGradientDirection4.BackgroundColor = Color.FromArgb(cGradientDirectionBackgroundColor);
            }
            else if (sender == btnQRCodeGradientDirection5)
            {
                ClassBarcodes.cQRCodeGradientDirection = "TopToBottom";
                btnQRCodeGradientDirection5.BackgroundColor = Color.FromArgb(cGradientDirectionBackgroundColor);
            }
            else if (sender == btnQRCodeGradientDirection6)
            {
                ClassBarcodes.cQRCodeGradientDirection = "TopRightToBottomLeft";
                btnQRCodeGradientDirection6.BackgroundColor = Color.FromArgb(cGradientDirectionBackgroundColor);
            }
            else if (sender == btnQRCodeGradientDirection7)
            {
                ClassBarcodes.cQRCodeGradientDirection = "RightToLeft";
                btnQRCodeGradientDirection7.BackgroundColor = Color.FromArgb(cGradientDirectionBackgroundColor);
            }
            else if (sender == btnQRCodeGradientDirection8)
            {
                ClassBarcodes.cQRCodeGradientDirection = "BottomRightToTopLeft";
                btnQRCodeGradientDirection8.BackgroundColor = Color.FromArgb(cGradientDirectionBackgroundColor);
            }
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
        /// Show a modal popup to inform the user about the recommended image size before opening the file picker
        /// </summary>
        /// <param name="cMessage"></param>
        /// <returns></returns>
        private static async Task OpenPopupColorPickerAsync(string cMessage)
        {
            // If the last character of the message is a : then remove it
            if (cMessage.EndsWith(':'))
            {
                cMessage = cMessage[..^1];
            }

            Page? currentPage = Application.Current?.Windows.Count > 0 ? Application.Current.Windows[0]?.Page : null;
            if (currentPage != null)
            {
                _ = await currentPage.ShowPopupAsync(new PopupColorPicker(cMessage));
            }
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
