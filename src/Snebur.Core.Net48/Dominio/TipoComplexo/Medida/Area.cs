using Snebur.Dominio.Atributos;
using System;

namespace Snebur.Dominio
{
    public partial class Area : BaseMedidaTipoComplexo, IArea
    {
        private double? _esquerda = null;
        private double? _superior = null;
        private double? _direita = null;
        private double? _inferior = null;

        private double _largura;
        private double _altura;

        public double? Esquerda { get => this._esquerda; set => this.NotificarValorPropriedadeAlterada(this._esquerda, this._esquerda = value); }

        public double? Superior { get => this._superior; set => this.NotificarValorPropriedadeAlterada(this._superior, this._superior = value); }

        public double? Direita { get => this._direita; set => this.NotificarValorPropriedadeAlterada(this._direita, this._direita = value); }

        public double? Inferior { get => this._inferior; set => this.NotificarValorPropriedadeAlterada(this._inferior, this._inferior = value); }

        public double Largura { get => this._largura; set => this.NotificarValorPropriedadeAlterada(this._largura, this._largura = value); }

        public double Altura { get => this._altura; set => this.NotificarValorPropriedadeAlterada(this._altura, this._altura = value); }

        public Area()
        {
        }
        [IgnorarConstrutorTS]
        public Area(Margem margem, Dimensao dimensao)
        {
            this.Margem = margem;
            this.Dimensao = dimensao;
        }
        [IgnorarConstrutorTS]
        public Area(double? esquerda, double? superior, double? direita, double? inferior, double largura, double altura)
        {
            this._esquerda = esquerda;
            this._superior = superior;
            this._direita = direita;
            this._inferior = inferior;
            this._largura = largura;
            this._altura = altura;
        }
        #region Operadores

        public static bool operator ==(Area area1, Area area2)
        {
            if (area1 != null && area2 != null)
            {
                return area1.Equals(area2);
            }
            if ((area1 is null) && area2 is null)
            {
                return true;
            }
            return false;
        }

        public static bool operator !=(Area area1, Area area2)
        {
            if (!(area1 is null) && !(area2 is null))
            {
                return !area1.Equals(area2);
            }
            if ((area1 is null) && area2 is null)
            {
                return true;
            }
            return false;
        }
        #endregion

        #region Métodos

        public override bool Equals(object obj)
        {
            if (obj != null && obj is Area area)
            {
                return this.Esquerda == area.Esquerda &&
                       this.Superior == area.Superior &&
                       this.Direita == area.Direita &&
                       this.Inferior == area.Inferior &&
                       this.Largura == area.Largura &&
                       this.Altura == area.Altura;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        public override string ToString()
        {
            return $"{this.Esquerda}-{this.Superior}-{this.Direita}-{this.Inferior}-{this.Largura}-{this.Altura}";
        }

        protected internal override BaseTipoComplexo BaseClone()
        {
            return new Area(this.Esquerda, this.Superior, this.Direita, this.Inferior, this.Largura, this.Altura);
        }

        public Regiao CalcularRegiao(Dimensao dimensaoRecipiente)
        {
            throw new NotImplementedException();
        }
        #endregion

    }
}