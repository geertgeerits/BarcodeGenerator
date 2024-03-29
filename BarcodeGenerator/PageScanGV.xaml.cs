using BarcodeScanner.Mobile;

namespace BarcodeGenerator;

public partial class PageScanGV : ContentPage
{
    public PageScanGV()
    {
        try
        {
            switch (Globals.nFormatScannerIndex)
            {
                // Aztec.
                case 0:
                    BarcodeScanner.Mobile.Methods.SetSupportBarcodeFormat(BarcodeScanner.Mobile.BarcodeFormats.Aztec);
                    break;

                // Codabar.
                case 1:
                    BarcodeScanner.Mobile.Methods.SetSupportBarcodeFormat(BarcodeScanner.Mobile.BarcodeFormats.CodaBar);
                    break;

                // Code128.
                case 2:
                    BarcodeScanner.Mobile.Methods.SetSupportBarcodeFormat(BarcodeScanner.Mobile.BarcodeFormats.Code128);
                    break;

                // Code39.
                case 3:
                    BarcodeScanner.Mobile.Methods.SetSupportBarcodeFormat(BarcodeScanner.Mobile.BarcodeFormats.Code39);
                    break;

                // Code93.
                case 4:
                    BarcodeScanner.Mobile.Methods.SetSupportBarcodeFormat(BarcodeScanner.Mobile.BarcodeFormats.Code93);
                    break;

                // DataMatrix.
                case 5:
                    BarcodeScanner.Mobile.Methods.SetSupportBarcodeFormat(BarcodeScanner.Mobile.BarcodeFormats.DataMatrix);
                    break;

                // Ean13.
                case 6:
                    BarcodeScanner.Mobile.Methods.SetSupportBarcodeFormat(BarcodeScanner.Mobile.BarcodeFormats.Ean13);
                    break;

                // Ean8.
                case 7:
                    BarcodeScanner.Mobile.Methods.SetSupportBarcodeFormat(BarcodeScanner.Mobile.BarcodeFormats.Ean8);
                    break;

                // Imb.
                case 8:
                    BarcodeScanner.Mobile.Methods.SetSupportBarcodeFormat(BarcodeScanner.Mobile.BarcodeFormats.All);
                    break;

                // Itf.
                case 9:
                    BarcodeScanner.Mobile.Methods.SetSupportBarcodeFormat(BarcodeScanner.Mobile.BarcodeFormats.Itf);
                    break;

                // MaxiCode.
                case 10:
                    BarcodeScanner.Mobile.Methods.SetSupportBarcodeFormat(BarcodeScanner.Mobile.BarcodeFormats.All);
                    break;

                // Msi.
                case 11:
                    BarcodeScanner.Mobile.Methods.SetSupportBarcodeFormat(BarcodeScanner.Mobile.BarcodeFormats.All);
                    break;

                // Pdf417.
                case 12:
                    BarcodeScanner.Mobile.Methods.SetSupportBarcodeFormat(BarcodeScanner.Mobile.BarcodeFormats.Pdf417);
                    break;

                // PharmaCode.
                case 13:
                    BarcodeScanner.Mobile.Methods.SetSupportBarcodeFormat(BarcodeScanner.Mobile.BarcodeFormats.All);
                    break;

                // Plessey.
                case 14:
                    BarcodeScanner.Mobile.Methods.SetSupportBarcodeFormat(BarcodeScanner.Mobile.BarcodeFormats.All);
                    break;

                // QrCode.
                case 15:
                    BarcodeScanner.Mobile.Methods.SetSupportBarcodeFormat(BarcodeScanner.Mobile.BarcodeFormats.QRCode);
                    break;

                // Rss14.
                case 16:
                    BarcodeScanner.Mobile.Methods.SetSupportBarcodeFormat(BarcodeScanner.Mobile.BarcodeFormats.All);
                    break;

                // RssExpanded.
                case 17:
                    BarcodeScanner.Mobile.Methods.SetSupportBarcodeFormat(BarcodeScanner.Mobile.BarcodeFormats.All);
                    break;

                // UpcA.
                case 18:
                    BarcodeScanner.Mobile.Methods.SetSupportBarcodeFormat(BarcodeScanner.Mobile.BarcodeFormats.Upca);
                    break;

                // UpcE.
                case 19:
                    BarcodeScanner.Mobile.Methods.SetSupportBarcodeFormat(BarcodeScanner.Mobile.BarcodeFormats.Upce);
                    break;

                // UpcEanExtension.
                case 20:
                    BarcodeScanner.Mobile.Methods.SetSupportBarcodeFormat(BarcodeScanner.Mobile.BarcodeFormats.All);
                    break;

                // All.
                case 21:
                    //BarcodeScanner.Mobile.Methods.SetSupportBarcodeFormat(BarcodeScanner.Mobile.BarcodeFormats.All);
                    break;
            }

            InitializeComponent();
        }
        catch (Exception ex)
        {
            Crashes.TrackError(ex);
            DisplayAlert("InitializeComponent: PageScanGV", ex.Message, "OK");
            return;
        }

        // Ask for permission to use the camera.
        BarcodeScanner.Mobile.Methods.AskForRequiredPermission();

        // The height of the title bar is lower when an iPhone is in horizontal position.
        if (DeviceInfo.Platform == DevicePlatform.iOS && DeviceInfo.Idiom == DeviceIdiom.Phone)
        {
            lblTitle.VerticalOptions = LayoutOptions.Start;
            lblTitle.VerticalTextAlignment = TextAlignment.Start;
            imgbtnTorch.VerticalOptions = LayoutOptions.Start;
        }

        // Put the default barcode format in the label control.
        string[] cFormatCode = [.. Globals.GetFormatCodeListScanner()];
        lblFormatCodeScanner.Text = cFormatCode[Globals.nFormatScannerIndex];

        // Initialize text to speech.
        if (Globals.InitializeTextToSpeechScannerAsync("PageScanGV.xaml.cs").Result)
        {
            lblTextToSpeech.IsVisible = true;
            imgbtnTextToSpeech.IsVisible = true;
        }

        // For testing crashes - DivideByZeroException.
        //int divByZero = 51 / int.Parse("0");
    }

    // Barcode detected event.
    private async void OnBarcodesDetected(object sender, OnDetectedEventArg e)
    {
        lblBarcodeResult.Text = "";

        imgbtnCopyToClipboard.IsEnabled = false;
        btnShare.IsEnabled = false;
        imgbtnTextToSpeech.IsEnabled = false;

        try
        {
            List<BarcodeResult> obj = e.BarcodeResults;

            string cBarcodeFormat = Convert.ToString(obj[0].BarcodeFormat);
            string cDisplayValue = obj[0].DisplayValue;

            _ = Dispatcher.Dispatch(() =>
            {
                btnShare.Text = $"{CodeLang.ButtonShare_Text} {cBarcodeFormat}";
                lblBarcodeResult.Text = cDisplayValue;

                imgbtnCopyToClipboard.IsEnabled = true;
                btnShare.IsEnabled = true;
                imgbtnTextToSpeech.IsEnabled = true;

                barcodeReader.IsScanning = true;
            });
        }
        catch (Exception ex)
        {
            await DisplayAlert(CodeLang.ErrorTitle_Text, ex.Message, CodeLang.ButtonClose_Text);
        }
    }

    // Button share event.
    private void OnShareClicked(object sender, EventArgs e)
    {
        _ = Globals.ShareBarcodeResultAsync(lblBarcodeResult.Text);
    }

    // Set language text to speech using the Appearing event of the PageScanGV.xaml.
    private void OnPageAppearing(object sender, EventArgs e)
    {
        lblTextToSpeech.Text = Globals.GetIsoLanguageCode();
    }

    // ImageButton torch clicked event.
    private void OnTorchClicked(object sender, EventArgs e)
    {
        barcodeReader.TorchOn = !barcodeReader.TorchOn;
    }

    // Cancel the text to speech and turn off the torch if on, when going back to the mainpage.
    // Called by the Disappearing event from the PageScanGV.xaml.
    private void OnPageDisappearing(object sender, EventArgs e)
    {
        imgbtnTextToSpeech.Source = Globals.CancelTextToSpeech();

        if (barcodeReader.TorchOn)
        {
            barcodeReader.TorchOn = false;
            Task.Delay(300).Wait();
        }
    }

    // Button text to speech event - Convert text to speech.
    private void OnTextToSpeechClicked(object sender, EventArgs e)
    {
        // Cancel the text to speech.
        if (Globals.bTextToSpeechIsBusy)
        {
            imgbtnTextToSpeech.Source = Globals.CancelTextToSpeech();
            return;
        }

        // Convert the text to speech.
        _ = Globals.ConvertTextToSpeechAsync(imgbtnTextToSpeech, lblBarcodeResult.Text);
    }

    // Copy text to the clipboard clicked event.
    private async void OnCopyToClipboardClicked(object sender, EventArgs e)
    {
        if (lblBarcodeResult.Text.Length > 0)
        {
            await Clipboard.Default.SetTextAsync(lblBarcodeResult.Text);
        }
    }
}
