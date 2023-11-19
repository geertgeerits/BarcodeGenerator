namespace BarcodeGenerator;

public partial class PageAbout : ContentPage
{
    public PageAbout()
	{
        try
        {
            InitializeComponent();
        }
        catch (Exception ex)
        {
            Crashes.TrackError(ex);
            DisplayAlert("InitializeComponent: PageAbout", ex.Message, "OK");
            return;
        }

        // Put text in the chosen language in the controls.
        lblVersion.Text = $"{CodeLang.Version_Text} 1.0.37";
        lblCopyright.Text = $"{CodeLang.Copyright_Text} © 2022-2023 Geert Geerits";
        lblEmail.Text = $"{CodeLang.Email_Text} geertgeerits@gmail.com";
        lblWebsite.Text = $"{CodeLang.Website_Text}: ../barcodegenerator";
        lblPrivacyPolicy.Text = $"\n{CodeLang.PrivacyPolicyTitle_Text} {CodeLang.PrivacyPolicy_Text}";
        lblCrashErrorReport.Text = $"\n{CodeLang.CrashErrorReport_Text}";
        lblLicense.Text = $"\n{CodeLang.LicenseTitle_Text}: {CodeLang.License_Text}";
        lblExplanation.Text = $"\n{CodeLang.InfoExplanation_Text}";
        lblLicenseMit.Text = $"\n{CodeLang.Copyright_Text} © {CodeLang.LicenseMit_Text}\n\n{CodeLang.LicenseMit2_Text}";
    }

    // Open the e-mail program.
    private async void OnBtnEmailLinkClicked(object sender, EventArgs e)
    {
        if (Email.Default.IsComposeSupported)
        {
            string subject = "Barcode generator and scanner";
            string body = "";
            string[] recipients = ["geertgeerits@gmail.com"];

            var message = new EmailMessage
            {
                Subject = subject,
                Body = body,
                BodyFormat = EmailBodyFormat.PlainText,
                To = new List<string>(recipients)
            };

            try
            {
                await Email.Default.ComposeAsync(message);
            }
            catch (Exception ex)
            {
                await DisplayAlert(CodeLang.ErrorTitle_Text, ex.Message, CodeLang.ButtonClose_Text);
            }
        }
    }

    // Open the page 'PageWebsite' to open the website in the WebView control.
    // !!!BUG!!! in Android: the WebView control gives an error when opening a link to the Google Play Console.
    private async void OnBtnWebsiteLinkClicked(object sender, EventArgs e)
    {
#if ANDROID
        try
        {
            Uri uri = new("https://geertgeerits.wixsite.com/geertgeerits/barcode-generator");
            BrowserLaunchOptions options = new()
            {
                LaunchMode = BrowserLaunchMode.SystemPreferred,
                TitleMode = BrowserTitleMode.Show
            };

            await Browser.Default.OpenAsync(uri, options);
        }
        catch (Exception ex)
        {
            await DisplayAlert(CodeLang.ErrorTitle_Text, ex.Message, CodeLang.ButtonClose_Text);
        }
#else
        await Navigation.PushAsync(new PageWebsite());
#endif
    }
}