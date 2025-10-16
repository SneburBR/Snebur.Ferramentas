using System;

namespace Snebur.Dominio
{
    public interface ISessaoUsuario : IEntidade, IInformacaoSessao, IIdentificadorSessaoUsuario, IIdentificadorProprietario
    {
        IUsuario Usuario { get; set; }

        long Usuario_Id { get; set; }

        string IP { get; set; }

        IIPInformacao IPInformacao { get; set; }

        EnumStatusSessaoUsuario Status { get; set; }

        EnumStatusServicoArquivo StatusServicoArquivo { get; set; }

        string MotivoBloqueio { get; set; }

        DateTime? DataHoraExpiracaoBloqueio { get; set; }

        DateTime? DataHoraInicio { get; set; }

        DateTime? DataHoraUltimoAcesso { get; set; }

        DateTime? DataHoraFim { get; set; }

        TimeSpan? Duracao { get; set; }
    }
}