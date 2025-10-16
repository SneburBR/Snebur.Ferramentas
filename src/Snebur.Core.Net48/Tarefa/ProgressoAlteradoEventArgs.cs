using System;

namespace Snebur.Tarefa
{
    public class ProgressoAlteradoEventArgs : EventArgs
    {
        public BaseTarefa Tarefa { get; set; }
        public double Progresso { get; set; }

        public ProgressoAlteradoEventArgs(BaseTarefa tarefa, double progresso)
        {
            this.Tarefa = tarefa;
            this.Progresso = progresso;
        }
    }
}
