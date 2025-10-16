using Snebur.Dominio.Atributos;

namespace Snebur.Dominio
{
    public partial class Posicao : BaseMedidaTipoComplexo, IPosicao
    {
        private double _x;
        private double _y;

        public double X { get => this._x; set => this.NotificarValorPropriedadeAlterada(this._x, this._x = value); }

        public double Y { get => this._y; set => this.NotificarValorPropriedadeAlterada(this._y, this._y = value); }

        public Posicao()
        {
        }

        [IgnorarConstrutorTS]
        public Posicao(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }

        protected internal override BaseTipoComplexo BaseClone()
        {
            return new Posicao(this.X, this.Y);
        }

        public override string ToString()
        {
            return $" {nameof(Posicao)} ({this.X}, {this.Y})";
        }
    }
}