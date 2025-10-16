using System;

namespace Snebur.Dominio
{
    public interface ILogServicoArquivo : IEntidade
    {
        EnumStatusServicoArquivo StatusServicoArquivo { get; set; }

        Guid IndetificadorLog { get; set; }

        ISessaoUsuario SessaoUsuario { get; set; }

        DateTime? DataHoraInicio { get; set; }

        DateTime? DataHoraUltimaAtividade { get; set; }

        DateTime? DataHoraFim { get; set; }

        long TotalBytesEnviado { get; set; }

        int TotalArquivosLocal { get; set; }

        int TotalArquivosEnviado { get; set; }

        double ProgressoEnvioArquivo { get; set; }

        double VelocidadeEnvio { get; set; }

    }
}
