using BarcodeScanning;

namespace BarcodeGenerator
{
    public sealed partial class PageScanNT : ContentPage
    {
        //// Local variables
        private readonly BarcodeDrawable _drawable = new();
        private readonly List<string> qualities = [];
        private int nQualityCameraBack;
        private int nQualityCameraFront;
        private bool bBeforeLeavingPage;

        public PageScanNT()
    	{
            try
            {
                InitializeComponent();
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
#if DEBUG
                DisplayAlert("InitializeComponent: PageScanNT", ex.Message, "OK");
#endif
                return;
            }

            //// Get the saved quality settings
            nQualityCameraBack = Preferences.Default.Get("SettingQualityCameraBack", 2);
            nQualityCameraFront = Preferences.Default.Get("SettingQualityCameraFront", 1);

#if DEBUG
            //// Read the device information
            //ReadDeviceInfo();
#endif
            //// Set the quality for the pickers
            // No support for the highest quality: iPad 8, iOS version 17.1.1 (back and front camera)
            // No support for the high and the highest quality: iPhone 7, iOS version 15.8 (front camera)
#if IOS
            // iOS front camera quality settings
            qualities.Add(CodeLang.CameraQualityLow_Text);
            qualities.Add(CodeLang.CameraQualityMedium_Text);
            pckCameraQualityFront.ItemsSource = qualities;

            // iOS back camera quality settings
            qualities.Add(CodeLang.CameraQualityHigh_Text);
            pckCameraQualityBack.ItemsSource = qualities;
#else
            // Front camera quality settings
            qualities.Add(CodeLang.CameraQualityLow_Text);
            qualities.Add(CodeLang.CameraQualityMedium_Text);
            qualities.Add(CodeLang.CameraQualityHigh_Text);
            pckCameraQualityFront.ItemsSource = qualities;

            // Back camera quality settings
            qualities.Add(CodeLang.CameraQualityHighest_Text);
            pckCameraQualityBack.ItemsSource = qualities;
#endif
            //// Set the quality for the camera
            pckCameraQualityBack.SelectedIndex = nQualityCameraBack;
            pckCameraQualityFront.SelectedIndex = nQualityCameraFront;

            //// The height of the title bar is lower when an iPhone is in horizontal position
            if (DeviceInfo.Platform == DevicePlatform.iOS && DeviceInfo.Idiom == DeviceIdiom.Phone)
            {
                lblTitle.VerticalOptions = LayoutOptions.Start;
                lblTitle.VerticalTextAlignment = TextAlignment.Start;
            }

#if ANDROID
            //// Set the barcodes in the picker for Android
            pckFormatCodeScanner.ItemsSource = Globals.GetFormatCodeListScannerNativeAndroid();
#elif IOS
            //// Set the barcodes in the picker for iOS
            pckFormatCodeScanner.ItemsSource = Globals.GetFormatCodeListScannerNativeIOS();
#endif
            //// Default format code = All codes
            pckFormatCodeScanner.SelectedIndex = Globals.nFormatScannerIndex;

            //// Initialize text to speech
            if (Globals.InitializeTextToSpeechScannerAsync("PageScanNT.xaml.cs").Result)
            {
                lblTextToSpeech.IsVisible = true;
                imgbtnTextToSpeech.IsVisible = true;
            }

            //// For testing crashes - DivideByZeroException
            //int divByZero = 51 / int.Parse("0");
        }

        /// <summary>
        /// Set the scanner properties for the selected format code
        /// Native nativeCameraView options
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPickerFormatCodeChanged(object sender, EventArgs e)
        {
            var picker = (Picker)sender;
            int nSelectedIndex = picker.SelectedIndex;

            if (nSelectedIndex != -1)
            {
#if ANDROID
                barcodeReader.BarcodeSymbologies = nSelectedIndex switch
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
#elif IOS
                barcodeReader.BarcodeSymbologies = nSelectedIndex switch
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
#endif
            }
        }

        /// <summary>
        /// On BackButton pressed event (does not work in iOS)
        /// </summary>
        /// <returns></returns>
        protected override bool OnBackButtonPressed()
        {
            // To do before leaving this page
            BeforeLeavingPageScanNT();

            Debug.WriteLine("Method: OnBackButtonPressed");

            // Allow the default back button action
            return base.OnBackButtonPressed();
        }

        /// <summary>
        /// Called by the Appearing event from the PageScanNT.xaml
        /// </summary>
        protected override async void OnAppearing()
        {
            // Ask for permission to use the camera
            _ = await Methods.AskForRequiredPermissionAsync();
            base.OnAppearing();

            // Enable the camera
            barcodeReader.CameraEnabled = true;
            Graphics.Drawable = _drawable;

            // Set language text to speech using the Appearing event of the PageScanNT.xaml
            lblTextToSpeech.Text = Globals.GetIsoLanguageCode();
        }

        /// <summary>
        /// Called by the Disappearing event from the PageScanNT.xaml
        /// </summary>
        protected override void OnDisappearing()
        {
            // To do before leaving this page
            BeforeLeavingPageScanNT();

            // Disable the camera
            base.OnDisappearing();
            barcodeReader.CameraEnabled = false;

            Debug.WriteLine("Method: OnDisappearing");
        }

        /// <summary>
        /// To do before leaving this page
        /// </summary>
        private void BeforeLeavingPageScanNT()
        {
            // Check if this method has or not been called
            if (bBeforeLeavingPage)
            {
                return;
            }

            // Save the quality settings
            Preferences.Default.Set("SettingQualityCameraBack", nQualityCameraBack);
            Preferences.Default.Set("SettingQualityCameraFront", nQualityCameraFront);

            // Set the capture quality to low (otherwise the app quit on an iPhone 7 - does not work allways)
            barcodeReader.CaptureQuality = CaptureQuality.Low;

            // Set the camera torch off
            barcodeReader.TorchOn = false;
            imgbtnCameraTorch.Source = "camera_torch_off_64x64p.png";

            // Cancel the text to speech when going back to the mainpage
            imgbtnTextToSpeech.Source = Globals.CancelTextToSpeech();

            // Give it some time to save the settings
            Task.Delay(400).Wait();

            // This method has been called
            bBeforeLeavingPage = true;

            Debug.WriteLine("Method: BeforeLeavingPageScanNT");
        }

        /// <summary>
        /// Unloaded event to disconnect the barcode handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ContentPage_Unloaded(object sender, EventArgs e)
        {
            barcodeReader.Handler?.DisconnectHandler();
            Debug.WriteLine("Method: ContentPage_Unloaded");
        }

        /// <summary>
        /// CameraView OnDetected event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCameraDetectionFinished(object sender, OnDetectionFinishedEventArg e)
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

                    // Add the barcode format and display value to the list 'listBarcodes'
                    listBarcodes.Add($"{cBarcodeFormat}:\n{cDisplayValue}");
                }

                // Remove duplicates
                listBarcodes = listBarcodes.Distinct().ToList();

                // Sort the list
                listBarcodes.Sort();

                // Set the barcode results in the label 'lblBarcodeResult.Text'
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
                SentrySdk.CaptureException(ex);
#if DEBUG
                _ = DisplayAlert(CodeLang.ErrorTitle_Text, ex.Message, CodeLang.ButtonClose_Text);
#endif
            }
        }

        /// <summary>
        /// ImageButton camera quality clicked event to open the picker
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCameraQualityClicked(object sender, EventArgs e)
        {
            if (barcodeReader.CameraFacing == CameraFacing.Back)
            {
                pckCameraQualityBack.Unfocus();
                pckCameraQualityBack.Focus();
            }

            if (barcodeReader.CameraFacing == CameraFacing.Front)
            {
                pckCameraQualityFront.Unfocus();
                pckCameraQualityFront.Focus();
            }

            // Set the title for the picker
            SetTitleCameraQualityPicker();
        }

        /// <summary>
        /// Picker quality changed event for the camera
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnCameraQualityChanged(object sender, EventArgs e)
        {
            int nSelectedIndex = 0;

            switch (barcodeReader.CameraFacing)
            {
                case CameraFacing.Back:
                    nSelectedIndex = pckCameraQualityBack.SelectedIndex;
                    break;
                case CameraFacing.Front:
                    nSelectedIndex = pckCameraQualityFront.SelectedIndex;
                    break;
            }

            try
            {
                // Set the quality for the camera
                if (nSelectedIndex > -1 && nSelectedIndex < 4)
                {
                    barcodeReader.CaptureQuality = nSelectedIndex switch
                    {
                        0 => CaptureQuality.Low,
                        1 => CaptureQuality.Medium,
                        2 => CaptureQuality.High,
                        3 => CaptureQuality.Highest,
                        _ => CaptureQuality.Medium
                    };
                }
            }
            catch (Exception ex)
            {
                _ = SentrySdk.CaptureException(ex);

                await DisplayAlert(CodeLang.ErrorTitle_Text, CodeLang.CameraQualityError_Text, CodeLang.ButtonClose_Text);
            }

            // Set the quality for the picker
            switch (barcodeReader.CameraFacing)
            {
                case CameraFacing.Back:
                    nQualityCameraBack = nSelectedIndex;
                    break;
                case CameraFacing.Front:
                    nQualityCameraFront = nSelectedIndex;
                    break;
            }

            //await DisplayAlert("nSelectedIndex", Convert.ToString(nSelectedIndex), "OK");  // For testing
        }

        /// <summary>
        /// Set the title for the picker
        /// </summary>
        private void SetTitleCameraQualityPicker()
        {
            string cTitle = $"{CodeLang.CameraQualityTitle_Text}: ";

            switch (barcodeReader.CameraFacing)
            {
                case CameraFacing.Back:
                    pckCameraQualityBack.Title = nQualityCameraBack switch
                    {
                        0 => cTitle + CodeLang.CameraQualityLow_Text,
                        1 => cTitle + CodeLang.CameraQualityMedium_Text,
                        2 => cTitle + CodeLang.CameraQualityHigh_Text,
                        3 => cTitle + CodeLang.CameraQualityHighest_Text,
                        _ => cTitle + CodeLang.CameraQualityMedium_Text
                    };
                    break;

                case CameraFacing.Front:
                    pckCameraQualityFront.Title = nQualityCameraFront switch
                    {
                        0 => cTitle + CodeLang.CameraQualityLow_Text,
                        1 => cTitle + CodeLang.CameraQualityMedium_Text,
                        2 => cTitle + CodeLang.CameraQualityHigh_Text,
                        3 => cTitle + CodeLang.CameraQualityHighest_Text,
                        _ => cTitle + CodeLang.CameraQualityMedium_Text
                    };
                    break;
            }
        }

        /// <summary>
        /// ImageButton camera facing clicked event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCameraFacingClicked(object sender, EventArgs e)
        {
            // If the high or the highest quality is selected and the front camera is used then set the quality to medium
            // The high and highest quality are not on every device supported by the front camera
            switch (barcodeReader.CameraFacing)
            {
                case CameraFacing.Back:
                    // Set the quality to medium when the front camera is used
                    if (nQualityCameraFront > 1)
                    {
                        nQualityCameraFront = 1;
                    }

                    pckCameraQualityBack.Unfocus();
                    pckCameraQualityFront.SelectedIndex = nQualityCameraFront;
                    barcodeReader.CameraFacing = CameraFacing.Front;
                    break;
            
                case CameraFacing.Front:
                    // Set the quality to the saved setting from the back camera
                    pckCameraQualityFront.Unfocus();
                    barcodeReader.CameraFacing = CameraFacing.Back;
                    pckCameraQualityBack.SelectedIndex = nQualityCameraBack;
                    break;
            }

            imgbtnCameraTorch.Source = "camera_torch_off_64x64p.png";
        }

        /// <summary>
        /// ImageButton camera detecting clicked event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// ImageButton camera vibrate clicked event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// ImageButton torch clicked event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Button share event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnShareClicked(object sender, EventArgs e)
        {
            _ = Globals.ShareBarcodeResultAsync(lblBarcodeResult.Text);
        }

        /// <summary>
        /// Button text to speech event - Convert text to speech
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTextToSpeechClicked(object sender, EventArgs e)
        {
            // Cancel the text to speech
            if (Globals.bTextToSpeechIsBusy)
            {
                imgbtnTextToSpeech.Source = Globals.CancelTextToSpeech();
                return;
            }

            // Convert the text to speech
            _ = Globals.ConvertTextToSpeechAsync(imgbtnTextToSpeech, lblBarcodeResult.Text);
        }

        /// <summary>
        /// Copy text to the clipboard clicked event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnCopyToClipboardClicked(object sender, EventArgs e)
        {
            if (lblBarcodeResult.Text.Length > 0)
            {
                await Clipboard.Default.SetTextAsync(lblBarcodeResult.Text);
            }
        }

        /// <summary>
        /// Class for drawing the barcode bounding box
        /// </summary>
        private sealed class BarcodeDrawable : IDrawable
        {
            //public HashSet<BarcodeResult>? barcodeResults;    // Till version 1.3.1
            public BarcodeResult[]? barcodeResults;         // From version 1.4.0

            public void Draw(ICanvas canvas, RectF dirtyRect)
            {
                if (barcodeResults is not null && barcodeResults.Length > 0)
                {
                    canvas.StrokeSize = 15;
                    canvas.StrokeColor = Colors.Green;
                    var scale = 1 / canvas.DisplayScale;
                    canvas.Scale(scale, scale);

                    try
                    {
                        foreach (var barcode in barcodeResults)
                        {
                            canvas.DrawRectangle(barcode.PreviewBoundingBox);
                        }
                    }
                    catch (Exception ex)
                    {
                        SentrySdk.CaptureException(ex);
#if DEBUG
                        Application.Current!.Windows[0].Page!.DisplayAlert(CodeLang.ErrorTitle_Text, ex.Message, CodeLang.ButtonClose_Text);
#endif
                    }               
                }
            }
        }

        ///// <summary>
        ///// Read the device information
        ///// </summary>
        //private void ReadDeviceInfo()
        //{
        //    System.Text.StringBuilder sb = new System.Text.StringBuilder();

        //    sb.AppendLine($"Model: {DeviceInfo.Current.Model}");
        //    sb.AppendLine($"Manufacturer: {DeviceInfo.Current.Manufacturer}");
        //    sb.AppendLine($"Name: {DeviceInfo.Current.Name}");
        //    sb.AppendLine($"OS Version: {DeviceInfo.Current.VersionString}");
        //    sb.AppendLine($"Idiom: {DeviceInfo.Current.Idiom}");
        //    sb.AppendLine($"Platform: {DeviceInfo.Current.Platform}");

        //    bool isVirtual = DeviceInfo.Current.DeviceType switch
        //    {
        //        DeviceType.Physical => false,
        //        DeviceType.Virtual => true,
        //        _ => false
        //    };

        //    sb.AppendLine($"Virtual device? {isVirtual}");

        //    _ = DisplayAlert("Device info", sb.ToString(), "OK");
        //}
    }
}

/* From NuGet Package BarcodeScanner.Native.Maui version 1.4.0 the app hangs or exit on the splash screen
   when the app is opened on a Samsung A320 phone with Android 8.0 when using Android local devices.
   Does not hangs or exit on the splash screen when using the released published .apk file.

   Order when leaving this page:
   Android
   1. Method: OnBackButtonPressed
   2. Method: OnDisappearing
   3. Method: ContentPage_Unloaded

   iOs - iPhone 7 with original back button
   method 'protected override bool OnBackButtonPressed()' is not called !!!
   1. Method: ContentPage_Unloaded
   2. Method: OnDisappearing

   iOs - iPhone 7 with the new back button
   1. Method: OnBackButtonPressed2
   2. Method: OnDisappearing
   3. Method: ContentPage_Unloaded */