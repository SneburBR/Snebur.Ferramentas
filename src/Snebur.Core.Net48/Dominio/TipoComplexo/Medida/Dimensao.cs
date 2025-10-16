using Snebur.Dominio.Atributos;
using Snebur.Utilidade;
using System;
using System.Text.RegularExpressions;

namespace Snebur.Dominio
{
    [IgnorarClasseTS]

    public partial class Dimensao : BaseMedidaTipoComplexo, IDimensao
    {
        private double _largura;
        private double _altura;

        [ValidacaoRequerido]
        public double Largura { get => this._largura; set => this.NotificarValorPropriedadeAlterada(this._largura, this._largura = value); }

        [ValidacaoRequerido]
        public double Altura { get => this._altura; set => this.NotificarValorPropriedadeAlterada(this._altura, this._altura = value); }

        public Dimensao()
        {
        }

        [IgnorarConstrutorTS]
        public Dimensao(double largura, double altura)
        {
            this._largura = largura;
            this._altura = altura;
        }

        [IgnorarConstrutorTS]
        public Dimensao(int largura, int altura)
        {
            this._largura = (double)largura;
            this._altura = (double)altura;
        }
        [IgnorarConstrutorTS]
        public Dimensao(decimal largura, decimal altura)
        {
            this._largura = (double)largura;
            this._altura = (double)altura;
        }

        [IgnorarConstrutorTS]
        public Dimensao(double largura, double altura,
                        Func<double?,double> funcaoDpiVisualizacao) : this(largura, altura)
        {
            this.FuncaoNormalizarDpiVisualizacao = funcaoDpiVisualizacao;
        }

        public override string ToString()
        {
            return base.ToString() + $" {this.Largura} x {this.Altura}";
        }

        public override bool Equals(object obj)
        {
            if (obj is Dimensao d)
            {
                return d.Largura == this.Largura && d.Altura == this.Altura;
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            //Problemas ao definir o dpi de impressão
            return base.GetHashCode();
            //return this.Largura.GetHashCode() + this.Altura.GetHashCode();
        }
        //public override Dimensao Clone()
        //{
        //    throw new System.NotImplementedException();
        //}

        //public static Tamanho Empty
        //{
        //    get
        //    {
        //        return new Tamanho(0, 0);
        //    }
        //}

        protected internal override BaseTipoComplexo BaseClone()
        {
            return new Dimensao(this.Largura, this.Altura)
            {
                FuncaoNormalizarDpiVisualizacao = this.FuncaoNormalizarDpiVisualizacao
            };
        }

        public Dimensao Escalar(double scalar)
        {
            return new Dimensao(this.Largura * scalar,
                                this.Altura * scalar);
        }

        public Dimensao ParaPixels(double dpi)
        {
            var largura = MedidaUtil.ParaPixels(this.Largura, dpi);
            var altura = MedidaUtil.ParaPixels(this.Altura, dpi);
            return new Dimensao(largura, altura);
        }

        public bool Menor(Dimensao dimensao)
        {
            return this.Largura < dimensao.Largura ||
              this.Altura < dimensao.Altura;
        }

        public Dimensao Inverter()
        {
            return new Dimensao(this.Altura, this.Largura)
            {
                FuncaoNormalizarDpiVisualizacao = this.FuncaoNormalizarDpiVisualizacao
            };
        }

        public static Dimensao Parse(string str)
        {
            if(String.IsNullOrWhiteSpace(str))
            {
                return Empty;
            }

            var regex = new Regex(@"[Xx;|]");
            var valores = regex.Split(str);
            if (valores.Length == 2)
            {
                if (Double.TryParse(valores[0], out double largura) &&
                    Double.TryParse(valores[1], out double altura))
                {
                    return new Dimensao(largura, altura);
                }
            }
            throw new FormatException($"Formato {str} inválido para Dimensao");
        }
    }
}
