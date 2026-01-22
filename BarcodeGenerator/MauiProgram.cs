using Microsoft.Extensions.Logging;
using ZXing.Net.Maui.Controls;
using Microsoft.Maui.LifecycleEvents;
using BarcodeScanning;

namespace BarcodeGenerator
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                })
                .UseBarcodeReader()
                .UseBarcodeScanning()

                .UseSentry(options =>
                {
                    options.Dsn = "https://988f47dd765ca32afe832320e77a3a7d@o4507011442933760.ingest.us.sentry.io/4507011456565248";
                    //options.ShutdownTimeout = System.TimeSpan.FromSeconds(5);
                })

                .ConfigureLifecycleEvents(static events =>
                {
#if ANDROID
                    events.AddAndroid(android => android
                        .OnPause((activity) => ProcessEvent(nameof(AndroidLifecycle.OnPause))));
#elif IOS
                    events.AddiOS(ios => ios
                        .OnResignActivation((app) => ProcessEvent(nameof(iOSLifecycle.OnResignActivation))));
#elif WINDOWS
                    events.AddWindows(windows => windows
                        .OnVisibilityChanged((window, args) => ProcessEvent(nameof(WindowsLifecycle.OnVisibilityChanged))));
#endif
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }

        /// <summary>
        /// Process lifecycle event
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private static bool ProcessEvent(string eventName, string? type = null)
        {
            Debug.WriteLine($"Lifecycle event: {eventName}{(type is null ? string.Empty : $" ({type})")}");

            // Cancel speech
            ClassSpeech.CancelTextToSpeech();

            return true;
        }
    }
}
