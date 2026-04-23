using static QRCoder.PayloadGenerator;
using Geolocation = QRCoder.PayloadGenerator.Geolocation;

namespace BarcodeGenerator
{
    internal class ClassPayloadTypes
    {
        // Global variables for Payload Types
        public static string cPayloadType_TEXT = string.Empty;
        public static string cPayloadType_WIFI = string.Empty;
        public static string cPayloadType_URL = string.Empty;
        public static string cPayloadType_BOOKMARK = string.Empty;
        public static string cPayloadType_MAIL = string.Empty;
        public static string cPayloadType_SMS = string.Empty;
        public static string cPayloadType_MMS = string.Empty;
        public static string cPayloadType_GEOLOCATION = string.Empty;
        public static string cPayloadType_PHONENUMBER = string.Empty;
        public static string cPayloadType_WHATSAPPMESSAGE = string.Empty;
        public static string cPayloadType_CONTACTDATA = string.Empty;
        public static string cPayloadType_CALENDAREVENT = string.Empty;

        // Global variables
        public static int nPayloadTypeIndex;
        public static string cPayloadType = string.Empty;
        public static string cPayloadTypeDefault = string.Empty;

        // Creating a public dictionaries to store the payload types for search operations
        public static Dictionary<string, string> payloadSearch = [];

        /// <summary>
        /// Initializes static payload type fields with their corresponding values.
        /// </summary>
        /// <remarks>Call this method before accessing any payload type fields to ensure they are properly
        /// initialized. This method is typically used during application startup or prior to using features that depend
        /// on payload type values.</remarks>
        public static void InitializePayloadTypes()
        {
            cPayloadType_TEXT = CodeLang.PayloadType_TEXT_Text;
            cPayloadType_WIFI = CodeLang.PayloadType_WIFI_Text;
            cPayloadType_URL = CodeLang.PayloadType_URL_Text;
            cPayloadType_BOOKMARK = CodeLang.PayloadType_BOOKMARK_Text;
            cPayloadType_MAIL = CodeLang.PayloadType_MAIL_Text;
            cPayloadType_SMS = CodeLang.PayloadType_SMS_Text;
            cPayloadType_MMS = CodeLang.PayloadType_MMS_Text;
            cPayloadType_GEOLOCATION = CodeLang.PayloadType_GEOLOCATION_Text;
            cPayloadType_PHONENUMBER = CodeLang.PayloadType_PHONENUMBER_Text;
            cPayloadType_WHATSAPPMESSAGE = CodeLang.PayloadType_WHATSAPPMESSAGE_Text;
            cPayloadType_CONTACTDATA = CodeLang.PayloadType_CONTACTDATA_Text;
            cPayloadType_CALENDAREVENT = CodeLang.PayloadType_CALENDAREVENT_Text;

            // Default values for the payload type
            cPayloadTypeDefault = CodeLang.PayloadType_TEXT_Text;
        }

        /// <summary>
        /// Initializes the set of supported payload types for lookup operations.
        /// </summary>
        /// <remarks>This method clears any existing payload type mappings and repopulates the internal
        /// dictionary with the standard set of payload types. Call this method before performing payload type
        /// lookups to ensure the dictionary is correctly initialized.</remarks>
        public static void InitializePayloadSearchTypes()
        {
            // Clear the dictionary before adding new elements
            payloadSearch.Clear();

            // Add elements to the dictionary
            payloadSearch.Add("TEXT", CodeLang.PayloadType_TEXT_Text);
            payloadSearch.Add("WIFI", CodeLang.PayloadType_WIFI_Text);
            payloadSearch.Add("URL", CodeLang.PayloadType_URL_Text);
            payloadSearch.Add("BOOKMARK", CodeLang.PayloadType_BOOKMARK_Text);
            payloadSearch.Add("MAIL", CodeLang.PayloadType_MAIL_Text);
            payloadSearch.Add("SMS", CodeLang.PayloadType_SMS_Text);
            payloadSearch.Add("MMS", CodeLang.PayloadType_MMS_Text);
            payloadSearch.Add("GEOLOCATION", CodeLang.PayloadType_GEOLOCATION_Text);
            payloadSearch.Add("PHONENUMBER", CodeLang.PayloadType_PHONENUMBER_Text);
            payloadSearch.Add("WHATSAPPMESSAGE", CodeLang.PayloadType_WHATSAPPMESSAGE_Text);
            payloadSearch.Add("CONTACTDATA", CodeLang.PayloadType_CONTACTDATA_Text);
            payloadSearch.Add("CALENDAREVENT", CodeLang.PayloadType_CALENDAREVENT_Text);
        }

        /// <summary>
        /// Retrieves a list of supported QR code payload types as display names, ordered alphabetically in a
        /// case-insensitive manner.
        /// </summary>
        /// <remarks>The returned payload types include common QR code formats such as Wi-Fi, URL,
        /// bookmark, email, SMS, MMS, geolocation, plain text, phone number, WhatsApp message, contact data, and
        /// calendar event. This method can be used to populate UI elements or validate supported QR code
        /// formats.</remarks>
        /// <returns>A list of strings representing the supported QR code payload types. The list is sorted alphabetically
        /// without regard to case.</returns>
        public static List<string> GetQRCodePayloadTypes() => [.. new List<string>
            {
                // https://github.com/Shane32/QRCoder    
                CodeLang.PayloadType_TEXT_Text,
                CodeLang.PayloadType_WIFI_Text,
                CodeLang.PayloadType_URL_Text,
                CodeLang.PayloadType_BOOKMARK_Text,
                CodeLang.PayloadType_MAIL_Text,
                CodeLang.PayloadType_SMS_Text,
                CodeLang.PayloadType_MMS_Text,
                CodeLang.PayloadType_GEOLOCATION_Text,
                CodeLang.PayloadType_PHONENUMBER_Text,
                CodeLang.PayloadType_WHATSAPPMESSAGE_Text,
                CodeLang.PayloadType_CONTACTDATA_Text,
                CodeLang.PayloadType_CALENDAREVENT_Text,
            }
            .OrderBy(x => x, StringComparer.OrdinalIgnoreCase)];

        /// <summary>
        /// Select the name and index in the payload type list
        /// </summary>
        /// <param name="picker"></param>
        public static void SelectPayloadTypeIndex(Picker picker)
        {
            // If there is no saved payload type name then set it to the default payload type name
            if (string.IsNullOrEmpty(cPayloadType))
            {
                cPayloadType = cPayloadTypeDefault;
            }

            // Search for the name of the payload type in the picker list
            nPayloadTypeIndex = picker.Items.IndexOf(cPayloadType);

            // If the payload type name was not found in the list then set it to the default name
            if (nPayloadTypeIndex < 0)
            {
                cPayloadType = cPayloadTypeDefault;

                // Search for the name of the payload type in the picker list
                nPayloadTypeIndex = picker.Items.IndexOf(cPayloadType);
            }

            // If the payload type name was not found in the list then set it to the first payload type name
            if (nPayloadTypeIndex < 0)
            {
                nPayloadTypeIndex = 0;
                cPayloadType = picker.Items[nPayloadTypeIndex];
            }

            // Select the payload type in the picker
            picker.SelectedIndex = nPayloadTypeIndex;
        }

        /// <summary>
        /// Builds a QR code payload string based on the selected payload type name. The method generates the appropriate
        /// payload string for the QR code based on the selected payload type.
        /// </summary>
        /// <param name="selectedName">The name of the selected payload type.</param>
        /// <returns>The generated payload string for the QR code.</returns>
        /// <remarks>https://github.com/Shane32/QRCoder</remarks>
        public static string BuildPayload(string selectedName)
        {
            string payload;

            if (selectedName == cPayloadType_WIFI)
            {
                WiFi generator = new("My-WiFis-Name", "s3cr3t-p4ssw0rd", WiFi.Authentication.WPA);
                payload = generator.ToString();
            }
            else if (selectedName == cPayloadType_URL)
            {
                Url generator = new("https://github.com/codebude/QRCoder/");
                payload = generator.ToString();
            }
            else if (selectedName == cPayloadType_BOOKMARK)
            {
                Bookmark generator = new("http://code-bude.net", "Blog of QRCoder's father");
                payload = generator.ToString();
            }
            else if (selectedName == cPayloadType_MAIL)
            {
                Mail generator = new("john@doe.com", "Look at this!", "Hi John, have a look at this QRCoder library!");
                payload = generator.ToString();
            }
            else if (selectedName == cPayloadType_SMS)
            {
                SMS generator = new("+491701234567", "Hi John, do you remember SMS?");
                payload = generator.ToString();
            }
            else if (selectedName == cPayloadType_MMS)
            {
                MMS generator = new("+491701234567", "Hi John, have a look at this QRCoder library!");
                payload = generator.ToString();
            }
            else if (selectedName == cPayloadType_GEOLOCATION)
            {
                Geolocation generator = new("51.26118", "6.6717");
                payload = generator.ToString();
            }
            else if (selectedName == cPayloadType_PHONENUMBER)
            {
                PhoneNumber generator = new("+491701234567");
                payload = generator.ToString();
            }
            else if (selectedName == cPayloadType_WHATSAPPMESSAGE)
            {
                WhatsAppMessage generator = new("Hi John, what do you think about QR codes?");
                payload = generator.ToString();
            }
            else if (selectedName == cPayloadType_CONTACTDATA)
            {
                ContactData generator = new(ContactData.ContactOutputType.VCard3, "John", "Doe");
                payload = generator.ToString();
            }
            else if (selectedName == cPayloadType_CALENDAREVENT)
            {
                CalendarEvent generator = new("Birthday party", "Join QRCoder's fourth birthday!", "51.26118,6.6717", new DateTime(2017, 10, 13), new DateTime(2017, 10, 13), true);
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
