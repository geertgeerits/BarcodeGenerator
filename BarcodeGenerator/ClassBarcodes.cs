namespace BarcodeGenerator
{
    internal class ClassBarcodes
    {
        // Global readonly variables for 1D barcode formats
        public static string cBarcode_CODABAR = string.Empty;

        public static string cBarcode_CODE_39 = string.Empty;
        public static string cBarcode_CODE_39_CHECKSUM_MOD_10 = string.Empty;
        public static string cBarcode_CODE_39_CHECKSUM_MOD_43 = string.Empty;
        public static string cBarcode_CODE_39_FULL_ASCII = string.Empty;
        public static string cBarcode_CODE_39_FULL_ASCII_CHECKSUM = string.Empty;

        public static string cBarcode_CODE_93 = string.Empty;
        public static string cBarcode_CODE_93_I = string.Empty;
        
        public static string cBarcode_CODE_128 = string.Empty;

        public static string cBarcode_DX_FILM_EDGE = string.Empty;              // DX Film Edge

        public static string cBarcode_EAN_2 = string.Empty;                     // European Article Number
        public static string cBarcode_EAN_5 = string.Empty;
        public static string cBarcode_EAN_8 = string.Empty;
        public static string cBarcode_EAN_13 = string.Empty;
        public static string cBarcode_EAN_UPC = string.Empty;

        public static string cBarcode_GS1_DATABAR = string.Empty;               // General Specifications 1
        public static string cBarcode_GS1_DATABAR_EXPANDED = string.Empty;
        public static string cBarcode_GS1_DATABAR_EXPANDED_STACKED = string.Empty;
        public static string cBarcode_GS1_DATABAR_LIMITED = string.Empty;
        public static string cBarcode_GS1_DATABAR_OMNI = string.Empty;
        public static string cBarcode_GS1_DATABAR_STACKED = string.Empty;
        public static string cBarcode_GS1_DATABAR_STACKED_OMNI = string.Empty;
        
        public static string cBarcode_IMB = string.Empty;                       // Intelligent Mail Barcode

        public static string cBarcode_ISBN = string.Empty;                      // International Standard Book Number

        public static string cBarcode_ITF = string.Empty;                       // Interleaved Two of Five
        public static string cBarcode_ITF_14 = string.Empty;
        public static string cBarcode_ITF_CHECKSUM = string.Empty;

        public static string cBarcode_MSI = string.Empty;                       // Modified Plessey

        public static string cBarcode_PHARMACODE = string.Empty;

        public static string cBarcode_PLESSEY = string.Empty;

        public static string cBarcode_PZN = string.Empty;                       // Pharma Central Number

        public static string cBarcode_RSS_14 = string.Empty;                    // Reduce Space Symbology
        public static string cBarcode_RSS_EXPANDED = string.Empty;

        public static string cBarcode_UPC_A = string.Empty;                     // Universal Product Code
        public static string cBarcode_UPC_E = string.Empty;
        public static string cBarcode_UPC_EAN_EXTENSION = string.Empty;

        // Global readonly variables for 2D barcode formats
        public static string cBarcode_AZTEC = string.Empty;
        public static string cBarcode_AZTEC_CODE = string.Empty;
        public static string cBarcode_AZTEC_RUNE = string.Empty;

        public static string cBarcode_DATA_MATRIX = string.Empty;

        public static string cBarcode_MAXICODE = string.Empty;

        public static string cBarcode_PDF_417 = string.Empty;                   // Portable Data File 417
        public static string cBarcode_COMPACT_PDF_417 = string.Empty;  
        public static string cBarcode_MICRO_PDF_417 = string.Empty;

        public static string cBarcode_QR_CODE = string.Empty;                   // Quick Response Code
        public static string cBarcode_QR_CODE_MODEL_1 = string.Empty;
        public static string cBarcode_QR_CODE_MODEL_2 = string.Empty;
        public static string cBarcode_FRAME_QR_CODE = string.Empty;
        public static string cBarcode_MICRO_QR_CODE = string.Empty;
        public static string cBarcode_RMQR = string.Empty;
        public static string cBarcode_QR_CODE_IMAGE = string.Empty;             // CodeLang.Barcode_QR_CODE_IMAGE_Text;

        // Global variables
        public static int nBarcodeGeneratorIndex;
        public static int nBarcodeScannerIndex;
        public static string cBarcodeGeneratorName = string.Empty;
        public static string cBarcodeScannerName = string.Empty;
        public static string cBarcodeGeneratorDefault = cBarcode_QR_CODE;
        public static string cBarcodeScannerDefault = string.Empty;

        /// <summary>
        /// Initialize the barcode formats by assigning the values from the resource file to the global readonly variables
        /// This method can be called in the constructor of the main page or in the startup class of the application
        /// </summary>
        public static void InitializeBarcodeFormats()
        {
            // Global readonly variables for 1D barcode formats
            cBarcode_CODABAR = CodeLang.Barcode_CODABAR_Text;

            cBarcode_CODE_39 = CodeLang.Barcode_CODE_39_Text;
            cBarcode_CODE_39_CHECKSUM_MOD_10 = CodeLang.Barcode_CODE_39_CHECKSUM_MOD_10_Text;
            cBarcode_CODE_39_CHECKSUM_MOD_43 = CodeLang.Barcode_CODE_39_CHECKSUM_MOD_43_Text;
            cBarcode_CODE_39_FULL_ASCII = CodeLang.Barcode_CODE_39_FULL_ASCII_Text;
            cBarcode_CODE_39_FULL_ASCII_CHECKSUM = CodeLang.Barcode_CODE_39_FULL_ASCII_CHECKSUM_Text;

            cBarcode_CODE_93 = CodeLang.Barcode_CODE_93_Text;
            cBarcode_CODE_93_I = CodeLang.Barcode_CODE_93_I_Text;

            cBarcode_CODE_128 = CodeLang.Barcode_CODE_128_Text;

            cBarcode_DX_FILM_EDGE = CodeLang.Barcode_DX_FILM_EDGE_Text;     // DX Film Edge
            cBarcode_EAN_2 = CodeLang.Barcode_EAN_2_Text;                   // European Article Number
            cBarcode_EAN_5 = CodeLang.Barcode_EAN_5_Text;
            cBarcode_EAN_8 = CodeLang.Barcode_EAN_8_Text;
            cBarcode_EAN_13 = CodeLang.Barcode_EAN_13_Text;
            cBarcode_EAN_UPC = CodeLang.Barcode_EAN_UPC_Text;

            cBarcode_GS1_DATABAR = CodeLang.Barcode_GS1_DATABAR_Text;       // General Specifications 1
            cBarcode_GS1_DATABAR_EXPANDED = CodeLang.Barcode_GS1_DATABAR_EXPANDED_Text;
            cBarcode_GS1_DATABAR_EXPANDED_STACKED = CodeLang.Barcode_GS1_DATABAR_EXPANDED_STACKED_Text;
            cBarcode_GS1_DATABAR_LIMITED = CodeLang.Barcode_GS1_DATABAR_LIMITED_Text;
            cBarcode_GS1_DATABAR_OMNI = CodeLang.Barcode_GS1_DATABAR_OMNI_Text;
            cBarcode_GS1_DATABAR_STACKED = CodeLang.Barcode_GS1_DATABAR_STACKED_Text;
            cBarcode_GS1_DATABAR_STACKED_OMNI = CodeLang.Barcode_GS1_DATABAR_STACKED_OMNI_Text;

            cBarcode_IMB = CodeLang.Barcode_IMB_Text;                       // Intelligent Mail Barcode

            cBarcode_ISBN = CodeLang.Barcode_ISBN_Text;                     // International Standard Book Number
            cBarcode_ITF = CodeLang.Barcode_ITF_Text;                       // Interleaved Two of Five
            cBarcode_ITF_14 = CodeLang.Barcode_ITF_14_Text;
            cBarcode_ITF_CHECKSUM = CodeLang.Barcode_ITF_CHECKSUM_Text;

            cBarcode_MSI = CodeLang.Barcode_MSI_Text;                       // Modified Plessey

            cBarcode_PHARMACODE = CodeLang.Barcode_PHARMACODE_Text;
            cBarcode_PLESSEY = CodeLang.Barcode_PLESSEY_Text;

            cBarcode_PZN = CodeLang.Barcode_PZN_Text;                       // Pharma Central Number

            cBarcode_RSS_14 = CodeLang.Barcode_RSS_14_Text;                 // Reduce Space Symbology
            cBarcode_RSS_EXPANDED = CodeLang.Barcode_RSS_EXPANDED_Text;
            cBarcode_UPC_A = CodeLang.Barcode_UPC_A_Text;                   // Universal Product Code
            cBarcode_UPC_E = CodeLang.Barcode_UPC_E_Text;
            cBarcode_UPC_EAN_EXTENSION = CodeLang.Barcode_UPC_EAN_EXTENSION_Text;

            // Global readonly variables for 2D barcode formats
            cBarcode_AZTEC = CodeLang.Barcode_AZTEC_Text;
            cBarcode_AZTEC_CODE = CodeLang.Barcode_AZTEC_CODE_Text;
            cBarcode_AZTEC_RUNE = CodeLang.Barcode_AZTEC_RUNE_Text;

            cBarcode_DATA_MATRIX = CodeLang.Barcode_DATA_MATRIX_Text;

            cBarcode_MAXICODE = CodeLang.Barcode_MAXICODE_Text;

            cBarcode_PDF_417 = CodeLang.Barcode_PDF_417_Text;               // Portable Data File 417
            cBarcode_COMPACT_PDF_417 = CodeLang.Barcode_COMPACT_PDF_417_Text;
            cBarcode_MICRO_PDF_417 = CodeLang.Barcode_MICRO_PDF_417_Text;

            cBarcode_QR_CODE = CodeLang.Barcode_QR_CODE_Text;               // Quick Response Code
            cBarcode_QR_CODE_MODEL_1 = CodeLang.Barcode_QR_CODE_MODEL_1_Text;
            cBarcode_QR_CODE_MODEL_2 = CodeLang.Barcode_QR_CODE_MODEL_2_Text;
            cBarcode_FRAME_QR_CODE = CodeLang.Barcode_FRAME_QR_CODE_Text;
            cBarcode_MICRO_QR_CODE = CodeLang.Barcode_MICRO_QR_CODE_Text;
            cBarcode_RMQR = CodeLang.Barcode_RMQR_Text;
            cBarcode_QR_CODE_IMAGE = CodeLang.Barcode_QR_CODE_IMAGE_Text;

            // Default values for the barcode generator and scanner
            cBarcodeGeneratorDefault = cBarcode_QR_CODE;
            cBarcodeScannerDefault = CodeLang.Barcode_AllCodes_Text;
        }

        /// <summary>
        /// Put the generator barcode formats in a List string using the ZXing library for all platforms
        /// </summary>
        /// <returns></returns>
        public static List<string> GetFormatCodeListGenerator_ZX() => [.. new List<string>
            {
                CodeLang.Barcode_AZTEC_Text,
                CodeLang.Barcode_CODABAR_Text,
                CodeLang.Barcode_CODE_39_Text,
                CodeLang.Barcode_CODE_93_Text,
                CodeLang.Barcode_CODE_128_Text,
                CodeLang.Barcode_DATA_MATRIX_Text,
                CodeLang.Barcode_EAN_8_Text,
                CodeLang.Barcode_EAN_13_Text,
                CodeLang.Barcode_ITF_Text,
                CodeLang.Barcode_MSI_Text,
                CodeLang.Barcode_PDF_417_Text,
                CodeLang.Barcode_PLESSEY_Text,
                CodeLang.Barcode_QR_CODE_Text,
                CodeLang.Barcode_QR_CODE_IMAGE_Text,
                CodeLang.Barcode_UPC_A_Text,
                CodeLang.Barcode_UPC_E_Text
            }
            .OrderBy(x => x, StringComparer.OrdinalIgnoreCase)];

        /// <summary>
        /// Put the generator barcode formats in a List string using the ZXing library for Windows
        /// </summary>
        /// <returns></returns>
        public static List<string> GetFormatCodeListGenerator_ZX_Windows() => [.. new List<string>
            {
                CodeLang.Barcode_AZTEC_Text,
                //CodeLang.Barcode_CODE_93_Text,
                CodeLang.Barcode_DATA_MATRIX_Text,
                CodeLang.Barcode_PDF_417_Text,
                CodeLang.Barcode_QR_CODE_Text,
                CodeLang.Barcode_QR_CODE_IMAGE_Text
                //CodeLang.Barcode_UPC_E_Text
            }
            .OrderBy(x => x, StringComparer.OrdinalIgnoreCase)];

        /// <summary>
        /// Put the scanner barcode formats in a List string using the ZXing library for all platforms
        /// </summary>
        /// <returns></returns>
        public static List<string> GetFormatCodeListScanner_ZX() => [.. new List<string>
            {
                CodeLang.Barcode_AllCodes_Text,
                CodeLang.Barcode_AZTEC_Text,
                CodeLang.Barcode_CODABAR_Text,
                CodeLang.Barcode_CODE_39_Text,
                CodeLang.Barcode_CODE_93_Text,
                CodeLang.Barcode_CODE_128_Text,
                CodeLang.Barcode_DATA_MATRIX_Text,
                CodeLang.Barcode_EAN_8_Text,
                CodeLang.Barcode_EAN_13_Text,
                CodeLang.Barcode_IMB_Text,
                CodeLang.Barcode_ITF_Text,
                CodeLang.Barcode_MAXICODE_Text,
                CodeLang.Barcode_MSI_Text,
                CodeLang.Barcode_PDF_417_Text,
                CodeLang.Barcode_PHARMACODE_Text,
                CodeLang.Barcode_PLESSEY_Text,
                CodeLang.Barcode_QR_CODE_Text,
                CodeLang.Barcode_RSS_14_Text,
                CodeLang.Barcode_RSS_EXPANDED_Text,
                CodeLang.Barcode_UPC_A_Text,
                CodeLang.Barcode_UPC_E_Text,
                CodeLang.Barcode_UPC_EAN_EXTENSION_Text
            }
            .OrderBy(x => x, StringComparer.OrdinalIgnoreCase)];

        /// <summary>
        /// Put the scanner barcode formats in a List string using the native library for Android
        /// https://developers.google.com/ml-kit/vision/barcode-scanning/android
        /// </summary>
        /// <returns></returns>
        public static List<string> GetFormatCodeListScanner_NT_Android() => [.. new List<string>
            {
                CodeLang.Barcode_AllCodes_Text,
                CodeLang.Barcode_AZTEC_Text,
                CodeLang.Barcode_CODABAR_Text,
                CodeLang.Barcode_CODE_39_Text,
                CodeLang.Barcode_CODE_93_Text,
                CodeLang.Barcode_CODE_128_Text,
                CodeLang.Barcode_DATA_MATRIX_Text,
                CodeLang.Barcode_EAN_8_Text,
                CodeLang.Barcode_EAN_13_Text,
                CodeLang.Barcode_ITF_Text,
                CodeLang.Barcode_PDF_417_Text,
                CodeLang.Barcode_QR_CODE_Text,
                CodeLang.Barcode_UPC_A_Text,
                CodeLang.Barcode_UPC_E_Text
            }
            .OrderBy(x => x, StringComparer.OrdinalIgnoreCase)];

        /// <summary>
        /// Put the scanner barcode formats in a List string using the native library for iOS
        /// https://developer.apple.com/documentation/avfoundation/avmetadataobject/objecttype
        /// </summary>
        /// <returns></returns>
        public static List<string> GetFormatCodeListScanner_NT_IOS() => [.. new List<string>
            {
                CodeLang.Barcode_AllCodes_Text,
                CodeLang.Barcode_AZTEC_Text,
                CodeLang.Barcode_CODABAR_Text,
                CodeLang.Barcode_CODE_39_Text,
                CodeLang.Barcode_CODE_93_Text,
                CodeLang.Barcode_CODE_128_Text,
                CodeLang.Barcode_DATA_MATRIX_Text,
                CodeLang.Barcode_EAN_8_Text,
                CodeLang.Barcode_EAN_13_Text,
                CodeLang.Barcode_GS1_DATABAR_Text,
                CodeLang.Barcode_ITF_Text,
                CodeLang.Barcode_MICRO_PDF_417_Text,
                CodeLang.Barcode_MICRO_QR_CODE_Text,
                CodeLang.Barcode_PDF_417_Text,
                CodeLang.Barcode_QR_CODE_Text,
                CodeLang.Barcode_UPC_A_Text,
                CodeLang.Barcode_UPC_E_Text
            }
            .OrderBy(x => x, StringComparer.OrdinalIgnoreCase)];

        /// <summary>
        /// Put the scanner barcode formats in a List string using the native library for Windows
        /// </summary>
        /// <returns></returns>
        public static List<string> GetFormatCodeListScanner_NT_Windows() => [.. new List<string>
            {
                CodeLang.Barcode_AllCodes_Text,
                CodeLang.Barcode_AZTEC_Text,
                CodeLang.Barcode_AZTEC_CODE_Text,
                CodeLang.Barcode_AZTEC_RUNE_Text,
                CodeLang.Barcode_CODABAR_Text,
                CodeLang.Barcode_CODE_39_Text,
                CodeLang.Barcode_CODE_93_Text,
                CodeLang.Barcode_CODE_128_Text,
                CodeLang.Barcode_COMPACT_PDF_417_Text,
                CodeLang.Barcode_DATA_MATRIX_Text,
                CodeLang.Barcode_DX_FILM_EDGE_Text,
                CodeLang.Barcode_EAN_2_Text,
                CodeLang.Barcode_EAN_5_Text,
                CodeLang.Barcode_EAN_8_Text,
                CodeLang.Barcode_EAN_13_Text,
                CodeLang.Barcode_EAN_UPC_Text,
                CodeLang.Barcode_GS1_DATABAR_Text,
                CodeLang.Barcode_GS1_DATABAR_EXPANDED_Text,
                CodeLang.Barcode_GS1_DATABAR_EXPANDED_STACKED_Text,
                CodeLang.Barcode_GS1_DATABAR_LIMITED_Text,
                CodeLang.Barcode_GS1_DATABAR_OMNI_Text,
                CodeLang.Barcode_GS1_DATABAR_STACKED_Text,
                CodeLang.Barcode_GS1_DATABAR_STACKED_OMNI_Text,
                CodeLang.Barcode_ISBN_Text,
                CodeLang.Barcode_ITF_Text,
                CodeLang.Barcode_ITF_14_Text,
                CodeLang.Barcode_MAXICODE_Text,
                CodeLang.Barcode_MICRO_PDF_417_Text,
                CodeLang.Barcode_MICRO_QR_CODE_Text,
                CodeLang.Barcode_PDF_417_Text,
                CodeLang.Barcode_PZN_Text,
                CodeLang.Barcode_QR_CODE_Text,
                CodeLang.Barcode_QR_CODE_MODEL_1_Text,
                CodeLang.Barcode_QR_CODE_MODEL_2_Text,
                CodeLang.Barcode_RMQR_Text,
                CodeLang.Barcode_UPC_A_Text,
                CodeLang.Barcode_UPC_E_Text
            }
            .OrderBy(x => x, StringComparer.OrdinalIgnoreCase)];

        /// <summary>
        /// Select the name and index in the generator barcode list
        /// </summary>
        /// <param name="picker"></param>
        public static void SelectBarcodeGeneratorNameIndex(Picker picker)
        {
            // If there is no saved barcode generator name then set it to the default barcode generator name
            if (string.IsNullOrEmpty(cBarcodeGeneratorName))
            {
                cBarcodeGeneratorName = cBarcodeGeneratorDefault;
            }

            // Search for the name of the barcode in the picker list
            nBarcodeGeneratorIndex = picker.Items.IndexOf(cBarcodeGeneratorName);

            // If the barcode name was not found in the list then set it to the default name
            if (nBarcodeGeneratorIndex < 0)
            {
                cBarcodeGeneratorName = cBarcodeGeneratorDefault;
                
                // Search for the name of the barcode in the picker list
                nBarcodeGeneratorIndex = picker.Items.IndexOf(cBarcodeGeneratorName);
            }

            // If the barcode name was not found in the list then set it to the first barcode name
            if (nBarcodeGeneratorIndex < 0)
            {
                nBarcodeGeneratorIndex = 0;
                cBarcodeGeneratorName = picker.Items[nBarcodeGeneratorIndex];
            }
            
            // Select the barcode format in the picker
            picker.SelectedIndex = nBarcodeGeneratorIndex;
        }

        /// <summary>
        /// Select the name and index in the scanner barcode list
        /// </summary>
        /// <param name="picker"></param>
        public static void SelectBarcodeScannerNameIndex(Picker picker)
        {
            // If there is no saved barcode scanner name then set it to the default barcode scanner name
            if (string.IsNullOrEmpty(cBarcodeScannerName))
            {
                cBarcodeScannerName = cBarcodeScannerDefault;
            }

            // Search for the name of the barcode in the picker list
            nBarcodeScannerIndex = picker.Items.IndexOf(cBarcodeScannerName);

            // If the barcode name was not found in the list then set it to the default name
            if (nBarcodeScannerIndex < 0)
            {
                cBarcodeScannerName = cBarcodeScannerDefault;

                // Search for the name of the barcode in the picker list
                nBarcodeScannerIndex = picker.Items.IndexOf(cBarcodeScannerName);
            }

            // If the barcode name was not found in the list then set it to the first barcode name
            if (nBarcodeScannerIndex < 0)
            {
                nBarcodeScannerIndex = 0;
                cBarcodeScannerName = picker.Items[nBarcodeScannerIndex];
            }

            // Select the barcode format in the picker
            picker.SelectedIndex = nBarcodeScannerIndex;
        }
    }
}

/* 1D barcode formats and maximum length of the text to encode

                      edtTextToCode    Numeric      Alpha    Binary     Kanji/Kana
                          MaxLength               Numeric    8-bits     JIS X 0208
                      -------------    -------    -------    -------    ----------
   Codabar                       43         43         43

   Code 39                       48         48         48
   Code 39 Mod 10
   Code 39 Mod 43
   Code 39 Full ASCII
   Code 39 Full ASCII Checksum
   
   Code 93                       48          48        48
   Code 93i

   Code 128                      48          48        48

   DX Film Edge
   
   EAN-2                          2           2
   EAN-5                          5           5
   EAN-8                          8           8
   EAN-13                        13          13
   EAN/UPC                       13          13

   GS1 DataBar          
   GS1 DataBar Expanded
   GS1 Databar Expanded Stacked
   GS1 DataBar Limited
   GS1 Databar Omni
   GS1 Databar Stacked
   GS1 Databar Stacked Omni

   Imb Intelligent Mail          31          31

   ISBN
   
   ITF                           30          30
   ITF 14               
   ITF Checksum

   MSI                          255         255

   Pharmacode                     6           6

   Plessey                       16          16        16

   PZN Pharma Central Number      8           8

   Rss 14                        14          14
   Rss Expanded                  74          74

   UPC-A                         12          12
   UPC-E                          8           8
   UPC-EAN Extension              2           2


   2D barcode formats and maximum length of the text to encode.

                      edtTextToCode    Numeric      Alpha    Binary     Kanji/Kana
                          MaxLength               Numeric    8-bits     JIS X 0208
                      -------------    -------    -------    -------    ----------
   Aztec                       1900       3832       3067       1914
   Aztec Code
   Aztec Rune
   
   Data Matrix                 1500       3116       2335       1555                     

   MaxiCode                      93         93

   Pdf 417                     1100       2710       1850       1108
   Compact PDF417
   Micro PDF 417                366        250        150

   QR Code                     1800       7089       4296       2953          1817
   QR Code Model 1
   QR Code Model 2
   Frame QR
   Micro QR                      35         21         15
   rMQR Rectangular Micro QR
   QR Code + Image             1800       7089       4296       1273          1817
   _____________________________________________________________________________________________ */

