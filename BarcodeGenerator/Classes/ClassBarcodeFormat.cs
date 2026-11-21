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
                SplitKeyValuePairs(url);

                if (ClassLoadDataFromFile.dicGs1.Count > 0)
                {
                    barcodeResult += $"\n{CodeLang.PayloadType_GS1DIGITALLINK_Text}";

                    foreach (var kvp in ClassLoadDataFromFile.dicGs1)
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

                            // Format the GS1 digital link key-value pairs in a readable format and append them to the barcode result string
                            barcodeResult += $"\n- {kvp.Key} {value}, {description}\n  = {kvp.Value}";

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
        /// Splits the GS1 identifier into key-value pairs and adds them to the dictionary
        /// </summary>
        /// <param name="gs1Identifier"></param>
        private static void SplitKeyValuePairs(string gs1Identifier)
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

            foreach (var key in keysToRemove)
            {
                ClassLoadDataFromFile.dicGs1.Remove(key);
            }
        }
    }
}
