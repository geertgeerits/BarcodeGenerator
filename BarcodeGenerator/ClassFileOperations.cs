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
                var photos = await MediaPicker.Default.PickPhotosAsync();
                var selected = photos?.FirstOrDefault();

                // Get the file name with extension
                // FileResult.FileName provides the name including the extension
                string? fileNameWithExt = selected?.FileName;

                Debug.WriteLine($"ClassFileOperations.PickImage: selected file name: {fileNameWithExt ?? "<none>"}");

                // Validate the selected file
                if (!string.IsNullOrEmpty(fileNameWithExt))
                {
                    if (fileNameWithExt.EndsWith("png", StringComparison.OrdinalIgnoreCase) ||
                        fileNameWithExt.EndsWith("jpg", StringComparison.OrdinalIgnoreCase) ||
                        fileNameWithExt.EndsWith("jpeg", StringComparison.OrdinalIgnoreCase))
                    {
                        return selected;
                    }
                    else
                    {
                        await Application.Current!.Windows[0].Page!.DisplayAlertAsync(CodeLang.ErrorTitle_Text, $"{CodeLang.QRCodeImageTypeError_Text}", CodeLang.ButtonClose_Text);
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
                Files = [new(ClassBarcodes.cFileBarcodePng), new ShareFile(ClassBarcodes.cFileBarcodeSvg)]
            });
            
            return true;
        }

        /// <summary>
        /// Deletes the specified file if it exists
        /// </summary>
        /// <param name="filePath">The path of the file to delete.</param>
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

        /////<summary>
        ///// Requests storage permissions and saves a file if permissions are granted.
        ///// </summary>
        ///// <param name="cancellationToken">A cancellation token to cancel the operation.</param>        
        //[SupportedOSPlatform("android")]
        //[SupportedOSPlatform("ios14.0")]
        //[SupportedOSPlatform("maccatalyst14.0")]
        ////[SupportedOSPlatform("tizen")]
        //[SupportedOSPlatform("windows")]
        //public static async Task RequestStoragePermissionsAndSaveFile(CancellationToken cancellationToken)
        //{
        //    var readPermissionStatus = await Permissions.RequestAsync<Permissions.StorageRead>();
        //    var writePermissionStatus = await Permissions.RequestAsync<Permissions.StorageWrite>();

        //    if (readPermissionStatus != PermissionStatus.Granted ||
        //        writePermissionStatus != PermissionStatus.Granted)
        //    {
        //        await Toast
        //            .Make("Storage permissions are required to save files.")
        //            .Show(cancellationToken);

        //        return;
        //    }

        //    await SaveFile(cancellationToken);
        //}

        ///// <summary>
        ///// Saves a file asynchronously.
        ///// </summary>
        ///// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
        ///// <returns>A task representing the asynchronous operation.</returns>
        //[SupportedOSPlatform("android")]
        //[SupportedOSPlatform("ios14.0")]
        //[SupportedOSPlatform("maccatalyst14.0")]
        ////[SupportedOSPlatform("tizen")]
        //[SupportedOSPlatform("windows")]
        //public static async Task SaveFile(CancellationToken cancellationToken)
        //{
        //    using var stream = new MemoryStream(Encoding.Default.GetBytes("Hello from the Community Toolkit!"));
        //    var fileSaverResult = await FileSaver.Default.SaveAsync("test.txt", stream, cancellationToken);
        //    if (fileSaverResult.IsSuccessful)
        //    {
        //        await Toast.Make($"The file was saved successfully to location: {fileSaverResult.FilePath}").Show(cancellationToken);
        //    }
        //    else
        //    {
        //        await Toast.Make($"The file was not saved successfully with error: {fileSaverResult.Exception.Message}").Show(cancellationToken);
        //    }
        //}
    }
}
