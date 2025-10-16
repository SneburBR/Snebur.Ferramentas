using System;

namespace Snebur.Dominio.Atributos
{
    public class ValidacaoImagemAttribute : Attribute
    {
        public EnumFormatoImagem Formato { get; set; }

        public Dimensao Dimensao { get; set; }

        public ValidacaoImagemAttribute()
        {
        }

        public ValidacaoImagemAttribute(EnumFormatoImagem formato, int larguraPixel, int alturaPixel)
        {
            this.Formato = formato;
            //this.Dimensao = new Dimensa(formato;
        }
    }
}