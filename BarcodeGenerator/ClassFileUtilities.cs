using SkiaSharp;

namespace BarcodeGenerator
{
    internal class ClassFileUtilities
    {
        /// <summary>
        /// Opens a media picker dialog that allows the user to select one image file
        /// </summary>
        /// <returns>The selected file result, or null if no file was selected.</returns>
        public static async Task<FileResult?> PickImage()
        {
            try
            {
                // Let user pick a photo. We take the first one
                FileResult? selected;

                bool bRotateImage;
#if WINDOWS
                bRotateImage = false;
#else
                bRotateImage = true;
#endif
                // Open the media picker to select a photo
                List<FileResult> photos = await MediaPicker.Default.PickPhotosAsync(new MediaPickerOptions
                {
                    SelectionLimit = 1,             // Default is 1; set to 0 for no limit
                    RotateImage = bRotateImage,
                    PreserveMetaData = true,
                    CompressionQuality = 100
                });

                selected = photos?.FirstOrDefault();

                // Correct the orientation of the selected image stream based on EXIF data
                // !!!BUG!!! in iOS, the orientation of the bounding box of the selected images may be incorrect with jpeg, heic and heif files.
                // However, the 'GetSelectedImageStreamAsync()' method does not correct the orientation of the bounding box.
                // Setting the 'RotateImage = true' in the MediaPickerOptions did fix the bounding box orientation issue on iOS.
                //selected = (await GetSelectedImageStreamAsync(photos)).selected;

                // Get the file name
                string? fileName = selected?.FileName;

                Debug.WriteLine($"ClassFileOperations.PickImage: selected file name: {fileName ?? "<none>"}");

                // Validate the selected file
                if (!string.IsNullOrEmpty(fileName))
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
                                // Ignore
                            }

                            DeleteFileInCache(selected.FullPath);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"ClassFileOperations.PickImage: cleanup error: {ex.Message}");
                    }

                    return null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ClassFileOperations.PickImage: File picking error: {ex.Message}");
                return null;
            }
        }

        /* Alternatives for 'PickImage()' method using custom file type filters.
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
                Debug.WriteLine($"ClassFileOperations.PickImage: selected file extension: {ext}");

                if (ext is ".png" or ".jpg" or ".jpeg" or ".bmp")
                {
                    // OK
                    return file;
                }
            }
        */

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
                Files = [new(ClassBarcodes.cFileBarcodePng), new(ClassBarcodes.cFileBarcodeSvg)]
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
                    if (filePath.StartsWith(FileSystem.Current.CacheDirectory, StringComparison.OrdinalIgnoreCase) && File.Exists(filePath))
                    {
                        File.Delete(filePath);
                        Debug.WriteLine($"ClassFileOperations.DeleteFileInCache: Deleted existing cache file at: {filePath}");
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
        public static async Task<(Stream? stream, FileResult? selected)> GetSelectedImageStreamAsync(IEnumerable<FileResult>? photos)
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
                    SKBitmap oriented = ClassImageUtilities.FixOrientation(bitmap, codec.EncodedOrigin);
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
                    if (bitmap == null)
                    {
                        return null;
                    }
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
    }
}
