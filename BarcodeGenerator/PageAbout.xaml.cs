using BarcodeGenerator.Resources.Languages;

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
            DisplayAlert("InitializeComponent: PageAbout", ex.Message, "OK");
            return;
        }

        // Put text in the chosen language in the controls.
        lblTitle.Text = CodeLang.About_Text;

        lblNameProgram.Text = CodeLang.NameProgram_Text;
        lblDescription.Text = CodeLang.Description_Text;
        lblVersion.Text = CodeLang.Version_Text + " 1.0.17 Beta";
        lblCopyright.Text = CodeLang.Copyright_Text + " © 2022-2022 Geert Geerits";
        lblEmail.Text = CodeLang.Email_Text + " " + lblEmail.Text;
        lblWebsite.Text = CodeLang.Website_Text + " " + lblWebsite.Text;
        lblPrivacyPolicy.Text = CodeLang.PrivacyPolicyTitle_Text + " " + CodeLang.PrivacyPolicy_Text;
        lblLicense.Text = CodeLang.LicenseTitle_Text + ": " + CodeLang.License_Text;
        lblLicenseMit.Text = CodeLang.Copyright_Text + " © " + CodeLang.LicenseMit_Text;
        lblAboutExplanation.Text = CodeLang.AboutExplanation_Text;
    }

    // Open e-mail program.
    private void OnbtnEmailLinkClicked(object sender, EventArgs e)
    {
        string cAddress = "geertgeerits@gmail.com";

        try
        {
            Launcher.OpenAsync(new Uri($"mailto:{cAddress}"));
        }
        catch (Exception ex)
        {
            DisplayAlert("Error", ex.Message, "OK");
        }
    }

    // Open website.
    private void OnbtnWebsiteLinkClicked(object sender, EventArgs e)
    {
        string cUrl = "https://geertgeerits.wixsite.com/barcodegenerator";

        try
        {
            Launcher.OpenAsync(new Uri(cUrl));
        }
        catch (Exception ex)
        {
            DisplayAlert("Error", ex.Message, "OK");
        }
    }
}