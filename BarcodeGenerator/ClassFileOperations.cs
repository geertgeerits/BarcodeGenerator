using SkiaSharp;

namespace BarcodeGenerator
{
    internal class ClassFileOperations
    {
        /// <summary>
        /// Opens a media picker dialog and return the selected file.
        /// </summary>
        /// <returns>The selected file result, or null if no file was selected.</returns>
        public static async Task<FileResult?> PickImage()
        {
            try
            {
                // Let user pick photos (multiple possible). We take the first one
                List<FileResult> photos = await MediaPicker.Default.PickPhotosAsync();
                FileResult? selected = photos?.FirstOrDefault();

                // Get the file name with extension
                // FileResult.FileName provides the name including the extension
                string? fileNameWithExt = selected?.FileName;

                Debug.WriteLine($"ClassFileOperations.PickImage: selected file name: {fileNameWithExt ?? "<none>"}");

                // Validate the selected file
                if (!string.IsNullOrEmpty(fileNameWithExt))
                {
                    if (fileNameWithExt.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                        fileNameWithExt.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                        fileNameWithExt.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase))
                    {
                        return selected;
                    }
                    else
                    {
                        await Application.Current!.Windows[0].Page!.DisplayAlertAsync(CodeLang.ErrorTitle_Text, $"{fileNameWithExt}\n\n{CodeLang.ErrorInvalidImageType_Text}", CodeLang.ButtonClose_Text);
                        return null;
                    }
                }

                return selected;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ClassFileOperations.PickImage: File picking error: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Opens a media picker dialog that allows the user to select one image file (PNG, JPG, JPEG, or BMP) and validates the selected file type.
        /// </summary>
        /// <returns>The selected file result, or null if no file was selected.</returns>
        /// <remarks>On iOS, if the user selects an invalid file type, the method attempts to clean up any temporary cached copy
        /// of the file in the app cache directory.</remarks>
        public static async Task<FileResult?> PickOneImage()
        {
            try
            {
                // Let user pick a photo. We take the first one
                FileResult? selected;
#if IOS
                // !!!BUG!!! in iOS, the temporary cached copy may not always be deleted correctly when using the 'MediaPickerOptions'
                var photos = await MediaPicker.Default.PickPhotosAsync();
                //selected = photos?.FirstOrDefault();

                // Correct the orientation of the selected image stream based on EXIF data
                // !!!BUG!!! in iOS, the orientation of the bounding box of the selected images may be incorrect with jpeg, heic and heif files.
                // However, the 'GetSelectedImageStreamAsync()' method does not correct the orientation of the bounding box
                selected = (await GetSelectedImageStreamAsync(photos)).selected;
#else
                List<FileResult> photos = await MediaPicker.Default.PickPhotosAsync(new MediaPickerOptions
                {
                    SelectionLimit = 1,             // Default is 1; set to 0 for no limit
                    RotateImage = true,
                    PreserveMetaData = true
                });
                
                selected = photos?.FirstOrDefault();
#endif
                // Get the file name with extension
                // FileResult.FileName provides the name including the extension
                string? fileNameWithExt = selected?.FileName;

                Debug.WriteLine($"ClassFileOperations.PickImage: selected file name: {fileNameWithExt ?? "<none>"}");

                // Validate the selected file
                if (!string.IsNullOrEmpty(fileNameWithExt))
                {
                    return selected;
                }
                else
                {
                    // On iOS the picker may leave a temporary cached copy. Make sure we close any handle and remove it
                    try
                    {
                        if (!string.IsNullOrEmpty(selected?.FullPath))
                        {
                            // Try to dispose any open stream (harmless if it fails)
                            try
                            {
                                using Stream s = await selected.OpenReadAsync();
                            }
                            catch
                            {
                                /* ignore */
                            }

                            DeleteFileInCache(selected.FullPath);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"ClassFileOperations.PickImage: cleanup error: {ex.Message}");
                    }

                    await Application.Current!.Windows[0].Page!.DisplayAlertAsync(CodeLang.ErrorTitle_Text, $"{fileNameWithExt}\n\n{CodeLang.ErrorInvalidImageType_Text}", CodeLang.ButtonClose_Text);
                    return null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ClassFileOperations.PickImage: File picking error: {ex.Message}");
                return null;
            }
        }

        /* Alternatives for 'PickOneImage()' method using custom file type filters.
         * The following code is commented out but kept for reference.
           
            var customFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.Android, new[] { "image/png", "image/jpeg" } },    // MIME type
                { DevicePlatform.iOS, new[] { "public.png", "public.jpeg" } },      // UTType values
                { DevicePlatform.WinUI, new[] { ".png", ".jpeg", ".jpg" } }         // file extension
            });

            PickOptions pickerOptions = new()
            {
                PickerTitle = "",
                FileTypes = FilePickerFileType.Images   // or use 'customFileType' for custom filtering
            };


            if (file != null)
            {
                string ext = Path.GetExtension(fullPath).ToLowerInvariant();
                Debug.WriteLine($"ClassFileOperations.PickOneImage: selected file extension: {ext}");

                if (ext is ".png" or ".jpg" or ".jpeg" or ".bmp")
                {
                    // OK
                    return file;
                }
            }
        */

        //public async static Task<Stream> ConvertImageSourceToStreamAsync(ImageSource imageSource)
        //{ 
        //    using var stream = await ((StreamImageSource)imageSource).Stream(CancellationToken.None); return stream;
        //}

        public static async Task<Stream> ConvertImageSourceToStreamAsync(ImageSource imageSource)
        {
            var sis = imageSource as StreamImageSource ?? throw new InvalidOperationException("ImageSource is not a StreamImageSource");
            Stream stream = await sis.Stream(CancellationToken.None);
            return stream;
        }

        /// <summary>
        /// Asynchronously saves a PNG image from the specified memory stream to the given file path.
        /// </summary>
        /// <remarks>The method sets the stream position to the beginning before saving and creates the
        /// target directory if it does not exist.</remarks>
        /// <param name="stream">The memory stream containing the PNG image data to save. Must not be null or empty.</param>
        /// <param name="filePath">The full file path where the PNG image will be saved. Must be a valid, non-empty path.</param>
        /// <returns>A task that represents the asynchronous save operation.</returns>
        /// <exception cref="ArgumentException">Thrown if <paramref name="stream"/> is null or empty, or if <paramref name="filePath"/> is null, empty,
        /// or consists only of white-space characters.</exception>
        /// <exception cref="IOException">Thrown if an I/O error occurs while saving the PNG file, such as issues with file access or directory
        /// creation.</exception>
        public static async Task SavePngFromStreamAsync(MemoryStream stream, string filePath)
        {
            if (stream == null || stream.Length == 0)
            {
                Debug.WriteLine("ClassFileOperations.SavePngFromStreamAsync: PNG stream is null or empty.", nameof(stream));
                return;
            }

            if (string.IsNullOrWhiteSpace(filePath))
            {
                Debug.WriteLine("ClassFileOperations.SavePngFromStreamAsync: Invalid file path.", nameof(filePath));
                return;
            }

            try
            {
                // Ensure the stream is at the beginning
                stream.Position = 0;

                // Create directory if it doesn't exist
                string? directory = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // Save the file asynchronously
                using FileStream fileStream = new(filePath, FileMode.Create, FileAccess.Write);
                await stream.CopyToAsync(fileStream);
            }
            catch (Exception ex)
            {
                // Log or handle the error as needed
                Debug.WriteLine($"ClassFileOperations.SavePngFromStreamAsync: Failed to save PNG file: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Save the barcode as an image file
        /// </summary>
        /// <param name="inputStream"></param>
        /// <param name="filePath"></param>
        public static void SaveStreamAsFilePng(Stream inputStream, string filePath)
        {
            if (inputStream == null || inputStream.Length == 0)
            {
                return;
            }
            
            // Save the image file
            try
            {
                using FileStream outputFileStream = new(filePath, FileMode.Create);
                inputStream.CopyTo(outputFileStream);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ClassFileOperations.SaveStreamAsFilePng: Failed to save image file: {ex.Message}", ex);
            }

            inputStream.Dispose();
        }

        /// <summary>
        /// Save the barcode as an SVG file
        /// </summary>
        /// <param name="svgContent"></param>
        /// <param name="filePath"></param>
        public static void SaveStringAsFileSvg(string svgContent, string filePath)
        {
            if (string.IsNullOrWhiteSpace(svgContent))
            {
                return;
            }

            // Save the image file
            try
            {
                using FileStream outputFileStream = new(filePath, FileMode.Create);
                byte[] svgBytes = System.Text.Encoding.UTF8.GetBytes(svgContent);
                using MemoryStream inputStream = new(svgBytes);
                inputStream.CopyTo(outputFileStream);
                inputStream.Dispose();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ClassFileOperations.SaveStringAsFileSvg: Failed to save SVG file: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Open the share interface
        /// </summary>
        /// <param name="cFile"></param>
        /// <returns></returns>
        public static async Task OpenShareInterfaceAsync(string cFile)
        {
            if (!string.IsNullOrEmpty(cFile))
            {
                await Share.Default.RequestAsync(new ShareFileRequest
                {
                    Title = "Barcode Generator",
                    File = new ShareFile(cFile)
                });
            }
        }

        /// <summary>
        /// Shares multiple files using the platform's share interface
        /// This method checks for the existence of the specified files before attempting to share them
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> ShareMultipleFilesAsync()
        {
            if (!File.Exists(ClassBarcodes.cFileBarcodePng) || !File.Exists(ClassBarcodes.cFileBarcodeSvg))
            {
                Debug.WriteLine("ClassFileOperations.ShareMultipleFilesAsync: One or more files to share do not exist.");
                return false;
            }

            await Share.Default.RequestAsync(new ShareMultipleFilesRequest
            {
                Title = "Barcode Generator",
                //Files = [new(ClassBarcodes.cFileBarcodePng), new ShareFile(ClassBarcodes.cFileBarcodeSvg)]
                Files = [new ShareFile(ClassBarcodes.cFileBarcodePng), new ShareFile(ClassBarcodes.cFileBarcodeSvg)]
            });
            
            return true;
        }

        /// <summary>
        /// Deletes the specified file if it exists
        /// </summary>
        /// <param name="filePath">The path of the file to delete</param>
        public static void DeleteFileIfExists(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    Debug.WriteLine($"ClassFileOperations.DeleteFileIfExists: Deleted existing file at: {filePath}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ClassFileOperations.DeleteFileIfExists: Failed to delete file at {filePath}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Deletes the specified file if it exists in the app cache directory
        /// </summary>
        /// <param name="filePath">The path of the file to delete</param>
        public static void DeleteFileInCache(string filePath)
        {
            Debug.WriteLine($"ClassFileOperations.DeleteFileInCache: Attempting to delete file in cache at: {filePath}");

            try
            {
                if (!string.IsNullOrEmpty(filePath))
                {
                    var cacheDir = FileSystem.CacheDirectory;
                    if (filePath.StartsWith(cacheDir, StringComparison.OrdinalIgnoreCase) && File.Exists(filePath))
                    {
                        File.Delete(filePath);
                        Debug.WriteLine($"ClassFileOperations.DeleteFileInCache: Deleted existing cache file at: {filePath}");
                    }
                    else if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                        Debug.WriteLine($"ClassFileOperations.DeleteFileInCache: Deleted existing file at: {filePath}");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ClassFileOperations.DeleteFileInCache: Failed to delete file at {filePath}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets the first selected image stream from a collection of file results, correcting the orientation based on EXIF data if necessary.
        /// </summary>
        /// <param name="photos">A collection of file results representing the selected images.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a tuple with the image stream and the selected file result, or null if no file is selected or cannot be processed.</returns>
        public async static Task<(Stream? stream, FileResult? selected)> GetSelectedImageStreamAsync(IEnumerable<FileResult>? photos)
        {
            FileResult? selected = photos?.FirstOrDefault();
            if (selected == null)
            {
                return (null, null);
            }

            Stream? stream = await GetImageStreamWithCorrectOrientationAsync(selected);
            return (stream, selected);
        }

        /// <summary>
        /// Gets an image stream from the specified file result, correcting the orientation based on EXIF data if necessary.
        /// </summary>
        /// <param name="file">The file result representing the image.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the image stream with the correct orientation, or null if the file is null or cannot be processed.</returns>
        public static async Task<Stream?> GetImageStreamWithCorrectOrientationAsync(FileResult? file)
        {
            if (file == null)
            {
                return null;
            }

            // Read into memory so SKCodec can seek
            MemoryStream input = new();
            using (Stream s = await file.OpenReadAsync())
            await s.CopyToAsync(input);
            input.Position = 0;

            SKBitmap bitmap;
            using (SKCodec? codec = SKCodec.Create(input))
            {
                if (codec != null)
                {
                    SKImageInfo info = codec.Info;
                    bitmap = new SKBitmap(info.Width, info.Height, info.ColorType, info.AlphaType);
                    codec.GetPixels(bitmap.Info, bitmap.GetPixels());

                    // Fix orientation according to EXIF
                    SKBitmap oriented = FixOrientation(bitmap, codec.EncodedOrigin);
                    if (!ReferenceEquals(oriented, bitmap))
                    {
                        bitmap.Dispose();
                        bitmap = oriented;
                    }
                }
                else
                {
                    input.Position = 0;
                    bitmap = SKBitmap.Decode(input);
                    if (bitmap == null) return null;
                }
            }

            // Encode to PNG into a MemoryStream and return
            using SKImage image = SKImage.FromBitmap(bitmap);
            using SKData encoded = image.Encode(SKEncodedImageFormat.Png, 100);
            MemoryStream output = new();
            encoded.SaveTo(output);
            output.Position = 0;

            bitmap.Dispose();
            return output;
        }

        // Helper rotation/flip methods (same strategy as used in ClassQRCodeImage)
        private static SKBitmap FixOrientation(SKBitmap src, SKEncodedOrigin origin)
        {
            switch (origin)
            {
                case SKEncodedOrigin.TopLeft: return src;
                case SKEncodedOrigin.TopRight: return FlipBitmap(src, horizontal: true);
                case SKEncodedOrigin.BottomRight: return RotateBitmap(src, 180);
                case SKEncodedOrigin.BottomLeft: return FlipBitmap(src, horizontal: false);
                case SKEncodedOrigin.LeftTop:
                    using (SKBitmap r1 = RotateBitmap(src, 90))
                    using (SKBitmap f1 = FlipBitmap(r1, horizontal: true))
                    {
                        return f1;
                    }

                case SKEncodedOrigin.RightTop: return RotateBitmap(src, 90);
                case SKEncodedOrigin.RightBottom:
                    using (SKBitmap r2 = RotateBitmap(src, 270))
                    using (SKBitmap f2 = FlipBitmap(r2, horizontal: true))
                    {
                        return f2;
                    }

                case SKEncodedOrigin.LeftBottom: return RotateBitmap(src, 270);
                default: return src;
            }
        }
        private static SKBitmap RotateBitmap(SKBitmap src, float degrees)
        {
            int w = (degrees % 180 == 0) ? src.Width : src.Height;
            int h = (degrees % 180 == 0) ? src.Height : src.Width;
            SKBitmap dest = new(new SKImageInfo(w, h, src.ColorType, src.AlphaType));
            using SKCanvas canvas = new(dest);
            canvas.Clear(SKColors.Transparent);
            canvas.Translate(w / 2f, h / 2f);
            canvas.RotateDegrees(degrees);
            canvas.Translate(-src.Width / 2f, -src.Height / 2f);
            canvas.DrawBitmap(src, 0, 0);
            canvas.Flush();
            return dest;
        }
        private static SKBitmap FlipBitmap(SKBitmap src, bool horizontal)
        {
            SKBitmap dest = new(new SKImageInfo(src.Width, src.Height, src.ColorType, src.AlphaType));
            using SKCanvas canvas = new(dest);
            canvas.Clear(SKColors.Transparent);
            
            if (horizontal) { canvas.Translate(src.Width, 0); canvas.Scale(-1, 1); }
            else { canvas.Translate(0, src.Height); canvas.Scale(1, -1); }
            
            canvas.DrawBitmap(src, 0, 0);
            canvas.Flush();
            return dest;
        }

        /*
        ///<summary>
        /// Requests storage permissions and saves a file if permissions are granted.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>        
        //[SupportedOSPlatform("android")]
        //[SupportedOSPlatform("ios14.0")]
        //[SupportedOSPlatform("maccatalyst14.0")]
        ////[SupportedOSPlatform("tizen")]
        //[SupportedOSPlatform("windows")]
        public static async Task RequestStoragePermissionsAndSaveFile(CancellationToken cancellationToken)
        {
            var readPermissionStatus = await Permissions.RequestAsync<Permissions.StorageRead>();
            var writePermissionStatus = await Permissions.RequestAsync<Permissions.StorageWrite>();

            if (readPermissionStatus != PermissionStatus.Granted ||
                writePermissionStatus != PermissionStatus.Granted)
            {
                await Toast
                    .Make("Storage permissions are required to save files.")
                    .Show(cancellationToken);

                return;
            }

            await SaveFile(cancellationToken);
        }*/

        /*
        /// <summary>
        /// Saves a file asynchronously.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        //[SupportedOSPlatform("android")]
        //[SupportedOSPlatform("ios14.0")]
        //[SupportedOSPlatform("maccatalyst14.0")]
        ////[SupportedOSPlatform("tizen")]
        //[SupportedOSPlatform("windows")]
        public static async Task SaveFile(CancellationToken cancellationToken)
        {
            using var stream = new MemoryStream(Encoding.Default.GetBytes("Hello from the Community Toolkit!"));
            var fileSaverResult = await FileSaver.Default.SaveAsync("test.txt", stream, cancellationToken);
            if (fileSaverResult.IsSuccessful)
            {
                await Toast.Make($"The file was saved successfully to location: {fileSaverResult.FilePath}").Show(cancellationToken);
            }
            else
            {
                await Toast.Make($"The file was not saved successfully with error: {fileSaverResult.Exception.Message}").Show(cancellationToken);
            }
        }*/
    }
}
