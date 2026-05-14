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
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

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
    }
}
