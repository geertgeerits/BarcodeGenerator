namespace BarcodeGenerator
{
    internal class ClassBarcodes
    {
        // Global variables for barcode formats
        public static int nBarcodeGeneratorIndex;
        public static int nBarcodeScannerIndex;

        public static string cBarcodeGeneratorName = string.Empty;
        public static string cBarcodeScannerName = string.Empty;

        public const string cBarcodeGeneratorDefault = "QR Code";
        public static string cBarcodeScannerDefault = CodeLang.AllCodes_Text;

        public const string cBarcode_AZTEC = "AZTEC";
        public const string cBarcode_CODABAR = "CODABAR";
        public const string cBarcode_CODE_128 = "CODE_128";
        public const string cBarcode_CODE_39 = "CODE_39";
        public const string cBarcode_CODE_39_MOD_43 = "CODE_39_MOD_43";
        public const string cBarcode_CODE_93 = "CODE_93";
        public const string cBarcode_DATA_MATRIX = "DATA_MATRIX";
        public const string cBarcode_EAN_13 = "EAN_13";
        public const string cBarcode_EAN_8 = "EAN_8";
        public const string cBarcode_GS1_DATABAR = "GS1_DATABAR";
        public const string cBarcode_GS1_DATABAR_EXPANDED = "GS1_DATABAR_EXPANDED";
        public const string cBarcode_GS1_DATABAR_LIMITED = "GS1_DATABAR_LIMITED";
        public const string cBarcode_IMB = "IMB";
        public const string cBarcode_ITF = "ITF";
        public const string cBarcode_ITF_14 = "ITF_14";
        public const string cBarcode_MAXICODE = "MAXICODE";
        public const string cBarcode_MICRO_PDF_417 = "MICRO_PDF_417";
        public const string cBarcode_MICRO_QR_CODE = "MICRO_QR_CODE";
        public const string cBarcode_MSI = "MSI";
        public const string cBarcode_PDF_417 = "PDF_417";
        public const string cBarcode_PHARMACODE = "PHARMACODE";
        public const string cBarcode_PLESSEY = "PLESSEY";
        public const string cBarcode_QR_CODE = "QR_CODE";
        public const string cBarcode_RSS_14 = "RSS_14";
        public const string cBarcode_RSS_EXPANDED = "RSS_EXPANDED";
        public const string cBarcode_UPC_A = "UPC_A";
        public const string cBarcode_UPC_E = "UPC_E";
        public const string cBarcode_UPC_EAN_EXTENSION = "UPC_EAN_EXTENSION";

        /// <summary>
        /// Put the generator barcode formats in a List string for Android and iOS
        /// </summary>
        /// <returns></returns>
        public static List<string> GetFormatCodeListGenerator() => [
                "Aztec",
                "Codabar",
                "Code 39",
                "Code 93",
                "Code 128",
                "Data Matrix",
                "EAN-8",
                "EAN-13",
                "ITF (Interleaved 2 of 5)",
                "MSI (Modified Plessey)",
                "PDF417",
                "Plessey",
                "QR Code",
                "UPC-A",
                "UPC-E"
            ];

        /// <summary>
        /// Put the generator barcode formats in a List string for Windows
        /// </summary>
        /// <returns></returns>
        public static List<string> GetFormatCodeListGeneratorWindows() => [
                "Aztec",
                //"Code 93",
                "Data Matrix",
                "PDF417",
                "QR Code"
                //"UPC-E"
            ];

        ///// <summary>
        ///// Put the scanner barcode formats in a List string using ZXing for all platforms
        ///// </summary>
        ///// <returns></returns>
        //public static List<string> GetFormatCodeListScanner() => [
        //        "Aztec",
        //        "Codabar",
        //        "Code 128",
        //        "Code 39",
        //        "Code 93",
        //        "Data Matrix",
        //        "EAN-13",
        //        "EAN-8",
        //        "IMb (Intelligent Mail)",
        //        "ITF (Interleaved 2 of 5)",
        //        "MaxiCode",
        //        "MSI (Modified Plessey)",
        //        "PDF417",
        //        "Pharmacode",
        //        "Plessey",
        //        "QR Code",
        //        "RSS 14",
        //        "RSS Expanded",
        //        "UPC-A",
        //        "UPC-E",
        //        "UPC EAN Extension",
        //        CodeLang.AllCodes_Text
        //    ];

        /// <summary>
        /// Put the scanner barcode formats in a List string for the Native scanner for Android
        /// https://developers.google.com/ml-kit/vision/barcode-scanning/android
        /// </summary>
        /// <returns></returns>
        public static List<string> GetFormatCodeListScannerNativeAndroid() => [
                CodeLang.AllCodes_Text,
                "Aztec",
                "Codabar",
                "Code 39",
                "Code 93",
                "Code 128",
                "Data Matrix",
                "EAN-8",
                "EAN-13",
                "ITF",
                "PDF417",
                "QR Code",
                "UPC-A",
                "UPC-E"
            ];

        /// <summary>
        /// Put the scanner barcode formats in a List string for the Native scanner for iOS
        /// https://developer.apple.com/documentation/avfoundation/avmetadataobject/objecttype
        /// </summary>
        /// <returns></returns>
        public static List<string> GetFormatCodeListScannerNativeIOS() => [
                CodeLang.AllCodes_Text,
                "Aztec",
                "Codabar",
                "Code 39",
                "Code 93",
                "Code 128",
                "Data Matrix",
                "EAN-8",
                "EAN-13",
                "GS1 DataBar",
                "ITF",
                "Micro PDF417",
                "Micro QR Code",
                "PDF417",
                "QR Code",
                "UPC-A",
                "UPC-E"
            ];

        /// <summary>
        /// Put the scanner barcode formats in a List string for the Native scanner for Windows
        /// </summary>
        /// <returns></returns>
        public static List<string> GetFormatCodeListScannerNativeWindows() => [
                CodeLang.AllCodes_Text,
                "Aztec",
                "Codabar",
                "Code 39",
                "Code 93",
                "Code 128",
                "Data Matrix",
                "EAN-8",
                "EAN-13",
                "GS1 DataBar",
                "ITF",
                "Micro QR Code",
                "PDF417",
                "QR Code",
                "UPC-A",
                "UPC-E"
            ];

        /// <summary>
        /// Set the barcode format name to common and unique name
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string SetBarcodeNameToCommonName(string cBarcode)
        {
            return cBarcode switch
            {
                "Aztec" => cBarcode_AZTEC,
                "Codabar" => cBarcode_CODABAR,
                "Code 128" => cBarcode_CODE_128,
                "Code 39" => cBarcode_CODE_39,
                "Code 39 Mod 43" => cBarcode_CODE_39_MOD_43,
                "Code 93" => cBarcode_CODE_93,
                "Data Matrix" => cBarcode_DATA_MATRIX,
                "EAN-13" => cBarcode_EAN_13,
                "EAN-8" => cBarcode_EAN_8,
                "GS1 DataBar" => cBarcode_GS1_DATABAR,
                "GS1 DataBar Expanded" => cBarcode_GS1_DATABAR_EXPANDED,
                "GS1 DataBar Limited" => cBarcode_GS1_DATABAR_LIMITED,
                "IMb" or "IMB" or "IMb (Intelligent Mail)" => cBarcode_IMB,
                "ITF" or "ITF (Interleaved 2 of 5)" => cBarcode_ITF,
                "ITF 14" => cBarcode_ITF_14,
                "MaxiCode" or "Maxi Code" => cBarcode_MAXICODE,
                "Micro PDF417" or "Micro PDF-417" => cBarcode_MICRO_PDF_417,
                "Micro QR Code" => cBarcode_MICRO_QR_CODE,
                "MSI" or "MSI (Modified Plessey)" => cBarcode_MSI,
                "PDF417" or "PDF-417" => cBarcode_PDF_417,
                "Pharmacode" => cBarcode_PHARMACODE,
                "Plessey" => cBarcode_PLESSEY,
                "QR Code" => cBarcode_QR_CODE,
                "RSS 14" => cBarcode_RSS_14,
                "RSS Expanded" => cBarcode_RSS_EXPANDED,
                "UPC-A" => cBarcode_UPC_A,
                "UPC-E" => cBarcode_UPC_E,
                "UPC EAN Extension" => cBarcode_UPC_EAN_EXTENSION,
                _ => CodeLang.AllCodes_Text,
            };
        }
    }
}
