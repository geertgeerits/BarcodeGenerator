using System.Collections;
using ZXing.Net.Maui;

namespace BarcodeGenerator
{
    public sealed partial class PageScanZX : ContentPage
    {
        public PageScanZX()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
#if DEBUG
                DisplayAlertAsync("InitializeComponent: PageScanZX", ex.Message, "OK");
#endif
                return;
            }

            // Set the flow direction of the text elements
            Globals.SetFlowDirection(this);

            // Check if the device supports barcode scanning with ZXing.Net.Maui
            if (!ZXing.Net.Maui.BarcodeScanning.IsSupported)
            {
                _ = DisplayAlertAsync(CodeLang.ErrorTitle_Text, CodeLang.CameraNotFound_Text, CodeLang.ButtonClose_Text);
                return;
            }
#if IOS
            // Check and request camera permission for iOS.
            Task<PermissionStatus> task = PageScanZX.CheckAndRequestCameraPermissionAsync();
#endif
            // The height of the title bar is lower when an iPhone is in horizontal position
            if (DeviceInfo.Platform == DevicePlatform.iOS && DeviceInfo.Idiom == DeviceIdiom.Phone)
            {
                lblTitle.VerticalOptions = LayoutOptions.Start;
                lblTitle.VerticalTextAlignment = TextAlignment.Start;
                imgbtnTorch.VerticalOptions = LayoutOptions.Start;
            }

            // Initialize the barcode pickers
            pckFormatCodeScanner.ItemsSource = ClassBarcodes.GetFormatCodeListScanner_ZX();

            // Set the barcode list and the select the saved or default barcode format for the barcode scanner
            ClassBarcodes.SelectBarcodeScannerNameIndex(pckFormatCodeScanner);
            Preferences.Default.Set("SettingBarcodeScannerName", ClassBarcodes.cBarcodeScannerName);

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
        /// ZXing CameraBarcodeReaderView options
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPickerFormatCodeChanged(object sender, EventArgs e)
        {
            Picker picker = (Picker)sender;
            int selectedIndex = picker.SelectedIndex;

            if (selectedIndex != -1)
            {
                lblBarcodeResult.Text = string.Empty;
                btnShare.Text = CodeLang.ButtonShare_Text;

                imgbtnCopyToClipboard.IsEnabled = false;
                btnShare.IsEnabled = false;
                imgbtnTextToSpeech.IsEnabled = false;

                IList? itemsSource = picker.ItemsSource;
                string? selectedName = itemsSource is not null && itemsSource.Count > selectedIndex
                    ? itemsSource[selectedIndex] as string
                    : string.Empty;

                // Use equality comparisons instead of a switch expression because ClassBarcodes members are not compile-time constants.
                if (selectedName == ClassBarcodes.cBarcode_AZTEC)
                {
                    barcodeReader.Options = new BarcodeReaderOptions
                    {
                        AutoRotate = true,
                        Formats = BarcodeFormat.Aztec,
                        Multiple = true,
                        TryHarder = true
                    };
                }

                else if (selectedName == ClassBarcodes.cBarcode_CODABAR)
                {
                    barcodeReader.Options = new BarcodeReaderOptions
                    {
                        AutoRotate = true,
                        Formats = BarcodeFormat.Codabar,
                        Multiple = true,
                        TryHarder = true
                    };
                }

                else if (selectedName == ClassBarcodes.cBarcode_CODE_128)
                {
                    barcodeReader.Options = new BarcodeReaderOptions
                    {
                        AutoRotate = true,
                        Formats = BarcodeFormat.Code128,
                        Multiple = true,
                        TryHarder = true
                    };
                }

                else if (selectedName == ClassBarcodes.cBarcode_CODE_39)
                {
                    barcodeReader.Options = new BarcodeReaderOptions
                    {
                        AutoRotate = true,
                        Formats = BarcodeFormat.Code39,
                        Multiple = true,
                        TryHarder = true
                    };
                }

                else if (selectedName == ClassBarcodes.cBarcode_CODE_93)
                {
                    barcodeReader.Options = new BarcodeReaderOptions
                    {
                        AutoRotate = true,
                        Formats = BarcodeFormat.Code93,
                        Multiple = true,
                        TryHarder = true
                    };
                }

                else if (selectedName == ClassBarcodes.cBarcode_DATA_MATRIX)
                {
                    barcodeReader.Options = new BarcodeReaderOptions
                    {
                        AutoRotate = true,
                        Formats = BarcodeFormat.DataMatrix,
                        Multiple = true,
                        TryHarder = true
                    };
                }

                else if (selectedName == ClassBarcodes.cBarcode_EAN_13)
                {
                    barcodeReader.Options = new BarcodeReaderOptions
                    {
                        AutoRotate = true,
                        Formats = BarcodeFormat.Ean13,
                        Multiple = true,
                        TryHarder = true
                    };
                }

                else if (selectedName == ClassBarcodes.cBarcode_EAN_8)
                {
                    barcodeReader.Options = new BarcodeReaderOptions
                    {
                        AutoRotate = true,
                        Formats = BarcodeFormat.Ean8,
                        Multiple = true,
                        TryHarder = true
                    };
                }

                else if (selectedName == ClassBarcodes.cBarcode_IMB)
                {
                    barcodeReader.Options = new BarcodeReaderOptions
                    {
                        AutoRotate = true,
                        Formats = BarcodeFormat.Imb,
                        Multiple = true,
                        TryHarder = true
                    };
                }

                else if (selectedName == ClassBarcodes.cBarcode_ITF)
                {
                    barcodeReader.Options = new BarcodeReaderOptions
                    {
                        AutoRotate = true,
                        Formats = BarcodeFormat.Itf,
                        Multiple = true,
                        TryHarder = true
                    };
                }

                else if (selectedName == ClassBarcodes.cBarcode_MAXICODE)
                {
                    barcodeReader.Options = new BarcodeReaderOptions
                    {
                        AutoRotate = true,
                        Formats = BarcodeFormat.MaxiCode,
                        Multiple = true,
                        TryHarder = true
                    };
                }

                else if (selectedName == ClassBarcodes.cBarcode_MSI)
                {
                    barcodeReader.Options = new BarcodeReaderOptions
                    {
                        AutoRotate = true,
                        Formats = BarcodeFormat.Msi,
                        Multiple = true,
                        TryHarder = true
                    };
                }

                else if (selectedName == ClassBarcodes.cBarcode_PDF_417)
                {
                    barcodeReader.Options = new BarcodeReaderOptions
                    {
                        AutoRotate = true,
                        Formats = BarcodeFormat.Pdf417,
                        Multiple = true,
                        TryHarder = true
                    };
                }

                else if (selectedName == ClassBarcodes.cBarcode_PHARMACODE)
                {
                    barcodeReader.Options = new BarcodeReaderOptions
                    {
                        AutoRotate = true,
                        Formats = BarcodeFormat.PharmaCode,
                        Multiple = true,
                        TryHarder = true
                    };
                }

                else if (selectedName == ClassBarcodes.cBarcode_PLESSEY)
                {
                    barcodeReader.Options = new BarcodeReaderOptions
                    {
                        AutoRotate = true,
                        Formats = BarcodeFormat.Plessey,
                        Multiple = true,
                        TryHarder = true
                    };
                }

                else if (selectedName == ClassBarcodes.cBarcode_QR_CODE)
                {
                    barcodeReader.Options = new BarcodeReaderOptions
                    {
                        AutoRotate = true,
                        Formats = BarcodeFormat.QrCode,
                        Multiple = true,
                        TryHarder = true
                    };
                }

                else if (selectedName == ClassBarcodes.cBarcode_RSS_14)
                {
                    barcodeReader.Options = new BarcodeReaderOptions
                    {
                        AutoRotate = true,
                        Formats = BarcodeFormat.Rss14,
                        Multiple = true,
                        TryHarder = true
                    };
                }

                else if (selectedName == ClassBarcodes.cBarcode_RSS_EXPANDED)
                {
                    barcodeReader.Options = new BarcodeReaderOptions
                    {
                        AutoRotate = true,
                        Formats = BarcodeFormat.RssExpanded,
                        Multiple = true,
                        TryHarder = true
                    };
                }

                else if (selectedName == ClassBarcodes.cBarcode_UPC_A)
                {
                    barcodeReader.Options = new BarcodeReaderOptions
                    {
                        AutoRotate = true,
                        Formats = BarcodeFormat.UpcA,
                        Multiple = true,
                        TryHarder = true
                    };
                }

                else if (selectedName == ClassBarcodes.cBarcode_UPC_E)
                {
                    barcodeReader.Options = new BarcodeReaderOptions
                    {
                        AutoRotate = true,
                        Formats = BarcodeFormat.UpcE,
                        Multiple = true,
                        TryHarder = true
                    };
                }

                else if (selectedName == ClassBarcodes.cBarcode_UPC_EAN_EXTENSION)
                {
                    barcodeReader.Options = new BarcodeReaderOptions
                    {
                        AutoRotate = true,
                        Formats = BarcodeFormat.UpcEanExtension,
                        Multiple = true,
                        TryHarder = true
                    };
                }

                else
                {
                    barcodeReader.Options = new BarcodeReaderOptions
                    {
                        AutoRotate = true,
                        Formats = BarcodeFormats.All,
                        Multiple = true,
                        TryHarder = true
                    };
                }
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
        /// Set language text to speech using the Appearing event of the PageScanZX.xaml
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPageAppearing(object sender, EventArgs e)
        {
            // Set the language for text to speech
            lblTextToSpeech.Text = ClassSpeech.GetIsoLanguageSpeechCode();
        }

        /// <summary>
        /// Toggle the camera detecting state of the barcode reader and update the image button source accordingly.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCameraDetecting_Clicked(object sender, EventArgs e)
        {
            if (barcodeReader.IsDetecting)
            {
                barcodeReader.IsDetecting = false;
                imgbtnCameraDetecting.Source = "camera_detect_off_128x128p.png";
            }
            else
            {
                barcodeReader.IsDetecting = true;
                imgbtnCameraDetecting.Source = "camera_detect_on_128x128p.png";
            }
        }

        /// <summary>
        /// Handles the value changed event of the camera zoom slider, updating the barcode reader's zoom factor accordingly.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSliderCameraZoom_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            barcodeReader.ZoomFactor = (float)e.NewValue;
        }

        /// <summary>
        /// ImageButton torch clicked event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTorchClicked(object sender, EventArgs e)
        {
            barcodeReader.IsTorchOn = !barcodeReader.IsTorchOn;
        }

        /// <summary>
        /// Cancel the text to speech and turn off the torch if on, when going back to the mainpage
        /// Called by the Disappearing event from the PageScanZX.xaml
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPageDisappearing(object sender, EventArgs e)
        {
            imgbtnTextToSpeech.Source = ClassSpeech.CancelTextToSpeech();

            if (barcodeReader.IsTorchOn)
            {
                barcodeReader.IsTorchOn = false;
                Task.Delay(300).Wait();
            }

            // Unsubscribe the barcode event
            barcodeReader.BarcodesDetected -= OnBarcodesDetected;
        }

        /// <summary>
        /// Button text to speech event - Convert text to speech
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTextToSpeechClicked(object sender, EventArgs e)
        {
            // Cancel the text to speech.
            if (Globals.bTextToSpeechIsBusy)
            {
                imgbtnTextToSpeech.Source = ClassSpeech.CancelTextToSpeech();
                return;
            }

            // Convert the text to speech.
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
        /// Check and request camera permission for iOS
        /// </summary>
        /// <returns></returns>
        public static async Task<PermissionStatus> CheckAndRequestCameraPermissionAsync()
        {
            PermissionStatus status = await Permissions.CheckStatusAsync<Permissions.Camera>();

            if (status == PermissionStatus.Granted)
            {
                return status;
            }

            if (status == PermissionStatus.Unknown && DeviceInfo.Platform == DevicePlatform.iOS)
            {
                // Prompt the user to turn on in settings
                // On iOS once a permission has been denied it may not be requested again from the application
                await Application.Current!.Windows[0].Page!.DisplayAlertAsync("", CodeLang.CameraPermissionIOS_Text, CodeLang.ButtonClose_Text);
                return status;
            }

            return status;
        }

        /// <summary>
        /// Handles the click event to activate camera-based barcode scanning by hiding the image scan option and
        /// enabling the camera reader.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void OnScanFromCamera_Clicked(object sender, EventArgs e)
        {
            sldCameraZoom.IsEnabled = true;
            imgbtnTorch.IsEnabled = true;
            imgScanFromImage.IsVisible = false;
            imgScanFromImage.Source = null;
            barcodeReader.IsEnabled = true;
            barcodeReader.IsVisible = true;
            barcodeReader.IsDetecting = true;
            lblBarcodeResult.Text = string.Empty;
            lblFileName.Text = string.Empty;
            lblFileName.IsVisible = false;
        }

        /// <summary>
        /// Barcode detected event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnBarcodesDetected(object? sender, BarcodeDetectionEventArgs e)
        {
            if (e.Results == null || e.Results.Length == 0)
            {
                return;
            }

            // Marshal UI updates to the main thread
            Dispatcher.Dispatch(() =>
            {
                // Settings
                imgbtnCopyToClipboard.IsEnabled = false;
                btnShare.IsEnabled = false;
                imgbtnTextToSpeech.IsEnabled = false;

                lblBarcodeResult.Text = string.Empty;
            });

            // Build the display list on the background thread
            string fmt = string.Empty;
            string val = string.Empty;

            List<string> list = [];
            foreach (BarcodeResult? barcode in e.Results)
            {
                fmt = barcode.Format.ToString();
                val = barcode.Value;

                // Decompress the QR code result if compressed
                val = ClassCompression.DecompressFromBase64(val);

                if (!string.IsNullOrEmpty(fmt) && !string.IsNullOrEmpty(val))
                {
                    list.Add($"{fmt}:\n{val}");
                }
            }

            // Marshal UI updates to the main thread
            Dispatcher.Dispatch(() =>
            {
                // Process the list of BarcodeResult objects, remove duplicates, sort them, and set the results in the label 'lblBarcodeResult.Text'
                lblBarcodeResult.Text = ClassBarcodes.ProcessScannedBarcodes(list, btnShare);

                // Enable the buttons after processing the results
                imgbtnCopyToClipboard.IsEnabled = true;
                btnShare.IsEnabled = true;
                imgbtnTextToSpeech.IsEnabled = true;
            });
        }

        /// <summary>
        /// Handles the click event to initiate scanning from an image asynchronously.
        /// </summary>
        /// <param name="sender">The source of the event, typically the button that was clicked.</param>
        /// <param name="e">An EventArgs object that contains the event data.</param>
        private async void OnScanFromImage_Clicked(object sender, EventArgs e)
        {
            // Not possible to implement scanning from an image with ZXing.Net.Maui,
            // because the BarcodeReader class and its Decode method for processing images are not available in the ZXing.Net.Maui library.

            // Start the activity indicator
            activityIndicator.IsVisible = true;
            activityIndicator.IsRunning = true;
            await Task.Delay(200);

            // Settings before scanning from an image
            sldCameraZoom.IsEnabled = false;
            imgbtnTorch.IsEnabled = false;
            barcodeReader.IsDetecting = false;
            imgScanFromImage.IsVisible = true;
            lblFileName.Text = string.Empty;
            lblFileName.IsVisible = true;

            lblBarcodeResult.Text = CodeLang.ImageScanning_Text;
            btnShare.Text = CodeLang.ButtonShare_Text;
            imgbtnCopyToClipboard.IsEnabled = false;
            btnShare.IsEnabled = false;
            imgbtnTextToSpeech.IsEnabled = false;

            // Clear the barcode results and invalidate the graphics to remove any existing bounding boxes
            imgScanFromImage.Source = null;
            imgScanFromImage.IsVisible = false;
            await Task.Delay(200);

            // Open the file picker to select an image and get the selected file as a FileResult object
            FileResult? file = await ClassFileUtilities.PickImage();
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
            List<string> listBarcodes = [];

            // Process the selected file
            // Clear the barcode results and invalidate the graphics to remove any existing bounding boxes
            imgScanFromImage.Source = null;
            imgScanFromImage.IsVisible = false;
            await Task.Delay(200);

            // Open the selected file as a stream and read its bytes
            using Stream stream = await file.OpenReadAsync();
            {
                byte[] bytes = new byte[stream.Length];
                stream.ReadExactly(bytes);
                stream.Seek(0, SeekOrigin.Begin);

                // Load the selected image in the image control
                try
                {
                    imgScanFromImage.Source = file.FullPath;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error loading image: {ex.Message}");
                    await Application.Current!.Windows[0].Page!.DisplayAlertAsync(CodeLang.ErrorTitle_Text, $"{file.FileName}\n\n{CodeLang.ErrorInvalidImageType_Text}", CodeLang.ButtonClose_Text);
                    imgScanFromImage.Source = null;
                }

                imgScanFromImage.IsVisible = true;
                await Task.Delay(200);  // Wait briefly for the image to load and layout to update

                if (imgScanFromImage.Source == null)
                {
                    stream.Dispose();
                    return;
                }

                // Scanning Micro QR code and Rectangular QR code are not yet supported by the Android, iOS and Windows native libraries,
                // but can be implemented with the SkiaSharp.QrCode decoder
                // Decode the QR code from the selected image file
                string cResult;

                cResult = ClassQRCodeScanning.QRCodeDecoderImage(file.FullPath);
                if (!string.IsNullOrEmpty(cResult))
                {
                    listBarcodes.Add($"{CodeLang.Barcode_QR_CODE_Text}:\n{cResult}");
                    Debug.WriteLine($"Result: {cResult}");
                }

                // Decode the Micro QR code from the selected image file
                cResult = ClassQRCodeScanning.MicroQRCodeDecoderImage(file.FullPath);
                if (!string.IsNullOrEmpty(cResult))
                {
                    listBarcodes.Add($"{CodeLang.Barcode_MICRO_QR_CODE_Text}:\n{cResult}");
                    Debug.WriteLine($"Result: {cResult}");
                }

                // Decode the Rectangular QR code from the selected image file
                cResult = ClassQRCodeScanning.RectangularQRCodeDecoderImage(file.FullPath);
                if (!string.IsNullOrEmpty(cResult))
                {
                    listBarcodes.Add($"{CodeLang.Barcode_RMQR_CODE_Text}:\n{cResult}");
                    Debug.WriteLine($"Result: {cResult}");
                }

                // Process the list of BarcodeResult objects, remove duplicates, sort them, and set the results in the label 'lblBarcodeResult.Text'
                lblBarcodeResult.Text = ClassBarcodes.ProcessScannedBarcodes(listBarcodes, btnShare);

                // Set the file name in the label 'lblFileName.Text'
                lblFileName.Text = file.FileName;

                // Settings after scanning from an image
                imgbtnCopyToClipboard.IsEnabled = true;
                btnShare.IsEnabled = true;
                imgbtnTextToSpeech.IsEnabled = true;

                // Stop the activity indicator
                activityIndicator.IsRunning = false;
                activityIndicator.IsVisible = false;

                // Delete the file from the cache and dispose the stream
                ClassFileUtilities.DeleteFileInCache(file.FullPath);
                stream.Dispose();
            }
        }
    }
}
