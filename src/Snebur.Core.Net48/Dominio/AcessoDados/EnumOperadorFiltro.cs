using System.ComponentModel;

namespace Snebur.AcessoDados
{
    public enum EnumOperadorFiltro
    {
        [Description("Igual")]
        Igual = 1,

        [Description("Igual absoluto")]
        IgualAbsoluto = 2,

        [Description("Diferente")]
        Diferente = 3,

        [Description("Maior")]
        Maior = 4,

        [Description("Menor")]
        Menor = 5,

        [Description("Maior ou igual")]
        MaiorIgual = 6,

        [Description("Menor ou igual")]
        MenorIgual = 7,

        [Description("Inicia com")]
        IniciaCom = 8,

        [Description("Termina com")]
        TerminaCom = 9,

        [Description("Possui")]
        Possui = 10
    }
}