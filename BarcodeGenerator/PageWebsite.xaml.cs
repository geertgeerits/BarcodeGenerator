namespace BarcodeGenerator;

public partial class PageWebsite : ContentPage
{
    public PageWebsite()
    {
        try
        {
            InitializeComponent();
        }
        catch (Exception ex)
        {
            DisplayAlert("InitializeComponent: PageWebsite", ex.Message, "OK");
            return;
        }

        // Set WebView properties.
        wvWebpage.Source = "https://geertgeerits.wixsite.com/barcodegenerator";
        wvWebpage.Navigating += OnNavigating;
        wvWebpage.Navigated += OnNavigated;
    }

    // if 'mailto' link in webpage then open the e-mail app.
    private async void OnNavigating(object sender, WebNavigatingEventArgs e)
    {
        if (e.Url.StartsWith("mailto"))
        {
            await Launcher.TryOpenAsync(e.Url);
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

    // Go backwards, if allowed.
    private void OnGoBackClicked(object sender, EventArgs e)
    {
        if (wvWebpage.CanGoBack)
        {
            wvWebpage.GoBack();
        }
    }

    // Go forwards, if allowed.
    private void OnGoForwardClicked(object sender, EventArgs e)
    {
        if (wvWebpage.CanGoForward)
        {
            wvWebpage.GoForward();
        }
    }
}