using Snebur.Dominio.Atributos;
using System.ComponentModel;

namespace Snebur.Dominio
{
    [IgnorarGlobalizacao]
    public enum EnumPlataforma
    {
        [Description("PC")]
        PC = 1,

        [Description("Celular")]
        Celular = 2,

        [Description("Tablet")]
        Tablet = 3,

        [Description("Desconhecido")]
        Desconhecido = 99,
    }
}