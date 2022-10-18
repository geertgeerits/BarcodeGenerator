using BarcodeGenerator.Resources.Languages;
using ZXing.Net.Maui;

namespace BarcodeGenerator;

public partial class PageScan : ContentPage
{
    // Local variables.
    private string cButtonShare;
    private string cButtonClose;
    private string cOpenLinkTitle;
    private string cOpenLinkText;
    private string cYes;
    private string cNo;
    private string cErrorTitle;

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

        // Put text in the chosen language in the controls and variables.
        lblTitle.Text = CodeLang.BarcodeScanner_Text;
        lblFormatCode.Text = CodeLang.FormatCode_Text;
        btnShare.Text = CodeLang.ButtonShareText_Text;

        cButtonShare = CodeLang.ButtonShareText_Text;
        cButtonClose = CodeLang.ButtonClose_Text;
        cOpenLinkTitle = CodeLang.OpenLinkTitle_Text;
        cOpenLinkText = CodeLang.OpenLinkText_Text;
        cYes = CodeLang.Yes_Text;
        cNo = CodeLang.No_Text;
        cErrorTitle = CodeLang.ErrorTitle_Text;

        // Default format code = QrCode.
        pckFormatCode.SelectedIndex = 20;
    }

    // ImageButton torch clicked event.
    private void OnTorchClicked(object sender, EventArgs e)
    {
        if (barcodeReader.IsTorchOn == false)
        {
            barcodeReader.IsTorchOn = true;
        }
        else
        {
            barcodeReader.IsTorchOn = false;
        }
    }

    // Set the scanner properties for the selected format code.
    // ZXing CameraBarcodeReaderView options.
    void OnPickerFormatCodeChanged(object sender, EventArgs e)
    {
        var picker = (Picker)sender;
        int selectedIndex = picker.SelectedIndex;

        if (selectedIndex != -1)
        {
            lblBarcodeResult.Text = "";
            btnShare.Text = cButtonShare;
            btnShare.IsEnabled = false;

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

                // Msi.
                case 10:
                    barcodeReader.Options = new BarcodeReaderOptions()
                    {
                        AutoRotate = true,
                        Formats = BarcodeFormat.Msi,
                        TryHarder = true,
                    };
                    break;

                // Pdf417.
                case 11:
                    barcodeReader.Options = new BarcodeReaderOptions()
                    {
                        AutoRotate = true,
                        Formats = BarcodeFormat.Pdf417,
                        TryHarder = true,
                    };
                    break;

                // PharmaCode.
                case 12:
                    barcodeReader.Options = new BarcodeReaderOptions()
                    {
                        AutoRotate = true,
                        Formats = BarcodeFormat.PharmaCode,
                        TryHarder = true,
                    };
                    break;

                // Plessey.
                case 13:
                    barcodeReader.Options = new BarcodeReaderOptions()
                    {
                        AutoRotate = true,
                        Formats = BarcodeFormat.Plessey,
                        TryHarder = true,
                    };
                    break;

                // QrCode.
                case 14:
                    barcodeReader.Options = new BarcodeReaderOptions()
                    {
                        AutoRotate = true,
                        Formats = BarcodeFormat.QrCode,
                        TryHarder = true,
                    };
                    break;

                // Rss14.
                case 15:
                    barcodeReader.Options = new BarcodeReaderOptions()
                    {
                        AutoRotate = true,
                        Formats = BarcodeFormat.Rss14,
                        TryHarder = true,
                    };
                    break;

                // RssExpanded.
                case 16:
                    barcodeReader.Options = new BarcodeReaderOptions()
                    {
                        AutoRotate = true,
                        Formats = BarcodeFormat.RssExpanded,
                        TryHarder = true,
                    };
                    break;

                // UpcA.
                case 17:
                    barcodeReader.Options = new BarcodeReaderOptions()
                    {
                        AutoRotate = true,
                        Formats = BarcodeFormat.UpcA,
                        TryHarder = true,
                    };
                    break;

                // UpcE.
                case 18:
                    barcodeReader.Options = new BarcodeReaderOptions()
                    {
                        AutoRotate = true,
                        Formats = BarcodeFormat.UpcE,
                        TryHarder = true,
                    };
                    break;

                // UpcEanExtension.
                case 19:
                    barcodeReader.Options = new BarcodeReaderOptions()
                    {
                        AutoRotate = true,
                        Formats = BarcodeFormat.UpcEanExtension,
                        TryHarder = true,
                    };
                    break;

                // All.
                case 20:
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
        // Open/Share link website.
        if (lblBarcodeResult.Text[..8] == "https://" || lblBarcodeResult.Text[..7] == "http://")
        {
            bool bAnswer = await DisplayAlert(cOpenLinkTitle, cOpenLinkText, cYes, cNo);

            // Open link website.
            if (bAnswer)
            {
                OpenWebsiteLink(lblBarcodeResult.Text);
            }
            // Share link website.
            else
            {
                await ShareText(lblBarcodeResult.Text);
            }
        }
        // Share the text.
        else
        {
            await ShareText(lblBarcodeResult.Text);
        }
    }

    // Open the website link.
    private async void OpenWebsiteLink(string cUrl)
    {
        try
        {
            await Launcher.OpenAsync(new Uri(cUrl));
        }
        catch (Exception ex)
        {
            await DisplayAlert(cErrorTitle, ex.Message, cButtonClose);
        }
    }

    // Open the share interface.
    public async Task ShareText(string cText)
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
}