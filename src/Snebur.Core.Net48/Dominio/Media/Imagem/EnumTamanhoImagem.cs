using System.ComponentModel;

namespace Snebur.Dominio
{
    public enum EnumTamanhoImagem
    {
        [Description("Miniatura")]
        Miniatura = 2,

        [Description("Pequena")]
        Pequena = 4,

        [Description("Media")]
        Media = 8,

        [Description("Grande")]
        Grande = 16,

        [Description("Impressão")]
        Impressao = 32,
    }
}