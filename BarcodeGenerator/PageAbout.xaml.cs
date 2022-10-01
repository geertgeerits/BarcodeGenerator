namespace BarcodeGenerator;

public partial class PageAbout : ContentPage
{
	public PageAbout()
	{
        //InitializeComponent();
        try
        {
            InitializeComponent();
        }
        catch (Exception ex)
        {
            DisplayAlert("InitializeComponent PageAbout", ex.Message, "OK");
            return;
        }
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