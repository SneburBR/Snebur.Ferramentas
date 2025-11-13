using Microsoft.VisualStudio.Imaging.Interop;
using Microsoft.VisualStudio.Shell.Interop;
using System.Runtime.InteropServices;
using System.Windows.Media;

namespace Snebur.VisualStudio;

public static class ImageHelper
{
    public static ImageSource GetImageSource(ImageMoniker moniker, int size = 16)
    {
        ThreadHelper.ThrowIfNotOnUIThread();

        var imageService = ServiceProvider.GlobalProvider.GetService(typeof(SVsImageService)) as IVsImageService2;
        if (imageService is null) return null;

        var attrs = new ImageAttributes
        {
            // this field must be set, or you get "StructSize is invalid"
            StructSize = (int)Marshal.SizeOf(typeof(ImageAttributes)),
            Flags = (uint)_ImageAttributesFlags.IAF_RequiredFlags,
            ImageType = (uint)_UIImageType.IT_Bitmap,
            Format = (uint)_UIDataFormat.DF_WPF,
            LogicalHeight = size,
            LogicalWidth = size,
            // optional, but good defaults
            Dpi = 96,
            Background = 0x00000000,  // ARGB, transparent
        };

        object obj = imageService.GetImage(moniker, attrs);

        // some VS builds return IVsUIObject, unwrap it to get the WPF ImageSource
        if (obj is IVsUIObject uiObj)
        {
            uiObj.get_Data(out obj);
        }

        return obj as ImageSource;
    }
}