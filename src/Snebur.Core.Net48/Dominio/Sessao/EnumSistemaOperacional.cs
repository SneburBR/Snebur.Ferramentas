using Snebur.Dominio.Atributos;
using System.ComponentModel;

namespace Snebur.Dominio
{
    [IgnorarGlobalizacao]
    public enum EnumSistemaOperacional
    {
        [Description("Windows")]
        Windows = 1,

        [Description("Mac OS X")]
        MacOS_X = 2,

        [Description("Windows Phone")]
        WindowsPhone = 3,

        [Description("Android")]
        Android = 4,

        [Description("iOS")]
        iOS = 5,

        [Description("Linux")]
        Linux = 6,

        [Description("Desconhecido")]
        Desconhecido = 99
    }
}