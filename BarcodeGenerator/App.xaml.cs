namespace BarcodeGenerator
{
    public sealed partial class App : Application
    {
        public App()
    	{
    		InitializeComponent();
        }

        /// <summary>
        /// Create a Window
        /// </summary>
        /// <param name="activationState"></param>
        /// <returns></returns>
        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}
