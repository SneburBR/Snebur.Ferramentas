using Snebur.Dominio.Atributos;
using System.ComponentModel;

namespace Snebur.Dominio
{
    [IgnorarGlobalizacao]
    public enum EnumNavegador
    {
        [Description("Internet Explorer")]
        InternetExplorer = 1,

        [Description("Chrome")]
        Chrome = 2,

        [Description("Safari")]
        Safari = 3,

        [Description("Firefox")]
        Firefox = 4,

        [Description("Opera")]
        Opera = 5,

        [Description("Edge")]
        Edge = 6,

        [Description("Edge Chromium")]
        EdgeChromium = 7,

        [Description("Nenhum")]
        Nenhum = 98,

        [Description("Desconhecido")]
        Desconhecido = 99,
    }
}