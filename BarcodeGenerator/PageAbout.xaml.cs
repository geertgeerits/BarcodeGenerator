using System.Windows.Input;

namespace BarcodeGenerator
{
    public sealed partial class PageAbout : ContentPage
    {
        //// TapCommand to open the crash and error report privacy information (Launcher.OpenAsync is provided by Essentials)
        public ICommand TapCommand => new Command(async () => await DisplayAlert(CodeLang.CrashErrorReport_Text, CodeLang.CrashErrorReportSentry_Text, CodeLang.ButtonClose_Text));

        public PageAbout()
    	{
            try
            {
                InitializeComponent();
                BindingContext = this;
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
#if DEBUG
                DisplayAlert("InitializeComponent: PageAbout", ex.Message, "OK");
#endif
                return;
            }

            // Put text in the chosen language in the controls
            lblVersion.Text = $"{CodeLang.Version_Text} 1.0.42";
            lblCopyright.Text = $"{CodeLang.Copyright_Text} © 2022-2025 Geert Geerits";
            lblPrivacyPolicy.Text = $"\n{CodeLang.PrivacyPolicyTitle_Text} {CodeLang.PrivacyPolicy_Text}";
            lblLicense.Text = $"\n{CodeLang.LicenseTitle_Text}: {CodeLang.License_Text}";
            lblExplanation.Text = $"\n{CodeLang.InfoExplanation_Text}";
            lblLicenseMit.Text = $"\n{CodeLang.Copyright_Text} © {CodeLang.LicenseMit_Text}\n\n{CodeLang.LicenseMit2_Text}";
        }
    }

    /// <summary>
    /// Open e-mail app and open webpage (reusable hyperlink class)
    /// </summary>
    public sealed class HyperlinkSpan : Span
    {
        public static readonly BindableProperty UrlProperty =
            BindableProperty.Create(nameof(Url), typeof(string), typeof(HyperlinkSpan), null);

        public string Url
        {
            get { return (string)GetValue(UrlProperty); }
            set { SetValue(UrlProperty, value); }
        }

        public HyperlinkSpan()
        {
            FontAttributes = FontAttributes.Bold;
            TextDecorations = TextDecorations.Underline;
        
            GestureRecognizers.Add(new TapGestureRecognizer
            {
                // Launcher.OpenAsync is provided by Essentials
                //Command = new Command(async () => await Launcher.OpenAsync(Url))
                Command = new Command(async () => await OpenHyperlink(Url))
            });
        }

        /// <summary>
        /// Open the e-mail program or the website link
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private static async Task OpenHyperlink(string url)
        {
            if (url.StartsWith("mailto:"))
            {
                await OpenEmailLink(url[7..]);
            }
            else
            {
                await OpenWebsiteLink(url);
            }
        }

        /// <summary>
        /// Open the e-mail program
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private static async Task OpenEmailLink(string url)
        {
            if (Email.Default.IsComposeSupported)
            {
                string subject = "Barcode generator and scanner";
                string body = "";
                string[] recipients = [url];

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
                    await Application.Current!.Windows[0].Page!.DisplayAlert(CodeLang.ErrorTitle_Text, ex.Message, CodeLang.ButtonClose_Text);
                }
            }
        }

        /// <summary>
        /// Open the website link in the default browser
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private static async Task OpenWebsiteLink(string url)
        {
            try
            {
                Uri uri = new(url);
                BrowserLaunchOptions options = new()
                {
                    LaunchMode = BrowserLaunchMode.SystemPreferred,
                    TitleMode = BrowserTitleMode.Show
                };

                await Browser.Default.OpenAsync(uri, options);
            }
            catch (Exception ex)
            {
                await Application.Current!.Windows[0].Page!.DisplayAlert(CodeLang.ErrorTitle_Text, ex.Message, CodeLang.ButtonClose_Text);
            }
        }
    }
}