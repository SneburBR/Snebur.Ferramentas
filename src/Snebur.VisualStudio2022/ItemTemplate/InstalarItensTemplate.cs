using System.Threading.Tasks;
using System.Threading;

namespace Snebur.VisualStudio
{
    public partial class InstalarItensTemplate
    {
        public static Task InstalarAsync()
        {

            return Task.Factory.StartNew(InstalarInterno,
                                        CancellationToken.None,
                                        TaskCreationOptions.None,
                                        TaskScheduler.Default);
        }

        private static void InstalarInterno()
        {
            var instalador = new InstalarItensTemplateInterno();
            instalador.Instalar();

        }
    }
    
}
