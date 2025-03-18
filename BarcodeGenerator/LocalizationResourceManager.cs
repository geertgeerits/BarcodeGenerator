﻿using System.ComponentModel;

namespace BarcodeGenerator
{
    public sealed class LocalizationResourceManager : INotifyPropertyChanged
    {
        private LocalizationResourceManager() {
            CodeLang.Culture = CultureInfo.CurrentUICulture;
        }

        public static LocalizationResourceManager Instance { get; } = new();

        public object this[string resourceKey]
            => CodeLang.ResourceManager.GetObject(resourceKey, CodeLang.Culture) ?? Array.Empty<byte>();

        public event PropertyChangedEventHandler? PropertyChanged;

        public void SetCulture(CultureInfo culture) {
            CodeLang.Culture = culture;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
        }
    }
}
