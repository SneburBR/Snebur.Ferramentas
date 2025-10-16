using System.ComponentModel;
#if !EXTENSAO_VISUALSTUDIO  
using Snebur.Dominio.Atributos;
#endif

namespace Snebur.Reflexao
{
    [IgnorarEnumTS]
    public enum EnumTipoPrimario
    {
        [Description("Desconhecido")]
        Desconhecido = -1,

        [Description("void")]
        Void = 0,

        [Description("Boolean")]
        Boolean = 1,

        [Description("String")]
        String = 2,

        [Description("Integer")]
        Integer = 3,

        [Description("Long")]
        Long = 4,

        [Description("Decimal")]
        Decimal = 5,

        [Description("Double")]
        Double = 6,

        [Description("DateTime")]
        DateTime = 7,

        [Description("TimeSpan")]
        TimeSpan = 8,

        [Description("EnumValor")]
        EnumValor = 9,

        [Description("Guid")]
        Guid = 10,

        [Description("Object")]
        Object = 11,

        [Description("Single")]
        Single = 12,

        [Description("Char")]
        Char = 13,

        [Description("Byte")]
        Byte = 14,
         
        //[Description("Uri")]
        //Uri = 11,

        //[Description("Object")]
        //Object = 12,

        //[Description("BaseDominio")]
        //BaseDominio = 50,
    }
}