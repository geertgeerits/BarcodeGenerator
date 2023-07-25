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
        lblVersion.Text = $"{CodeLang.Version_Text} 1.0.34";
        lblCopyright.Text = $"{CodeLang.Copyright_Text} © 2022-2023 Geert Geerits";
        lblEmail.Text = $"{CodeLang.Email_Text} geertgeerits@gmail.com";
        lblWebsite.Text = $"{CodeLang.Website_Text}: ../barcodegenerator";
        lblPrivacyPolicy.Text = $"\n{CodeLang.PrivacyPolicyTitle_Text} {CodeLang.PrivacyPolicy_Text}";
        lblLicense.Text = $"\n{CodeLang.LicenseTitle_Text}: {CodeLang.License_Text}";
        lblExplanation.Text = $"\n{CodeLang.InfoExplanation_Text}";
        lblLicenseMit.Text = $"\n{CodeLang.Copyright_Text} © {CodeLang.LicenseMit_Text}\n\n{CodeLang.LicenseMit2_Text}";
    }

    // Open the e-mail program.
    private async void OnbtnEmailLinkClicked(object sender, EventArgs e)
    {
#if IOS || MACCATALYST
        string cAddress = "geertgeerits@gmail.com";

        try
        {
            await Launcher.OpenAsync(new Uri($"mailto:{cAddress}"));
        }
        catch (Exception ex)
        {
            await DisplayAlert(CodeLang.ErrorTitle_Text, ex.Message, CodeLang.ButtonClose_Text);
        }
#else
        if (Email.Default.IsComposeSupported)
        {
            string subject = "Barcode generator and scanner";
            string body = "";
            string[] recipients = new[] { "geertgeerits@gmail.com" };

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
#endif
    }

    // Open the page 'PageWebsite' to open the website in the WebView control.
    private async void OnbtnWebsiteLinkClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new PageWebsite());
    }

    // Open the website in the default browser.
    //private async void OnbtnWebsiteLinkClicked(object sender, EventArgs e)
    //{
    //    try
    //    {
    //        Uri uri = new("https://geertgeerits.wixsite.com/barcodegenerator");
    //        await Browser.Default.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
    //    }
    //    catch (Exception ex)
    //    {
    //        await DisplayAlert(CodeLang.ErrorTitle_Text, ex.Message, CodeLang.ButtonClose_Text);
    //    }
    //}
}