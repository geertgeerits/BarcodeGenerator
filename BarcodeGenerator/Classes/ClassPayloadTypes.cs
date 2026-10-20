namespace BarcodeGenerator
{
    internal class ClassPayloadTypes
    {
        // Global variables for Payload Types - https://github.com/Shane32/QRCoder
        // https://www.europeanpaymentscouncil.eu/sites/default/files/KB/files/EPC069-12%20v2.1%20Quick%20Response%20Code%20-%20Guidelines%20to%20Enable%20the%20Data%20Capture%20for%20the%20Initiation%20of%20a%20SCT.pdf
        // https://en.wikipedia.org/wiki/EPC_QR_code
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
        public static string cPayloadType_SEPACREDITTRANSFER = string.Empty;
        public static string cPayloadType_GS1 = string.Empty;

        // Global variables
        public static int nPayloadTypeIndex;                        // Index of the payload type in the list of payload types
        public static string cPayloadType = string.Empty;           // Name of the payload type - used for sharing the payload with the name in the text
        public static string cPayloadCaption = string.Empty;        // Caption text for the payload - used for sharing the payload with the caption in the text
        public static string cPayloadTypeDefault = string.Empty;    // Default payload type name - used for sharing the payload with the default name in the text
        public static string cPayloadResult = string.Empty;         // Result of the payload type - used for sharing the payload with the result in the text

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
            cPayloadType_SEPACREDITTRANSFER = CodeLang.PayloadType_SEPACREDITTRANSFER_Text;
            cPayloadType_GS1 = CodeLang.PayloadType_GS1_Text;

            // Default values for the payload type
            cPayloadTypeDefault = CodeLang.PayloadType_GEOLOCATION_Text;
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
            payloadSearch.Add("SEPACREDITTRANSFER", CodeLang.PayloadType_SEPACREDITTRANSFER_Text);
            payloadSearch.Add("GS1", CodeLang.PayloadType_GS1_Text);
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
                CodeLang.PayloadType_WIFI_Text,
                CodeLang.PayloadType_URL_Text,
                //CodeLang.PayloadType_BOOKMARK_Text,   // Does not work - No longer supported ???
                CodeLang.PayloadType_MAIL_Text,
                CodeLang.PayloadType_SMS_Text,
                CodeLang.PayloadType_MMS_Text,
                CodeLang.PayloadType_GEOLOCATION_Text,
                CodeLang.PayloadType_PHONENUMBER_Text,
                CodeLang.PayloadType_WHATSAPPMESSAGE_Text,
                CodeLang.PayloadType_CONTACTDATA_Text,
                CodeLang.PayloadType_CALENDAREVENT_Text,
                CodeLang.PayloadType_SEPACREDITTRANSFER_Text,
                CodeLang.PayloadType_GS1_Text
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
        /// Attempts to handle and share the specified payload text using the most appropriate mechanism based on its
        /// content, such as opening links, sharing contacts, or displaying Wi‑Fi details.
        /// </summary>
        /// <remarks>The method detects the type of the payload by inspecting its prefix and performs an
        /// action suitable for that type, such as launching a browser, opening Wi‑Fi settings, or sharing files. If the
        /// payload type is not recognized, a generic share operation is used. The method is asynchronous and returns
        /// immediately; callers should not rely on its completion for further logic.</remarks>
        /// <param name="text">The payload text to process and share. This can be a URL, Wi‑Fi QR payload, telephone number, vCard,
        /// calendar event, or other supported format. Cannot be null or whitespace.</param>
        public static async Task SharePayloadTypes(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return;
            }

            // WhatsApp short link
            if (text.StartsWith("https://wa.me/", StringComparison.OrdinalIgnoreCase))
            {
                await Launcher.Default.OpenAsync(new Uri(text));
            }

            // Wi‑Fi QR payload: show details and offer to copy or open Wi‑Fi settings
            else if (text.StartsWith("WIFI:", StringComparison.OrdinalIgnoreCase))
            {
                string ssid = string.Empty, pass = string.Empty, auth = string.Empty;
                string payload = text[5..];
                
                foreach (string part in payload.Split(';', StringSplitOptions.RemoveEmptyEntries))
                {
                    if (part.StartsWith("S:"))
                    {
                        ssid = Uri.UnescapeDataString(part.AsSpan(2));
                    }
                    else if (part.StartsWith("P:"))
                    {
                        pass = Uri.UnescapeDataString(part.AsSpan(2));
                    }
                    else if (part.StartsWith("T:"))
                    {
                        auth = part[2..];
                    }
                }

                // string message = $"SSID: {ssid}\nPassword: {pass}\nEncryption: {auth}";
                string action = await Application.Current!.Windows[0].Page!.DisplayActionSheetAsync(CodeLang.WiFiNetworkDetected_Text, CodeLang.ButtonClose_Text, null, CodeLang.ButtonCopy_Text, CodeLang.WiFiOpenSettings_Text);

                if (action == CodeLang.ButtonCopy_Text)
                {
                    await Clipboard.Default.SetTextAsync($"SSID:{ssid};P:{pass};");
                }
                else if (action == CodeLang.WiFiOpenSettings_Text)
                {
#if ANDROID
                    try
                    {
                        var intent = new Android.Content.Intent(Android.Provider.Settings.ActionWifiSettings);
                        intent.SetFlags(Android.Content.ActivityFlags.NewTask);
                        Android.App.Application.Context.StartActivity(intent);
                    }
                    catch
                    {
                        // fallback to launcher
                        await Launcher.Default.OpenAsync(new Uri("ms-settings:network-wifi"));
                    }
#elif WINDOWS
                    await Launcher.Default.OpenAsync(new Uri("ms-settings:network-wifi"));
#elif IOS
                    // iOS doesn't reliably allow opening Wi‑Fi settings from apps on all iOS versions.
                    await Application.Current!.Windows[0].Page!.DisplayAlertAsync("", CodeLang.WiFiJoinNetwork_Text, CodeLang.ButtonClose_Text);
#else
                    await Application.Current!.Windows[0].Page!.DisplayAlertAsync("", CodeLang.WiFiOpenSettingsNotSupported_Text, CodeLang.ButtonClose_Text);
#endif
                }
            }

            // Generic URI handlers (URL, geo, mailto, sms, mms, etc.)
            else if (text.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                text.StartsWith("https://", StringComparison.OrdinalIgnoreCase) ||
                text.StartsWith("mailto:", StringComparison.OrdinalIgnoreCase) ||
                text.StartsWith("sms:", StringComparison.OrdinalIgnoreCase) ||
                text.StartsWith("mms:", StringComparison.OrdinalIgnoreCase) ||
                text.StartsWith("geo:", StringComparison.OrdinalIgnoreCase))
            {
                await Launcher.Default.OpenAsync(new Uri(text));
            }

            // Telephone numbers - prefer PhoneDialer where available
            else if (text.StartsWith("tel:", StringComparison.OrdinalIgnoreCase))
            {
                string number = text[4..];
                try
                {
                    PhoneDialer.Default.Open(number);
                }
                catch
                {
                    await Launcher.Default.OpenAsync(new Uri(text));
                }
            }

            // Contact (vCard) - write to a temp .vcf and let the system open/share it
            else if (text.StartsWith("BEGIN:VCARD", StringComparison.OrdinalIgnoreCase))
            {
                string file = System.IO.Path.Combine(FileSystem.Current.CacheDirectory, $"contact_{DateTime.Now:yyyyMMddHHmmss}.vcf");
                System.IO.File.WriteAllText(file, text);
                await Launcher.Default.OpenAsync(new OpenFileRequest
                {
                    File = new ReadOnlyFile(file)
                });
            }

            // Calendar event (iCal) - write to a temp .ics and let the system open/share it
            else if (text.StartsWith("BEGIN:VCALENDAR", StringComparison.OrdinalIgnoreCase) || text.Contains("BEGIN:VEVENT"))
            {
                string file = System.IO.Path.Combine(FileSystem.Current.CacheDirectory, $"event_{DateTime.Now:yyyyMMddHHmmss}.ics");
                System.IO.File.WriteAllText(file, text);
                await Launcher.Default.OpenAsync(new OpenFileRequest
                {
                    File = new ReadOnlyFile(file)
                });
            }

            // Bookmark - treat as URL - Does not work - No longer supported ???
            else if (text.StartsWith("MEBKM:", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    await Launcher.Default.OpenAsync(new Uri(text));
                }
                catch (Exception Exception)
                {
                    // No Activity found to handle Intent { act=android.intent.action.VIEW dat=MEBKM: flg=0x14000000 xflg=0x4 }
                    Debug.WriteLine("SharePayloadTypes Bookmark: " + Exception.Message);
                }
            }

            // Fallback: use existing generic share method
            else
            {
                _ = ClassUtilities.ShareBarcodeResultAsync(text);
            }
        }
    }
}
/*
GS1 - Global Standards One
https://www.gs1.org/standards/gs1-digital-link
https://ref.gs1.org/standards/digital-link/uri-syntax/

Primary identification keys
The following rules indicate which GS1 Application Identifiers (GS1 AI) for simple GS1 identifiers are considered as primary identification keys for GS1 Digital Link URI.

gtin-code	=	"01"	; GTIN
itip-code	=	"8006"	; ITIP
gmn-code	=	"8013"	; Global Model Number
cpid-code	=	"8010"	; CPID
gln-code	=	"414"	; Physical Location GLN
payTo-code	=	"415"	; GLN of invoicing party
partyGln-code	=	"417"	; Party GLN
gsrnp-code	=	"8017"	; GSRN of the Provider
gsrn-code	=	"8018"	; GSRN of the Recipient
gcn-code	=	"255"	; GCN
sscc-code	=	"00"	; SSCC
gdti-code	=	"253"	; GDTI
ginc-code	=	"401"	; GINC
gsin-code	=	"402"	; GSIN
grai-code	=	"8003"	; GRAI
giai-code	=	"8004"	; GIAI

Key qualifiers
The following rules indicate which GS1 Application Identifiers (GS1 AI) for GS1 identifier extensions are considered as key qualifiers for a GS1 Digital Link URI. The combination of a primary identification key and key qualifier forms a compound GS1 identifier.

cpv-code	=	"22"	; Consumer Product Variant
lot-code	=	"10"	; Batch/Lot identifier
ser-code	=	"21"	; GTIN Serial Number
cpsn-code	=	"8011"	; CPID Serial Number
glnx-code	=	"254"	; GLN extension
refno-code	=	"8020"	; Payment Reference Number
srin-code	=	"8019"	; Service Relation Instance Number
tpx-code	=	"235"	; third-party controlled serialised extension to GTIN
uic-ext-code	=	"7040"	; GS1 UIC with Extension 1 and Importer Index

Primary identification key formats
The following rules express the format of the values of the primary identification keys.

Note : the GS1 General Specifications [GENSPECS] define further restrictions on some of these values, particularly for those which include a GS1 check digit, GS1 check character pair, indicator digit or extension digit. Please refer to the GS1 General Specifications [GENSPECS] for further details.

gtin-value	=	14DIGIT	; GTIN-8, GTIN-12 and GTIN-13 SHALL be expressed as 14 digits, with leading zeroes serving as filler digits
itip-value	=	14DIGIT 2DIGIT 2DIGIT	; 14 digits then 2 digits then 2 digits
gmn-value	=	1*25 XCHAR	; 1-25 characters from 82-chr subset
cpid-value	=	1*30 YCHAR	; 1-30 characters from 39-chr subset
gln-value	=	13DIGIT	; exactly 13 digits
payTo-value	=	13DIGIT	; exactly 13 digits
partyGln-value	=	13DIGIT	; exactly 13 digits
gsrnp-value	=	18DIGIT	; exactly 18 digits
gsrn-value	=	18DIGIT	; exactly 18 digits
gcn-value	=	13DIGIT [1*12DIGIT]	; 13 digits then optional 1-12 digits
sscc-value	=	18DIGIT	; exactly 18 digits
gdti-value	=	13DIGIT [1*17XCHAR]	; 13 digits then optional 1-17 characters from the 82-character subset
ginc-value	=	1*30XCHAR	; 1-30 characters from the 82-character subset
gsin-value	=	17DIGIT	; exactly 17 digits
grai-value	=	13DIGIT [1*16XCHAR]	; 13 digits then optional 1-16 characters from the 82-character subset of ISO/IEC 646
giai-value	=	1*30 XCHAR	; 1-30 characters from 82-chr subset
 
 */