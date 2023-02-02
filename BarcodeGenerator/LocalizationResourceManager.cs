using BarcodeGenerator.Resources.Languages;
using System.ComponentModel;
using System.Globalization;

namespace BarcodeGenerator {
    public class LocalizationResourceManager : INotifyPropertyChanged {
        private LocalizationResourceManager() {
            CodeLang.Culture = CultureInfo.CurrentCulture;
        }

        public static LocalizationResourceManager Instance { get; } = new();

        public object this[string resourceKey]
            => CodeLang.ResourceManager.GetObject(resourceKey, CodeLang.Culture) ?? Array.Empty<byte>();

        public event PropertyChangedEventHandler PropertyChanged;

        public void SetCulture(CultureInfo culture) {
            CodeLang.Culture = culture;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
        }
    }
}
