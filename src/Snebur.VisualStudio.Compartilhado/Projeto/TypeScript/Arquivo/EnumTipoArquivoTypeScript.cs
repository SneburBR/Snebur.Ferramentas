using System;

namespace Snebur.VisualStudio
{
    public enum EnumTipoArquivoTypeScript
    {
        SistemaInicio = 0,

        SistemaDeclarationType = 5,

        Nativo = 10,

        EnumBase = 25,

        EnumExport = 30,

        ClasseBaseAbstrata = 32,

        ClasseBase = 35,

        ClasseExportAbstrata = 40,

        ClasseExport = 50,

        Inteface = 60,

        IntefaceExport = 65,

        SistemaReflexao = 70,

        SistemaExtensaoReflexao = 75,

        DominioClasses = 80,

        DominioEnums = 90,

        DominioAtributos = 100,

        DominioInterfaces = 105,

        DominioConstantes = 108,

        DominioReflexao = 110,

        //SistemaReferencias = 115,

        ClassePartial = 120,

        ClasseStatica = 125,

        BaseClasseViewModel = 126,

        ClasseViewModel = 127,

        BaseClasseViewModelAbstrata = 128,

        ClasseViewModelAbstrataExport = 129,

        SistemaReferencias = 860,

        //SistemaMapeamento = 865,
        SistemaMapeamentos = 865,

        SistemaExports = 870,
         
        SistemaLocalConfig = 880,

        SistemaAplicacaoConfiguracao = 890,

        SistemaExtensaoAplicacaoConfiguracao = 895,

        SistemaAplicacao = 900,

        SistemaExtensaoAplicacao = 950,

        Vazio = 999,

        Teste = 1050,

        Desconhecido = int.MaxValue,

    }
}
