using CommunityToolkit.Maui.Extensions;

namespace BarcodeGenerator
{
    public partial class ClassArtQRCodeSettings : ContentView
    {
        private readonly string cGradientDirectionBackgroundColor = "000099";  // navy blue

        public ClassArtQRCodeSettings()
    	{
    		InitializeComponent();

            // Get the current display information
            DisplayInfo displayInfo = DeviceDisplay.MainDisplayInfo;

            // Adjust the column widths based on the current orientation
            UpdateGridColumns(displayInfo.Orientation);

            // Set the initial states of the radio buttons based on the current QR Code module shape
            rbtQRCodeModuleShapeSquare.IsChecked = ClassBarcodes.cQRCodeModuleShape == "Square";
            rbtQRCodeModuleShapeRounded.IsChecked = ClassBarcodes.cQRCodeModuleShape == "Rounded";
            rbtQRCodeModuleShapeCircle.IsChecked = ClassBarcodes.cQRCodeModuleShape == "Circle";

            // Set the initial states of the switches based on the current settings
            swtForegroundImage.IsToggled = ClassBarcodes.bQRCodeForegroundImage;
            swtBackgroundImage.IsToggled = ClassBarcodes.bQRCodeBackgroundImage;
            swtQRCodeGradient.IsToggled = ClassBarcodes.bQRCodeGradientColor;

            // Set the current color in the box view
            bxvColorFgArtQRCode.Color = Color.FromArgb(ClassBarcodes.cCodeColorFgArtQRCode);
            bxvColorBgArtQRCode.Color = Color.FromArgb(ClassBarcodes.cCodeColorBgArtQRCode);
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

            DeviceDisplay.MainDisplayInfoChanged += OnMainDisplayInfoChanged;
            this.Unloaded += OnUnloaded;
        }

        /// <summary>
        /// Handles the event that occurs when the main display's information changes, such as orientation updates.
        /// </summary>
        /// <remarks>This method ensures that updates to the grid columns in response to display changes
        /// are performed on the main UI thread.</remarks>
        /// <param name="sender">The source of the event. This is typically the display information provider.</param>
        /// <param name="e">An object that contains the event data, including the updated display information.</param>
        private void OnMainDisplayInfoChanged(object? sender, DisplayInfoChangedEventArgs e)
        {
            MainThread.BeginInvokeOnMainThread(() => UpdateGridColumns(e.DisplayInfo.Orientation));
        }

        private void UpdateGridColumns(DisplayOrientation orientation)
        {
            grdSettingsArtQRCode.ColumnDefinitions.Clear();

            if (Globals.cCurrentPage == "PageSettings")
            {
                if (orientation == DisplayOrientation.Portrait)
                {
                    grdSettingsArtQRCode.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(150) });
                    grdSettingsArtQRCode.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(190) });
                }
                else // Landscape
                {
                    grdSettingsArtQRCode.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(250) });
                    grdSettingsArtQRCode.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(290) });
                }
            }
            else
            {
                if (orientation == DisplayOrientation.Portrait)
                {
                    grdSettingsArtQRCode.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(150) });
                    grdSettingsArtQRCode.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(190) });
                }
                else // Landscape
                {
                    grdSettingsArtQRCode.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(150) });
                    grdSettingsArtQRCode.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(190) });
                }
            }
        }

        /// <summary>
        /// Set focus to the close button when the popup is loaded, so that pressing Enter will close it immediately.
        /// </summary>
        /// <param name="sender">The source of the event, typically the popup that was loaded.</param>
        /// <param name="e">An EventArgs object that contains the event data.</param>
        private void Popup_Loaded(object sender, EventArgs e)
        {
            //btnButtonClose.Focus();
        }

        /// <summary>
        /// On button color forground clicked event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnButtonColorForgroundClicked(object sender, EventArgs e)
        {
            Globals.cCodeColor = ClassBarcodes.cCodeColorFgArtQRCode;
            await OpenPopupColorPickerAsync(CodeLang.ForegroundColorArtQRCode_Text);

            if (!Globals.bPopupCanceled)
            {
                ClassBarcodes.cCodeColorFgArtQRCode = Globals.cCodeColor;
                bxvColorFgArtQRCode.Color = Color.FromArgb(ClassBarcodes.cCodeColorFgArtQRCode);
            }
        }

        /// <summary>
        /// On button color background Art QR code clicked event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnButtonColorBackgroundArtQRCodeClicked(object sender, EventArgs e)
        {
            Globals.cCodeColor = ClassBarcodes.cCodeColorBgArtQRCode;
            await OpenPopupColorPickerAsync(CodeLang.BackgroundColorArtQRCode_Text);

            if (!Globals.bPopupCanceled)
            {
                ClassBarcodes.cCodeColorBgArtQRCode = Globals.cCodeColor;
                bxvColorBgArtQRCode.Color = Color.FromArgb(ClassBarcodes.cCodeColorBgArtQRCode);
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
        /// Unsubscribe from events when the popup is unloaded to prevent memory leaks and unintended behavior when the popup is closed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnUnloaded(object? sender, EventArgs e)
        {
            DeviceDisplay.MainDisplayInfoChanged -= OnMainDisplayInfoChanged;
            this.Unloaded -= OnUnloaded;
        }
    }
}