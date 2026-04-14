using System.IO.Compression;

namespace BarcodeGenerator
{
    internal class ClassCompression
    {
        /// <summary>
        /// Compresses the specified string using GZip compression and encodes the result as a Base64 string.
        /// </summary>
        /// <remarks>The input string is first encoded as UTF-8 before compression. The returned Base64
        /// string can be decoded and decompressed to retrieve the original input.</remarks>
        /// <param name="s">The string to compress. Cannot be null.</param>
        /// <returns>A Base64-encoded string representing the compressed input.</returns>
        public static string CompressToBase64(string s)
        {
            byte[] input = System.Text.Encoding.UTF8.GetBytes(s);
            using MemoryStream ms = new();
            using (GZipStream gz = new(ms, CompressionLevel.Optimal, leaveOpen: true))
            {
                gz.Write(input, 0, input.Length);
            }

            ms.Position = 0;

            return Convert.ToBase64String(ms.ToArray());
        }

        /// <summary>
        /// Decompresses a Base64-encoded string that was compressed using GZip compression
        /// The input string is first decoded from Base64, then decompressed to retrieve the original string
        /// </summary>
        /// <param name="base64"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static string DecompressFromBase64(string base64)
        {
            if (string.IsNullOrEmpty(base64) || !IsGzipBase64(base64))
            {
                return base64 ?? string.Empty;
            }

            byte[] compressed = Convert.FromBase64String(base64);
            using MemoryStream ms = new(compressed);
            using GZipStream gz = new(ms, CompressionMode.Decompress);
            using MemoryStream outMs = new();
            
            gz.CopyTo(outMs);
            
            return System.Text.Encoding.UTF8.GetString(outMs.ToArray());
        }

        /// <summary>
        /// Checks if the provided Base64 string represents data that is compressed using GZip. The method decodes the Base64 string and checks for the GZip header bytes (0x1F, 0x8B, 0x08) to determine if it is GZip-compressed data.
        /// </summary>
        /// <param name="base64">The Base64 string to check.</param>
        /// <returns>True if the Base64 string represents GZip-compressed data; otherwise, false.</returns>
        private static bool IsGzipBase64(string base64)
        {
            if (string.IsNullOrEmpty(base64)) return false;

            byte[] data;
            try
            {
                data = Convert.FromBase64String(base64);
            }
            catch (FormatException)
            {
                return false; // not valid Base64
            }

            return IsGzip(data);
        }

        /// <summary>
        /// Determines whether the specified byte array represents the beginning of a GZIP-compressed stream.
        /// </summary>
        /// <remarks>This method checks for the standard GZIP magic numbers (0x1F, 0x8B, 0x08) at the
        /// start of the array. It does not validate the entire stream or check for additional GZIP format
        /// requirements.</remarks>
        /// <param name="data">The byte array to examine for the GZIP file signature. Cannot be null.</param>
        /// <returns>true if the byte array starts with the GZIP file signature; otherwise, false.</returns>
        private static bool IsGzip(byte[] data)
        {
            return data != null && data.Length >= 3
                && data[0] == 0x1F && data[1] == 0x8B && data[2] == 0x08;
        }
    }
}
