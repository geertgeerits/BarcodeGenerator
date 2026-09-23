using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using SkiaSharp;

namespace BarcodeGenerator
{
    public static class ClassBarcodeCaption
    {
        // Save using an existing IScreenshotResult-like object (no compile-time dependency).
        // This method uses reflection to call OpenReadAsync() on the provided object and forwards the stream.
        public static async Task<string> SaveBarcodeWithCaptionFromScreenshotAsync(object screenshotResult, string caption, bool includeCaptionInImage = false, CancellationToken ct = default)
        {
            if (screenshotResult == null) throw new ArgumentNullException(nameof(screenshotResult));

            // Try to find and invoke OpenReadAsync() via reflection to obtain the stream
            var mi = screenshotResult.GetType().GetMethod("OpenReadAsync", BindingFlags.Public | BindingFlags.Instance);
            if (mi == null) throw new InvalidOperationException("Provided screenshotResult does not have an OpenReadAsync() method.");

            var taskObj = mi.Invoke(screenshotResult, Array.Empty<object?>());
            if (taskObj == null) throw new InvalidOperationException("OpenReadAsync() returned null.");

            // Expect Task<Stream>
            var task = taskObj as Task;
            await task.ConfigureAwait(false);

            // Get Result property of Task<Stream>
            var resultProp = taskObj.GetType().GetProperty("Result", BindingFlags.Public | BindingFlags.Instance);
            if (resultProp == null) throw new InvalidOperationException("Unable to obtain stream result from OpenReadAsync() task.");

            var stream = resultProp.GetValue(taskObj) as Stream;
            if (stream == null) throw new InvalidOperationException("OpenReadAsync() did not return a Stream.");

            // Forward to stream-based implementation
            return await SaveBarcodeWithCaptionFromStreamAsync(stream, caption, includeCaptionInImage, ct).ConfigureAwait(false);
        }

        // New overload: accept an existing PNG Stream (e.g. File.OpenRead(ClassBarcodes.cFileBarcodePng))
        // Returns the path of the saved image (either a copy or image with caption).
        public static async Task<string> SaveBarcodeWithCaptionFromStreamAsync(Stream pngStream, string caption, bool includeCaptionInImage = false, CancellationToken ct = default)
        {
            if (pngStream == null) throw new ArgumentNullException(nameof(pngStream));

            // Read incoming stream into memory (so we can decode with SkiaSharp)
            using var ms = new MemoryStream();
            await pngStream.CopyToAsync(ms, ct).ConfigureAwait(false);
            ms.Seek(0, SeekOrigin.Begin);

            // Decide destination path
            string sourcePath = ClassBarcodes.cFileBarcodePng ?? string.Empty;
            string destDir;
            string baseName;
            if (!string.IsNullOrEmpty(sourcePath) && Path.IsPathRooted(sourcePath))
            {
                destDir = Path.GetDirectoryName(sourcePath) ?? Environment.CurrentDirectory;
                baseName = Path.GetFileNameWithoutExtension(sourcePath);
            }
            else
            {
                destDir = Environment.CurrentDirectory;
                baseName = "barcode";
            }

            string destFileName = includeCaptionInImage ? $"{baseName}_caption.png" : $"{baseName}_copy.png";
            string destPath = Path.Combine(destDir, destFileName);

            // If no caption required, just write the bytes and return
            if (!includeCaptionInImage || string.IsNullOrEmpty(caption))
            {
                await File.WriteAllBytesAsync(destPath, ms.ToArray(), ct).ConfigureAwait(false);
                return destPath;
            }

            // Decode PNG to SKBitmap
            ms.Seek(0, SeekOrigin.Begin);
            using var decoded = SKBitmap.Decode(ms);
            if (decoded == null)
            {
                // fallback: just write the original bytes
                await File.WriteAllBytesAsync(destPath, ms.ToArray(), ct).ConfigureAwait(false);
                return destPath;
            }

            // Prepare paint for caption and measure required caption height
            using var paint = new SKPaint
            {
                IsAntialias = true,
                Color = SKColors.Black,
                TextSize = Math.Max(20, decoded.Width / 24f),
                TextAlign = SKTextAlign.Center,
                Typeface = SKTypeface.Default
            };

            var fm = paint.FontMetrics;
            int captionHeight = (int)Math.Ceiling((fm.Descent - fm.Ascent) + 8); // add some padding

            int width = decoded.Width;
            int height = decoded.Height + captionHeight;

            var info = new SKImageInfo(width, height);
            using var surface = SKSurface.Create(info);
            var canvas = surface.Canvas;
            canvas.Clear(SKColors.White);

            // Draw original image at top
            var destRect = new SKRect(0, 0, width, decoded.Height);
            canvas.DrawBitmap(decoded, destRect);

            // Draw caption centered in the remaining area
            float x = width / 2f;
            // place baseline such that text is vertically centered in caption area
            float y = decoded.Height + (captionHeight / 2f) - (fm.Ascent + fm.Descent) / 2f;
            canvas.DrawText(caption, x, y, paint);

            using var image = surface.Snapshot();
            using var data = image.Encode(SKEncodedImageFormat.Png, 100);
            using var outFs = File.Open(destPath, FileMode.Create, FileAccess.Write, FileShare.None);
            data.SaveTo(outFs);
            await outFs.FlushAsync(ct).ConfigureAwait(false);

            return destPath;
        }
    }
}