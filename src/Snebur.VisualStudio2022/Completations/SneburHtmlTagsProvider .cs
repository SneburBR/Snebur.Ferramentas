using Microsoft.VisualStudio.Utilities;
using Microsoft.WebTools.Languages.Html.Editor.Completion;
using Microsoft.WebTools.Languages.Html.Editor.Completion.Def;
using System.Collections.Generic;

namespace Snebur.VisualStudio.Completations;
[ContentType("Html")]
[Name("Snebur HTML Tags Provider")]
[HtmlCompletionProvider("Children", "*")]
public sealed class SneburHtmlTagsProvider : IHtmlCompletionListProvider
{
    public SneburHtmlTagsProvider()
    {
        LogVSUtil.Alerta("Inicializando Snebur  IHtmlCompletionListProvider sem injeção de dependencia");
    }

    public IList<HtmlCompletion> GetEntries(HtmlCompletionContext context)
    {
        var tags = HtmlIntelliSenseUtils.GetSneburTagElements();
        var entries = new List<HtmlCompletion>();
        var icon = ImageMonikerUtils.TagIconImage;
        foreach (var tag in tags)
        {
            entries.Add(new HtmlCompletion(
                 displayText: tag.TagName,
                 insertionText: tag.TagName,
                 description: "Controle Snebur",
                 iconSource: icon,
                 iconAutomationText: "HTML Element Icon",
                 session: context.Session));
        }

        return entries;
    }
}
