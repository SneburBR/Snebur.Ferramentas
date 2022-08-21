//using System;
//using System.Collections.Generic;

//using Microsoft.VisualStudio.Utilities;
//using Microsoft.Web.Editor;
//using Microsoft.Html.Editor.Completion.Def;
//using Microsoft.Web.Core.ContentTypes;

//namespace Snebur.VisualStudio.HtmlEditor
//{
//    [HtmlCompletionProvider(Microsoft.Html.Editor.Completion.Def.CompletionTypes.Values, "meta", "content")]
//    [ContentType(HtmlContentTypeDefinition.HtmlContentType)]
//    public class AppleMetaCompletion : StaticListCompletion
//    {
//        protected override string KeyProperty { get { return "name"; } }
//        public AppleMetaCompletion()
//            : base(new Dictionary<string, IEnumerable<string>>(StringComparer.OrdinalIgnoreCase)
//            {
//                { "Snebur-mobile-web-app-capable",           Values("yes", "no") },
//                { "Snebur-detection",                        Values("telephone=yes", "telephone=no") },
//                { "Snebur-mobile-web-app-status-bar-style",  Values("default", "black", "black-translucent") }
//            })
//        { }
//    }
//}
