namespace BarcodeGenerator;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();

        // Set the language to test the application, otherwise comment out the next line.
        //System.Globalization.CultureInfo.CurrentCulture = new System.Globalization.CultureInfo("nl-BE");

        //MainPage = new AppShell();
        MainPage = new NavigationPage(new MainPage());
    }

    // Window dimensions and location for desktop apps.
    protected override Window CreateWindow(IActivationState activationState)
    {
        var window = base.CreateWindow(activationState);

        const int newHeight = 900;
        const int newWidth = 450;

        window.X = 200;
        window.Y = 50;

        window.Height = newHeight;
        window.Width = newWidth;

        window.MinimumHeight = 760;
        window.MinimumWidth = 360;

        return window;
    }
}
