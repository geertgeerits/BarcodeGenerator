using BarcodeGenerator.Resources.Languages;
using ZXing.Net.Maui;

namespace BarcodeGenerator;

public partial class PageScan : ContentPage
{
    // Local variables.
    private string cButtonShare;
    private string cButtonClose;
    private string cOpenLinkTitle;
    private string cOpenLinkText;
    private string cYes;
    private string cNo;
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

        barcodeReader.Options = new BarcodeReaderOptions()
        {
            AutoRotate = true,
            Formats = BarcodeFormats.All
        };

        // Put text in the chosen language in the controls and variables.
        lblTitle.Text = CodeLang.BarcodeScanner_Text;
        lblScanCode.Text = CodeLang.ScanCode_Text;
        btnShare.Text = CodeLang.ButtonShareText_Text;

        cButtonShare = CodeLang.ButtonShareText_Text;
        cButtonClose = CodeLang.ButtonClose_Text;
        cOpenLinkTitle = CodeLang.OpenLinkTitle_Text;
        cOpenLinkText = CodeLang.OpenLinkText_Text;
        cYes = CodeLang.Yes_Text;
        cNo = CodeLang.No_Text;
        cErrorTitle = CodeLang.ErrorTitle_Text;
    }

    // ImageButton torch clicked event.
    private void OnTorchClicked(object sender, EventArgs e)
    {
        if (barcodeReader.IsTorchOn == false)
        {
            barcodeReader.IsTorchOn = true;
        }
        else
        {
            barcodeReader.IsTorchOn = false;
        }
    }
    
    // Barcode detected event.
    private async void OnBarcodesDetected(object sender, BarcodeDetectionEventArgs e)
    {
        try
        {
            Dispatcher.Dispatch(() =>
            {
                btnShare.Text = cButtonShare + " " + e.Results[0].Format;
                lblBarcodeResult.Text = e.Results[0].Value;

                btnShare.IsEnabled = true;
            });
        }
        catch (Exception ex)
        {
            await DisplayAlert(cErrorTitle, ex.Message, cButtonClose);
        }
    }

    // Button share event.
    private async void OnShareClicked(object sender, EventArgs e)
    {
        //lblBarcodeResult.Text = "https://www.google.be/";
        //lblBarcodeResult.Text = "dkjfkfjd ke kejk k kjkerjl kjkjekrjk";

        // Open/Share link website.
        if (lblBarcodeResult.Text[..8] == "https://" || lblBarcodeResult.Text[..7] == "http://")
        {
            bool bAnswer = await DisplayAlert(cOpenLinkTitle, cOpenLinkText, cYes, cNo);

            // Open link website.
            if (bAnswer)
            {
                OpenWebsiteLink(lblBarcodeResult.Text);
            }
            // Share link website.
            else
            {
                await ShareText(lblBarcodeResult.Text);
            }
        }

        // Share the text.
        else
        {
            await ShareText(lblBarcodeResult.Text);
        }
    }

    // Open the website link.
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
    public async Task ShareText(string cText)
    {
        try
        {
            await Share.Default.RequestAsync(new ShareTextRequest
            {
                Text = cText,
                Title = "Barcode Scanner"
            });
        }
        catch (Exception ex)
        {
            await DisplayAlert(cErrorTitle, ex.Message, cButtonClose);
        }
    }
}