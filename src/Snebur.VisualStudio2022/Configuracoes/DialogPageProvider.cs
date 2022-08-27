using System.Runtime.InteropServices;

namespace Snebur.VisualStudio
{
    /// <summary>
    /// A provider for custom <see cref="DialogPage" /> implementations.
    /// </summary>
    internal class DialogPageProvider
    {
        [ComVisible(true)]
        public class Geral : BaseOptionPage<ConfiguracaoGeral> { }
        //public class Foo : BaseOptionPage<GeneralOptions> { }
    }
}
