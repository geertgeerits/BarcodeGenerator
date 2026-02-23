namespace BarcodeGenerator
{
    internal class ClassValidateBarcodes
    {
        /// <summary>
        /// Test for allowed characters
        /// </summary>
        /// <param name="cAllowedCharacters"></param>
        /// <param name="cTextToCode"></param>
        /// <returns></returns>
        public async Task<bool> TestAllowedCharacters(string cAllowedCharacters, string cTextToCode)
        {
            foreach (char cChar in cTextToCode)
            {
                bool bResult = cAllowedCharacters.Contains(cChar);

                if (bResult == false)
                {
                    await Application.Current!.Windows[0].Page!.DisplayAlertAsync(CodeLang.ErrorTitle_Text, $"{CodeLang.AllowedChar_Text}\n{cAllowedCharacters}\n\n{CodeLang.AllowedCharNot_Text} {cChar}", CodeLang.ButtonClose_Text);
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Test for allowed minimum and maximum ASCII values
        /// </summary>
        /// <param name="nMinAsciiValue"></param>
        /// <param name="nMaxAsciiValue"></param>
        /// <param name="cTextToCode"></param>
        /// <returns></returns>
        public async Task<bool> TestAllowedAsciiValues(int nMinAsciiValue, int nMaxAsciiValue, string cTextToCode)
        {
            // Test for allowed minimum and maximum ASCII values
            foreach (char cChar in cTextToCode)
            {
                //Console.WriteLine($"{"ASCII value: "} {(int)cChar}");  // For testing

                if ((int)cChar < nMinAsciiValue || (int)cChar > nMaxAsciiValue)
                {
                    await Application.Current!.Windows[0].Page!.DisplayAlertAsync(CodeLang.ErrorTitle_Text, $"{CodeLang.TextContainsChar_Text} {cChar}", CodeLang.ButtonClose_Text);
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Replace special characters in strings for ASCII output (iOS keyboard marks are curved instead of straight)
        /// </summary>
        /// <param name="cText"></param>
        /// <returns></returns>
        public static string ReplaceCharacters(string cText)
        {
            // Convert characters from UTF-8 or ASCII extended to characters that are supported in ASCII
            cText = cText.Replace('‘', '\'');       // Left single quotation mark replaced with apostrophe
            cText = cText.Replace('’', '\'');       // Right single quotation mark replaced with apostrophe
            cText = cText.Replace('“', '"');        // Left double quotation mark replaced with quotation mark
            cText = cText.Replace('”', '"');        // Right double quotation mark replaced with quotation mark

            return cText;
        }

        /// <summary>
        /// Test start and end guards
        /// </summary>
        /// <param name="cStartEndGuards"></param>
        /// <param name="cTextToCode"></param>
        /// <returns></returns>
        public static async Task<bool> TestStartEndGuards(string cStartEndGuards, string cTextToCode)
        {
            int nPos;
            char cChar;

            // Control of start and end guards in the wrong place
            for (nPos = 0; nPos < cTextToCode.Length; nPos++)
            {
                cChar = cTextToCode[nPos];

                if (cStartEndGuards.Contains(cChar) && nPos > 0 && nPos < cTextToCode.Length - 1)
                {
                    await Application.Current!.Windows[0].Page!.DisplayAlertAsync(CodeLang.ErrorTitle_Text, $"{CodeLang.GuardInvalidStartEnd_Text} {cChar}", CodeLang.ButtonClose_Text);
                    return false;
                }
            }

            // Control of missing start or end guard
            if (cStartEndGuards.Contains(cTextToCode[..1]) && cStartEndGuards.Contains(cTextToCode.Substring(cTextToCode.Length - 1, 1)) == false)
            {
                await Application.Current!.Windows[0].Page!.DisplayAlertAsync(CodeLang.ErrorTitle_Text, CodeLang.GuardMissingEnd_Text, CodeLang.ButtonClose_Text);
                return false;
            }
            else if (cStartEndGuards.Contains(cTextToCode[..1]) == false && cStartEndGuards.Contains(cTextToCode.Substring(cTextToCode.Length - 1, 1)))
            {
                await Application.Current!.Windows[0].Page!.DisplayAlertAsync(CodeLang.ErrorTitle_Text, CodeLang.GuardMissingStart_Text, CodeLang.ButtonClose_Text);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Reverse the characters in a string
        /// </summary>
        /// <param name="cText"></param>
        /// <returns></returns>
        public static string ReverseString(string cText)
        {
            char[] charArray = cText.ToCharArray();
            Array.Reverse(charArray);

            return string.Concat(charArray);
        }

        /// <summary>
        /// Calculate the checksum of an EAN-13, EAN-8 and UPC-A code
        /// </summary>
        /// <param name="cTextToCode"></param>
        /// <returns></returns>
        public static string CalculateChecksumEanUpcA(string cTextToCode)
        {
            int nPos;
            int nPartialSum;
            int nChecksum = 0;
            int nCheckDigit = 0;

            // Loop over string
            for (nPos = 0; nPos < cTextToCode.Length; nPos++)
            {
                if ((nPos + 1) % 2 == 0)
                {
                    nPartialSum = (int)char.GetNumericValue(cTextToCode[nPos]);
                }
                else
                {
                    nPartialSum = (int)char.GetNumericValue(cTextToCode[nPos]) * 3;
                }
                Debug.WriteLine($"{"nPartialSum: "} {nPartialSum}");  // For testing

                nChecksum += nPartialSum;
            }
            Debug.WriteLine($"{"nChecksum: "} {nChecksum}");  // For testing

            int nRemainder = nChecksum % 10;
            if (nRemainder != 0)
            {
                nCheckDigit = 10 - nRemainder;
            }
            Debug.WriteLine($"{"nCheckDigit: "} {nCheckDigit}");  // For testing

            return Convert.ToString(nCheckDigit);
        }
    }
}

/* How to convert a UPC-E code back to UPC-A ?
   A 6-digit UPC-E code is derived from a UPC-A 12-digit code.
   You can convert a UPC-E code back to its UPC-A format using the following scenarios.

   If the UPC-E code ends in 0, 1, or 2:
   Convert the UPC-E back to UPC-A code by picking the first two digits in the UPC-E code.
   Add the last digit (still of using the UPC-E code) and then four zeros(0).
   Complete the conversion by adding characters 3 -5 of your UPC-E code.

   If the UPC-E code ends in 3:
   Determine what the UPC-A code is by picking the first three digits used in your UPC-E code.
   Add five zeros (0), and then characters 4 and 5 of the UPC-E.

   Where the UPC-E code ends in 4:
   Determine the UPC-A code in this way: take the UPC-E code and write out the first four digits.
   Add five zeros (0), then the fifth character of the UPC-E code.

   If the UPC-E code ends in any of 5, 6, 7, 8, or 9:
   Convert the UPC-E code to UPC-A by first picking the leading five digits in the UPC-E code.
   Add four 0 digits and the last character of the UPC-E code.

   Samples:
   UPC-E: 01326901 -> UPC-A: 013000002691
   UPC-E: 01810905 -> UPC-A: 018000001095

   Source: https://bytescout.com/blog/2013/10/upc-and-upc-e-purpose-advantages.html
   _____________________________________________________________________________________________ */