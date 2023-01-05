using BarcodeGenerator.Resources.Languages;
using ZXing.Net.Maui;
using System.Text.RegularExpressions;

namespace BarcodeGenerator;

public partial class PageScan : ContentPage
{
    // Local variables.
    private readonly string cButtonShare;
    private readonly string cButtonClose;
    private readonly string cOpenLinkTitle;
    private readonly string cOpenLinkText;
    private readonly string cYes;
    private readonly string cNo;
    private readonly string cErrorTitle;
    private IEnumerable<Locale> locales;

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

        // Put text in the chosen language in the controls and variables.
        lblTitle.Text = CodeLang.BarcodeScanner_Text;
        lblFormatCode.Text = CodeLang.FormatCode_Text;
        btnShare.Text = CodeLang.ButtonShareText_Text;
        //btnTextToSpeech.Text = CodeLang.ButtonListen_Text;

        pckFormatCodeScanner.ItemsSource = MainPage.GetFormatCodeListScanner();

        cButtonShare = CodeLang.ButtonShareText_Text;
        cButtonClose = CodeLang.ButtonClose_Text;
        cOpenLinkTitle = CodeLang.OpenLinkTitle_Text;
        cOpenLinkText = CodeLang.OpenLinkText_Text;
        cYes = CodeLang.Yes_Text;
        cNo = CodeLang.No_Text;
        cErrorTitle = CodeLang.ErrorTitle_Text;

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
            btnShare.Text = cButtonShare;
            btnShare.IsEnabled = false;
            imgbtnTextToSpeech.IsEnabled = false;

            switch (selectedIndex)
            {
                // Aztec.
                case 0:
                    barcodeReader.Options = new BarcodeReaderOptions()
                    {
                        AutoRotate = true,
                        Formats = BarcodeFormat.Aztec,
                        TryHarder = true,
                    };
                    break;

                // Codabar.
                case 1:
                    barcodeReader.Options = new BarcodeReaderOptions()
                    {
                        AutoRotate = true,
                        Formats = BarcodeFormat.Codabar,
                        TryHarder = true,
                    };
                    break;

                // Code128.
                case 2:
                    barcodeReader.Options = new BarcodeReaderOptions()
                    {
                        AutoRotate = true,
                        Formats = BarcodeFormat.Code128,
                        TryHarder = true,
                    };
                    break;

                // Code39.
                case 3:
                    barcodeReader.Options = new BarcodeReaderOptions()
                    {
                        AutoRotate = true,
                        Formats = BarcodeFormat.Code39,
                        TryHarder = true,
                    };
                    break;

                // Code93.
                case 4:
                    barcodeReader.Options = new BarcodeReaderOptions()
                    {
                        AutoRotate = true,
                        Formats = BarcodeFormat.Code93,
                        TryHarder = true,
                    };
                    break;

                // DataMatrix.
                case 5:
                    barcodeReader.Options = new BarcodeReaderOptions()
                    {
                        AutoRotate = true,
                        Formats = BarcodeFormat.DataMatrix,
                        TryHarder = true,
                    };
                    break;

                // Ean13.
                case 6:
                    barcodeReader.Options = new BarcodeReaderOptions()
                    {
                        AutoRotate = true,
                        Formats = BarcodeFormat.Ean13,
                        TryHarder = true,
                    };
                    break;

                // Ean8.
                case 7:
                    barcodeReader.Options = new BarcodeReaderOptions()
                    {
                        AutoRotate = true,
                        Formats = BarcodeFormat.Ean8,
                        TryHarder = true,
                    };
                    break;

                // Imb.
                case 8:
                    barcodeReader.Options = new BarcodeReaderOptions()
                    {
                        AutoRotate = true,
                        Formats = BarcodeFormat.Imb,
                        TryHarder = true,
                    };
                    break;

                // Itf.
                case 9:
                    barcodeReader.Options = new BarcodeReaderOptions()
                    {
                        AutoRotate = true,
                        Formats = BarcodeFormat.Itf,
                        TryHarder = true,
                    };
                    break;

                // MaxiCode.
                case 10:
                    barcodeReader.Options = new BarcodeReaderOptions()
                    {
                        AutoRotate = true,
                        Formats = BarcodeFormat.MaxiCode,
                        TryHarder = true,
                    };
                    break;

                // Msi.
                case 11:
                    barcodeReader.Options = new BarcodeReaderOptions()
                    {
                        AutoRotate = true,
                        Formats = BarcodeFormat.Msi,
                        TryHarder = true,
                    };
                    break;

                // Pdf417.
                case 12:
                    barcodeReader.Options = new BarcodeReaderOptions()
                    {
                        AutoRotate = true,
                        Formats = BarcodeFormat.Pdf417,
                        TryHarder = true,
                    };
                    break;

                // PharmaCode.
                case 13:
                    barcodeReader.Options = new BarcodeReaderOptions()
                    {
                        AutoRotate = true,
                        Formats = BarcodeFormat.PharmaCode,
                        TryHarder = true,
                    };
                    break;

                // Plessey.
                case 14:
                    barcodeReader.Options = new BarcodeReaderOptions()
                    {
                        AutoRotate = true,
                        Formats = BarcodeFormat.Plessey,
                        TryHarder = true,
                    };
                    break;

                // QrCode.
                case 15:
                    barcodeReader.Options = new BarcodeReaderOptions()
                    {
                        AutoRotate = true,
                        Formats = BarcodeFormat.QrCode,
                        TryHarder = true,
                    };
                    break;

                // Rss14.
                case 16:
                    barcodeReader.Options = new BarcodeReaderOptions()
                    {
                        AutoRotate = true,
                        Formats = BarcodeFormat.Rss14,
                        TryHarder = true,
                    };
                    break;

                // RssExpanded.
                case 17:
                    barcodeReader.Options = new BarcodeReaderOptions()
                    {
                        AutoRotate = true,
                        Formats = BarcodeFormat.RssExpanded,
                        TryHarder = true,
                    };
                    break;

                // UpcA.
                case 18:
                    barcodeReader.Options = new BarcodeReaderOptions()
                    {
                        AutoRotate = true,
                        Formats = BarcodeFormat.UpcA,
                        TryHarder = true,
                    };
                    break;

                // UpcE.
                case 19:
                    barcodeReader.Options = new BarcodeReaderOptions()
                    {
                        AutoRotate = true,
                        Formats = BarcodeFormat.UpcE,
                        TryHarder = true,
                    };
                    break;

                // UpcEanExtension.
                case 20:
                    barcodeReader.Options = new BarcodeReaderOptions()
                    {
                        AutoRotate = true,
                        Formats = BarcodeFormat.UpcEanExtension,
                        TryHarder = true,
                    };
                    break;

                // All.
                case 21:
                    barcodeReader.Options = new BarcodeReaderOptions()
                    {
                        AutoRotate = true,
                        Formats = BarcodeFormats.All,
                        TryHarder = true,
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
                btnShare.Text = cButtonShare + " " + e.Results[0].Format;
                lblBarcodeResult.Text = e.Results[0].Value;

                btnShare.IsEnabled = true;
                imgbtnTextToSpeech.IsEnabled = true;
            });
        }
        catch (Exception ex)
        {
            await DisplayAlert(cErrorTitle, ex.Message, cButtonClose);
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
                    bool bAnswer = await DisplayAlert(cOpenLinkTitle, match.Value + "\n\n" + cOpenLinkText, cYes, cNo);

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
            await DisplayAlert(cErrorTitle, ex.Message, cButtonClose);
        }

        // Wait 500 milliseconds otherwise the ShareText() is not executed after the last opened link.
        Task.Delay(500).Wait();

        // Open share interface.
        await ShareText(cText);
    }

    // Initialize text to speech.
    private async void InitializeTextToSpeech()
    {
        try
        {
            locales = await TextToSpeech.GetLocalesAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert(cErrorTitle, ex.Message, cButtonClose);
        }
    }

    // Button text to speech event.
    private void OnTextToSpeechClicked(object sender, EventArgs e)
    {
        try
        {
            TextToSpeech.SpeakAsync(lblBarcodeResult.Text, new SpeechOptions
            {
                Locale = locales.Single(l => l.Language + "-" + l.Country + " " + l.Name == MainPage.cLanguageSpeech)
            });
        }
        catch (Exception ex)
        {
            DisplayAlert(cErrorTitle, ex.Message, cButtonClose);
        }
    }

    // Button text to speech cancel event.
    private void OnTextToSpeechCancelClicked(object sender, EventArgs e)
    {

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
            Uri uri = new Uri(cUrl);
            await Browser.Default.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
        }
        catch (Exception ex)
        {
            await DisplayAlert(cErrorTitle, ex.Message, cButtonClose);
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
            await DisplayAlert(cErrorTitle, ex.Message, cButtonClose);
        }
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
        double nCameraRowHeight = 440;

        if (nDisplayResHeight < 740)
        {
            nCameraRowHeight = 540;
        }

        Grid grid = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition { Height = new GridLength(40) },
                new RowDefinition { Height = new GridLength(10) },
                new RowDefinition { Height = new GridLength(nCameraRowHeight) },
                new RowDefinition { Height = new GridLength(10) },
                new RowDefinition { Height = new GridLength(40) },
                new RowDefinition { Height = new GridLength(100, GridUnitType.Star) }
            }
        };
        grdScanner.RowDefinitions = grid.RowDefinitions;
    }
}
