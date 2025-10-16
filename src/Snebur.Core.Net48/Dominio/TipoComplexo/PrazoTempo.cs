using Snebur.Dominio.Atributos;
using Snebur.Utilidade;
using System;

namespace Snebur.Dominio
{
    public class PrazoTempo : BaseTipoComplexo, IPrazoTempo
    {
        public static PrazoTempo Empty => new PrazoTempo(EnumTipoPrazo.DiasUteis, 0);

        //private double? _prazoMinimo;
        private double _prazo;
        private EnumTipoPrazo _tipoPrazo;

        //public double? PrazoMinimo { get => this._prazoMinimo; set => this.NotificarValorPropriedadeAlterada(this._prazoMinimo, this._prazoMinimo = value); }
        public double Prazo { get => this._prazo; set => this.NotificarValorPropriedadeAlterada(this._prazo, this._prazo = value); }

        public EnumTipoPrazo TipoPrazo { get => this._tipoPrazo; set => this.NotificarValorPropriedadeAlterada(this._tipoPrazo, this._tipoPrazo = value); }

        [NaoMapear]
        [IgnorarPropriedade]
        [IgnorarPropriedadeTSReflexao]
        public TimeSpan Tempo
        {
            get
            {
                switch (this.TipoPrazo)
                {
                    case EnumTipoPrazo.DiasUteis:
                    case EnumTipoPrazo.DiasCorrido:

                        return TimeSpan.FromDays(this.Prazo);

                    case EnumTipoPrazo.Horas:

                        return TimeSpan.FromMilliseconds(this.Prazo);

                    default:
                        throw new Erro("o tipo de prazo não é suportado");
                }
            }
            set
            {
                switch (this.TipoPrazo)
                {
                    case EnumTipoPrazo.DiasUteis:
                    case EnumTipoPrazo.DiasCorrido:
                        this.Prazo = value.TotalDays;
                        break;
                    case EnumTipoPrazo.Horas:
                        this.Prazo = value.TotalMilliseconds;
                        break;
                    default:
                        throw new Erro("o tipo de prazo não é suportado");
                }
            }
        }

        public PrazoTempo()
        {
        }

        public PrazoTempo(EnumTipoPrazo tipo, double prazo)
        {
            this.TipoPrazo = tipo;
            this.Prazo = prazo;
        }

        public DateTime RetornarDataPrevista(DateTime dataInicio)
        {
            if (this.Prazo > 0)
            {
                var dataFim = dataInicio.AddMilliseconds(this.Tempo.TotalMilliseconds);
                if (this.TipoPrazo == EnumTipoPrazo.DiasUteis)
                {
                    var diasInuteis = DataHoraUtil.RetornarTotalDiasNaoUtieis(dataInicio, dataFim);
                    return dataFim.AddDays(diasInuteis);
                }
                return dataFim;
            }
            return dataInicio;

        }

        protected internal override BaseTipoComplexo BaseClone()
        {
            return new PrazoTempo
            {
                Prazo = this.Prazo,
                //PrazoMinimo = this.PrazoMinimo,
                TipoPrazo = this.TipoPrazo
            };
        }

        public override bool Equals(object obj)
        {
            if (obj is PrazoTempo prazo)
            {
                return this.Prazo == prazo.Prazo &&
                       //this.PrazoMinimo == prazo.PrazoMinimo &&
                       this.TipoPrazo == prazo.TipoPrazo;
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return Convert.ToInt32(this.Prazo + /* Convert.ToDouble(this.PrazoMinimo) + */  Convert.ToDouble(this.TipoPrazo));
        }
        #region Operadores

        public static bool operator ==(PrazoTempo prazoTempo1, PrazoTempo prazoTempo2)
        {
            if (prazoTempo1 != null && prazoTempo2 != null)
            {
                return prazoTempo1.Equals(prazoTempo2);
            }
            if ((prazoTempo1 is null) && prazoTempo2 is null)
            {
                return true;
            }
            return false;
        }

        public static bool operator !=(PrazoTempo prazoTempo1, PrazoTempo prazoTempo2)
        {
            if (!(prazoTempo1 is null) && !(prazoTempo2 is null))
            {
                return !prazoTempo1.Equals(prazoTempo2);
            }
            if ((prazoTempo1 is null) && prazoTempo2 is null)
            {
                return true;
            }
            return false;
        }

        public static PrazoTempo operator +(PrazoTempo prazoTempo1, PrazoTempo prazoTempo2)
        {
            return prazoTempo1.Adicionar(prazoTempo2);
        }

        public static PrazoTempo operator -(PrazoTempo prazoTempo1, PrazoTempo prazoTempo2)
        {
            return prazoTempo1.Remover(prazoTempo2);
        }

        private PrazoTempo Remover(PrazoTempo prazoTempo)
        {
            var tempoTotal = this.Tempo - prazoTempo.Tempo;
            return new PrazoTempo
            {
                TipoPrazo = this.TipoPrazo,
                Tempo = tempoTotal
            };
        }

        public PrazoTempo Adicionar(PrazoTempo prazoTempo)
        {
            var tempoTotal = this.Tempo + prazoTempo.Tempo;
            return new PrazoTempo
            {
                TipoPrazo = this.TipoPrazo,
                Tempo = tempoTotal
            };
        }
        #endregion
    }
}