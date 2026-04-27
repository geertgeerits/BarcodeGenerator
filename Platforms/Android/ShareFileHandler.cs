using Android.Content;
using AndroidX.Core.Content;
namespace BarcodeGenerator.Platforms.Android;
public static class ShareFileHandler
{
    public static void ShareFile(string filePath, string mimeType, string title)
    {
        var file = new Java.IO.File(filePath);
        if (!file.Exists()) return;
        var context = Android.App.Application.Context!;
        var authority = context.PackageName + ".fileprovider";
        var uri = FileProvider.GetUriForFile(context, authority, file);
        var intent = new Intent(Intent.ActionSend);
        intent.SetType(mimeType ?? "*/*");
        intent.PutExtra(Intent.ExtraStream, uri);
        intent.AddFlags(ActivityFlags.GrantReadUriPermission | ActivityFlags.NewTask);
        var chooser = Intent.CreateChooser(intent, title);
        chooser.SetFlags(ActivityFlags.NewTask);
        context.StartActivity(chooser);
    }
}