using Microsoft.Maui.Graphics.Platform;

namespace BarcodeGenerator
{
    public static class ImageSizeHelper
    {
        public static async Task<(float pixelWidth, float pixelHeight)> GetPixelSizeAsync(ImageSource source)
        {
            if (source == null)
                return (0, 0);

            Stream? stream = source switch
            {
                FileImageSource file => await FileSystem.OpenAppPackageFileAsync(file.File),
                StreamImageSource sis => await sis.Stream(CancellationToken.None),
                UriImageSource uri => await ((IStreamImageSource)uri).GetStreamAsync(CancellationToken.None),
                _ => null
            };

            if (stream == null)
                return (0, 0);

            using var image = PlatformImage.FromStream(stream);
            return (image.Width, image.Height);
        }

        public static TaskCompletionSource<(double width, double height)>
            GetRenderedSizeAsync(Image image)
        {
            var tcs = new TaskCompletionSource<(double, double)>();

            void Handler(object? s, EventArgs e)
            {
                if (image.Width > 0 && image.Height > 0)
                {
                    image.SizeChanged -= Handler;
                    tcs.TrySetResult((image.Width, image.Height));
                }
            }

            image.SizeChanged += Handler;

            return tcs;
        }
    }

    /*
    How to use it
    1. Get original pixel size
    csharp
    var (pxW, pxH) = await ImageSizeHelper.GetPixelSizeAsync(MyImage.Source);
    Console.WriteLine($"Pixel size: {pxW} x {pxH}");

    2. Get rendered size
    csharp
    var (renderW, renderH) = await ImageSizeHelper.GetRenderedSizeAsync(MyImage);
    Console.WriteLine($"Rendered size: {renderW} x {renderH}");

    3. Combine both
    csharp
    var pixel = await ImageSizeHelper.GetPixelSizeAsync(MyImage.Source);
    var rendered = await ImageSizeHelper.GetRenderedSizeAsync(MyImage);

    Console.WriteLine($"Pixel: {pixel.pixelWidth} x {pixel.pixelHeight}");
    Console.WriteLine($"Rendered: {rendered.width} x {rendered.height}");
    */
}
