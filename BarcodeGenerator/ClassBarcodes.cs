namespace BarcodeGenerator
{
    internal class ClassBarcodes
    {
        // Global variables for 1D barcode formats
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

        // Global variables for 2D barcode formats
        public static string cBarcode_AZTEC = string.Empty;
        public static string cBarcode_AZTEC_CODE = string.Empty;
        public static string cBarcode_AZTEC_RUNE = string.Empty;

        public static string cBarcode_DATA_MATRIX = string.Empty;

        public static string cBarcode_MAXICODE = string.Empty;

        public static string cBarcode_PDF_417 = string.Empty;                   // Portable Data File 417
        public static string cBarcode_COMPACT_PDF_417 = string.Empty;
        public static string cBarcode_MICRO_PDF_417 = string.Empty;

        public static string cBarcode_QR_CODE = string.Empty;                   // Quick Response Code Model 2
        public static string cBarcode_QR_CODE_MODEL_1 = string.Empty;           // QR Code Model 1
        public static string cBarcode_QR_CODE_MODEL_2 = string.Empty;           // QR Code Model 2
        public static string cBarcode_FRAME_QR_CODE = string.Empty;             // Frame QR Code
        public static string cBarcode_MICRO_QR_CODE = string.Empty;             // Micro QR Code symbol version -4 (M4) with error correction level L, M, or Q
        public static string cBarcode_RMQR = string.Empty;                      // Rectangular Micro QR Code
        public static string cBarcode_SQRC = string.Empty;                      // Secure Quick Response Code
        public static string cBarcode_QR_CODE_IMAGE = string.Empty;             // QR Code with image in the center
        public static string cBarcode_ART_QR_CODE = string.Empty;               // Artistic QR Code

        // Global variables
        public static int nBarcodeGeneratorIndex;
        public static int nBarcodeScannerIndex;
        public static string cBarcodeGeneratorName = string.Empty;
        public static string cBarcodeScannerName = string.Empty;
        public static string cBarcodeGeneratorDefault = string.Empty;
        public static string cBarcodeScannerDefault = string.Empty;

        // Global variables to control the QR code and image
        public static string cQRCodeType = string.Empty;                // QR code type: QR Code Model 1, QR Code Model 2, Frame QR Code, Micro QR Code, RMQR, SQRC, QR Code with image in the center or Artistic QR Code
        public static bool bQRCodeSizeVariable;                         // Control whether the size of the QR code is variable or fixed
        public static int nQRCodeSizePixels;                            // Size of the QR code in pixels (if the size is fixed)
        public static float nQRCodeImageSizePercent;                    // Size of the QR code image as a percentage
        public static string cQRCodeFinderPatternShape = string.Empty;  // Shape of the QR code finder pattern shapes
        public static string cQRCodeModuleShape = string.Empty;         // Shape of the QR code modules
        public static string cCodeColor = string.Empty;                 // Color of the barcode (text and bars) for the popup color picker
        public static string cCodeColorFg = string.Empty;               // Foreground color of the barcode (text and bars)
        public static string cCodeColorBg = string.Empty;               // Background color of the barcode
        public static string cCodeColorFgArtQRCode = string.Empty;      // Foreground color of the art image
        public static string cCodeColorBgArtQRCode = string.Empty;      // Background color of the art image
        public static bool bQRCodeGradientColor;                        // Flag to indicate if the QR code has a gradient
        public static string cQRCodeGradientColor1 = string.Empty;      // First color of the QR code gradient
        public static string cQRCodeGradientColor2 = string.Empty;      // Second color of the QR code gradient
        public static string cQRCodeGradientColor3 = string.Empty;      // Third color of the QR code gradient
        public static string cQRCodeGradientDirection = string.Empty;   // Direction of the QR code gradient 
        public static bool bQRCodeBackgroundImage;                      // Flag to indicate if the QR code has a background image
        public static bool bQRCodeForegroundImage;                      // Flag to indicate if the QR code has a foreground image
        public static bool bBarcodeWithCaption;                         // Control whether the barcode image should include a caption
        public static bool bCompressionEnabled;                         // Control whether the barcode text should be compressed before encoding it in the barcode and decompressed after decoding it from the barcode
        public static string cFileBarcodePng = string.Empty;            // Path and file name of the saved barcode image
        public static string cFileBarcodeSvg = string.Empty;            // Path and file name of the saved barcode image

        // Creating a public dictionary
        public static Dictionary<string, string> barcodeSearch = [];

        /// <summary>
        /// Initialize the barcode formats by assigning the values from the resource file to the global readonly variables
        /// This method can be called in the constructor of the main page or in the startup class of the application
        /// </summary>
        public static void InitializeBarcodeFormats()
        {
            // Global variables for 1D barcode formats
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

            // Global variables for 2D barcode formats
            cBarcode_AZTEC = CodeLang.Barcode_AZTEC_Text;
            cBarcode_AZTEC_CODE = CodeLang.Barcode_AZTEC_CODE_Text;
            cBarcode_AZTEC_RUNE = CodeLang.Barcode_AZTEC_RUNE_Text;

            cBarcode_DATA_MATRIX = CodeLang.Barcode_DATA_MATRIX_Text;

            cBarcode_MAXICODE = CodeLang.Barcode_MAXICODE_Text;

            cBarcode_PDF_417 = CodeLang.Barcode_PDF_417_Text;               // Portable Data File 417
            cBarcode_COMPACT_PDF_417 = CodeLang.Barcode_COMPACT_PDF_417_Text;
            cBarcode_MICRO_PDF_417 = CodeLang.Barcode_MICRO_PDF_417_Text;

            cBarcode_QR_CODE = CodeLang.Barcode_QR_CODE_Text;               // Quick Response Code Model 2
            cBarcode_QR_CODE_MODEL_1 = CodeLang.Barcode_QR_CODE_MODEL_1_Text;  // QR Code Model 1
            cBarcode_QR_CODE_MODEL_2 = CodeLang.Barcode_QR_CODE_MODEL_2_Text;  // QR Code Model 2
            cBarcode_FRAME_QR_CODE = CodeLang.Barcode_FRAME_QR_CODE_Text;   // Frame QR Code
            cBarcode_MICRO_QR_CODE = CodeLang.Barcode_MICRO_QR_CODE_Text;   // Micro QR Code symbol version -4 (M4) with error correction level L, M, or Q
            cBarcode_RMQR = CodeLang.Barcode_RMQR_Text;                     // Rectangular Micro QR Code
            cBarcode_SQRC = CodeLang.Barcode_SQRC_Text;                     // Secure Quick Response Code
            cBarcode_QR_CODE_IMAGE = CodeLang.Barcode_QR_CODE_IMAGE_Text;   // QR Code with image in the center
            cBarcode_ART_QR_CODE = CodeLang.Barcode_ART_QR_CODE_Text;       // Artistic QR Code

            // Default values for the barcode generator and scanner
            cBarcodeGeneratorDefault = cBarcode_ART_QR_CODE;
            cBarcodeScannerDefault = CodeLang.Barcode_AllCodes_Text;
        }

        // Initialize the barcode search formats by putting the name of the barcode format in a dictionary
        public static void InitializeBarcodeSearchFormats()
        {
            // Clear the dictionary before adding new elements
            barcodeSearch.Clear();

            // Add elements to the dictionary
            // All codes
            barcodeSearch.Add("ALL_CODES", CodeLang.Barcode_AllCodes_Text);

            // 1D barcode formats
            barcodeSearch.Add("CODABAR", CodeLang.Barcode_CODABAR_Text);
            barcodeSearch.Add("CODE_39", CodeLang.Barcode_CODE_39_Text);
            barcodeSearch.Add("CODE_39_CHECKSUM_MOD_10", CodeLang.Barcode_CODE_39_CHECKSUM_MOD_10_Text);
            barcodeSearch.Add("CODE_39_CHECKSUM_MOD_43", CodeLang.Barcode_CODE_39_CHECKSUM_MOD_43_Text);
            barcodeSearch.Add("CODE_39_FULL_ASCII", CodeLang.Barcode_CODE_39_FULL_ASCII_Text);
            barcodeSearch.Add("CODE_39_FULL_ASCII_CHECKSUM", CodeLang.Barcode_CODE_39_FULL_ASCII_CHECKSUM_Text);
            barcodeSearch.Add("CODE_93", CodeLang.Barcode_CODE_93_Text);
            barcodeSearch.Add("CODE_93I", CodeLang.Barcode_CODE_93_I_Text);
            barcodeSearch.Add("CODE_128", CodeLang.Barcode_CODE_128_Text);
            barcodeSearch.Add("DX_FILM_EDGE", CodeLang.Barcode_DX_FILM_EDGE_Text);
            barcodeSearch.Add("EAN_2", CodeLang.Barcode_EAN_2_Text);
            barcodeSearch.Add("EAN_5", CodeLang.Barcode_EAN_5_Text);
            barcodeSearch.Add("EAN_8", CodeLang.Barcode_EAN_8_Text);
            barcodeSearch.Add("EAN_13", CodeLang.Barcode_EAN_13_Text);
            barcodeSearch.Add("EAN_UPC", CodeLang.Barcode_EAN_UPC_Text);
            barcodeSearch.Add("GS1_DATABAR", CodeLang.Barcode_GS1_DATABAR_Text);
            barcodeSearch.Add("GS1_DATABAR_EXPANDED", CodeLang.Barcode_GS1_DATABAR_EXPANDED_Text);
            barcodeSearch.Add("GS1_DATABAR_EXPANDED_STACKED", CodeLang.Barcode_GS1_DATABAR_EXPANDED_STACKED_Text);
            barcodeSearch.Add("GS1_DATABAR_LIMITED", CodeLang.Barcode_GS1_DATABAR_LIMITED_Text);
            barcodeSearch.Add("GS1_DATABAR_OMNI", CodeLang.Barcode_GS1_DATABAR_OMNI_Text);
            barcodeSearch.Add("GS1_DATABAR_STACKED", CodeLang.Barcode_GS1_DATABAR_STACKED_Text);
            barcodeSearch.Add("GS1_DATABAR_STACKED_OMNI", CodeLang.Barcode_GS1_DATABAR_STACKED_OMNI_Text);
            barcodeSearch.Add("IMB", CodeLang.Barcode_IMB_Text);
            barcodeSearch.Add("ISBN", CodeLang.Barcode_ISBN_Text);
            barcodeSearch.Add("ITF", CodeLang.Barcode_ITF_Text);
            barcodeSearch.Add("ITF_14", CodeLang.Barcode_ITF_14_Text);
            barcodeSearch.Add("ITF_CHECKSUM", CodeLang.Barcode_ITF_CHECKSUM_Text);
            barcodeSearch.Add("MSI", CodeLang.Barcode_MSI_Text);
            barcodeSearch.Add("PHARMACODE", CodeLang.Barcode_PHARMACODE_Text);
            barcodeSearch.Add("PLESSEY", CodeLang.Barcode_PLESSEY_Text);
            barcodeSearch.Add("PZN", CodeLang.Barcode_PZN_Text);
            barcodeSearch.Add("RSS_14", CodeLang.Barcode_RSS_14_Text);
            barcodeSearch.Add("RSS_EXPANDED", CodeLang.Barcode_RSS_EXPANDED_Text);
            barcodeSearch.Add("UPC_A", CodeLang.Barcode_UPC_A_Text);
            barcodeSearch.Add("UPC_E", CodeLang.Barcode_UPC_E_Text);
            barcodeSearch.Add("UPC_EAN_EXTENSION", CodeLang.Barcode_UPC_EAN_EXTENSION_Text);
            
            // 2D barcode formats
            barcodeSearch.Add("AZTEC", CodeLang.Barcode_AZTEC_Text);
            barcodeSearch.Add("AZTEC_CODE", CodeLang.Barcode_AZTEC_CODE_Text);
            barcodeSearch.Add("AZTEC_RUNE", CodeLang.Barcode_AZTEC_RUNE_Text);
            barcodeSearch.Add("DATA_MATRIX", CodeLang.Barcode_DATA_MATRIX_Text);
            barcodeSearch.Add("MAXICODE", CodeLang.Barcode_MAXICODE_Text);
            barcodeSearch.Add("PDF_417", CodeLang.Barcode_PDF_417_Text);
            barcodeSearch.Add("COMPACT_PDF_417", CodeLang.Barcode_COMPACT_PDF_417_Text);
            barcodeSearch.Add("MICRO_PDF_417", CodeLang.Barcode_MICRO_PDF_417_Text);
            barcodeSearch.Add("QR_CODE", CodeLang.Barcode_QR_CODE_Text);
            barcodeSearch.Add("QR_CODE_MODEL_1", CodeLang.Barcode_QR_CODE_MODEL_1_Text);
            barcodeSearch.Add("QR_CODE_MODEL_2", CodeLang.Barcode_QR_CODE_MODEL_2_Text);
            barcodeSearch.Add("FRAME_QR_CODE", CodeLang.Barcode_FRAME_QR_CODE_Text);
            barcodeSearch.Add("MICRO_QR_CODE", CodeLang.Barcode_MICRO_QR_CODE_Text);
            barcodeSearch.Add("RMQR", CodeLang.Barcode_RMQR_Text);
            barcodeSearch.Add("SQRC", CodeLang.Barcode_SQRC_Text);
            barcodeSearch.Add("QR_CODE_IMAGE", CodeLang.Barcode_QR_CODE_IMAGE_Text);
            barcodeSearch.Add("ART_QR_CODE", CodeLang.Barcode_ART_QR_CODE_Text);
        }

        /// <summary>
        /// Put the generator barcode formats in a List string using the ZXing, QRCoder and SkiaSharp.QrCode library for all platforms
        /// </summary>
        /// <returns></returns>
        public static List<string> GetFormatCodeListGenerator_ZX() => [.. new List<string>
            {
                // https://github.com/redth/ZXing.Net.Maui
                // 1D barcode formats    
                CodeLang.Barcode_CODABAR_Text,
                CodeLang.Barcode_CODE_39_Text,
                CodeLang.Barcode_CODE_93_Text,
                CodeLang.Barcode_CODE_128_Text,
                CodeLang.Barcode_EAN_8_Text,
                CodeLang.Barcode_EAN_13_Text,
                CodeLang.Barcode_ITF_Text,
                CodeLang.Barcode_MSI_Text,
                CodeLang.Barcode_PLESSEY_Text,
                CodeLang.Barcode_UPC_A_Text,
                CodeLang.Barcode_UPC_E_Text,
                // 2D barcode formats
                CodeLang.Barcode_AZTEC_Text,
                CodeLang.Barcode_DATA_MATRIX_Text,
                CodeLang.Barcode_PDF_417_Text,
                CodeLang.Barcode_QR_CODE_Text,
                CodeLang.Barcode_QR_CODE_IMAGE_Text,
                CodeLang.Barcode_ART_QR_CODE_Text,
                CodeLang.Barcode_MICRO_QR_CODE_Text
            }
            .OrderBy(x => x, StringComparer.OrdinalIgnoreCase)];

        /// <summary>
        /// Put the generator barcode formats in a List string using the ZXing and QRCoder library for Windows
        /// </summary>
        /// <returns></returns>
        public static List<string> GetFormatCodeListGenerator_ZX_Windows() => [.. new List<string>
            {
                // https://github.com/redth/ZXing.Net.Maui    
                // 1D barcode formats
                //CodeLang.Barcode_CODE_93_Text,
                //CodeLang.Barcode_UPC_E_Text,
                // 2D barcode formats
                CodeLang.Barcode_AZTEC_Text,
                CodeLang.Barcode_DATA_MATRIX_Text,
                CodeLang.Barcode_PDF_417_Text,
                CodeLang.Barcode_QR_CODE_Text,
                CodeLang.Barcode_QR_CODE_IMAGE_Text,
                CodeLang.Barcode_ART_QR_CODE_Text,
                CodeLang.Barcode_MICRO_QR_CODE_Text
            }
            .OrderBy(x => x, StringComparer.OrdinalIgnoreCase)];

        /// <summary>
        /// Put the scanner barcode formats in a List string using the ZXing library for all platforms
        /// </summary>
        /// <returns></returns>
        public static List<string> GetFormatCodeListScanner_ZX() => [.. new List<string>
            {
                // https://github.com/redth/ZXing.Net.Maui
                CodeLang.Barcode_AllCodes_Text,
                // 1D barcode formats
                CodeLang.Barcode_CODABAR_Text,
                CodeLang.Barcode_CODE_39_Text,
                CodeLang.Barcode_CODE_93_Text,
                CodeLang.Barcode_CODE_128_Text,
                CodeLang.Barcode_EAN_8_Text,
                CodeLang.Barcode_EAN_13_Text,
                //CodeLang.Barcode_IMB_Text,
                CodeLang.Barcode_ITF_Text,
                CodeLang.Barcode_MSI_Text,
                //CodeLang.Barcode_PHARMACODE_Text,
                //CodeLang.Barcode_PLESSEY_Text,
                //CodeLang.Barcode_RSS_14_Text,
                CodeLang.Barcode_RSS_EXPANDED_Text,
                CodeLang.Barcode_UPC_A_Text,
                CodeLang.Barcode_UPC_E_Text,
                //CodeLang.Barcode_UPC_EAN_EXTENSION_Text,
                // 2D barcode formats
                CodeLang.Barcode_AZTEC_Text,
                CodeLang.Barcode_DATA_MATRIX_Text,
                //CodeLang.Barcode_MAXICODE_Text,
                CodeLang.Barcode_PDF_417_Text,
                CodeLang.Barcode_QR_CODE_Text
            }
            .OrderBy(x => x, StringComparer.OrdinalIgnoreCase)];

        /// <summary>
        /// Put the scanner barcode formats in a List string using the native library for Android
        /// https://developers.google.com/ml-kit/vision/barcode-scanning/android
        /// </summary>
        /// <returns></returns>
        public static List<string> GetFormatCodeListScanner_NT_Android() => [.. new List<string>
            {
                // https://github.com/afriscic/BarcodeScanning.Native.Maui
                CodeLang.Barcode_AllCodes_Text,
                // 1D barcode formats                
                CodeLang.Barcode_CODABAR_Text,
                CodeLang.Barcode_CODE_39_Text,
                CodeLang.Barcode_CODE_93_Text,
                CodeLang.Barcode_CODE_128_Text,
                CodeLang.Barcode_EAN_8_Text,
                CodeLang.Barcode_EAN_13_Text,
                CodeLang.Barcode_ITF_Text,
                CodeLang.Barcode_UPC_A_Text,
                CodeLang.Barcode_UPC_E_Text,
                // 2D barcode formats
                CodeLang.Barcode_AZTEC_Text,
                CodeLang.Barcode_DATA_MATRIX_Text,
                CodeLang.Barcode_PDF_417_Text,
                CodeLang.Barcode_QR_CODE_Text
            }
            .OrderBy(x => x, StringComparer.OrdinalIgnoreCase)];

        /// <summary>
        /// Put the scanner barcode formats in a List string using the native library for iOS
        /// https://developer.apple.com/documentation/avfoundation/avmetadataobject/objecttype
        /// </summary>
        /// <returns></returns>
        public static List<string> GetFormatCodeListScanner_NT_IOS() => [.. new List<string>
            {
                // https://github.com/afriscic/BarcodeScanning.Native.Maui    
                CodeLang.Barcode_AllCodes_Text,
                // 1D barcode formats
                CodeLang.Barcode_CODABAR_Text,
                CodeLang.Barcode_CODE_39_Text,
                CodeLang.Barcode_CODE_93_Text,
                CodeLang.Barcode_CODE_128_Text,
                CodeLang.Barcode_EAN_8_Text,
                CodeLang.Barcode_EAN_13_Text,
                CodeLang.Barcode_GS1_DATABAR_Text,
                CodeLang.Barcode_ITF_Text,
                CodeLang.Barcode_UPC_A_Text,
                CodeLang.Barcode_UPC_E_Text,
                // 2D barcode formats
                CodeLang.Barcode_AZTEC_Text,
                CodeLang.Barcode_DATA_MATRIX_Text,
                CodeLang.Barcode_PDF_417_Text,
                CodeLang.Barcode_MICRO_PDF_417_Text,
                CodeLang.Barcode_QR_CODE_Text,
                CodeLang.Barcode_MICRO_QR_CODE_Text,
            }
            .OrderBy(x => x, StringComparer.OrdinalIgnoreCase)];

        /// <summary>
        /// Put the scanner barcode formats in a List string using the native library for Windows
        /// </summary>
        /// <returns></returns>
        public static List<string> GetFormatCodeListScanner_NT_Windows() => [.. new List<string>
            {
                // https://github.com/afriscic/BarcodeScanning.Native.Maui    
                CodeLang.Barcode_AllCodes_Text,
                // 1D barcode formats
                CodeLang.Barcode_CODABAR_Text,
                CodeLang.Barcode_CODE_39_Text,
                CodeLang.Barcode_CODE_93_Text,
                CodeLang.Barcode_CODE_128_Text,
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
                CodeLang.Barcode_PZN_Text,
                CodeLang.Barcode_UPC_A_Text,
                CodeLang.Barcode_UPC_E_Text,
                // 2D barcode formats
                CodeLang.Barcode_AZTEC_Text,
                CodeLang.Barcode_AZTEC_CODE_Text,
                CodeLang.Barcode_AZTEC_RUNE_Text,
                CodeLang.Barcode_DATA_MATRIX_Text,
                CodeLang.Barcode_MAXICODE_Text,
                CodeLang.Barcode_PDF_417_Text,
                CodeLang.Barcode_COMPACT_PDF_417_Text,
                CodeLang.Barcode_MICRO_PDF_417_Text,
                CodeLang.Barcode_QR_CODE_Text,
                CodeLang.Barcode_QR_CODE_MODEL_1_Text,
                CodeLang.Barcode_QR_CODE_MODEL_2_Text,
                CodeLang.Barcode_MICRO_QR_CODE_Text,
                CodeLang.Barcode_RMQR_Text
            }
            .OrderBy(x => x, StringComparer.OrdinalIgnoreCase)];

        /// <summary>
        /// Searches for the specified key in the barcode dictionary and returns the associated value, if found.    
        /// </summary>
        /// <remarks>If the key is found, the method writes the key and its value to the console. If the
        /// key is not found, a message indicating the missing key is written to the console.</remarks>
        /// <param name="searchKey">The key to locate in the barcode dictionary. This value cannot be null.</param>
        /// <returns>The value associated with the specified key if the key exists in the dictionary; otherwise, an empty string.</returns>
        public static string SearchKeyInDictionary(string searchKey)
        {
            if (!string.IsNullOrEmpty(searchKey))
            {
                if (barcodeSearch.TryGetValue(searchKey, out var value))
                {
                    return value;
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Searches for the specified value in the barcode dictionary and returns the corresponding key if found.
        /// </summary>
        /// <remarks>This method writes a message to the console indicating whether the value was found
        /// and, if so, the associated key. The search is case-sensitive.</remarks>
        /// <param name="searchValue">The value to locate within the barcode dictionary. This parameter must not be null.</param>
        /// <returns>The key associated with the specified value if it exists in the dictionary; otherwise, an empty string.</returns>
        public static string SearchValueInDictionary(string searchValue)
        {
            if (!string.IsNullOrEmpty(searchValue))
            {
                // More efficient reverse lookup using LINQ
                string foundKey = barcodeSearch.FirstOrDefault(x => x.Value == searchValue).Key;
                
                if (!string.IsNullOrEmpty(foundKey))
                {
                    return foundKey;
                }
            }

            return string.Empty;
        }

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
   Codabar                       20         20         20

   Code 39                       43         43         43
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
   
   ITF                           14          14
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

                      edtTextToCode    Numeric      Alpha    Binary     Kanji/Kana    Version    Correction
                          MaxLength               Numeric    8-bits     JIS X 0208                    level
                      -------------    -------    -------    -------    ----------    -------    ----------
   Aztec                       1900       3832       3067       1914
   Aztec Code
   Aztec Rune
   
   Data Matrix                 1500       3116       2335       1555                     

   MaxiCode                      93         93

   Pdf 417                     1100       2710       1850       1108
   Compact PDF417
   Micro PDF 417                366        250        150

   QR Code Model 1                        1101        667        458       282             14       L (low)
   
   QR Code Model 2                        7089       4296       2953      1817             40       L (low)
   QR Code Model 2                        5596       3391       2331      1435             40       M (medium)
   QR Code Model 2             3993       3993       2420       1663      1024             40       Q (quartile)
   QR Code Model 2                        3057       1852       1273       784             40       H (high)
   
   QR Code with Image          3057       3057       1852       1273       784             40       H (high)
   Art QR Code                 3057       3057       1852       1273       784             40       H (high)
   
   Frame QR
   
   Micro QR                      35         35         21         15         9             M4       L (low)
   Micro QR                      30         30         18         13         8             M4       M (medium)
   Micro QR                      21         21         13          9         5             M4       Q (quartile)
   
   rMQR Rectangular Micro QR
   
   SQRC Secure QR Code

   https://www.qrcode.com/en/about/version.html
   https://camcode.com/blog/guide-to-barcode-types-standards/
   https://www.bartendersoftware.com/
   _____________________________________________________________________________________________________________ */
