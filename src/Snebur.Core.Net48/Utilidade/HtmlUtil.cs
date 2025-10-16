using System;

namespace Snebur.Utilidade
{
    public static class HtmlUtil
    {
        public static string RetornarTexto(string html)
        {
            return html.ReplaceRegex(@"<br\s*/?>", "\r\n")
                       .ReplaceRegex(@"<p\s*/?>", "\r\n")
                       .ReplaceRegex(@"<[^>]+>", string.Empty).
                        ReplaceRegex(@"&[A-Za-z][A-Za-z0-9]{1,4};", string.Empty);
        }

    }
}