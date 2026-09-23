using Microsoft.Maui.Controls;
using Microsoft.Maui.Platform;
using System;
#if IOS
using UIKit;
using Foundation;
#endif
#if ANDROID
using Android.Views;
#endif

namespace BarcodeGenerator;

public partial class KeyboardAwareBehavior : Behavior<Layout>
{
    Layout? _layout;

#if ANDROID
    ViewTreeObserver.IOnGlobalLayoutListener _listener;
    Android.Views.View _rootView;
    int _originalPadding;
#endif

#if IOS
    NSObject _showObserver;
    NSObject _hideObserver;
    nfloat _originalPadding;
#endif

    protected override void OnAttachedTo(Layout bindable)
    {
        base.OnAttachedTo(bindable);
        _layout = bindable;

#if ANDROID
        _originalPadding = (int)_layout.Padding.Bottom;

        var activity = Platform.CurrentActivity;
        _rootView = activity?.Window?.DecorView?.RootView;

        if (_rootView != null)
        {
            _listener = new GlobalLayoutListener(_rootView, OnKeyboardHeightChanged);
            _rootView.ViewTreeObserver.AddOnGlobalLayoutListener(_listener);
        }
#endif

#if IOS
        _originalPadding = (nfloat)_layout.Padding.Bottom;

        _showObserver = NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillShowNotification, OnKeyboardShow);
        _hideObserver = NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillHideNotification, OnKeyboardHide);
#endif
    }

    protected override void OnDetachingFrom(Layout bindable)
    {
        base.OnDetachingFrom(bindable);

#if ANDROID
        if (_rootView != null && _listener != null)
        {
            _rootView.ViewTreeObserver.RemoveOnGlobalLayoutListener(_listener);
        }
#endif

#if IOS
        _showObserver?.Dispose();
        _hideObserver?.Dispose();
#endif
    }

#if ANDROID
    void OnKeyboardHeightChanged(int keyboardHeight)
    {
        if (keyboardHeight > 0)
        {
            _layout.Padding = new Thickness(
                _layout.Padding.Left,
                _layout.Padding.Top,
                _layout.Padding.Right,
                keyboardHeight);
        }
        else
        {
            _layout.Padding = new Thickness(
                _layout.Padding.Left,
                _layout.Padding.Top,
                _layout.Padding.Right,
                _originalPadding);
        }
    }

    class GlobalLayoutListener : Java.Lang.Object, ViewTreeObserver.IOnGlobalLayoutListener
    {
        readonly Android.Views.View _rootView;
        readonly Action<int> _callback;

        public GlobalLayoutListener(Android.Views.View rootView, Action<int> callback)
        {
            _rootView = rootView;
            _callback = callback;
        }

        public void OnGlobalLayout()
        {
            var rect = new Android.Graphics.Rect();
            _rootView.GetWindowVisibleDisplayFrame(rect);

            int screenHeight = _rootView.RootView.Height;
            int keypadHeight = screenHeight - rect.Bottom;

            _callback(keypadHeight);
        }
    }
#endif

#if IOS
    void OnKeyboardShow(NSNotification notification)
    {
        var frame = UIKeyboard.FrameEndFromNotification(notification);
        var height = frame.Height;

        _layout.Padding = new Thickness(
            _layout.Padding.Left,
            _layout.Padding.Top,
            _layout.Padding.Right,
            height);
    }

    void OnKeyboardHide(NSNotification notification)
    {
        _layout.Padding = new Thickness(
            _layout.Padding.Left,
            _layout.Padding.Top,
            _layout.Padding.Right,
            _originalPadding);
    }
#endif
}
