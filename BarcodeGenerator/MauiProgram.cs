using Microsoft.Extensions.Logging;
//using BarcodeScanner.Mobile;
using ZXing.Net.Maui.Controls;
//using Microsoft.AppCenter;
//using Microsoft.AppCenter.Crashes;
using Microsoft.Maui.LifecycleEvents;
using BarcodeScanning;

namespace BarcodeGenerator;

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
            })

            //.ConfigureMauiHandlers(handlers =>
            //{
            //    // Add the handlers
            //    handlers.AddBarcodeScannerHandler();
            //})

            .ConfigureLifecycleEvents(events =>
            {
#if ANDROID
                events.AddAndroid(android => android
                    .OnPause((activity) => ProcessEvent(nameof(AndroidLifecycle.OnPause))));
#endif

#if IOS
                events.AddiOS(ios => ios
                    .OnResignActivation((app) => ProcessEvent(nameof(iOSLifecycle.OnResignActivation))));
#endif

#if WINDOWS
                events.AddWindows(windows => windows
                    .OnVisibilityChanged((window, args) => ProcessEvent(nameof(WindowsLifecycle.OnVisibilityChanged))));
#endif

                static bool ProcessEvent(string eventName, string type = null)
                {
                    //System.Diagnostics.Debug.WriteLine($"Lifecycle event: {eventName}{(type == null ? string.Empty : $" ({type})")}");

                    // Cancel speech if a cancellation token exists & hasn't been already requested.
                    if (Globals.bTextToSpeechIsBusy)
                    {
                        if (Globals.cts?.IsCancellationRequested ?? true)
                            return true;

                        Globals.cts.Cancel();
                        Globals.bTextToSpeechIsBusy = false;
                    }
                    
                    return true;
                }
            });

        //AppCenter.Start("windowsdesktop=7b81fd09-0be8-45e0-a4f0-4a23dd20a00c;" +
        //    "android=a7116ecb-f402-403c-8a9d-58e295913d73;" +
        //    "ios=e0281fe4-3662-46b5-a15f-fa0a73595d57;" +
        //    "macos={Your macOS App secret here};",
        //    typeof(Crashes));

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}

//.UseSentry(options =>
//{
//    options.Dsn = "https://988f47dd765ca32afe832320e77a3a7d@o4507011442933760.ingest.us.sentry.io/4507011456565248";
//})
