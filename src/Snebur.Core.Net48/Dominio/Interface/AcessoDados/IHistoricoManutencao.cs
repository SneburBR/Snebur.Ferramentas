using System;

namespace Snebur.Dominio
{
    public interface IHistoricoManutencao : IEntidade
    {
        string MigrationId { get; set; }

        int Prioridade { get; set; }

        string NomeTipoManutencao { get; set; }

        DateTime? DataHoraExecucao { get; set; }

        DateTime? DataHoraUltimaExecucao { get; set; }

        bool IsSucesso { get; set; }

        string MensagemErro { get; set; }

        int NumeroTentativa { get; set; }

    }
}
