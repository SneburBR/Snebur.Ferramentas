using Snebur.Dominio.Atributos;
using System;

namespace Snebur.Dominio
{
    //public class Margem : BaseTipoComplexo<Margem>
    public partial class Margem : BaseMedidaTipoComplexo, IMargem
    {
        private double? _esquerda = null;
        private double? _superior = null;
        private double? _direita = null;
        private double? _inferior = null;

        public double? Esquerda { get => this._esquerda; set => this.NotificarValorPropriedadeAlterada(this._esquerda, this._esquerda = value); }

        public double? Superior { get => this._superior; set => this.NotificarValorPropriedadeAlterada(this._superior, this._superior = value); }

        public double? Direita { get => this._direita; set => this.NotificarValorPropriedadeAlterada(this._direita, this._direita = value); }

        public double? Inferior { get => this._inferior; set => this.NotificarValorPropriedadeAlterada(this._inferior, this._inferior = value); }
         
        public double MaiorValor => Math.Max(this.Esquerda ?? 0, Math.Max(this.Superior ?? 0, Math.Max(this.Direita ?? 0, this.Inferior ?? 0)));

        public double MenorValor => Math.Min(this.Esquerda ?? 0, Math.Min(this.Superior ?? 0, Math.Min(this.Direita ?? 0, this.Inferior ?? 0)));
        public double ValorMedio => (this.Esquerda ?? 0 + this.Superior ?? 0 + this.Direita ?? 0 + this.Inferior ?? 0) / 4;

        public double Vertical => Math.Min((this.Superior ?? 0), (this.Inferior ?? 0));
        public double Horizontal => Math.Min((this.Esquerda ?? 0), (this.Direita ?? 0));
        
        
        public Margem()
        {
        }
        [IgnorarConstrutorTS]
        public Margem(double margem)
        {
            this._esquerda = margem;
            this._superior = margem;
            this._direita = margem;
            this._inferior = margem;
        }
        [IgnorarConstrutorTS]
        public Margem(double margemHorizontal, double margemVertical)
        {
            this._esquerda = margemHorizontal;
            this._superior = margemVertical;
            this._direita = margemHorizontal;
            this._inferior = margemVertical;
        }
        [IgnorarConstrutorTS]
        public Margem(double? esquerda, double? superior, double? direita, double? inferior)
        {
            this._esquerda = esquerda;
            this._superior = superior;
            this._direita = direita;
            this._inferior = inferior;
        }

        #region Operadores

        public static bool operator ==(Margem margem1, Margem margem2)
        {
            if (margem1 != null && margem2 != null)
            {
                return margem1.Equals(margem2);
            }
            if ((margem1 is null) && margem2 is null)
            {
                return true;
            }
            return false;
        }

        public static bool operator !=(Margem margem1, Margem margem2)
        {
            if (!(margem1 is null) && !(margem2 is null))
            {
                return !margem1.Equals(margem2);
            }
            if ((margem1 is null) && margem2 is null)
            {
                return true;
            }
            return false;
        }

        public override bool Equals(object obj)
        {
            if (obj != null && obj is Margem margem)
            {
                return this.Esquerda == margem.Esquerda &&
                       this.Superior == margem.Superior &&
                       this.Direita == margem.Direita &&
                       this.Inferior == margem.Inferior;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
            //return this.ToString().GetHashCode();
        }

        public override string ToString()
        {
            return $"{this.Esquerda}-{this.Superior}-{this.Direita}-{this.Inferior}-DPI-{this.DpiVisualizacao:0.0}";
        }

        public string ToStringDecimal(int casas = 2)
        {
            return $"{this.Esquerda.FormatDecimal(casas)},{this.Superior.FormatDecimal(casas)},{this.Direita.FormatDecimal(casas)}-{this.Inferior.FormatDecimal(casas)}";
        }

        #endregion

        protected internal override BaseTipoComplexo BaseClone()
        {
            return new Margem(this.Esquerda, this.Superior, this.Direita, this.Inferior);
        }
    }
}