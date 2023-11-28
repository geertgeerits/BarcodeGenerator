using BarcodeScanning;

namespace BarcodeGenerator;

public partial class PageScanNT : ContentPage
{
    private readonly BarcodeDrawable _drawable = new();
    private readonly List<string> qualitys = new();

    public PageScanNT()
	{
		InitializeComponent();

        // Set the qualitys for the picker.
        qualitys.Add("Lowest");
        qualitys.Add("Low");
        qualitys.Add("Medium");
        qualitys.Add("High");
        qualitys.Add("Highest");

        Quality.ItemsSource = qualitys;

        // The height of the title bar is lower when an iPhone is in horizontal position.
        if (DeviceInfo.Platform == DevicePlatform.iOS && DeviceInfo.Idiom == DeviceIdiom.Phone)
        {
            lblTitle.VerticalOptions = LayoutOptions.Start;
            lblTitle.VerticalTextAlignment = TextAlignment.Start;
            imgbtnCameraFacing.VerticalOptions = LayoutOptions.Start;
            imgbtnCameraStartStop.VerticalOptions = LayoutOptions.Start;
            imgbtnCameraTorch.VerticalOptions = LayoutOptions.Start;
        }

        // Put text in the chosen language in the controls.
        pckFormatCodeScanner.ItemsSource = Globals.GetFormatCodeListScannerNative();

        // Default format code = All codes
        pckFormatCodeScanner.SelectedIndex = Globals.nFormatScannerIndex;

        // Initialize text to speech.
        if (Globals.InitializeTextToSpeechScannerAsync("PageScanNT.xaml.cs").Result)
        {
            lblTextToSpeech.IsVisible = true;
            imgbtnTextToSpeech.IsVisible = true;
        }

        // For testing crashes - DivideByZeroException.
        //int divByZero = 51 / int.Parse("0");
    }

    // Set the scanner properties for the selected format code.
    // Native nativeCameraView options.
    private void OnPickerFormatCodeChanged(object sender, EventArgs e)
    {
        var picker = (Picker)sender;
        int selectedIndex = picker.SelectedIndex;

        if (selectedIndex != -1)
        {
            barcodeReader.CameraEnabled = false;
            
            lblBarcodeResult.Text = "";
            btnShare.Text = CodeLang.ButtonShare_Text;

            imgbtnCopyToClipboard.IsEnabled = false;
            btnShare.IsEnabled = false;
            imgbtnTextToSpeech.IsEnabled = false;

            switch (selectedIndex)
            {
                // Aztec.
                case 0:
                    barcodeReader.BarcodeSymbologies = BarcodeFormats.Aztec;
                    break;

                // Codabar.
                case 1:
                    barcodeReader.BarcodeSymbologies = BarcodeFormats.CodaBar;
                    break;

                // Code39.
                case 2:
                    barcodeReader.BarcodeSymbologies = BarcodeFormats.Code39;
                    break;

                // Code93.
                case 3:
                    barcodeReader.BarcodeSymbologies = BarcodeFormats.Code93;
                    break;

                // Code128.
                case 4:
                    barcodeReader.BarcodeSymbologies = BarcodeFormats.Code128;
                    break;

                // DataMatrix.
                case 5:
                    barcodeReader.BarcodeSymbologies = BarcodeFormats.DataMatrix;
                    break;

                // Ean8.
                case 6:
                    barcodeReader.BarcodeSymbologies = BarcodeFormats.Ean8;
                    break;

                // Ean13.
                case 7:
                    barcodeReader.BarcodeSymbologies = BarcodeFormats.Ean13;
                    break;

                // GS1DataBar.
                case 8:
                    barcodeReader.BarcodeSymbologies = BarcodeFormats.GS1DataBar;
                    break;

                // Itf.
                case 9:
                    barcodeReader.BarcodeSymbologies = BarcodeFormats.Itf;
                    break;

                // MicroPdf417.
                case 10:
                    barcodeReader.BarcodeSymbologies = BarcodeFormats.MicroPdf417;
                    break;

                // MicroQR.
                case 11:
                    barcodeReader.BarcodeSymbologies = BarcodeFormats.MicroQR;
                    break;

                // Pdf417.
                case 12:
                    barcodeReader.BarcodeSymbologies = BarcodeFormats.Pdf417;
                    break;

                // QrCode.
                case 13:
                    barcodeReader.BarcodeSymbologies = BarcodeFormats.QRCode;
                    break;

                // UpcA.
                case 14:
                    barcodeReader.BarcodeSymbologies = BarcodeFormats.Upca;
                    break;

                // UpcE.
                case 15:
                    barcodeReader.BarcodeSymbologies = BarcodeFormats.Upce;
                    break;

                // All.
                case 16:
                    barcodeReader.BarcodeSymbologies = BarcodeFormats.All;
                    break;
            }
            
            barcodeReader.CameraEnabled = true;
        }
    }

    // Called by the Appearing event from the PageScanNT.xaml.
    protected override async void OnAppearing()
    {
        await Methods.AskForRequiredPermissionAsync();
        base.OnAppearing();

        barcodeReader.VibrationOnDetected = true;
        barcodeReader.PauseScanning = false;
        barcodeReader.CameraEnabled = true;
        barcodeReader.CameraFacing = CameraFacing.Back;

        Graphics.Drawable = _drawable;

        // Set language text to speech using the Appearing event of the PageScanNT.xaml.
        lblTextToSpeech.Text = Globals.GetIsoLanguageCode();
    }

    // Called by the Disappearing event from the PageScanNT.xaml.
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        barcodeReader.CameraEnabled = false;

        // Cancel the text to speech when going back to the mainpage.
        imgbtnTextToSpeech.Source = Globals.CancelTextToSpeech();
    }

    // CameraView OnDetected event.
    private void CameraView_OnDetectionFinished(object sender, OnDetectionFinishedEventArg e)
    {
        _drawable.barcodeResults = e.BarcodeResults;
        Graphics.Invalidate();

        string cBarcodeFormat = "";
        string cDisplayValue = "";

        foreach (var barcode in _drawable.barcodeResults)
        {
            cBarcodeFormat = barcode.BarcodeFormat.ToString();
            cDisplayValue = barcode.DisplayValue;
        }

        btnShare.Text = $"{CodeLang.ButtonShare_Text} {cBarcodeFormat}";
        lblBarcodeResult.Text = cDisplayValue;

        imgbtnCopyToClipboard.IsEnabled = true;
        btnShare.IsEnabled = true;
        imgbtnTextToSpeech.IsEnabled = true;
    }

    // ImageButton camera facing clicked event.
    private void OnCameraFacingClicked(object sender, EventArgs e)
    {
        barcodeReader.CameraFacing = barcodeReader.CameraFacing == CameraFacing.Back ? CameraFacing.Front : CameraFacing.Back;
    }

    // ImageButton camera start/stop clicked event.
    private void OnCameraStartStopClicked(object sender, EventArgs e)
    {
        if (barcodeReader.PauseScanning)
        {
            barcodeReader.PauseScanning = false;
            imgbtnCameraStartStop.Source = "camera_stop_128x128p.png";
        }
        else
        {
            barcodeReader.PauseScanning = true;
            imgbtnCameraStartStop.Source = "camera_start_128x128p.png";
        }
    }

    // ImageButton torch clicked event.
    private void OnCameraTorchClicked(object sender, EventArgs e)
    {
        barcodeReader.TorchOn = !barcodeReader.TorchOn;
    }

    // ImageButton camera vibrite clicked event.
    private void VibrateButton_Clicked(object sender, EventArgs e)
    {
        barcodeReader.VibrationOnDetected = !barcodeReader.VibrationOnDetected;
    }

    // Picker quality changed event.
    private void Quality_SelectedIndexChanged(object sender, EventArgs e)
    {
        var picker = (Picker)sender;
        if (picker.SelectedIndex > -1 && picker.SelectedIndex < 5)
            barcodeReader.CaptureQuality = (CaptureQuality)picker.SelectedIndex;
    }

    // Class for drawing the barcode bounding box.
    private class BarcodeDrawable : IDrawable
    {
        public HashSet<BarcodeResult>? barcodeResults;
        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            if (barcodeResults is not null && barcodeResults.Count > 0)
            {
                canvas.StrokeSize = 15;
                canvas.StrokeColor = Colors.Red;
                var scale = 1 / canvas.DisplayScale;
                canvas.Scale(scale, scale);

                foreach (var barcode in barcodeResults)
                {
                    canvas.DrawRectangle(barcode.BoundingBox);
                    //Console.WriteLine(barcode.BarcodeFormat + ": " + barcode.DisplayValue);  // For testing.
                }
            }
        }
    }

    // Button share event.
    private void OnShareClicked(object sender, EventArgs e)
    {
        _ = Globals.ShareBarcodeResultAsync(lblBarcodeResult.Text);
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
        await Clipboard.Default.SetTextAsync(lblBarcodeResult.Text);
    }
}