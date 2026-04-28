namespace BarcodeGenerator
{
    internal class ClassGeolocation
    {
        private CancellationTokenSource? _cancelTokenSource;
        private bool _isCheckingLocation;

        /// <summary>
        /// Checking for a cached location
        /// </summary>
        /// <returns></returns>
        public static async Task<Location?> GetCachedLocation()
        {
            try
            {
                Location? location = await Microsoft.Maui.Devices.Sensors.Geolocation.Default.GetLastKnownLocationAsync();
                if (location != null)
                {
                    return location;
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Handle not supported on device exception
            }
            catch (FeatureNotEnabledException fneEx)
            {
                // Handle not enabled on device exception
            }
            catch (PermissionException pEx)
            {
                // Handle permission exception
            }
            catch (Exception ex)
            {
                // Unable to get location
            }

            return null;
        }

        /// <summary>
        /// Get the current location
        /// </summary>
        /// <returns></returns>
        public async Task<Location?> GetCurrentLocation()
        {
            Location? location = null;

            try
            {
                _isCheckingLocation = true;

                GeolocationRequest request = new(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));

                _cancelTokenSource = new CancellationTokenSource();

                location = await Microsoft.Maui.Devices.Sensors.Geolocation.Default.GetLocationAsync(request, _cancelTokenSource.Token);

                if (location != null)
                    Console.WriteLine($"Latitude: {location.Latitude}, Longitude: {location.Longitude}, Altitude: {location.Altitude}");
            }
            catch (Exception ex)
            {
                // Unable to get location
            }
            finally
            {
                _isCheckingLocation = false;
            }

            return location;
        }

        public void CancelRequest()
        {
            if (_isCheckingLocation && _cancelTokenSource != null && _cancelTokenSource.IsCancellationRequested == false)
                _cancelTokenSource.Cancel();
        }

        ///// <summary>
        ///// Listen for location changes
        ///// </summary>
        //async void OnStartListening()
        //{
        //    try
        //    {
        //        Microsoft.Maui.Devices.Sensors.Geolocation.LocationChanged += Geolocation_LocationChanged;
        //        // Using GeolocationAccuracy.Medium as a balance between accuracy and power consumption.
        //        // Developers can adjust this value to High or Low based on their specific requirements.
        //        var request = new Microsoft.Maui.Devices.Sensors.GeolocationListeningRequest(Microsoft.Maui.Devices.Sensors.GeolocationAccuracy.Medium);
        //        var success = await Microsoft.Maui.Devices.Sensors.Geolocation.StartListeningForegroundAsync(request);
        //        string status = success
        //            ? "Started listening for foreground location updates"
        //            : "Couldn't start listening";
        //    }
        //    catch (Exception ex)
        //    {
        //        // Unable to start listening for location changes
        //    }
        //}

        ///// <summary>
        /////Handles the event that is raised when the device's location changes.
        ///// </summary>
        ///// <param name="sender">The source of the event, typically the geolocation service or provider.</param>
        ///// <param name="e">An object containing the event data, including the updated location information.</param>
        //void Geolocation_LocationChanged(object sender, GeolocationLocationChangedEventArgs e)
        //{
        //    // Process e.Location to get the new location
        //}

        ///// <summary>
        ///// Stops listening for foreground location updates by unsubscribing from location change events and halting
        ///// location tracking.top listening for location changes
        ///// </summary>
        ///// <remarks>Call this method to stop receiving location updates when they are no longer needed.
        ///// This helps conserve device resources and battery life.</remarks>
        //void OnStopListening()
        //{
        //    try
        //    {
        //        Microsoft.Maui.Devices.Sensors.Geolocation.LocationChanged -= Geolocation_LocationChanged;
        //        Microsoft.Maui.Devices.Sensors.Geolocation.StopListeningForeground();
        //        string status = "Stopped listening for foreground location updates";
        //    }
        //    catch (Exception ex)
        //    {
        //        // Unable to stop listening for location changes
        //    }
        //}
    }
}
