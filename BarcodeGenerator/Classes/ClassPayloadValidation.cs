using System.ComponentModel.DataAnnotations;
using System.Numerics;
using System.Text.RegularExpressions;

namespace BarcodeGenerator
{
    internal partial class ClassPayloadValidation
    {
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
    }
}
