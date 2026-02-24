using ZXing.Net.Maui;

namespace BarcodeGenerator
{
    public partial class PageScanZX : ContentPage
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
            var picker = (Picker)sender;
            int selectedIndex = picker.SelectedIndex;

            if (selectedIndex != -1)
            {
                lblBarcodeResult.Text = "";
                btnShare.Text = CodeLang.ButtonShare_Text;

                imgbtnCopyToClipboard.IsEnabled = false;
                btnShare.IsEnabled = false;
                imgbtnTextToSpeech.IsEnabled = false;

                var itemsSource = picker.ItemsSource;
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
        /// Barcode detected event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnBarcodesDetected(object? sender, BarcodeDetectionEventArgs e)
        {
            if (e.Results == null || e.Results.Length == 0) return;

            // Build the display list on the background thread
            var list = new List<string>();
            foreach (var barcode in e.Results)
            {
                var fmt = barcode.Format.ToString();
                var val = barcode.Value;
                if (!string.IsNullOrEmpty(fmt) && !string.IsNullOrEmpty(val))
                    list.Add($"{fmt}:\n{val}");
            }

            // Remove duplicates and sort the list
            list = [.. list.Distinct().OrderBy(x => x)];

            // Marshal UI updates to the main thread
            Dispatcher.Dispatch(() =>
            {
                imgbtnCopyToClipboard.IsEnabled = false;
                btnShare.IsEnabled = false;
                imgbtnTextToSpeech.IsEnabled = false;

                lblBarcodeResult.Text = "";

                // Set the barcode results in the label 'lblBarcodeResult.Text'
                if (list.Count == 1)
                {
                    var parts = list[0].Split([":\n"], StringSplitOptions.None);
                    btnShare.Text = $"{CodeLang.ButtonShare_Text} {parts[0]}";
                    lblBarcodeResult.Text = parts.Length > 1 ? parts[1] : "";
                }
                else if (list.Count > 1)
                {
                    btnShare.Text = CodeLang.ButtonShare_Text;
                    var sb = new System.Text.StringBuilder();
                    foreach (var item in list) sb.AppendLine(item).AppendLine();
                    lblBarcodeResult.Text = sb.ToString();
                }

                imgbtnCopyToClipboard.IsEnabled = true;
                btnShare.IsEnabled = true;
                imgbtnTextToSpeech.IsEnabled = true;
            });
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
        /// Set language text to speech using the Appearing event of the PageScanZX.xaml
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPageAppearing(object sender, EventArgs e)
        {
            lblTextToSpeech.Text = Globals.GetIsoLanguageCode();
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
    }
}
