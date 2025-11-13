//using Microsoft.VisualStudio.Language.Intellisense.AsyncCompletion;
//using Microsoft.VisualStudio.Text.Editor;
//using Microsoft.VisualStudio.Utilities;
//using System.ComponentModel.Composition;

//namespace Snebur.VisualStudio.Completations;

//[Export(typeof(IAsyncCompletionSourceProvider))]
//[ContentType("TypeScript")]
//[Name("Hello World TypeScript completion item source")]
//public class TypeScriptCompletation : IAsyncCompletionSourceProvider
//{
//    private readonly IAsyncCompletionItemManager _asyncCompletionItemManager;

//    //[ImportingConstructor]
//    //public TypeScriptCompletation(IAsyncCompletionItemManager asyncCompletionItemManager)
//    //{
//    //    LogVSUtil.Alerta("Inicializando TypeScriptCompletation sem injeção de dependencia");
//    //    this._asyncCompletionItemManager = asyncCompletionItemManager;
//    //}
     
//    public TypeScriptCompletation()
//    {
//        LogVSUtil.Alerta("Inicializando TypeScriotCompletation sem injeção de dependencia");

//    }
//    public IAsyncCompletionSource GetOrCreate(ITextView textView)
//    {
//        LogVSUtil.Alerta("TypeScriotCompletation GetOrCreate chamado");

//        if (!textView.Properties.TryGetProperty(
//            typeof(SneburCompletionSource),
//            out SneburCompletionSource completionSource))
//        {
//            completionSource = new SneburCompletionSource(this, this._asyncCompletionItemManager);
//            textView.Properties.AddProperty(typeof(SneburCompletionSource), completionSource);
//        }
//        return completionSource;
//    }
//}