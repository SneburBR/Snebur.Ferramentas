using Microsoft.VisualStudio.Utilities;
using Microsoft.WebTools.Languages.Html.Editor.Completion;
using Microsoft.WebTools.Languages.Html.Editor.Completion.Def;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Remoting.Contexts;

namespace Snebur.VisualStudio.Completations;


public abstract class AbstractSneburHtmlValuesProvider : IHtmlCompletionListProvider
{
    private readonly Dictionary<string, SneburHtmlAttribute> _attrDictionary;
    public AbstractSneburHtmlValuesProvider()
    {
        Debugger.Break();
        this._attrDictionary = HtmlIntelliSenseUtils.GetSneburAttributesDictionary();
        LogVSUtil.Alerta($"Inicializando Snebur  {this.GetType().Name} sem injeção de dependencia");
    }

    public virtual IList<HtmlCompletion> GetEntries(HtmlCompletionContext context)
    {
        var attrName = context.Attribute?.Name;
        if (attrName is not null && this._attrDictionary.ContainsKey(attrName))
        {
            return AttributeValuesHelper.GetAtributeValueEntries(attrName);
        }
        return Array.Empty<HtmlCompletion>();
    }
     
}

[ContentType("Html")]
[Name("Snebur HTML Atributes Provider")]
[HtmlCompletionProvider("Values", "*")]
public sealed class SneburHtmlValuesProvider : AbstractSneburHtmlValuesProvider, IHtmlCompletionListProvider
{
    public override IList<HtmlCompletion> GetEntries(HtmlCompletionContext context)
    {
        Debugger.Break();
        return base.GetEntries(context);
    }
}

[ContentType("Html")]
[Name("Snebur HTML GroupValues Provider")]
[HtmlCompletionProvider("GroupValues", "*")]
public sealed class SneburHtmlGroupValuesProvider : AbstractSneburHtmlValuesProvider, IHtmlCompletionListProvider
{
    public override IList<HtmlCompletion> GetEntries(HtmlCompletionContext context)
    {
        Debugger.Break();
        return base.GetEntries(context);
    }
}
[ContentType("Html")]
[Name("Snebur HTML GroupValues Provider")]
[HtmlCompletionProvider("Group", "*")]
public sealed class SneburHtmlGroupProvider : AbstractSneburHtmlValuesProvider, IHtmlCompletionListProvider
{
    public override IList<HtmlCompletion> GetEntries(HtmlCompletionContext context)
    {
        Debugger.Break();
        return base.GetEntries(context);
    }
}

[ContentType("Html")]
[Name("Snebur HTML GroupValues Provider")]
[HtmlCompletionProvider("Entities", "*")]
public sealed class SneburHtmlEntitiesProvider : AbstractSneburHtmlValuesProvider, IHtmlCompletionListProvider
{
    public override IList<HtmlCompletion> GetEntries(HtmlCompletionContext context)
    {
        Debugger.Break();
        return base.GetEntries(context);
    }
}
[ContentType("Html")]
[Name("Snebur HTML GroupValues Provider")]
[HtmlCompletionProvider("Artifacts", "*")]
public sealed class SneburHtmlArtifactsProvider : AbstractSneburHtmlValuesProvider, IHtmlCompletionListProvider
{
    public override IList<HtmlCompletion> GetEntries(HtmlCompletionContext context)
    {
        Debugger.Break();
        return base.GetEntries(context);
    }
}

[ContentType("Html")]
[Name("Snebur HTML GroupValues Provider")]
[HtmlCompletionProvider("GroupAttributes", "*")]
public sealed class SneburHtmlGroupAttributesProvider : AbstractSneburHtmlValuesProvider, IHtmlCompletionListProvider
{
    public override IList<HtmlCompletion> GetEntries(HtmlCompletionContext context)
    {
        Debugger.Break();
        return base.GetEntries(context);
    }
}
