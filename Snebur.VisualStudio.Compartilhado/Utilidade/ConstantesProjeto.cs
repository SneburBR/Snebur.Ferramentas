//futuramente vamos normalizar os Error, passando um parametro da mapeamento do arquivo e linha
//detalhes aqui https://www.html5rocks.com/en/tutorials/developertools/sourcemaps/

namespace Snebur.VisualStudio
{
    public static class ConstantesProjeto
    {
        public const string PASTA_BUILD = "build";
        public const string PASTA_SRC = "src";

        public const string EXTENSAO_ESTILO = ".css";
        public const string EXTENSAO_CONTROLE_SHTML = ".shtml";
        public const string EXTENSAO_CONTROLE_SHTML_TYPESCRIPT = ".shtml.ts";
        public const string EXTENSAO_CONTROLE_SHTML_ESTILO = ".shtml.scss";
        public const string EXTENSAO_SCRIPT = ".js";
        public const string EXTENSAO_TYPESCRIPT = ".ts";
        public const string EXTENSAO_SASS = ".scss";

        public const string CONFIGURACAO_DOMINIO = "dominio.json";
        public const string CONFIGURACAO_TYPESCRIPT = "tsconfig.json";
        public const string CONFIGURACAO_CONTEXTO_DADOS = "contextodados.json";
        public const string CONFIGURACAO_REGRIAS_NEGOCIO = "regrasnegocio.json";
        public const string CONFIGURACAO_SASS = "compilerconfig.json";
        public const string CONFIGURACAO_SERVICOS = "servicos.json";
        public const string CONFIGURACAO_WEB_CONFIG = "Web.config";
        public const string CONFIGURACAO_APP_SETTINGS = "appSettings.config";


    }

    public static class VsConstantes
    {
        public const string WebApplicationProjectTypeGuid = "{349C5851-65DF-11DA-9384-00065B846F21}";
        public const string WebSiteProjectTypeGuid = "{E24C65DC-7377-472B-9ABA-BC803B73C61A}";
        public const string CsharpProjectTypeGuid = "{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}";
        public const string VbProjectTypeGuid = "{F184B08F-C81C-45F6-A57F-5ABD9991F28F}";
        public const string CppProjectTypeGuid = "{8BC9CEB8-8B4A-11D0-8D11-00A0C91BC942}";
        public const string FsharpProjectTypeGuid = "{F2A71F9B-5D33-465A-A702-920D77279786}";
        public const string JsProjectTypeGuid = "{262852C6-CD72-467D-83FE-5EEB1973A190}";
        public const string WixProjectTypeGuid = "{930C7802-8A8C-48F9-8165-68863BCCD9DD}";
        public const string LightSwitchProjectTypeGuid = "{ECD6D718-D1CF-4119-97F3-97C25A0DFBF9}";
        public const string NemerleProjectTypeGuid = "{edcc3b85-0bad-11db-bc1a-00112fde8b61}";
        public const string InstallShieldLimitedEditionTypeGuid = "{FBB4BD86-BF63-432a-A6FB-6CF3A1288F83}";
        public const string WindowsStoreProjectTypeGuid = "{BC8A1FFA-BEE3-4634-8014-F334798102B3}";
        public const string SynergexProjectTypeGuid = "{BBD0F5D1-1CC4-42fd-BA4C-A96779C64378}";
        public const string NomadForVisualStudioProjectTypeGuid = "{4B160523-D178-4405-B438-79FB67C8D499}";

        // Copied from EnvDTE.Constants since that type can't be embedded
        public const string VsProjectItemKindPhysicalFile = "{6BB5F8EE-4483-11D3-8BCF-00C04F8EC28C}";
        public const string VsProjectItemKindPhysicalFolder = "{6BB5F8EF-4483-11D3-8BCF-00C04F8EC28C}";
        public const string VsProjectItemKindSolutionFolder = "{66A26720-8FB5-11D2-AA7E-00C04F688DDE}";
        public const string VsProjectItemKindSolutionItem = "{66A26722-8FB5-11D2-AA7E-00C04F688DDE}";
        public const string VsWindowKindSolutionExplorer = "{3AE79031-E1BC-11D0-8F78-00A0C9110057}";

        // All unloaded projects have this Kind value
        internal const string UnloadedProjectTypeGuid = "{67294A52-A4F0-11D2-AA88-00C04F688DDE}";
    }

}