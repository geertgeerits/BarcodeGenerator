namespace BarcodeGenerator
{
    internal class ClassPayloadTypes
    {
        // Global variables for Payload Types - https://github.com/Shane32/QRCoder    
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
        public static bool bPayloadEnabled;
        public static string cPayloadType = string.Empty;
        public static string cPayloadTypeDefault = string.Empty;
        public static string cPayloadResult = string.Empty;

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

            // Default values for the payload type
            cPayloadTypeDefault = CodeLang.PayloadType_URL_Text;
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
                return;
            }

            // Wi‑Fi QR payload: show details and offer to copy or open Wi‑Fi settings
            if (text.StartsWith("WIFI:", StringComparison.OrdinalIgnoreCase))
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

                string message = $"SSID: {ssid}\nPassword: {pass}\nEncryption: {auth}";
                string action = await Application.Current!.Windows[0].Page!.DisplayActionSheetAsync("Wi‑Fi network detected", CodeLang.ButtonClose_Text, null, "Copy", "Open Wi‑Fi Settings");

                if (action == "Copy")
                {
                    await Clipboard.Default.SetTextAsync($"SSID:{ssid};P:{pass};");
                }
                else if (action == "Open Wi‑Fi Settings")
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
                    await Application.Current!.Windows[0].Page!.DisplayAlertAsync("", "Please open Settings → Wi‑Fi to join the network.", CodeLang.ButtonClose_Text);
#else
                    await Application.Current!.Windows[0].Page!.DisplayAlertAsync("", "Open Wi‑Fi settings is not supported on this platform.", CodeLang.ButtonClose_Text);
#endif
                }

                return;
            }

            // Generic URI handlers (URL, geo, mailto, sms, mms, etc.)
            if (text.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                text.StartsWith("https://", StringComparison.OrdinalIgnoreCase) ||
                text.StartsWith("mailto:", StringComparison.OrdinalIgnoreCase) ||
                text.StartsWith("sms:", StringComparison.OrdinalIgnoreCase) ||
                text.StartsWith("mms:", StringComparison.OrdinalIgnoreCase) ||
                text.StartsWith("geo:", StringComparison.OrdinalIgnoreCase))
            {
                await Launcher.Default.OpenAsync(new Uri(text));
                return;
            }

            // Telephone numbers - prefer PhoneDialer where available
            if (text.StartsWith("tel:", StringComparison.OrdinalIgnoreCase))
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
                return;
            }

            // Contact (vCard) - write to a temp .vcf and let the system open/share it
            if (text.StartsWith("BEGIN:VCARD", StringComparison.OrdinalIgnoreCase))
            {
                //string file = System.IO.Path.Combine(FileSystem.Current.CacheDirectory, $"contact_{DateTime.Now:yyyyMMddHHmmss}.vcf");
                //System.IO.File.WriteAllText(file, text);
                //await Share.Default.RequestAsync(new ShareFileRequest
                //{
                //    Title = "Import Contact",
                //    File = new ShareFile(file, "text/vcard")
                //});
                
                string file = Path.Combine(FileSystem.Current.CacheDirectory, $"contact_{DateTime.Now:yyyyMMddHHmmss}.vcf");
                //File.WriteAllText(file, text, System.Text.Encoding.UTF8);  // Unable to read vCard data
                File.WriteAllText(file, text);
#if ANDROID
                BarcodeGenerator.Platforms.Android.ShareFileHandler.ShareFile(file, "text/x-vcard", "Import Contact");
#elif IOS
                BarcodeGenerator.Platforms.iOS.ShareFileHandler.ShareFile(file, "text/x-vcard", "Import Contact");
#else
                await Share.Default.RequestAsync(new ShareFileRequest { Title = "Import Contact", File = new ShareFile(file, "text/x-vcard") });
#endif
                return;
            }

            // Calendar event (iCal) - write to a temp .ics and let the system open/share it
            if (text.StartsWith("BEGIN:VCALENDAR", StringComparison.OrdinalIgnoreCase) || text.Contains("BEGIN:VEVENT"))
            {
                //string file = System.IO.Path.Combine(FileSystem.Current.CacheDirectory, $"event_{DateTime.Now:yyyyMMddHHmmss}.ics");
                //System.IO.File.WriteAllText(file, text);
                //await Share.Default.RequestAsync(new ShareFileRequest
                //{
                //    Title = "Add to Calendar",
                //    File = new ShareFile(file, "text/calendar")
                //});
                
                string file = Path.Combine(FileSystem.Current.CacheDirectory, $"event_{DateTime.Now:yyyyMMddHHmmss}.ics");
                //File.WriteAllText(file, text, System.Text.Encoding.UTF8);    // Unable to read data
                File.WriteAllText(file, text);
#if ANDROID
                BarcodeGenerator.Platforms.Android.ShareFileHandler.ShareFile(file, "text/calendar", "Add to Calendar");
#elif IOS
                BarcodeGenerator.Platforms.iOS.ShareFileHandler.ShareFile(file, "text/calendar", "Add to Calendar");
#else
                await Share.Default.RequestAsync(new ShareFileRequest { Title = "Add to Calendar", File = new ShareFile(file, "text/calendar") });
#endif
                return;
            }

            // Fallback: use existing generic share method
            _ = Globals.ShareBarcodeResultAsync(text);
        }
    }
}
