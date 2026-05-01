using CommunityToolkit.Maui.Views;
using static QRCoder.PayloadGenerator;

namespace BarcodeGenerator
{
    public partial class PopupPayloadTypes : Popup
    {
        public PopupPayloadTypes()
    	{
            InitializeComponent();

            // Indicate that the popup has been opened, which can be used to prevent certain actions in the MainPage OnAppearing event
            Globals.bPopupOpened = true;

            // Reset the popup canceled flag to false when the popup is opened
            Globals.bPopupCanceled = false;

            // Set the payload types in the picker
            pckPayloadType.ItemsSource = ClassPayloadTypes.GetQRCodePayloadTypes();

            // Set the geolocation encoding options in the picker to the first item (Google maps) by default
            pckGeolocationEncoding.SelectedIndex = Preferences.Default.Get("SettingGeolocationEncoding", 0);

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
            
            // Set the control properties based on the selected payload type
            SetControlsProperties(ClassPayloadTypes.cPayloadType);
        }

        /// <summary>
        /// Sets the properties of various controls based on the selected payload type name
        /// The method checks the selected payload type
        /// </summary>
        /// <param name="selectedName"></param>
        private void SetControlsProperties(string selectedName)
        {
            if (selectedName == ClassPayloadTypes.cPayloadType_WIFI)
            {
                lblPayloadTypeSSID.IsVisible = true;
                brdPayloadTypeSSID.IsVisible = true;
                lblPayloadTypePassword.IsVisible = true;
                brdPayloadTypePassword.IsVisible = true;
                _ = entPayloadTypeSSID.Focus();
            }
            else if (selectedName == ClassPayloadTypes.cPayloadType_URL)
            {
                lblPayloadTypeURL.IsVisible = true;
                brdPayloadTypeURL.IsVisible = true;
                _ = entPayloadTypeURL.Focus();
                entPayloadTypeURL.CursorPosition = entPayloadTypeURL.Text?.Length ?? 0; // Move cursor to the end of the text
            }
            else if (selectedName == ClassPayloadTypes.cPayloadType_BOOKMARK)
            {
                lblPayloadTypeURL.IsVisible = true;
                brdPayloadTypeURL.IsVisible = true;
                lblPayloadTypeTitle.IsVisible = true;
                brdPayloadTypeTitle.IsVisible = true;
                _ = entPayloadTypeURL.Focus();
                entPayloadTypeURL.CursorPosition = entPayloadTypeURL.Text?.Length ?? 0;
            }
            else if (selectedName == ClassPayloadTypes.cPayloadType_MAIL)
            {
                lblPayloadTypeReceiver.IsVisible = true;
                brdPayloadTypeReceiver.IsVisible = true;
                lblPayloadTypeSubject.IsVisible = true;
                brdPayloadTypeSubject.IsVisible = true;
                lblPayloadTypeMessage.IsVisible = true;
                brdPayloadTypeMessage.IsVisible = true;
                _ = entPayloadTypeReceiver.Focus();
            }
            else if (selectedName == ClassPayloadTypes.cPayloadType_SMS)
            {
                lblPayloadTypeNumber.IsVisible = true;
                brdPayloadTypeNumber.IsVisible = true;
                _ = entPayloadTypeNumber.Focus();
                entPayloadTypeNumber.CursorPosition = entPayloadTypeNumber.Text?.Length ?? 0;
            }
            else if (selectedName == ClassPayloadTypes.cPayloadType_MMS)
            {
                lblPayloadTypeNumber.IsVisible = true;
                brdPayloadTypeNumber.IsVisible = true;
                _ = entPayloadTypeNumber.Focus();
                entPayloadTypeNumber.CursorPosition = entPayloadTypeNumber.Text?.Length ?? 0;
            }
            else if (selectedName == ClassPayloadTypes.cPayloadType_GEOLOCATION)
            {
                brdGeolocationEncoding.IsVisible = true;
                lblPayloadTypeLatitude.IsVisible = true;
                brdPayloadTypeLatitude.IsVisible = true;
                lblPayloadTypeLongitude.IsVisible = true;
                brdPayloadTypeLongitude.IsVisible = true;
                lblPayloadTypeLatitudeDMS.IsVisible = true;
                lblPayloadTypeLongitudeDMS.IsVisible = true;
                brdPayloadTypeLatitudeDMSResult.IsVisible = true;
                brdPayloadTypeLongitudeDMSResult.IsVisible = true;
                btnButtonGeoLocation.IsVisible = true;
                btnButtonGeoMap.IsVisible = true;

                _ = entPayloadTypeLatitude.Focus();
                entPayloadTypeLatitude.CursorPosition = entPayloadTypeLatitude.Text?.Length ?? 0;
            }
            else if (selectedName == ClassPayloadTypes.cPayloadType_PHONENUMBER)
            {
                lblPayloadTypeNumber.IsVisible = true;
                brdPayloadTypeNumber.IsVisible = true;
                _ = entPayloadTypeNumber.Focus();
                entPayloadTypeNumber.CursorPosition = entPayloadTypeNumber.Text?.Length ?? 0;
            }
            else if (selectedName == ClassPayloadTypes.cPayloadType_WHATSAPPMESSAGE)
            {
                lblPayloadTypeMessage.IsVisible = true;
                brdPayloadTypeMessage.IsVisible = true;
                _ = entPayloadTypeMessage.Focus();
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
                _ = entPayloadTypeFirstname.Focus();
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
                _ = entPayloadTypeSubject.Focus();
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
                SetControlsProperties(ClassPayloadTypes.cPayloadType);
            }
        }

        /// <summary>
        /// Save the selected geolocation encoding option in the application preferences when the picker selection changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PckGeolocationEncoding_SelectedIndexChanged(object sender, EventArgs e)
        {
            Preferences.Default.Set("SettingGeolocationEncoding", pckGeolocationEncoding.SelectedIndex);
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
            brdGeolocationEncoding.IsVisible = false;
            lblPayloadTypeLatitude.IsVisible = false;
            brdPayloadTypeLatitude.IsVisible = false;
            lblPayloadTypeLongitude.IsVisible = false;
            brdPayloadTypeLongitude.IsVisible = false;
            lblPayloadTypeLatitudeDMS.IsVisible = false;
            lblPayloadTypeLongitudeDMS.IsVisible = false;
            brdPayloadTypeLatitudeDMSResult.IsVisible = false;
            brdPayloadTypeLongitudeDMSResult.IsVisible = false;
            btnButtonGeoLocation.IsVisible = false;
            btnButtonGeoMap.IsVisible = false;
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
            ClassPayloadTypes.cPayloadResult = await BuildPayload(ClassPayloadTypes.cPayloadType);

            // If the payload result is empty, it indicates that there was an error in generating the payload (e.g., invalid input).
            // In this case, do not close the popup and allow the user to correct their input.
            if (string.IsNullOrEmpty(ClassPayloadTypes.cPayloadResult))
            {
                return;
            }

            // If the payload result is not empty, proceed to close the popup
            await CloseAsync();
        }

        /// <summary>
        /// Displays the device's cached and current geographic location to the user in alert dialogs.
        /// </summary>
        /// <remarks>This method first attempts to retrieve a cached location and displays it if
        /// available. It then attempts to obtain the current location and displays the result. If either location is
        /// unavailable, an appropriate message is shown. This method is asynchronous and returns immediately; any
        /// exceptions thrown during location retrieval or display may not be observed by the caller.</remarks>
        public async void OnButtonGeoLocation_Clicked(object sender, EventArgs e)
        {
            if (!Microsoft.Maui.Devices.Sensors.Geolocation.IsEnabled)
            {
                await Application.Current!.Windows[0].Page!.DisplayAlertAsync(CodeLang.GeolocationTitle_Text, CodeLang.GeolocationMessage_Text, CodeLang.ButtonClose_Text);
                return;
            }
            
            Location? location;

            // First attempt to get the cached location, which may be faster and does not require a new location request.
            // If a cached location is available, display it immediately.
            //location = await ClassGeolocation.GetCachedLocation();
            //if (location != null)
            //{
            //    entPayloadTypeLatitude.Text = location.Latitude.ToString();
            //    entPayloadTypeLongitude.Text = location.Longitude.ToString();
            //}

            // Then attempt to get the current location, which may provide a more accurate and up-to-date location but may take longer and require user permission.
            // If a current location is available, display it and convert it to DMS format for display.
            // If no location is available, show an appropriate message.
            location = await new ClassGeolocation().GetCurrentLocation();
            if (location != null)
            {
                entPayloadTypeLatitude.Text = location.Latitude.ToString();
                entPayloadTypeLongitude.Text = location.Longitude.ToString();
            }

            // If a location was obtained (either cached or current), convert the latitude and longitude to DMS format for display.
            // If no location was obtained, show an appropriate message.
            if (location != null)
            {
                // Convert to DMS format for display
                lblPayloadTypeLatitudeDMSResult.Text = ClassGeolocation.DecimalToDMS(location.Latitude, true);
                lblPayloadTypeLongitudeDMSResult.Text = ClassGeolocation.DecimalToDMS(location.Longitude, false);
            }
            else
            {
                await Application.Current!.Windows[0].Page!.DisplayAlertAsync(CodeLang.GeolocationTitle_Text, CodeLang.GeolocationMessage_Text, CodeLang.ButtonClose_Text);
            }
        }

        /// <summary>
        /// On click event for the geolocation map button, which opens the device's default web browser to display
        /// the specified latitude and longitude on Google Maps
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnButtonGeoMap_Clicked(object sender, EventArgs e)
        {
            // Validate the latitude and longitude input values before attempting to open the map.
            // If the validation fails, the method will return early and not attempt to open the map.
            string url = await ValidateGeolocationValues();

            if (url == string.Empty)
            {
                return;
            }

            await Launcher.Default.OpenAsync(new Uri(url));
        }

        /// <summary>
        /// Handles the TextChanged event for the latitude input control and resets the associated DMS label.
        /// </summary>
        /// <param name="sender">The source of the event, typically the latitude input control.</param>
        /// <param name="e">The event data containing information about the text change.</param>
        private void EntPayloadTypeLatitude_TextChanged(object sender, TextChangedEventArgs e)
        {
            lblPayloadTypeLatitudeDMSResult.Text = string.Empty;
        }

        /// <summary>
        /// Handles the TextChanged event for the longitude input field and resets the associated DMS label.
        /// </summary>
        /// <param name="sender">The source of the event, typically the longitude input control.</param>
        /// <param name="e">The event data containing information about the text change.</param>
        private void EntPayloadTypeLongitude_TextChanged(object sender, TextChangedEventArgs e)
        {
            lblPayloadTypeLongitudeDMSResult.Text = string.Empty;
        }

        /// <summary>
        /// Handles the Unfocused event for the latitude entry field, validating and normalizing the input value to
        /// ensure it represents a valid latitude in decimal degrees.   
        /// </summary>
        /// <remarks>If the input is empty or invalid, the method displays an error message and returns
        /// focus to the entry field. The latitude value is constrained to the range -90 to 90 degrees. The method also
        /// updates the display of the latitude in degrees, minutes, and seconds (DMS) format.</remarks>
        /// <param name="sender">The source of the event, typically the latitude entry control.</param>
        /// <param name="e">The event data associated with the Unfocused event.</param>
        private async void EntPayloadTypeLatitude_Unfocused(object sender, FocusEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(entPayloadTypeLatitude.Text))
            {
                entPayloadTypeLatitude.Text = string.Empty;
            }

            // Replace the decimal comma with a decimal point in both values to ensure correct parsing regardless of the user's locale settings
            string latText = (entPayloadTypeLatitude.Text ?? string.Empty).Trim().Replace(',', '.');

            // Validate that the input can be parsed as a double using invariant culture to ensure consistent decimal separator handling
            if (double.TryParse(latText, NumberStyles.Float, CultureInfo.InvariantCulture, out double latitude))
            {
                lblPayloadTypeLatitudeDMSResult.Text = ClassGeolocation.DecimalToDMS(latitude, true);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void EntPayloadTypeLongitude_Unfocused(object sender, FocusEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(entPayloadTypeLongitude.Text))
            {
                entPayloadTypeLongitude.Text = string.Empty;
            }

            // Replace the decimal comma with a decimal point in both values to ensure correct parsing regardless of the user's locale settings
            string lonText = (entPayloadTypeLongitude.Text ?? string.Empty).Trim().Replace(',', '.');

            // Validate that the input can be parsed as a double using invariant culture to ensure consistent decimal separator handling
            if (double.TryParse(lonText, NumberStyles.Float, CultureInfo.InvariantCulture, out double longitude))
            {
                lblPayloadTypeLongitudeDMSResult.Text = ClassGeolocation.DecimalToDMS(longitude, false);
            }
        }

        /// <summary>
        /// Checks if the end date is earlier than the start date when the end date picker loses focus,
        /// and if so, sets the end date to match the start date to ensure a valid date range for calendar event payloads.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DtpPayloadTypeDate_Unfocused(object sender, FocusEventArgs e)
        {
            if (dtpPayloadTypeEndDate.Date < dtpPayloadTypeStartDate.Date)
            {
                dtpPayloadTypeEndDate.Date = dtpPayloadTypeStartDate.Date;
            }
        }

        /// <summary>
        /// Checks if the end time is earlier than the start time when the end time picker loses focus,
        /// and if so, sets the end time to match the start time to ensure a valid time range for calendar event payloads.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DtpPayloadTypeTime_Unfocused(object sender, FocusEventArgs e)
        {
            if (dtpPayloadTypeEndDate.Date == dtpPayloadTypeStartDate.Date && tmpPayloadTypeEndTime.Time < tmpPayloadTypeStartTime.Time)
            {
                tmpPayloadTypeEndTime.Time = tmpPayloadTypeStartTime.Time;
            }
        }

        /// <summary>
        /// Builds a QR code payload string based on the selected payload type name.
        /// The method generates the appropriate payload string for the QR code based on the selected payload type.
        /// </summary>
        /// <param name="selectedName">The name of the selected payload type.</param>
        /// <returns>The generated payload string for the QR code.</returns>
        /// <remarks>https://github.com/Shane32/QRCoder</remarks>
        public async Task<string> BuildPayload(string selectedName)
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
                // Validate the latitude and longitude input values and generate the geolocation payload string if valid.
                payload = await ValidateGeolocationValues();

                // If the payload is empty, it indicates that there was an error in validating the geolocation values (e.g., invalid latitude or longitude).
                if (payload == string.Empty)
                {
                    return string.Empty;
                }
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
                // https://github.com/Shane32/QRCoder
                DateTime startDate = dtpPayloadTypeStartDate.Date!.Value + tmpPayloadTypeStartTime.Time!.Value;
                DateTime endDate = dtpPayloadTypeEndDate.Date!.Value + tmpPayloadTypeEndTime.Time!.Value;

                if (startDate > endDate)
                {
                    await Application.Current!.Windows[0].Page!.DisplayAlertAsync(CodeLang.ErrorTitle_Text, CodeLang.ErrorStartdateLaterEnddate_Text, CodeLang.ButtonClose_Text);
                    return string.Empty;
                }

                // Format as YYYYMMDDThhmm00Z (UTC, no seconds = 00)
                string startUtc = startDate.ToUniversalTime().ToString("yyyyMMdd'T'HHmm'00Z'");
                string endUtc = endDate.ToUniversalTime().ToString("yyyyMMdd'T'HHmm'00Z'");

                Random random = new();
                payload = $@"BEGIN:VCALENDAR
VERSION:2.0
BEGIN:VEVENT
SUMMARY:{entPayloadTypeSubject.Text}
DESCRIPTION:{entPayloadTypeDescription.Text}
LOCATION:{entPayloadTypeLocation.Text}
DTSTART:{startUtc}
DTEND:{endUtc}
UID:{DateTime.UtcNow.Ticks + random.Next(1000, 9999)}
DTSTAMP:{DateTime.UtcNow:yyyyMMddTHHmmssZ}
END:VEVENT
END:VCALENDAR";
            }
            else
            {
                payload = string.Empty;
            }

            return payload;
        }

        /// <summary>
        /// Validates the latitude and longitude input values for geolocation payloads, ensuring they are properly formatted as decimal degrees and within valid ranges.
        /// If the inputs are valid, it generates a geolocation payload string; otherwise, it displays appropriate error messages and returns an empty string.
        /// </summary>
        /// <returns></returns>
        private async Task<string> ValidateGeolocationValues()
        {
            // Replace the decimal comma with a decimal point in both values to ensure correct parsing regardless of the user's locale settings
            string latText = (entPayloadTypeLatitude.Text ?? string.Empty).Trim().Replace(',', '.');
            string lonText = (entPayloadTypeLongitude.Text ?? string.Empty).Trim().Replace(',', '.');

            // Validate that the inputs can be parsed as doubles using invariant culture to ensure consistent decimal separator handling
            if (!double.TryParse(latText, NumberStyles.Float, CultureInfo.InvariantCulture, out double latitude))
            {
                await Application.Current!.Windows[0].Page!.DisplayAlertAsync(CodeLang.ErrorTitle_Text, CodeLang.ErrorLatitudeInvalid_Text, CodeLang.ButtonClose_Text);
                _ = entPayloadTypeLatitude.Focus();
                return string.Empty;
            }

            if (!double.TryParse(lonText, NumberStyles.Float, CultureInfo.InvariantCulture, out double longitude))
            {
                await Application.Current!.Windows[0].Page!.DisplayAlertAsync(CodeLang.ErrorTitle_Text, CodeLang.ErrorLongitudeInvalid_Text, CodeLang.ButtonClose_Text);
                _ = entPayloadTypeLongitude.Focus();
                return string.Empty;
            }

            // Validate inclusive min/max bounds
            if (latitude < -90.0 || latitude > 90.0)
            {
                await Application.Current!.Windows[0].Page!.DisplayAlertAsync(CodeLang.ErrorTitle_Text, CodeLang.ErrorLatitudeRange_Text, CodeLang.ButtonClose_Text);
                _ = entPayloadTypeLatitude.Focus();
                return string.Empty;
            }

            if (longitude < -180.0 || longitude > 180.0)
            {
                await Application.Current!.Windows[0].Page!.DisplayAlertAsync(CodeLang.ErrorTitle_Text, CodeLang.ErrorLongitudeRange_Text, CodeLang.ButtonClose_Text);
                _ = entPayloadTypeLongitude.Focus();
                return string.Empty;
            }

            // Construct the Google Maps URL using the validated latitude and longitude values.
            // Use invariant string formatting to ensure consistent decimal separator expected by payload generator
            string url = string.Empty;

            switch (pckGeolocationEncoding.SelectedIndex)
            {
                case 0:
                    // GeolocationEncoding = GoogleMaps
                    url = $"http://maps.google.com/maps?q={latitude.ToString(CultureInfo.InvariantCulture)},{longitude.ToString(CultureInfo.InvariantCulture)}";
                    break;
                case 1:
                    // GeolocationEncoding = GEO
                    url = $"geo:{latitude.ToString(CultureInfo.InvariantCulture)},{longitude.ToString(CultureInfo.InvariantCulture)}";
                    break;
            }

            return url;
        }
    }
}