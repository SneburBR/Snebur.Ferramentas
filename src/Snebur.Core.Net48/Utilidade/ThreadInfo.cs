using System;
using System.Threading;

namespace Snebur.Utilidade
{
    public class ThreadInfo
    {
        public DateTime DataHoraInicio { get; }
        public DateTime DataHoraFim { get; }
        public TimeSpan Duracao { get; }
        public Thread Thread { get; }

        public ThreadInfo(Thread thread)
        {
            this.Thread = thread;
            this.DataHoraInicio = DateTime.Now;
        }

        public override string ToString()
        {
            var tempo = DateTime.Now - this.DataHoraInicio;
            var descricaoAtiva = this.Thread.IsAlive ? "Ativa" : "Finalizada";
            return String.Format("{0} -  {1} - {2}", this.Thread.Name, descricaoAtiva, tempo.ToString());
        }
    }
}