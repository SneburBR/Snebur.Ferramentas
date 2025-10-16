//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Snebur;
//using Snebur.Utilidade;
//using Snebur.Dominio;
//using Snebur.Dominio.Atributos;

//namespace Snebur.Dominio
//{
//    //public class Retangulo : BaseTipoComplexo<Retangulo>
//    public class Retangulo : BaseTipoComplexo, IRetangulo
//    {
//        private double _x;
//        private double _y;
//        private double _largura;
//        private double _altura;

//        public double X { get => this._x; set => this.NotificarValorPropriedadeAlterada(this._x, this._x = value); }

//        public double Y { get => this._y; set => this.NotificarValorPropriedadeAlterada(this._y, this._y = value); }

//        public double Largura { get => this._largura; set => this.NotificarValorPropriedadeAlterada(this._largura, this._largura = value); }

//        public double Altura { get => this._altura; set => this.NotificarValorPropriedadeAlterada(this._altura, this._altura = value); }

//        [IgnorarPropriedadeTS]
//        [IgnorarPropriedadeTSReflexao]
//        public double LarguraVisualizacao { get => this.RetornarValorVisualizacao(this._largura); }

//        [IgnorarPropriedadeTS]
//        [IgnorarPropriedadeTSReflexao]
//        public double AlturaVisualizacao { get => this.RetornarValorVisualizacao(this._largura); }

//        [IgnorarPropriedadeTS]
//        [IgnorarPropriedadeTSReflexao]
//        public double XVisualizacao { get => this.RetornarValorVisualizacao(this._x); }

//        [IgnorarPropriedadeTS]
//        [IgnorarPropriedadeTSReflexao]
//        public double YVisualizacao { get => this.RetornarValorVisualizacao(this._y); }

//        [IgnorarConstrutorTS]
//        public Retangulo()
//        {
//        }

//        public Retangulo(double x, double y, double largura, double altura)
//        {
//            this.Largura = largura;
//            this.Altura = altura;
//            this.X = x;
//            this.Y = y;
//        }

//        public static Retangulo Empty
//        {
//            get
//            {
//                return new Retangulo(0, 0, 0, 0);
//            }
//        }

//        protected internal override BaseTipoComplexo BaseClone()
//        {
//            return new Retangulo(this.X, this.Y, this.Largura, this.Altura);
//        }
//    }
//}