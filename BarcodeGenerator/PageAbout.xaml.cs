using System.Windows.Input;

namespace BarcodeGenerator;

public partial class PageAbout : ContentPage
{
    // TapCommand to open the crash and error report privacy information (Launcher.OpenAsync is provided by Essentials)
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
            //Crashes.TrackError(ex);
            SentrySdk.CaptureException(ex);
#if DEBUG
            DisplayAlert("InitializeComponent: PageAbout", ex.Message, "OK");
#endif
            return;
        }

        // Put text in the chosen language in the controls
        lblVersion.Text = $"{CodeLang.Version_Text} 1.0.39";
        lblCopyright.Text = $"{CodeLang.Copyright_Text} © 2022-2024 Geert Geerits";
        lblPrivacyPolicy.Text = $"\n{CodeLang.PrivacyPolicyTitle_Text} {CodeLang.PrivacyPolicy_Text}";
        lblLicense.Text = $"\n{CodeLang.LicenseTitle_Text}: {CodeLang.License_Text}";
        lblExplanation.Text = $"\n{CodeLang.InfoExplanation_Text}";
        lblLicenseMit.Text = $"\n{CodeLang.Copyright_Text} © {CodeLang.LicenseMit_Text}\n\n{CodeLang.LicenseMit2_Text}";
    }
}

// Open e-mail app and open webpage (reusable hyperlink class)
public class HyperlinkSpan : Span
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
            Command = new Command(async () => await Launcher.OpenAsync(Url))
        });
    }
}