using Snebur.Utilidade;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Timers;

namespace Snebur.Tarefa
{
    public abstract class BaseTarefa : ITarefa, INotifyPropertyChanged
    {
        /// <summary>
        /// Tempo maixmo ocioso
        /// </summary>
        private static TimeSpan TIMEOUT_PADRAO = TimeSpan.FromSeconds(45);

        #region Propriedades

        private double _progresso;
        private EnumStatusTarefa _statusTarefa { get; set; }

        public double Progresso
        {
            get
            {
                return this._progresso;
            }
            set
            {
                this._progresso = value;
                this.NotificarPropriedadeAlterada(nameof(this.Progresso));
            }
        }

        public EnumStatusTarefa Status
        {
            get
            {
                return this._statusTarefa;
            }
            set
            {
                this._statusTarefa = value;
                this.NotificarPropriedadeAlterada(nameof(this.Status));
                this.NotificarStatusTarefaAlterado();
            }
        }

        public Guid Identificador { get; }

        public TimeSpan Timeout { get; set; }
        public bool AtivarProgresso { get; set; }
        public DateTime DataHoraUltimaAtividade { get; set; }
        public Exception Erro { get; set; }

        protected Action<ResultadoTarefaFinalizadaEventArgs> CallbackTarefaConcluida;

        #endregion

        #region Eventos

        public event EventHandler<ProgressoAlteradoEventArgs> ProgressoAlterado;
        public event EventHandler<StatusTarefaAlteradoEventArgs> StatusTarefaAlterado;
        //public event EventHandler<Exception> EventoErro;

        #endregion

        public static readonly object _bloqueio = new object();

        private Timer TimerAnalizarTimeout { get; set; }

        private TimeSpan IntervalorAnalisarTimeout { get; set; } = TIMEOUT_PADRAO;

        #region Construtor

        public BaseTarefa()
        {
            this.Identificador = Guid.NewGuid();
            this._statusTarefa = EnumStatusTarefa.Aguardando;
            this.Timeout = TIMEOUT_PADRAO;
            this.DataHoraUltimaAtividade = DateTime.Now;
        }
        #endregion

        #region Métodos publicos

        public virtual void IniciarAsync(Action<ResultadoTarefaFinalizadaEventArgs> callback)
        {
            this.CallbackTarefaConcluida = callback;
            ThreadUtil.ExecutarAsync(this.Iniciar, true);
        }

        private void Iniciar()
        {
            this.TimerAnalizarTimeout = new Timer((int)this.IntervalorAnalisarTimeout.TotalMilliseconds);
            this.TimerAnalizarTimeout.Start();

            this.Status = EnumStatusTarefa.Executando;
            this.DataHoraUltimaAtividade = DateTime.Now;
            this.ExecutarInterno();
        }

        public void FinalizarTarefa(Exception erro)
        {
            lock (_bloqueio)
            {
                this.TimerAnalizarTimeout?.Stop();
                this.Status = (erro != null) ? EnumStatusTarefa.Erro : EnumStatusTarefa.Concluida;
                if (this.CallbackTarefaConcluida != null)
                {
                    var callback = this.CallbackTarefaConcluida;
                    this.CallbackTarefaConcluida = null;

                    var resultado = new ResultadoTarefaFinalizadaEventArgs(this, erro);
                    callback(resultado);
                }
            }
        }

        public void NotificarProgresso(double progresso)
        {
            this.DataHoraUltimaAtividade = DateTime.Now;
            this.Progresso = progresso;
            if (progresso < 0) progresso = 0;
            if (progresso > 100) progresso = 100;

            if (this.ProgressoAlterado != null)
            {
                var progressoEventArgs = new ProgressoAlteradoEventArgs(this, progresso);
                TaskUtil.Run(() =>
                {
                    this.ProgressoAlterado(this, progressoEventArgs);
                });
            }
        }

        private void NotificarStatusTarefaAlterado()
        {
            this.DataHoraUltimaAtividade = DateTime.Now;
            if (this.StatusTarefaAlterado != null)
            {
                throw new NotImplementedException();
            }
        }
        #endregion

        #region Métodos privados

        #endregion

        #region Métodos notificar eventos

        protected void NotificarProgressoAlterado(double progresso)
        {
            var args = new ProgressoAlteradoEventArgs(this, progresso);
            this.NotificarProgressoAlterado(progresso);
        }

        protected void NotificarProgressoAlterado(ProgressoAlteradoEventArgs progressoEventArgs)
        {
            this.Progresso = progressoEventArgs.Progresso;

            if (this.ProgressoAlterado != null)
            {
                TaskUtil.Run(() =>
                {
                    try { this.ProgressoAlterado(this, progressoEventArgs); } catch { };
                });
            }
        }
        #endregion

        #region Métodos abstratodos

        protected void IniciarPausamento()
        {
            this.Status = EnumStatusTarefa.Pausando;
            this.DataHoraUltimaAtividade = DateTime.Now;
            this.TimerAnalizarTimeout.Stop();
        }

        protected void IniciarCancelamento()
        {
            this.Status = EnumStatusTarefa.Cancelando;
            this.DataHoraUltimaAtividade = DateTime.Now;
            this.TimerAnalizarTimeout.Stop();
        }

        protected void IniciarContinuar()
        {
            this.Status = EnumStatusTarefa.Executando;
            this.DataHoraUltimaAtividade = DateTime.Now;
            this.TimerAnalizarTimeout.Start();
        }

        internal abstract void ExecutarInterno();

        public void PausarAsync(Action callbackConcluido)
        {
            this.IniciarPausamento();
            Task.Factory.StartNew(() =>
            {
                this.Pausar(() =>
                {
                    this.Status = EnumStatusTarefa.Pausada;
                    callbackConcluido?.Invoke();
                });
            });
        }
        public void CancelarAsync(Action callbackConcluido)
        {
            this.IniciarCancelamento();
            TaskUtil.Run(() =>
            {
                this.Cancelar(() =>
                {
                    this.Status = EnumStatusTarefa.Cancelada;
                    callbackConcluido?.Invoke();
                });
            });
        }

        public void ContinuarAsync()
        {
            this.IniciarContinuar();
            TaskUtil.Run(this.Continuar);
        }

        protected abstract void Pausar(Action callbackConcluido);

        protected abstract void Cancelar(Action callbackConcluido);

        protected abstract void Continuar();

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotificarPropriedadeAlterada([CallerMemberName] string nomePropriedade = "")
        {
            if (String.IsNullOrEmpty(nomePropriedade))
            {
                throw new ErroNaoDefinido(String.Format("O {0} não foi definida", nameof(nomePropriedade)));
            }
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nomePropriedade));
        }
        #endregion
    }
}