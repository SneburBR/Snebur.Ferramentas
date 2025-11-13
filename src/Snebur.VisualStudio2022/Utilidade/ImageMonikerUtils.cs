using Microsoft.VisualStudio.Imaging.Interop;
using System.Windows.Media;

namespace Snebur.VisualStudio;

//[Export(typeof(IHtmlCompletionListProvider))]
//[HtmlCompletionProvider("Tags", "*")]

public static class ImageMonikerUtils
{
    private static ImageSource _tagIconImage;
    private static ImageSource _attributeIconImage;
    private static ImageSource _eventIconImage;
    private static ImageSource _presentationIconImage;
    private static ImageSource _enumerationValueIconImage;


    public readonly static ImageMoniker TagIcon = new ImageMoniker
    {
        Guid = new Guid("ae27a6b0-e345-4288-96df-5eaf394ee369"),
        Id = 3335
    };

    public readonly static ImageMoniker AttributeIcon = new ImageMoniker
    {
        Guid = new Guid("ae27a6b0-e345-4288-96df-5eaf394ee369"),
        Id = 3335
    };
     
    public readonly static ImageMoniker EventIcon = new ImageMoniker
    {
        Guid = new Guid("ae27a6b0-e345-4288-96df-5eaf394ee369"),
        Id = 1142
    };

    public readonly static ImageMoniker PresentationIcon = new ImageMoniker
    {
        Guid = new Guid("ae27a6b0-e345-4288-96df-5eaf394ee369"),
        Id = 7
    };

    public readonly static ImageMoniker EnumerationValue = new ImageMoniker
    {
        Guid = new Guid("ae27a6b0-e345-4288-96df-5eaf394ee369"),
        Id = 1120
    };

    public static ImageSource TagIconImage
        => _tagIconImage ??= GetImageSource(TagIcon);

    public static ImageSource AttributeIconImage
        => _attributeIconImage ??= GetImageSource(AttributeIcon);

    public static ImageSource EventIconImage
        => _eventIconImage ??= GetImageSource(EventIcon);

    public static ImageSource PresentationIconImage
        => _presentationIconImage ??= GetImageSource(PresentationIcon);

    public static ImageSource EnumerationValueImage
        => _enumerationValueIconImage ??= GetImageSource(EnumerationValue);


    private static ImageSource GetImageSource(ImageMoniker icon)
    {
        var source = ImageHelper.GetImageSource(icon);
        source.Freeze();
        return source;
    }
}
