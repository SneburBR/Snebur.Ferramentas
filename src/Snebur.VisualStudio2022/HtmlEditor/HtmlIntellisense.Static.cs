using System.Threading;
using System.Threading.Tasks;

namespace Snebur.VisualStudio
{
    public partial class HtmlIntelliSense
    {
        public static bool Inicializado = false;
        public static Task InicializarAsync()
        {
            return Task.Factory.StartNew(Inicializar,
                                        CancellationToken.None,
                                        TaskCreationOptions.None,
                                        TaskScheduler.Default);
        }
        private static void Inicializar()
        {
            if (!HtmlIntelliSense.Inicializado)
            {
                HtmlIntelliSense.Inicializado = true;
                AjudanteAssembly.Inicializar();
                using (var html = new HtmlIntelliSense())
                {
                    html.Atualizar();
                }
            }
        }

       
    }
}
