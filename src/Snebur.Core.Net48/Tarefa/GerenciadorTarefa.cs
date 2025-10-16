using Snebur.Utilidade;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Snebur.Tarefa
{
    public class GerenciadorTarefa<TTarefa> : BaseTarefa, IDisposable where TTarefa : ITarefa
    {
        public const int MAXIMO_THEAD_SIMULTANEAS_DEBUG_ATTACH = 1;
        public const int MAXIMO_THEAD_SIMULTANEAS_PADRAO = 5;

        //private List<TTarefa> Tarefas { get; set; }
        public ConcurrentQueue<TTarefa> Fila { get; set; }
        public ConcurrentDictionary<Guid, TTarefa> Executando { get; set; }

        public List<TTarefa> TarefasFinalizadas { get; set; }
        public List<TTarefa> TarefasComErros { get; set; }

        private int? _maximoTarefasSimultaneas;

        public int MaximoTarefasSimultaneas
        {
            get
            {
                if (this.Status == EnumStatusTarefa.Pausada)
                {
                    return 0;
                }
                if (this._maximoTarefasSimultaneas.HasValue)
                {
                    return this._maximoTarefasSimultaneas.Value;
                }
                if (DebugUtil.IsAttached)
                {
                    return MAXIMO_THEAD_SIMULTANEAS_DEBUG_ATTACH;
                }
                return MAXIMO_THEAD_SIMULTANEAS_PADRAO;
            }

            protected set
            {
                this._maximoTarefasSimultaneas = value;
            }
        }

        public GerenciadorTarefa()
        {
            //this.Tarefas = new List<TTarefa>();
            this.TarefasComErros = new List<TTarefa>();
            this.TarefasFinalizadas = new List<TTarefa>();
            this.Fila = new ConcurrentQueue<TTarefa>();
            this.Executando = new ConcurrentDictionary<Guid, TTarefa>();
        }

        public override void IniciarAsync(Action<ResultadoTarefaFinalizadaEventArgs> callback)
        {
            base.IniciarAsync(callback);
        }

        public void AdicionarTarefa(TTarefa tarefa)
        {
            this.Fila.Enqueue(tarefa);
        }
        #region Overrides 

        internal override void ExecutarInterno()
        {
            this.ExecutarProximaTarefa();
        }

        protected override void Continuar()
        {
            this.ExecutarProximaTarefa();
        }

        protected override void Pausar(Action callback)
        {
            this.ExecutarProximaTarefa();
            while (this.Executando.Values.Any(x => x.Status == EnumStatusTarefa.Pausando))
            {
                System.Threading.Thread.Sleep(500);
            }
            callback();
        }

        protected override void Cancelar(Action callback)
        {
            lock (_bloqueio)
            {
                foreach (var executando in this.Executando.Values)
                {
                    executando.CancelarAsync(null);
                }
            }
            while (this.Executando.Values.Any(x => x.Status == EnumStatusTarefa.Cancelando))
            {
                System.Threading.Thread.Sleep(500);
            }
            callback();
        }
        #endregion

        public void FinalizarGerenciadorTarefa()
        {
            FinalizarTarefa(this.RetornarErro());
        }

        private void ExecutarProximaTarefa()
        {
            lock (_bloqueio)
            {
                if (this.Fila.Count == 0 && this.Executando.Count == 0)
                {
                    this.FinalizarGerenciadorTarefa();
                }
                else
                {
                    //################## Pausado e continuando as Tarefas ####################

                    var totalTarefasEmExecucao = this.Executando.Values.Where(x => x.Status == EnumStatusTarefa.Executando).Count();
                    if (totalTarefasEmExecucao > this.MaximoTarefasSimultaneas)
                    {
                        var tarefasExecutandos = this.Executando.ToList();
                        for (var i = tarefasExecutandos.Count - 1; i >= this.MaximoTarefasSimultaneas; i--)
                        {
                            var tarefa = tarefasExecutandos[i].Value;
                            tarefa.PausarAsync(null);
                        }
                    }
                    else
                    {
                        var tarefasPausadas = this.Executando.Values.Where(x => x.Status == EnumStatusTarefa.Pausada).ToList();
                        if (tarefasPausadas.Count > 0)
                        {
                            var fim = this.MaximoTarefasSimultaneas - totalTarefasEmExecucao;
                            if (fim > tarefasPausadas.Count) fim = tarefasPausadas.Count;
                            for (var i = 0; i < fim; i++)
                            {
                                var tarefaPausada = tarefasPausadas[i];
                                tarefaPausada.ContinuarAsync();
                            }
                        }
                    }
                    //#############################################################

                    if (this.Executando.Count < this.MaximoTarefasSimultaneas)
                    {
                        while ((this.Fila.Count > 0) && (this.Executando.Count < this.MaximoTarefasSimultaneas))
                        {
                            TTarefa proximaTarefa;
                            if (this.Fila.TryDequeue(out proximaTarefa))
                            {
                                this.Executando.TryAdd(proximaTarefa.Identificador, proximaTarefa);
                                proximaTarefa.IniciarAsync(this.Tarefa_Finalizada);
                            }
                        }
                    }
                }
            }
        }

        private void Tarefa_Finalizada(ResultadoTarefaFinalizadaEventArgs args)
        {
            if (!this.Executando.TryRemove(args.Tarefa.Identificador, out TTarefa tarefaRemovida))
            {
                throw new InvalidOperationException("O tarefa já foi removida");
            }

            if (args.Erro != null)
            {
                this.TarefasComErros.Add((TTarefa)args.Tarefa);
            }
            this.NotificarProgresso();
            this.TarefasFinalizadas.Add((TTarefa)args.Tarefa);
            this.ExecutarProximaTarefa();
        }

        #region Métodos privados

        private Erro RetornarErro()
        {
            if (this.TarefasComErros.Count() > 0)
            {
                return new ErroGerenciadorTarefa<TTarefa>(this.TarefasComErros);
            }
            return null;
        }

        private void NotificarProgresso()
        {
            var totalTarefas = (double)(this.Fila.Count + this.Executando.Count + this.TarefasFinalizadas.Count);
            var emExecutacao = this.Executando.Values.Sum(x => (x.Progresso / 100D));
            var progresso = ((this.TarefasFinalizadas.Count + emExecutacao) / totalTarefas) * 100;
            this.NotificarProgresso(progresso);
        }
        #endregion

        #region IDisposable

        public void Dispose()
        {
        }
        #endregion
    }
}