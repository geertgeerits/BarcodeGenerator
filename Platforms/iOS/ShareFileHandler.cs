using Foundation;
using UIKit;
using CoreGraphics;
namespace BarcodeGenerator.Platforms.iOS;
public static class ShareFileHandler
{
    public static void ShareFile(string filePath, string mimeType, string title)
    {
        var url = NSUrl.FromFilename(filePath);
        if (url == null) return;
        var doc = UIDocumentInteractionController.FromUrl(url);
        doc.Name = title ?? System.IO.Path.GetFileName(filePath);
        var vc = GetTopViewController();
        doc.PresentOptionsMenu(new CGRect(0, 0, vc.View.Frame.Width, vc.View.Frame.Height), vc.View, true);
    }

    static UIViewController GetTopViewController()
    {
        var window = UIApplication.SharedApplication.KeyWindow ?? UIApplication.SharedApplication.Windows.FirstOrDefault(w => w.IsKeyWindow);
        var vc = window.RootViewController;
        while (vc.PresentedViewController != null) vc = vc.PresentedViewController;
        return vc;
    }
}