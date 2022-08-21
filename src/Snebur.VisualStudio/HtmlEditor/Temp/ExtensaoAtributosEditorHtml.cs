//using Microsoft.Html.Core;
//using Microsoft.Html.Core.Tree.Nodes;
//using Microsoft.Html.Editor.Completion;
//using Microsoft.Html.Editor.Completion.Def;
//using Microsoft.VisualStudio.Utilities;
//using Microsoft.Web.Core.ContentTypes;
//using Microsoft.Web.Editor;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Windows.Media.Imaging;

//namespace Snebur.VisualStudio.HtmlEditor
//{
//    [HtmlCompletionProvider(CompletionTypes.Values, "*", "*")]
//    [ContentType(HtmlContentTypeDefinition.HtmlContentType)]
//    public class ExtensaoAtributosEditorHtml : IHtmlCompletionListProvider, IHtmlTreeVisitor
//    {

//        private static BitmapFrame Icone = BitmapFrame.Create(new Uri("pack://application:,,,/Snebur.VisualStudio;component/Recursos/icones/Snebur.png", UriKind.RelativeOrAbsolute));

//        private static List<string> Atributos = new List<string>()
//        {
//            "sn-bind",
//            "sn-bind-textp",
//            "sn-click",
//            "sn-navegar",
//            "ng-controle",
//            "ng-itemcontrole",
//        };


//        public string CompletionType
//        {
//            get { return CompletionTypes.Values; }
//        }

//        public IList<HtmlCompletion> GetEntries(HtmlCompletionContext context)
//        {
//            HashSet<bool> isAngular = new HashSet<bool>();
//            context.Document.HtmlEditorTree.RootNode.Accept(this, isAngular);

//            if (isAngular.Count == 0)
//                return new List<HtmlCompletion>();

//            return CreateCompletionItems(context).ToArray();
//        }

//        private static IEnumerable<HtmlCompletion> CreateCompletionItems(HtmlCompletionContext context)
//        {
//            foreach (string item in Atributos)
//            {
//                yield return new SimpleHtmlCompletion(item, context.Session) { IconSource = Icone };
//            }
//        }

//        public bool Visit(ElementNode element, object parameter)
//        {
//            // Search in class names to in order to make Intellisense show after typing "ng-" as a class value
//            if (element.Attributes.Any(a => (a.Name.StartsWith("sn-", StringComparison.Ordinal)
//                                         || a.Name.StartsWith("data-sn-", StringComparison.Ordinal)
//                                         || (a.Name == "class" && a.Value != null && a.Value.StartsWith("nn-", StringComparison.Ordinal)))))
//            {
//                var list = (HashSet<bool>)parameter;
//                list.Add(true);
//                return true;
//            }

//            return true;
//        }

//    }


//}
