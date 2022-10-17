using BarcodeGenerator.Resources.Languages;
using ZXing.Net.Maui;

namespace BarcodeGenerator;

public partial class PageScan : ContentPage
{
    // Local variables.
    private string cButtonShare;
    private string cButtonClose;
    private string cErrorTitle;

    public PageScan()
	{
        try
        {
            InitializeComponent();
        }
        catch (Exception ex)
        {
            DisplayAlert("InitializeComponent: PageScan", ex.Message, "OK");
            return;
        }

        //barcodeReader.Options = new ZXing.Net.Maui.BarcodeReaderOptions()
        //{
        //    Formats = BarcodeFormat.All
        //};

        // Put text in the chosen language in the controls and variables.
        lblTitle.Text = CodeLang.BarcodeScanner_Text;
        lblScanCode.Text = CodeLang.ScanCode_Text;
        btnShare.Text = CodeLang.ButtonShare_Text;

        cButtonShare = CodeLang.ButtonShare_Text;
        cButtonClose = CodeLang.ButtonClose_Text;
        cErrorTitle = CodeLang.ErrorTitle_Text;
    }

    private void OnBarcodesDetected(object sender, BarcodeDetectionEventArgs e)
    {
        Dispatcher.Dispatch(() =>
        {
            btnShare.Text = cButtonShare + " " + e.Results[0].Format;
            lblBarcodeResult.Text = e.Results[0].Value;

            btnShare.IsEnabled = true;
        });
    }

    // Button share event.
    private async void OnShareClicked(object sender, EventArgs e)
    {
        // Open link website.
        if (lblBarcodeResult.Text[..8] == "https://" || lblBarcodeResult.Text[..7] == "http://")
        {
            OpenWebsiteLink(lblBarcodeResult.Text);
        }

        // Write text to file.
        else
        {
            string cFileName = Path.Combine(FileSystem.CacheDirectory, "BarcodeScanner.txt");

            try
            {
                using StreamWriter sw = new(cFileName, false);

                // Write the text to a file.
                sw.WriteLine(lblBarcodeResult.Text);

                // Close the StreamWriter object.
                sw.Close();
            }
            catch (Exception ex)
            {
                await DisplayAlert(cErrorTitle, ex.Message, cButtonClose);
                return;
            }

            // Open the share interface to share the file.
            await OpenShareInterface(cFileName);
        }
    }

    // Open website.
    private async void OpenWebsiteLink(string cUrl)
    {
        try
        {
            await Launcher.OpenAsync(new Uri(cUrl));
        }
        catch (Exception ex)
        {
            await DisplayAlert(cErrorTitle, ex.Message, cButtonClose);
        }
    }
    // Open the share interface.
    private static async Task OpenShareInterface(string cFile)
    {
        await Share.Default.RequestAsync(new ShareFileRequest
        {
            Title = "Barcode Scanner",
            File = new ShareFile(cFile)
        });
    }
}