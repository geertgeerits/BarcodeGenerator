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
            using (GZipStream gz = new(ms, System.IO.Compression.CompressionLevel.Optimal, leaveOpen: true))
            {
                gz.Write(input, 0, input.Length);
            }

            ms.Position = 0;

            return Convert.ToBase64String(ms.ToArray());
        }

        /// <summary>
        /// Decompresses a Base64-encoded string that was compressed using GZip compression. The input string is first decoded from Base64, then decompressed to retrieve the original string
        /// </summary>
        /// <param name="base64"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static string DecompressFromBase64(string base64)
        {
            if (base64 is null)
            {
                return string.Empty;
            }

            byte[] compressed = Convert.FromBase64String(base64);
            using MemoryStream ms = new(compressed);
            using GZipStream gz = new(ms, CompressionMode.Decompress);
            using MemoryStream outMs = new();
            
            gz.CopyTo(outMs);
            
            return System.Text.Encoding.UTF8.GetString(outMs.ToArray());
        }
    }
}
