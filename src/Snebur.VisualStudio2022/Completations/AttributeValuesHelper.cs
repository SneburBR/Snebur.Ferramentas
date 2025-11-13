using Microsoft.WebTools.Languages.Html.Editor.Completion;
using System.Collections.Generic;

namespace Snebur.VisualStudio.Completations;

public static class AttributeValuesHelper
{
    public static IList<HtmlCompletion> GetAtributeValueEntries(string attrName)
    {
        var dictionary = HtmlIntelliSenseUtils.GetSneburAttributesDictionary();
        if (dictionary.ContainsKey(attrName))
        {
            var attribute = dictionary[attrName];
            var entries = new List<HtmlCompletion>();
            var icon = attribute.GetValueIconImageSource();
            foreach (var enumValue in attribute.EnumValues)
            {
                // Create HtmlCompletion for each enum value
                entries.Add(new HtmlCompletion(
                     displayText: enumValue,
                     insertionText: enumValue,
                     description: $"Value for {attribute.Name}",
                     iconSource: icon,
                     iconAutomationText: "HTML Attribute Value Icon",
                     session: null));
            }
            return entries;
        }
        return Array.Empty<HtmlCompletion>();
    }
}
