using Microsoft.VisualStudio.Imaging.Interop;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Utilities;
using Microsoft.WebTools.Languages.Html.Editor.Completion;
using Microsoft.WebTools.Languages.Html.Editor.Completion.Def;
using Microsoft.WebTools.Languages.Html.Editor.Completion.Html;
using Microsoft.WebTools.Languages.Shared.Editor.Completion;
using System;
using System.Linq;
using System.Windows.Media;

namespace Snebur.VisualStudio.Completations;
public class AttributeHtmlCompletion : HtmlCompletion
{
    private static readonly char[] _commitChars = new[] { ' ', '=', '>' };
    private static readonly string[] _commitCharsAsString = [.. _commitChars.Select(c => c.ToString())];
    public SneburHtmlAttribute SneburHtmlAttribute { get; }
    public override bool RetriggerIntellisense
        => true;

    public override ImageMoniker IconMoniker
        => SneburHtmlAttribute.ImageMoniker;
    public AttributeHtmlCompletion(
        SneburHtmlAttribute sneburAttribute,
        ICompletionSession session)
        : base(displayText: sneburAttribute.Name,
               insertionText: $"{sneburAttribute.Name}=\"\"",
               description: "",
               iconSource: null,
               iconAutomationText: HtmlIconAutomationText.AttributeIconText,
               session: session, 
               charactersBeforeCaret:2, 
               retriggerIntellisense: true)
    {
        this.SneburHtmlAttribute = sneburAttribute;
        //this.CaretOffsetUponInsertion = sneburAttribute.Name.Length + 2;
        this.CaseSensitive = false;
        this.SortingPriority = 1000;
        this.InsertCommitCharactersInternalProperty();
    }

    public override void Commit()
    {
        base.Commit();

        //_ = ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
        //{
        //    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
        //    await Task.Yield();  // let the HTML tree update
        //    this.Session?.Recalculate();
        //});
        //_ = ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
        //{
        //    await Task.Delay(50);
        //    this.Session?.Recalculate();
        //});
    }
    public override bool IsCommitChar(char typedCharacter)
    {
        if (_commitChars.Contains(typedCharacter))
        {
            return true;
        }
        return base.IsCommitChar(typedCharacter);
    }

    public override ITrackingSpan ApplicableTo
    {
        get => base.ApplicableTo;
        set => base.ApplicableTo = value;
    }

    protected override int InternalCompareTo(CompletionEntry other)
    {
        return base.InternalCompareTo(other);
    }

    public override int SelectionMatchPriority
    {
        get => base.SelectionMatchPriority;
        set => base.SelectionMatchPriority = value;
    }

    public override PropertyCollection Properties
        => base.Properties;
     
    private void InsertCommitCharactersInternalProperty()
    {
        var flags = System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance;
        var pi = typeof(HtmlCompletion).GetProperty("CommitCharacters", flags);
        if (pi is not null)
        {
            pi.SetValue(this, _commitCharsAsString);
        }
    }
}

public class HtmlAttributeValueCompletion : HtmlCompletion
{
    public QuoteSkipType QuoteSkip { get; private set; }

    public HtmlAttributeValueCompletion(
        string displayText, 
        string insertionText, 
        string description,
        ImageSource? iconSource,
        QuoteSkipType quoteSkip, 
        ICompletionSession session)
        : this(displayText, insertionText, description, iconSource, quoteSkip, retriggerIntellisense: false, session)
    {

    }

    public HtmlAttributeValueCompletion(
        string displayText,
        string insertionText,
        string description, 
        ImageSource? iconSource, 
        QuoteSkipType quoteSkip,
        bool retriggerIntellisense, 
        ICompletionSession session)
        : base(displayText, 
            insertionText, 
            description, 
            iconSource, 
            HtmlIconAutomationText.AttributeIconText,
            0, 
            retriggerIntellisense,
            session)
    {
        QuoteSkip = quoteSkip;
    }

    public static QuoteSkipType GetQuoteSkipType(
        HtmlCompletionContext context,
        bool allowMultipleValues = false)
    {
        if (context.Attribute != null &&
            context.Attribute.HasValue() &&
            context.Attribute.ValueToken.CloseQuote != 0)
        {
            if (context.Attribute.ValueToken.CloseQuote != '\'')
            {
                if (!allowMultipleValues)
                {
                    return QuoteSkipType.AlwaysDouble;
                }
                return QuoteSkipType.MultiValueDouble;
            }

            if (!allowMultipleValues)
            {
                return QuoteSkipType.AlwaysSingle;
            }

            return QuoteSkipType.MultiValueSingle;
        }

        return QuoteSkipType.None;
    }
}
public enum QuoteSkipType
{
    None,
    AlwaysSingle,
    AlwaysDouble,
    MultiValueSingle,
    MultiValueDouble
}

