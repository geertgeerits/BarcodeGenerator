using CommunityToolkit.Maui.Views;
using QRCoder;
using System.ComponentModel.DataAnnotations;
using System.Numerics;
using System.Text.RegularExpressions;
using static QRCoder.PayloadGenerator;
using static QRCoder.PayloadGenerator.Girocode;

namespace BarcodeGenerator
{
    public partial class PopupPayloadTypes : Popup
    {
        public static ImageSource? qrCodeImage;
        public static bool bPayloadSepaCreditTransfer;

        public PopupPayloadTypes()
    	{
            InitializeComponent();
#if IOS
            // Configure the numeric keyboard for these entry fields on iOS to use the NumbersAndPunctuation keyboard type
            BarcodeGenerator.Platforms.iOS.NumericKeyboardHandler.ConfigureNumbersAndPunctuation(entPayloadTypeLatitude);
            BarcodeGenerator.Platforms.iOS.NumericKeyboardHandler.ConfigureNumbersAndPunctuation(entPayloadTypeLongitude);
#endif
            // Ensure any existing barcode files are deleted before generating new ones to avoid confusion and manage storage
            ClassFileOperations.DeleteFileIfExists(ClassBarcodes.cFileBarcodePng);
            ClassFileOperations.DeleteFileIfExists(ClassBarcodes.cFileBarcodeSvg);

            // Indicate that the popup has been opened, which can be used to prevent certain actions in the MainPage OnAppearing event
            Globals.bPopupOpened = true;

            // Reset the popup canceled flag to false when the popup is opened
            Globals.bPopupCanceled = false;

            // Set the payload types in the picker
            pckPayloadType.ItemsSource = ClassPayloadTypes.GetQRCodePayloadTypes();

            // Set the authentication options in the picker to the second item (WPA) by default
            pckWiFiAuthentication.SelectedIndex = Preferences.Default.Get("SettingWiFiAuthentication", 1);

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
                lblWiFiAuthentication.IsVisible = true;
                brdWiFiAuthentication.IsVisible = true;
                brdPayloadTypeSSID.IsVisible = true;
                brdPayloadTypePassword.IsVisible = true;
                _ = entPayloadTypeSSID.Focus();
            }
            else if (selectedName == ClassPayloadTypes.cPayloadType_URL)
            {
                brdPayloadTypeURL.IsVisible = true;
                btnButtonURL.IsVisible = true;
                _ = entPayloadTypeURL.Focus();
                entPayloadTypeURL.CursorPosition = entPayloadTypeURL.Text?.Length ?? 0; // Move cursor to the end of the text
            }
            else if (selectedName == ClassPayloadTypes.cPayloadType_BOOKMARK)
            {
                brdPayloadTypeURL.IsVisible = true;
                brdPayloadTypeTitle.IsVisible = true;
                btnButtonURL.IsVisible = true;
                _ = entPayloadTypeURL.Focus();
                entPayloadTypeURL.CursorPosition = entPayloadTypeURL.Text?.Length ?? 0;
            }
            else if (selectedName == ClassPayloadTypes.cPayloadType_MAIL)
            {
                brdPayloadTypeReceiver.IsVisible = true;
                brdPayloadTypeSubject.IsVisible = true;
                brdPayloadTypeMessage.IsVisible = true;
                _ = entPayloadTypeReceiver.Focus();
            }
            else if (selectedName == ClassPayloadTypes.cPayloadType_SMS)
            {
                brdPayloadTypePhoneNumber.IsVisible = true;
                brdPayloadTypeMessage.IsVisible = true;
                _ = entPayloadTypePhoneNumber.Focus();
                entPayloadTypePhoneNumber.CursorPosition = entPayloadTypePhoneNumber.Text?.Length ?? 0;
            }
            else if (selectedName == ClassPayloadTypes.cPayloadType_MMS)
            {
                brdPayloadTypePhoneNumber.IsVisible = true;
                brdPayloadTypeMessage.IsVisible = true;
                _ = entPayloadTypePhoneNumber.Focus();
                entPayloadTypePhoneNumber.CursorPosition = entPayloadTypePhoneNumber.Text?.Length ?? 0;
            }
            else if (selectedName == ClassPayloadTypes.cPayloadType_GEOLOCATION)
            {
                lblGeolocationEncoding.IsVisible = true;
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
                brdPayloadTypePhoneNumber.IsVisible = true;
                _ = entPayloadTypePhoneNumber.Focus();
                entPayloadTypePhoneNumber.CursorPosition = entPayloadTypePhoneNumber.Text?.Length ?? 0;
            }
            else if (selectedName == ClassPayloadTypes.cPayloadType_WHATSAPPMESSAGE)
            {
                brdPayloadTypePhoneNumber.IsVisible = true;
                brdPayloadTypeMessage.IsVisible = true;
                _ = entPayloadTypePhoneNumber.Focus();
            }
            else if (selectedName == ClassPayloadTypes.cPayloadType_CONTACTDATA)
            {
                brdPayloadTypeFirstname.IsVisible = true;
                brdPayloadTypeLastname.IsVisible = true;
                brdPayloadTypePhoneNumber.IsVisible = true;
                brdPayloadTypeMail.IsVisible = true;
                _ = entPayloadTypeFirstname.Focus();
            }
            else if (selectedName == ClassPayloadTypes.cPayloadType_CALENDAREVENT)
            {
                brdPayloadTypeSubject.IsVisible = true;
                brdPayloadTypeDescription.IsVisible = true;
                brdPayloadTypeLocation.IsVisible = true;
                lblPayloadTypeStart.IsVisible = true;
                brdPayloadTypeStartDate.IsVisible = true;
                brdPayloadTypeStartTime.IsVisible = true;
                lblPayloadTypeEnd.IsVisible = true;
                brdPayloadTypeEndDate.IsVisible = true;
                brdPayloadTypeEndTime.IsVisible = true;
                _ = entPayloadTypeSubject.Focus();
            }
            else if (selectedName == ClassPayloadTypes.cPayloadType_SEPACREDITTRANSFER)
            {
                brdPayloadTypeSctBic.IsVisible = true;
                brdPayloadTypeSctName.IsVisible = true;
                brdPayloadTypeSctIban.IsVisible = true;
                brdPayloadTypeSctAmountEur.IsVisible = true;
                brdPayloadTypeSctPurpose.IsVisible = true;
                brdPayloadTypeSctRemittanceInfoStructured.IsVisible = true;
                brdPayloadTypeSctRemittanceInfoUnstructured.IsVisible = true;
                brdPayloadTypeSctInformation.IsVisible = true;
                _ = entPayloadTypeSctBic.Focus();
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

        private void PckWiFiAuthentication_SelectedIndexChanged(object sender, EventArgs e)
        {
            Preferences.Default.Set("SettingWiFiAuthentication", pckWiFiAuthentication.SelectedIndex);
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
            lblWiFiAuthentication.IsVisible = false;
            brdWiFiAuthentication.IsVisible = false;
            brdPayloadTypeSSID.IsVisible = false;
            brdPayloadTypePassword.IsVisible = false;
            brdPayloadTypeURL.IsVisible = false;
            brdPayloadTypeTitle.IsVisible = false;
            brdPayloadTypeReceiver.IsVisible = false;
            brdPayloadTypeSubject.IsVisible = false;
            brdPayloadTypeMessage.IsVisible = false;
            lblGeolocationEncoding.IsVisible = false;
            brdGeolocationEncoding.IsVisible = false;
            lblPayloadTypeLatitude.IsVisible = false;
            brdPayloadTypeLatitude.IsVisible = false;
            lblPayloadTypeLongitude.IsVisible = false;
            brdPayloadTypeLongitude.IsVisible = false;
            lblPayloadTypeLatitudeDMS.IsVisible = false;
            lblPayloadTypeLongitudeDMS.IsVisible = false;
            brdPayloadTypeLatitudeDMSResult.IsVisible = false;
            brdPayloadTypeLongitudeDMSResult.IsVisible = false;
            brdPayloadTypeFirstname.IsVisible = false;
            brdPayloadTypeLastname.IsVisible = false;
            brdPayloadTypePhoneNumber.IsVisible = false;
            brdPayloadTypeMail.IsVisible = false;
            brdPayloadTypeDescription.IsVisible = false;
            brdPayloadTypeLocation.IsVisible = false;
            lblPayloadTypeStart.IsVisible = false;
            brdPayloadTypeStartDate.IsVisible = false;
            brdPayloadTypeStartTime.IsVisible = false;
            lblPayloadTypeEnd.IsVisible = false;
            brdPayloadTypeEndDate.IsVisible = false;
            brdPayloadTypeEndTime.IsVisible = false;
            brdPayloadTypeSctBic.IsVisible = false;
            brdPayloadTypeSctName.IsVisible = false;
            brdPayloadTypeSctIban.IsVisible = false;
            brdPayloadTypeSctAmountEur.IsVisible = false;
            brdPayloadTypeSctPurpose.IsVisible = false;
            brdPayloadTypeSctRemittanceInfoStructured.IsVisible = false;
            brdPayloadTypeSctRemittanceInfoUnstructured.IsVisible = false;
            brdPayloadTypeSctInformation.IsVisible = false;

            btnButtonGeoLocation.IsVisible = false;
            btnButtonGeoMap.IsVisible = false;
            btnButtonURL.IsVisible = false;
        }

        /// <summary>
        /// Handles the focus event for the payload type phone number entry field, initializing it with a default plus sign
        /// if it is empty.
        /// </summary>
        /// <param name="sender">The source of the event, typically the entry field that received focus.</param>
        /// <param name="e">The event data associated with the focus event.</param>
        private void EntPayloadTypePhoneNumber_Focused(object sender, FocusEventArgs e)
        {
            if (entPayloadTypePhoneNumber.Text == string.Empty)
            {
                entPayloadTypePhoneNumber.Text = "+";
                entPayloadTypePhoneNumber.CursorPosition = entPayloadTypePhoneNumber.Text.Length; // Move cursor to the end of the text
            }
        }

        /// <summary>
        /// Handles the focus event for the payload type URL entry field, initializing it with a default URL if it is empty.
        /// </summary>
        /// <remarks>If the entry field is empty when it receives focus, this method sets its text to a
        /// default URL and moves the cursor to the end.</remarks>
        /// <param name="sender">The source of the event, typically the entry field that received focus.</param>
        /// <param name="e">The event data associated with the focus event.</param>
        private void EntPayloadTypeURL_Focused(object sender, FocusEventArgs e)
        {
            if (entPayloadTypeURL.Text == string.Empty)
            {
                entPayloadTypeURL.Text = "http://";
                entPayloadTypeURL.CursorPosition = entPayloadTypeURL.Text.Length; // Move cursor to the end of the text
            }
        }

        /// <summary>
        /// Open the URL specified in the URL entry field using the device's default web browser when the associated button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnButtonURL_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (await IsValidUrl(entPayloadTypeURL.Text))
                {
#if ANDROID
                    // !!!BUGG!!! in Android - System.MissingMethodException - Method not found: void AndroidX.Core.View.Accessibility.AccessibilityNodeInfoCompat.set_Checked(bool)
                    await Browser.Default.OpenAsync(new Uri(entPayloadTypeURL.Text), BrowserLaunchMode.SystemPreferred);
#else
                    await Launcher.Default.OpenAsync(new Uri(entPayloadTypeURL.Text));
#endif
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"OnButtonURL_Clicked: {ex.Message}");
            }
        }

        /// <summary>
        /// Displays the device's cached and current geographic location to the user in alert dialogs.
        /// </summary>
        /// <remarks>This method first attempts to retrieve a cached location and displays it if
        /// available. It then attempts to obtain the current location and displays the result. If either location is
        /// unavailable, an appropriate message is shown. This method is asynchronous and returns immediately; any
        /// exceptions thrown during location retrieval or display may not be observed by the caller.</remarks>
        private async void OnButtonGeoLocation_Clicked(object sender, EventArgs e)
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
                lblPayloadTypeLatitudeDMSResult.Text = ClassGeolocation.DecimalToDMS(location.Latitude, isLatitude: true);
                lblPayloadTypeLongitudeDMSResult.Text = ClassGeolocation.DecimalToDMS(location.Longitude, isLatitude: false);
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

            // If the URL is valid, open it in the device's default web browser to display the location on the map
            await Launcher.Default.OpenAsync(new Uri(url));
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
            try
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
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in OnButtonClose_Clicked: {ex.Message}");
            }
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
                lblPayloadTypeLatitudeDMSResult.Text = ClassGeolocation.DecimalToDMS(latitude, isLatitude: true);
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
                lblPayloadTypeLongitudeDMSResult.Text = ClassGeolocation.DecimalToDMS(longitude, isLatitude: false);
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
        /// <remarks>https://github.com/Shane32/QRCoder - https://deepwiki.com/codebude/QRCoder/3.2-basic-payloads</remarks>
        private async Task<string> BuildPayload(string selectedName)
        {
            string payload = string.Empty;

            // PayloadType WIFI
            if (selectedName == ClassPayloadTypes.cPayloadType_WIFI)
            {
                string cSsid = entPayloadTypeSSID.Text.Trim();
                string cPassword = entPayloadTypePassword.Text.Trim();
                string cAuthenticationMode = pckWiFiAuthentication.SelectedIndex != -1 ? pckWiFiAuthentication.Items[pckWiFiAuthentication.SelectedIndex].Trim() : "WPA";

                payload = $"WIFI:S:{cSsid};T:{cAuthenticationMode};P:{cPassword};H:false;;";  // WIFI:S:Orange;T:WPA;P:123456;H:false;;

                //WiFi generator = new(ssid: cSsid, password: cPassword, authenticationMode: WiFi.Authentication.WPA, isHiddenSSID: false, escapeHexStrings: false);
                //payload = generator.ToString();
            }

            // PayloadType URL
            else if (selectedName == ClassPayloadTypes.cPayloadType_URL)
            {
                try
                {
                    entPayloadTypeURL.Text = entPayloadTypeURL.Text.Trim();
                    if (!await IsValidUrl(entPayloadTypeURL.Text))
                    {
                        return string.Empty;
                    }

                    Url generator = new(url: entPayloadTypeURL.Text);
                    payload = generator.ToString();
                    //payload = entPayloadTypeURL.Text;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error in BuildPayload (URL): {ex.Message}");
                }
            }

            // PayloadType Bookmark
            else if (selectedName == ClassPayloadTypes.cPayloadType_BOOKMARK)
            {
                entPayloadTypeURL.Text = entPayloadTypeURL.Text.Trim();
                if (!await IsValidUrl(entPayloadTypeURL.Text))
                {
                    return string.Empty;
                }

                Bookmark generator = new(url: entPayloadTypeURL.Text, title: entPayloadTypeTitle.Text.Trim());
                payload = generator.ToString();
            }

            // PayloadType Mail
            else if (selectedName == ClassPayloadTypes.cPayloadType_MAIL)
            {
                entPayloadTypeReceiver.Text = entPayloadTypeReceiver.Text.Trim();
                if (!await IsValidEmail(entPayloadTypeReceiver.Text))
                {
                    return string.Empty;
                }

                Mail generator = new(mailReceiver: entPayloadTypeReceiver.Text, subject: entPayloadTypeSubject.Text.Trim(), message: entPayloadTypeMessage.Text.Trim());
                payload = generator.ToString();
            }

            // PayloadType SMS
            else if (selectedName == ClassPayloadTypes.cPayloadType_SMS)
            {
                entPayloadTypePhoneNumber.Text = entPayloadTypePhoneNumber.Text.Trim();
                if (! await IsValidPhoneNumber(entPayloadTypePhoneNumber.Text))
                {
                    return string.Empty;
                }

                SMS generator = new(number: entPayloadTypePhoneNumber.Text, subject: entPayloadTypeMessage.Text.Trim());
                payload = generator.ToString();
            }

            // PayloadType MMS
            else if (selectedName == ClassPayloadTypes.cPayloadType_MMS)
            {
                entPayloadTypePhoneNumber.Text = entPayloadTypePhoneNumber.Text.Trim();
                if (!await IsValidPhoneNumber(entPayloadTypePhoneNumber.Text))
                {
                    return string.Empty;
                }

                MMS generator = new(number: entPayloadTypePhoneNumber.Text, subject: entPayloadTypeMessage.Text.Trim());
                payload = generator.ToString();
            }
            // PayloadType Geolocation
            else if (selectedName == ClassPayloadTypes.cPayloadType_GEOLOCATION)
            {
                // Validate the latitude and longitude input values and generate the geolocation payload string if valid
                payload = await ValidateGeolocationValues();

                // If the payload is empty, it indicates that there was an error in validating the geolocation values
                if (payload == string.Empty)
                {
                    return string.Empty;
                }
            }

            // PayloadType Phone number
            else if (selectedName == ClassPayloadTypes.cPayloadType_PHONENUMBER)
            {
                entPayloadTypePhoneNumber.Text = entPayloadTypePhoneNumber.Text.Trim();
                if (!await IsValidPhoneNumber(entPayloadTypePhoneNumber.Text))
                {
                    return string.Empty;
                }

                PhoneNumber generator = new(number: entPayloadTypePhoneNumber.Text);
                payload = generator.ToString();
            }

            // PayloadType WhatsApp message
            else if (selectedName == ClassPayloadTypes.cPayloadType_WHATSAPPMESSAGE)
            {
                entPayloadTypePhoneNumber.Text = entPayloadTypePhoneNumber.Text.Trim();
                if (!await IsValidPhoneNumber(entPayloadTypePhoneNumber.Text))
                {
                    return string.Empty;
                }

                WhatsAppMessage generator = new(number: entPayloadTypePhoneNumber.Text, message: entPayloadTypeMessage.Text.Trim());
                payload = generator.ToString();
            }

            // PayloadType Contact data
            else if (selectedName == ClassPayloadTypes.cPayloadType_CONTACTDATA)
            {
                entPayloadTypePhoneNumber.Text = entPayloadTypePhoneNumber.Text.Trim();
                if (entPayloadTypePhoneNumber.Text.Length < 2)
                {
                    entPayloadTypePhoneNumber.Text = string.Empty;
                }
                else if (!await IsValidPhoneNumber(entPayloadTypePhoneNumber.Text))
                {
                    return string.Empty;
                }

                entPayloadTypeMail.Text = entPayloadTypeMail.Text.Trim();
                if (entPayloadTypeMail.Text.Length < 3)
                {
                    entPayloadTypeMail.Text = string.Empty;
                }
                else if (!await IsValidEmail(entPayloadTypeMail.Text))
                {
                    return string.Empty;
                }

                ContactData generator = new(ContactData.ContactOutputType.VCard3, firstname: entPayloadTypeFirstname.Text.Trim(), lastname: entPayloadTypeLastname.Text.Trim(), nickname: "", phone: "", mobilePhone: entPayloadTypePhoneNumber.Text, workPhone: "", email: entPayloadTypeMail.Text);
                payload = generator.ToString();
            }

            // PayloadType Calendar event
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
SUMMARY:{entPayloadTypeSubject.Text.Trim()}
DESCRIPTION:{entPayloadTypeDescription.Text.Trim()}
LOCATION:{entPayloadTypeLocation.Text.Trim()}
DTSTART:{startUtc}
DTEND:{endUtc}
UID:{DateTime.UtcNow.Ticks + random.Next(1000, 9999)}
DTSTAMP:{DateTime.UtcNow:yyyyMMddTHHmmssZ}
END:VEVENT
END:VCALENDAR";
            }

            // PayloadType SEPA Credit Transfer
            else if (selectedName == ClassPayloadTypes.cPayloadType_SEPACREDITTRANSFER)
            {
                // https://github.com/Shane32/QRCoder/wiki/Advanced-usage---Payload-generators#37-girocode
                // Trim all input values to remove leading and trailing whitespace, which can cause validation errors or incorrect payload generation
                entPayloadTypeSctBic.Text = entPayloadTypeSctBic.Text.Trim();
                entPayloadTypeSctName.Text = entPayloadTypeSctName.Text.Trim();
                entPayloadTypeSctIban.Text = entPayloadTypeSctIban.Text.Trim();
                entPayloadTypeSctAmountEur.Text = entPayloadTypeSctAmountEur.Text.Trim();
                entPayloadTypeSctPurpose.Text = entPayloadTypeSctPurpose.Text.Trim();
                entPayloadTypeSctRemittanceInfoStructured.Text = entPayloadTypeSctRemittanceInfoStructured.Text.Trim();
                entPayloadTypeSctRemittanceInfoUnstructured.Text = entPayloadTypeSctRemittanceInfoUnstructured.Text.Trim();
                entPayloadTypeSctInformation.Text = entPayloadTypeSctInformation.Text.Trim();

                // Validate the SEPA Credit Transfer input values
                if (!await IsValidSepaCreditTransfer())
                {
                    return string.Empty;
                }

                // Create the SEPA Credit Transfer payload string in the EPC QR code format
                //                payload = $@"BCD
                //002
                //1
                //SCT
                //{entPayloadTypeSctBic.Text}
                //{entPayloadTypeSctName.Text}
                //{entPayloadTypeSctIban.Text}
                //EUR{entPayloadTypeSctAmountEur.Text}
                //{entPayloadTypeSctPurpose.Text}
                //{entPayloadTypeSctRemittanceInfoStructured.Text}
                //{entPayloadTypeSctRemittanceInfoUnstructured.Text}
                //{entPayloadTypeSctInformation.Text}";

                // Use the QRCoder library to generate the SEPA Credit Transfer payload and create the QR code image
                string cRemittance = string.Empty;
                TypeOfRemittance typeOfRemittance = new();

                if (entPayloadTypeSctRemittanceInfoStructured.Text == string.Empty)
                {
                    cRemittance = entPayloadTypeSctRemittanceInfoUnstructured.Text;
                    typeOfRemittance = TypeOfRemittance.Unstructured;
                }
                else
                {
                    cRemittance = entPayloadTypeSctRemittanceInfoStructured.Text;
                    typeOfRemittance = TypeOfRemittance.Structured;
                }

                try
                {
                    Girocode generator = new(iban: entPayloadTypeSctIban.Text,
                        bic: entPayloadTypeSctBic.Text,
                        name: entPayloadTypeSctName.Text,
                        amount: decimal.Parse(entPayloadTypeSctAmountEur.Text, CultureInfo.InvariantCulture),
                        remittanceInformation: cRemittance,
                        typeOfRemittance: typeOfRemittance,
                        purposeOfCreditTransfer: entPayloadTypeSctPurpose.Text,
                        messageToGirocodeUser: entPayloadTypeSctInformation.Text,
                        version: GirocodeVersion.Version2,
                        encoding: GirocodeEncoding.UTF_8);

                    QRCodeGenerator qrGenerator = new();
                    QRCodeData qrCodeData = qrGenerator.CreateQrCode(generator);
                    PngByteQRCode qrCode = new(qrCodeData);
                    byte[] qrCodeAsPngByteArr = qrCode.GetGraphic(20, System.Drawing.Color.FromArgb(Convert.ToInt32(ClassBarcodes.cCodeColorFg, 16)), System.Drawing.Color.FromArgb(Convert.ToInt32(ClassBarcodes.cCodeColorBg, 16)));

                    payload = generator.ToString();
                    qrCodeImage = ImageSource.FromStream(() => new MemoryStream(qrCodeAsPngByteArr));

                    // The total payload is limited to 331 bytes for the SEPA Credit Transfer type
                    if (System.Text.Encoding.UTF8.GetByteCount(payload) > 331)
                    {
                        await Application.Current!.Windows[0].Page!.DisplayAlertAsync(CodeLang.ErrorTitle_Text, CodeLang.ErrorSepaCreditTransferPayloadTooLong_Text, CodeLang.ButtonClose_Text);
                        return string.Empty;
                    }

                    // Save the generated QR code image to a file in the cache directory for later retrieval and display
                    await File.WriteAllBytesAsync(ClassBarcodes.cFileBarcodePng, qrCodeAsPngByteArr);

                    // Generate the QR code as an SVG string and save it to disk for sharing or other purposes
                    SvgQRCode qrCodeSvg = new(qrCodeData);
                    string qrCodeAsSvg = qrCodeSvg.GetGraphic(20, System.Drawing.Color.FromArgb(Convert.ToInt32(ClassBarcodes.cCodeColorFg, 16)), System.Drawing.Color.FromArgb(Convert.ToInt32(ClassBarcodes.cCodeColorBg, 16)));
                    
                    await File.WriteAllTextAsync(ClassBarcodes.cFileBarcodeSvg, qrCodeAsSvg);

                    bPayloadSepaCreditTransfer = true;
                }
                catch (Exception ex)
                {
#if DEBUG
                    await Application.Current!.Windows[0].Page!.DisplayAlertAsync(CodeLang.ErrorTitle_Text, ex.Message, CodeLang.ButtonClose_Text);
#endif
                    return string.Empty;
                }
            }

            // If the selected payload type does not match any of the known types, return an empty string to indicate that no payload could be generated
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

            // Construct the Maps URL using the validated latitude and longitude values
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

        /// <summary>
        /// Determines whether the specified string is a valid email address format.
        /// </summary>
        /// <remarks>This method checks the format of the email address but does not verify that the address exists or is reachable.</remarks>
        /// <param name="email">The email address to validate. Cannot be null, empty, or consist only of white-space characters.</param>
        /// <returns>true if the specified string is a valid email address format; otherwise, false.</returns>
        private async Task<bool> IsValidEmail(string email)
        {
            bool bIsValid = !string.IsNullOrWhiteSpace(email) && new EmailAddressAttribute().IsValid(email);

            if (!bIsValid)
            {
                await Application.Current!.Windows[0].Page!.DisplayAlertAsync(CodeLang.ErrorTitle_Text, CodeLang.ErrorEmailInvalid_Text, CodeLang.ButtonClose_Text);
                _ = entPayloadTypeReceiver.Focus();
                _ = entPayloadTypeMail.Focus();
                return false;
            }

            return true;
        }

        /// <summary>
        /// Determines whether the specified string is a valid absolute HTTP or HTTPS URL.
        /// </summary>
        /// <param name="url">The URL string to validate. Must not be null.</param>
        /// <returns>true if the string is a well-formed absolute HTTP or HTTPS URL; otherwise, false.</returns>
        private async Task<bool> IsValidUrl(string url)
        {
            bool bIsValid = Uri.TryCreate(url, UriKind.Absolute, out Uri? uriResult) &&
            (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

            if (!bIsValid)
            {
                await Application.Current!.Windows[0].Page!.DisplayAlertAsync(CodeLang.ErrorTitle_Text, CodeLang.ErrorUrlInvalid_Text, CodeLang.ButtonClose_Text);
                _ = entPayloadTypeURL.Focus();
                return false;
            }

            return true;
        }

        /// <summary>
        /// Determines whether the specified string is a valid phone number format based on a regular expression pattern that allows for international and local formats, including optional country code, spaces, dashes, and parentheses.
        /// </summary>
        /// <param name="cPhoneNumber">The phone number string to validate. Cannot be null, empty, or consist only of white-space characters.</param>
        /// <returns>true if the specified string is a valid phone number format; otherwise, false.</returns>
        private async Task<bool> IsValidPhoneNumber(string cPhoneNumber)
        {
            if (!PhoneRegex().IsMatch(cPhoneNumber))
            {
                await Application.Current!.Windows[0].Page!.DisplayAlertAsync(CodeLang.ErrorTitle_Text, CodeLang.ErrorPhoneNumberInvalid_Text, CodeLang.ButtonClose_Text);
                _ = entPayloadTypePhoneNumber.Focus();
                return false;
            }

            return true;
        }

        /// <summary>
        /// Basic regex for validating phone numbers (international + local formats).
        /// Allows optional '+' at start, digits, spaces, dashes, parentheses, and dots.
        /// Minimum length of 3 characters and maximum length of 20 characters to prevent excessively long inputs.
        /// </summary>
        /// <returns>A compiled regular expression for validating phone numbers.</returns>
        private static Regex PhoneRegex()
        {
            return RegexPhone();
        }

        /// <summary>
        /// Validates the input values for a SEPA Credit Transfer payload, ensuring that the amount is properly formatted as a decimal number and that all required fields are filled out. If the inputs are valid, it generates the appropriate payload string; otherwise, it displays error messages and returns an empty string.
        /// </summary>
        /// <returns></returns>
        private async Task<bool> IsValidSepaCreditTransfer()
        {
            // BIC: validate that the BIC, as they are required for a valid SEPA Credit Transfer payload.
            // BIC: remove spaces for standardization
            entPayloadTypeSctBic.Text = entPayloadTypeSctBic.Text.Replace(" ", string.Empty);

            if (entPayloadTypeSctBic.Text.Length is not (0 or 8 or 11))
            {
                await Application.Current!.Windows[0].Page!.DisplayAlertAsync(CodeLang.ErrorTitle_Text, CodeLang.ErrorBicInvalid_Text, CodeLang.ButtonClose_Text);
                _ = entPayloadTypeSctBic.Focus();
                return false;
            }

            // Name: validate that the name field is not empty or whitespace, as it is required for a valid SEPA Credit Transfer payload. If the name is invalid, display an error message and return false to indicate that the validation failed.
            if (string.IsNullOrWhiteSpace(entPayloadTypeSctName.Text))
            {
                await Application.Current!.Windows[0].Page!.DisplayAlertAsync(CodeLang.ErrorTitle_Text, CodeLang.ErrorNameInvalid_Text, CodeLang.ButtonClose_Text);
                _ = entPayloadTypeSctName.Focus();
                return false;
            }

            // IBAN: validate the IBAN number using the IsValidIban method, which checks both the format and checksum of the IBAN. If the IBAN is invalid, display an error message and return false to indicate that the validation failed.
            if (!IsValidIban(entPayloadTypeSctIban.Text))
            {
                await Application.Current!.Windows[0].Page!.DisplayAlertAsync(CodeLang.ErrorTitle_Text, CodeLang.ErrorIbanInvalid_Text, CodeLang.ButtonClose_Text);
                _ = entPayloadTypeSctIban.Focus();
                return false;
            }

            // Amount: replace the decimal comma with a decimal point in the value to ensure correct parsing regardless of the user's locale settings
            entPayloadTypeSctAmountEur.Text = (entPayloadTypeSctAmountEur.Text ?? string.Empty).Replace(',', '.');

            // Amount: validate that the inputs can be parsed as decimals using invariant culture to ensure consistent decimal separator handling
            if (!decimal.TryParse(entPayloadTypeSctAmountEur.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out decimal nAmount) || nAmount is < 0.01m or > 999_999_999.99m)
            {
                await Application.Current!.Windows[0].Page!.DisplayAlertAsync(CodeLang.ErrorTitle_Text, CodeLang.ErrorAmountInvalid_Text, CodeLang.ButtonClose_Text);
                _ = entPayloadTypeSctAmountEur.Focus();
                return false;
            }

            // Amount: format the amount to have exactly two decimal places and use a period as the decimal separator, regardless of the user's locale settings. This ensures that the amount is correctly formatted for the SEPA Credit Transfer payload.
            entPayloadTypeSctAmountEur.Text = nAmount.ToString("F2", CultureInfo.InvariantCulture);

            // Remittance information: Only one of the remittance fields may be populated but both can be empty.
            if (!string.IsNullOrWhiteSpace(entPayloadTypeSctRemittanceInfoStructured.Text) && !string.IsNullOrWhiteSpace(entPayloadTypeSctRemittanceInfoUnstructured.Text))
            {
                await Application.Current!.Windows[0].Page!.DisplayAlertAsync(CodeLang.ErrorTitle_Text, CodeLang.ErrorRemittanceInfoTwoFieldsInvalid_Text, CodeLang.ButtonClose_Text);
                _ = entPayloadTypeSctRemittanceInfoStructured.Focus();
                return false;
            }

            // Remittance information: If the structured remittance reference field is populated, validate that it meets the required format for structured remittance information.
            if (!IsValidRemittanceInfoStructured(entPayloadTypeSctRemittanceInfoStructured.Text))
            {
                await Application.Current!.Windows[0].Page!.DisplayAlertAsync(CodeLang.ErrorTitle_Text, CodeLang.ErrorRemittanceInfoStructuredInvalid_Text, CodeLang.ButtonClose_Text);
                _ = entPayloadTypeSctRemittanceInfoStructured.Focus();
                return false;
            }

            return true;
        }

        /// <summary>
        /// Determines whether the specified string is a valid IBAN (International Bank Account Number).
        /// </summary>
        /// <remarks>Validates both the format and checksum using the mod-97 algorithm defined in ISO
        /// 13616. Spaces in the input are automatically removed during validation.</remarks>
        /// <param name="iban">The IBAN string to validate.</param>
        /// <returns>true if the IBAN is valid; otherwise, false.</returns>
        private bool IsValidIban(string iban)
        {
            // Remove spaces for standardization
            string cleanIban = iban.Replace(" ", string.Empty);
            entPayloadTypeSctIban.Text = cleanIban;

            // Basic IBAN format check: starts with 2 letters (country code), followed by 2 digits (check digits), and then up to 30 alphanumeric characters (BBAN)
            if (!RegexIban().IsMatch(cleanIban))
            {
                return false;
            }

            // Move the first four characters to the end of the string
            string rearrangedIban = string.Concat(cleanIban.AsSpan(4), cleanIban.AsSpan(0, 4));

            // Convert letters to numbers (A=10, B=11, ..., Z=35) and concatenate the result into a single numeric string
            string numericIban = string.Empty;
            
            foreach (char character in rearrangedIban)
            {
                if (char.IsLetter(character))
                {
                    numericIban += (character - 55).ToString();
                }
                else
                {
                    numericIban += character;
                }
            }

            // Perform the modulo 97 operation on the numeric IBAN string.
            // This is done by processing the string in chunks to avoid overflow issues with very long numbers.
            // If the result is 1, the IBAN is valid; otherwise, it is invalid.
            int remainder = 0;
            
            foreach (char digit in numericIban)
            {
                remainder = (remainder * 10 + int.Parse(digit.ToString())) % 97;
            }
            
            return remainder == 1;
        }

        /// <summary>
        /// Determines whether a remittance information string is in a valid structured format (European RF or Belgian).
        /// </summary>
        /// <param name="cRemittanceInfo">The remittance information string to validate.</param>
        /// <returns>true if valid structured remittance information; otherwise, false.</returns>
        private bool IsValidRemittanceInfoStructured(string cRemittanceInfo)
        {
            // Structured remittance information is optional, so empty or whitespace is considered valid
            if (string.IsNullOrWhiteSpace(cRemittanceInfo))
            {
                return true;
            }

            // Remove spaces for standardization
            cRemittanceInfo = cRemittanceInfo.Replace(" ", string.Empty);
            entPayloadTypeSctRemittanceInfoStructured.Text = cRemittanceInfo;

            // European RF structured remittance information looks like this: RF18539007547034
            if (cRemittanceInfo.StartsWith("RF"))
            {
                return IsValidRfStructuredReference(cRemittanceInfo);
            }

            // Belgian structured remittance information looks like this: +++026/0129/40240+++
            else if (cRemittanceInfo.StartsWith("+++"))
            {
                return IsValidBelgianStructuredReference(cRemittanceInfo);
            }

            return false;
        }

        /// <summary>
        /// Validates an ISO 11649 RF Creditor Reference
        /// </summary>
        /// <param name="reference"></param>
        /// <returns></returns>
        private static bool IsValidRfStructuredReference(string reference)
        {
            // RF + 2 check digits + up to 21 alphanumeric characters
            // The check digits are calculated using the mod 97 algorithm (similar to IBAN validation)

            // RF18539007547034     // valid
            // RF001234567890       // invalid
            // RF18 5390 0754 7034  // valid with spaces

            // Remove spaces
            string cleanRef = new([.. reference.Where(char.IsLetterOrDigit)]);

            // Must start with "RF" and be between 5 and 25 characters
            if (!cleanRef.StartsWith("RF") || cleanRef.Length < 5 || cleanRef.Length > 25)
            {
                return false;
            }

            // Move first 4 chars to the end (like IBAN check)
            string rearranged = string.Concat(cleanRef.AsSpan(4), cleanRef.AsSpan(0, 4));

            // Convert letters to numbers (A=10, B=11, ..., Z=35)
            string numericString = "";
            foreach (char c in rearranged)
            {
                if (char.IsLetter(c))
                {
                    numericString += (c - 'A' + 10).ToString();
                }
                else if (char.IsDigit(c))
                {
                    numericString += c;
                }
                else
                {
                    return false; // Invalid character
                }
            }

            // Use BigInteger for large numbers
            if (!BigInteger.TryParse(numericString, out BigInteger bigNum))
            {
                return false;
            }

            // Valid if mod 97 == 1
            return bigNum % 97 == 1;
        }

        /// <summary>
        /// Validates a Belgian structured reference (OGM) using modulo 97 check digit verification.
        /// </summary>
        /// <param name="reference">The structured reference to validate. May contain spaces, dots, plus signs, or slashes as separators.</param>
        /// <returns><see langword="true"/> if the reference is valid; otherwise, <see langword="false"/>.</returns>
        private static bool IsValidBelgianStructuredReference(string reference)
        {
            // First 10 digits: base number
            // Last 2 digits: check digits
            // Check rule: Compute first 10 % 97
            // If result is 0, check digits must be 97
            // Otherwise, check digits must equal the remainder

            // +++026/0129/40240+++ // Valid
            // 123456789012         // Invalid
            // 12345678903          // Invalid

            if (reference.Substring(6, 1) != "/" || reference.Substring(11, 1) != "/" || !reference.EndsWith("+++"))
            {
                return false;
            }

            // Remove spaces, dots, and other separators
            string cleanRef = reference.Replace(" ", "").Replace(".", "").Replace("+", "").Replace("/", "").Replace("-", "");

            // Must be exactly 12 digits
            if (cleanRef.Length != 12 || !ulong.TryParse(cleanRef, out _))
            {
                return false;
            }

            // Extract parts
            string first10 = cleanRef[..10];
            string last2 = cleanRef.Substring(10, 2);

            if (!ulong.TryParse(first10, out ulong baseNumber) || !int.TryParse(last2, out int checkDigits))
            {
                return false;
            }

            // Calculate modulo 97
            int remainder = (int)(baseNumber % 97);
            int expectedCheck = remainder == 0 ? 97 : remainder;

            return checkDigits == expectedCheck;
        }

        /// <summary>
        /// Basic regex for validating phone number format
        /// </summary>
        /// <returns></returns>
        [GeneratedRegex(@"^\+?[0-9\s\-\(\).]{3,20}$", RegexOptions.Compiled)]
        private static partial Regex RegexPhone();

        /// <summary>
        /// Basic regex for validating IBAN format: starts with 2 letters (country code), followed by 2 digits (check digits), and then up to 30 alphanumeric characters (BBAN).
        /// </summary>
        /// <returns></returns>
        [GeneratedRegex("^[A-Z]{2}[0-9]{2}[A-Z0-9]{1,30}$")]
        private static partial Regex RegexIban();
    }
}
