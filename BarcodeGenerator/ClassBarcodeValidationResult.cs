namespace BarcodeGenerator
{
    public sealed class ClassBarcodeValidationResult
    {
        public bool Success { get; set; }
        public string Text { get; set; } = string.Empty;        // Possibly modified text (checksums, replacements)
        public string Caption { get; set; } = string.Empty;     // Caption to show under barcode
        public string? ErrorMessage { get; set; }               // If not null -> an error occurred
    }

    public sealed class BarcodeValidationService
    {
        private const string cAllowedCharactersDecimal = "0123456789";
        private const string cAllowedCharactersHex = "0123456789ABCDEF";
        private const string cAllowedCharactersCode39_93 = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789 -.$/+%*";
        private const string cAllowedCharactersCodabar = "0123456789-$:/.+ABCD";

        public static async Task<ClassBarcodeValidationResult> ValidateAsync(string selectedName, string cTextToCode, int nLenTextToCode)
        {
            ClassBarcodeValidationResult result = new()
            {
                Success = false,
                Text = cTextToCode,
                Caption = string.Empty
            };

            try
            {
                string cChecksum = string.Empty;

                // Validate 1D barcodes
                if (selectedName == ClassBarcodes.cBarcode_CODABAR)
                {
                    cTextToCode = cTextToCode.ToUpper();

                    if (!await TestAllowedCharacters(cAllowedCharactersCodabar, cTextToCode))
                        return result;

                    if (!await TestStartEndGuards("ABCD", cTextToCode))
                        return result;

                    result.Text = cTextToCode;
                    result.Caption = cTextToCode;
                }
                else if (selectedName == ClassBarcodes.cBarcode_CODE_39 || selectedName == ClassBarcodes.cBarcode_CODE_93)
                {
                    cTextToCode = cTextToCode.ToUpper();

                    if (!await TestAllowedCharacters(cAllowedCharactersCode39_93, cTextToCode))
                        return result;

                    if (!await TestStartEndGuards("*", cTextToCode))
                        return result;

                    result.Text = cTextToCode;
                    result.Caption = cTextToCode;
                }
                else if (selectedName == ClassBarcodes.cBarcode_CODE_128)
                {
                    cTextToCode = ReplaceCharacters(cTextToCode);

                    if (!await TestAllowedAsciiValues(1, 127, cTextToCode))
                        return result;

                    result.Text = cTextToCode;
                    result.Caption = cTextToCode;
                }
                else if (selectedName == ClassBarcodes.cBarcode_EAN_8)
                {
                    if (!await TestAllowedCharacters(cAllowedCharactersDecimal, cTextToCode))
                        return result;

                    if (nLenTextToCode < 7 || nLenTextToCode > 8)
                    {
                        result.ErrorMessage = $"{CodeLang.CodeLengthPart1_Text} 7 {CodeLang.CodeLengthPart2_Text} 8 {CodeLang.CodeLengthPart3_Text}";
                        return result;
                    }

                    if (nLenTextToCode == 8)
                        cChecksum = cTextToCode.Substring(7, 1);

                    cTextToCode = cTextToCode[..7];
                    cTextToCode += CalculateChecksumEanUpcA(ReverseString(cTextToCode));

                    if (nLenTextToCode == 8 && cChecksum != cTextToCode.Substring(7, 1))
                        result.ErrorMessage = CodeLang.CheckDigitError_Text;

                    result.Text = cTextToCode;
                    result.Caption = InsertCharacterInCaption(cTextToCode, 4);
                }
                else if (selectedName == ClassBarcodes.cBarcode_EAN_13)
                {
                    if (!await TestAllowedCharacters(cAllowedCharactersDecimal, cTextToCode))
                        return result;

                    if (nLenTextToCode < 12 || nLenTextToCode > 13)
                    {
                        result.ErrorMessage = $"{CodeLang.CodeLengthPart1_Text} 12 {CodeLang.CodeLengthPart2_Text} 13 {CodeLang.CodeLengthPart3_Text}";
                        return result;
                    }

                    if (nLenTextToCode == 13)
                        cChecksum = cTextToCode.Substring(12, 1);

                    cTextToCode = cTextToCode[..12];
                    cTextToCode += CalculateChecksumEanUpcA(ReverseString(cTextToCode));

                    if (nLenTextToCode == 13 && cChecksum != cTextToCode.Substring(12, 1))
                        result.ErrorMessage = CodeLang.CheckDigitError_Text;

                    result.Text = cTextToCode;
                    result.Caption = InsertCharacterInCaption(cTextToCode, 7);
                    result.Caption = InsertCharacterInCaption(result.Caption, 1);
                }
                else if (selectedName == ClassBarcodes.cBarcode_ITF)
                {
                    if (!await TestAllowedCharacters(cAllowedCharactersDecimal, cTextToCode))
                        return result;

                    if (nLenTextToCode % 2 != 0)
                    {
                        result.ErrorMessage = CodeLang.LengthInputEven_Text;
                        return result;
                    }

                    result.Text = cTextToCode;
                    result.Caption = cTextToCode;
                }
                else if (selectedName == ClassBarcodes.cBarcode_MSI)
                {
                    if (!await TestAllowedCharacters(cAllowedCharactersDecimal, cTextToCode))
                        return result;

                    result.Text = cTextToCode;
                    result.Caption = cTextToCode;
                }
                else if (selectedName == ClassBarcodes.cBarcode_PLESSEY)
                {
                    cTextToCode = cTextToCode.ToUpper();

                    if (!await TestAllowedCharacters(cAllowedCharactersHex, cTextToCode))
                        return result;

                    result.Text = cTextToCode;
                    result.Caption = cTextToCode;
                }
                else if (selectedName == ClassBarcodes.cBarcode_UPC_A)
                {
                    if (!await TestAllowedCharacters(cAllowedCharactersDecimal, cTextToCode))
                        return result;

                    if (nLenTextToCode < 11 || nLenTextToCode > 12)
                    {
                        result.ErrorMessage = $"{CodeLang.CodeLengthPart1_Text} 11 {CodeLang.CodeLengthPart2_Text} 12 {CodeLang.CodeLengthPart3_Text}";
                        return result;
                    }

                    if (nLenTextToCode == 12)
                        cChecksum = cTextToCode.Substring(11, 1);

                    cTextToCode = cTextToCode[..11];
                    cTextToCode += CalculateChecksumEanUpcA(cTextToCode);

                    if (nLenTextToCode == 12 && cChecksum != cTextToCode.Substring(11, 1))
                        result.ErrorMessage = CodeLang.CheckDigitError_Text;

                    result.Text = cTextToCode;
                    result.Caption = InsertCharacterInCaption(cTextToCode, 11);
                    result.Caption = InsertCharacterInCaption(result.Caption, 6);
                    result.Caption = InsertCharacterInCaption(result.Caption, 1);
                }
                else if (selectedName == ClassBarcodes.cBarcode_UPC_E)
                {
                    if (!await TestAllowedCharacters(cAllowedCharactersDecimal, cTextToCode))
                        return result;

                    if (nLenTextToCode < 7 || nLenTextToCode > 8)
                    {
                        result.ErrorMessage = $"{CodeLang.CodeLengthPart1_Text} 7 {CodeLang.CodeLengthPart2_Text} 8 {CodeLang.CodeLengthPart3_Text}";
                        return result;
                    }

                    if (cTextToCode[..1] != "0")
                    {
                        result.ErrorMessage = CodeLang.FirstNumber0_Text;
                        return result;
                    }

                    if (nLenTextToCode == 8)
                        cChecksum = cTextToCode.Substring(7, 1);

                    string cUpcA = ConvertUpcEToUpcA(cTextToCode);
                    cUpcA += CalculateChecksumEanUpcA(cUpcA);

                    cTextToCode = string.Concat(cTextToCode.AsSpan(0, 7), cUpcA.AsSpan(11, 1));

                    if (nLenTextToCode == 8 && cChecksum != cTextToCode.Substring(7, 1))
                        result.ErrorMessage = CodeLang.CheckDigitError_Text;

                    result.Text = cTextToCode;
                    result.Caption = InsertCharacterInCaption(cTextToCode, 7);
                    result.Caption = InsertCharacterInCaption(result.Caption, 1);
                }

                // Validate 2D barcodes
                else if (selectedName == ClassBarcodes.cBarcode_AZTEC)
                {
                    cTextToCode = ReplaceCharacters(cTextToCode);

                    if (!await TestAllowedAsciiValues(1, 255, cTextToCode))
                        return result;

                    result.Text = cTextToCode;
                }
                else if (selectedName == ClassBarcodes.cBarcode_DATA_MATRIX)
                {
                    cTextToCode = ReplaceCharacters(cTextToCode);

                    if (!await TestAllowedAsciiValues(1, 255, cTextToCode))
                        return result;

                    result.Text = cTextToCode;
                }
                else if (selectedName == ClassBarcodes.cBarcode_PDF_417)
                {
                    cTextToCode = ReplaceCharacters(cTextToCode);

                    if (!await TestAllowedAsciiValues(1, 255, cTextToCode))
                        return result;

                    result.Text = cTextToCode;
                }
                else if (selectedName == ClassBarcodes.cBarcode_QR_CODE)
                {
                    if (!await CheckValidateTextAsync(cTextToCode, 3993, 2420, 1663, 1024))
                        return result;

                    result.Text = cTextToCode;
                }
                else if (selectedName == ClassBarcodes.cBarcode_QR_CODE_IMAGE || selectedName == ClassBarcodes.cBarcode_ART_QR_CODE)
                {
                    if (!await CheckValidateTextAsync(cTextToCode, 3057, 1852, 1273, 784))
                        return result;

                    result.Text = cTextToCode;
                }
                else if (selectedName == ClassBarcodes.cBarcode_MICRO_QR_CODE)
                {
                    if (!await CheckValidateTextAsync(cTextToCode, 35, 21, 15, 9))
                        return result;

                    result.Text = cTextToCode;
                }

                // If no error message was set above, success
                if (string.IsNullOrEmpty(result.ErrorMessage))
                {
                    result.Success = true;
                    // keep updated text and caption already set
                }

                return result;
            }
            catch (Exception)
            {
                // Do not propagate exceptions for UI code; caller will handle a failed result
                return result;
            }
        }

        /// <summary>
        /// Test for allowed characters
        /// </summary>
        /// <param name="cAllowedCharacters"></param>
        /// <param name="cTextToCode"></param>
        /// <returns></returns>
        private static async Task<bool> TestAllowedCharacters(string cAllowedCharacters, string cTextToCode)
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
        private static async Task<bool> TestAllowedAsciiValues(int nMinAsciiValue, int nMaxAsciiValue, string cTextToCode)
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
        private static string ReplaceCharacters(string cText)
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
        private static async Task<bool> TestStartEndGuards(string cStartEndGuards, string cTextToCode)
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
        private static string ReverseString(string cText)
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
        private static string CalculateChecksumEanUpcA(string cTextToCode)
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

        /// <summary>
        /// Convert a UPC-E code back to its UPC-A format
        /// </summary>
        /// <param name="cUpcE"></param>
        /// <returns></returns>
        private static string ConvertUpcEToUpcA(string cUpcE)
        {
            cUpcE = cUpcE.Substring(1, 6);
            string cLastDigit = cUpcE.Substring(cUpcE.Length - 1, 1);
            int nLastDigit = Convert.ToInt32(cLastDigit);
            string cUpcA;

            if (nLastDigit < 3)
            {
                cUpcA = string.Concat(cUpcE.AsSpan(0, 2), cLastDigit, "0000", cUpcE.AsSpan(2, 3));
            }
            else if (nLastDigit == 3)
            {
                cUpcA = string.Concat(cUpcE.AsSpan(0, 3), "00000", cUpcE.AsSpan(3, 2));
            }
            else if (nLastDigit == 4)
            {
                cUpcA = string.Concat(cUpcE.AsSpan(0, 4), "00000", cUpcE.AsSpan(4, 1));
            }
            else
            {
                cUpcA = string.Concat(cUpcE.AsSpan(0, 5), "0000", cLastDigit);
            }

            return $"0{cUpcA}";
        }

        /// <summary>
        /// Asynchronously validates the input text against code character count limits for each encoding mode.
        /// </summary>
        /// <remarks>If the input text exceeds the character limit for its detected code mode, an alert
        /// dialog is displayed to the user and the method returns <see langword="false"/>. This method is intended to
        /// be used before encoding text into a code to ensure compatibility with the specified mode
        /// limits.</remarks>
        /// <param name="cText">The text to be validated for code encoding. The detected mode of this text determines which character
        /// limit applies.</param>
        /// <param name="nNumeric">The maximum allowed number of characters for the Numeric mode</param>
        /// <param name="nAlphanumeric">The maximum allowed number of characters for the Alphanumeric mode</param>
        /// <param name="nByte">The maximum allowed number of characters for the Byte mode</param>
        /// <param name="nKanji">The maximum allowed number of characters for the Kanji mode</param>
        /// <returns>A task that represents the asynchronous operation. The task result is <see langword="true"/> if the input
        /// text does not exceed the character limit for its detected mode; otherwise, <see langword="false"/>.</returns>
        private static async Task<bool> CheckValidateTextAsync(string cText, int nNumeric, int nAlphanumeric, int nByte, int nKanji)
        {
            // Check input text length against code limits based on detected mode and error correction level,
            // and show an alert if it exceeds the limits            

            string cMessage;

            switch (ClassQrModeDetector.Detect(cText))
            {
                case ClassQrModeDetector.Mode.Numeric when cText.Length > nNumeric:
                    cMessage = $"{CodeLang.CharacterNumericDetected_Text}\n{string.Format(CodeLang.TextLengthLimitedCharacter_Text, nNumeric.ToString("N0", CultureInfo.CurrentCulture))}";
                    await DisplayErrorMessage(cMessage);
                    return false;
                case ClassQrModeDetector.Mode.Alphanumeric when cText.Length > nAlphanumeric:
                    cMessage = $"{CodeLang.CharacterAlphanumericDetected_Text}\n{string.Format(CodeLang.TextLengthLimitedCharacter_Text, nAlphanumeric.ToString("N0", CultureInfo.CurrentCulture))}";
                    await DisplayErrorMessage(cMessage);
                    return false;
                case ClassQrModeDetector.Mode.Byte when cText.Length > nByte:
                    cMessage = $"{CodeLang.CharacterBinaryByteDetected_Text}\n{string.Format(CodeLang.TextLengthLimitedByte_Text, nByte.ToString("N0", CultureInfo.CurrentCulture))}";
                    await DisplayErrorMessage(cMessage);
                    return false;
                case ClassQrModeDetector.Mode.Kanji when cText.Length > nKanji:
                    cMessage = $"{CodeLang.CharacterKanjiKanaDetected_Text}\n{string.Format(CodeLang.TextLengthLimitedCharacter_Text, nKanji.ToString("N0", CultureInfo.CurrentCulture))}";
                    await DisplayErrorMessage(cMessage);
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Insert a character in a caption at a specified position. If the position is out of bounds, the original caption is returned.
        /// </summary>
        /// <param name="cCaption"></param>
        /// <param name="nPosition"></param>
        /// <param name="cCharacter"></param>
        /// <returns></returns>
        private static string InsertCharacterInCaption(string cCaption, int nPosition, string cCharacter = " ")
        {
            if (cCaption == null || nPosition < 0 || nPosition > cCaption.Length)
            {
                return cCaption ?? string.Empty;
            }

            return cCaption.Insert(nPosition, cCharacter);
        }

        /// <summary>
        /// Display an error message
        /// </summary>
        /// <param name="cMessage"></param>
        private static async Task DisplayErrorMessage(string cMessage)
        {
            await Application.Current!.Windows[0].Page!.DisplayAlertAsync(CodeLang.ErrorTitle_Text, cMessage, CodeLang.ButtonClose_Text);
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