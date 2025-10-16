using Snebur.Dominio.Atributos;
using System;

namespace Snebur.Tarefa
{
    [IgnorarInterfaceTS]
    public interface ITarefa
    {
        double Progresso { get; set; }

        Guid Identificador { get; }

        EnumStatusTarefa Status { get; set; }

        bool AtivarProgresso { get; set; }

        DateTime DataHoraUltimaAtividade { get; set; }

        Exception Erro { get; set; }

        void IniciarAsync(Action<ResultadoTarefaFinalizadaEventArgs> callbackConcluido);

        void PausarAsync(Action callbackConcluido);

        void CancelarAsync(Action callbackConcluido);

        void ContinuarAsync();
    }
}