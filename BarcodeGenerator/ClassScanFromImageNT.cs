using BarcodeScanning;

namespace BarcodeGenerator
{
    internal class ClassScanFromImageNT
    {
        // https://github.com/afriscic/BarcodeScanning.Native.Maui/issues/107

        /// <summary>
        /// This method allows you to scan a barcode from an image selected by the user. It uses the MediaPicker to let the user choose a photo, reads the image data, and then processes it to extract any barcodes present in the image. If a barcode is found, it displays the result; otherwise, it logs that no QR code was found.
        /// </summary>
        /// <returns></returns>
        public static async Task ScanFromImageNTAsync()
        {
            // Open the media picker to select photos
            var results = await MediaPicker.PickPhotosAsync(new MediaPickerOptions
            {
                // Default is 1; set to 0 for no limit
                SelectionLimit = 1,
                RotateImage = true,
                PreserveMetaData = true,
            });

            // Process each selected file
            foreach (var file in results)
            {
                using var stream = await file.OpenReadAsync();
                {
                    byte[] bytes = new byte[stream.Length];
                    stream.ReadExactly(bytes);
                    stream.Seek(0, SeekOrigin.Begin);
                    
                    var list = await Methods.ScanFromImageAsync(bytes);
                    List<BarcodeResult> obj = list.ToList();
                    
                    if (obj.Count > 0)
                    {
                        string result = string.Empty;
                        for (int i = 0; i < obj.Count; i++)
                        {
                            result = $"{obj[i].DisplayValue}";
                        }
                        await MainThread.InvokeOnMainThreadAsync(async () =>
                        {
                            await ClassPayloadTypes.SharePayloadTypes(result);
                            //await ProcessScannedText(result);
                        });
                    }
                    else
                    {
                        Debug.WriteLine("QrCodeNotFound");
                    }
                }
            }
        }
    }
}
