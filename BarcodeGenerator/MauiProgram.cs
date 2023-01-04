using Microsoft.Extensions.Logging;
using ZXing.Net.Maui.Controls;
using BarcodeGenerator.Platforms;

namespace BarcodeGenerator;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseBarcodeReader()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        builder.Services.AddTransient<MainPage>();
        //builder.Services.AddSingleton<ISpeechToText>();
        builder.Services.AddSingleton<ISpeechToText, SpeechToTextImplementation>();

        return builder.Build();
    }
}

