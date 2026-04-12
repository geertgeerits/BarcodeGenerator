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

            DeviceDisplay.MainDisplayInfoChanged += OnMainDisplayInfoChanged;
            this.Unloaded += OnUnloaded;

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

        /// <summary>
        /// Updates the column definitions of the settings grid based on the specified display orientation and current
        /// device type.
        /// </summary>
        /// <remarks>This method adjusts the layout of the settings grid to provide an optimal user
        /// experience for different device types (phone, tablet, desktop) and display orientations (portrait or
        /// landscape). The changes are applied only to the relevant settings page or popup, depending on the current
        /// context.</remarks>
        /// <param name="orientation">The orientation of the display, which determines how the grid columns are configured.</param>
        private void UpdateGridColumns(DisplayOrientation orientation)
        {
            // Clear existing column definitions to avoid conflicts and ensure the new layout is applied correctly.
            grdSettingsArtQRCode.ColumnDefinitions.Clear();

            // Set column widths based on the current page, device type, and orientation to optimize the layout for user interaction
            switch (Globals.cCurrentPage)
            {
                // cCurrentPage = PageSettings
                case "PageSettings":
                    if (DeviceInfo.Idiom == DeviceIdiom.Phone)
                    {
                        switch (orientation)
                        {
                            case DisplayOrientation.Portrait:
                                grdSettingsArtQRCode.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(150) });
                                grdSettingsArtQRCode.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(190) });
                                break;
                            default:
                                grdSettingsArtQRCode.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(250) });
                                grdSettingsArtQRCode.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(290) });
                                break;
                        }
                    }
                    else if (DeviceInfo.Idiom == DeviceIdiom.Tablet)  // For tablets
                    {
                        switch (orientation)
                        {
                            case DisplayOrientation.Portrait:
                                grdSettingsArtQRCode.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(290) });
                                grdSettingsArtQRCode.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(330) });
                                break;
                            default:
                                grdSettingsArtQRCode.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(390) });
                                grdSettingsArtQRCode.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(430) });
                                break;
                        }
                    }
                    else  // Desktop
                    {
                        switch (orientation)
                        {
                            case DisplayOrientation.Portrait:
                                grdSettingsArtQRCode.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(290) });
                                grdSettingsArtQRCode.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(330) });
                                break;
                            default:
                                grdSettingsArtQRCode.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(390) });
                                grdSettingsArtQRCode.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(430) });
                                break;
                        }
                    }
                    break;
                // cCurrentPage = PopupSettingsArtQRCode
                // The Style 'scrollviewStylePopup' is applied to the scroll view in the popup, so these column widths will be used when the popup is open
                default:
                    if (DeviceInfo.Idiom == DeviceIdiom.Phone)
                    {
                        switch (orientation)
                        {
                            case DisplayOrientation.Portrait:
                                grdSettingsArtQRCode.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(150) });
                                grdSettingsArtQRCode.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(190) });
                                break;
                            default:
                                grdSettingsArtQRCode.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(250) });
                                grdSettingsArtQRCode.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(290) });
                                break;
                        }
                    }
                    else if (DeviceInfo.Idiom == DeviceIdiom.Tablet)
                    {
                        switch (orientation)
                        {
                            case DisplayOrientation.Portrait:
                                grdSettingsArtQRCode.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(250) });
                                grdSettingsArtQRCode.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(290) });
                                break;
                            default:
                                grdSettingsArtQRCode.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(250) });
                                grdSettingsArtQRCode.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(290) });
                                break;
                        }
                    }
                    else  // Desktop
                    {
                        switch (orientation)
                        {
                            case DisplayOrientation.Portrait:
                                grdSettingsArtQRCode.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(250) });
                                grdSettingsArtQRCode.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(290) });
                                break;
                            default:
                                grdSettingsArtQRCode.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(250) });
                                grdSettingsArtQRCode.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(290) });
                                break;
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// On button color forground clicked event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnButtonColorForgroundClicked(object sender, EventArgs e)
        {
            ClassBarcodes.cCodeColor = ClassBarcodes.cCodeColorFgArtQRCode;
            await OpenPopupColorPickerAsync(CodeLang.ForegroundColorArtQRCode_Text);

            if (!Globals.bPopupCanceled)
            {
                ClassBarcodes.cCodeColorFgArtQRCode = ClassBarcodes.cCodeColor;
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
            ClassBarcodes.cCodeColor = ClassBarcodes.cCodeColorBgArtQRCode;
            await OpenPopupColorPickerAsync(CodeLang.BackgroundColorArtQRCode_Text);

            if (!Globals.bPopupCanceled)
            {
                ClassBarcodes.cCodeColorBgArtQRCode = ClassBarcodes.cCodeColor;
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
            ClassBarcodes.cCodeColor = ClassBarcodes.cQRCodeGradientColor1;
            await OpenPopupColorPickerAsync(CodeLang.QRCodeGradientColor1_Text);

            if (!Globals.bPopupCanceled)
            {
                ClassBarcodes.cQRCodeGradientColor1 = ClassBarcodes.cCodeColor;
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
            ClassBarcodes.cCodeColor = ClassBarcodes.cQRCodeGradientColor2;
            await OpenPopupColorPickerAsync(CodeLang.QRCodeGradientColor2_Text);

            if (!Globals.bPopupCanceled)
            {
                ClassBarcodes.cQRCodeGradientColor2 = ClassBarcodes.cCodeColor;
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
            ClassBarcodes.cCodeColor = ClassBarcodes.cQRCodeGradientColor3;
            await OpenPopupColorPickerAsync(CodeLang.QRCodeGradientColor3_Text);

            if (!Globals.bPopupCanceled)
            {
                ClassBarcodes.cQRCodeGradientColor3 = ClassBarcodes.cCodeColor;
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