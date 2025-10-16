using Snebur.Utilidade;
using System;

namespace Snebur
{
    public class ExecutarDepois : IDisposable
    {
        private int _totalMilesegundos;
        private int Identificador;
        private readonly Delegate Acao;

        private object[] Parametros;
        public bool IsExecutarPedente { get; private set; }

        public bool IsNuncaExecutado { get; private set; } = true;
        private readonly object _bloqueio = new object();
        public int TotalMilesegundos
        {
            get => this._totalMilesegundos;
            set
            {
                if (this._totalMilesegundos < 0)
                {
                    throw new Exception($"O  {nameof(this.TotalMilesegundos)} deve ser maior que 0(zero)");
                }
                this._totalMilesegundos = value;
            }
        }

        public ExecutarDepois(Delegate acao, int totalMilisegundos)
        {
            this.Acao = acao;
            this._totalMilesegundos = totalMilisegundos;
        }

        public void Executar(params object[] parametros)
        {
            lock (this._bloqueio)
            {
                ThreadUtil.ClearTimeout(this.Identificador);
                this.Identificador = ThreadUtil.SetTimeout(this.Executar_Depois, this.TotalMilesegundos);
                this.Parametros = parametros;
                this.IsExecutarPedente = true;
            }
        }

        public void ExecutarAgoara(params object[] parametros)
        {
            this.Parametros = parametros;
            this.ExecutarInterno(parametros);
        }

        private void Executar_Depois()
        {
            if (this.IsExecutarPedente)
            {
                this.ExecutarInterno(this.Parametros);
            }
        }

        private void ExecutarInterno(object[] parametros)
        {
            if (this.Acao is Delegate acaoTipada)
            {
                if (this.Parametros != null)
                {
                    acaoTipada.DynamicInvoke(this.Parametros);
                }
                else
                {
                    acaoTipada.DynamicInvoke();
                }
            }
            this.IsNuncaExecutado = false;
            this.IsExecutarPedente = false;
            ThreadUtil.ClearTimeout(this.Identificador);
        }

        public void Cancelar()
        {
            ThreadUtil.ClearTimeout(this.Identificador);
        }

        public void Dispose()
        {
            ThreadUtil.ClearTimeout(this.Identificador);
        }
    }
}