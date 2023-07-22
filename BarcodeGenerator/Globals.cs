// Global usings.
global using BarcodeGenerator.Resources.Languages;
global using System.Globalization;
global using Microsoft.AppCenter.Crashes;

namespace BarcodeGenerator;

// Global variables and methods.
static class Globals
{
    // Global variables.
    public static string cTheme;
    public static int nFormatGeneratorIndex;
    public static int nFormatScannerIndex;
    public static string cCodeColorFg;
    public static string cCodeColorBg;
    public static string cLanguage;
    public static bool bLanguageChanged = false;
    public static string cLanguageSpeech;
    public static string[] cLanguageLocales;
    public static bool bLanguageLocalesExist = false;
    public static string cImageTextToSpeech = "speaker_64p_blue_green.png";
    public static string cImageTextToSpeechCancel = "speaker_cancel_64p_blue_red.png";

    // Global methods.
    // Set the current UI culture of the selected language.
    public static void SetCultureSelectedLanguage()
    {
        try
        {
            CultureInfo switchToCulture = new(cLanguage);
            LocalizationResourceManager.Instance.SetCulture(switchToCulture);
        }
        catch
        {
            // Do nothing.
        }
    }

    // Get ISO language (and country) code from locales.
    public static string GetIsoLanguageCode()
    {
        // Split before first space and remove last character '-' if there.
        string cLanguageIso = Globals.cLanguageSpeech.Split(' ').First();

        if (cLanguageIso.EndsWith("-"))
        {
            cLanguageIso = cLanguageIso.Remove(cLanguageIso.Length - 1, 1);
        }

        return cLanguageIso;
    }

    // Put the generator barcode formats in a List string.
    public static List<string> GetFormatCodeListGenerator()
    {
        return new List<string>
        {
            "Aztec",
            "Codabar",
            "Code 128",
            "Code 39",
            "Code 93",
            "Data Matrix",
            "EAN-13",
            "EAN-8",
            "(IMb (Intelligent Mail))",
            "ITF (Interleaved 2 of 5)",
            "(MaxiCode)",
            "MSI (Modified Plessey)",
            "PDF417",
            "(Pharmacode)",
            "Plessey",
            "QR Code",
            "(RSS 14)",
            "(RSS Expanded)",
            "UPC-A",
            "UPC-E",
            "(UPC EAN Extension)"
        };
    }

    // Put the scanner barcode formats in a List string.
    public static List<string> GetFormatCodeListScanner()
    {
        return new List<string>
        {
            "Aztec",
            "Codabar",
            "Code 128",
            "Code 39",
            "Code 93",
            "Data Matrix",
            "EAN-13",
            "EAN-8",
            "(IMb (Intelligent Mail))",
            "ITF (Interleaved 2 of 5)",
            "(MaxiCode)",
            "MSI (Modified Plessey)",
            "PDF417",
            "(Pharmacode)",
            "Plessey",
            "QR Code",
            "RSS 14",
            "RSS Expanded",
            "UPC-A",
            "UPC-E",
            "(UPC EAN Extension)",
            CodeLang.AllCodes_Text
        };
    }
}