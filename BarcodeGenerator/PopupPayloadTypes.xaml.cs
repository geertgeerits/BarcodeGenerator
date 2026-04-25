using CommunityToolkit.Maui.Views;
using static QRCoder.PayloadGenerator;
using Geolocation = QRCoder.PayloadGenerator.Geolocation;

namespace BarcodeGenerator
{
    public partial class PopupPayloadTypes : Popup
    {
    	public PopupPayloadTypes()
    	{
            InitializeComponent();

            // Get the current display information
            DisplayInfo displayInfo = DeviceDisplay.MainDisplayInfo;

            // Adjust the column widths based on the current orientation
            UpdateGridColumns(displayInfo.Orientation);

            DeviceDisplay.MainDisplayInfoChanged += OnMainDisplayInfoChanged;
            this.Unloaded += OnUnloaded;

            // Indicate that the popup has been opened, which can be used to prevent certain actions in the MainPage OnAppearing event
            Globals.bPopupOpened = true;

            // Set the payload types in the picker
            pckPayloadType.ItemsSource = ClassPayloadTypes.GetQRCodePayloadTypes();

            // Select the current barcode format in the picker for the barcode generator and scanner
            ClassPayloadTypes.SelectPayloadTypeIndex(pckPayloadType);

            // Set default date and time for calendar event payload type to the current date and time
            dtpPayloadTypeStartDate.Date = DateTime.Now.Date;
            dtpPayloadTypeEndDate.Date = DateTime.Now.Date;
            tmpPayloadTypeStartTime.Time = DateTime.Now.TimeOfDay;
            tmpPayloadTypeEndTime.Time = DateTime.Now.TimeOfDay;
            dtpPayloadTypeStartDate.Format = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
            dtpPayloadTypeEndDate.Format = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
            tmpPayloadTypeStartTime.Format = CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern;
            tmpPayloadTypeEndTime.Format = CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern;

            // Initially set the visibility of all controls to false
            SetControlsVisibilityFalse();
            
            // Set the visibility of controls based on the selected payload type
            SetControlsVisibilityTrue(ClassPayloadTypes.cPayloadType);
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
        /// Sets the visibility of various controls based on the selected payload type name
        /// The method checks the selected payload type
        /// </summary>
        /// <param name="selectedName"></param>
        private void SetControlsVisibilityTrue(string selectedName)
        {
            if (selectedName == ClassPayloadTypes.cPayloadType_WIFI)
            {
                lblPayloadTypeSSID.IsVisible = true;
                brdPayloadTypeSSID.IsVisible = true;
                lblPayloadTypePassword.IsVisible = true;
                brdPayloadTypePassword.IsVisible = true;
            }
            else if (selectedName == ClassPayloadTypes.cPayloadType_URL)
            {
                lblPayloadTypeURL.IsVisible = true;
                brdPayloadTypeURL.IsVisible = true;
            }
            else if (selectedName == ClassPayloadTypes.cPayloadType_BOOKMARK)
            {
                lblPayloadTypeURL.IsVisible = true;
                brdPayloadTypeURL.IsVisible = true;
                lblPayloadTypeTitle.IsVisible = true;
                brdPayloadTypeTitle.IsVisible = true;
            }
            else if (selectedName == ClassPayloadTypes.cPayloadType_MAIL)
            {
                lblPayloadTypeReceiver.IsVisible = true;
                brdPayloadTypeReceiver.IsVisible = true;
                lblPayloadTypeSubject.IsVisible = true;
                brdPayloadTypeSubject.IsVisible = true;
                lblPayloadTypeMessage.IsVisible = true;
                brdPayloadTypeMessage.IsVisible = true;
            }
            else if (selectedName == ClassPayloadTypes.cPayloadType_SMS)
            {
                lblPayloadTypeNumber.IsVisible = true;
                brdPayloadTypeNumber.IsVisible = true;
            }
            else if (selectedName == ClassPayloadTypes.cPayloadType_MMS)
            {
                lblPayloadTypeNumber.IsVisible = true;
                brdPayloadTypeNumber.IsVisible = true;
            }
            else if (selectedName == ClassPayloadTypes.cPayloadType_GEOLOCATION)
            {
                lblPayloadTypeLatitude.IsVisible = true;
                brdPayloadTypeLatitude.IsVisible = true;
                lblPayloadTypeLongitude.IsVisible = true;
                brdPayloadTypeLongitude.IsVisible = true;
            }
            else if (selectedName == ClassPayloadTypes.cPayloadType_PHONENUMBER)
            {
                lblPayloadTypeNumber.IsVisible = true;
                brdPayloadTypeNumber.IsVisible = true;
            }
            else if (selectedName == ClassPayloadTypes.cPayloadType_WHATSAPPMESSAGE)
            {
                lblPayloadTypeMessage.IsVisible = true;
                brdPayloadTypeMessage.IsVisible = true;
            }
            else if (selectedName == ClassPayloadTypes.cPayloadType_CONTACTDATA)
            {
                lblPayloadTypeFirstname.IsVisible = true;
                brdPayloadTypeFirstname.IsVisible = true;
                lblPayloadTypeLastname.IsVisible = true;
                brdPayloadTypeLastname.IsVisible = true;
                lblPayloadTypeNumber.IsVisible = true;
                brdPayloadTypeNumber.IsVisible = true;
                lblPayloadTypeMail.IsVisible = true;
                brdPayloadTypeMail.IsVisible = true;
            }
            else if (selectedName == ClassPayloadTypes.cPayloadType_CALENDAREVENT)
            {
                lblPayloadTypeSubject.IsVisible = true;
                brdPayloadTypeSubject.IsVisible = true;
                lblPayloadTypeDescription.IsVisible = true;
                brdPayloadTypeDescription.IsVisible = true;
                lblPayloadTypeLocation.IsVisible = true;
                brdPayloadTypeLocation.IsVisible = true;
                lblPayloadTypeStart.IsVisible = true;
                brdPayloadTypeStartDate.IsVisible = true;
                brdPayloadTypeStartTime.IsVisible = true;
                lblPayloadTypeEnd.IsVisible = true;
                brdPayloadTypeEndDate.IsVisible = true;
                brdPayloadTypeEndTime.IsVisible = true;
            }
        }

        /// <summary>
        /// Picker payload type clicked event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPickerPayloadTypeChanged(object sender, EventArgs e)
        {
            Picker picker = (Picker)sender;
            int selectedIndex = picker.SelectedIndex;

            if (selectedIndex != -1)
            {
                ClassPayloadTypes.nPayloadTypeIndex = selectedIndex;
                ClassPayloadTypes.cPayloadType = pckPayloadType.Items[ClassPayloadTypes.nPayloadTypeIndex];

                SetControlsVisibilityFalse();
                SetControlsVisibilityTrue(ClassPayloadTypes.cPayloadType);
            }
        }

        /// <summary>
        /// Sets the visibility of all relevant controls to false, effectively hiding them from the user interface.
        /// </summary>
        /// <remarks>Call this method to ensure that none of the associated controls are visible. This can
        /// be useful when resetting the UI or preparing it for a different state.</remarks>
        private void SetControlsVisibilityFalse()
        {
            lblPayloadTypeSSID.IsVisible = false;
            brdPayloadTypeSSID.IsVisible = false;
            lblPayloadTypePassword.IsVisible = false;
            brdPayloadTypePassword.IsVisible = false;
            lblPayloadTypeURL.IsVisible = false;
            brdPayloadTypeURL.IsVisible = false;
            lblPayloadTypeTitle.IsVisible = false;
            brdPayloadTypeTitle.IsVisible = false;
            lblPayloadTypeReceiver.IsVisible = false;
            brdPayloadTypeReceiver.IsVisible = false;
            lblPayloadTypeSubject.IsVisible = false;
            brdPayloadTypeSubject.IsVisible = false;
            lblPayloadTypeMessage.IsVisible = false;
            brdPayloadTypeMessage.IsVisible = false;
            lblPayloadTypeLatitude.IsVisible = false;
            brdPayloadTypeLatitude.IsVisible = false;
            lblPayloadTypeLongitude.IsVisible = false;
            brdPayloadTypeLongitude.IsVisible = false;
            lblPayloadTypeFirstname.IsVisible = false;
            brdPayloadTypeFirstname.IsVisible = false;
            lblPayloadTypeLastname.IsVisible = false;
            brdPayloadTypeLastname.IsVisible = false;
            lblPayloadTypeNumber.IsVisible = false;
            brdPayloadTypeNumber.IsVisible = false;
            lblPayloadTypeMail.IsVisible = false;
            brdPayloadTypeMail.IsVisible = false;
            lblPayloadTypeDescription.IsVisible = false;
            brdPayloadTypeDescription.IsVisible = false;
            brdPayloadTypeLocation.IsVisible = false;
            lblPayloadTypeLocation.IsVisible = false;
            lblPayloadTypeStart.IsVisible = false;
            brdPayloadTypeStartDate.IsVisible = false;
            brdPayloadTypeStartTime.IsVisible = false;
            lblPayloadTypeEnd.IsVisible = false;
            brdPayloadTypeEndDate.IsVisible = false;
            brdPayloadTypeEndTime.IsVisible = false;
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
            // Generate the payload result based on the selected payload type and user input, and then close the popup
            ClassPayloadTypes.cPayloadResult = BuildPayload(ClassPayloadTypes.cPayloadType);

            await CloseAsync();
        }

        /// <summary>
        /// Builds a QR code payload string based on the selected payload type name. The method generates the appropriate
        /// payload string for the QR code based on the selected payload type.
        /// </summary>
        /// <param name="selectedName">The name of the selected payload type.</param>
        /// <returns>The generated payload string for the QR code.</returns>
        /// <remarks>https://github.com/Shane32/QRCoder</remarks>
        public string BuildPayload(string selectedName)
        {
            string payload;

            if (selectedName == ClassPayloadTypes.cPayloadType_WIFI)
            {
                WiFi generator = new(ssid: entPayloadTypeSSID.Text, password: entPayloadTypePassword.Text, authenticationMode: WiFi.Authentication.WPA);
                payload = generator.ToString();
            }
            else if (selectedName == ClassPayloadTypes.cPayloadType_URL)
            {
                Url generator = new(url: entPayloadTypeURL.Text);
                payload = generator.ToString();
            }
            else if (selectedName == ClassPayloadTypes.cPayloadType_BOOKMARK)
            {
                Bookmark generator = new(url: entPayloadTypeURL.Text, title: entPayloadTypeTitle.Text);
                payload = generator.ToString();
            }
            else if (selectedName == ClassPayloadTypes.cPayloadType_MAIL)
            {
                Mail generator = new(mailReceiver: entPayloadTypeReceiver.Text, subject: entPayloadTypeSubject.Text, message: entPayloadTypeMessage.Text);
                payload = generator.ToString();
            }
            else if (selectedName == ClassPayloadTypes.cPayloadType_SMS)
            {
                SMS generator = new(number: entPayloadTypeNumber.Text);
                payload = generator.ToString();
            }
            else if (selectedName == ClassPayloadTypes.cPayloadType_MMS)
            {
                MMS generator = new(number: entPayloadTypeNumber.Text);
                payload = generator.ToString();
            }
            else if (selectedName == ClassPayloadTypes.cPayloadType_GEOLOCATION)
            {
                Geolocation generator = new(latitude: entPayloadTypeLatitude.Text, longitude: entPayloadTypeLongitude.Text);
                payload = generator.ToString();
            }
            else if (selectedName == ClassPayloadTypes.cPayloadType_PHONENUMBER)
            {
                PhoneNumber generator = new(number: entPayloadTypeNumber.Text);
                payload = generator.ToString();
            }
            else if (selectedName == ClassPayloadTypes.cPayloadType_WHATSAPPMESSAGE)
            {
                WhatsAppMessage generator = new(message: entPayloadTypeMessage.Text);
                payload = generator.ToString();
            }
            else if (selectedName == ClassPayloadTypes.cPayloadType_CONTACTDATA)
            {
                ContactData generator = new(ContactData.ContactOutputType.VCard3, firstname: entPayloadTypeFirstname.Text, lastname: entPayloadTypeLastname.Text, nickname: "", phone: "", mobilePhone: entPayloadTypeNumber.Text, workPhone: "", email: entPayloadTypeMail.Text);
                payload = generator.ToString();
            }
            else if (selectedName == ClassPayloadTypes.cPayloadType_CALENDAREVENT)
            {
                DateTime startDate = dtpPayloadTypeStartDate.Date!.Value + tmpPayloadTypeStartTime.Time!.Value;
                DateTime endDate = dtpPayloadTypeEndDate.Date!.Value + tmpPayloadTypeEndTime.Time!.Value;
                CalendarEvent generator = new(subject: entPayloadTypeSubject.Text, description: entPayloadTypeDescription.Text, location: entPayloadTypeLocation.Text, start: new DateTimeOffset(startDate), end: new DateTimeOffset(endDate), allDayEvent: false);
                payload = generator.ToString();
            }
            else
            {
                payload = string.Empty;
            }

            return payload;
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
            grdSettingsPayloadTypes.ColumnDefinitions.Clear();

            // Set column widths based on the current page, device type, and orientation to optimize the layout for user interaction
            // The Style 'scrollviewStylePopup' is applied to the scroll view in the popup, so these column widths will be used when the popup is open
            if (DeviceInfo.Idiom == DeviceIdiom.Phone)
            {
                switch (orientation)
                {
                    case DisplayOrientation.Portrait:
                        grdSettingsPayloadTypes.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(150) });
                        grdSettingsPayloadTypes.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(190) });
                        break;
                    default:
                        grdSettingsPayloadTypes.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(250) });
                        grdSettingsPayloadTypes.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(290) });
                        break;
                }
            }
            else if (DeviceInfo.Idiom == DeviceIdiom.Tablet)
            {
                switch (orientation)
                {
                    case DisplayOrientation.Portrait:
                        grdSettingsPayloadTypes.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(250) });
                        grdSettingsPayloadTypes.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(290) });
                        break;
                    default:
                        grdSettingsPayloadTypes.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(250) });
                        grdSettingsPayloadTypes.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(290) });
                        break;
                }
            }
            else  // Desktop
            {
                switch (orientation)
                {
                    case DisplayOrientation.Portrait:
                        grdSettingsPayloadTypes.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(250) });
                        grdSettingsPayloadTypes.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(290) });
                        break;
                    default:
                        grdSettingsPayloadTypes.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(250) });
                        grdSettingsPayloadTypes.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(290) });
                        break;
                }
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