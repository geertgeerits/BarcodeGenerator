/*
Put this in the BarcodeGenerator project file:
  
<ItemGroup>
    <Reference Include="ZXingCpp">
        <HintPath>..\..\..\..\nuget\zxingcpp\0.5.1\lib\net5.0\ZXingCpp.dll</HintPath>
    </Reference>
</ItemGroup>
*/

using ZXing;
using ZXing.Net.Maui;
//using ZXingCpp;

namespace BarcodeGenerator
{
    public static class ClassScanFromImageZX
    {
        /// <summary>
        /// Scan barcode from an image file
        /// </summary>
//        public static async Task ScanFromImageAsyncZX()
//        {
//            try
//            {
//                // Pick an image file
//                FilePickerFileType imageTypes = new(new Dictionary<DevicePlatform, IEnumerable<string>>
//                {
//                    { DevicePlatform.iOS, new[] { "public.image" } },
//                    { DevicePlatform.Android, new[] { "image/*" } },
//                    { DevicePlatform.WinUI, new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" } },
//                    { DevicePlatform.macOS, new[] { "public.image" } }
//                });

//                PickOptions options = new()
//                {
//                    PickerTitle = "CodeLang.SelectImage_Text",
//                    FileTypes = imageTypes
//                };

//                FileResult? result = await FilePicker.Default.PickAsync(options);

//                if (result == null)
//                {
//                    return;
//                }

//                // Open the file stream
//                using Stream stream = await result.OpenReadAsync();

//                // Create a BarcodeReader instance
//                BarcodeReader reader = new();

//                // Decode the barcode from the image stream
//                BarcodeResult[]? barcodes = reader.Decode(stream);

//                if (barcodes == null || barcodes.Length == 0)
//                {
//                    await Application.Current!.Windows[0].Page!.DisplayAlertAsync(
//                        CodeLang.ErrorTitle_Text,
//                        "CodeLang.NoBarcodeFound_Text",
//                        CodeLang.ButtonClose_Text);
//                    return;
//                }

//                // Build result string
//                System.Text.StringBuilder sb = new();
//                foreach (BarcodeResult barcode in barcodes)
//                {
//                    sb.AppendLine($"{barcode.Format}:");
//                    sb.AppendLine(barcode.Value);
//                    sb.AppendLine();
//                }

//                // Display the results
//                await Application.Current!.Windows[0].Page!.DisplayAlertAsync(
//                    "CodeLang.BarcodeScanned_Text",
//                    sb.ToString().TrimEnd(),
//                    CodeLang.ButtonClose_Text);
//            }
//            catch (Exception ex)
//            {
//                SentrySdk.CaptureException(ex);
//#if DEBUG
//                await Application.Current!.Windows[0].Page!.DisplayAlertAsync(
//                    "Error: ScanFromImageAsync",
//                    ex.Message,
//                    CodeLang.ButtonClose_Text);
//#endif
//            }
//        }
    }
}