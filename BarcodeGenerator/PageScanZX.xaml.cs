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
                DisplayAlertAsync("InitializeComponent: PageScanNT", ex.Message, "OK");
#endif
                return;
            }

            // Check if the device supports barcode scanning with ZXing.Net.Maui
            if (!ZXing.Net.Maui.BarcodeScanning.IsSupported)
            {
                _ = DisplayAlertAsync(CodeLang.ErrorTitle_Text, "CodeLang.NoCamera_Text", CodeLang.ButtonClose_Text);
                return;
            }
#if IOS
            // Check and request camera permission for iOS.
            Task<PermissionStatus> task = CheckAndRequestCameraPermissionAsync();
#endif
            // The height of the title bar is lower when an iPhone is in horizontal position.
            if (DeviceInfo.Platform == DevicePlatform.iOS && DeviceInfo.Idiom == DeviceIdiom.Phone)
            {
                lblTitle.VerticalOptions = LayoutOptions.Start;
                lblTitle.VerticalTextAlignment = TextAlignment.Start;
                imgbtnTorch.VerticalOptions = LayoutOptions.Start;
            }

            // Initialize the barcode pickers
            pckFormatCodeScanner.ItemsSource = ClassBarcodes.GetFormatCodeListScanner();

            // Search for the name of the saved barcode in the picker list
            ClassBarcodes.nBarcodeScannerIndex = !string.IsNullOrEmpty(ClassBarcodes.cBarcodeScannerName)
                ? Globals.SearchIndexInPickerList(pckFormatCodeScanner, ClassBarcodes.cBarcodeScannerName)
                : -1;

            // If the saved barcode name was not found in the list then set the default index to 0 (All codes)
            if (ClassBarcodes.nBarcodeScannerIndex == -1)
            {
                ClassBarcodes.nBarcodeScannerIndex = 0;
                ClassBarcodes.cBarcodeScannerName = pckFormatCodeScanner.Items[ClassBarcodes.nBarcodeScannerIndex];

                Preferences.Default.Set("SettingBarcodeScannerName", ClassBarcodes.cBarcodeScannerName);
            }

            // Select the barcode format in the picker
            pckFormatCodeScanner.SelectedIndex = ClassBarcodes.nBarcodeScannerIndex;

            //// Set controls for text to speech
            if (Globals.bTextToSpeechAvailable)
            {
                lblTextToSpeech.IsVisible = true;
                imgbtnTextToSpeech.IsVisible = true;
            }

            // For testing crashes - DivideByZeroException.
            //int divByZero = 51 / int.Parse("0");
        }

        // Set the scanner properties for the selected format code.
        // ZXing CameraBarcodeReaderView options.
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
                string? item = itemsSource is not null && itemsSource.Count > selectedIndex
                    ? itemsSource[selectedIndex] as string : null;

                string? selectedName = item is not null
                    ? picker.ItemsSource[selectedIndex] as string : string.Empty;

                barcodeReader.Options = selectedName switch
                {
                    ClassBarcodes.cBarcode_AZTEC => new BarcodeReaderOptions
                    {
                        AutoRotate = true,
                        Formats = BarcodeFormat.Aztec,
                        Multiple = true,
                        TryHarder = true
                    },
                    ClassBarcodes.cBarcode_CODABAR => new BarcodeReaderOptions
                    {
                        AutoRotate = true,
                        Formats = BarcodeFormat.Codabar,
                        Multiple = true,
                        TryHarder = true
                    },
                    ClassBarcodes.cBarcode_CODE_128 => new BarcodeReaderOptions
                    {
                        AutoRotate = true,
                        Formats = BarcodeFormat.Code128,
                        Multiple = true,
                        TryHarder = true
                    },
                    ClassBarcodes.cBarcode_CODE_39 => new BarcodeReaderOptions
                    {
                        AutoRotate = true,
                        Formats = BarcodeFormat.Code39,
                        Multiple = true,
                        TryHarder = true
                    },
                    ClassBarcodes.cBarcode_CODE_93 => new BarcodeReaderOptions
                    {
                        AutoRotate = true,
                        Formats = BarcodeFormat.Code93,
                        Multiple = true,
                        TryHarder = true
                    },
                    ClassBarcodes.cBarcode_DATA_MATRIX => new BarcodeReaderOptions
                    {
                        AutoRotate = true,
                        Formats = BarcodeFormat.DataMatrix,
                        Multiple = true,
                        TryHarder = true
                    },
                    ClassBarcodes.cBarcode_EAN_13 => new BarcodeReaderOptions
                    {
                        AutoRotate = true,
                        Formats = BarcodeFormat.Ean13,
                        Multiple = true,
                        TryHarder = true
                    },
                    ClassBarcodes.cBarcode_EAN_8 => new BarcodeReaderOptions
                    {
                        AutoRotate = true,
                        Formats = BarcodeFormat.Ean8,
                        Multiple = true,
                        TryHarder = true
                    },
                    ClassBarcodes.cBarcode_IMB => new BarcodeReaderOptions
                    {
                        AutoRotate = true,
                        Formats = BarcodeFormat.Imb,
                        Multiple = true,
                        TryHarder = true
                    },
                    ClassBarcodes.cBarcode_ITF => new BarcodeReaderOptions
                    {
                        AutoRotate = true,
                        Formats = BarcodeFormat.Itf,
                        Multiple = true,
                        TryHarder = true
                    },
                    ClassBarcodes.cBarcode_MAXICODE => new BarcodeReaderOptions
                    {
                        AutoRotate = true,
                        Formats = BarcodeFormat.MaxiCode,
                        Multiple = true,
                        TryHarder = true
                    },
                    ClassBarcodes.cBarcode_MSI => new BarcodeReaderOptions
                    {
                        AutoRotate = true,
                        Formats = BarcodeFormat.Msi,
                        Multiple = true,
                        TryHarder = true
                    },
                    ClassBarcodes.cBarcode_PDF_417 => new BarcodeReaderOptions
                    {
                        AutoRotate = true,
                        Formats = BarcodeFormat.Pdf417,
                        Multiple = true,
                        TryHarder = true
                    },
                    ClassBarcodes.cBarcode_PHARMACODE => new BarcodeReaderOptions
                    {
                        AutoRotate = true,
                        Formats = BarcodeFormat.PharmaCode,
                        Multiple = true,
                        TryHarder = true
                    },
                    ClassBarcodes.cBarcode_PLESSEY => new BarcodeReaderOptions
                    {
                        AutoRotate = true,
                        Formats = BarcodeFormat.Plessey,
                        Multiple = true,
                        TryHarder = true
                    },
                    ClassBarcodes.cBarcode_QR_CODE => new BarcodeReaderOptions
                    {
                        AutoRotate = true,
                        Formats = BarcodeFormat.QrCode,
                        Multiple = true,
                        TryHarder = true
                    },
                    ClassBarcodes.cBarcode_RSS_14 => new BarcodeReaderOptions
                    {
                        AutoRotate = true,
                        Formats = BarcodeFormat.Rss14,
                        Multiple = true,
                        TryHarder = true
                    },
                    ClassBarcodes.cBarcode_RSS_EXPANDED => new BarcodeReaderOptions
                    {
                        AutoRotate = true,
                        Formats = BarcodeFormat.RssExpanded,
                        Multiple = true,
                        TryHarder = true
                    },
                    ClassBarcodes.cBarcode_UPC_A => new BarcodeReaderOptions
                    {
                        AutoRotate = true,
                        Formats = BarcodeFormat.UpcA,
                        Multiple = true,
                        TryHarder = true
                    },
                    ClassBarcodes.cBarcode_UPC_E => new BarcodeReaderOptions
                    {
                        AutoRotate = true,
                        Formats = BarcodeFormat.UpcE,
                        Multiple = true,
                        TryHarder = true
                    },
                    ClassBarcodes.cBarcode_UPC_EAN_EXTENSION => new BarcodeReaderOptions
                    {
                        AutoRotate = true,
                        Formats = BarcodeFormat.UpcEanExtension,
                        Multiple = true,
                        TryHarder = true
                    },
                    _ => new BarcodeReaderOptions
                    {
                        AutoRotate = true,
                        Formats = BarcodeFormats.All,
                        Multiple = true,
                        TryHarder = true
                    },
                };
            }
        }

        // Barcode detected event
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
                    var parts = list[0].Split(new[] { ":\n" }, StringSplitOptions.None);
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

        // Button share event.
        private void OnShareClicked(object sender, EventArgs e)
        {
            _ = Globals.ShareBarcodeResultAsync(lblBarcodeResult.Text);
        }

        // Set language text to speech using the Appearing event of the PageScanZX.xaml.
        private void OnPageAppearing(object sender, EventArgs e)
        {
            lblTextToSpeech.Text = Globals.GetIsoLanguageCode();
        }

        // ImageButton torch clicked event.
        private void OnTorchClicked(object sender, EventArgs e)
        {
            barcodeReader.IsTorchOn = !barcodeReader.IsTorchOn;
        }

        // Cancel the text to speech and turn off the torch if on, when going back to the mainpage.
        // Called by the Disappearing event from the PageScanZX.xaml.
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

        // Button text to speech event - Convert text to speech.
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

        // Copy text to the clipboard clicked event.
        private async void OnCopyToClipboardClicked(object sender, EventArgs e)
        {
            if (lblBarcodeResult.Text.Length > 0)
            {
                await Clipboard.Default.SetTextAsync(lblBarcodeResult.Text);
            }
        }

        // Check and request camera permission for iOS.
        public async Task<PermissionStatus> CheckAndRequestCameraPermissionAsync()
        {
            PermissionStatus status = await Permissions.CheckStatusAsync<Permissions.Camera>();

            if (status == PermissionStatus.Granted)
            {
                return status;
            }
        
            if (status == PermissionStatus.Unknown && DeviceInfo.Platform == DevicePlatform.iOS)
            {
                // Prompt the user to turn on in settings.
                // On iOS once a permission has been denied it may not be requested again from the application.
                await Application.Current!.Windows[0].Page!.DisplayAlertAsync("", CodeLang.CameraPermissionIOS_Text, CodeLang.ButtonClose_Text);
                return status;
            }

            return status;
        }
    }
}
