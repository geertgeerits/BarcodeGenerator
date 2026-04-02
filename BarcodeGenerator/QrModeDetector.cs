using System.Text;

namespace BarcodeGenerator
{
    static class QrModeDetector
    {
        public enum Mode { Numeric, Alphanumeric, Kanji, Byte }

        static readonly string AlnumSet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ $%*+-./:";

        // Call once at startup on non-Windows runtimes
        public static void RegisterCodePages() =>
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        // QR code can encode any text, but some characters may not be supported in certain modes
        // This method can be used to validate input before encoding
        public static async Task<bool> CheckInputTextAsync(string text)
        {
            // Check input text length against QR code limits based on detected mode and error correction level (H),
            // and show an alert if it exceeds the limits            
            if (Detect(text) == Mode.Numeric && text.Length > 3993)
            {
                await Application.Current!.Windows[0].Page!.DisplayAlertAsync("Numeric characters detected", "The length of the text is limited to 3993 characters", "OK");
                return false;
            }
            else if (Detect(text) == Mode.Alphanumeric && text.Length > 2420)
            {
                await Application.Current!.Windows[0].Page!.DisplayAlertAsync("Alphanumeric characters detected", "The length of the text is limited to 2420 characters", "OK");
                return false;
            }
            else if (Detect(text) == Mode.Byte && text.Length > 1663)
            {
                await Application.Current!.Windows[0].Page!.DisplayAlertAsync("Binary characters detected", "The length of the text is limited to 1663 characters", "OK");
                return false;
            }
            else if (Detect(text) == Mode.Kanji && text.Length > 1024)
            {
                await Application.Current!.Windows[0].Page!.DisplayAlertAsync("Kanji characters detected", "The length of the text is limited to 1024 characters", "OK");
                return false;
            }
            return true;
        }

        public static Mode Detect(string s)
        {
            if (string.IsNullOrEmpty(s)) return Mode.Byte;
            if (IsNumeric(s)) return Mode.Numeric;
            if (IsAlphanumeric(s)) return Mode.Alphanumeric;
            if (IsKanji(s)) return Mode.Kanji;
            return Mode.Byte;
        }

        static bool IsNumeric(string s) => s.All(char.IsDigit);

        static bool IsAlphanumeric(string s)
        {
            // QR alphanumeric only accepts uppercase letters
            foreach (char c in s)
                if (!AlnumSet.Contains(c)) return false;
            return true;
        }

        static bool IsKanji(string s)
        {
            // Ensure Shift_JIS support is registered (see RegisterCodePages)
            Encoding sjis;
            try { sjis = Encoding.GetEncoding("shift_jis"); }
            catch { return false; }

            foreach (char c in s)
            {
                byte[] bs = sjis.GetBytes(new[] { c });
                if (bs.Length != 2) return false;
                int v = (bs[0] << 8) | bs[1];
                
                // QR Kanji allowed ranges in Shift_JIS
                if (!((v >= 0x8140 && v <= 0x9FFC) || (v >= 0xE040 && v <= 0xEBBF)))
                    return false;
            }
            return true;
        }
    }
}