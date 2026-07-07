#if IOS
using Foundation;
using UIKit;

namespace BarcodeGenerator.Platforms.iOS
{
    public class KeyboardEventArgs : EventArgs
    {
        public double Height { get; set; }
    }

    // Simple platform helper: listens to iOS keyboard notifications and forwards keyboard height via events
    public static class KeyboardHelper
    {
        static NSObject? _willShow;
        static NSObject? _willHide;
        static bool _started;

        public static event EventHandler<KeyboardEventArgs>? KeyboardWillShow;
        public static event EventHandler<KeyboardEventArgs>? KeyboardWillHide;

        public static void StartListening()
        {
            if (_started) return;
            _started = true;

            _willShow = NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillShowNotification, notification =>
            {
                if (notification?.UserInfo == null) return;
                var nsVal = notification.UserInfo[UIKeyboard.FrameEndUserInfoKey] as NSValue;
                if (nsVal == null) return;
                var frame = nsVal.CGRectValue;
                var height = frame.Height; // points
                KeyboardWillShow?.Invoke(null, new KeyboardEventArgs { Height = height });
            });

            _willHide = NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillHideNotification, notification =>
            {
                KeyboardWillHide?.Invoke(null, new KeyboardEventArgs { Height = 0 });
            });
        }

        public static void StopListening()
        {
            if (!_started) return;
            if (_willShow != null) NSNotificationCenter.DefaultCenter.RemoveObserver(_willShow);
            if (_willHide != null) NSNotificationCenter.DefaultCenter.RemoveObserver(_willHide);
            _willShow = _willHide = null;
            _started = false;
        }
    }
}
#endif