using BarcodeScanning;

namespace BarcodeGenerator;

public partial class PageScanNT : ContentPage
{
    private readonly BarcodeDrawable _drawable = new();
    private readonly List<string> qualitys = [];
    private int nQualityCameraBack = 3;

    public PageScanNT()
	{
        try
        {
            InitializeComponent();
        }
        catch (Exception ex)
        {
            Crashes.TrackError(ex);
            DisplayAlert("InitializeComponent: PageScanNT", ex.Message, "OK");
            return;
        }

        // Set the camera enabled off and on because after opening the page for the first time the scanning is not always working.
        // Does also not help always.
        barcodeReader.PauseScanning = true;
        barcodeReader.CameraEnabled = false;
        Task.Delay(400).Wait();
        barcodeReader.CameraEnabled = true;
        barcodeReader.PauseScanning = false;

        // Set the title and quality for the picker.
        // No support for the highest quality:
        // - iPad 8, iOS version 17.1.1 (back and front camera)
        // - iPhone 7, iOS version 15.8 (only front camera).
        pckCameraQuality.Title = CodeLang.CameraQualityTitle_Text + ": " + CodeLang.CameraQualityHigh_Text;

        qualitys.Add(CodeLang.CameraQualityLowest_Text);
        qualitys.Add(CodeLang.CameraQualityLow_Text);
        qualitys.Add(CodeLang.CameraQualityMedium_Text);
        qualitys.Add(CodeLang.CameraQualityHigh_Text);
#if ANDROID
        qualitys.Add(CodeLang.CameraQualityHighest_Text);
#endif
        //if (DeviceInfo.Platform == DevicePlatform.iOS && DeviceInfo.Idiom == DeviceIdiom.Phone)
        //{
        //    qualitys.Add(CodeLang.CameraQualityHighest_Text);
        //}

        pckCameraQuality.ItemsSource = qualitys;
        
        // Set the default quality for the back camera to high.
        pckCameraQuality.SelectedIndex = 3;

        // The height of the title bar is lower when an iPhone is in horizontal position.
        if (DeviceInfo.Platform == DevicePlatform.iOS && DeviceInfo.Idiom == DeviceIdiom.Phone)
        {
            lblTitle.VerticalOptions = LayoutOptions.Start;
            lblTitle.VerticalTextAlignment = TextAlignment.Start;
        }

#if ANDROID
        // Set the barcodes in the picker for Android.
        pckFormatCodeScanner.ItemsSource = Globals.GetFormatCodeListScannerNativeAndroid();
#elif IOS
        // Set the barcodes in the picker for iOS.
        pckFormatCodeScanner.ItemsSource = Globals.GetFormatCodeListScannerNativeIOS();

        // Setting the image button property InputTransparent="True" does not work on iOS. !!!BUG!!!?
        imgbtnCameraQuality.InputTransparent = false;
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
            SetScannerBarcodeFormatAndroid(selectedIndex);
#elif IOS
            SetScannerBarcodeFormatIOS(selectedIndex);
#endif

            barcodeReader.CameraEnabled = true;
            barcodeReader.PauseScanning = false;
            imgbtnCameraDetecting.Source = "camera_detect_off_128x128p.png";
        }
    }

    // Set the scanner barcode format for Android.
    private void SetScannerBarcodeFormatAndroid(int selectedIndex)
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
            _ => BarcodeFormats.All
        };
    }

    // // Set the scanner barcode format for iOS.
    private void SetScannerBarcodeFormatIOS(int selectedIndex)
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
            _ => BarcodeFormats.All
        };
    }

    // Called by the Appearing event from the PageScanNT.xaml.
    protected override async void OnAppearing()
    {
        // Ask for permission to use the camera.
        _ = await Methods.AskForRequiredPermissionAsync();
        base.OnAppearing();

        // Set the camera torch off - (does not work - !!!BUG!!!?).
        barcodeReader.TorchOn = false;
        imgbtnCameraTorch.Source = "camera_torch_off_64x64p.png";

        // Enable the camera.
        barcodeReader.CameraEnabled = true;
        Graphics.Drawable = _drawable;

        // Set language text to speech using the Appearing event of the PageScanNT.xaml.
        lblTextToSpeech.Text = Globals.GetIsoLanguageCode();
    }

    // Called by the Disappearing event from the PageScanNT.xaml.
    protected override void OnDisappearing()
    {
        // Set the camera torch off - (does not work - !!!BUG!!!?).
        barcodeReader.TorchOn = false;
        imgbtnCameraTorch.Source = "camera_torch_off_64x64p.png";

        // Disable the camera.
        base.OnDisappearing();
        barcodeReader.CameraEnabled = false;

        // Cancel the text to speech when going back to the mainpage.
        imgbtnTextToSpeech.Source = Globals.CancelTextToSpeech();
    }

    // CameraView OnDetected event.
    private async void OnCameraDetectionFinished(object sender, OnDetectionFinishedEventArg e)
    {
        imgbtnCopyToClipboard.IsEnabled = false;
        btnShare.IsEnabled = false;
        imgbtnTextToSpeech.IsEnabled = false;

        lblBarcodeResult.Text = "";
        string cBarcodeFormat = "";
        string cDisplayValue = "";
        List<string> listBarcodes = [];

        try
        {
            _drawable.barcodeResults = e.BarcodeResults;
            Graphics.Invalidate();

            foreach (var barcode in e.BarcodeResults)
            {
                cBarcodeFormat = barcode.BarcodeFormat.ToString();
                cDisplayValue = barcode.DisplayValue;

                // Add the barcode format and display value to the list 'listBarcodes'.
                listBarcodes.Add($"{cBarcodeFormat}:\n{cDisplayValue}");
            }

            // Remove duplicates.
            listBarcodes = listBarcodes.Distinct().ToList();

            // Sort the list.
            listBarcodes.Sort();

            // Set the barcode results in the label 'lblBarcodeResult.Text'.
            if (listBarcodes.Count == 1)
            {
                btnShare.Text = $"{CodeLang.ButtonShare_Text} {cBarcodeFormat}";
                lblBarcodeResult.Text = cDisplayValue;
            }
            else if (listBarcodes.Count > 1)
            {
                btnShare.Text = CodeLang.ButtonShare_Text;
                foreach (string barcode in listBarcodes)
                {
                    lblBarcodeResult.Text = $"{lblBarcodeResult.Text}{barcode}\n\n";
                }
            }
            else
            {
                return;
            }

            imgbtnCopyToClipboard.IsEnabled = true;
            btnShare.IsEnabled = true;
            imgbtnTextToSpeech.IsEnabled = true;
        }
        catch (Exception ex)
        {
            Crashes.TrackError(ex);
            await DisplayAlert(CodeLang.ErrorTitle_Text, ex.Message, CodeLang.ButtonClose_Text);
        }
    }

    // ImageButton camera quality clicked event to open the picker.
    // Setting the image button property InputTransparent="True" does not work on iOS. !!!BUG!!!?
    private void OnCameraQualityClicked(object sender, EventArgs e)
    {
#if IOS
        pckCameraQuality.Focus();
#endif
    }

    // Picker quality changed event.
    private async void OnCameraQualityChanged(object sender, EventArgs e)
    {
        var picker = (Picker)sender;
        int selectedIndex = picker.SelectedIndex;

        // If (the high) or the highest quality is selected and the front camera is used then set the quality to medium.
        // The high and highest quality are not on every device supported by the front camera.
        if (selectedIndex > 2 && barcodeReader.CameraFacing == CameraFacing.Front)
        {
            barcodeReader.CaptureQuality = CaptureQuality.Medium;
        }
        // Set the quality for the camera.
        else if (selectedIndex > -1 && selectedIndex < 5)
        {
            try
            {
                barcodeReader.CaptureQuality = selectedIndex switch
                {
                    0 => CaptureQuality.Lowest,
                    1 => CaptureQuality.Low,
                    2 => CaptureQuality.Medium,
                    3 => CaptureQuality.High,
                    4 => CaptureQuality.Highest,
                    _ => CaptureQuality.High
                };
            }
            catch (Exception ex)
            {
                var properties = new Dictionary<string, string> {
                    { "File:", "PageScanNT.xaml.cs" },
                    { "Method:", "OnCameraQualityChanged" },
                    { "CameraFacing:", Convert.ToString(barcodeReader.CameraFacing) },
                    { "selectedIndex:", Convert.ToString(selectedIndex) }
                };
                Crashes.TrackError(ex, properties);
                await DisplayAlert(CodeLang.ErrorTitle_Text, ex.Message, CodeLang.ButtonClose_Text);
                return;
            }

            // Set the title for the picker.
            string cTitle = $"{CodeLang.CameraQualityTitle_Text}: ";
            picker.Title = selectedIndex switch
            {
                0 => cTitle + CodeLang.CameraQualityLowest_Text,
                1 => cTitle + CodeLang.CameraQualityLow_Text,
                2 => cTitle + CodeLang.CameraQualityMedium_Text,
                3 => cTitle + CodeLang.CameraQualityHigh_Text,
                4 => cTitle + CodeLang.CameraQualityHighest_Text,
                _ => cTitle + CodeLang.CameraQualityHigh_Text
            };
        }
    }

    // ImageButton camera facing clicked event.
    private void OnCameraFacingClicked(object sender, EventArgs e)
    {
        // If (the high) or the highest quality is selected and the front camera is used then set the quality to medium.
        // The high and highest quality are not on every device supported by the front camera.
        if (barcodeReader.CameraFacing == CameraFacing.Back)
        {
            nQualityCameraBack = pckCameraQuality.SelectedIndex;

            // Set the quality to medium when the front camera is used.
            if (pckCameraQuality.SelectedIndex > 2)
            {
                pckCameraQuality.SelectedIndex = 2;
            }

            barcodeReader.CameraFacing = CameraFacing.Front;
        }
        else
        {          
            // Set the quality to the saved setting from the back camera.
            barcodeReader.CameraFacing = CameraFacing.Back;
            pckCameraQuality.SelectedIndex = nQualityCameraBack;
        }
    }

    // ImageButton camera detecting clicked event.
    private void OnCameraDetectingClicked(object sender, EventArgs e)
    {
        if (barcodeReader.PauseScanning)
        {
            barcodeReader.PauseScanning = false;
            imgbtnCameraDetecting.Source = "camera_detect_off_128x128p.png";
        }
        else
        {
            barcodeReader.PauseScanning = true;
            imgbtnCameraDetecting.Source = "camera_detect_on_128x128p.png";
        }
    }

    // ImageButton camera vibrate clicked event.
    private void OnCameraVibrateClicked(object sender, EventArgs e)
    {
        if (barcodeReader.VibrationOnDetected)
        {
            barcodeReader.VibrationOnDetected = false;
            imgbtnCameraVibrate.Source = "camera_vibrate_off_128x128p.png";
        }
        else
        {
            barcodeReader.VibrationOnDetected = true;
            imgbtnCameraVibrate.Source = "camera_vibrate_on_128x128p.png";
        }
    }

    // ImageButton torch clicked event.
    private void OnCameraTorchClicked(object sender, EventArgs e)
    {
        if (barcodeReader.TorchOn)
        {
            barcodeReader.TorchOn = false;
            imgbtnCameraTorch.Source = "camera_torch_off_64x64p.png";
        }
        else
        {
            barcodeReader.TorchOn = true;
            imgbtnCameraTorch.Source = "camera_torch_on_64x64p.png";
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
        if (lblBarcodeResult.Text.Length > 0)
        {
            await Clipboard.Default.SetTextAsync(lblBarcodeResult.Text);
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
                canvas.StrokeColor = Colors.Green;
                var scale = 1 / canvas.DisplayScale;
                canvas.Scale(scale, scale);

                foreach (var barcode in barcodeResults)
                {
                    canvas.DrawRectangle(barcode.BoundingBox);
                }
            }
        }
    }
}