//using Snebur.Utilidade;
//using System.IO;

//namespace Snebur.VisualStudio
//{
//    public class CaminhosUtil
//    {

//        public static string REPOSITORIO_SCHEMA_HTML => Path.Combine(ConfiguracaoGeral.Instance.CaminhoInstalacaoVisualStudio, @"Common7\Packages\Schemas\html");
//        public const string ARQUIVO_HTML_5 = "html_5.xsd";
//        public const string ARQUIVO_X_HTML_5 = "xhtml_5.xsd";


//        private const string CAMINHO_PARCIAL_ATRIBUTOS_TYPESCRIPT = @"Snebur.Typescript\src\Snebur.UI.Typescript\src\Atributo\AtributosHtml.Statica.ts";
//        private const string CAMINHO_PARCIAL_CONTROLES_TYPESCRIPT = @"Snebur.Typescript\src\Snebur.UI.Typescript\src\Controle\ElementoControle\ElementoControle.Registrar.ts";
//        private const string CAMINHO_PARCIAL_ELEMENTOS_APRESENTACAO_TYPESCRIPT = @"Snebur.Typescript\src\Snebur.UI.Typescript\src\Componentes\Registrar\ComponentesApresentacao.Registrar.ts";

//        private const string CAMINHO_PARCIAL_ASSEMBLY_SNEBUR = @"Snebur.Framework\src\Core\bin\Debug\Snebur.dll";
//        private const string CAMINHO_PARCIAL_ASSEMBLY_ZYONCORE = @"Snebur.Framework\src\Core\bin\Debug\Zyoncore.dll";

//        private const string CAMINHO_PARCIAL_ASSEMBLY_SNEBUR_DEPURACAO = @"Snebur.Framework\src\Depuracao\bin\Debug\Snebur.Depuracao.dll";
//        private const string CAMINHO_PARCIAL_ASSEMBLY_SNEBUR_COMUNICACAO = @"Snebur.Framework\src\Comunicacao\src\Comunicacao\bin\Debug\Snebur.Comunicacao.dll";
//        private const string CAMINHO_PARCIAL_ASSEMBLY_SNEBUR_ACESSO_DADOS = @"Snebur.Framework\src\AcessoDados\src\AcessoDados\bin\Debug\Snebur.AcessoDados.dll";
//        private const string CAMINHO_PARCIAL_ASSEMBLY_SNEBUR_ACESSO_DADOS_SERVIDOR = @"Snebur.Framework\src\AcessoDados\src\AcessoDados.Servidor\bin\Debug\Snebur.AcessoDados.Servidor.dll";
//        private const string CAMINHO_PARCIAL_ASSEMBLY_SNEBUR_ACESSO_DADOS_MIGRACAO = @"Snebur.Framework\src\AcessoDados\src\AcessoDados.Migracao\bin\Debug\Snebur.AcessoDados.Migracao.dll";

//        private const string CAMINHO_PARCIAL_ASSEMBLY_NEWTONSOFT_JSON = @"Snebur.Framework\src\Newtonsoft.Json\Newtonsoft.Json\bin\Debug\Net45\Newtonsoft.Json.dll";
//        private const string CAMINHO_PARCIAL_ASSEMBLY_NEWTONSOFT_JSON_ALTERADO = @"Snebur.Framework\src\Newtonsoft.Json.Alterado\bin\Debug\Newtonsoft.Json.Alterado.dll";

//        private static string _caminhoProjetos;

//        public const string ENCONTRAR = "<xsd:complexType mixed=\"true\" name=\"simpleFlowContentElement\">";

//        //private const string CAMINHO_PROJETOS_PADRAO = @"Projetos\TFS";

//        public static string CaminhoProjetos => ThreadUtil.RetornarValorComBloqueio(ref _caminhoProjetos, RetornarCaminhoProjetos);
//        public static string RetornarCaminhoProjetos()
//        {
//            var caminhoProejtos = ConfiguracaoGeral.Instance.CaminhoProjetos;
//            if (Directory.Exists(caminhoProejtos))
//            {
//                return ConfiguracaoGeral.Instance.CaminhoProjetos;
//            }

//            //var discos = DriveInfo.GetDrives();
//            //foreach (var disco in discos)
//            //{
//            //    var caminho = Path.Combine(disco.RootDirectory.FullName, CAMINHO_PROJETOS_PADRAO);
//            //    if (Directory.Exists(caminho))
//            //    {
//            //        return caminho;
//            //    }
//            //}


//            throw new Exception($"Não foi possível encontrar o diretórios padrão dos projetos {ConfiguracaoGeral.Instance.CaminhoProjetos}");


//        }

//        public static string CaminhoSchemaHTML5
//        {
//            get
//            {
//                return Path.Combine(CaminhosUtil.REPOSITORIO_SCHEMA_HTML, CaminhosUtil.ARQUIVO_HTML_5);
//            }
//        }

//        public static string CaminhoSchemaXHTML5
//        {
//            get
//            {
//                return Path.Combine(CaminhosUtil.REPOSITORIO_SCHEMA_HTML, CaminhosUtil.ARQUIVO_X_HTML_5);
//            }
//        }

//        public static string CaminhoAtributosTypescript
//        {
//            get
//            {
//                return Path.Combine(CaminhosUtil.CaminhoProjetos, CaminhosUtil.CAMINHO_PARCIAL_ATRIBUTOS_TYPESCRIPT);
//            }
//        }

//        public static string CaminhoControlesTypescript
//        {
//            get
//            {
//                return Path.Combine(CaminhosUtil.CaminhoProjetos, CaminhosUtil.CAMINHO_PARCIAL_CONTROLES_TYPESCRIPT);
//            }
//        }

//        public static string CaminhoComponentesApresentacaoTypescript
//        {
//            get
//            {
//                return Path.Combine(CaminhosUtil.CaminhoProjetos, CaminhosUtil.CAMINHO_PARCIAL_ELEMENTOS_APRESENTACAO_TYPESCRIPT);
//            }
//        }

//        public static string[] CaminhoAssemblySnebur
//        {
//            get
//            {
//                //if (ProjetoUtil.IsProjetoZyoncore)
//                //{
//                var caminhoZyoncore = Path.Combine(CaminhosUtil.CaminhoProjetos, CAMINHO_PARCIAL_ASSEMBLY_ZYONCORE);
//                var caminhoSnebur = Path.Combine(CaminhosUtil.CaminhoProjetos, CAMINHO_PARCIAL_ASSEMBLY_SNEBUR);
//                //}
//                return new string[] { caminhoZyoncore , caminhoSnebur };
//            }
//        }

//        public static string[] CaminhoAssemblySneburComunicao
//        {
//            get
//            {
//                return new string[] { Path.Combine(CaminhosUtil.CaminhoProjetos, CAMINHO_PARCIAL_ASSEMBLY_SNEBUR_COMUNICACAO) };
//            }
//        }

//        public static string[] CaminhoAssemblySneburAcessoDados
//        {
//            get
//            {
//                return new string[] { Path.Combine(CaminhosUtil.CaminhoProjetos, CAMINHO_PARCIAL_ASSEMBLY_SNEBUR_ACESSO_DADOS) };
//            }
//        }

//        public static string[] CaminhoAssemblySneburAcessoDadosServidor
//        {
//            get
//            {
//                return new string[] { Path.Combine(CaminhosUtil.CaminhoProjetos, CAMINHO_PARCIAL_ASSEMBLY_SNEBUR_ACESSO_DADOS_SERVIDOR) };
//            }
//        }

//        public static string[] CaminhoAssemblySneburAcessoDadosMigracao
//        {
//            get
//            {
//                return new string[] { Path.Combine(CaminhosUtil.CaminhoProjetos, CAMINHO_PARCIAL_ASSEMBLY_SNEBUR_ACESSO_DADOS_MIGRACAO) };
//            }
//        }

//        public static string[] CaminhoAssemblyNewtonsoftJson => new string[] { Path.Combine(CaminhosUtil.CaminhoProjetos, CAMINHO_PARCIAL_ASSEMBLY_NEWTONSOFT_JSON) };

//        public static string[] CaminhoAssemblyNewtonsoftJsonAlterado => new string[] { Path.Combine(CaminhosUtil.CaminhoProjetos, CAMINHO_PARCIAL_ASSEMBLY_NEWTONSOFT_JSON_ALTERADO) };

//        public static string[] CaminhoAssemblySneburDepuracao
//        {
//            get
//            {
//                return new string[] { Path.Combine(CaminhosUtil.CaminhoProjetos, CAMINHO_PARCIAL_ASSEMBLY_SNEBUR_DEPURACAO) };
//            }
//        }


//        public static string RetornarNomeSemExtensao(string caminho, bool removerTodas = false)
//        {
//            var nomeSemExtensao = Path.GetFileNameWithoutExtension(caminho);
//            if (removerTodas)
//            {
//                while (nomeSemExtensao != Path.GetFileNameWithoutExtension(nomeSemExtensao))
//                {
//                    nomeSemExtensao = Path.GetFileNameWithoutExtension(nomeSemExtensao);
//                }
//            }
//            return nomeSemExtensao;
//        }

//        public static void Clear()
//        {
//            _caminhoProjetos = null;
//        }

//    }
//}
