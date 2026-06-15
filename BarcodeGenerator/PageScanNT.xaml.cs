using BarcodeScanning;

namespace BarcodeGenerator
{
    public sealed partial class PageScanNT : ContentPage
    {
        // Local variables
        private readonly BarcodeDrawable _drawable = new();
        private readonly List<string> qualities = [];
        private int nQualityCameraBack;
        private int nQualityCameraFront;

        private static double nOffsetX;
        private static double nOffsetY;
        private static double nScaleWidth;
        private static double nScaleHeight;
        private static bool bScanningFromImage;

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
                DisplayAlertAsync("InitializeComponent: PageScanNT", ex.Message, "OK");
#endif
                return;
            }

            // Get the saved quality settings
            nQualityCameraBack = Preferences.Default.Get("SettingQualityCameraBack", 2);
            nQualityCameraFront = Preferences.Default.Get("SettingQualityCameraFront", 1);

#if DEBUG
            // Read the device information
            //Task task = ClassDeviceInfo.ReadDeviceInfoAsync();
#endif
            // Settings for iOS and Android
            // Set the quality for the camera pickers
            // No support for the highest quality: iPad 8, iOS version 17.1.1 (back and front camera)
            // No support for the high and the highest quality: iPhone 7, iOS version 15.8 (front camera)
#if IOS
            // iOS front and back camera quality settings
            qualities.Add(CodeLang.CameraQualityLow_Text);
            qualities.Add(CodeLang.CameraQualityMedium_Text);
            pckCameraQualityFront.ItemsSource = qualities;

            // iOS back camera quality settings
            qualities.Add(CodeLang.CameraQualityHigh_Text);
            pckCameraQualityBack.ItemsSource = qualities;

            // Set the scale for the activity indicator for iOS (otherwise it is very small on an iPad)
            activityIndicator.Scale = 2;
#else
            // Front and back camera quality settings
            qualities.Add(CodeLang.CameraQualityLow_Text);
            qualities.Add(CodeLang.CameraQualityMedium_Text);
            qualities.Add(CodeLang.CameraQualityHigh_Text);
            pckCameraQualityFront.ItemsSource = qualities;

            // Back camera quality settings
            qualities.Add(CodeLang.CameraQualityHighest_Text);
            pckCameraQualityBack.ItemsSource = qualities;
#endif
            // Set the quality for the camera pickers
            pckCameraQualityBack.SelectedIndex = nQualityCameraBack;
            pckCameraQualityFront.SelectedIndex = nQualityCameraFront;

            // The height of the title bar is lower when an iPhone is in horizontal position
            if (DeviceInfo.Platform == DevicePlatform.iOS && DeviceInfo.Idiom == DeviceIdiom.Phone)
            {
                lblTitle.VerticalOptions = LayoutOptions.Start;
                lblTitle.VerticalTextAlignment = TextAlignment.Start;
            }

            // Initialize the barcode pickers
#if ANDROID
            // Set the barcodes in the picker for Android
            pckFormatCodeScanner.ItemsSource = ClassBarcodes.GetFormatCodeListScanner_NT_Android();
#elif IOS
            // Set the barcodes in the picker for iOS
            pckFormatCodeScanner.ItemsSource = ClassBarcodes.GetFormatCodeListScanner_NT_IOS();
#elif WINDOWS
            // Set the barcodes in the picker for Windows
            pckFormatCodeScanner.ItemsSource = ClassBarcodes.GetFormatCodeListScanner_NT_Windows();
#endif
            // Set the barcode list and the select the saved or default barcode format for the barcode scanner
            ClassBarcodes.SelectBarcodeScannerNameIndex(pckFormatCodeScanner);

            // Set controls for text to speech
            if (Globals.bTextToSpeechAvailable)
            {
                lblTextToSpeech.IsVisible = true;
                imgbtnTextToSpeech.IsVisible = true;
            }

            // For testing crashes - DivideByZeroException
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
            // Pseudocode / Plan:
            // 1. Get the Picker and its SelectedIndex.
            // 2. If an item is selected, obtain the selectedName (string).
            // 3. Use a switch expression that uses "when" guards (non-constant patterns)
            //    to compare the selectedName to ClassBarcodes.* string values.
            //    This avoids CS9135 which requires compile-time constant patterns.
            // 4. Map each known barcode name to the corresponding BarcodeFormats value.
            // 5. Fallback to BarcodeFormats.All for unknown or null values.
            var picker = (Picker)sender;
            int selectedIndex = picker.SelectedIndex;

            if (selectedIndex != -1)
            {
                var itemsSource = picker.ItemsSource;
                string? item = itemsSource is not null && itemsSource.Count > selectedIndex
                    ? itemsSource[selectedIndex] as string : null;

                string? selectedName = item is not null
                    ? picker.ItemsSource[selectedIndex] as string : string.Empty;

                barcodeReader.BarcodeSymbologies = selectedName switch
                {
                    // Use "var s when s == ..." so patterns do not need to be compile-time constants.
                    var s when s == ClassBarcodes.cBarcode_AZTEC => BarcodeFormats.Aztec,
                    var s when s == ClassBarcodes.cBarcode_CODABAR => BarcodeFormats.CodaBar,
                    var s when s == ClassBarcodes.cBarcode_CODE_128 => BarcodeFormats.Code128,
                    var s when s == ClassBarcodes.cBarcode_CODE_39 => BarcodeFormats.Code39,
                    var s when s == ClassBarcodes.cBarcode_CODE_93 => BarcodeFormats.Code93,
                    var s when s == ClassBarcodes.cBarcode_DATA_MATRIX => BarcodeFormats.DataMatrix,
                    var s when s == ClassBarcodes.cBarcode_DX_FILM_EDGE => BarcodeFormats.DXFilmEdge,
                    var s when s == ClassBarcodes.cBarcode_EAN_13 => BarcodeFormats.Ean13,
                    var s when s == ClassBarcodes.cBarcode_EAN_8 => BarcodeFormats.Ean8,
                    var s when s == ClassBarcodes.cBarcode_GS1_DATABAR => BarcodeFormats.GS1DataBar,
                    var s when s == ClassBarcodes.cBarcode_ITF => BarcodeFormats.Itf,
                    var s when s == ClassBarcodes.cBarcode_MAXICODE => BarcodeFormats.MaxiCode,
                    var s when s == ClassBarcodes.cBarcode_MICRO_PDF_417 => BarcodeFormats.MicroPdf417,
                    var s when s == ClassBarcodes.cBarcode_MICRO_QR_CODE => BarcodeFormats.MicroQR,
                    var s when s == ClassBarcodes.cBarcode_PDF_417 => BarcodeFormats.Pdf417,
                    var s when s == ClassBarcodes.cBarcode_QR_CODE => BarcodeFormats.QRCode,
                    var s when s == ClassBarcodes.cBarcode_UPC_A => BarcodeFormats.Upca,
                    var s when s == ClassBarcodes.cBarcode_UPC_E => BarcodeFormats.Upce,
                    _ => BarcodeFormats.All
                };
            }
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
            graphicsBox.Drawable = _drawable;

            // Set language text to speech using the Appearing event of the PageScanNT.xaml
            lblTextToSpeech.Text = Globals.GetIsoLanguageCode();
        }

        /// <summary>
        /// Called by the Disappearing event from the PageScanNT.xaml
        /// </summary>
        protected override void OnDisappearing()
        {
            Debug.WriteLine("Method: OnDisappearing");

            // Disable the camera
            base.OnDisappearing();
            barcodeReader.CameraEnabled = false;

            // To do before leaving this page
            BeforeLeavingPageScanNT();
        }

        /// <summary>
        /// To do before leaving this page
        /// </summary>
        private void BeforeLeavingPageScanNT()
        {
            // Save the quality settings
            Preferences.Default.Set("SettingQualityCameraBack", nQualityCameraBack);
            Preferences.Default.Set("SettingQualityCameraFront", nQualityCameraFront);

            // Set the capture quality to low (otherwise the app quit on an iPhone 7 - does not work allways)
            barcodeReader.CaptureQuality = CaptureQuality.Low;

            // Set the camera torch off
            barcodeReader.TorchOn = false;
            imgbtnCameraTorch.Source = "camera_torch_off_64x64p.png";

            // Cancel the text to speech when going back to the mainpage
            imgbtnTextToSpeech.Source = ClassSpeech.CancelTextToSpeech();

            // Give it some time to save the settings
            Task.Delay(200).Wait();
        }

        /// <summary>
        /// Handles the event triggered when the camera zoom slider value changes
        /// </summary>
        /// <param name="sender">The source of the event, typically the slider control.</param>
        /// <param name="e">The event data containing the old and new values of the slider.</param>
        private void OnSliderCameraZoomValueChanged(object sender, ValueChangedEventArgs e)
        {
            barcodeReader.RequestZoomFactor = (float)sldCameraZoom.Value;
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

                await DisplayAlertAsync(CodeLang.ErrorTitle_Text, CodeLang.CameraQualityError_Text, CodeLang.ButtonClose_Text);
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

            Debug.WriteLine("nSelectedIndex: " + nSelectedIndex);  // For testing

            // Set the title for the quality picker
            SetTitleCameraQualityPicker();

            // Reset the slider camera zoom
#if IOS
            sldCameraZoom.Value = 1;
#endif
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

            // Set the title for the quality picker
            SetTitleCameraQualityPicker();
        }

        /// <summary>
        /// Set the title for the quality picker
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

                // Clear the barcode results and invalidate the graphics to remove any existing bounding boxes
                _drawable.barcodeResults = null;
                graphicsBox.Invalidate();
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
        private async void OnShareClicked(object sender, EventArgs e)
        {
            // Share the payload types - this will parse the text and offer relevant share/open options for recognized payload types like URLs, Wi‑Fi config, contact (vCard), calendar event (iCal), etc.
            await ClassPayloadTypes.SharePayloadTypes(lblBarcodeResult.Text);            
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
                imgbtnTextToSpeech.Source = ClassSpeech.CancelTextToSpeech();
                return;
            }

            // Convert the text to speech
            _ = ClassSpeech.ConvertTextToSpeechAsync(imgbtnTextToSpeech, lblBarcodeResult.Text);
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
        /// Handles the click event to activate camera-based barcode scanning by hiding the image scan option and
        /// enabling the camera reader.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private async void OnScanFromCamera_Clicked(object sender, EventArgs e)
        {
            // Clear the barcode results and invalidate the graphics to remove any existing bounding boxes
            _drawable.barcodeResults = null;
            graphicsBox.Invalidate();
            graphicsBox.IsVisible = false;

            // Settings before scanning from the camera
            imgScanFromImage.Source = null;
            imgScanFromImage.IsVisible = false;
            await Task.Delay(200);

            bScanningFromImage = false;
            barcodeReader.CameraEnabled = true;

            sldCameraZoom.IsEnabled = true;
            sldCameraZoom.Value = 1;

            pckCameraQualityBack.IsVisible = true;
            pckCameraQualityFront.IsVisible = true;
            imgbtnCameraQuality.IsVisible = true;                                                                                                
            imgbtnCameraFacing.IsVisible = true;
            imgbtnCameraDetecting.IsVisible = true;
            imgbtnCameraVibrate.IsVisible = true;
            imgbtnCameraTorch.IsVisible = true;

            barcodeReader.PauseScanning = false;
            imgbtnCameraDetecting.Source = "camera_detect_off_128x128p.png";

            lblBarcodeResult.Text = string.Empty;
            btnShare.Text = CodeLang.ButtonShare_Text;
        }

        /// <summary>
        /// CameraView OnDetected event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCameraDetectionFinished(object sender, OnDetectionFinishedEventArg e)
        {
            if (e.BarcodeResults.Count == 0)
            {
                return;
            }

            bScanningFromImage = false;
            graphicsBox.IsVisible = true;

            imgbtnCopyToClipboard.IsEnabled = false;
            btnShare.IsEnabled = false;
            imgbtnTextToSpeech.IsEnabled = false;

            lblBarcodeResult.Text = string.Empty;
            string cBarcodeFormat = string.Empty;
            string cDisplayValue = string.Empty;
            List<string> listBarcodes = [];

            try
            {
                _drawable.barcodeResults = e.BarcodeResults;
                graphicsBox.Invalidate();

                foreach (var barcode in e.BarcodeResults)
                {
                    cBarcodeFormat = barcode.BarcodeFormat.ToString();
                    // Use RawValue for consistent raw data across platforms
                    // The DisplayValue property may be parsed differently by the underlying platform barcode APIs:
                    // - Android(Google ML Kit): Automatically parses Wi-Fi QR codes and returns formatted text
                    // - iOS(Apple Vision / AVFoundation): Returns the raw QR code string
                    cDisplayValue = barcode.RawValue ?? barcode.DisplayValue;

                    // Decompress the QR code result if compressed
                    cDisplayValue = ClassCompression.DecompressFromBase64(cDisplayValue);

                    // Add the barcode format and display value to the list 'listBarcodes'
                    listBarcodes.Add($"{cBarcodeFormat}:\n{cDisplayValue}");
                }

                // Process the list of BarcodeResult objects, remove duplicates, sort them, and set the results in the label 'lblBarcodeResult.Text'
                lblBarcodeResult.Text = ClassBarcodes.ProcessScannedBarcodes(listBarcodes, btnShare);

                imgbtnCopyToClipboard.IsEnabled = true;
                btnShare.IsEnabled = true;
                imgbtnTextToSpeech.IsEnabled = true;
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
#if DEBUG
                _ = DisplayAlertAsync("OnCameraDetectionFinished", ex.Message, CodeLang.ButtonClose_Text);
#endif
            }
        }

        /// <summary>
        /// Handles the click event to initiate scanning from an image asynchronously.
        /// </summary>
        /// <param name="sender">The source of the event, typically the button that was clicked.</param>
        /// <param name="e">An EventArgs object that contains the event data.</param>
        /// <remarks>https://github.com/afriscic/BarcodeScanning.Native.Maui/issues/107
        /// Why such a difference in values between Android and iOS for the PreviewBoundingBox?
        /// Android (and Windows?) are OK but iOS is a disaster because the bounding box is not correctly placed on the image when scanning
        /// from an image (the file path flow).
        /// - Android returns absolute pixel coordinates for bounding boxes (so values look like 78, 420, …).
        /// - iOS (Vision/AVFoundation) often returns normalized coordinates(0..1) for image/object bounds, 
        ///   those fractions are relative to the image size, not pixels. That's why you see values like 0.14 and 0.72.
        /// - Also be aware of coordinate - origin differences (some iOS APIs return normalized coords with a flipped Y axis).
        ///   PreviewBoundingBox is typically only populated for camera frames,
        ///   when scanning from a static image it is often empty(0s) on both platforms.
        /// Keep the mapping call only when scanning from images (the file path flow).
        /// For camera frames you can keep the previous PreviewBoundingBox logic.
        /// If you still see vertical flip or misplacement on iOS, tweak the flip logic in MapImageBoundingBoxToControl
        /// (the code already tries a Y-flip when mapped rect seems outside bounds).  
        /// </remarks>
        private async void OnScanFromImage_Clicked(object sender, EventArgs e)
        {
            // Start the activity indicator
            activityIndicator.IsVisible = true;
            activityIndicator.IsRunning = true;
            await Task.Delay(200);

            // Settings before scanning from an image
            barcodeReader.CameraEnabled = false;

            sldCameraZoom.IsEnabled = false;
            pckCameraQualityBack.IsVisible = false;
            pckCameraQualityFront.IsVisible = false;
            imgbtnCameraQuality.IsVisible = false;
            imgbtnCameraFacing.IsVisible = false;
            imgbtnCameraDetecting.IsVisible = false;
            imgbtnCameraVibrate.IsVisible = false;
            imgbtnCameraTorch.IsVisible = false;

            imgScanFromImage.IsVisible = true;
            bScanningFromImage = true;

            lblBarcodeResult.Text = string.Empty;
            btnShare.Text = CodeLang.ButtonShare_Text;
            imgbtnCopyToClipboard.IsEnabled = false;
            btnShare.IsEnabled = false;
            imgbtnTextToSpeech.IsEnabled = false;

            // Clear the barcode results and invalidate the graphics to remove any existing bounding boxes
            _drawable.barcodeResults = null;
            graphicsBox.Invalidate();
            graphicsBox.IsVisible = false;
            imgScanFromImage.Source = null;
            imgScanFromImage.IsVisible = false;
            await Task.Delay(200);

            // Open the file picker to select an image and get the selected file as a FileResult object
            FileResult? file = await ClassFileOperations.PickOneImage();
            Debug.WriteLine($"Selected file: {file?.FullPath} - ContentType: {file?.ContentType}");

            // Add null check
            if (file == null)
            {
                // Stop the activity indicator
                activityIndicator.IsRunning = false;
                activityIndicator.IsVisible = false;

                return;
            }

            // Initialize variables for processing the image and barcode results
            string cBarcodeFormat = string.Empty;
            string cDisplayValue = string.Empty;
            List<string> listBarcodes = [];

            // Process the selected file
            // Clear the barcode results and invalidate the graphics to remove any existing bounding boxes
            _drawable.barcodeResults = null;
            graphicsBox.Invalidate();
            graphicsBox.IsVisible = false;
            imgScanFromImage.Source = null;
            imgScanFromImage.IsVisible = false;
            await Task.Delay(200);

            // Open the selected file as a stream and read its bytes
            using Stream stream = await file.OpenReadAsync();
            {
                byte[] bytes = new byte[stream.Length];
                stream.ReadExactly(bytes);
                stream.Seek(0, SeekOrigin.Begin);

                // Get the dimensions and aspect ratio of the image on disk using the utility method
                (double fileWidth, double fileHeight) = ClassImageUtilities.GetImageDimensions(file.FullPath);
                double nAspectRatio = ClassImageUtilities.GetAspectRatioImage(fileWidth, fileHeight);
                Debug.WriteLine($"File dimensions: {fileWidth}x{fileHeight} - Aspect ratio: {nAspectRatio}");

                // Load the selected image in the image control
                try
                {
                    imgScanFromImage.Source = ImageSource.FromStream(() => stream);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error loading image: {ex.Message}");
                    imgScanFromImage.Source = null;
                }
                
                imgScanFromImage.IsVisible = true;
                await Task.Delay(200);  // Wait briefly for the image to load and layout to update

                if (imgScanFromImage.Source == null)
                {
                    stream.Dispose();
                    return;
                }

                // Get the rendered size of the image in the image control after it has been laid
                double nImageWidthInControl;
                double nImageHeightInControl;

                (nOffsetX, nOffsetY, nImageWidthInControl, nImageHeightInControl) = await GetRenderedImageRectAsync(imgScanFromImage);
                Debug.WriteLine($"Rendered image rect - X: {nOffsetX}, Y: {nOffsetY}, Width: {nImageWidthInControl}, Height: {nImageHeightInControl}");

                // Calculate the scale factors for width and height
                nScaleWidth = fileWidth / nImageWidthInControl;
                nScaleHeight = fileHeight / nImageHeightInControl;
                Debug.WriteLine($"Scale factors - Width: {nScaleWidth}, Height: {nScaleHeight}");

                // Scan the image for barcodes using the native library and get the results as a list of BarcodeResult objects
                IReadOnlySet<BarcodeResult> list = await Methods.ScanFromImageAsync(bytes);
                List<BarcodeResult> obj = [.. list];

                if (obj.Count > 0)
                {
                    Debug.WriteLine($"obj.Count: {obj.Count}");

                    // The location and size of the rectangle is wrong when scanning from an image,
                    // the ImageBoundingBox is used instead of the PreviewBoundingBox,
                    // this is a known issue in the native libraries
                    _drawable.barcodeResults = list;
                    graphicsBox.Invalidate();
                    graphicsBox.IsVisible = true;

                    foreach (BarcodeResult code in obj)
                    {
                        cBarcodeFormat = code.BarcodeFormat.ToString();
                        cDisplayValue = code.RawValue ?? cDisplayValue;

                        Debug.WriteLine($"cBarcodeFormat: {code.BarcodeFormat} - cDisplayValue: {cDisplayValue}");

                        // Decompress the QR code result if compressed
                        cDisplayValue = ClassCompression.DecompressFromBase64(cDisplayValue);

                        // Add the barcode format and display value to the list 'listBarcodes'
                        // If all symbologies are selected in the picker
                        if (barcodeReader.BarcodeSymbologies == BarcodeFormats.All)
                        {
                            listBarcodes.Add($"{cBarcodeFormat}:\n{cDisplayValue}");
                        }
                        // If the barcode symbology is the same as the selected one in the picker
                        else if (barcodeReader.BarcodeSymbologies == code.BarcodeFormat)
                        {
                            Debug.WriteLine($"Selected symbology: {barcodeReader.BarcodeSymbologies} == {code.BarcodeFormat}");
                            listBarcodes.Add($"{cBarcodeFormat}:\n{cDisplayValue}");
                        }
                    }
                }

                //// Replace or insert this right after obtaining 'list' and after computing nOffsetX,nOffsetY,nImageWidthInControl,nImageHeightInControl,nScaleWidth,nScaleHeight
                //List<RectF> mapped = new();

                //if (list.Count > 0)
                //{
                //    double renderedX = nOffsetX;
                //    double renderedY = nOffsetY;
                //    double renderedW = nImageWidthInControl;
                //    double renderedH = nImageHeightInControl;

                //    foreach (var code in list)
                //    {
                //        // Prefer PreviewBoundingBox if present (camera case)
                //        var box = code.PreviewBoundingBox.Width > 0 && code.PreviewBoundingBox.Height > 0
                //            ? code.PreviewBoundingBox
                //            : code.ImageBoundingBox;

                //        // If still empty, skip
                //        if (box.Width <= 0 || box.Height <= 0) continue;

                //        mapped.Add(MapImageBoundingBoxToControl(box, renderedX, renderedY, renderedW, renderedH, nScaleWidth, nScaleHeight));
                //    }
                //}

                //// Keep the raw results for debug if you want
                //_drawable.barcodeResults = list;

                //// Set mapped rects on the drawable (new field shown below)
                //_drawable.mappedRectangles = mapped;
                //graphicsBox.Invalidate();
                //graphicsBox.IsVisible = true;
            }

            // Process the list of BarcodeResult objects, remove duplicates, sort them, and set the results in the label 'lblBarcodeResult.Text'
            lblBarcodeResult.Text = file.FileName + "\n\n" + ClassBarcodes.ProcessScannedBarcodes(listBarcodes, btnShare);

            // Settings after scanning from an image
            imgbtnCopyToClipboard.IsEnabled = true;
            btnShare.IsEnabled = true;
            imgbtnTextToSpeech.IsEnabled = true;

            // Stop the activity indicator
            activityIndicator.IsRunning = false;
            activityIndicator.IsVisible = false;

            // Delete the file from the cache and dispose the stream
            ClassFileOperations.DeleteFileInCache(file.FullPath);
            stream.Dispose();
        }

        /// <summary>
        /// Call this async method after the Image control has been created (for example from OnAppearing
        /// or in a SizeChanged handler). It waits briefly for layout, retrieves the native image size
        /// and computes the rectangle within the control where the image content is actually drawn
        /// (taking Aspect, AspectFit/AspectFill/Fill into account).
        /// </summary>
        /// <param name="image"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        private static async Task<(double X, double Y, double Width, double Height)> GetRenderedImageRectAsync(Image image, CancellationToken ct = default)
        {
            if (image == null)
            {
                return (0, 0, 0, 0);
            }

            // Wait until the control has a measured size and a handler is attached (up to a short timeout).
            Stopwatch sw = Stopwatch.StartNew();
            while ((image.Width <= 0 || image.Height <= 0 || image.Handler == null) && sw.ElapsedMilliseconds < 2000)
            {
                await Task.Delay(25, ct).ConfigureAwait(false);
            }

            double containerW = image.Width;
            double containerH = image.Height;

            // Fallback: if control size is still zero, return empty rect
            if (containerW <= 0 || containerH <= 0)
            {
                return (0, 0, 0, 0);
            }

            // Try to get the intrinsic/native pixel size of the image source
            double nativeW = 0, nativeH = 0;

#if ANDROID
            try
            {
                var nativeView = image.Handler?.PlatformView as Android.Widget.ImageView;
                var drawable = nativeView?.Drawable;
                if (drawable != null)
                {
                    nativeW = drawable.IntrinsicWidth;
                    nativeH = drawable.IntrinsicHeight;
                }
            }
            catch { }
#elif IOS || MACCATALYST
    try
    {
        var nativeView = image.Handler?.PlatformView as UIKit.UIImageView;
        var uiImage = nativeView?.Image;
        if (uiImage != null)
        {
            nativeW = uiImage.Size.Width;
            nativeH = uiImage.Size.Height;
        }
    }
    catch { }
#elif WINDOWS
    try
    {
        var nativeView = image.Handler?.PlatformView as Microsoft.UI.Xaml.Controls.Image;
        if (nativeView?.Source is Microsoft.UI.Xaml.Media.Imaging.BitmapSource bmp)
        {
            nativeW = bmp.PixelWidth;
            nativeH = bmp.PixelHeight;
        }
    }
    catch { }
#endif
            // If we couldn't get intrinsic size, fall back to using control size (image fills the control)
            if (nativeW <= 0 || nativeH <= 0)
            {
                return (0, 0, containerW, containerH);
            }

            // Compute scaling based on Aspect
            double displayW = containerW;
            double displayH = containerH;
            double offsetX = 0;
            double offsetY = 0;

            Aspect aspect = image.Aspect;

            if (aspect == Aspect.AspectFill)
            {
                var scale = Math.Max(containerW / nativeW, containerH / nativeH);
                displayW = nativeW * scale;
                displayH = nativeH * scale;
                offsetX = (containerW - displayW) / 2.0;
                offsetY = (containerH - displayH) / 2.0;
            }
            else if (aspect == Aspect.AspectFit)
            {
                var scale = Math.Min(containerW / nativeW, containerH / nativeH);
                displayW = nativeW * scale;
                displayH = nativeH * scale;
                offsetX = (containerW - displayW) / 2.0;
                offsetY = (containerH - displayH) / 2.0;
            }
            else // Aspect.Fill
            {
                // Fill stretches to container; content scaled independently in X/Y
                displayW = containerW;
                displayH = containerH;
                offsetX = 0;
                offsetY = 0;
            }

            return (offsetX, offsetY, displayW, displayH);
        }

        ///// <summary>
        ///// Maps the bounding box of the detected barcode from the image coordinates to the control coordinates, taking into account the rendered size and position of the image in the control, as well as the scale factors between the original image and the rendered image.
        ///// </summary>
        ///// <param name="imageBox"></param>
        ///// <param name="renderedX"></param>
        ///// <param name="renderedY"></param>
        ///// <param name="renderedW"></param>
        ///// <param name="renderedH"></param>
        ///// <param name="nScaleWidth"></param>
        ///// <param name="nScaleHeight"></param>
        ///// <returns></returns>
        //private static RectF MapImageBoundingBoxToControl(RectF imageBox, double renderedX, double renderedY, double renderedW, double renderedH, double nScaleWidth, double nScaleHeight)
        //{
        //    // device density used elsewhere in Draw
        //    double nDensity = DeviceDisplay.Current.MainDisplayInfo.Density;

        //    // Decide if values are normalized (0..1) or absolute pixels
        //    bool normalized = imageBox.X <= 1.0 && imageBox.Y <= 1.0 && imageBox.Width <= 1.0 && imageBox.Height <= 1.0;

        //    float x, y, w, h;

        //    // iOS
        //    if (normalized)
        //    {
        //        // normalized -> control coordinates
        //        x = (float)(renderedX + imageBox.X * renderedW);
        //        y = (float)(renderedY + imageBox.Y * renderedH);
        //        w = (float)(imageBox.Width * renderedW);
        //        h = (float)(imageBox.Height * renderedH);

        //        // If box looks outside, try flipping Y (iOS origin differences)
        //        if (y + h < renderedY || y > renderedY + renderedH)
        //        {
        //            y = (float)(renderedY + (1.0 - imageBox.Y - imageBox.Height) * renderedH);
        //        }

        //        // Convert to device pixels the same way existing Draw expects (it multiplies by density)
        //        return new RectF(x, y, w * (float)nDensity, h * (float)nDensity);
        //    }
        //    // Android and Windows
        //    else
        //    {
        //        // values appear to be pixel coords relative to full image -> convert to control coords using scales
        //        x = (float)(renderedX + imageBox.X / nScaleWidth);
        //        y = (float)(renderedY + imageBox.Y / nScaleHeight);
        //        w = (float)(imageBox.Width / nScaleWidth);
        //        h = (float)(imageBox.Height / nScaleHeight);

        //        // Convert to device pixels the same way existing Draw expects (it multiplies by density)
        //        return new RectF(x * (float)nDensity, y * (float)nDensity, w * (float)nDensity, h * (float)nDensity);
        //    }
        //}

        /// <summary>
        /// Class for drawing the barcode bounding box.
        /// Inside the BarcodeDrawable class: add a field and prefer mappedRectangles when drawing
        /// </summary>
        public sealed class BarcodeDrawable : IDrawable
        {
            public IReadOnlySet<BarcodeResult>? barcodeResults;
            public List<RectF>? mappedRectangles; // new

            public void Draw(ICanvas canvas, RectF dirtyRect)
            {
                if ((mappedRectangles is null || mappedRectangles.Count == 0) && (barcodeResults is null || barcodeResults.Count == 0))
                {
                    return;
                }
#if IOS
                if (bScanningFromImage)
                {
                    //canvas.Rotate(180, 300, 300); // Workaround for a possible SkiaSharp bug that causes the canvas to be rotated on some platforms after invalidation, resulting in incorrectly drawn rectangles

                    // Skip drawing rectangles when scanning from an image on iOS due to the bounding box mapping issues described in the comments. The rectangles are not drawn correctly on iOS when scanning from images, so we avoid drawing them to prevent confusion.
                    return;
                }
                canvas.StrokeSize = 10;
#else
                canvas.StrokeSize = 15;
#endif
                canvas.StrokeColor = Colors.Green;
                float scale = 1 / canvas.DisplayScale;
                canvas.Scale(scale, scale);

                try
                {
                    if (mappedRectangles is not null && mappedRectangles.Count > 0)
                    {
                        // mappedRectangles already includes density multiplication (matches prior Draw logic)
                        foreach (var r in mappedRectangles)
                        {
                            canvas.DrawRectangle(r);
                        }
                    }
                    else
                    {
                        // Fallback to original behavior (camera preview / pixel coords)
                        double nDensity = DeviceDisplay.Current.MainDisplayInfo.Density;

                        foreach (var barcode in barcodeResults!)
                        {
                            if (barcode.PreviewBoundingBox.Width > 0 && barcode.PreviewBoundingBox.Height > 0)
                            {
                                canvas.DrawRectangle(barcode.PreviewBoundingBox);
                            }
                            else
                            {
                                float nX = ((float)nOffsetX + (barcode.ImageBoundingBox.X / (float)nScaleWidth)) * (float)nDensity;
                                float nY = ((float)nOffsetY + (barcode.ImageBoundingBox.Y / (float)nScaleHeight)) * (float)nDensity;
                                float nWidth = barcode.ImageBoundingBox.Width / (float)nScaleWidth * (float)nDensity;
                                float nHeight = barcode.ImageBoundingBox.Height / (float)nScaleHeight * (float)nDensity;

                                canvas.DrawRectangle(nX, nY, nWidth, nHeight);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    SentrySdk.CaptureException(ex);
#if DEBUG
                    Application.Current!.Windows[0].Page!.DisplayAlertAsync("Draw", ex.Message, CodeLang.ButtonClose_Text);
#endif
                }
            }
        }

        ///// <summary>
        ///// Class for drawing the barcode bounding box
        ///// </summary>
        //        private sealed class BarcodeDrawable : IDrawable
        //        {
        //            public IReadOnlySet<BarcodeResult>? barcodeResults;

        //            public void Draw(ICanvas canvas, RectF dirtyRect)
        //            {
        //#if IOS
        //                /*
        //                !!!BUG!!! in iOS? The values of the ImageBoundingBox in iOS are all less than 1 and do not correspond to the actual location and size
        //                of the barcode in the image, even when adjusting with the scale factors and offsets.
        //                It seems like the ImageBoundingBox values are normalized to the range [0, 1] relative to the original image dimensions,
        //                but this does not match the expected pixel coordinates for drawing the rectangle.
        //                This issue does not occur in Android where the ImageBoundingBox values are in pixel coordinates as expected.
        //                Sample scanning from image:
        //                - Android: PreviewBoundingBox: {X=0 Y=0 Width=0 Height=0} - ImageBoundingBox: {X=78 Y=79 Width=420 Height=421}
        //                - iOS: PreviewBoundingBox:     {X=0 Y=0 Width=0 Height=0} - ImageBoundingBox: {X=0.14041096 Y=0.14041096 Width=0.7191781 Height=0.7191781}
        //                - Windows: PreviewBoundingBox: ? - ImageBoundingBox: ?
        //                */

        //                //MapImageBoundingBoxToControl(barcode.ImageBoundingBox, fileWidth, fileHeight, renderedX, renderedY, renderedW, renderedH);

        //                return;
        //#else
        //                if (barcodeResults is not null && barcodeResults.Count > 0)
        //                {
        //                    canvas.StrokeSize = 15;
        //                    canvas.StrokeColor = Colors.Green;
        //                    float scale = 1 / canvas.DisplayScale;
        //                    canvas.Scale(scale, scale);

        //                    // Get the density of the device display if the barcode is scanned from an image
        //                    double nDensity = DeviceDisplay.Current.MainDisplayInfo.Density;

        //                    try
        //                    {
        //                        foreach (var barcode in barcodeResults)
        //                        {
        //                            Debug.WriteLine($"PreviewBoundingBox: {barcode.PreviewBoundingBox}");
        //                            Debug.WriteLine($"ImageBoundingBox: {barcode.ImageBoundingBox}");

        //                            // If barcode is scanned from the camera use the PreviewBoundingBox
        //                            if (barcode.PreviewBoundingBox.Width > 0 && barcode.PreviewBoundingBox.Height > 0)
        //                            {
        //                                canvas.DrawRectangle(barcode.PreviewBoundingBox);
        //                            }
        //                            // If barcode is scanned from an image use the ImageBoundingBox.
        //                            // The location and size of the rectangle has to be adjusted based on the scale factors and offsets of the rendered image in the control.
        //                            // The ImageBoundingBox is used instead of the PreviewBoundingBox, because the PreviewBoundingBox is not supported when scanning from an image,
        //                            // this is a known issue in the native libraries.
        //                            else
        //                            {
        //                                // Solution: works when the image control is filled with the image, but not when the image is resized to fit the control (the location and size of the rectangle are wrong)
        //                                float nX = ((float)nOffsetX + (barcode.ImageBoundingBox.X / (float)nScaleWidth)) * (float)nDensity;
        //                                float nY = ((float)nOffsetY + (barcode.ImageBoundingBox.Y / (float)nScaleHeight)) * (float)nDensity;
        //                                float nWidth = barcode.ImageBoundingBox.Width / (float)nScaleWidth * (float)nDensity;
        //                                float nHeight = barcode.ImageBoundingBox.Height / (float)nScaleHeight * (float)nDensity;
        //                                Debug.WriteLine($"nOffsetX: {nOffsetX}, nOffsetY: {nOffsetY}, nX: {nX}, nY: {nY}, nWidth: {nWidth}, nHeight: {nHeight}");

        //                                canvas.DrawRectangle(nX, nY, nWidth, nHeight);
        //                            }
        //                        }
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        SentrySdk.CaptureException(ex);
        //#if DEBUG
        //                        Application.Current!.Windows[0].Page!.DisplayAlertAsync("Draw", ex.Message, CodeLang.ButtonClose_Text);
        //#endif
        //                    }
        //                }
        //#endif
        //            }
        //        }
    }
}
