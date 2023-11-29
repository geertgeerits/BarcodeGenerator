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
            imgbtnCameraStartStop.VerticalOptions = LayoutOptions.Start;
            imgbtnCameraFacing.VerticalOptions = LayoutOptions.Start;
            imgbtnCameraTorch.VerticalOptions = LayoutOptions.Start;
        }

        // Put text in the chosen language in the controls.
#if ANDROID        
        pckFormatCodeScanner.ItemsSource = Globals.GetFormatCodeListScannerNativeAndroid();
#elif IOS
        pckFormatCodeScanner.ItemsSource = Globals.GetFormatCodeListScannerNativeIOS();
#endif

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

#if ANDROID
            SetScannerPropertiesAndroid(selectedIndex);
#elif IOS
            SetScannerPropertiesIOS(selectedIndex);
#endif

            barcodeReader.CameraEnabled = true;
        }
    }

    // Set the scanner properties for the selected format code for Android.
    private void SetScannerPropertiesAndroid(int selectedIndex)
    {
        barcodeReader.BarcodeSymbologies = selectedIndex switch
        {
            0 => BarcodeFormats.All,
            1 => BarcodeFormats.Aztec,
            2 => BarcodeFormats.CodaBar,
            3 => BarcodeFormats.Code39,
            4 => BarcodeFormats.Code93,
            5 => BarcodeFormats.Code128,
            6 => BarcodeFormats.DataMatrix,
            7 => BarcodeFormats.Ean8,
            8 => BarcodeFormats.Ean13,
            9 => BarcodeFormats.Itf,
            10 => BarcodeFormats.Pdf417,
            11 => BarcodeFormats.QRCode,
            12 => BarcodeFormats.Upca,
            13 => BarcodeFormats.Upce,
            _ => BarcodeFormats.All,
        };
    }

    // Set the scanner properties for the selected format code for iOS.
    private void SetScannerPropertiesIOS(int selectedIndex)
    {
        barcodeReader.BarcodeSymbologies = selectedIndex switch
        {
            0 => BarcodeFormats.All,
            1 => BarcodeFormats.Aztec,
            2 => BarcodeFormats.CodaBar,
            3 => BarcodeFormats.Code39,
            4 => BarcodeFormats.Code93,
            5 => BarcodeFormats.Code128,
            6 => BarcodeFormats.DataMatrix,
            7 => BarcodeFormats.Ean8,
            8 => BarcodeFormats.Ean13,
            9 => BarcodeFormats.GS1DataBar,
            10 => BarcodeFormats.Itf,
            11 => BarcodeFormats.MicroPdf417,
            12 => BarcodeFormats.MicroQR,
            13 => BarcodeFormats.Pdf417,
            14 => BarcodeFormats.QRCode,
            15 => BarcodeFormats.Upca,
            16 => BarcodeFormats.Upce,
            _ => BarcodeFormats.All,
        };
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

    // ImageButton camera facing clicked event.
    private void OnCameraFacingClicked(object sender, EventArgs e)
    {
        barcodeReader.CameraFacing = barcodeReader.CameraFacing == CameraFacing.Back ? CameraFacing.Front : CameraFacing.Back;
    }

    // ImageButton torch clicked event.
    private void OnCameraTorchClicked(object sender, EventArgs e)
    {
        barcodeReader.TorchOn = !barcodeReader.TorchOn;
    }

    // ImageButton camera vibrite clicked event.
    private void OnCameraVibrateClicked(object sender, EventArgs e)
    {
        barcodeReader.VibrationOnDetected = !barcodeReader.VibrationOnDetected;
    }

    // Picker quality changed event.
    private void OnCameraQualityChanged(object sender, EventArgs e)
    {
        var picker = (Picker)sender;

        if (picker.SelectedIndex > -1 && picker.SelectedIndex < 5)
        {
            barcodeReader.CaptureQuality = (CaptureQuality)picker.SelectedIndex;
        }           
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