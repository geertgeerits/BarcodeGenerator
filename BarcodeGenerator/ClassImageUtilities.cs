namespace BarcodeGenerator
{
    internal class ClassImageUtilities
    {
        // Returns (width, height). Returns (0,0) if format not recognized or on error.
        public static (int width, int height) GetImageDimensions(string path)
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
                return (0, 0);

            try
            {
                using var fs = File.OpenRead(path);
                using var br = new BinaryReader(fs);

                // Read header signature
                var sig = br.ReadBytes(8);

                // PNG: signature then IHDR chunk contains width/height at offset 16 (big-endian)
                if (sig.Length >= 8 && sig[0] == 0x89 && sig[1] == 0x50 && sig[2] == 0x4E && sig[3] == 0x47)
                {
                    br.BaseStream.Seek(16, SeekOrigin.Begin);
                    int w = ReadInt32BigEndian(br);
                    int h = ReadInt32BigEndian(br);
                    return (w, h);
                }

                // GIF: header "GIF87a" or "GIF89a", width/height at offset 6 (little-endian, 2 bytes each)
                if (sig.Length >= 6 && sig[0] == 'G' && sig[1] == 'I' && sig[2] == 'F')
                {
                    br.BaseStream.Seek(6, SeekOrigin.Begin);
                    int w = br.ReadUInt16();
                    int h = br.ReadUInt16();
                    return (w, h);
                }

                // JPEG: scan markers for Start Of Frame (SOF0/SOF2 etc.) segments which contain height/width (big-endian)
                fs.Seek(0, SeekOrigin.Begin);
                if (br.ReadByte() == 0xFF && br.ReadByte() == 0xD8)
                {
                    while (fs.Position < fs.Length)
                    {
                        // read to 0xFF marker prefix
                        byte markerPrefix = br.ReadByte();
                        while (markerPrefix != 0xFF)
                        {
                            if (fs.Position >= fs.Length) return (0, 0);
                            markerPrefix = br.ReadByte();
                        }

                        // skip any padding 0xFF bytes
                        byte marker = br.ReadByte();
                        while (marker == 0xFF)
                            marker = br.ReadByte();

                        // Start of Scan or End of Image: stop
                        if (marker == 0xDA || marker == 0xD9)
                            break;

                        int length = ReadUInt16BigEndian(br);
                        // check for SOF markers: 0xC0..0xC3, 0xC5..0xC7, 0xC9..0xCB, 0xCD..0xCF
                        if ((marker >= 0xC0 && marker <= 0xC3) ||
                            (marker >= 0xC5 && marker <= 0xC7) ||
                            (marker >= 0xC9 && marker <= 0xCB) ||
                            (marker >= 0xCD && marker <= 0xCF))
                        {
                            // sample precision (1), then height (2), width (2)
                            br.ReadByte(); // precision
                            int h = ReadUInt16BigEndian(br);
                            int w = ReadUInt16BigEndian(br);
                            return (w, h);
                        }
                        // skip the rest of the segment
                        if (length < 2) break;
                        fs.Seek(length - 2, SeekOrigin.Current);
                    }
                }

                // BMP: header "BM" then width/height at offset 18 (little-endian 4 bytes)
                fs.Seek(0, SeekOrigin.Begin);
                var start = br.ReadBytes(2);
                if (start.Length == 2 && start[0] == 'B' && start[1] == 'M')
                {
                    br.BaseStream.Seek(18, SeekOrigin.Begin);
                    int w = br.ReadInt32();
                    int h = br.ReadInt32();
                    return (w, Math.Abs(h));
                }
            }
            catch
            {
                // ignore and fall through to return (0,0)
            }

            return (0, 0);
        }

        static int ReadInt32BigEndian(BinaryReader br)
        {
            var b = br.ReadBytes(4);
            if (b.Length < 4) return 0;
            return (b[0] << 24) | (b[1] << 16) | (b[2] << 8) | b[3];
        }

        static int ReadUInt16BigEndian(BinaryReader br)
        {
            var b = br.ReadBytes(2);
            if (b.Length < 2) return 0;
            return (b[0] << 8) | b[1];
        }
    }
}
