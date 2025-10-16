using Snebur.Dominio.Atributos;
using Snebur.Utilidade;
using System;
using System.Drawing;
using System.Globalization;
using System.Linq;

namespace Snebur.Dominio
{
    //Imports System.Windows.Media

    //public class Cor : BaseTipoComplexo<Cor>
    public class Cor : BaseTipoComplexo, ICor
    {
        //private byte _r;
        //private byte _g;
        //private byte _b;
        //private byte _a;

        [NaoMapear]
        public byte Red { get; private set; }

        [NaoMapear]
        public byte Green { get; private set; }

        [NaoMapear]
        public byte Blue { get; private set; }

        [NaoMapear]
        public byte Alpha { get; private set; }

        [NaoMapear]
        public double AlphaDecimal { get; private set; }

        public string _rgba;

        [ValidacaoTextoTamanho(32)]
        public string Rgba
        {
            get => this._rgba;
            set
            {
                this.NotificarValorPropriedadeAlterada(this._rgba, this._rgba = value, nameof(this.Rgba));
                this.AtualizarValores();
            }
        }
        [IgnorarPropriedade]
        [IgnorarPropriedadeTSReflexao]
        public bool IsTransparente
        {
            get
            {
                return this.Alpha == 0;
            }
        }
        [IgnorarPropriedade]
        [IgnorarPropriedadeTSReflexao]
        public bool IsBranca
        {
            get
            {
                return this.Alpha == 255 && 
                       this.Red == 255 && 
                       this.Green == 255 && 
                       this.Blue == 255;
            }
        }
        
        [IgnorarConstrutorTS]
        public Cor() : this(0, 0, 0, 0)
        {
        }
        [IgnorarConstrutorTS]
        public Cor(int r, int g, int b) : this(r, g, b, 1)
        {
        }

        [IgnorarConstrutorTS]
        public Cor(int r, int g, int b, double a)
        {
            this._rgba = String.Format("rgba({0},{1},{2},{3})", r, g, b, a.ToString(CultureInfo.InvariantCulture));
            this.AtualizarValores();
        }

        private void AtualizarValores()
        {
            if (!String.IsNullOrEmpty(this._rgba))
            {
                var valores = TextoUtil.RetornarSomenteNumeros(this._rgba, ",.".ToArray()).Split(',');
                if (valores.Length != 4)
                {
                    throw new ErroOperacaoInvalida("o valor do rgba é invalido");
                }
                this.Red = Byte.Parse(valores[0]);
                this.Green = Byte.Parse(valores[1]);
                this.Blue = Byte.Parse(valores[2]);
                this.AlphaDecimal = ConverterUtil.ParaDouble(valores[3]);
                this.Alpha = Convert.ToByte(Byte.MaxValue * this.AlphaDecimal);
            }
        }

        public Cor(string rgba)
        {
            this._rgba = rgba;
            this.AtualizarValores();
        }
        [IgnorarPropriedade]
        [IgnorarPropriedadeTSReflexao]
        public Color ColorDrawing
        {
            get
            {
                return Color.FromArgb(this.Red, this.Green, this.Blue);
            }
        }

        protected internal override BaseTipoComplexo BaseClone()
        {
            return new Cor(this.Red, this.Green, this.Blue, this.AlphaDecimal);
        }

        public override bool Equals(object obj)
        {
            if (obj is Cor cor)
            {
                return this.Alpha == cor.Alpha &&
                       this.Red == cor.Red &&
                       this.Green == cor.Green &&
                       this.Blue == cor.Blue;
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return (this.Alpha * 1000) +
                   (this.Red * 10000) +
                   (this.Blue * 100000) +
                   (this.Green * 1000000);
        }
        //public override Cor Clone()
        //{
        //    return new Cor(this.Rgba);
        //}
    }
}