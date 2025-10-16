using System;

namespace Snebur.Tarefa
{
    public class ResultadoTarefaFinalizadaEventArgs : EventArgs
    {
        public ITarefa Tarefa { get; }
        public Exception Erro { get; }
        public bool IsCancelado { get; }
        public EnumStatusTarefa Status { get; }

        public ResultadoTarefaFinalizadaEventArgs(BaseTarefa tarefa, 
                                                  Exception erro)
        {
            this.Tarefa = tarefa;
            this.Erro = erro;
            this.Status = this.Tarefa.Status;
        }
    }
}