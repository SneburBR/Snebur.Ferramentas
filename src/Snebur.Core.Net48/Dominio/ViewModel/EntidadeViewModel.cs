using Snebur.Dominio.Atributos;

namespace Snebur.Dominio
{
    [IgnorarTSReflexao]
    [IgnorarClasseTS]
    public class EntidadeViewModel : BaseViewModel
    {
        public Entidade Entidade { get; set; }
    }
}
