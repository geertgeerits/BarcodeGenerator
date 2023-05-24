using BarcodeGenerator.Resources.Languages;
using ZXing.Net.Maui;
using System.Text.RegularExpressions;

namespace BarcodeGenerator;

public partial class PageScan : ContentPage
{
    // Local variables.
    private IEnumerable<Locale> locales;
    private CancellationTokenSource cts;
    private bool bTextToSpeechIsBusy = false;

    public PageScan()
    {
        try
        {
            InitializeComponent();
        }
        catch (Exception ex)
        {
            DisplayAlert("InitializeComponent: PageScan", ex.Message, "OK");
            return;
        }

        // Workaround for !!!BUG!!! in zxing:CameraBarcodeReaderView HeightRequest.
        // The camera sometimes overlaps adjacent rows in the grid.
        // Code to run on Android, Windows and on the main thread for iOS\MacOS.
#if IOS
        MainThread.BeginInvokeOnMainThread(SetGridRowHeightCamera);
        barcodeReader.HeightRequest = 400;
#else
        SetGridRowHeightCamera();
        barcodeReader.HeightRequest = 300;
#endif

        // Put text in the chosen language in the controls.
        pckFormatCodeScanner.ItemsSource = MainPage.GetFormatCodeListScanner();

        // Default format code = All codes
        pckFormatCodeScanner.SelectedIndex = MainPage.nFormatScannerIndex;

        // Initialize text to speech.
        InitializeTextToSpeech();
    }

    // ImageButton torch clicked event.
    private void OnTorchClicked(object sender, EventArgs e)
    {
        barcodeReader.IsTorchOn = !barcodeReader.IsTorchOn;
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
                btnShare.Text = CodeLang.ButtonShare_Text + " " + e.Results[0].Format;
                lblBarcodeResult.Text = e.Results[0].Value;

                imgbtnCopyToClipboard.IsEnabled = true;
                btnShare.IsEnabled = true;
                imgbtnTextToSpeech.IsEnabled = true;
            });
        }
        catch (Exception ex)
        {
            await DisplayAlert(CodeLang.ErrorTitle_Text, ex.Message, CodeLang.ButtonClose_Text);
        }
    }

    // Button share event.
    private async void OnShareClicked(object sender, EventArgs e)
    {
        // For testing.
        //lblBarcodeResult.Text = "http://www.google.com";
        //lblBarcodeResult.Text = "url http://www.google.com, visit website url https://www.microsoft.com, www.yahou.com and WWW.MODEGEERITS.BE and geertgeerits@gmail.com address";
        //lblBarcodeResult.Text = "Share text from barcode scanner";

        string cText = lblBarcodeResult.Text;

        //string cPattern = @"(http|ftp|https):\/\/([\w\-_]+(?:(?:\.[\w\-_]+)+))([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?";
        string cPattern = @"((https?|ftp|file)\://|www.)[A-Za-z0-9\.\-]+(/[A-Za-z0-9\?\&\=;\+!'\(\)\*\-\._~%]*)*";

        // Call Matches method for case-insensitive matching.
        try
        {
            foreach (Match match in Regex.Matches(cText, cPattern, RegexOptions.IgnoreCase).Cast<Match>())
            {
                if (match.Success)
                {
                    bool bAnswer = await DisplayAlert(CodeLang.OpenLinkTitle_Text, match.Value + "\n\n" + CodeLang.OpenLinkText_Text, CodeLang.Yes_Text, CodeLang.No_Text);

                    // Open link website.
                    if (bAnswer)
                    {
                        await OpenWebsiteLink(match.Value);
                    }
                }
                else
                {
                    break;
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert(CodeLang.ErrorTitle_Text, ex.Message, CodeLang.ButtonClose_Text);
        }

        // Wait 500 milliseconds otherwise the ShareText() is not executed after the last opened link.
        Task.Delay(500).Wait();

        // Open share interface.
        await ShareText(cText);
    }
   
    // Open the website link.
    private async Task OpenWebsiteLink(string cUrl)
    {
        if (cUrl[..4] is "www." or "WWW.")
        {
            cUrl = "http://" + cUrl;
        }

        try
        {
            Uri uri = new(cUrl);
            await Browser.Default.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
        }
        catch (Exception ex)
        {
            await DisplayAlert(CodeLang.ErrorTitle_Text, ex.Message, CodeLang.ButtonClose_Text);
        }
    }

    // Open the share interface.
    private async Task ShareText(string cText)
    {
        try
        {
            await Share.Default.RequestAsync(new ShareTextRequest
            {
                Text = cText,
                Title = "Barcode Scanner"
            });
        }
        catch (Exception ex)
        {
            await DisplayAlert(CodeLang.ErrorTitle_Text, ex.Message, CodeLang.ButtonClose_Text);
        }
    }

    // Set language text to speech using the Appearing event of the PageScan.xaml.
    private void OnPageAppearing(object sender, EventArgs e)
    {
        lblTextToSpeech.Text = MainPage.GetIsoLanguageCode();
    }

    // Turn off the torch if on, when going back to the mainpage.
    private void OnPageDisappearing(object sender, EventArgs e)
    {
        if (barcodeReader.IsTorchOn)
        {
            barcodeReader.IsTorchOn = false;
            Task.Delay(200).Wait();
        }
    }

    // Initialize text to speech.
    private async void InitializeTextToSpeech()
    {
        if (!MainPage.bLanguageLocalesExist)
        {
            return;
        }

        try
        {
            locales = await TextToSpeech.Default.GetLocalesAsync();
        }
        catch (Exception ex)
        {
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
            imgbtnTextToSpeech.Source = MainPage.cImageTextToSpeech;
            return;
        }

        // Start with the text to speech.
        if (lblBarcodeResult.Text != null && lblBarcodeResult.Text != "")
        {
            bTextToSpeechIsBusy = true;
            imgbtnTextToSpeech.Source = MainPage.cImageTextToSpeechCancel;

            try
            {
                cts = new CancellationTokenSource();

                SpeechOptions options = new()
                {
                    Locale = locales.Single(l => l.Language + "-" + l.Country + " " + l.Name == MainPage.cLanguageSpeech)
                };

                await TextToSpeech.Default.SpeakAsync(lblBarcodeResult.Text, options, cancelToken: cts.Token);
                bTextToSpeechIsBusy = false;
            }
            catch (Exception ex)
            {
                await DisplayAlert(CodeLang.ErrorTitle_Text, ex.Message, CodeLang.ButtonClose_Text);
            }

            imgbtnTextToSpeech.Source = MainPage.cImageTextToSpeech;
        }
    }

    // Copy text to the clipboard clicked event.
    private async void OnCopyToClipboardClicked(object sender, EventArgs e)
    {
        await Clipboard.Default.SetTextAsync(lblBarcodeResult.Text);
    }

    // Workaround for !!!BUG!!! in zxing:CameraBarcodeReaderView HeightRequest.
    // The camera sometimes overlaps adjacent rows in the grid.
    // Code to run on Android, Windows and on the main thread for iOS\MacOS.
    private void SetGridRowHeightCamera()
    {
        // Get the screen size.
        double nDisplayHeight;
        double nDisplayDensity;

        try
        {
            nDisplayHeight = DeviceDisplay.Current.MainDisplayInfo.Height;
            nDisplayDensity = DeviceDisplay.Current.MainDisplayInfo.Density;
        }
        catch
        {
            // Default values voor a Samsung Galaxy S21 phone.
            nDisplayHeight = 2340;
            nDisplayDensity = 2.75;
        }

        if (nDisplayDensity == 0)
        {
            nDisplayDensity = 1;
        }

        double nDisplayResHeight = nDisplayHeight / nDisplayDensity;

        // Set the grid row height.
        double nCameraRowHeight = 460;

        if (nDisplayResHeight < 740)
        {
            nCameraRowHeight = 540;
        }

        Grid grid = new()
        {
            RowDefinitions =
            {
                new RowDefinition { Height = new GridLength(50, GridUnitType.Star) },
                new RowDefinition { Height = new GridLength(10) },
                new RowDefinition { Height = new GridLength(nCameraRowHeight) },
                new RowDefinition { Height = new GridLength(10) },
                new RowDefinition { Height = new GridLength(50, GridUnitType.Star) },
                new RowDefinition { Height = new GridLength(100, GridUnitType.Star) }
            }
        };
        
        grdScanner.RowDefinitions = grid.RowDefinitions;
    }
}
