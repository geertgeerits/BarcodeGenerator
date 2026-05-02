#if IOS
using Microsoft.Maui.Handlers;
using UIKit;

namespace BarcodeGenerator.Platforms.iOS
{
    public static class NumericKeyboardHandler
    {
        // Attached property to mark entries that should use NumbersAndPunctuation
        public static readonly BindableProperty UseNumbersAndPunctuationProperty =
            BindableProperty.CreateAttached(
                "UseNumbersAndPunctuation",
                typeof(bool),
                typeof(NumericKeyboardHandler),
                false);

        public static bool GetUseNumbersAndPunctuation(BindableObject view) =>
            (bool)view.GetValue(UseNumbersAndPunctuationProperty);

        public static void SetUseNumbersAndPunctuation(BindableObject view, bool value) =>
            view.SetValue(UseNumbersAndPunctuationProperty, value);

        // Call this from your shared code to enable the keyboard for a specific Entry:
        // BarcodeGenerator.Platforms.iOS.NumericKeyboardHandler.ConfigureNumbersAndPunctuation(myEntry);
        public static void ConfigureNumbersAndPunctuation(Entry entry)
        {
            SetUseNumbersAndPunctuation(entry, true);

            EntryHandler.Mapper.AppendToMapping("NumbersAndPunctuation", (handler, view) =>
            {
                if (handler.PlatformView is not UITextField textField)
                {
                    return;
                }

                // Cast view to Entry to access BindableObject properties
                if (view is not Entry entryControl)
                {
                    return;
                }

                if (!GetUseNumbersAndPunctuation(entryControl))
                {
                    return;
                }

                textField.KeyboardType = UIKeyboardType.NumbersAndPunctuation;

                // If the field is currently first responder (keyboard shown), reload the input view to apply the change immediately
                if (textField.IsFirstResponder)
                {
                    textField.ReloadInputViews();
                }
            });
        }
    }
}
#endif
