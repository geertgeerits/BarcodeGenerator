using System.Web;

namespace BarcodeGenerator
{
    internal class ClassBarcodeFormat
    {
        /// Process the list of scanned BarcodeResult objects, remove duplicates and sort them
        /// </summary>
        /// <param name="list"></param>
        /// <param name="btnShare"></param>
        public static string ProcessScannedBarcodes(List<string> list, Button btnShare)
        {
            // Remove duplicates and sort the list
            list = [.. list.Distinct().OrderBy(x => x)];

            // If there is only one result, display the barcode format in the share button and show the barcode value in the label
            string cBarcodeResult = string.Empty;

            if (list.Count == 1)
            {
                // parts[0] = barcode format, parts[1] = barcode value
                // Split the string using both ":" and "\n" as delimiters, and remove empty entries
                string[] parts = list[0].Split([":\n"], StringSplitOptions.None);
                btnShare.Text = $"{CodeLang.ButtonShare_Text} {parts[0]}";
                cBarcodeResult = parts.Length > 1 ? parts[1] : "";

                // If the barcode value starts with a URL, it can be parsed for GS1 digital link key-value pairs
                FormatGS1DigitalLinkAppIdValuePairs(parts[1], ref cBarcodeResult);
            }

            // If there are multiple results, display the count in the share button and show all results in the label
            else if (list.Count > 1)
            {
                btnShare.Text = $"{CodeLang.ButtonShare_Text} ({list.Count})";

                foreach (string item in list)
                {
                    string[] parts = item.Split([":\n"], StringSplitOptions.None);
                    cBarcodeResult += parts[0] + "\n";
                    cBarcodeResult += parts.Length > 1 ? parts[1] : "";

                    // If the barcode value starts with a URL, it can be parsed for GS1 digital link key-value pairs
                    FormatGS1DigitalLinkAppIdValuePairs(parts[1], ref cBarcodeResult);

                    cBarcodeResult += "\n\n";
                }
            }

            else
            {
                cBarcodeResult = CodeLang.BarcodeNotFound_Text;
            }

            return cBarcodeResult;
        }

        /// <summary>
        /// Format the GS1 Digital Link key-value pairs from the URL and append them to the barcode result string.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="barcodeResult"></param>
        /// <returns></returns>
        private static void FormatGS1DigitalLinkAppIdValuePairs(string url, ref string barcodeResult)
        {
            // If the barcode value starts with a URL, it can be parsed for GS1 digital link key-value pairs
            if (url.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            {
                // Parse the URL for GS1 digital link key-value pairs and display them in a formatted manner
                SplitScannedKeyValuePairs(url);

                if (ClassLoadDataFromFile.dicGs1.Count > 0)
                {
                    barcodeResult += $"\r\n\r\n{CodeLang.PayloadType_GS1DIGITALLINK_Text}";

                    foreach (KeyValuePair<string, string> kvp in ClassLoadDataFromFile.dicGs1)
                    {
                        Debug.WriteLine($"dicGs1 kvp Key and Value: {kvp.Key} - {kvp.Value}");

                        string value = ClassPayloadGS1Validation.GetKnownGs1AppIDValue(kvp.Key);
                        string valueDate1 = string.Empty;
                        string valueDate2 = string.Empty;
                        string countryName = string.Empty;
                        string currencyName = string.Empty;

                        // Get the known GS1 Application Identifier (AI) name from the language resource file and append it to the barcode result string
                        string description = ClassPayloadGS1Validation.GetKnownGs1AppIDValueFromLangResource(kvp.Key);

                        try
                        {
                            // Dates / Times
                            // If the value is a date or date/time, format it as a date or date/time string in the current culture format
                            // 6 DIGIT (date YYMMDD) - Format the first 6 digits of the value as a date string in the current culture format
                            if (kvp.Key is "11" or "12" or "13" or "15" or "16" or "17" or "4324" or "4325" or "4326"
                                or "7003" or "7006" or "7007" or "7011" or "8008")
                            {
                                if (kvp.Value.Length >= 6)
                                {
                                    if (DateTime.TryParseExact(kvp.Value[..6], "yyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateValue))
                                    {
                                        valueDate1 = dateValue.ToString("d", CultureInfo.CurrentCulture);
                                    }
                                }
                            }

                            // 8 DIGIT (date YYYYMMDD) - Format the first 8 digits of the value as a date string in the current culture format
                            else if (kvp.Key is "7250" or "7251")
                            {
                                if (kvp.Value.Length >= 8)
                                {
                                    if (DateTime.TryParseExact(kvp.Value[..8], "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateValue))
                                    {
                                        valueDate1 = dateValue.ToString("d", CultureInfo.CurrentCulture);
                                    }
                                }
                            }

                            // 6 DIGIT [4 DIGIT] (date/time YYMMDD[hhmm])
                            if (kvp.Key == "7011" && kvp.Value.Length == 10)
                            {
                                valueDate1 += $" {kvp.Value[6..10].Insert(2, ":")}";
                            }

                            // 6 DIGIT [6 DIGIT] (date YYMMDD[YYMMDD]) or 12 DIGIT (date YYMMDDYYMMDD)
                            else if ((kvp.Key is "7007") && kvp.Value.Length == 12)
                            {
                                if (DateTime.TryParseExact(kvp.Value[6..12], "yyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateValue2))
                                {
                                    valueDate2 = dateValue2.ToString("d", CultureInfo.CurrentCulture);
                                    valueDate1 += $" - {valueDate2}";
                                }
                            }

                            // 8 DIGIT [2 DIGIT] [2 DIGIT] (date/time YYMMDDhh[mm][ss])
                            else if (kvp.Key == "8008")
                            {
                                switch (kvp.Value.Length)
                                {
                                    case 8:
                                        valueDate2 = $" {kvp.Value[6..8]}:00";
                                        break;
                                    case 10:
                                        valueDate2 = $" {kvp.Value[6..8]}:{kvp.Value[8..10]}";
                                        break;
                                    case 12:
                                        valueDate2 = $" {kvp.Value[6..8]}:{kvp.Value[8..10]}:{kvp.Value[10..12]}";
                                        break;
                                }

                                valueDate1 += $" {valueDate2}";
                            }

                            // 10 DIGIT (date/time YYMMDDhhmm)
                            else if ((kvp.Key is "4324" or "4325" or "7003") && kvp.Value.Length == 10)
                            {
                                valueDate2 = $" {kvp.Value[6..8]}:{kvp.Value[8..10]}";
                                valueDate1 += $" {valueDate2}";
                            }

                            // 12 DIGIT (date/time YYYYMMDDhhmm)
                            else if ((kvp.Key is "7251") && kvp.Value.Length == 12)
                            {
                                valueDate2 = $" {kvp.Value[8..10]}:{kvp.Value[10..12]}";
                                valueDate1 += $" {valueDate2}";
                            }

                            // Decode the value to ensure it is human-readable, especially for values that may have been URL-encoded
                            string decodedValue = HttpUtility.UrlDecode(kvp.Value);

                            // Format the GS1 digital link key-value pairs in a readable format and append them to the barcode result string
                            barcodeResult += $"\n- {kvp.Key} {value}, {description}\n  = {decodedValue}";

                            if (valueDate1 != string.Empty)
                            {
                                barcodeResult += $" ({valueDate1})";
                            }

                            // ISO Country Codes
                            // If the value is an ISO country code, format it as a country name in the current culture format
                            if (kvp.Key is "421" or "7030" or "7031" or "7032" or "7033" or "7034" or "7035" or "7036" or "7037" or "7038" or "7039")
                            {
                                countryName = ClassPayloadGS1Validation.GetKnownISOCountry(kvp.Value[..3]);

                                if (countryName != string.Empty)
                                {
                                    barcodeResult += $" ({countryName} {kvp.Value[3..]})";
                                }
                            }

                            // ISO Currency Codes
                            // If the value is an ISO currency code, format it as a currency name in the current culture format
                            if (kvp.Key is "3910" or "3911" or "3912" or "3913" or "3914" or "3915" or "3916" or "3917" or "3918" or "3919"
                                or "3930" or "3931" or "3932" or "3933" or "3934" or "3935" or "3936" or "3937" or "3938" or "3939")
                            {
                                currencyName = ClassPayloadGS1Validation.GetKnownISOCurrency(kvp.Value[..3]);

                                if (currencyName != string.Empty)
                                {
                                    barcodeResult += $" ({currencyName} {kvp.Value[3..]})";
                                }
                            }

                            // Numeric values with decimal point
                            if (kvp.Key.StartsWith('3') && kvp.Key.Length == 4)
                            {
                                string formattedValue = FormatGs1ValueWithDecimalPoint(kvp.Key, kvp.Value);
                                barcodeResult += $" ({formattedValue})";
                            }
                        }

                        catch (Exception ex)
                        {
                            Debug.WriteLine($"ClassBarcodes - FormatGS1DigitalLinkAppIdValuePairs:\n\n{ex.Message}", "OK");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Format the numeric GS1 Digital Link value with a decimal point
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string FormatGs1ValueWithDecimalPoint(string key, string value)
        {
            // The last number of the key is the number of decimal places, the value is a string of digits
            int decimalPlaces = int.Parse(key.Last().ToString());

            // Set the decimal point in the value
            if (decimalPlaces > 0 && value.Length > decimalPlaces)
            {
                int integerPartLength = value.Length - decimalPlaces;
                string integerPart = value[..integerPartLength];
                string decimalPart = value[integerPartLength..];
                value = $"{integerPart}.{decimalPart}";
            }

            // Format the value according the UI - decimal point or decimal comma
            decimal number = decimal.Parse(value, CultureInfo.InvariantCulture);
            return number.ToString("N" + decimalPlaces, CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Splits the GS1 identifier into key-value pairs and adds them to the dictionary after scanning the GS1 digital link barcode.
        /// The keys are surrounded by slashes (/) or start with a question mark (?) or start with an ampersand (&), and values are separated by equal signs (=) for keys started with question mark or ampersand.
        /// </summary>
        /// <param name="gs1Identifier"></param>
        private static void SplitScannedKeyValuePairs(string gs1Identifier)
        {
            // Keys are surrounded by slashes (/) or start with a question mark (?) or start with an ampersand (&)
            // and values are separated by equal signs (=) for keys started with question mark or ampersand.
            // Example: /00/195201234567891232?02=09520123456788&37=25&10=ABC123
            //          /01/09520123456788?8004=9520614141234567

            ClassLoadDataFromFile.dicGs1.Clear();

            if (string.IsNullOrWhiteSpace(gs1Identifier))
            {
                return;
            }

            // Split into path part (/01/09520123456788/...) and query part (?10=ABC123&37=25)
            int queryStart = gs1Identifier.IndexOf('?');
            string pathPart = queryStart >= 0 ? gs1Identifier[..queryStart] : gs1Identifier;
            string queryPart = queryStart >= 0 ? gs1Identifier[(queryStart + 1)..] : string.Empty;

            // Parse slash-based key/value pairs: /01/09520123456788/21/ABC123
            string[] pathSegments = pathPart.Split('/', StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i + 1 < pathSegments.Length; i += 2)
            {
                string key = pathSegments[i];
                string value = pathSegments[i + 1];
                ClassLoadDataFromFile.dicGs1[key] = value;
            }

            // Parse query-based key/value pairs: 10=ABC123&37=25
            if (!string.IsNullOrWhiteSpace(queryPart))
            {
                string[] queryPairs = queryPart.Split('&', StringSplitOptions.RemoveEmptyEntries);
                foreach (string pair in queryPairs)
                {
                    int equalsIndex = pair.IndexOf('=');
                    if (equalsIndex > 0)
                    {
                        string key = pair[..equalsIndex];
                        string value = pair[(equalsIndex + 1)..];
                        ClassLoadDataFromFile.dicGs1[key] = value;
                    }
                }
            }

            // Remove the keys from the dictionary if the key is not in the list of known GS1 application identifiers
            List<string> keysToRemove = [.. ClassLoadDataFromFile.dicGs1.Keys.Where(static k => !ClassPayloadGS1Validation.IsKnownGs1AppID(k))];

            foreach (string key in keysToRemove)
            {
                ClassLoadDataFromFile.dicGs1.Remove(key);
            }
        }

        /// <summary>
        /// Order the GS1 Digital Link with the primary identification keys and key qualifiers in the correct order according to the GS1 Digital Link specification.
        /// I do not understand why the GS1 Digital Link specification require the primary identification keys and key qualifiers to be
        /// in a specific order, with the application identifiers (AIs) in the path and the data attributes in the query string,
        /// the reading software should be able to parse the GS1 Digital Link regardless of the order of the AIs and data attributes.
        /// </summary>
        /// <param name="gs1IdentifierOriginal"></param>
        /// <returns></returns>
        public static string OrderGS1DigitalLink(string gs1IdentifierOriginal)
        {
            /*
            Path element order
            The following rules express how the URI path information should be structured for each primary 
            identification key. Note that some primary identification keys such as SSCC do not have any 
            associated key qualifier. Other primary identification keys such as GTIN, may have multiple key 
            qualifiers. The square bracket notation indicates that the enclosed key qualifier component may be 
            omitted but the sequence in which they appear is important and must be preserved. For example, 
            the rule for gtin-path would permit any of these: 
            
            01   gtin-path       =   01 gtin-comp       22 [cpv-comp]    10 [lot-comp]    21 [ser-comp] 
            8006 itip-path       = 8006 itip-comp       22 [cpv-comp]    10 [lot-comp]    21 [ser-comp]
            8013 gmn-path        = 8013 gmn-comp
            8010 cpid-path       = 8010 cpid-comp     8011 [cpsn-comp] 
            414  gln-path        =  414 gln-comp       254 [glnx-comp]
            415  payTo-path      =  415 payTo-comp    8020 refNo-comp
            417  partyGln-path   =  417 partyGln-comp 
            8017 gsrnp-path      = 8017 gsrnp-comp    8019 [srin-comp]
            8018 gsrn-path       = 8018 gsrn-comp     8019 [srin-comp] 
            255  gcn-path        =  255 gcn-comp 
            00   sscc-path       =   00 sscc-comp 
            253  gdti-path       =  253 gdti-comp  
            401  ginc-path       =  401 ginc-comp 
            402  gsin-path       =  402 gsin-comp
            8003 grai-path       = 8003 grai-comp
            8004 giai-path       = 8004 giai-comp
                 upui-path       =   01 gtin-comp      235 tpx-comp
                 eoid-path       =  417 partyGln-comp 7040 uic-ext-comp 
                 fid-path        =  414 gln-comp      7040 uic-ext-comp
                 mid-path        = 8004 giai-comp     7040 uic-ext-comp
            */

            string gs1Identifier = string.Empty;
            
            if (ClassLoadDataFromFile.dicGs1.ContainsKey("00")) gs1Identifier = $"(00){GetdicGs1Value("00")}";

            if (ClassLoadDataFromFile.dicGs1.ContainsKey("01")) gs1Identifier += $"(01){GetdicGs1Value("01")}";
            if (ClassLoadDataFromFile.dicGs1.ContainsKey("8006")) gs1Identifier += $"(8006){GetdicGs1Value("8006")}";
            if (ClassLoadDataFromFile.dicGs1.ContainsKey("22")) gs1Identifier += $"(22){GetdicGs1Value("22")}";
            if (ClassLoadDataFromFile.dicGs1.ContainsKey("10")) gs1Identifier += $"(10){GetdicGs1Value("10")}";
            if (ClassLoadDataFromFile.dicGs1.ContainsKey("21")) gs1Identifier += $"(21){GetdicGs1Value("21")}";
            if (ClassLoadDataFromFile.dicGs1.ContainsKey("01"))
            {
                if (ClassLoadDataFromFile.dicGs1.ContainsKey("235")) gs1Identifier += $"(235){GetdicGs1Value("235")}";
            }

            if (ClassLoadDataFromFile.dicGs1.ContainsKey("8013")) gs1Identifier += $"(8013){GetdicGs1Value("8013")}";

            if (ClassLoadDataFromFile.dicGs1.ContainsKey("8010")) gs1Identifier += $"(8010){GetdicGs1Value("8010")}";
            if (ClassLoadDataFromFile.dicGs1.ContainsKey("8011")) gs1Identifier += $"(8011){GetdicGs1Value("8011")}";

            if (ClassLoadDataFromFile.dicGs1.ContainsKey("414")) gs1Identifier += $"(414){GetdicGs1Value("414")}";
            if (ClassLoadDataFromFile.dicGs1.ContainsKey("254")) gs1Identifier += $"(254){GetdicGs1Value("254")}";

            if (ClassLoadDataFromFile.dicGs1.ContainsKey("415")) gs1Identifier += $"(415){GetdicGs1Value("415")}";
            if (ClassLoadDataFromFile.dicGs1.ContainsKey("8020")) gs1Identifier += $"(8020){GetdicGs1Value("8020")}";

            if (ClassLoadDataFromFile.dicGs1.ContainsKey("417")) gs1Identifier += $"(417){GetdicGs1Value("417")}";

            if (ClassLoadDataFromFile.dicGs1.ContainsKey("8017")) gs1Identifier += $"(8017){GetdicGs1Value("8017")}";
            if (ClassLoadDataFromFile.dicGs1.ContainsKey("8018")) gs1Identifier += $"(8018){GetdicGs1Value("8018")}";
            if (ClassLoadDataFromFile.dicGs1.ContainsKey("8019")) gs1Identifier += $"(8019){GetdicGs1Value("8019")}";

            if (ClassLoadDataFromFile.dicGs1.ContainsKey("255")) gs1Identifier += $"(255){GetdicGs1Value("255")}";

            if (ClassLoadDataFromFile.dicGs1.ContainsKey("253")) gs1Identifier += $"(253){GetdicGs1Value("253")}";

            if (ClassLoadDataFromFile.dicGs1.ContainsKey("401")) gs1Identifier += $"(401){GetdicGs1Value("401")}";

            if (ClassLoadDataFromFile.dicGs1.ContainsKey("402")) gs1Identifier += $"(402){GetdicGs1Value("402")}";

            if (ClassLoadDataFromFile.dicGs1.ContainsKey("8003")) gs1Identifier += $"(8003){GetdicGs1Value("8003")}";

            if (ClassLoadDataFromFile.dicGs1.ContainsKey("8004")) gs1Identifier += $"(8004){GetdicGs1Value("8004")}";
            if (ClassLoadDataFromFile.dicGs1.ContainsKey("7040")) gs1Identifier += $"(7040){GetdicGs1Value("7040")}";

            // Sort the dictionary by key and then iterate through it to ensure that the AIs are added in order
            // This way, you can maintain the order of the AIs in the GS1 identifier string
            IOrderedEnumerable<KeyValuePair<string, string>> sortedDicGs1Order = ClassLoadDataFromFile.dicGs1.OrderBy(static kvp => kvp.Key);

            // Add any remaining GS1 Application Identifiers (AIs) that are not part of the primary identification keys or key qualifiers
            foreach (KeyValuePair<string, string> kvp in sortedDicGs1Order)
            {
                if (kvp.Key is not "01" and not "8006" and not "8013" and not "8010" and not "414" and not "415" and not "417"
                    and not "8017" and not "8018" and not "255" and not "00" and not "253" and not "401" and not "402" and not "8003" and not "8004"
                    and not "22" and not "10" and not "21" and not "8011" and not "254" and not "8020" and not "8019" and not "235" and not "7040")
                {
                    gs1Identifier += $"({kvp.Key}){GetdicGs1Value(kvp.Key)}";
                }
            }

            // Return the ordered GS1 identifier
            return gs1Identifier;
        }

        /// <summary>
        /// Format the GS1 identifier to a valid GS1 Digital Link
        /// </summary>
        /// <param name="gs1Identifier"></param>
        /// <param name="cAppId"></param>
        /// <param name="cValue"></param>
        /// <returns></returns>
        public static string FormatGS1DigitalLink(string gs1Identifier, string cAppId, string cValue)
        {
            // If the AI is one of the primary identification keys or key qualifiers, it must be preceded and followed by a slash.
            if (cAppId is "01" or "8006" or "8013" or "8010" or "414" or "415" or "417" or "8017"
                or "8018" or "255" or "00" or "253" or "401" or "402" or "8003" or "8004"
                or "22" or "10" or "21" or "8011" or "254" or "8020" or "235" or "7040")
            {
                // Replace the parentheses with slashes
                if (gs1Identifier.Contains($"({cAppId})"))
                {
                    gs1Identifier = gs1Identifier.Replace($"({cAppId})", $"/{cAppId}/");
                }
            }

            // If the AI is one of the data attributes, it must be preceded by a question mark and followed by an equal sign,
            // or preceded by an ampersand and followed by an equal sign.
            else
            {
                // The AI must be preceded by a question mark or ampersand and followed by an equal sign
                if (gs1Identifier.Contains($"({cAppId})"))
                {
                    // Replace the parentheses with a question mark and equal sign.
                    // There can be only one data attribute that is preceded by a question mark and followed by an equal sign,
                    // and all other data attributes must be preceded by an ampersand and followed by an equal sign.
                    if (!gs1Identifier.Contains('?'))
                    {
                        gs1Identifier = gs1Identifier.Replace($"({cAppId})", $"?{cAppId}=");
                    }

                    else
                    {
                        gs1Identifier = gs1Identifier.Replace($"({cAppId})", $"&{cAppId}=");
                    }
                }
            }

            // Escape the value to ensure it is valid for an URL
            // (414)9520123456788(254)32a/b -> /414/9520123456788/254/32a/b -> /414/9520123456788/254/32a%2Fb
            gs1Identifier = gs1Identifier.Replace(cValue, Uri.EscapeDataString(cValue));

            return gs1Identifier;
        }

        /// <summary>
        /// Get the value connected to a certain key cAppId in the dictionary dicGs1
        /// </summary>
        /// <param name="cAppId"></param>
        /// <returns></returns>
        private static string GetdicGs1Value(string cAppId)
        {
            return ClassLoadDataFromFile.dicGs1.TryGetValue(cAppId, out string? value) ? value : string.Empty;
        }
    }
}
