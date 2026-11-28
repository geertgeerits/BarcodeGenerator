using System.Resources;

namespace BarcodeGenerator
{
    internal partial class ClassPayloadGS1Validation
    {
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
        /// Splits the GS1 identifier into key-value pairs, where the key is surrounded by and left parenthesis and
        /// a right parenthesis, and adds them to the dictionary dicGs1 - example: (01)09520123456788(21)ABC123
        /// </summary>
        /// <param name="gs1Identifier">The GS1 identifier to split into key-value pairs.</param>
        private static void SplitKeyValuePairsParenthesis(string gs1Identifier)
        {
            ClassLoadDataFromFile.dicGs1.Clear();
            
            if (string.IsNullOrWhiteSpace(gs1Identifier))
            {
                return;
            }
            
            int index = 0;
            while (index < gs1Identifier.Length)
            {
                if (gs1Identifier[index] == '(')
                {
                    int closingParenIndex = gs1Identifier.IndexOf(')', index);
                    if (closingParenIndex > index)
                    {
                        string key = gs1Identifier[(index + 1)..closingParenIndex];
                        index = closingParenIndex + 1;
                        
                        // Find the next opening parenthesis or the end of the string
                        int nextOpeningParenIndex = gs1Identifier.IndexOf('(', index);
                        string value = nextOpeningParenIndex > index ? gs1Identifier[index..nextOpeningParenIndex] : gs1Identifier[index..];
                        ClassLoadDataFromFile.dicGs1[key] = value;
                        index += value.Length;
                    }
                    else
                    {
                        break; // Malformed input, exit the loop
                    }
                }
                else
                {
                    index++;
                }
            }
        }

        /// <summary>
        /// Determines whether the specified string is a valid GS1 identifier
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="editor"></param>
        /// <returns></returns>
        public static async Task<bool> IsValidGS1IdentifierAsync(string cPayloadTypeURL, Editor editor)
        {
            bool bIsValid = true;

            // Split the GS1 identifier into key-value pairs and add them to the dictionary
            SplitKeyValuePairsParenthesis(editor.Text);

            Debug.WriteLine($"Length dictionary _knownGs1AppIDs: {ClassLoadDataFromFile._knownGs1AppIDs.Count}");

            // Validate each key-value pair in the dictionary
            foreach (KeyValuePair<string, string> kvp in ClassLoadDataFromFile.dicGs1)
            {
                Debug.WriteLine($"AI: {kvp.Key}, Value: {kvp.Value}, Value length: {kvp.Value.Length}");
                bIsValid &= await IsValidGS1IdentifierPart2Async(editor, kvp.Key, kvp.Value);

                // If the GS1 identifier is not valid, display an error message and return false to indicate that the validation failed
                if (!bIsValid)
                {
                    await Application.Current!.Windows[0].Page!.DisplayAlertAsync(
                        CodeLang.ErrorTitle_Text,
                        $"{CodeLang.GS1ApplicationIdentifier_Text} {kvp.Key} - {GetKnownGs1AppIDValue(kvp.Key)} - {GetKnownGs1ValuePattern(kvp.Key)}\n\n{GetKnownGs1AppIDValueFromLangResource(kvp.Key)}.\n\n{cErrorMessage}",
                        CodeLang.ButtonClose_Text);

                    _ = editor.Focus();
                    return false;
                }
            }

            // Split the GS1 identifier into key-value pairs and add them to the dictionary
            // Needs to be done again because the GS1 identifier may have been modified during validation (e.g., padding with leading zeroes)
            SplitKeyValuePairsParenthesis(editor.Text);

            // Order the GS1 identifier to a valid GS1 Digital Link
            string gs1DigitalLink = ClassBarcodeFormat.OrderGS1DigitalLink();

            // Format the GS1 identifier to a valid GS1 Digital Link
            gs1DigitalLink = ClassBarcodeFormat.FormatGS1DigitalLink(cPayloadTypeURL, gs1DigitalLink);
            
            editor.Text = gs1DigitalLink;
            return bIsValid;
        }

        /// <summary>
        /// Determines whether the specified string is a valid GS1 identifier
        /// </summary>
        /// <param name="editor">The Editor containing the GS1 identifier string to validate.</param>
        /// <param name="cKey">The GS1 key to validate.</param>
        /// <param name="cValue">The GS1 value to validate.</param>
        /// <returns>true if the specified string is a valid GS1 identifier; otherwise, false.</returns>
        private static async Task<bool> IsValidGS1IdentifierPart2Async(Editor editor, string cKey, string cValue)
        {
            // GS1 Digital Link Test Suite: https://ref.gs1.org/test-suites/resolver/
            // GS1 Digital Link Validator: https://qrbolt.com/en/validate-gs1
            // FeatherQR Playground: https://guitarrapc.github.io/FeatherQR/

            // Variables
            string gs1Identifier = editor.Text;
            cErrorMessage = string.Empty;

            // Check if the key in the GS1 identifier exists in the array of valid keys
            if (!IsKnownGs1AppID(cKey))
            {
                cErrorMessage = CodeLang.ErrorGS1AppIDInvalid_Text;
                return false;
            }

            // Primary identification key formats (x 16)

            // gtin-code 01 - Global Trade Item Number (GTIN) = 14DIGIT ; GTIN-8, GTIN-12 and GTIN-13 SHALL be expressed as 14 digits, with leading zeroes serving as filler digits
            if (cKey == "01")
            {
                // Pad the GTIN to 14 digits and replace only the value belonging to AI 01.
                if (cValue.Length < 14)
                {
                    int valueStart = GetValueStart(gs1Identifier, cKey, cValue);
                    int originalLength = cValue.Length;
                    cValue = cValue.PadLeft(14, '0');
                    gs1Identifier = gs1Identifier.Remove(valueStart, originalLength).Insert(valueStart, cValue);
                    editor.Text = gs1Identifier;
                }

                if (cValue.Length > 14)
                {
                    cErrorMessage = $"{string.Format(CodeLang.ErrorLengthDigits_Text, 14)}";
                    return false;
                }

                // The GTIN must have a valid check digit - (01)09520123456788 is valid, (01)09520123456789 is invalid
                if (!ValidateValueCheckDigitModulo10(cValue, 14))
                {
                    return false;
                }
            }

            // itip-code 8006 - International Ticketing Information Protocol (ITIP) = 14DIGIT 2DIGIT 2DIGIT ; 14 digits then 2 digits then 2 digits
            else if (cKey == "8006")
            {
                // If the GS1 identifier length is less than 18 characters, add zeroes to the left to make it 18 characters
                // and replace the original value with the new value in the editor
                if (cValue.Length < 18)
                {
                    int valueStart = GetValueStart(gs1Identifier, cKey, cValue);
                    int originalLength = cValue.Length;
                    cValue = cValue.PadLeft(18, '0');
                    gs1Identifier = gs1Identifier.Remove(valueStart, originalLength).Insert(valueStart, cValue);
                    editor.Text = gs1Identifier;
                }

                if (cValue.Length > 18)
                {
                    cErrorMessage = $"{string.Format(CodeLang.ErrorLengthDigits_Text, 18)}";
                    return false;
                }

                // The ITIP must have a valid check digit - (8006)095201234567881122 is valid, (8006)095201234567891122 is invalid
                if (!ValidateValueCheckDigitModulo10(cValue[..14], 14))
                {
                    return false;
                }
            }

            // gmn-code 8013 - Global Model Number (GMN) = 1*25XCHAR ; 1-25 characters from 82-character subset
            else if (cKey == "8013")
            {
                // If the GS1 identifier does not have a length of 1-25 characters, return false
                if (cValue.Length is < 1 or > 25)
                {
                    cErrorMessage = $"{string.Format(CodeLang.ErrorLengthBetweenCharacters_Text, 1, 25)}";
                    return false;
                }

                // The characters of the GS1 identifier must be from the 82-character subset
                else if (!ValueIsInCharacterSubset(cValue, _XCHAR_82CharacterSubset))
                {
                    return false;
                }
            }

            // cpid-code 8010 - Component / Part Identifier (CPID) = 1*30YCHAR ; 1-30 characters from 39-character subset
            else if (cKey == "8010")
            {
                // If the GS1 identifier does not have a length of 1-30 characters, return false
                if (cValue.Length is < 1 or > 30)
                {
                    cErrorMessage = $"{string.Format(CodeLang.ErrorLengthBetweenCharacters_Text, 1, 30)}";
                    return false;
                }

                // The characters of the GS1 identifier must be from the 39-character subset
                else if (!ValueIsInCharacterSubset(cValue, _YCHAR_39CharacterSubset))
                {
                    return false;
                }
            }

            // gln-code 414 - Physical / Global Location Number (GLN) = 13DIGIT ; exactly 13 digits
            else if (cKey == "414")
            {
                // If the GS1 identifier length is less than 13 characters, add zeroes to the left to make it 13 characters
                // and replace the original value with the new value in the editor
                if (cValue.Length < 13)
                {
                    int valueStart = GetValueStart(gs1Identifier, cKey, cValue);
                    int originalLength = cValue.Length;
                    cValue = cValue.PadLeft(13, '0');
                    gs1Identifier = gs1Identifier.Remove(valueStart, originalLength).Insert(valueStart, cValue);
                    editor.Text = gs1Identifier;
                }

                if (cValue.Length > 13)
                {
                    cErrorMessage = $"{string.Format(CodeLang.ErrorLengthDigits_Text, 13)}";
                    return false;
                }

                // The GLN must have a valid check digit - (414)9520123456788 is valid, (414)9520123456780 is invalid
                if (!ValidateValueCheckDigitModulo10(cValue, 13))
                {
                    return false;
                }
            }

            // payTo-code 415 - GLN of invoicing party = 13DIGIT ; exactly 13 digits
            else if (cKey == "415")
            {
                // If the GS1 identifier does not have a length of 13 characters, return false
                if (cValue.Length != 13)
                {
                    cErrorMessage = $"{string.Format(CodeLang.ErrorLengthDigits_Text, 13)}";
                    return false;
                }

                // The characters of the GS1 identifier must be numeric
                else if (!ValueIsDigit(cValue))
                {
                    return false;
                }
            }

            // partyGln-code 417 - Party GLN = 13DIGIT ; exactly 13 digits
            else if (cKey == "417")
            {
                // If the GS1 identifier does not have a length of 13 characters, return false
                if (cValue.Length != 13)
                {
                    cErrorMessage = $"{string.Format(CodeLang.ErrorLengthDigits_Text, 13)}";
                    return false;
                }

                // The characters of the GS1 identifier must be numeric
                else if (!ValueIsDigit(cValue))
                {
                    return false;
                }
            }

            // gsrnp-code 8017 -  GSRN of the Provider = 18DIGIT ; exactly 18 digits
            else if (cKey == "8017")
            {
                // If the GS1 identifier does not have a length of 18 characters, return false
                if (cValue.Length != 18)
                {
                    cErrorMessage = $"{string.Format(CodeLang.ErrorLengthDigits_Text, 18)}";
                    return false;
                }

                // The characters of the GS1 identifier must be numeric
                else if (!ValueIsDigit(cValue))
                {
                    return false;
                }
            }

            // gsrn-code 8018 - GSRN of the Recipient = 18DIGIT ; exactly 18 digits
            else if (cKey == "8018")
            {
                // If the GS1 identifier length is less than 18 characters, add zeroes to the left to make it 18 characters
                // and replace the original value with the new value in the editor
                if (cValue.Length < 18)
                {
                    int valueStart = GetValueStart(gs1Identifier, cKey, cValue);
                    int originalLength = cValue.Length;
                    cValue = cValue.PadLeft(18, '0');
                    gs1Identifier = gs1Identifier.Remove(valueStart, originalLength).Insert(valueStart, cValue);
                    editor.Text = gs1Identifier;
                }

                // If the GS1 identifier does not have a length of 18 digits, return false
                if (cValue.Length != 18)
                {
                    cErrorMessage = $"{string.Format(CodeLang.ErrorLengthDigits_Text, 18)}";
                    return false;
                }

                // The GSRN must have a valid check digit  - (8018)195201234567891232 is valid, (8018)195201234567891230 is invalid
                if (!ValidateValueCheckDigitModulo10(cValue, 18))
                {
                    return false;
                }
            }

            // gcn-code 255 - Global Coupon Number (GCN) = 13DIGIT [1*12DIGIT] ; 13 digits then optional 1 - 12 digits
            else if (cKey == "255")
            {
                // If the GCN identifier length is less than 13 digits, add zeroes to the left to make it 13 digits
                // and replace the original value with the new value in the editor
                if (cValue.Length < 13)
                {
                    int valueStart = GetValueStart(gs1Identifier, cKey, cValue);
                    int originalLength = cValue.Length;
                    cValue = cValue.PadLeft(13, '0');
                    gs1Identifier = gs1Identifier.Remove(valueStart, originalLength).Insert(valueStart, cValue);
                    editor.Text = gs1Identifier;
                }

                // If the GS1 identifier does not have a length of 13-25 digits, return false
                if (cValue.Length > 25)
                {
                    cErrorMessage = $"{string.Format(CodeLang.ErrorLengthBetweenDigits_Text, 13, 25)}";
                    return false;
                }

                // The GCN must have a valid check digit - (255)0952012345678 is valid, (255)0952012345679 is invalid
                if (!ValidateValueCheckDigitModulo10(cValue[..13], 13))
                {
                    return false;
                }

                // The characters of the GS1 identifier must be numeric
                else if (!ValueIsDigit(cValue))
                {
                    return false;
                }
            }

            // sscc-code 00 - Serial Shipping Container Code (SSCC) = 18DIGIT ; exactly 18 digits 
            else if (cKey == "00")
            {
                // If the GS1 identifier length is less than 18 characters, add zeroes to the left to make it 18 characters
                // and replace the original value with the new value in the editor
                if (cValue.Length < 18)
                {
                    int valueStart = GetValueStart(gs1Identifier, cKey, cValue);
                    int originalLength = cValue.Length;
                    cValue = cValue.PadLeft(18, '0');
                    gs1Identifier = gs1Identifier.Remove(valueStart, originalLength).Insert(valueStart, cValue);
                    editor.Text = gs1Identifier;
                }

                if (cValue.Length > 18)
                {
                    cErrorMessage = $"{string.Format(CodeLang.ErrorLengthDigits_Text, 18)}";
                    return false;
                }

                // The GTIN must have a valid check digit - (00)195201234567891232 is valid, (00)195201234567891230 is invalid
                if (!ValidateValueCheckDigitModulo10(cValue, 18))
                {
                    return false;
                }
            }

            // gdti-code 253 - Global Document Type Identifier (GDTI) = 13DIGIT [1*17XCHAR] ; 13 digits then optional 1 - 17 characters ; from the 82-character subset
            else if (cKey == "253")
            {
                // If the GS1 identifier does not have a length of 13-30 characters, return false
                if (cValue.Length is < 13 or > 30)
                {
                    cErrorMessage = $"{string.Format(CodeLang.ErrorLengthBetweenCharacters_Text, 13, 30)}";
                    return false;
                }

                // The first 13characters of the GS1 identifier must be numeric
                else if (!ValueIsDigit(cValue[..13]))
                {
                    return false;
                }

                // The characters of the GS1 identifier must be from the 82-character subset (from character 14 to the end)
                else if (cValue.Length > 13)
                {
                    if (!ValueIsInCharacterSubset(cValue[13..], _XCHAR_82CharacterSubset))
                    {
                        return false;
                    }
                }
            }

            // ginc-code 401 - Global Identification Number for Consignment (GINC) = 1*30XCHAR ; 1 - 30 characters from the 82-character subset
            else if (cKey == "401")
            {
                // If the GS1 identifier does not have a length of 1-30 characters, return false
                if (cValue.Length is < 1 or > 30)
                {
                    cErrorMessage = $"{string.Format(CodeLang.ErrorLengthBetweenCharacters_Text, 1, 30)}";
                    return false;
                }

                // The characters of the GS1 identifier must be from the 82-character subset
                else if (!ValueIsInCharacterSubset(cValue, _XCHAR_82CharacterSubset))
                {
                    return false;
                }
            }

            // gsin-code 402 - Global Shipment Identification Number (GSIN) = 17DIGIT ; exactly 17 digits
            else if (cKey == "402")
            {
                // If the GS1 identifier length is less than 14 characters, add zeroes to the left to make it 14 characters
                // and replace the original value with the new value in the editor
                if (cValue.Length < 17)
                {
                    int valueStart = GetValueStart(gs1Identifier, cKey, cValue);
                    int originalLength = cValue.Length;
                    cValue = cValue.PadLeft(17, '0');
                    gs1Identifier = gs1Identifier.Remove(valueStart, originalLength).Insert(valueStart, cValue);
                    editor.Text = gs1Identifier;
                }

                // If the GS1 identifier does not have a length of 17 digits, return false
                if (cValue.Length != 17)
                {
                    cErrorMessage = $"{string.Format(CodeLang.ErrorLengthDigits_Text, 17)}";
                    return false;
                }

                // The GSIN must have a valid check digit - (402)9520123456788 is valid, (402)09520123456789000 is invalid
                if (!ValidateValueCheckDigitModulo10(cValue, 17))
                {
                    return false;
                }
            }

            /* Why the confusion about "13 or 14 digits" for AI 8003?
            The core asset identity (Company Prefix +Asset Type + Check Digit) technically totals 13 digits of unique payload data.
            However, GS1 standard data structures always require a mandatory leading filler zero in the leftmost position when
            encoding AI 8003. This zero pads the field to an absolute total of 14 digits, making it structurally consistent
            for processing systems and barcode symbologies. */

            // grai-code 8003 - Global Returnable Asset Identifier (GRAI) = 13DIGIT [1*16XCHAR] ; 13 digits then optional 1 - 16 characters ; from the 82-character subset of ISO / IEC 646
            else if (cKey == "8003")
            {
                // If the GS1 identifier length is less than 13 digits, add zeroes to the left to make it 13 digits
                // and replace the original value with the new value in the editor
                int valueStart;
                int originalLength;

                if (cValue.Length < 13)
                {
                    valueStart = GetValueStart(gs1Identifier, cKey, cValue);
                    originalLength = cValue.Length;
                    cValue = cValue.PadLeft(13, '0');
                    gs1Identifier = gs1Identifier.Remove(valueStart, originalLength).Insert(valueStart, cValue);
                    editor.Text = gs1Identifier;
                }

                // If the GS1 identifier does not have a length of 13-29 characters, return false
                if (cValue.Length > 29)
                {
                    cErrorMessage = $"{string.Format(CodeLang.ErrorLengthBetweenCharacters_Text, 13, 29)}";
                    return false;
                }

                // The characters of the GS1 identifier must be from the 82-character subset (from character 14 to the end)
                else if (cValue.Length > 13)
                {
                    if (!ValueIsInCharacterSubset(cValue[13..], _XCHAR_82CharacterSubset))
                    {
                        return false;
                    }
                }

                // The GRAI must have a valid check digit - (8003)9520123456788 is valid, (8003)9520123456789 is invalid
                if (!ValidateValueCheckDigitModulo10(cValue[..13], 13))
                {
                    return false;
                }

                // ABNF syntax for GRAI [AI (8003)] corrected to align with GS1 General Specifications,
                // use of "0" as pad character, not as part of GRAI value
                valueStart = GetValueStart(gs1Identifier, cKey, cValue);
                originalLength = cValue.Length;
                cValue = $"0{cValue}";
                gs1Identifier = gs1Identifier.Remove(valueStart, originalLength).Insert(valueStart, cValue);
                editor.Text = gs1Identifier;
            }

            // giai-code 8004 - Global Individual Asset Identifier (GIAI) = 1*30XCHAR  ; 1-30 characters from 82-character subset
            else if (cKey == "8004")
            {
                // If the GS1 identifier does not have a length of 1-30 characters, return false
                if (cValue.Length is < 1 or > 30)
                {
                    cErrorMessage = $"{string.Format(CodeLang.ErrorLengthBetweenCharacters_Text, 1, 30)}";
                    return false;
                }

                // The characters of the GS1 identifier must be from the 82-character subset
                if (!ValueIsInCharacterSubset(cValue, _XCHAR_82CharacterSubset))
                {
                    return false;
                }
            }

            // Key qualifier formats ( x 9)

            // cpv-code 22 - Consumer Product Variant (CPV) = 1*20XCHAR ; 1-20 characters from 82-character subset
            else if (cKey == "22")
            {
                // If the GS1 identifier does not have a length of 1-20 characters, return false
                if (cValue.Length is < 1 or > 20)
                {
                    cErrorMessage = $"{string.Format(CodeLang.ErrorLengthBetweenCharacters_Text, 1, 20)}";
                    return false;
                }

                // The characters of the GS1 identifier must be from the 82-character subset
                if (!ValueIsInCharacterSubset(cValue, _XCHAR_82CharacterSubset))
                {
                    return false;
                }
            }

            // lot-code 10 - Batch/Lot identifier = 1*20XCHAR ; 1-20 characters from 82-character subset
            else if (cKey == "10")
            {
                // If the GS1 identifier does not have a length of 1-20 characters, return false
                if (cValue.Length is < 1 or > 20)
                {
                    cErrorMessage = $"{string.Format(CodeLang.ErrorLengthBetweenCharacters_Text, 1, 20)}";
                    return false;
                }

                // The characters of the GS1 identifier must be from the 82-character subset
                if (!ValueIsInCharacterSubset(cValue, _XCHAR_82CharacterSubset))
                {
                    return false;
                }
            }

            // ser-code 21 - GTIN Serial Number = 1*20XCHAR ; 1-20 characters from 82-character subset
            else if (cKey == "21")
            {
                // If the GS1 identifier does not have a length of 1-20 characters, return false
                if (cValue.Length is < 1 or > 20)
                {
                    cErrorMessage = $"{string.Format(CodeLang.ErrorLengthBetweenCharacters_Text, 1, 20)}";
                    return false;
                }

                // The characters of the GS1 identifier must be from the 82-character subset
                if (!ValueIsInCharacterSubset(cValue, _XCHAR_82CharacterSubset))
                {
                    return false;
                }
            }

            // cpsn-code 8011 - CPID Serial Number = 1*12DIGIT ; 1-12 digits
            else if (cKey == "8011")
            {
                // If the GS1 identifier does not have a length of 1-12 digits, return false
                if (cValue.Length is < 1 or > 12)
                {
                    cErrorMessage = $"{string.Format(CodeLang.ErrorLengthBetweenDigits_Text, 1, 12)}";
                    return false;
                }

                // The characters of the GS1 identifier must be numeric
                if (!ValueIsDigit(cValue))
                {
                    return false;
                }
            }

            // glnx-code 254 - GLN extension = 1*20XCHAR ; 1-20 characters from 82-character subset
            else if (cKey == "254")
            {
                // If the GS1 identifier does not have a length of 1-20 characters, return false
                if (cValue.Length is < 1 or > 20)
                {
                    cErrorMessage = $"{string.Format(CodeLang.ErrorLengthBetweenCharacters_Text, 1, 20)}";
                    return false;
                }

                // The characters of the GS1 identifier must be from the 82-character subset
                if (!ValueIsInCharacterSubset(cValue, _XCHAR_82CharacterSubset))
                {
                    return false;
                }
            }

            // refno-code 8020 - Payment Reference Number = 1*25XCHAR ; 1-25 characters from 82-character subset
            else if (cKey == "8020")
            {
                // If the GS1 identifier does not have a length of 1-25 characters, return false
                if (cValue.Length is < 1 or > 25)
                {
                    cErrorMessage = $"{string.Format(CodeLang.ErrorLengthBetweenCharacters_Text, 1, 25)}";
                    return false;
                }

                // The characters of the GS1 identifier must be from the 82-character subset
                if (!ValueIsInCharacterSubset(cValue, _XCHAR_82CharacterSubset))
                {
                    return false;
                }
            }

            // srin-code 8019 - Service Relation Instance Number = 1*10DIGIT ; 1-10 digits
            else if (cKey == "8019")
            {
                // If the GS1 identifier does not have a length of 1-10 digits, return false
                if (cValue.Length is < 1 or > 10)
                {
                    cErrorMessage = $"{string.Format(CodeLang.ErrorLengthBetweenDigits_Text, 1, 10)}";
                    return false;
                }

                // The characters of the GS1 identifier must be numeric
                if (!ValueIsDigit(cValue))
                {
                    return false;
                }
            }

            // tpx-code 235 - third-party controlled serialised extension to GTIN = 1*28XCHAR ; 1-28 characters from 82-character subset
            else if (cKey == "235")
            {
                // If the GS1 identifier does not have a length of 1-28 characters, return false
                if (cValue.Length is < 1 or > 28)
                {
                    cErrorMessage = $"{string.Format(CodeLang.ErrorLengthBetweenCharacters_Text, 1, 28)}";
                    return false;
                }

                // The characters of the GS1 identifier must be from the 82-character subset
                if (!ValueIsInCharacterSubset(cValue, _XCHAR_82CharacterSubset))
                {
                    return false;
                }
            }

            // uic-ext-code 7040 - GS1 UIC with Extension 1 and Importer Index = 1DIGIT 3XCHAR ; 1 digit then 3 characters from 82-character subset
            else if (cKey == "7040")
            {
                // If the GS1 identifier does not have a length of 4 characters, return false
                if (cValue.Length != 4)
                {
                    cErrorMessage = $"{string.Format(CodeLang.ErrorLengthCharacters_Text, 4)}";
                    return false;
                }

                // The first character of the GS1 identifier must be numeric
                if (!ValueIsDigit(cValue[..1]))
                {
                    return false;
                }

                // The characters of the GS1 identifier must be from the 82-character subset (from character 2 to the end)
                else if (cValue.Length > 1)
                {
                    if (!ValueIsInCharacterSubset(cValue[1..], _XCHAR_82CharacterSubset))
                    {
                        return false;
                    }
                }
            }

            // Validate the GS1 identifier using the ValidateDataAttributeAsync method, which checks the overall structure and format of the GS1 identifier
            if (!await ValidateDataAttributeAsync(cKey, cValue))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Validation method for GS1 identifier data attributes
        /// </summary>
        /// <param name="dataAttribute"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static async Task<bool> ValidateDataAttributeAsync(string dataAttribute, string value)
        {
            // Variable
            cErrorMessage = CodeLang.ErrorGS1ValueLengthInvalid_Text;

            Debug.WriteLine($"Validating data attribute: {dataAttribute} - Value: {value}");

            // Boolean

            // BOOLEAN
            if (dataAttribute is "4321" or "4322" or "4323")
            {
                cErrorMessage = $"{string.Format(CodeLang.ErrorLengthDigits_Text, 1)}  -> 0 / 1";

                if (value.Length != 1 || value != "0" || value != "1" || !ValueIsDigit(value))
                {
                    return false;
                }
            }

            // Characters

            // 1 * 2 XCHAR
            else if (dataAttribute is "7010")
            {
                cErrorMessage = $"{string.Format(CodeLang.ErrorLengthBetweenCharacters_Text, 1, 2)}";

                if (value.Length is < 1 or > 2 || !ValueIsInCharacterSubset(value, _XCHAR_82CharacterSubset))
                {
                    return false;
                }
            }

            // 1 * 3 XCHAR
            else if (dataAttribute is "427" or "7008")
            {
                cErrorMessage = $"{string.Format(CodeLang.ErrorLengthBetweenCharacters_Text, 1, 3)}";

                if (value.Length is < 1 or > 3 || !ValueIsInCharacterSubset(value, _XCHAR_82CharacterSubset))
                {
                    return false;
                }
            }

            //  1 * 4 XCHAR
            else if (dataAttribute is "7041")
            {
                cErrorMessage = $"{string.Format(CodeLang.ErrorLengthBetweenCharacters_Text, 1, 4)}";

                if (value.Length is < 1 or > 4 || !ValueIsInCharacterSubset(value, _XCHAR_82CharacterSubset))
                {
                    return false;
                }
            }

            // 1 * 10 XCHAR
            else if (dataAttribute is "7009" or "7255")
            {
                cErrorMessage = $"{string.Format(CodeLang.ErrorLengthBetweenCharacters_Text, 1, 10)}";

                if (value.Length is < 1 or > 10 || !ValueIsInCharacterSubset(value, _XCHAR_82CharacterSubset))
                {
                    return false;
                }
            }

            // 1 * 12 XCHAR
            else if (dataAttribute is "7005")
            {
                cErrorMessage = $"{string.Format(CodeLang.ErrorLengthBetweenCharacters_Text, 1, 12)}";

                if (value.Length is < 1 or > 12 || !ValueIsInCharacterSubset(value, _XCHAR_82CharacterSubset))
                {
                    return false;
                }
            }

            // 1 * 20 XCHAR
            else if (dataAttribute is "243" or "420" or "710" or "711" or "712" or "713" or "714" or "715" or "716" or "717"
                or "7020" or "7021" or "7022" or "7240" or "8002" or "8012" or "4318")
            {
                cErrorMessage = $"{string.Format(CodeLang.ErrorLengthBetweenCharacters_Text, 1, 20)}";

                if (value.Length is < 1 or > 20 || !ValueIsInCharacterSubset(value, _XCHAR_82CharacterSubset))
                {
                    return false;
                }
            }

            // 1 * 25 XCHAR
            else if (dataAttribute is "7242")
            {
                cErrorMessage = $"{string.Format(CodeLang.ErrorLengthBetweenCharacters_Text, 1, 25)}";

                if (value.Length is < 1 or > 25 || !ValueIsInCharacterSubset(value, _XCHAR_82CharacterSubset))
                {
                    return false;
                }
            }

            // 1 * 30 XCHAR
            else if (dataAttribute is "90" or "240" or "241" or "250" or "251"
                or "400" or "403" or "7002" or "7023" or "4308" or "4319")
            {
                cErrorMessage = $"{string.Format(CodeLang.ErrorLengthBetweenCharacters_Text, 1, 30)}";

                if (value.Length is < 1 or > 30 || !ValueIsInCharacterSubset(value, _XCHAR_82CharacterSubset))
                {
                    return false;
                }
            }

            // 1 * 34 XCHAR - IBAN (International Bank Account Number)
            else if (dataAttribute is "8007")
            {
                if (value.Length is < 1 or > 34 || !ValueIsInCharacterSubset(value, _IBAN_CharacterSet) || !ClassPayloadValidation.IsValidIban(null!, value))
                {
                    bool bResult = await Application.Current!.Windows[0].Page!.DisplayAlertAsync(CodeLang.ErrorTitle_Text, $"{CodeLang.ErrorIbanInvalid_Text}\n\n{CodeLang.ErrorIbanInvalidKeep_Text}", CodeLang.Yes_Text, CodeLang.No_Text);

                    if (bResult)
                    {
                        cErrorMessage = $"{string.Format(CodeLang.ErrorLengthBetweenCharacters_Text, 1, 34)}";

                        if (value.Length is < 1 or > 34 || !ValueIsInCharacterSubset(value, _XCHAR_82CharacterSubset))
                        {
                            return false;
                        }
                    }
                    
                    else
                    {
                        return false;
                    }
                }
            }

            // 1 * 35 XCHAR
            else if (dataAttribute is "4300" or "4301" or "4310" or "4311" or "4320")
            {
                cErrorMessage = $"{string.Format(CodeLang.ErrorLengthBetweenCharacters_Text, 1, 35)}";

                if (value.Length is < 1 or > 35 || !ValueIsInCharacterSubset(value, _XCHAR_82CharacterSubset))
                {
                    return false;
                }
            }

            // 1 * 40 XCHAR
            else if (dataAttribute is "7253" or "7254" or "7259")
            {
                cErrorMessage = $"{string.Format(CodeLang.ErrorLengthBetweenCharacters_Text, 1, 40)}";

                if (value.Length is < 1 or > 40 || !ValueIsInCharacterSubset(value, _XCHAR_82CharacterSubset))
                {
                    return false;
                }
            }

            // 1 * 50 XCHAR
            else if (dataAttribute is "8009")
            {
                cErrorMessage = $"{string.Format(CodeLang.ErrorLengthBetweenCharacters_Text, 1, 50)}";

                if (value.Length is < 1 or > 50 || !ValueIsInCharacterSubset(value, _XCHAR_82CharacterSubset))
                {
                    return false;
                }
            }

            // 1 * 70 XCHAR
            else if (dataAttribute is "7257" or "8110" or "8112" or "4302" or "4303" or "4304" or "4305" or "4306"
                or "4312" or "4313" or "4314" or "4315" or "4316")
            {
                cErrorMessage = $"{string.Format(CodeLang.ErrorLengthBetweenCharacters_Text, 1, 70)}";

                if (value.Length is < 1 or > 70 || !ValueIsInCharacterSubset(value, _XCHAR_82CharacterSubset))
                {
                    return false;
                }
            }

            // 1 * 90 XCHAR
            else if (dataAttribute is "7256" or "91" or "92" or "93" or "94" or "95" or "96" or "97" or "98" or "99")
            {
                cErrorMessage = $"{string.Format(CodeLang.ErrorLengthBetweenCharacters_Text, 1, 90)}";

                if (value.Length is < 1 or > 90 || !ValueIsInCharacterSubset(value, _XCHAR_82CharacterSubset))
                {
                    return false;
                }
            }

            // 1 * 90 ZCHAR
            else if (dataAttribute is "8030")
            {
                cErrorMessage = $"{string.Format(CodeLang.ErrorLengthBetweenCharacters_Text, 1, 90)}";

                if (value.Length is < 1 or > 90 || !ValueIsInCharacterSubset(value, _ZCHAR_64CharacterSubset))
                {
                    return false;
                }
            }

            // 2 XCHAR
            else if (dataAttribute is "4307" or "4317")
            {
                cErrorMessage = $"{string.Format(CodeLang.ErrorLengthCharacters_Text, 2)}";

                if (value.Length != 2 || !ValueIsInCharacterSubset(value, _XCHAR_82CharacterSubset))
                {
                    return false;
                }
            }

            // 2 XCHAR 1 * 28 XCHAR
            else if (dataAttribute is "7230" or "7231" or "7232" or "7233" or "7234" or "7235" or "7236" or "7237" or "7238" or "7239")
            {
                cErrorMessage = $"{string.Format(CodeLang.ErrorLengthBetweenCharacters_Text, 3, 30)}";

                if (value.Length is < 3 or > 30 || !ValueIsInCharacterSubset(value, _XCHAR_82CharacterSubset))
                {
                    return false;
                }
            }

            // Digits

            // 1 * 4 DIGIT
            else if (dataAttribute is "7004")
            {
                cErrorMessage = $"{string.Format(CodeLang.ErrorLengthBetweenDigits_Text, 1, 4)}";

                if (value.Length is < 1 or > 4 || !ValueIsDigit(value))
                {
                    return false;
                }
            }

            // 1 * 6 DIGIT
            else if (dataAttribute is "242")
            {
                cErrorMessage = $"{string.Format(CodeLang.ErrorLengthBetweenDigits_Text, 1, 6)}";

                if (value.Length is < 1 or > 6 || !ValueIsDigit(value))
                {
                    return false;
                }
            }

            // 1 * 8 DIGIT
            else if (dataAttribute is "30" or "37")
            {
                cErrorMessage = $"{string.Format(CodeLang.ErrorLengthBetweenDigits_Text, 1, 8)}";

                if (value.Length is < 1 or > 8 || !ValueIsDigit(value))
                {
                    return false;
                }
            }

            // 1 * 15 DIGIT
            else if (dataAttribute is "3900" or "3901" or "3902" or "3903" or "3904" or "3905" or "3906" or "3907" or "3908" or "3909"
                or "3920" or "3921" or "3922" or "3923" or "3924" or "3925" or "3926" or "3927" or "3928" or "3929")
            {
                cErrorMessage = $"{string.Format(CodeLang.ErrorLengthBetweenDigits_Text, 1, 15)}";

                if (value.Length is < 1 or > 15 || !ValueIsDigit(value))
                {
                    return false;
                }
            }

            // 1 DIGIT
            else if (dataAttribute is "7252")
            {
                cErrorMessage = $"{string.Format(CodeLang.ErrorLengthDigits_Text, 1)}";

                if (value.Length != 1 || !ValueIsDigit(value))
                {
                    return false;
                }
            }

            // 1 DIGIT "/" 1 DIGIT
            else if (dataAttribute is "7258")
            {
                cErrorMessage = $"{string.Format(CodeLang.ErrorLengthDigits_Text, 3)}  -> 0/0";

                if (value.Length != 3 || !ValueIsDigit(value[..1]) || value[1] != '/' || !ValueIsDigit(value[2..]))
                {
                    return false;
                }
            }

            // 2 DIGIT
            else if (dataAttribute is "20" or "7241")
            {
                cErrorMessage = $"{string.Format(CodeLang.ErrorLengthDigits_Text, 2)}";

                if (value.Length != 2 || !ValueIsDigit(value))
                {
                    return false;
                }
            }

            // 3 DIGIT
            else if (dataAttribute is "422" or "424" or "426")
            {
                cErrorMessage = $"{string.Format(CodeLang.ErrorLengthDigits_Text, 3)}";

                if (value.Length != 3 || !ValueIsDigit(value))
                {
                    return false;
                }
            }

            // 3 DIGIT 1 * 12 DIGIT
            else if (dataAttribute is "423" or "425")
            {
                cErrorMessage = $"{string.Format(CodeLang.ErrorLengthBetweenDigits_Text, 4, 15)}";

                if (value.Length is < 4 or > 15 || !ValueIsDigit(value))
                {
                    return false;
                }
            }

            // 3 DIGIT 1 * 15 DIGIT - ISO currency code
            else if (dataAttribute is "3910" or "3911" or "3912" or "3913" or "3914" or "3915" or "3916" or "3917" or "3918" or "3919"
                or "3930" or "3931" or "3932" or "3933" or "3934" or "3935" or "3936" or "3937" or "3938" or "3939")
            {
                cErrorMessage = $"{string.Format(CodeLang.ErrorLengthBetweenDigits_Text, 4, 18)}";

                if (value.Length is < 4 or > 18 || !ValueIsDigit(value) || !IsKnownISOCurrencyCode(value[..3]))
                {
                    return false;
                }
            }

            // 4 DIGIT
            else if (dataAttribute is "3940" or "3941" or "3942" or "3943" or "8111")
            {
                cErrorMessage = $"{string.Format(CodeLang.ErrorLengthDigits_Text, 4)}";

                if (value.Length != 4 || !ValueIsDigit(value))
                {
                    return false;
                }
            }

            // 6 DIGIT ["-"]
            else if (dataAttribute is "4330" or "4331" or "4332" or "4333")
            {
                cErrorMessage = $"{string.Format(CodeLang.ErrorLengthBetweenDigits_Text, 6, 7)}  -> 000000 / 000000-";

                if (value.Length is < 6 or > 7 || !ValueIsDigit(value[..6]))
                {
                    return false;
                }

                if (value.Length == 7 && !value.EndsWith('-'))
                {
                    cErrorMessage = $"{string.Format(CodeLang.ErrorLengthDigits_Text, 7)}  -> 000000-";
                    return false;
                }
            }

            // 13 DIGIT
            else if (dataAttribute is "410" or "411" or "412" or "413" or "416" or "7001")
            {
                cErrorMessage = $"{string.Format(CodeLang.ErrorLengthDigits_Text, 13)}";

                if (value.Length != 13 || !ValueIsDigit(value))
                {
                    return false;
                }
            }

            // 14 DIGIT
            else if (dataAttribute is "02" or "8001")
            {
                cErrorMessage = $"{string.Format(CodeLang.ErrorLengthDigits_Text, 14)}";

                if (value.Length != 14 || !ValueIsDigit(value))
                {
                    return false;
                }
            }

            // 14 DIGIT 2 DIGIT 2 DIGIT
            else if (dataAttribute is "8026")
            {
                cErrorMessage = $"{string.Format(CodeLang.ErrorLengthDigits_Text, 18)}";

                if (value.Length != 18 || !ValueIsDigit(value))
                {
                    return false;
                }
            }

            // 15 DIGIT
            else if (dataAttribute is "8040" or "8041")
            {
                cErrorMessage = $"{string.Format(CodeLang.ErrorLengthDigits_Text, 15)}";

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
                        cErrorMessage = $"{string.Format(CodeLang.ErrorLengthBetweenDigits_Text, 18, 20)}";
                        return false;
                }
            }

            // 20 DIGIT
            else if (dataAttribute is "4309")
            {
                cErrorMessage = $"{string.Format(CodeLang.ErrorLengthDigits_Text, 20)}";

                if (value.Length != 20 || !ValueIsDigit(value))
                {
                    return false;
                }
            }

            // 32 DIGIT
            else if (dataAttribute is "8042")
            {
                cErrorMessage = $"{string.Format(CodeLang.ErrorLengthDigits_Text, 32)}";

                if (value.Length != 32 || !ValueIsDigit(value))
                {
                    return false;
                }
            }

            // Mixed/others: digits and characters

            // 3 DIGIT 1 * 9 XCHAR - ISO country code
            else if (dataAttribute is "421")
            {
                cErrorMessage = $"{string.Format(CodeLang.ErrorLengthBetweenCharacters_Text, 4, 12)}";

                if (value.Length is < 4 or > 12 || !ValueIsDigit(value[..3]) || !IsKnownISOCountryCode(value[..3]))
                {
                    return false;
                }
            }

            // 3 DIGIT 1 * 27 XCHAR - ISO country code
            else if (dataAttribute is "7030" or "7031" or "7032" or "7033" or "7034" or "7035" or "7036" or "7037" or "7038" or "7039")
            {
                cErrorMessage = $"{string.Format(CodeLang.ErrorLengthBetweenCharacters_Text, 4, 30)}";

                if (value.Length is < 4 or > 30 || !ValueIsDigit(value[..3]) || !IsKnownISOCountryCode(value[..3]))
                {
                    return false;
                }
            }

            // Dates / times (x 16)

            // 6 DIGIT (date YYMMDD)
            else if (dataAttribute is "11" or "12" or "13" or "15" or "16" or "17" or "4326" or "7006")
            {
                cErrorMessage = $"{string.Format(CodeLang.ErrorLengthDigits_Text, 6)}";

                if (value.Length != 6 || !ValueIsDigit(value) || !IsDate(value, "yyMMdd"))
                {
                    return false;
                }
            }

            // 6 DIGIT [4 DIGIT] (date/time YYMMDD[hhmm])
            else if (dataAttribute is "7011")
            {
                switch (value.Length)
                {
                    case 6 when ValueIsDigit(value) && IsDate(value, "yyMMdd"):
                        break;
                    case 10 when ValueIsDigit(value) && IsDate(value[..6], "yyMMdd") && IsTime(value.Substring(6, 4), "HHmm"):
                        break;
                    default:
                        cErrorMessage = $"{string.Format(CodeLang.ErrorLengthBetweenDigits_Text, 6, 10)}  -> 6 / 10";
                        return false;
                }
            }

            // 6 DIGIT [6 DIGIT] (date YYMMDD[YYMMDD])
            else if (dataAttribute is "7007")
            {
                switch (value.Length)
                {
                    case 6 when ValueIsDigit(value) && IsDate(value, "yyMMdd"):
                        break;
                    case 12 when ValueIsDigit(value) && IsDate(value[..6], "yyMMdd") && IsDate(value.Substring(6, 6), "yyMMdd"):
                        break;
                    default:
                        cErrorMessage = $"{string.Format(CodeLang.ErrorLengthBetweenDigits_Text, 6, 12)}  -> 6 / 12";
                        return false;
                }
            }

            // 8 DIGIT (date YYYYMMDD)
            else if (dataAttribute is "7250")
            {
                cErrorMessage = $"{string.Format(CodeLang.ErrorLengthDigits_Text, 8)}";

                if (value.Length != 8 || !ValueIsDigit(value) || !IsDate(value, "yyyyMMdd"))
                {
                    return false;
                }
            }

            // 8 DIGIT [2 DIGIT] [2 DIGIT] (date/time YYMMDDhh[mm][ss])
            else if (dataAttribute is "8008")
            {
                switch (value.Length)
                {
                    case 8 when ValueIsDigit(value) && IsDate(value[..6], "yyMMdd") && IsTime(value.Substring(6, 2), "HH"):
                        break;
                    case 10 when ValueIsDigit(value) && IsDate(value[..6], "yyMMdd") && IsTime(value.Substring(6, 4), "HHmm"):
                        break;
                    case 12 when ValueIsDigit(value) && IsDate(value[..6], "yyMMdd") && IsTime(value.Substring(6, 6), "HHmmss"):
                        break;
                    default:
                        cErrorMessage = $"{string.Format(CodeLang.ErrorLengthBetweenDigits_Text, 8, 12)}  -> 8 / 10 / 12";
                        return false;
                }
            }

            // 10 DIGIT (date/time YYMMDDhhmm)
            else if (dataAttribute is "4324" or "4325" or "7003")
            {
                cErrorMessage = $"{string.Format(CodeLang.ErrorLengthDigits_Text, 10)}";

                if (value.Length != 10 || !ValueIsDigit(value) || !IsDate(value[..6], "yyMMdd") || !IsTime(value.Substring(6, 4), "HHmm"))
                {
                    return false;
                }
            }

            // 12 DIGIT (date/time YYYYMMDDhhmm)
            else if (dataAttribute is "7251")
            {
                cErrorMessage = $"{string.Format(CodeLang.ErrorLengthDigits_Text, 12)}";

                if (value.Length != 12 || !ValueIsDigit(value) || !IsDate(value[..8], "yyyyMMdd") || !IsTime(value.Substring(8, 4), "HHmm"))
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
                cErrorMessage = $"{string.Format(CodeLang.ErrorLengthDigits_Text, 6)}";

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
        internal static bool IsKnownGs1AppID(string cAppId)
        {
            return ClassLoadDataFromFile._knownGs1AppIDs.ContainsKey(cAppId);
        }

        /// <summary>
        /// Get the short description of the provided application identifier (AI) and return the value from the predefined list of known
        /// GS1 Application Identifiers (AIs). If the AI is not found, return an empty string.
        /// </summary>
        /// <param name="cAppId"></param>
        /// <returns></returns>
        public static string GetKnownGs1AppIDValue(string cAppId)
        {
            return ClassLoadDataFromFile._knownGs1AppIDs.TryGetValue(cAppId, out string? value) ? value : string.Empty;
        }

        /// <summary>
        /// Read a key from the language resource file and return the corresponding long description
        /// </summary>
        /// <param name="cAppId"></param>
        /// <returns></returns>
        public static string GetKnownGs1AppIDValueFromLangResource(string cAppId)
        {
            // If the key starts with 3 and the length is 4, if the last character is not 0, then replace the last character with a zero
            if (cAppId.StartsWith('3') && cAppId.Length == 4 && cAppId[^1] != '0')
            {
                cAppId = cAppId[..3] + "0";
            }

            else if (cAppId.StartsWith("703") && cAppId.Length == 4 && cAppId[^1] != '0')
            {
                cAppId = cAppId[..3] + "0";
            }

            else if (cAppId.StartsWith("723") && cAppId.Length == 4 && cAppId[^1] != '0')
            {
                cAppId = cAppId[..3] + "0";
            }

            else if (cAppId.StartsWith('9') && cAppId.Length == 2 && cAppId[^1] != '1')
            {
                cAppId = cAppId[..1] + "1";
            }

            // Search the value in the resource file using the key and return the value. If the value is not found, return an empty string.
            ResourceManager rm = GS1AppIdLang.ResourceManager;
            string? value = rm.GetString(cAppId, CultureInfo.CurrentUICulture);
            return value ?? string.Empty;
        }

        /// <summary>
        /// Get the pattern of the provided application identifier(AI) and return the value from the predefined list of known
        /// GS1 Application Identifiers (AIs). If the AI is not found, return an empty string.
        /// </summary>
        /// <param name="cAppId"></param>
        /// <returns></returns>
        public static string GetKnownGs1ValuePattern(string cAppId)
        {
            return ClassLoadDataFromFile._knownGs1ValuePatterns.TryGetValue(cAppId, out string? value) ? value : string.Empty;
        }

        /// <summary>
        /// Get the length of the IBAN code for a country code and return the value from the predefined list of known country codes
        /// </summary>
        /// <param name="cCountryCode"></param>
        /// <returns></returns>
        public static int GetKnownIbanLengthValue(string cCountryCode)
        {
            return ClassLoadDataFromFile._knownIbanData.TryGetValue(cCountryCode, out int value) ? value : 0;
        }

        /// <summary>
        /// Check if the provided country code is a valid ISO country code by searching in the predefined list of known ISO country codes
        /// </summary>
        /// <param name="cCountryCode"></param>
        /// <returns></returns>
        private static bool IsKnownISOCountryCode(string cCountryCode)
        {
            try
            {
                for (int i = 0; i < ClassLoadDataFromFile._knownIsoCountryCodes.GetLength(0); i++)
                {
                    if (ClassLoadDataFromFile._knownIsoCountryCodes[i, 3] == cCountryCode)
                    {
                        return true;
                    }
                }

                // If the country code is not found, return false
                cErrorMessage = CodeLang.ErrorGS1CountryInvalid_Text;
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in IsKnownISOCountryCode: {ex.Message}");
                cErrorMessage = CodeLang.ErrorGS1CountryInvalid_Text;
                return false;
            }
        }

        /// <summary>
        /// Get the name of the country for a country code and return the value from the predefined list of known numeric country code
        /// </summary>
        /// <param name="cCountryCodeNum"></param>
        /// <returns></returns>
        public static string GetKnownISOCountry(string cCountryCodeNum)
        {
            try
            {
                for (int i = 0; i < ClassLoadDataFromFile._knownIsoCountryCodes.GetLength(0); i++)
                {
                    if (ClassLoadDataFromFile._knownIsoCountryCodes[i, 3] == cCountryCodeNum)
                    {
                        return ClassLoadDataFromFile._knownIsoCountryCodes[i, 0];
                    }
                }

                // If the country code is not found, return an empty string
                return string.Empty;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in GetKnownISOCountry: {ex.Message}");
                return string.Empty;
            }
        }

        /// <summary>
        /// Check if the provided currency code is a valid ISO currency code by searching in the predefined list of known ISO currency codes
        /// </summary>
        /// <param name="cCurrencyCode"></param>
        /// <returns></returns>
        private static bool IsKnownISOCurrencyCode(string cCurrencyCode)
        {
            try
            {
                for (int i = 0; i < ClassLoadDataFromFile._knownIsoCurrencyCodes.GetLength(0); i++)
                {
                    if (ClassLoadDataFromFile._knownIsoCurrencyCodes[i, 3] == cCurrencyCode)
                    {
                        return true;
                    }
                }

                // If the currency code is not found, return false
                cErrorMessage = CodeLang.ErrorGS1CurrencyInvalid_Text;
                return false;
            }

            catch (Exception ex)
            {
                Debug.WriteLine($"Error in IsKnownISOCurrencyCode: {ex.Message}");
                cErrorMessage = CodeLang.ErrorGS1CurrencyInvalid_Text;
                return false;
            }
        }

        /// <summary>
        /// Get the name of the currency for a currency code and return the value from the predefined list of known numeric currency code
        /// </summary>
        /// <param name="cCurrencyCodeNum"></param>
        /// <returns></returns>
        public static string GetKnownISOCurrency(string cCurrencyCodeNum)
        {
            try
            {
                for (int i = 0; i < ClassLoadDataFromFile._knownIsoCurrencyCodes.GetLength(0); i++)
                {
                    if (ClassLoadDataFromFile._knownIsoCurrencyCodes[i, 3] == cCurrencyCodeNum)
                    {
                        return ClassLoadDataFromFile._knownIsoCurrencyCodes[i, 2];
                    }
                }

                // If the currency code is not found, return an empty string
                return string.Empty;
            }
            
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in GetKnownISOCurrency: {ex.Message}");
                return string.Empty;
            }
        }

        /// <summary>
        /// Get the start index of the value for the specified GS1 Application Identifier (AI) in the GS1 identifier string.
        /// </summary>
        /// <param name="gs1Identifier"></param>
        /// <param name="appID"></param>
        /// <param name="expectedValue"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        private static int GetValueStart(string gs1Identifier, string appID, string expectedValue)
        {
            int queryStart = gs1Identifier.IndexOf('(');
            string pathPart = queryStart >= 0 ? gs1Identifier[..queryStart] : gs1Identifier;
            string[] pathSegments = pathPart.Split(')', StringSplitOptions.RemoveEmptyEntries);
            int valueStart = -1;
            string? value = null;
            int position = 0;

            for (int i = 0; i + 1 < pathSegments.Length; i += 2)
            {
                int keyStart = pathPart.IndexOf(pathSegments[i], position, StringComparison.Ordinal);
                int segmentStart = pathPart.IndexOf(pathSegments[i + 1], keyStart + pathSegments[i].Length, StringComparison.Ordinal);
                if (pathSegments[i] == appID)
                {
                    valueStart = segmentStart;
                    value = pathSegments[i + 1];
                }
                position = segmentStart + pathSegments[i + 1].Length;
            }

            if (queryStart >= 0)
            {
                string queryPart = gs1Identifier[(queryStart + 1)..];
                string[] queryPairs = queryPart.Split('(', StringSplitOptions.RemoveEmptyEntries);
                position = queryStart + 1;
                foreach (string pair in queryPairs)
                {
                    int pairStart = gs1Identifier.IndexOf(pair, position, StringComparison.Ordinal);
                    int equalsIndex = pair.IndexOf(')');
                    if (equalsIndex > 0 && pair[..equalsIndex] == appID)
                    {
                        valueStart = pairStart + equalsIndex + 1;
                        value = pair[(equalsIndex + 1)..];
                    }
                    position = pairStart + pair.Length;
                }
            }

            if (valueStart < 0 || !string.Equals(value, expectedValue, StringComparison.Ordinal))
            {
                Debug.WriteLine("The AI 01 value no longer matches the GS1 identifier.");
            }

            return valueStart;
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
        /// <param name="format">The date format to validate against.</param>
        /// <returns>`true` if `input` is a valid date in `yyMMdd` format; otherwise, `false`.</returns>
        private static bool IsDate(string input, string format)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return false;
            }

            // Try parsing with exact format "yyMMdd"
            bool bIsValid = DateTime.TryParseExact(
                input,
                format,
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

https://ref.gs1.org/
https://ref.gs1.org/ai/
https://ref.gs1.org/standards/digital-link/uri-syntax/
https://github.com/gs1/DigitalLinkDocs
https://github.com/Evebury/Evebury.Gs1.DigitalLink
https://github.com/duruilhan/gs1-digital-link-toolkit
https://gs1-2027.com/structure-and-parse-gs1-digital-link-uris/
https://github.com/Solidsoft-Reply/Gs1DigitalLinkLib
*/