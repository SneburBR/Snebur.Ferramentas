using Snebur.Dominio.Atributos;
using System;

namespace Snebur.Dominio
{
    public interface IUsuario : IIdentificacao, ICredencial
    {
        [Indexar]
        [ValidacaoRequerido]
        [ValidacaoTextoTamanho(100)]
        string Nome { get; set; }

        DateTime? DataHoraUltimoAcesso { get; set; }

        bool IsDesativado { get; set; }

        bool IsAlterarSenhaProximoAcesso { get; set; }

        EnumStatusUsuario Status { get; }

    }
}