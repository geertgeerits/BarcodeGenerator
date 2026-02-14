namespace BarcodeGenerator
{
    internal class ClassBarcodes
    {
        // Global constants for 1D barcode formats
        public const string cBarcode_CODABAR = "Codabar";
        
        public const string cBarcode_CODE_128 = "Code 128";
        
        public const string cBarcode_CODE_39 = "Code 39";
        public const string cBarcode_CODE_39_CHECKSUM_MOD_10 = "Code 39 Checksum Mod 10";
        public const string cBarcode_CODE_39_CHECKSUM_MOD_43 = "Code 39 Checksum Mod 43";
        public const string cBarcode_CODE_39_FULL_ASCII = "Code 39 Full ASCII";
        public const string cBarcode_CODE_39_FULL_ASCII_CHECKSUM = "Code 39 Full ASCII Checksum";
        
        public const string cBarcode_CODE_93 = "Code 93";
        public const string cBarcode_CODE_93_I = "Code 93i";
        
        public const string cBarcode_DX_FILM_EDGE = "DX Film Edge";
        
        public const string cBarcode_EAN_13 = "EAN-13";
        public const string cBarcode_EAN_2 = "EAN-2";
        public const string cBarcode_EAN_5 = "EAN-5";
        public const string cBarcode_EAN_8 = "EAN-8";
        public const string cBarcode_EAN_UPC = "EAN/UPC";

        public const string cBarcode_GS1_DATABAR = "GS1 Databar";
        public const string cBarcode_GS1_DATABAR_EXPANDED = "GS1 Databar Expanded";
        public const string cBarcode_GS1_DATABAR_EXPANDED_STACKED = "GS1 Databar Expanded Stacked";
        public const string cBarcode_GS1_DATABAR_LIMITED = "GS1 Databar Limited";
        public const string cBarcode_GS1_DATABAR_OMNI = "GS1 Databar Omni";
        public const string cBarcode_GS1_DATABAR_STACKED = "GS1 Databar Stacked";
        public const string cBarcode_GS1_DATABAR_STACKED_OMNI = "GS1 Databar Stacked Omni";

        public const string cBarcode_IMB = "IMb Intelligent Mail";
        
        public const string cBarcode_ISBN = "ISBN";
        
        public const string cBarcode_ITF = "ITF Interleaved 2 of 5";
        public const string cBarcode_ITF_14 = "ITF Interleaved 2 of 5 14";
        public const string cBarcode_ITF_CHECKSUM = "ITF Interleaved 2 of 5 Checksum";
        
        public const string cBarcode_MSI = "MSI Modified Plessey";
        
        public const string cBarcode_PHARMACODE = "Pharmacode";
        
        public const string cBarcode_PLESSEY = "Plessey";
        
        public const string cBarcode_PZN = "PZN";
        
        public const string cBarcode_RSS_14 = "RSS 14";
        public const string cBarcode_RSS_EXPANDED = "RSS Expanded";
        
        public const string cBarcode_UPC_A = "UPC-A";
        public const string cBarcode_UPC_E = "UPC-E";
        public const string cBarcode_UPC_EAN_EXTENSION = "UPC EAN Extension";

        // Global constants for 2D barcode formats
        public const string cBarcode_AZTEC = "Aztec";
        public const string cBarcode_AZTEC_CODE = "Aztec Code";
        public const string cBarcode_AZTEC_RUNE = "Aztec Rune";

        public const string cBarcode_DATA_MATRIX = "Data Matrix";

        public const string cBarcode_MAXICODE = "MaxiCode";

        public const string cBarcode_PDF_417 = "PDF417";
        public const string cBarcode_COMPACT_PDF_417 = "Compact PDF417";
        public const string cBarcode_MICRO_PDF_417 = "Micro PDF417";

        public const string cBarcode_MICRO_QR_CODE = "Micro QR Code";
        public const string cBarcode_QR_CODE = "QR Code";
        public const string cBarcode_QR_CODE_MODEL_1 = "QR Code Model 1";
        public const string cBarcode_QR_CODE_MODEL_2 = "QR Code Model 2";
        public const string cBarcode_RMQR = "rMQR";

        // Global constants and variables
        public static int nBarcodeGeneratorIndex;
        public static int nBarcodeScannerIndex;
        public static string cBarcodeGeneratorName = string.Empty;
        public static string cBarcodeScannerName = string.Empty;
        public const string cBarcodeGeneratorDefault = cBarcode_QR_CODE;
        public static string cBarcodeScannerDefault = CodeLang.AllCodes_Text;

        /// <summary>
        /// Put the generator barcode formats in a List string using the ZXing library for all platforms
        /// </summary>
        /// <returns></returns>
        public static List<string> GetFormatCodeListGenerator_ZX() => [
                cBarcode_AZTEC,
                cBarcode_CODABAR,
                cBarcode_CODE_39,
                cBarcode_CODE_93,
                cBarcode_CODE_128,
                cBarcode_DATA_MATRIX,
                cBarcode_EAN_8,
                cBarcode_EAN_13,
                cBarcode_ITF,
                cBarcode_MSI,
                cBarcode_PDF_417,
                cBarcode_PLESSEY,
                cBarcode_QR_CODE,
                cBarcode_UPC_A,
                cBarcode_UPC_E
            ];

        /// <summary>
        /// Put the generator barcode formats in a List string using the ZXing library for Windows
        /// </summary>
        /// <returns></returns>
        public static List<string> GetFormatCodeListGenerator_ZX_Windows() => [
                cBarcode_AZTEC,
                //cBarcode_CODE_93,
                cBarcode_DATA_MATRIX,
                cBarcode_PDF_417,
                cBarcode_QR_CODE
                //cBarcode_UPC_E
            ];

        /// <summary>
        /// Put the scanner barcode formats in a List string using the ZXing library for all platforms
        /// </summary>
        /// <returns></returns>
        public static List<string> GetFormatCodeListScanner_ZX() => [
                CodeLang.AllCodes_Text,
                cBarcode_AZTEC,
                cBarcode_CODABAR,
                cBarcode_CODE_128,
                cBarcode_CODE_39,
                cBarcode_CODE_93,
                cBarcode_DATA_MATRIX,
                cBarcode_EAN_13,
                cBarcode_EAN_8,
                cBarcode_IMB,
                cBarcode_ITF,
                cBarcode_MAXICODE,
                cBarcode_MSI,
                cBarcode_PDF_417,
                cBarcode_PHARMACODE,
                cBarcode_PLESSEY,
                cBarcode_QR_CODE,
                cBarcode_RSS_14,
                cBarcode_RSS_EXPANDED,
                cBarcode_UPC_A,
                cBarcode_UPC_E,
                cBarcode_UPC_EAN_EXTENSION
            ];

        /// <summary>
        /// Put the scanner barcode formats in a List string using the native library for Android
        /// https://developers.google.com/ml-kit/vision/barcode-scanning/android
        /// </summary>
        /// <returns></returns>
        public static List<string> GetFormatCodeListScanner_NT_Android() => [
                CodeLang.AllCodes_Text,
                cBarcode_AZTEC,
                cBarcode_CODABAR,
                cBarcode_CODE_39,
                cBarcode_CODE_93,
                cBarcode_CODE_128,
                cBarcode_DATA_MATRIX,
                cBarcode_EAN_8,
                cBarcode_EAN_13,
                cBarcode_ITF,
                cBarcode_PDF_417,
                cBarcode_QR_CODE,
                cBarcode_UPC_A,
                cBarcode_UPC_E
            ];

        /// <summary>
        /// Put the scanner barcode formats in a List string using the native library for iOS
        /// https://developer.apple.com/documentation/avfoundation/avmetadataobject/objecttype
        /// </summary>
        /// <returns></returns>
        public static List<string> GetFormatCodeListScanner_NT_IOS() => [
                CodeLang.AllCodes_Text,
                cBarcode_AZTEC,
                cBarcode_CODABAR,
                cBarcode_CODE_39,
                cBarcode_CODE_93,
                cBarcode_CODE_128,
                cBarcode_DATA_MATRIX,
                cBarcode_EAN_8,
                cBarcode_EAN_13,
                cBarcode_GS1_DATABAR,
                cBarcode_ITF,
                cBarcode_MICRO_PDF_417,
                cBarcode_MICRO_QR_CODE,
                cBarcode_PDF_417,
                cBarcode_QR_CODE,
                cBarcode_UPC_A,
                cBarcode_UPC_E
            ];

        /// <summary>
        /// Put the scanner barcode formats in a List string using the native library for Windows
        /// </summary>
        /// <returns></returns>
        public static List<string> GetFormatCodeListScanner_NT_Windows() => [
                CodeLang.AllCodes_Text,
                cBarcode_AZTEC,
                cBarcode_AZTEC_CODE,
                cBarcode_AZTEC_RUNE,
                cBarcode_CODABAR,
                cBarcode_CODE_39,
                cBarcode_CODE_93,
                cBarcode_CODE_128,
                cBarcode_COMPACT_PDF_417,
                cBarcode_DATA_MATRIX,
                cBarcode_DX_FILM_EDGE,
                cBarcode_EAN_2,
                cBarcode_EAN_5,
                cBarcode_EAN_8,
                cBarcode_EAN_13,
                cBarcode_EAN_UPC,
                cBarcode_GS1_DATABAR,
                cBarcode_GS1_DATABAR_EXPANDED,
                cBarcode_GS1_DATABAR_EXPANDED_STACKED,
                cBarcode_GS1_DATABAR_LIMITED,
                cBarcode_GS1_DATABAR_OMNI,
                cBarcode_GS1_DATABAR_STACKED,
                cBarcode_GS1_DATABAR_STACKED_OMNI,
                cBarcode_ISBN,
                cBarcode_ITF,
                cBarcode_ITF_14,
                cBarcode_MAXICODE,
                cBarcode_MICRO_PDF_417,
                cBarcode_MICRO_QR_CODE,
                cBarcode_PDF_417,
                cBarcode_PZN,
                cBarcode_QR_CODE,
                cBarcode_QR_CODE_MODEL_1,
                cBarcode_QR_CODE_MODEL_2,
                cBarcode_RMQR,
                cBarcode_UPC_A,
                cBarcode_UPC_E
            ];
    }
}
