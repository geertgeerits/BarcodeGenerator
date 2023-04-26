namespace BarcodeGenerator;

public partial class PageWebsite : ContentPage
{
    // Local variables.
    private readonly string cUrl = "https://geertgeerits.wixsite.com/barcodegenerator";

    public PageWebsite()
	{
		InitializeComponent();

        // Set WebView properties.
        wvWebpage.Source = cUrl;
        wvWebpage.Navigating += OnNavigating;
        wvWebpage.Navigated += OnNavigated;
    }

    // if 'mailto' link in webpage then open the e-mail app.
    private async void OnNavigating(object sender, WebNavigatingEventArgs e)
    {
        if (e.Url.StartsWith("mailto"))
        {
            //e.Cancel = true;
            //await Browser.OpenAsync(e.Url);
            await Launcher.TryOpenAsync(e.Url);
            //await Launcher.OpenAsync(new Uri($"mailto:{"geertgeerits@gmail.com"}"));
            e.Cancel = true;
        }
    }

    // The following code changes the target of all the links in _self.
    private void OnNavigated(object sender, WebNavigatedEventArgs e)
    {
        wvWebpage.EvaluateJavaScriptAsync(@"(function() {
            var links = document.getElementsByTagName('a');
            for (var i = 0; i < links.length; i++)
            {
                links[i].setAttribute('target', '_self');
            }
        })()");
    }

    // Return to the first website.
    private void btnHomeClicked(object sender, EventArgs e)
    {
        wvWebpage.Source = cUrl;
    }

    // Return to the previous page.
    private void btnGoBackClicked(object sender, EventArgs e)
    {
        // Go backwards, if allowed.
        if (wvWebpage.CanGoBack)
        {
            wvWebpage.GoBack();
        }

        // Go forwards, if allowed.
        //if (wvWebpage.CanGoForward)
        //{
        //    wvWebpage.GoForward();
        //}
    }
}