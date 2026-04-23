using CommunityToolkit.Maui.Views;
using static QRCoder.PayloadGenerator;
using Geolocation = QRCoder.PayloadGenerator.Geolocation;

namespace BarcodeGenerator
{
    public partial class PopupPayloadTypes : Popup
    {
    	public PopupPayloadTypes(string cMessage = "")
    	{
            // Name of the current page
            Globals.cCurrentPage = "PopupPayloadTypes";

            InitializeComponent();

            // Indicate that the popup has been opened, which can be used to prevent certain actions in the MainPage OnAppearing event
            Globals.bPopupOpened = true;

            // Set the message text
            lblPopupTitle.Text = $"{cMessage} {ClassPayloadTypes.cPayloadType}";

            // Set default dates for calendar event payload type to the current date and time if they are not already set
            dtpPayloadTypeStart.Date ??= DateTime.Now;
            dtpPayloadTypeEnd.Date ??= DateTime.Now;

            SetControls(ClassPayloadTypes.cPayloadType);
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
        /// Sets the visibility of various controls based on the selected payload type name
        /// The method checks the selected payload type
        /// </summary>
        /// <param name="selectedName"></param>
        public void SetControls(string selectedName)
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
                brdPayloadTypeStart.IsVisible = true;
                lblPayloadTypeEnd.IsVisible = true;
                brdPayloadTypeEnd.IsVisible = true;
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
                DateTime startDate = dtpPayloadTypeStart.Date ?? DateTime.Now;
                DateTime endDate = dtpPayloadTypeEnd.Date ?? DateTime.Now;
                CalendarEvent generator = new(subject: entPayloadTypeSubject.Text, description: entPayloadTypeDescription.Text, location: entPayloadTypeLocation.Text, start: new DateTimeOffset(startDate), end: new DateTimeOffset(endDate), allDayEvent: false);
                payload = generator.ToString();
            }
            else
            {
                payload = string.Empty;
            }

            return payload;
        }
    }
}