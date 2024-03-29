using ZXing.Net.Maui;

namespace BarcodeGenerator;

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
            Crashes.TrackError(ex);
            DisplayAlert("InitializeComponent: PageScanZX", ex.Message, "OK");
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

        // Put text in the chosen language in the controls.
        pckFormatCodeScanner.ItemsSource = Globals.GetFormatCodeListScanner();

        // Default format code = All codes
        pckFormatCodeScanner.SelectedIndex = Globals.nFormatScannerIndex;

        // Initialize text to speech.
        if (Globals.InitializeTextToSpeechScannerAsync("PageScanZX.xaml.cs").Result)
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

            switch (selectedIndex)
            {
                // Aztec.
                case 0:
                    barcodeReader.Options = new BarcodeReaderOptions
                    {
                        AutoRotate = true,
                        Formats = BarcodeFormat.Aztec,
                        Multiple = false,
                        TryHarder = true
                    };
                    break;

                // Codabar.
                case 1:
                    barcodeReader.Options = new BarcodeReaderOptions
                    {
                        AutoRotate = true,
                        Formats = BarcodeFormat.Codabar,
                        Multiple = false,
                        TryHarder = true
                    };
                    break;

                // Code128.
                case 2:
                    barcodeReader.Options = new BarcodeReaderOptions
                    {
                        AutoRotate = true,
                        Formats = BarcodeFormat.Code128,
                        Multiple = false,
                        TryHarder = true
                    };
                    break;

                // Code39.
                case 3:
                    barcodeReader.Options = new BarcodeReaderOptions
                    {
                        AutoRotate = true,
                        Formats = BarcodeFormat.Code39,
                        Multiple = false,
                        TryHarder = true
                    };
                    break;

                // Code93.
                case 4:
                    barcodeReader.Options = new BarcodeReaderOptions
                    {
                        AutoRotate = true,
                        Formats = BarcodeFormat.Code93,
                        Multiple = false,
                        TryHarder = true
                    };
                    break;

                // DataMatrix.
                case 5:
                    barcodeReader.Options = new BarcodeReaderOptions
                    {
                        AutoRotate = true,
                        Formats = BarcodeFormat.DataMatrix,
                        Multiple = false,
                        TryHarder = true
                    };
                    break;

                // Ean13.
                case 6:
                    barcodeReader.Options = new BarcodeReaderOptions
                    {
                        AutoRotate = true,
                        Formats = BarcodeFormat.Ean13,
                        Multiple = false,
                        TryHarder = true
                    };
                    break;

                // Ean8.
                case 7:
                    barcodeReader.Options = new BarcodeReaderOptions
                    {
                        AutoRotate = true,
                        Formats = BarcodeFormat.Ean8,
                        Multiple = false,
                        TryHarder = true
                    };
                    break;

                // Imb.
                case 8:
                    barcodeReader.Options = new BarcodeReaderOptions
                    {
                        AutoRotate = true,
                        Formats = BarcodeFormat.Imb,
                        Multiple = false,
                        TryHarder = true
                    };
                    break;

                // Itf.
                case 9:
                    barcodeReader.Options = new BarcodeReaderOptions
                    {
                        AutoRotate = true,
                        Formats = BarcodeFormat.Itf,
                        Multiple = false,
                        TryHarder = true
                    };
                    break;

                // MaxiCode.
                case 10:
                    barcodeReader.Options = new BarcodeReaderOptions
                    {
                        AutoRotate = true,
                        Formats = BarcodeFormat.MaxiCode,
                        Multiple = false,
                        TryHarder = true
                    };
                    break;

                // Msi.
                case 11:
                    barcodeReader.Options = new BarcodeReaderOptions
                    {
                        AutoRotate = true,
                        Formats = BarcodeFormat.Msi,
                        Multiple = false,
                        TryHarder = true
                    };
                    break;

                // Pdf417.
                case 12:
                    barcodeReader.Options = new BarcodeReaderOptions
                    {
                        AutoRotate = true,
                        Formats = BarcodeFormat.Pdf417,
                        Multiple = false,
                        TryHarder = true
                    };
                    break;

                // PharmaCode.
                case 13:
                    barcodeReader.Options = new BarcodeReaderOptions
                    {
                        AutoRotate = true,
                        Formats = BarcodeFormat.PharmaCode,
                        Multiple = false,
                        TryHarder = true
                    };
                    break;

                // Plessey.
                case 14:
                    barcodeReader.Options = new BarcodeReaderOptions
                    {
                        AutoRotate = true,
                        Formats = BarcodeFormat.Plessey,
                        Multiple = false,
                        TryHarder = true
                    };
                    break;

                // QrCode.
                case 15:
                    barcodeReader.Options = new BarcodeReaderOptions
                    {
                        AutoRotate = true,
                        Formats = BarcodeFormat.QrCode,
                        Multiple = false,
                        TryHarder = true
                    };
                    break;

                // Rss14.
                case 16:
                    barcodeReader.Options = new BarcodeReaderOptions
                    {
                        AutoRotate = true,
                        Formats = BarcodeFormat.Rss14,
                        Multiple = false,
                        TryHarder = true
                    };
                    break;

                // RssExpanded.
                case 17:
                    barcodeReader.Options = new BarcodeReaderOptions
                    {
                        AutoRotate = true,
                        Formats = BarcodeFormat.RssExpanded,
                        Multiple = false,
                        TryHarder = true
                    };
                    break;

                // UpcA.
                case 18:
                    barcodeReader.Options = new BarcodeReaderOptions
                    {
                        AutoRotate = true,
                        Formats = BarcodeFormat.UpcA,
                        Multiple = false,
                        TryHarder = true
                    };
                    break;

                // UpcE.
                case 19:
                    barcodeReader.Options = new BarcodeReaderOptions
                    {
                        AutoRotate = true,
                        Formats = BarcodeFormat.UpcE,
                        Multiple = false,
                        TryHarder = true
                    };
                    break;

                // UpcEanExtension.
                case 20:
                    barcodeReader.Options = new BarcodeReaderOptions
                    {
                        AutoRotate = true,
                        Formats = BarcodeFormat.UpcEanExtension,
                        Multiple = false,
                        TryHarder = true
                    };
                    break;

                // All.
                case 21:
                    barcodeReader.Options = new BarcodeReaderOptions
                    {
                        AutoRotate = true,
                        Formats = BarcodeFormats.All,
                        Multiple = false,
                        TryHarder = true
                    };
                    break;
            }
        }
    }

    // Barcode detected event.
    private async void OnBarcodesDetected(object sender, BarcodeDetectionEventArgs e)
    {
        try
        {
            Dispatcher.Dispatch(() =>
            {
                btnShare.Text = $"{CodeLang.ButtonShare_Text} {e.Results[0].Format}";
                lblBarcodeResult.Text = e.Results[0].Value;

                imgbtnCopyToClipboard.IsEnabled = true;
                btnShare.IsEnabled = true;
                imgbtnTextToSpeech.IsEnabled = true;
            });
        }
        catch (Exception ex)
        {
            Crashes.TrackError(ex);
            await DisplayAlert(CodeLang.ErrorTitle_Text, ex.Message, CodeLang.ButtonClose_Text);
        }
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
        imgbtnTextToSpeech.Source = Globals.CancelTextToSpeech();

        if (barcodeReader.IsTorchOn)
        {
            barcodeReader.IsTorchOn = false;
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
            await DisplayAlert("", CodeLang.CameraPermissionIOS_Text, CodeLang.ButtonClose_Text);
            return status;
        }

        return status;
    }
}
