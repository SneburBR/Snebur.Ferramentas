using Snebur.Dominio.Atributos;

namespace Snebur.Dominio
{
    public partial class Regiao : BaseMedidaTipoComplexo, IRegiao
    {
        private double _x;
        private double _y;
        private double _largura;
        private double _altura;

        public double X { get => this._x; set => this.NotificarValorPropriedadeAlterada(this._x, this._x = value); }

        public double Y { get => this._y; set => this.NotificarValorPropriedadeAlterada(this._y, this._y = value); }

        public double Largura { get => this._largura; set => this.NotificarValorPropriedadeAlterada(this._largura, this._largura = value); }

        public double Altura { get => this._altura; set => this.NotificarValorPropriedadeAlterada(this._altura, this._altura = value); }

        public Regiao()
        {

        }

        [IgnorarConstrutorTS]
        public Regiao(double x, double y, double largura, double altura)
        {
            this.X = x;
            this.Y = y;
            this.Largura = largura;
            this.Altura = altura;
        }

        public Regiao(Posicao posicao, Dimensao dimensao)
        {
            this.Posicao = posicao;
            this.Dimensao = dimensao;
        }
        #region Operadores

        public static bool operator ==(Regiao regiao1, Regiao regiao2)
        {
            if (regiao1 != null && regiao2 != null)
            {
                return regiao1.Equals(regiao2);
            }
            if ((regiao1 is null) && regiao2 is null)
            {
                return true;
            }
            return false;
        }

        public static bool operator !=(Regiao regiao1, Regiao regiao2)
        {
            if (!(regiao1 is null) && !(regiao2 is null))
            {
                return !regiao1.Equals(regiao2);
            }
            if ((regiao1 is null) && regiao2 is null)
            {
                return true;
            }
            return false;
        }

        public override bool Equals(object obj)
        {
            if (obj != null && obj is Regiao regiao)
            {
                return this.X == regiao.X &&
                       this.Y == regiao.Y &&
                       this.Largura == regiao.Largura &&
                       this.Altura == regiao.Altura;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        public override string ToString()
        {
            return $"{this.X}-{this.Y}-{this.Largura}-{this.Altura}";
        }
        #endregion

        protected internal override BaseTipoComplexo BaseClone()
        {
            return new Regiao(this.X, this.Y, this.Largura, this.Altura);
        }
    }
}