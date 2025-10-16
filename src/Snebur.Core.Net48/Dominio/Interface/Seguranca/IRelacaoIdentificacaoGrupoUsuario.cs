using Snebur.Dominio.Atributos;

namespace Snebur.Dominio
{
    [IgnorarInterfaceTS]
    public interface IRelacaoIdentificacaoGrupoUsuario : IEntidadeSeguranca
    {
        [RelacaoPai]
        IIdentificacao Identificacao { get; set; }

        [RelacaoPai]
        IGrupoUsuario GrupoUsuario { get; set; }
    }
}
