using System.ComponentModel.DataAnnotations;
using System.Numerics;
using System.Text.RegularExpressions;

namespace BarcodeGenerator
{
    internal partial class ClassPayloadValidation
    {
        // Add once in the class (central place for all supported GS1 keys)
        private static readonly string[] _knownGs1Keys =
        [
            // Primary identification key formats (x 16)
            "/01/",     // gtin-code
            "/8006/",   // itip-code
            "/8013/",   // gmn-code
            "/8010/",   // cpid-code
            "/414/",    // gln-code
            "/415/",    // payTo-code
            "/417/",    // partyGln-code
            "/8017/",   // gsrnp-code
            "/8018/",   // gsrn-code
            "/255/",    // gcn-code
            "/00/",     // sscc-code
            "/253/",    // gdti-code
            "/401/",    // ginc-code
            "/402/",    // gsin-code
            "/8003/",   // grai-code
            "/8004/",   // giai-code
            
            // Key qualifier formats ( x 9)
            "/22/",     // cpv-code
            "/10/",     // lot-code
            "/21/",     // ser-code
            "/8011/",   // cpsn-code
            "/254/",    // glnx-code
            "/8020/",   // refno-code
            "/8019/",   // srin-code
            "/235/",    // tpx-code
            "/7040/"    // uic-ext-code
        ];

        //  82-character subset of ISO/IEC 646 (for GS1 identifiers)
        private static readonly string _82CharacterSubset = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz\"-._!%&+,/*[]';:<>=?";

        // 39-character subset of ISO/IEC 646 (for GS1 identifiers)
        private static readonly string _39CharacterSubset = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ-#/";

        /// <summary>
        /// Determines whether the specified string is a valid phone number format based on a regular expression pattern that allows for international and local formats, including optional country code, spaces, dashes, and parentheses.
        /// </summary>
        /// <param name="entry">The Entry containing the phone number string to validate. Must not be null.</param>
        /// <returns>true if the specified string is a valid phone number format; otherwise, false.</returns>
        public static async Task<bool> IsValidPhoneNumber(Entry entry)
        {
            if (!PhoneRegex().IsMatch(entry.Text))
            {
                await Application.Current!.Windows[0].Page!.DisplayAlertAsync(CodeLang.ErrorTitle_Text, CodeLang.ErrorPhoneNumberInvalid_Text, CodeLang.ButtonClose_Text);
                _ = entry.Focus();
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
            return RegexPhone;
        }

        /// <summary>
        /// Basic regex for validating phone number format
        /// </summary>
        /// <returns></returns>
        [GeneratedRegex(@"^\+?[0-9\s\-\(\).]{3,20}$", RegexOptions.Compiled)]
        private static partial Regex RegexPhone { get; }

        /// <summary>
        /// Determines whether the specified string is a valid email address format.
        /// </summary>
        /// <remarks>This method checks the format of the email address but does not verify that the address exists or is reachable.</remarks>
        /// <param name="entry">The Entry control containing the email address to validate. Cannot be null, empty, or consist only of white-space characters.</param>
        /// <returns>true if the specified string is a valid email address format; otherwise, false.</returns>
        public static async Task<bool> IsValidEmail(Entry entry)
        {
            bool bIsValid = !string.IsNullOrWhiteSpace(entry.Text) && new EmailAddressAttribute().IsValid(entry.Text);

            if (!bIsValid)
            {
                await Application.Current!.Windows[0].Page!.DisplayAlertAsync(CodeLang.ErrorTitle_Text, CodeLang.ErrorEmailInvalid_Text, CodeLang.ButtonClose_Text);
                _ = entry.Focus();
                return false;
            }

            return true;
        }

        /// <summary>
        /// Determines whether the specified string is a valid IBAN (International Bank Account Number).
        /// </summary>
        /// <remarks>Validates both the format and checksum using the mod-97 algorithm defined in ISO
        /// 13616. Spaces in the input are automatically removed during validation.</remarks>
        /// <param name="entry" The Entry control containing the IBAN to validate.</param>
        /// <returns>true if the IBAN is valid; otherwise, false.</returns>
        public static bool IsValidIban(Entry entry)
        {
            // Remove spaces for standardization
            string cleanIban = entry.Text.Replace(" ", string.Empty);
            entry.Text = cleanIban;

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
        /// Basic regex for validating IBAN format: starts with 2 letters (country code), followed by 2 digits (check digits),
        /// and then up to 30 alphanumeric characters (BBAN).
        /// </summary>
        /// <returns></returns>
        [GeneratedRegex("^[A-Z]{2}[0-9]{2}[A-Z0-9]{1,30}$")]
        private static partial Regex RegexIban();

        /// <summary>
        /// Determines whether a remittance information string is in a valid structured format (European RF or Belgian).
        /// </summary>
        /// <param name="entry">The Entry control containing the remittance information string to validate.</param>
        /// <returns>true if valid structured remittance information; otherwise, false.</returns>
        public static bool IsValidRemittanceInfoStructured(Entry entry)
        {
            // Structured remittance information is optional, so empty or whitespace is considered valid
            if (string.IsNullOrWhiteSpace(entry.Text))
            {
                return true;
            }

            // Remove spaces for standardization
            string cRemittanceInfo = entry.Text;
            cRemittanceInfo = cRemittanceInfo.Replace(" ", string.Empty);
            entry.Text = cRemittanceInfo;

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
        /// Determines whether the specified string is a valid absolute HTTP or HTTPS URL.
        /// </summary>
        /// <param name="entry">The Entry containing the URL string to validate. Must not be null.</param>
        /// <returns>true if the string is a well-formed absolute HTTP or HTTPS URL; otherwise, false.</returns>
        public static async Task<bool> IsValidUrl(Entry entry)
        {
            bool bIsValid = Uri.TryCreate(entry.Text, UriKind.Absolute, out Uri? uriResult) &&
                (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

            if (!bIsValid)
            {
                await Application.Current!.Windows[0].Page!.DisplayAlertAsync(CodeLang.ErrorTitle_Text, CodeLang.ErrorUrlInvalid_Text, CodeLang.ButtonClose_Text);

                _ = entry.Focus();
                return false;
            }

            return true;
        }

        /// <summary>
        /// Determines whether the specified string is a valid GS1 identifier
        /// </summary>
        /// <param name="editor">The Editor containing the GS1 identifier string to validate.</param>
        /// <returns>true if the specified string is a valid GS1 identifier; otherwise, false.</returns>
        public static async Task<bool> IsValidGS1Identifier(Editor editor)
        {
            // GS1 Digital Link Test Suite: https://ref.gs1.org/test-suites/resolver/
            // GS1 Digital Link Validator: https://qrbolt.com/en/validate-gs1
            // FeatherQR Playground: https://guitarrapc.github.io/FeatherQR/

            string gs1Identifier = editor.Text;
            bool bIsValid = true;
            string cValue;
            string cKey = string.Empty;

            // Add a null check for the GS1 identifier to ensure that it is not null, empty, or consists only of white-space characters
            if (string.IsNullOrWhiteSpace(gs1Identifier))
            {
                bIsValid = false;
            }

            //// If the GS1 identifier does not start with /01/, return false
            //if (!gs1Identifier.StartsWith("/01/"))
            //{
            //    bIsValid = false;
            //}

            // Primary identification key formats (x 16)

            // gtin-code 01 - Global Trade Item Number (GTIN) = 14DIGIT ; GTIN-8, GTIN-12 and GTIN-13 SHALL be expressed as 14 digits, with leading zeroes serving as filler digits
            if (gs1Identifier.Contains("/01/"))
            {
                // Get the value of the GS1 identifier following the key
                cKey = "/01/";
                cValue = GetKeyValue(cKey, gs1Identifier);

                // If the GS1 identifier length is less than 14 characters, add zeroes to the left to make it 14 characters
                // and replace the original value with the new value in the editor
                if (cValue.Length < 14)
                {
                    cValue = cValue.PadLeft(14, '0');
                    editor.Text = gs1Identifier.Replace(GetKeyValue(cKey, gs1Identifier), cValue);
                    gs1Identifier = editor.Text;
                }

                // The GTIN must have a valid check digit (/01/09520123456788 is valid, /01/09520123456789 is invalid)
                if (bIsValid && !ValidateValueCheckDigit(cValue, 14))
                {
                    bIsValid = false;
                }

                // Check the path element order for /22/, /10/, and /21/ if they exist in the GS1 identifier
                if (bIsValid)
                {
                    int nStartPos22 = GetKeyStartPosition("/22/", gs1Identifier);
                    int nStartPos10 = GetKeyStartPosition("/10/", gs1Identifier);
                    int nStartPos21 = GetKeyStartPosition("/21/", gs1Identifier);
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
                }
            }

            // itip-code 8006 - International Ticketing Information Protocol (ITIP) = 14DIGIT 2DIGIT 2DIGIT ; 14 digits then 2 digits then 2 digits
            if (gs1Identifier.Contains("/8006/"))
            {
                // Get the value of the GS1 identifier following the key
                cKey = "/8006/";
                cValue = GetKeyValue(cKey, gs1Identifier);

                // If the GS1 identifier does not have a length of 18 characters, return false
                if (cValue.Length != 18)
                {
                    bIsValid = false;
                }

                // The characters of the GS1 identifier must be numeric
                else if (bIsValid)
                {
                    bIsValid = ValueIsDigit(cValue);
                }

                // Check the path element order for /22/, /10/, and /21/ if they exist in the GS1 identifier
                if (bIsValid)
                {
                    int nStartPos22 = GetKeyStartPosition("/22/", gs1Identifier);
                    int nStartPos10 = GetKeyStartPosition("/10/", gs1Identifier);
                    int nStartPos21 = GetKeyStartPosition("/21/", gs1Identifier);

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
                }
            }

            // gmn-code 8013 - Global Model Number (GMN) = 1*25XCHAR ; 1-25 characters from 82-character subset
            if (gs1Identifier.Contains("/8013/"))
            {
                // Get the value of the GS1 identifier following the key
                cKey = "/8013/";
                cValue = GetKeyValue(cKey, gs1Identifier);

                // If the GS1 identifier does not have a length of 1-25 characters, return false
                if (cValue.Length is < 1 or > 25)
                {
                    bIsValid = false;
                }

                // The characters of the GS1 identifier must be from the 82-character subset
                else if (bIsValid)
                {
                    bIsValid = ValueIs82CharacterSubset(cValue);
                }
            }

            // cpid-code 8010 - Component / Part Identifier (CPID) = 1*30YCHAR ; 1-30 characters from 39-character subset
            if (gs1Identifier.Contains("/8010/"))
            {
                // Get the value of the GS1 identifier following the key
                cKey = "/8010/";
                cValue = GetKeyValue(cKey, gs1Identifier);

                // If the GS1 identifier does not have a length of 1-30 characters, return false
                if (cValue.Length is < 1 or > 30)
                {
                    bIsValid = false;
                }

                // The characters of the GS1 identifier must be from the 39-character subset
                else if (bIsValid)
                {
                    bIsValid = ValueIs39CharacterSubset(cValue);
                }
            }

            // gln-code 414 - Physical / Global Location Number (GLN) = 13DIGIT ; exactly 13 digits
            if (gs1Identifier.Contains("/414/"))
            {
                // Get the value of the GS1 identifier following the key
                cKey = "/414/";
                cValue = GetKeyValue(cKey, gs1Identifier);

                // If the GS1 identifier length is less than 13 characters, add zeroes to the left to make it 13 characters
                // and replace the original value with the new value in the editor
                if (cValue.Length < 13)
                {
                    cValue = cValue.PadLeft(13, '0');
                    editor.Text = gs1Identifier.Replace(GetKeyValue(cKey, gs1Identifier), cValue);
                    gs1Identifier = editor.Text;
                }

                // The GTIN must have a valid check digit (/414/9520123456788 is valid, /414/9520123456780 is invalid)
                if (bIsValid && !ValidateValueCheckDigit(cValue, 13))
                {
                    bIsValid = false;
                }
            }

            // payTo-code 415 - GLN of invoicing party = 13DIGIT ; exactly 13 digits
            if (gs1Identifier.Contains("/415/"))
            {
                // Get the value of the GS1 identifier following the key
                cKey = "/415/";
                cValue = GetKeyValue(cKey, gs1Identifier);

                // If the GS1 identifier does not have a length of 13 characters, return false
                if (cValue.Length != 13)
                {
                    bIsValid = false;
                }

                // The characters of the GS1 identifier must be numeric
                else if (bIsValid)
                {
                    bIsValid = ValueIsDigit(cValue);
                }
            }

            // partyGln-code 417 - Party GLN = 13DIGIT ; exactly 13 digits
            if (gs1Identifier.Contains("/417/"))
            {
                // Get the value of the GS1 identifier following the key
                cKey = "/417/";
                cValue = GetKeyValue(cKey, gs1Identifier);

                // If the GS1 identifier does not have a length of 13 characters, return false
                if (cValue.Length != 13)
                {
                    bIsValid = false;
                }

                // The characters of the GS1 identifier must be numeric
                else if (bIsValid)
                {
                    bIsValid = ValueIsDigit(cValue);
                }
            }

            // gsrnp-code 8017 -  GSRN of the Provider = 18DIGIT ; exactly 18 digits
            if (gs1Identifier.Contains("/8017/"))
            {
                // Get the value of the GS1 identifier following the key
                cKey = "/8017/";
                cValue = GetKeyValue(cKey, gs1Identifier);

                // If the GS1 identifier does not have a length of 18 characters, return false
                if (cValue.Length != 18)
                {
                    bIsValid = false;
                }

                // The characters of the GS1 identifier must be numeric
                else if (bIsValid)
                {
                    bIsValid = ValueIsDigit(cValue);
                }
            }

            // gsrn-code 8018 - GSRN of the Recipient = 18DIGIT ; exactly 18 digits
            if (gs1Identifier.Contains("/8018/"))
            {
                // Get the value of the GS1 identifier following the key
                cKey = "/8018/";
                cValue = GetKeyValue(cKey, gs1Identifier);

                // If the GS1 identifier does not have a length of 18 characters, return false
                if (cValue.Length != 18)
                {
                    bIsValid = false;
                }

                // The characters of the GS1 identifier must be numeric
                else if (bIsValid)
                {
                    bIsValid = ValueIsDigit(cValue);
                }
            }

            // gcn-code 255 - Global Coupon Number (GCN) =  13DIGIT [1*12DIGIT] ; 13 digits then optional 1 - 12 digits
            if (gs1Identifier.Contains("/255/"))
            {
                // Get the value of the GS1 identifier following the key
                cKey = "/255/";
                cValue = GetKeyValue(cKey, gs1Identifier);

                // If the GS1 identifier does not have a length of 13-25 characters, return false
                if (cValue.Length is < 13 or > 25)
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
            if (gs1Identifier.Contains("/00/"))
            {
                // Get the value of the GS1 identifier following the key
                cKey = "/00/";
                cValue = GetKeyValue(cKey, gs1Identifier);

                // If the GS1 identifier length is less than 18 characters, add zeroes to the left to make it 18 characters
                // and replace the original value with the new value in the editor
                if (cValue.Length < 18)
                {
                    cValue = cValue.PadLeft(18, '0');
                    editor.Text = gs1Identifier.Replace(GetKeyValue(cKey, gs1Identifier), cValue);
                    gs1Identifier = editor.Text;
                }

                // The GTIN must have a valid check digit (/00/195201234567891232 is valid, /00/195201234567891230 is invalid)
                if (bIsValid && !ValidateValueCheckDigit(cValue, 18))
                {
                    bIsValid = false;
                }
            }

            // gdti-code 253 - Global Document Type Identifier (GDTI) = 13DIGIT [1*17XCHAR] ; 13 digits then optional 1 - 17 characters ; from the 82-character subset
            if (gs1Identifier.Contains("/253/"))
            {
                // Get the value of the GS1 identifier following the key
                cKey = "/253/";
                cValue = GetKeyValue(cKey, gs1Identifier);

                // If the GS1 identifier does not have a length of 13-30 characters, return false
                if (cValue.Length is < 13 or > 30)
                {
                    bIsValid = false;
                }

                // The first 13characters of the GS1 identifier must be numeric
                else if (bIsValid)
                {
                    bIsValid = ValueIsDigit(cValue[..13]);
                }

                // The characters of the GS1 identifier must be from the 82-character subset (from character 14 to the end)
                else if (bIsValid && cValue.Length > 13)
                {
                    bIsValid = ValueIs82CharacterSubset(cValue[13..]);
                }
            }

            // ginc-code 401 - Global Identification Number for Consignment (GINC) = 1*30XCHAR ; 1 - 30 characters from the 82-character subset
            if (gs1Identifier.Contains("/401/"))
            {
                // Get the value of the GS1 identifier following the key
                cKey = "/401/";
                cValue = GetKeyValue(cKey, gs1Identifier);

                // If the GS1 identifier does not have a length of 1-30 characters, return false
                if (cValue.Length is < 1 or > 30)
                {
                    bIsValid = false;
                }

                // The characters of the GS1 identifier must be from the 82-character subset
                else if (bIsValid)
                {
                    bIsValid = ValueIs82CharacterSubset(cValue);
                }
            }

            // gsin-code 402 - Global Shipment Identification Number (GSIN) = 17DIGIT ; exactly 17 digits
            if (gs1Identifier.Contains("/402/"))
            {
                // Get the value of the GS1 identifier following the key
                cKey = "/402/";
                cValue = GetKeyValue(cKey, gs1Identifier);

                // If the GS1 identifier does not have a length of 17 characters, return false
                if (cValue.Length != 17)
                {
                    bIsValid = false;
                }

                // The characters of the GS1 identifier must be numeric
                else if (bIsValid)
                {
                    bIsValid = ValueIsDigit(cValue);
                }
            }

            // grai-code 8003 - Global Returnable Asset Identifier (GRAI) = 13DIGIT [1*16XCHAR] ; 13 digits then optional 1 - 16 characters ; from the 82-character subset of ISO / IEC 646
            if (gs1Identifier.Contains("/8003/"))
            {
                // Get the value of the GS1 identifier following the key
                cKey = "/8003/";
                cValue = GetKeyValue(cKey, gs1Identifier);

                // If the GS1 identifier does not have a length of 13-29 characters, return false
                if (cValue.Length is < 13 or > 29)
                {
                    bIsValid = false;
                }

                // The first 13 characters of the GS1 identifier must be numeric
                else if (bIsValid)
                {
                    bIsValid = ValueIsDigit(cValue[..13]);
                }

                // The characters of the GS1 identifier must be from the 82-character subset (from character 14 to the end)
                else if (bIsValid && cValue.Length > 13)
                {
                    bIsValid = ValueIs82CharacterSubset(cValue[13..]);
                }
            }

            // giai-code 8004 - Global Individual Asset Identifier (GIAI) = 1*30XCHAR  ; 1-30 characters from 82-character subset
            if (gs1Identifier.Contains("/8004/"))
            {
                // Get the value of the GS1 identifier following the key
                cKey = "/8004/";
                cValue = GetKeyValue(cKey, gs1Identifier);

                // If the GS1 identifier does not have a length of 1-30 characters, return false
                if (cValue.Length is < 1 or > 30)
                {
                    bIsValid = false;
                }

                // The characters of the GS1 identifier must be from the 82-character subset
                else if (bIsValid)
                {
                    bIsValid = ValueIs82CharacterSubset(cValue);
                }
            }

            // Key qualifier formats ( x 9)

            // cpv-code 22 - Consumer Product Variant (CPV) = 1*20XCHAR ; 1-20 characters from 82-character subset
            if (gs1Identifier.Contains("/22/"))
            {
                // Get the value of the GS1 identifier following the key
                cKey = "/22/";
                cValue = GetKeyValue(cKey, gs1Identifier);

                // If the GS1 identifier does not have a length of 1-20 characters, return false
                if (cValue.Length is < 1 or > 20)
                {
                    bIsValid = false;
                }

                // The characters of the GS1 identifier must be from the 82-character subset
                else if (bIsValid)
                {
                    bIsValid = ValueIs82CharacterSubset(cValue);
                }
            }

            // lot-code 10 - Batch/Lot identifier = 1*20XCHAR ; 1-20 characters from 82-character subset
            if (gs1Identifier.Contains("/10/"))
            {
                // Get the value of the GS1 identifier following the key
                cKey = "/10/";
                cValue = GetKeyValue(cKey, gs1Identifier);

                // If the GS1 identifier does not have a length of 1-20 characters, return false
                if (cValue.Length is < 1 or > 20)
                {
                    bIsValid = false;
                }

                // The characters of the GS1 identifier must be from the 82-character subset
                else if (bIsValid)
                {
                    bIsValid = ValueIs82CharacterSubset(cValue);
                }
            }

            // ser-code 21 - GTIN Serial Number = 1*20XCHAR ; 1-20 characters from 82-character subset
            if (gs1Identifier.Contains("/21/"))
            {
                // Get the value of the GS1 identifier following the key
                cKey = "/21/";
                cValue = GetKeyValue(cKey, gs1Identifier);

                // If the GS1 identifier does not have a length of 1-20 characters, return false
                if (cValue.Length is < 1 or > 20)
                {
                    bIsValid = false;
                }

                // The characters of the GS1 identifier must be from the 82-character subset
                else if (bIsValid)
                {
                    bIsValid = ValueIs82CharacterSubset(cValue);
                }
            }

            // cpsn-code 8011 - CPID Serial Number = 1*12DIGIT ; 1-12 digits
            if (gs1Identifier.Contains("/8011/"))
            {
                // Get the value of the GS1 identifier following the key
                cKey = "/8011/";
                cValue = GetKeyValue(cKey, gs1Identifier);

                // If the GS1 identifier does not have a length of 1-12 digits, return false
                if (cValue.Length is < 1 or > 12)
                {
                    bIsValid = false;
                }

                // The characters of the GS1 identifier must be numeric
                else if (bIsValid)
                {
                    bIsValid = ValueIsDigit(cValue);
                }
            }

            // glnx-code 254 - GLN extension = 1*20XCHAR ; 1-20 characters from 82-character subset
            if (gs1Identifier.Contains("/254/"))
            {
                // Get the value of the GS1 identifier following the key
                cKey = "/254/";
                cValue = GetKeyValue(cKey, gs1Identifier);

                // If the GS1 identifier does not have a length of 1-20 characters, return false
                if (cValue.Length is < 1 or > 20)
                {
                    bIsValid = false;
                }

                // The characters of the GS1 identifier must be from the 82-character subset
                else if (bIsValid)
                {
                    bIsValid = ValueIs82CharacterSubset(cValue);
                }
            }

            // refno-code 8020 - Payment Reference Number = 1*25XCHAR ; 1-25 characters from 82-character subset
            if (gs1Identifier.Contains("/8020/"))
            {
                // Get the value of the GS1 identifier following the key
                cKey = "/8020/";
                cValue = GetKeyValue(cKey, gs1Identifier);

                // If the GS1 identifier does not have a length of 1-25 characters, return false
                if (cValue.Length is < 1 or > 25)
                {
                    bIsValid = false;
                }
                
                // The characters of the GS1 identifier must be from the 82-character subset
                else if (bIsValid)
                {
                    bIsValid = ValueIs82CharacterSubset(cValue);
                }
            }

            // srin-code 8019 - Service Relation Instance Number = 1*10DIGIT ; 1-10 digits
            if (gs1Identifier.Contains("/8019/"))
            {
                // Get the value of the GS1 identifier following the key
                cKey = "/8019/";
                cValue = GetKeyValue(cKey, gs1Identifier);

                // If the GS1 identifier does not have a length of 1-10 digits, return false
                if (cValue.Length is < 1 or > 10)
                {
                    bIsValid = false;
                }

                // The characters of the GS1 identifier must be numeric
                else if (bIsValid)
                {
                    bIsValid = ValueIsDigit(cValue);
                }
            }

            // tpx-code 235 - third-party controlled serialised extension to GTIN = 1*28XCHAR ; 1-28 characters from 82-character subset
            if (gs1Identifier.Contains("/235/"))
            {
                // Get the value of the GS1 identifier following the key
                cKey = "/235/";
                cValue = GetKeyValue(cKey, gs1Identifier);

                // If the GS1 identifier does not have a length of 1-28 characters, return false
                if (cValue.Length is < 1 or > 28)
                {
                    bIsValid = false;
                }

                // The characters of the GS1 identifier must be from the 82-character subset
                else if (bIsValid)
                {
                    bIsValid = ValueIs82CharacterSubset(cValue);
                }
            }

            // uic-ext-code 7040 - GS1 UIC with Extension 1 and Importer Index = 1DIGIT 3XCHAR ; 1 digit then 3 characters from 82-character subset
            if (gs1Identifier.Contains("/7040/"))
            {
                // Get the value of the GS1 identifier following the key
                cKey = "/7040/";
                cValue = GetKeyValue(cKey, gs1Identifier);

                // If the GS1 identifier does not have a length of 4 characters, return false
                if (cValue.Length != 4)
                {
                    bIsValid = false;
                }

                // The first character of the GS1 identifier must be numeric
                else if (bIsValid)
                {
                    bIsValid = ValueIsDigit(cValue[..1]);
                }

                // The characters of the GS1 identifier must be from the 82-character subset (from character 2 to the end)
                else if (bIsValid && cValue.Length > 1)
                {
                    bIsValid = ValueIs82CharacterSubset(cValue[1..]);
                }
            }

            // If the GS1 identifier does not contain any of the known keys, return false
            if (!ContainsKnownGs1Key(gs1Identifier))
            {
                bIsValid = false;
            }

            // If the GS1 identifier is not valid, display an error message and return false to indicate that the validation failed
            if (!bIsValid)
            {
                await Application.Current!.Windows[0].Page!.DisplayAlertAsync(CodeLang.ErrorTitle_Text, $"{CodeLang.ErrorGS1IdentifierInvalid_Text} ({cKey})", CodeLang.ButtonClose_Text);
                _ = editor.Focus();
                return false;
            }

            return bIsValid;
        }

        /// <summary>
        /// Determines whether the specified GS1 identifier contains a known GS1 key.
        /// </summary>
        /// <param name="gs1Identifier">The GS1 identifier to check.</param>
        /// <returns><see langword="true"/> if the GS1 identifier contains a known GS1 key; otherwise, <see langword="false"/>.</returns>
        private static bool ContainsKnownGs1Key(string gs1Identifier)
        {
            foreach (string key in _knownGs1Keys)
            {
                if (gs1Identifier.Contains(key, StringComparison.Ordinal))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Get the value of the Gs1 identifier following a certain key
        /// </summary>
        /// <param name="cKey"></param>
        /// <param name="cGs1Identifier"></param>
        /// <returns></returns>
        private static string GetKeyValue(string cKey, string cGs1Identifier)
        {
            int index = cGs1Identifier.IndexOf(cKey);

            if (index >= 0)
            {
                // Find the start position of the value after the key
                int nStartPosition = index + cKey.Length;

                // Find the end position of the value, which is either the next forward slash or question mark or the end of the string
                int nEndPosition = cGs1Identifier.IndexOfAny(['/', '?'], nStartPosition);

                // If no forward slash or question mark is found, set the end position to the end of the string
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
        /// Finds the start index of a key segment within a GS1 identifier string
        /// </summary>
        /// <param name="cKey"></param>
        /// <param name="cGs1Identifier"></param>
        /// <returns></returns>
        private static int GetKeyStartPosition(string cKey, string cGs1Identifier)
        {
            int index = cGs1Identifier.IndexOf(cKey);
            if (index >= 0)
            {
                // Find the start position of the key
                return index;
            }
            // If no forward slash is found, return -1
            return -1;
        }

        ///// <summary>
        ///// Finds the start and end indexes of a key segment within a GS1 identifier string.
        ///// </summary>
        ///// <param name="cKey">Key segment to locate.</param>
        ///// <param name="cGs1Identifier">GS1 identifier string to search.</param>
        ///// <returns>A tuple containing the zero-based start index and end index of the located key segment; returns (-1, -1)
        ///// when the key segment is not found. The end index is the position of the next '/' or '?' delimiter after the
        ///// start index, or the string length when no such delimiter exists.</returns>
        ///// <remarks>Use: (int nStartPos22, int nEndPos22) = GetKeyPosition("/22/", gs1Identifier);</remarks>
        //private static (int start, int end) GetKeyPosition(string cKey, string cGs1Identifier)
        //{
        //    int index = cGs1Identifier.IndexOf(cKey);
            
        //    if (index >= 0)
        //    {
        //        // Find the start position of the key
        //        int nStartPosition = index;
                
        //        // Find the end position of the key, which is either the next forward slash or question mark or the end of the string
        //        int nEndPosition = cGs1Identifier.IndexOfAny(['/', '?'], nStartPosition + 1);
                
        //        // If no forward slash or question mark is found, set the end position to the end of the string
        //        if (nEndPosition == -1)
        //        {
        //            nEndPosition = cGs1Identifier.Length;
        //        }
                
        //        return (nStartPosition, nEndPosition);
        //    }
            
        //    // If no forward slash is found, return -1 for both start and end positions
        //    return (-1, -1);
        //}

        /// <summary>
        /// Determines whether a string consists only of digit characters.
        /// </summary>
        /// <remarks>An empty string returns <see langword="true"/>. This method expects a non-null value.</remarks>
        /// <param name="cValue">The string to evaluate.</param>
        /// <returns><see langword="true"/> if all characters in <paramref name="cValue"/> are digits; otherwise, <see langword="false"/>.</returns>
        private static bool ValueIsDigit(string cValue)
        {
            foreach (char c in cValue)
            {
                if (!char.IsDigit(c))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Check if all characters in the input are in the allowed 39-character subset
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool ValueIs39CharacterSubset(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }

            // Check if all characters in the input are in the allowed set
            return value.All(c => _39CharacterSubset.Contains(c));
        }

        /// <summary>
        /// Check if all characters in the input are in the allowed 82-character subset
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool ValueIs82CharacterSubset(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }

            // Check if all characters in the input are in the allowed set
            return value.All(c => _82CharacterSubset.Contains(c));
        }

        /// <summary>
        /// Calculate the gtin check digit
        /// </summary>
        /// <param name="value"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static bool ValidateValueCheckDigit(string value, int length)
        {
            // Validate input: the value must be a non-empty string containing only digits
            if (string.IsNullOrWhiteSpace(value) || !value.All(char.IsDigit))
            {
                return false;
            }

            // Store the original value for later comparison
            string valueOriginal = value;

            // If the value is less than length digits return false
            if (value.Length != length)
            {
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
            return valueOriginal[^1] == checkDigit.ToString()[0];
        }
    }
}
