using Microsoft.VisualStudio.Utilities;
using Microsoft.WebTools.Languages.Html.Editor.Completion;
using Microsoft.WebTools.Languages.Html.Editor.Completion.Def;
using Microsoft.WebTools.Languages.Html.Editor.Completion.Html;
using System.Collections.Generic;

namespace Snebur.VisualStudio.Completations;

[ContentType("Html")]
[Name("Snebur HTML Atributes Provider")]
[HtmlCompletionProvider("Attributes", "*")]
public sealed class SneburHtmlAttributesProvider : IHtmlCompletionListProvider
{
    //private static IList<HtmlCompletion> _entriesCache = null;

    private readonly Dictionary<string, SneburHtmlAttribute> _attrDictionary;

    public SneburHtmlAttributesProvider()
    {
        LogVSUtil.Alerta("Inicializando Snebur  SneburHtmlAttributesProvider sem injeção de dependencia");
        this._attrDictionary = HtmlIntelliSenseUtils.GetSneburAttributesDictionary();
    }

    public IList<HtmlCompletion> GetEntries(HtmlCompletionContext context)
    {
        return GetEntriesInternal(context);
    }

    private IList<HtmlCompletion> GetEntriesInternal(HtmlCompletionContext context)
    {
        //var attrName = context.Attribute?.Name;
        //if (attrName is not null && attrName.Length > 
        //    (attrName.StartsWith("sn-") || (attrName.StartsWith("ap-"))))
        //{
        //    return GetAtributeValueEntries(attrName);
        //}

        var attrName = context.Attribute?.Name;
        if (attrName is not null && this._attrDictionary.ContainsKey(attrName))
        {
            return AttributeValuesHelper.GetAtributeValueEntries(attrName);
        }

        var attributes = HtmlIntelliSenseUtils.GetSneburAttributes();
        var entries = new List<HtmlCompletion>();

        foreach (var sneburAttribute in attributes)
        {
            var icon = sneburAttribute.GetAttributeIcon();
            var insertionText = $"{sneburAttribute.Name}=\"\"";
            entries.Add(new AttributeHtmlCompletion(
                 sneburAttribute: sneburAttribute,
                 session: context.Session));
        }

        return entries;
    }


}