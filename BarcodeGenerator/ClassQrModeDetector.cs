using System.Text;

namespace BarcodeGenerator
{
    internal static class ClassQrModeDetector
    {
        public enum Mode { Numeric, Alphanumeric, Kanji, Byte }

        private static readonly string AlnumSet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ $%*+-./:";

        // Call once at startup on non-Windows runtimes
        public static void RegisterCodePages() =>
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        public static Mode Detect(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return Mode.Byte;
            }

            if (IsNumeric(s))
            {
                return Mode.Numeric;
            }

            if (IsAlphanumeric(s))
            {
                return Mode.Alphanumeric;
            }

            if (IsKanji(s))
            {
                return Mode.Kanji;
            }

            return Mode.Byte;
        }

        private static bool IsNumeric(string s) => s.All(char.IsDigit);

        private static bool IsAlphanumeric(string s)
        {
            // QR alphanumeric only accepts uppercase letters
            foreach (char c in s)
            {
                if (!AlnumSet.Contains(c))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool IsKanji(string s)
        {
            // Ensure Shift_JIS support is registered (see RegisterCodePages)
            Encoding sjis;
            try
            {
                sjis = Encoding.GetEncoding("shift_jis");
            }
            catch
            {
                return false;
            }

            foreach (char c in s)
            {
                byte[] bs = sjis.GetBytes(new[] { c });
                if (bs.Length != 2)
                {
                    return false;
                }

                int v = (bs[0] << 8) | bs[1];
                
                // QR Kanji allowed ranges in Shift_JIS
                if (!((v >= 0x8140 && v <= 0x9FFC) || (v >= 0xE040 && v <= 0xEBBF)))
                {
                    return false;
                }
            }
            return true;
        }
    }
}