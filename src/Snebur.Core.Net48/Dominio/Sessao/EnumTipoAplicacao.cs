using Snebur.Dominio.Atributos;
using System.ComponentModel;

namespace Snebur.Dominio
{
    [IgnorarGlobalizacao]
    public enum EnumTipoAplicacao
    {
        [Description("Typescript Client")]
        Typescript = 1,

        [Description("Asp.NET .NET Framework")]
        Web_AspNet = 2,
         
        [Description("Servicos web")]
        DotNet_WebService = 3,

        [Description("Windows Service .NET Framework")]
        DotNet_WindowService = 4,

        [Description("Wpf .NET Framework")]
        DotNet_Wpf = 5,

        [Description("UnidadeTeste")]
        DotNet_UnitTest = 6,

        [Description("ExtensaoVisualStudio")]
        ExtensaoVisualStudio = 7,
         
        [Description("AspNet Core web service")]
        AspNetCore_WebService = 8,

        [Description("Maui .NET")]
        MaiuNet = 9,

        [Description("Console .NET Framework")]
        DotNet_Console = 10,

        [Description("Desconhecido")]
        Desconhecido = 99,

    }
}