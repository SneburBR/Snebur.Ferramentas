using Snebur.Dominio.Atributos;
using System.Collections.Generic;

namespace Snebur.Dominio
{
    [IgnorarInterfaceTS]
    public interface IGrupoUsuario : IIdentificacao
    {
        [ValidacaoRequerido]
        [ValidacaoTextoTamanho(100)]
        [ValidacaoUnico]
        string Nome { get; set; }

        IEnumerable<IIdentificacao> Membros { get; }
    }
}
