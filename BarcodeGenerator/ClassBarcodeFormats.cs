namespace BarcodeGenerator
{
    internal class ClassBarcodeFormats
    {
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
                "Code 93",
                "Data Matrix",
                "PDF417",
                "QR Code",
                "UPC-E"
            ];

        ///// <summary>
        ///// Put the scanner barcode formats in a List string
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
        /// https://developer.apple.com/documentation/avfoundation/avmetadataobject/objecttype
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
        /// Set the barcode format name to common or unique name
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string SetBarcodeNameToCommonName(string format)
        {
            return format switch
            {
                "Aztec" => "AZTEC",
                "Codabar" => "CODABAR",
                "Code 128" => "CODE_128",
                "Code 39" => "CODE_39",
                "Code 93" => "CODE_93",
                "Data Matrix" => "DATA_MATRIX",
                "EAN-13" => "EAN_13",
                "EAN-8" => "EAN_8",
                "GS1 DataBar" => "GS1_DATABAR",
                "IMb" or "IMB" or "IMb (Intelligent Mail)" => "IMB",
                "ITF" or "ITF (Interleaved 2 of 5)" => "ITF",
                "MaxiCode" => "MAXICODE",
                "Micro PDF417" => "MICRO_PDF417",
                "Micro QR Code" => "MICRO_QR_CODE",
                "MSI" or "MSI (Modified Plessey)" => "MSI",
                "PDF417" or "PDF-417" => "PDF_417",
                "Pharmacode" => "PHARMACODE",
                "Plessey" => "PLESSEY",
                "QR Code" => "QR_CODE",
                "RSS 14" => "RSS_14",
                "RSS Expanded" => "RSS_EXPANDED",
                "UPC-A" => "UPC_A",
                "UPC-E" => "UPC_E",
                "UPC EAN Extension" => "UPC_EAN_EXTENSION",
                _ => CodeLang.AllCodes_Text,
            };
        }
    }
}
