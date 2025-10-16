using System;

namespace Snebur.Dominio.Atributos
{
    public class FormatarDadosAttribute : Attribute
    {
        public EnumFormatacaoDados FormatadoDados { get; }

        public FormatarDadosAttribute(EnumFormatacaoDados formatacao)
        {
            this.FormatadoDados = formatacao;
        }
    }

    public enum EnumFormatacaoDados
    {
        SomenteNumeros,
        SomenteLetras,
        LetrasNumeros,
        Personalizado
    }

}
