using System;
using System.Collections.Generic;
using System.Text;

namespace BarcodeGenerator
{
    internal class ClassDeviceInfo
    {
        /// <summary>
        /// Read the device information
        /// </summary>
        public static async Task ReadDeviceInfoAsync()
        {
            System.Text.StringBuilder sb = new();

            sb.AppendLine($"Model: {DeviceInfo.Current.Model}");
            sb.AppendLine($"Manufacturer: {DeviceInfo.Current.Manufacturer}");
            sb.AppendLine($"Name: {DeviceInfo.Current.Name}");
            sb.AppendLine($"OS Version: {DeviceInfo.Current.VersionString}");
            sb.AppendLine($"Idiom: {DeviceInfo.Current.Idiom}");
            sb.AppendLine($"Platform: {DeviceInfo.Current.Platform}");

            bool isVirtual = DeviceInfo.Current.DeviceType switch
            {
                DeviceType.Physical => false,
                DeviceType.Virtual => true,
                _ => false
            };

            sb.AppendLine($"Virtual device? {isVirtual}");

            await Application.Current!.Windows[0].Page!.DisplayAlertAsync("Device info", sb.ToString(), "OK");
        }

        /// <summary>
        /// Get the main display info
        /// </summary>
        public static void ReadDeviceDisplay()
        {
            System.Text.StringBuilder sb = new();

            sb.AppendLine($"Pixel width: {DeviceDisplay.Current.MainDisplayInfo.Width} / Pixel Height: {DeviceDisplay.Current.MainDisplayInfo.Height}");
            sb.AppendLine($"Density: {DeviceDisplay.Current.MainDisplayInfo.Density}");
            sb.AppendLine($"Orientation: {DeviceDisplay.Current.MainDisplayInfo.Orientation}");
            sb.AppendLine($"Rotation: {DeviceDisplay.Current.MainDisplayInfo.Rotation}");
            sb.AppendLine($"Refresh Rate: {DeviceDisplay.Current.MainDisplayInfo.RefreshRate}");

            //DisplayDetailsLabel.Text = sb.ToString();
            Debug.WriteLine($"ReadDeviceDisplay:\n{sb}");
        }
    }
}
