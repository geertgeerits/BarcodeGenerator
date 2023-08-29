using BarcodeScanner.Mobile;

namespace BarcodeGenerator;

public partial class PageScanGV : ContentPage
{
    // Local variables.
    private IEnumerable<Locale> locales;
    private CancellationTokenSource cts;
    private bool bTextToSpeechIsBusy = false;

    public PageScanGV()
    {
        try
        {
#if ANDROID31_0_OR_GREATER || IOS15_4_OR_GREATER
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
#endif
            InitializeComponent();
        }
        catch (Exception ex)
        {
            Crashes.TrackError(ex);
            DisplayAlert("InitializeComponent: PageScanGV", ex.Message, "OK");
            return;
        }

#if ANDROID31_0_OR_GREATER || IOS15_4_OR_GREATER
        // Ask for permission to use the camera.
        BarcodeScanner.Mobile.Methods.AskForRequiredPermission();
#endif

#if IOS
        // Check and request camera permission for iOS.
        //Task<PermissionStatus> task = CheckAndRequestCameraPermission();

        // The height of the title bar is lower when an iPhone is in horizontal position (!!!BUG!!! ?). 
        lblTitle.VerticalOptions = LayoutOptions.Start;
        imgbtnTorch.VerticalOptions = LayoutOptions.Start;
#endif

        // Put the default barcode format in the label control.
        string[] cFormatCode = Globals.GetFormatCodeListScanner().ToArray();
        lblFormatCodeScanner.Text = cFormatCode[Globals.nFormatScannerIndex];

        // Initialize text to speech.
        InitializeTextToSpeech();

        // For testing crashes - DivideByZeroException.
        //int divByZero = 51 / int.Parse("0");
    }

    // Barcode detected event.
    private async void OnBarcodesDetected(object sender, OnDetectedEventArg e)
    {
        lblBarcodeResult.Text = "";
        btnShare.Text = CodeLang.ButtonShare_Text;

        imgbtnCopyToClipboard.IsEnabled = false;
        btnShare.IsEnabled = false;
        imgbtnTextToSpeech.IsEnabled = false;

        try
        {
            List<BarcodeResult> obj = e.BarcodeResults;

            string cBarcodeFormat = Convert.ToString(obj[0].BarcodeFormat);
            string cDisplayValue = obj[0].DisplayValue;

            Dispatcher.Dispatch(() =>
            {
                barcodeReader.IsScanning = true;
            });

            btnShare.Text = $"{CodeLang.ButtonShare_Text} {cBarcodeFormat}";
            lblBarcodeResult.Text = cDisplayValue;

            imgbtnCopyToClipboard.IsEnabled = true;
            btnShare.IsEnabled = true;
            imgbtnTextToSpeech.IsEnabled = true;
        }
        catch (Exception ex)
        {
            await DisplayAlert(CodeLang.ErrorTitle_Text, ex.Message, CodeLang.ButtonClose_Text);
        }
    }

    // Button share event.
    private void OnShareClicked(object sender, EventArgs e)
    {
        Globals.ShareBarcodeResult(lblBarcodeResult.Text);
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

    // Turn off the torch if on, when going back to the mainpage.
    // Called by the Disappearing and Unloaded event from the PageScanGV.xaml.
    // Does not works always on Android but works on iOS.
    private void TurnOffTorch(object sender, EventArgs e)
    {
        if (barcodeReader.TorchOn)
        {
            barcodeReader.TorchOn = false;
            Task.Delay(300).Wait();
        }
    }

    // Initialize text to speech.
    private async void InitializeTextToSpeech()
    {
        if (!Globals.bLanguageLocalesExist)
        {
            return;
        }

        try
        {
            locales = await TextToSpeech.Default.GetLocalesAsync();
        }
        catch (Exception ex)
        {
            var properties = new Dictionary<string, string> {
                { "File:", "PageScanGV.xaml.cs" },
                { "Method:", "InitializeTextToSpeech" },
                { "AppLanguage:", Globals.cLanguage },
                { "AppLanguageSpeech:", Globals.cLanguageSpeech }
            };
            Crashes.TrackError(ex, properties);

            await DisplayAlert(CodeLang.ErrorTitle_Text, ex.Message, CodeLang.ButtonClose_Text);
            return;
        }

        lblTextToSpeech.IsVisible = true;
        imgbtnTextToSpeech.IsVisible = true;
    }

    // Button text to speech event.
    private async void OnTextToSpeechClicked(object sender, EventArgs e)
    {
        // Cancel the text to speech.
        if (bTextToSpeechIsBusy)
        {
            if (cts?.IsCancellationRequested ?? true)
                return;

            cts.Cancel();
            imgbtnTextToSpeech.Source = Globals.cImageTextToSpeech;
            return;
        }

        // Start with the text to speech.
        if (lblBarcodeResult.Text != null && lblBarcodeResult.Text != "")
        {
            bTextToSpeechIsBusy = true;
            imgbtnTextToSpeech.Source = Globals.cImageTextToSpeechCancel;

            try
            {
                cts = new CancellationTokenSource();

                SpeechOptions options = new()
                {
                    Locale = locales.Single(l => $"{l.Language}-{l.Country} {l.Name}" == Globals.cLanguageSpeech)
                };

                await TextToSpeech.Default.SpeakAsync(lblBarcodeResult.Text, options, cancelToken: cts.Token);
                bTextToSpeechIsBusy = false;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                await DisplayAlert(CodeLang.ErrorTitle_Text, ex.Message, CodeLang.ButtonClose_Text);
            }

            imgbtnTextToSpeech.Source = Globals.cImageTextToSpeech;
        }
    }

    // Copy text to the clipboard clicked event.
    private async void OnCopyToClipboardClicked(object sender, EventArgs e)
    {
        await Clipboard.Default.SetTextAsync(lblBarcodeResult.Text);
    }

    // Check and request camera permission for iOS.
    //public async Task<PermissionStatus> CheckAndRequestCameraPermission()
    //{
    //    PermissionStatus status = await Permissions.CheckStatusAsync<Permissions.Camera>();

    //    if (status == PermissionStatus.Granted)
    //    {
    //        return status;
    //    }

    //    if (status == PermissionStatus.Unknown && DeviceInfo.Platform == DevicePlatform.iOS)
    //    {
    //        // Prompt the user to turn on in settings.
    //        // On iOS once a permission has been denied it may not be requested again from the application.
    //        await DisplayAlert("", CodeLang.CameraPermissionIOS_Text, CodeLang.ButtonClose_Text);
    //        return status;
    //    }

    //    return status;
    //}
}
