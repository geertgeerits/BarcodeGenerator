namespace BarcodeGenerator
{
    internal partial class ClassPayloadGS1Validation
    {
        // Dictionary to hold GS1 Application Identifiers (AI) and their corresponding values
        public static readonly Dictionary<string, string> dicGs1 = [];

        // Add once in the class - central place for all supported GS1 Application Identifiers (AI) (keys and data attributes)
        public static readonly Dictionary<string, string> _knownGs1AppIDs = [];

        /// <summary>
        /// Copy the GS1 Application Identifiers and their values from a text file to a dictionary for validation
        /// </summary>
        public static async Task LoadKnownGs1AppIDs()
        {
            // Load the GS1 Application Identifiers and their values from a text file into the dictionary
            await using var stream = await FileSystem.OpenAppPackageFileAsync("GS1ApplicationIdentifiers.txt");
            using var reader = new StreamReader(stream);

            string? line;
            while ((line = await reader.ReadLineAsync()) is not null)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                string[] parts = line.Split(';', 2, StringSplitOptions.TrimEntries);
                if (parts.Length != 2 || string.IsNullOrWhiteSpace(parts[0]))
                {
                    continue;
                }

                if (_knownGs1AppIDs.TryAdd(parts[0], parts[1]))
                {
                    Debug.WriteLine($"Added GS1 Application Identifier: {parts[0]} = {parts[1]}");
                }
            }
        }

        // Character sets
        private static readonly string _DIGIT = "0123456789";
        private static readonly string _UPPERALPHA = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private static readonly string _LOWERALPHA = "abcdefghijklmnopqrstuvwxyz";
        private static readonly string _YSYMBOL = "-#/";
        private static readonly string _ZSYMBOL = "-_=";
        private static readonly string _XSYMBOL = "\"-._!%&+,/*[]';:<>=?";

        // 39-character subset of ISO/IEC 646 (for GS1 identifiers) - YCHAR = DIGIT / UPPERALPHA / YSYMBOL 
        private static readonly string _YCHAR_39CharacterSubset = _DIGIT + _UPPERALPHA + _YSYMBOL;

        // 64-character subset of ISO/IEC 646 (for GS1 identifiers) - ZCHAR = DIGIT / UPPERALPHA / LOWERALPHA / ZSYMBOL
        private static readonly string _ZCHAR_64CharacterSubset = _DIGIT + _UPPERALPHA + _LOWERALPHA + _ZSYMBOL;

        // 82-character subset of ISO/IEC 646 (for GS1 identifiers) - XCHAR = DIGIT / UPPERALPHA / LOWERALPHA / XSYMBOL
        private static readonly string _XCHAR_82CharacterSubset = _DIGIT + _UPPERALPHA + _LOWERALPHA + _XSYMBOL;

        // Character set for IBAN (International Bank Account Number) validation - 1 * 34 XCHAR = 1-34 characters
        private static readonly string _IBAN_CharacterSet = _DIGIT + _UPPERALPHA;

        // Error message for GS1 identifier validation
        private static string cErrorMessage = string.Empty;

        /// <summary>
        /// Splits the GS1 identifier into key-value pairs and adds them to the dictionary
        /// </summary>
        /// <param name="gs1Identifier"></param>
        public static void SplitKeyValuePairs(string gs1Identifier)
        {
            // Keys are surrounded by slashes (/) or start with a question mark (?) or start with an ampersand (&)
            // and values are separated by equal signs (=) for keys started with question mark or ampersand.
            // Example: /00/195201234567891232?02=09520123456788&37=25&10=ABC123
            //          /01/09520123456788?8004=9520614141234567

            dicGs1.Clear();

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
                dicGs1[key] = value;
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
                        dicGs1[key] = value;
                    }
                }
            }

            // Delete the key-value pairs if they are not in the _knownGs1AppIDs dictionary
            List<string> keysToRemove = [.. dicGs1.Keys.Where(static key => !IsKnownGs1AppID(key))];
            
            foreach (string key in keysToRemove)
            {
                dicGs1.Remove(key);
            }
        }

        /// <summary>
        /// Determines whether the specified string is a valid GS1 identifier
        /// </summary>
        /// <param name="editor"></param>
        /// <returns></returns>
        public static async Task<bool> IsValidGS1Identifier(Editor editor)
        {
            bool bIsValid = true;

            SplitKeyValuePairs(editor.Text);

            Debug.WriteLine($"Length dictionary _knownGs1AppIDs: {_knownGs1AppIDs.Count}");

            foreach (var kvp in dicGs1)
            {
                Debug.WriteLine($"AI: {kvp.Key}, Value: {kvp.Value}, Value length: {kvp.Value.Length}");
                bIsValid &= await IsValidGS1IdentifierPart2(editor, kvp.Key, kvp.Value);
            }

            return bIsValid;
        }

        /// <summary>
        /// Determines whether the specified string is a valid GS1 identifier
        /// </summary>
        /// <param name="editor">The Editor containing the GS1 identifier string to validate.</param>
        /// <param name="cKey">The GS1 key to validate.</param>
        /// <param name="cValue">The GS1 value to validate.</param>
        /// <returns>true if the specified string is a valid GS1 identifier; otherwise, false.</returns>
        private static async Task<bool> IsValidGS1IdentifierPart2(Editor editor, string cKey, string cValue)
        {
            // GS1 Digital Link Test Suite: https://ref.gs1.org/test-suites/resolver/
            // GS1 Digital Link Validator: https://qrbolt.com/en/validate-gs1
            // FeatherQR Playground: https://guitarrapc.github.io/FeatherQR/

            // Variables
            string gs1Identifier = editor.Text;
            bool bIsValid = true;
            cErrorMessage = string.Empty;

            // Check if the key in the GS1 identifier exists in the array of valid keys
            bIsValid &= IsKnownGs1AppID(cKey);
            if (!bIsValid)
            {
                cErrorMessage = CodeLang.ErrorGS1AppIDInvalid_Text;
            }

            // Primary identification key formats (x 16)

            // gtin-code 01 - Global Trade Item Number (GTIN) = 14DIGIT ; GTIN-8, GTIN-12 and GTIN-13 SHALL be expressed as 14 digits, with leading zeroes serving as filler digits
            if (bIsValid && cKey == "01")
            {
                // If the GS1 identifier length is less than 14 characters, add zeroes to the left to make it 14 characters
                // and replace the original value with the new value in the editor
                if (cValue.Length < 14)
                {
                    cValue = cValue.PadLeft(14, '0');
                    editor.Text = gs1Identifier.Replace(GetValue(GetAppID("01", gs1Identifier), gs1Identifier), cValue);
                    gs1Identifier = editor.Text;
                }

                // The GTIN must have a valid check digit (/01/09520123456788 is valid, /01/09520123456789 is invalid)
                if (bIsValid && !ValidateValueCheckDigitModulo10(cValue, 14))
                {
                    bIsValid = false;
                }

                // Check the path element order for /22/, /10/, and /21/ if they exist in the GS1 identifier
                if (bIsValid)
                {
                    int nStartPos22 = GetAppIDStartPosition("/22/", gs1Identifier);
                    int nStartPos10 = GetAppIDStartPosition("/10/", gs1Identifier);
                    int nStartPos21 = GetAppIDStartPosition("/21/", gs1Identifier);
                    Debug.WriteLine($"Start Position 22: {nStartPos22}");
                    Debug.WriteLine($"Start Position 10: {nStartPos10}");
                    Debug.WriteLine($"Start Position 21: {nStartPos21}");

                    // nStartPos22 must be less than nStartPos10 and nStartPos10 must be less than nStartPos21 - /22/, /10/, and /21/
                    if (nStartPos22 != -1 && nStartPos10 != -1 && nStartPos21 != -1)
                    {
                        if (!(nStartPos22 < nStartPos10 && nStartPos10 < nStartPos21))
                        {
                            bIsValid = false;
                        }
                    }

                    // If only two of the three keys exist, check their order - /22/ and /10/
                    else if (nStartPos22 != -1 && nStartPos10 != -1 && nStartPos21 == -1)
                    {
                        if (!(nStartPos22 < nStartPos10))
                        {
                            bIsValid = false;
                        }
                    }

                    // If only two of the three keys exist, check their order - /22/ and /21/
                    else if (nStartPos22 != -1 && nStartPos10 == -1 && nStartPos21 != -1)
                    {
                        if (!(nStartPos22 < nStartPos21))
                        {
                            bIsValid = false;
                        }
                    }

                    // If only two of the three keys exist, check their order - /10/ and /21/
                    else if (nStartPos22 == -1 && nStartPos10 != -1 && nStartPos21 != -1)
                    {
                        if (!(nStartPos10 < nStartPos21))
                        {
                            bIsValid = false;
                        }
                    }

                    // If only one of the three keys exist, no order check is needed

                    // If the order of the keys is invalid, set the error message
                    if (!bIsValid)
                    {
                        cErrorMessage = CodeLang.ErrorGS1AppIDOrderInvalid_Text;
                    }
                }
            }

            // itip-code 8006 - International Ticketing Information Protocol (ITIP) = 14DIGIT 2DIGIT 2DIGIT ; 14 digits then 2 digits then 2 digits
            else if (bIsValid && cKey == "8006")
            {
                // If the GS1 identifier length is less than 18 characters, add zeroes to the left to make it 18 characters
                // and replace the original value with the new value in the editor
                if (cValue.Length < 18)
                {
                    cValue = cValue.PadLeft(18, '0');
                    editor.Text = gs1Identifier.Replace(GetValue(GetAppID("8006", gs1Identifier), gs1Identifier), cValue);
                    gs1Identifier = editor.Text;
                }

                // The ITIP must have a valid check digit (/8006/095201234567881122 is valid, /8006/095201234567891122 is invalid)
                if (bIsValid && !ValidateValueCheckDigitModulo10(cValue[..14], 14))
                {
                    bIsValid = false;
                }

                // Check the path element order for /22/, /10/, and /21/ if they exist in the GS1 identifier
                if (bIsValid)
                {
                    int nStartPos22 = GetAppIDStartPosition("/22/", gs1Identifier);
                    int nStartPos10 = GetAppIDStartPosition("/10/", gs1Identifier);
                    int nStartPos21 = GetAppIDStartPosition("/21/", gs1Identifier);

                    // nStartPos22 must be less than nStartPos10 and nStartPos10 must be less than nStartPos21 - /22/, /10/, and /21/
                    if (nStartPos22 != -1 && nStartPos10 != -1 && nStartPos21 != -1)
                    {
                        if (!(nStartPos22 < nStartPos10 && nStartPos10 < nStartPos21))
                        {
                            bIsValid = false;
                        }
                    }

                    // If only two of the three keys exist, check their order - /22/ and /10/
                    else if (nStartPos22 != -1 && nStartPos10 != -1 && nStartPos21 == -1)
                    {
                        if (!(nStartPos22 < nStartPos10))
                        {
                            bIsValid = false;
                        }
                    }

                    // If only two of the three keys exist, check their order - /22/ and /21/
                    else if (nStartPos22 != -1 && nStartPos10 == -1 && nStartPos21 != -1)
                    {
                        if (!(nStartPos22 < nStartPos21))
                        {
                            bIsValid = false;
                        }
                    }

                    // If only two of the three keys exist, check their order - /10/ and /21/
                    else if (nStartPos22 == -1 && nStartPos10 != -1 && nStartPos21 != -1)
                    {
                        if (!(nStartPos10 < nStartPos21))
                        {
                            bIsValid = false;
                        }
                    }

                    // If only one of the three keys exist, no order check is needed

                    // If the order of the keys is invalid, set the error message
                    if (!bIsValid)
                    {
                        cErrorMessage = CodeLang.ErrorGS1AppIDOrderInvalid_Text;
                    }
                }
            }

            // gmn-code 8013 - Global Model Number (GMN) = 1*25XCHAR ; 1-25 characters from 82-character subset
            else if (bIsValid && cKey == "8013")
            {
                // If the GS1 identifier does not have a length of 1-25 characters, return false
                if (cValue.Length is < 1 or > 25)
                {
                    bIsValid = false;
                    cErrorMessage = CodeLang.ErrorGS1ValueLengthInvalid_Text;
                }

                // The characters of the GS1 identifier must be from the 82-character subset
                else if (bIsValid)
                {
                    bIsValid = ValueIsInCharacterSubset(cValue, _XCHAR_82CharacterSubset);
                }
            }

            // cpid-code 8010 - Component / Part Identifier (CPID) = 1*30YCHAR ; 1-30 characters from 39-character subset
            else if (bIsValid && cKey == "8010")
            {
                // If the GS1 identifier does not have a length of 1-30 characters, return false
                if (cValue.Length is < 1 or > 30)
                {
                    bIsValid = false;
                    cErrorMessage = CodeLang.ErrorGS1ValueLengthInvalid_Text;
                }

                // The characters of the GS1 identifier must be from the 39-character subset
                else if (bIsValid)
                {
                    bIsValid = ValueIsInCharacterSubset(cValue, _YCHAR_39CharacterSubset);
                }
            }

            // gln-code 414 - Physical / Global Location Number (GLN) = 13DIGIT ; exactly 13 digits
            else if (bIsValid && cKey == "414")
            {
                // If the GS1 identifier length is less than 13 characters, add zeroes to the left to make it 13 characters
                // and replace the original value with the new value in the editor
                if (cValue.Length < 13)
                {
                    cValue = cValue.PadLeft(13, '0');
                    editor.Text = gs1Identifier.Replace(GetValue(GetAppID("414", gs1Identifier), gs1Identifier), cValue);
                }

                // The GLN must have a valid check digit (/414/9520123456788 is valid, /414/9520123456780 is invalid)
                if (bIsValid && !ValidateValueCheckDigitModulo10(cValue, 13))
                {
                    bIsValid = false;
                }
            }

            // payTo-code 415 - GLN of invoicing party = 13DIGIT ; exactly 13 digits
            else if (bIsValid && cKey == "415")
            {
                // If the GS1 identifier does not have a length of 13 characters, return false
                if (cValue.Length != 13)
                {
                    bIsValid = false;
                    cErrorMessage = CodeLang.ErrorGS1ValueLengthInvalid_Text;
                }

                // The characters of the GS1 identifier must be numeric
                else if (bIsValid)
                {
                    bIsValid = ValueIsDigit(cValue);
                }
            }

            // partyGln-code 417 - Party GLN = 13DIGIT ; exactly 13 digits
            else if (bIsValid && cKey == "417")
            {
                // If the GS1 identifier does not have a length of 13 characters, return false
                if (cValue.Length != 13)
                {
                    bIsValid = false;
                    cErrorMessage = CodeLang.ErrorGS1ValueLengthInvalid_Text;
                }

                // The characters of the GS1 identifier must be numeric
                else if (bIsValid)
                {
                    bIsValid = ValueIsDigit(cValue);
                }
            }

            // gsrnp-code 8017 -  GSRN of the Provider = 18DIGIT ; exactly 18 digits
            else if (bIsValid && cKey == "8017")
            {
                // If the GS1 identifier does not have a length of 18 characters, return false
                if (cValue.Length != 18)
                {
                    bIsValid = false;
                    cErrorMessage = CodeLang.ErrorGS1ValueLengthInvalid_Text;
                }

                // The characters of the GS1 identifier must be numeric
                else if (bIsValid)
                {
                    bIsValid = ValueIsDigit(cValue);
                }
            }

            // gsrn-code 8018 - GSRN of the Recipient = 18DIGIT ; exactly 18 digits
            else if (bIsValid && cKey == "8018")
            {
                // If the GS1 identifier length is less than 18 characters, add zeroes to the left to make it 18 characters
                // and replace the original value with the new value in the editor
                if (cValue.Length < 18)
                {
                    cValue = cValue.PadLeft(18, '0');
                    editor.Text = gs1Identifier.Replace(GetValue(GetAppID("8018", gs1Identifier), gs1Identifier), cValue);
                }

                // If the GS1 identifier does not have a length of 18 characters, return false
                if (cValue.Length != 18)
                {
                    bIsValid = false;
                    cErrorMessage = CodeLang.ErrorGS1ValueLengthInvalid_Text;
                }

                // The GSRN must have a valid check digit (/8018/195201234567891232 is valid, /8018/195201234567891230 is invalid)
                if (bIsValid && !ValidateValueCheckDigitModulo10(cValue, 18))
                {
                    bIsValid = false;
                }
            }

            // gcn-code 255 - Global Coupon Number (GCN) =  13DIGIT [1*12DIGIT] ; 13 digits then optional 1 - 12 digits
            else if (bIsValid && cKey == "255")
            {
                // If the GS1 identifier does not have a length of 13-25 characters, return false
                if (cValue.Length is < 13 or > 25)
                {
                    bIsValid = false;
                    cErrorMessage = CodeLang.ErrorGS1ValueLengthInvalid_Text;
                }

                // If the GCN identifier length is less than 18 characters, add zeroes to the left to make it 18 characters
                // and replace the original value with the new value in the editor
                if (cValue.Length < 13)
                {
                    cValue = cValue.PadLeft(13, '0');
                    editor.Text = gs1Identifier.Replace(GetValue(GetAppID("255", gs1Identifier), gs1Identifier), cValue);
                }

                // The GCN must have a valid check digit (/255/0952012345678 is valid, /255/0952012345679 is invalid)
                if (bIsValid && !ValidateValueCheckDigitModulo10(cValue[..13], 13))
                {
                    bIsValid = false;
                }

                // The characters of the GS1 identifier must be numeric
                else if (bIsValid)
                {
                    bIsValid = ValueIsDigit(cValue);
                }
            }

            // sscc-code 00 - Serial Shipping Container Code (SSCC) = 18DIGIT ; exactly 18 digits 
            else if (bIsValid && cKey == "00")
            {
                // If the GS1 identifier length is less than 18 characters, add zeroes to the left to make it 18 characters
                // and replace the original value with the new value in the editor
                if (cValue.Length < 18)
                {
                    cValue = cValue.PadLeft(18, '0');
                    editor.Text = gs1Identifier.Replace(GetValue(GetAppID("00", gs1Identifier), gs1Identifier), cValue);
                }

                // The GTIN must have a valid check digit (/00/195201234567891232 is valid, /00/195201234567891230 is invalid)
                if (bIsValid && !ValidateValueCheckDigitModulo10(cValue, 18))
                {
                    bIsValid = false;
                }
            }

            // gdti-code 253 - Global Document Type Identifier (GDTI) = 13DIGIT [1*17XCHAR] ; 13 digits then optional 1 - 17 characters ; from the 82-character subset
            else if (bIsValid && cKey == "253")
            {
                // If the GS1 identifier does not have a length of 13-30 characters, return false
                if (cValue.Length is < 13 or > 30)
                {
                    bIsValid = false;
                    cErrorMessage = CodeLang.ErrorGS1ValueLengthInvalid_Text;
                }

                // The first 13characters of the GS1 identifier must be numeric
                else if (bIsValid)
                {
                    bIsValid = ValueIsDigit(cValue[..13]);
                }

                // The characters of the GS1 identifier must be from the 82-character subset (from character 14 to the end)
                else if (bIsValid && cValue.Length > 13)
                {
                    bIsValid = ValueIsInCharacterSubset(cValue[13..], _XCHAR_82CharacterSubset);
                }
            }

            // ginc-code 401 - Global Identification Number for Consignment (GINC) = 1*30XCHAR ; 1 - 30 characters from the 82-character subset
            else if (bIsValid && cKey == "401")
            {
                // If the GS1 identifier does not have a length of 1-30 characters, return false
                if (cValue.Length is < 1 or > 30)
                {
                    bIsValid = false;
                    cErrorMessage = CodeLang.ErrorGS1ValueLengthInvalid_Text;
                }

                // The characters of the GS1 identifier must be from the 82-character subset
                else if (bIsValid)
                {
                    bIsValid = ValueIsInCharacterSubset(cValue, _XCHAR_82CharacterSubset);
                }
            }

            // gsin-code 402 - Global Shipment Identification Number (GSIN) = 17DIGIT ; exactly 17 digits
            else if (bIsValid && cKey == "402")
            {
                // If the GS1 identifier length is less than 14 characters, add zeroes to the left to make it 14 characters
                // and replace the original value with the new value in the editor
                if (cValue.Length < 17)
                {
                    cValue = cValue.PadLeft(17, '0');
                    editor.Text = gs1Identifier.Replace(GetValue(GetAppID("402", gs1Identifier), gs1Identifier), cValue);
                }

                // If the GS1 identifier does not have a length of 17 characters, return false
                if (cValue.Length != 17)
                {
                    bIsValid = false;
                    cErrorMessage = CodeLang.ErrorGS1ValueLengthInvalid_Text;
                }

                // The GSIN must have a valid check digit (/402/9520123456788 is valid, /402/09520123456789000 is invalid)
                if (bIsValid && !ValidateValueCheckDigitModulo10(cValue, 17))
                {
                    bIsValid = false;
                }
            }

            // grai-code 8003 - Global Returnable Asset Identifier (GRAI) = 13DIGIT [1*16XCHAR] ; 13 digits then optional 1 - 16 characters ; from the 82-character subset of ISO / IEC 646
            else if (bIsValid && cKey == "8003")
            {
                // If the GS1 identifier does not have a length of 13-29 characters, return false
                if (cValue.Length is < 13 or > 29)
                {
                    bIsValid = false;
                    cErrorMessage = CodeLang.ErrorGS1ValueLengthInvalid_Text;
                }

                // The characters of the GS1 identifier must be from the 82-character subset (from character 14 to the end)
                else if (bIsValid && cValue.Length > 13)
                {
                    bIsValid = ValueIsInCharacterSubset(cValue[13..], _XCHAR_82CharacterSubset);
                }

                // If the GS1 identifier length is less than 13 characters, add zeroes to the left to make it 13 characters
                // and replace the original value with the new value in the editor
                if (cValue.Length < 13)
                {
                    cValue = cValue.PadLeft(13, '0');
                    editor.Text = gs1Identifier.Replace(GetValue(GetAppID("8003", gs1Identifier), gs1Identifier), cValue);
                }

                // The GRAI must have a valid check digit (/8003/9520123456788 is valid, /8003/9520123456789 is invalid)
                if (bIsValid && !ValidateValueCheckDigitModulo10(cValue, 13))
                {
                    bIsValid = false;
                }
            }

            // giai-code 8004 - Global Individual Asset Identifier (GIAI) = 1*30XCHAR  ; 1-30 characters from 82-character subset
            else if (bIsValid && cKey == "8004")
            {
                // If the GS1 identifier does not have a length of 1-30 characters, return false
                if (cValue.Length is < 1 or > 30)
                {
                    bIsValid = false;
                    cErrorMessage = CodeLang.ErrorGS1ValueLengthInvalid_Text;
                }

                // The characters of the GS1 identifier must be from the 82-character subset
                else if (bIsValid)
                {
                    bIsValid = ValueIsInCharacterSubset(cValue, _XCHAR_82CharacterSubset);
                }
            }

            // Key qualifier formats ( x 9)

            // cpv-code 22 - Consumer Product Variant (CPV) = 1*20XCHAR ; 1-20 characters from 82-character subset
            else if (bIsValid && cKey == "22")
            {
                // If the GS1 identifier does not have a length of 1-20 characters, return false
                if (cValue.Length is < 1 or > 20)
                {
                    bIsValid = false;
                    cErrorMessage = CodeLang.ErrorGS1ValueLengthInvalid_Text;
                }

                // The characters of the GS1 identifier must be from the 82-character subset
                else if (bIsValid)
                {
                    bIsValid = ValueIsInCharacterSubset(cValue, _XCHAR_82CharacterSubset);
                }
            }

            // lot-code 10 - Batch/Lot identifier = 1*20XCHAR ; 1-20 characters from 82-character subset
            else if (bIsValid && cKey == "10")
            {
                // If the GS1 identifier does not have a length of 1-20 characters, return false
                if (cValue.Length is < 1 or > 20)
                {
                    bIsValid = false;
                    cErrorMessage = CodeLang.ErrorGS1ValueLengthInvalid_Text;
                }

                // The characters of the GS1 identifier must be from the 82-character subset
                else if (bIsValid)
                {
                    bIsValid = ValueIsInCharacterSubset(cValue, _XCHAR_82CharacterSubset);
                }
            }

            // ser-code 21 - GTIN Serial Number = 1*20XCHAR ; 1-20 characters from 82-character subset
            else if (bIsValid && cKey == "21")
            {
                // If the GS1 identifier does not have a length of 1-20 characters, return false
                if (cValue.Length is < 1 or > 20)
                {
                    bIsValid = false;
                    cErrorMessage = CodeLang.ErrorGS1ValueLengthInvalid_Text;
                }

                // The characters of the GS1 identifier must be from the 82-character subset
                else if (bIsValid)
                {
                    bIsValid = ValueIsInCharacterSubset(cValue, _XCHAR_82CharacterSubset);
                }
            }

            // cpsn-code 8011 - CPID Serial Number = 1*12DIGIT ; 1-12 digits
            else if (bIsValid && cKey == "8011")
            {
                // If the GS1 identifier does not have a length of 1-12 digits, return false
                if (cValue.Length is < 1 or > 12)
                {
                    bIsValid = false;
                    cErrorMessage = CodeLang.ErrorGS1ValueLengthInvalid_Text;
                }

                // The characters of the GS1 identifier must be numeric
                else if (bIsValid)
                {
                    bIsValid = ValueIsDigit(cValue);
                }
            }

            // glnx-code 254 - GLN extension = 1*20XCHAR ; 1-20 characters from 82-character subset
            else if (bIsValid && cKey == "254")
            {
                // If the GS1 identifier does not have a length of 1-20 characters, return false
                if (cValue.Length is < 1 or > 20)
                {
                    bIsValid = false;
                    cErrorMessage = CodeLang.ErrorGS1ValueLengthInvalid_Text;
                }

                // The characters of the GS1 identifier must be from the 82-character subset
                else if (bIsValid)
                {
                    bIsValid = ValueIsInCharacterSubset(cValue, _XCHAR_82CharacterSubset);
                }
            }

            // refno-code 8020 - Payment Reference Number = 1*25XCHAR ; 1-25 characters from 82-character subset
            else if (bIsValid && cKey == "8020")
            {
                // If the GS1 identifier does not have a length of 1-25 characters, return false
                if (cValue.Length is < 1 or > 25)
                {
                    bIsValid = false;
                    cErrorMessage = CodeLang.ErrorGS1ValueLengthInvalid_Text;
                }

                // The characters of the GS1 identifier must be from the 82-character subset
                else if (bIsValid)
                {
                    bIsValid = ValueIsInCharacterSubset(cValue, _XCHAR_82CharacterSubset);
                }
            }

            // srin-code 8019 - Service Relation Instance Number = 1*10DIGIT ; 1-10 digits
            else if (bIsValid && cKey == "8019")
            {
                // If the GS1 identifier does not have a length of 1-10 digits, return false
                if (cValue.Length is < 1 or > 10)
                {
                    bIsValid = false;
                    cErrorMessage = CodeLang.ErrorGS1ValueLengthInvalid_Text;
                }

                // The characters of the GS1 identifier must be numeric
                else if (bIsValid)
                {
                    bIsValid = ValueIsDigit(cValue);
                }
            }

            // tpx-code 235 - third-party controlled serialised extension to GTIN = 1*28XCHAR ; 1-28 characters from 82-character subset
            else if (bIsValid && cKey == "235")
            {
                // If the GS1 identifier does not have a length of 1-28 characters, return false
                if (cValue.Length is < 1 or > 28)
                {
                    bIsValid = false;
                    cErrorMessage = CodeLang.ErrorGS1ValueLengthInvalid_Text;
                }

                // The characters of the GS1 identifier must be from the 82-character subset
                else if (bIsValid)
                {
                    bIsValid = ValueIsInCharacterSubset(cValue, _XCHAR_82CharacterSubset);
                }
            }

            // uic-ext-code 7040 - GS1 UIC with Extension 1 and Importer Index = 1DIGIT 3XCHAR ; 1 digit then 3 characters from 82-character subset
            else if (bIsValid && cKey == "7040")
            {
                // If the GS1 identifier does not have a length of 4 characters, return false
                if (cValue.Length != 4)
                {
                    bIsValid = false;
                    cErrorMessage = CodeLang.ErrorGS1ValueLengthInvalid_Text;
                }

                // The first character of the GS1 identifier must be numeric
                else if (bIsValid)
                {
                    bIsValid = ValueIsDigit(cValue[..1]);
                }

                // The characters of the GS1 identifier must be from the 82-character subset (from character 2 to the end)
                else if (bIsValid && cValue.Length > 1)
                {
                    bIsValid = ValueIsInCharacterSubset(cValue[1..], _XCHAR_82CharacterSubset);
                }
            }

            // Validate the GS1 identifier using the ValidateDataAttribute method, which checks the overall structure and format of the GS1 identifier
            if (bIsValid)
            {
                bIsValid = ValidateDataAttribute(cKey, cValue);
            }

            // If the GS1 identifier is not valid, display an error message and return false to indicate that the validation failed
            if (!bIsValid)
            {
                await Application.Current!.Windows[0].Page!.DisplayAlertAsync(CodeLang.ErrorTitle_Text, $"{CodeLang.GS1ApplicationIdentifier_Text} {cKey} {GetKnownGs1AppIDValue(cKey)} \n\n{cErrorMessage}", CodeLang.ButtonClose_Text);
                _ = editor.Focus();
                return false;
            }

            return bIsValid;
        }

        /// <summary>
        /// Validation method for GS1 identifier data attributes
        /// </summary>
        /// <param name="dataAttribute"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static bool ValidateDataAttribute(string dataAttribute, string value)
        {
            // Variable
            cErrorMessage = CodeLang.ErrorGS1ValueLengthInvalid_Text;

            Debug.WriteLine($"Validating data attribute: {dataAttribute} - Value: {value}");

            // Boolean

            // BOOLEAN
            if (dataAttribute is "4321" or "4322" or "4323")
            {
                if (value.Length != 1 || value != "0" || value != "1" || !ValueIsDigit(value))
                {
                    return false;
                }
            }

            // Characters

            // 1 * 2 XCHAR
            else if (dataAttribute is "7010")
            {
                if (value.Length is < 1 or > 2 || !ValueIsInCharacterSubset(value, _XCHAR_82CharacterSubset))
                {
                    return false;
                }
            }

            // 1 * 3 XCHAR
            else if (dataAttribute is "427" or "7008")
            {
                if (value.Length is < 1 or > 3 || !ValueIsInCharacterSubset(value, _XCHAR_82CharacterSubset))
                {
                    return false;
                }
            }

            //  1 * 4 XCHAR
            else if (dataAttribute is "7041")
            {
                if (value.Length is < 1 or > 4 || !ValueIsInCharacterSubset(value, _XCHAR_82CharacterSubset))
                {
                    return false;
                }
            }

            // 1 * 10 XCHAR
            else if (dataAttribute is "7009" or "7255")
            {
                if (value.Length is < 1 or > 10 || !ValueIsInCharacterSubset(value, _XCHAR_82CharacterSubset))
                {
                    return false;
                }
            }

            // 1 * 12 XCHAR
            else if (dataAttribute is "7005")
            {
                if (value.Length is < 1 or > 12 || !ValueIsInCharacterSubset(value, _XCHAR_82CharacterSubset))
                {
                    return false;
                }
            }

            // 1 * 20 XCHAR
            else if (dataAttribute is "243" or "420" or "710" or "711" or "712" or "713" or "714" or "715" or "716" or "717"
                or "7020" or "7021" or "7022" or "7240" or "8002" or "8012" or "4318")
            {
                if (value.Length is < 1 or > 20 || !ValueIsInCharacterSubset(value, _XCHAR_82CharacterSubset))
                {
                    return false;
                }
            }

            // 1 * 25 XCHAR
            else if (dataAttribute is "7242")
            {
                if (value.Length is < 1 or > 25 || !ValueIsInCharacterSubset(value, _XCHAR_82CharacterSubset))
                {
                    return false;
                }
            }

            // 1 * 30 XCHAR
            else if (dataAttribute is "90" or "240" or "241" or "250" or "251"
                or "400" or "403" or "7002" or "7023" or "4308" or "4319")
            {
                if (value.Length is < 1 or > 30 || !ValueIsInCharacterSubset(value, _XCHAR_82CharacterSubset))
                {
                    return false;
                }
            }

            // 1 * 34 XCHAR
            //else if (dataAttribute is "8007")
            //{
            //    if (value.Length is < 1 or > 34 || !ValueIsInCharacterSubset(value, _XCHAR_82CharacterSubset))
            //    {
            //        return false;
            //    }
            //}

            // 1 * 34 XCHAR - IBAN (International Bank Account Number)
            else if (dataAttribute is "8007")
            {
                if (value.Length is < 1 or > 34 || !ValueIsInCharacterSubset(value, _IBAN_CharacterSet) || !ClassPayloadValidation.IsValidIban(null!, value))
                {
                    cErrorMessage = CodeLang.ErrorIbanInvalid_Text;
                    return false;
                }
            }

            // 1 * 35 XCHAR
            else if (dataAttribute is "4300" or "4301" or "4310" or "4311" or "4320")
            {
                if (value.Length is < 1 or > 35 || !ValueIsInCharacterSubset(value, _XCHAR_82CharacterSubset))
                {
                    return false;
                }
            }

            // 1 * 40 XCHAR
            else if (dataAttribute is "7253" or "7254" or "7259")
            {
                if (value.Length is < 1 or > 40 || !ValueIsInCharacterSubset(value, _XCHAR_82CharacterSubset))
                {
                    return false;
                }
            }

            // 1 * 50 XCHAR
            else if (dataAttribute is "8009")
            {
                if (value.Length is < 1 or > 50 || !ValueIsInCharacterSubset(value, _XCHAR_82CharacterSubset))
                {
                    return false;
                }
            }

            // 1 * 70 XCHAR
            else if (dataAttribute is "7257" or "8110" or "8112" or "4302" or "4303" or "4304" or "4305" or "4306"
                or "4312" or "4313" or "4314" or "4315" or "4316")
            {
                if (value.Length is < 1 or > 70 || !ValueIsInCharacterSubset(value, _XCHAR_82CharacterSubset))
                {
                    return false;
                }
            }

            // 1 * 90 XCHAR
            else if (dataAttribute is "7256" or "91" or "92" or "93" or "94" or "95" or "96" or "97" or "98" or "99")
            {
                if (value.Length is < 1 or > 90 || !ValueIsInCharacterSubset(value, _XCHAR_82CharacterSubset))
                {
                    return false;
                }
            }

            // 1 * 90 ZCHAR
            else if (dataAttribute is "8030")
            {
                if (value.Length is < 1 or > 90 || !ValueIsInCharacterSubset(value, _ZCHAR_64CharacterSubset))
                {
                    return false;
                }
            }

            // 2 XCHAR
            else if (dataAttribute is "4307" or "4317")
            {
                if (value.Length != 2 || !ValueIsInCharacterSubset(value, _XCHAR_82CharacterSubset))
                {
                    return false;
                }
            }

            // 2 XCHAR 1 * 28 XCHAR
            else if (dataAttribute is "7230" or "7231" or "7232" or "7233" or "7234" or "7235" or "7236" or "7237" or "7238" or "7239")
            {
                if (value.Length is < 3 or > 30 || !ValueIsInCharacterSubset(value, _XCHAR_82CharacterSubset))
                {
                    return false;
                }
            }

            // Digits

            // 1 * 4 DIGIT
            else if (dataAttribute is "7004")
            {
                if (value.Length is < 1 or > 4 || !ValueIsDigit(value))
                {
                    return false;
                }
            }

            // 1 * 6 DIGIT
            else if (dataAttribute is "242")
            {
                if (value.Length is < 1 or > 6 || !ValueIsDigit(value))
                {
                    return false;
                }
            }

            // 1 * 8 DIGIT
            else if (dataAttribute is "30" or "37")
            {
                if (value.Length is < 1 or > 8 || !ValueIsDigit(value))
                {
                    return false;
                }
            }

            // 1 * 15 DIGIT
            else if (dataAttribute is "3900" or "3901" or "3902" or "3903" or "3904" or "3905" or "3906" or "3907" or "3908" or "3909"
                or "3920" or "3921" or "3922" or "3923" or "3924" or "3925" or "3926" or "3927" or "3928" or "3929")
            {
                if (value.Length is < 1 or > 15 || !ValueIsDigit(value))
                {
                    return false;
                }
            }

            // 1 DIGIT
            else if (dataAttribute is "7252")
            {
                if (value.Length != 1 || !ValueIsDigit(value))
                {
                    return false;
                }
            }

            // 1 DIGIT "/" 1 DIGIT
            else if (dataAttribute is "7258")
            {
                if (value.Length != 3 || !ValueIsDigit(value[..1]) || value[1] != '/' || !ValueIsDigit(value[2..]))
                {
                    return false;
                }
            }

            // 2 DIGIT
            else if (dataAttribute is "20" or "7241")
            {
                if (value.Length != 2 || !ValueIsDigit(value))
                {
                    return false;
                }
            }

            // 3 DIGIT
            else if (dataAttribute is "422" or "424" or "426")
            {
                if (value.Length != 3 || !ValueIsDigit(value))
                {
                    return false;
                }
            }

            // 3 DIGIT 1 * 12 DIGIT
            else if (dataAttribute is "423" or "425")
            {
                if (value.Length is < 4 or > 15 || !ValueIsDigit(value))
                {
                    return false;
                }
            }

            // 3 DIGIT 1 * 15 DIGIT
            else if (dataAttribute is "3910" or "3911" or "3912" or "3913" or "3914" or "3915" or "3916" or "3917" or "3918" or "3919"
                or "3930" or "3931" or "3932" or "3933" or "3934" or "3935" or "3936" or "3937" or "3938" or "3939")
            {
                if (value.Length is < 4 or > 18 || !ValueIsDigit(value))
                {
                    return false;
                }
            }

            // 4 DIGIT
            else if (dataAttribute is "3940" or "3941" or "3942" or "3943" or "8111")
            {
                if (value.Length != 4 || !ValueIsDigit(value))
                {
                    return false;
                }
            }

            // 6 DIGIT ["-"]
            else if (dataAttribute is "4330" or "4331" or "4332" or "4333")
            {
                if (value.Length is < 6 or > 7 || !ValueIsDigit(value[..6]))
                {
                    return false;
                }

                if (value.Length == 7 && !value.EndsWith('-'))
                {
                    return false;
                }
            }

            // 8 DIGIT
            else if (dataAttribute is "7250")
            {
                if (value.Length != 8 || !ValueIsDigit(value))
                {
                    return false;
                }
            }

            // 13 DIGIT
            else if (dataAttribute is "410" or "411" or "412" or "413" or "416" or "7001")
            {
                if (value.Length != 13 || !ValueIsDigit(value))
                {
                    return false;
                }
            }

            // 14 DIGIT
            else if (dataAttribute is "02" or "8001")
            {
                if (value.Length != 14 || !ValueIsDigit(value))
                {
                    return false;
                }
            }

            // 14 DIGIT 2 DIGIT 2 DIGIT
            else if (dataAttribute is "8026")
            {
                if (value.Length != 18 || !ValueIsDigit(value))
                {
                    return false;
                }
            }

            // 15 DIGIT
            else if (dataAttribute is "8040" or "8041")
            {
                if (value.Length != 15 || !ValueIsDigit(value))
                {
                    return false;
                }
            }

            // 18 DIGIT [1 * 2 DIGIT]
            else if (dataAttribute is "8043")
            {
                switch (value.Length)
                {
                    case 18 when ValueIsDigit(value):
                        break;
                    case 19 when ValueIsDigit(value):
                        break;
                    case 20 when ValueIsDigit(value):
                        break;
                    default:
                        return false;
                }
            }

            // 20 DIGIT
            else if (dataAttribute is "4309")
            {
                if (value.Length != 20 || !ValueIsDigit(value))
                {
                    return false;
                }
            }

            // 32 DIGIT
            else if (dataAttribute is "8042")
            {
                if (value.Length != 32 || !ValueIsDigit(value))
                {
                    return false;
                }
            }

            // Mixed/others: digits and characters

            // 3 DIGIT 1 * 9 XCHAR
            else if (dataAttribute is "421")
            {
                if (value.Length is < 4 or > 12 || !ValueIsDigit(value[..3]))
                {
                    return false;
                }
            }

            // 3 DIGIT 1 * 27 XCHAR
            else if (dataAttribute is "7030" or "7031" or "7032" or "7033" or "7034" or "7035" or "7036" or "7037" or "7038" or "7039")
            {
                if (value.Length is < 4 or > 30 || !ValueIsDigit(value[..3]))
                {
                    return false;
                }
            }

            // Dates / times

            // 6 DIGIT (date YYMMDD)
            else if (dataAttribute is "11" or "12" or "13" or "15" or "16" or "17" or "4326" or "7006")
            {
                if (value.Length != 6 || !ValueIsDigit(value) || !IsDateYYMMDD(value))
                {
                    return false;
                }
            }

            // 6 DIGIT [4 DIGIT] (date/time YYMMDD[hhmm])
            else if (dataAttribute is "7011")
            {
                switch (value.Length)
                {
                    case 6 when ValueIsDigit(value) && IsDateYYMMDD(value):
                        break;
                    case 10 when ValueIsDigit(value) && IsDateYYMMDD(value[..6]) && IsTime(value.Substring(6, 4), "HHmm"):
                        break;
                    default:
                        return false;
                }
            }

            // 6 DIGIT [6 DIGIT] (date YYMMDD[YYMMDD])
            else if (dataAttribute is "7007")
            {
                switch (value.Length)
                {
                    case 6 when ValueIsDigit(value) && IsDateYYMMDD(value):
                        break;
                    case 12 when ValueIsDigit(value) && IsDateYYMMDD(value[..6]) && IsDateYYMMDD(value.Substring(6, 6)):
                        break;
                    default:
                        return false;
                }
            }

            // 8 DIGIT [2 DIGIT] [2 DIGIT] (date/time YYMMDDhh[mm][ss])
            else if (dataAttribute is "8008")
            {
                switch (value.Length)
                {
                    case 8 when ValueIsDigit(value) && IsDateYYMMDD(value[..6]) && IsTime(value.Substring(6, 2), "HH"):
                        break;
                    case 10 when ValueIsDigit(value) && IsDateYYMMDD(value[..6]) && IsTime(value.Substring(6, 4), "HHmm"):
                        break;
                    case 12 when ValueIsDigit(value) && IsDateYYMMDD(value[..6]) && IsTime(value.Substring(6, 6), "HHmmss"):
                        break;
                    default:
                        return false;
                }
            }

            // 10 DIGIT (date/time YYMMDDhhmm)
            else if (dataAttribute is "4324" or "4325" or "7003")
            {
                if (value.Length != 10 || !ValueIsDigit(value) || !IsDateYYMMDD(value[..6]) || !IsTime(value.Substring(6, 4), "HHmm"))
                {
                    return false;
                }
            }

            // 12 DIGIT (date YYMMDDYYMMDD)
            else if (dataAttribute is "7251")
            {
                if (value.Length != 12 || !ValueIsDigit(value) || !IsDateYYMMDD(value[..6]) || !IsDateYYMMDD(value.Substring(6, 6)))
                {
                    return false;
                }
            }

            // Default 6 digits - Except for the following keys, which have specific validation rules and are handled separately above.

            // 6 DIGIT
            else if (dataAttribute is not "01" and not "8006" and not "8013" and not "8010" and not "414" and not "415" and not "417"
                and not "8017" and not "8018" and not "255" and not "00" and not "253" and not "401" and not "402" and not "8003" and not "8004"
                and not "22" and not "10" and not "21" and not "8011" and not "254" and not "8020" and not "8019" and not "235" and not "7040")
            {
                if (value.Length != 6 || !ValueIsDigit(value))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Check if the provided key is a known GS1 Application Identifier (AI) by searching in the predefined list of known AIs
        /// </summary>
        /// <param name="cAppId"></param>
        /// <returns></returns>
        private static bool IsKnownGs1AppID(string cAppId)
        {
            return _knownGs1AppIDs.ContainsKey(cAppId);
        }

        /// <summary>
        /// Get the value of the provided application identifier (AI) and return the value from the predefined list of known
        /// GS1 Application Identifiers (AIs). If the AI is not found, return an empty string.
        /// </summary>
        /// <param name="cAppId"></param>
        /// <returns></returns>
        public static string GetKnownGs1AppIDValue(string cAppId)
        {
            return _knownGs1AppIDs.TryGetValue(cAppId, out string? value) ? value : string.Empty;
        }

        /// <summary>
        /// Get the application identifier (AI) of the GS1 identifier
        /// </summary>
        /// <param name="cAppId"></param>
        /// <param name="cGs1Identifier"></param>
        /// <returns></returns>
        private static string GetAppID(string cAppId, string cGs1Identifier)
        {
            if (cGs1Identifier.Contains($"/{cAppId}/"))
            {
                return $"/{cAppId}/";
            }
            else if (cGs1Identifier.Contains($"?{cAppId}="))
            {
                return $"?{cAppId}=";
            }
            else if (cGs1Identifier.Contains($"&{cAppId}="))
            {
                return $"&{cAppId}=";
            }

            return string.Empty;
        }

        /// <summary>
        /// Get the value of the Gs1 identifier following a certain application identifier (AI). The value is extracted from the
        /// GS1 identifier string based on the position of the AI and the next delimiter (forward slash, question mark, or ampersand) or the end of the string.
        /// </summary>
        /// <param name="cAppId"></param>
        /// <param name="cGs1Identifier"></param>
        /// <returns></returns>
        private static string GetValue(string cAppId, string cGs1Identifier)
        {
            int index = cGs1Identifier.IndexOf(cAppId);

            if (index >= 0)
            {
                // Find the start position of the value after the key
                int nStartPosition = index + cAppId.Length;

                // Find the end position of the value, which is either the next forward slash or question mark or ampersand or the end of the string
                int nEndPosition = cGs1Identifier.IndexOfAny(['/', '?', '&'], nStartPosition);

                // If no forward slash or question mark or ampersand is found, set the end position to the end of the string
                if (nEndPosition == -1)
                {
                    nEndPosition = cGs1Identifier.Length;
                }

                Debug.WriteLine(string.Concat("cGs1IdentifierValue: ", cGs1Identifier.AsSpan(nStartPosition, nEndPosition - nStartPosition)));

                return cGs1Identifier[nStartPosition..nEndPosition];
            }

            // If no forward slash is found, return an empty string
            return string.Empty;
        }

        /// <summary>
        /// Finds the start index of a GS1 Application Identifier (AI) segment within a GS1 identifier string
        /// </summary>
        /// <param name="cAppId"></param>
        /// <param name="cGs1Identifier"></param>
        /// <returns></returns>
        private static int GetAppIDStartPosition(string cAppId, string cGs1Identifier)
        {
            int index = cGs1Identifier.IndexOf(cAppId);
            if (index >= 0)
            {
                // Find the start position of the key
                return index;
            }

            // If no forward slash is found, return -1
            return -1;
        }

        /// <summary>
        /// Determines whether a string consists only of digit characters - DIGIT
        /// </summary>
        /// <remarks>An empty string returns <see langword="true"/>. This method expects a non-null value.</remarks>
        /// <param name="value">The string to evaluate.</param>
        /// <returns><see langword="true"/> if all characters in <paramref name="value"/> are digits; otherwise, <see langword="false"/>.</returns>
        private static bool ValueIsDigit(string value)
        {
            foreach (char c in value)
            {
                if (!char.IsDigit(c))
                {
                    cErrorMessage = CodeLang.ErrorGS1NumberOnlyDigits_Text;
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Check if all characters are in the allowed character subset
        /// </summary>
        /// <param name="value"></param>
        /// <param name="characterSubset"></param>
        /// <returns></returns>
        private static bool ValueIsInCharacterSubset(string value, string characterSubset)
        {
            bool bIsValid;
            
            // If the input string is null or empty, it is considered invalid
            if (string.IsNullOrEmpty(value))
            {
                bIsValid = false;
            }

            // Check if all characters in the value are in the allowed set
            else
            {
                bIsValid = value.All(c => characterSubset.Contains(c));
            }

            if (!bIsValid)
            {
                cErrorMessage = CodeLang.ErrorGS1ValueContainsCharacterInvalid_Text;
            }

            return bIsValid;
        }

        /// <summary>
        /// Validates whether a string represents a date in the `yyMMdd` format using the invariant culture.
        /// </summary>
        /// <remarks>Parsing uses `DateTime.TryParseExact` with `DateTimeStyles.None`. When validation
        /// fails, `cErrorMessage` is set to `CodeLang.ErrorGS1DateInvalid_Text`.</remarks>
        /// <param name="input">Text to validate as a date in `yyMMdd` format.</param>
        /// <returns>`true` if `input` is a valid date in `yyMMdd` format; otherwise, `false`.</returns>
        private static bool IsDateYYMMDD(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return false;
            }

            // Try parsing with exact format "yyMMdd"
            bool bIsValid = DateTime.TryParseExact(
                input,
                "yyMMdd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out _ // discard parsed DateTime
            );

            if (!bIsValid)
            {
                cErrorMessage = CodeLang.ErrorGS1DateInvalid_Text;
            }

            return bIsValid;
        }

        /// <summary>
        /// Validate if a string is a time in the specified format
        /// </summary>
        /// <param name="input"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        private static bool IsTime(string input, string format)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return false;
            }

            // Try parsing with exact format "HHmm"
            bool bIsValid = DateTime.TryParseExact(
                input,
                format,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out _ // discard parsed DateTime
            );

            if (!bIsValid)
            {
                cErrorMessage = CodeLang.ErrorGS1TimeInvalid_Text;
            }

            return bIsValid;
        }

        /// <summary>
        /// Calculate the gtin check digit (Modulo10) and validate the value against the check digit
        /// </summary>
        /// <param name="value">The GTIN value to validate.</param>
        /// <param name="length">The expected length of the GTIN value.</param>
        /// <returns>`true` if the GTIN value is valid; otherwise, `false`.</returns>
        private static bool ValidateValueCheckDigitModulo10(string value, int length)
        {
            // Validate input: the value must be a non-empty string containing only digits
            if (string.IsNullOrWhiteSpace(value) || !value.All(char.IsDigit))
            {
                cErrorMessage = CodeLang.ErrorGS1NumberOnlyDigits_Text;
                return false;
            }

            // Store the original value for later comparison
            string valueOriginal = value;

            // If the value is less than length digits return false
            if (value.Length != length)
            {
                cErrorMessage = CodeLang.ErrorGS1ValueLengthInvalid_Text;
                return false;
            }

            // If the value is already length digits, we assume the last digit is the check digit and remove it for calculation
            else
            {
                value = value[..^1];
            }

            int sum = 0;
            int weight = 3;

            // Starting from the right, multiply digits by alternating weights of 3 and 1
            for (int i = value.Length - 1; i >= 0; i--)
            {
                sum += (value[i] - '0') * weight;
                weight = weight == 3 ? 1 : 3;
            }

            int checkDigit = (10 - (sum % 10)) % 10;

            // If the last digit of the valueOriginal is equal to the calculated check digit, the value is valid
            if (valueOriginal[^1] != checkDigit.ToString()[0])
            {
                cErrorMessage = CodeLang.ErrorGS1CheckDigitInvalid_Text;
                return false;
            }

            return true;
        }
    }
}

/*
Note that the following data attributes are intentionally omitted: 
■ Extended Packaging URL [ AI (8200) ] is not expected to be used within a GS1 Digital Link URI, 
as it is an alternate approach to enable extended packaging applications pre-dating the 
development of the GS1 Digital Link Standard: URI Syntax.  
■ Identification of a Made-to-order (MtO) trade item (GTIN) [AI (03)] and Highly Individualised 
Device Registration Identifier (HIDRI) [AI (8014)] are defined for the Master Unique Device 
Identifier - Device Identifier (M-UDI-DI) restricted application, and as such are not permitted for 
use in a GS1 Digital Link URI syntax.
*/