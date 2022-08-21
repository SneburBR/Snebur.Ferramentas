using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Snebur.Comunicacao;
using Snebur.Dominio;
using Snebur.Dominio.Atributos;
using Snebur.Utilidade;

namespace Snebur.VisualStudio
{
    public static class AjudanteAssembly
    {
        public const string NAMESPACE_SNEBUR = "Snebur";
        public const string NAMESPACE_SNEBUR_DOMINIO = "Snebur.Dominio";
        public const string NAMESPACE_SNEBUR_COMUNICACAO = "Snebur.Comunicacao";
        public const string NAMESPACE_SNEBUR_ACESSO_DADOS = "Snebur.AcessoDados";
        public const string NOME_TIPO_BASE_ATRIBUTO_VALIDACAO = "BaseAtributoValidacao";

        private static Dictionary<string, string[]> _assemblyCaminhos;


        public static readonly string NomeTipoBaseAtributoValidacao = nameof(BaseAtributoValidacao);
        public static readonly string NomeTipoBaseAtributoValidacaoAsync = nameof(BaseAtributoValidacaoAsync);


        public static readonly string NomeTipoBaseAtributoDominio = nameof(BaseAtributoDominio);
        public static readonly string NomeTipoBasePropriedadeComputada = nameof(BasePropriedadeComputadaAttribute);
        public static readonly string NomeTipoBaseDominio = nameof(BaseDominio);
        public static readonly string NomeTipoBaseViewModel = nameof(BaseViewModel);
        public static readonly string NomeTipoEntidade = nameof(Entidade);
        public static readonly string NomeTipoBaseTipoComplexo = nameof(BaseTipoComplexo);
        public static readonly string NomeTipoInterfaceIEntidade = nameof(IEntidade);
        public static readonly string NomeTipoInterfaceIImagem = nameof(IImagem);
        public static readonly string NomeTipoInterfaceIArquivo = nameof(IArquivo);
        public static readonly string NomeTipoInterfaceIBaseServico = nameof(IBaseServico);

        public static readonly string NomeTipoIgnorarClasseTS = nameof(IgnorarClasseTSAttribute);
        public static readonly string NomeTipoIgnorarConstrutorTS = nameof(IgnorarConstrutorTSAttribute);
        public static readonly string NomeTipoIgnorarEnumTS = nameof(IgnorarEnumTSAttribute);
        public static readonly string NomeTipoIgnorarInterfaceTS = nameof(IgnorarInterfaceTSAttribute);
        public static readonly string NomeTipoIgnorarGlobalizacao = nameof(IgnorarGlobalizacaoAttribute);
        public static readonly string NomeTipoIgnorarPropriedadeTS = nameof(IgnorarPropriedadeTSAttribute);
        public static readonly string NomeTipoIgnorarPropriedadeTSReflexao = nameof(IgnorarPropriedadeTSReflexaoAttribute);
        public static readonly string NomeTipoIgnorarMetodoTS = nameof(IgnorarMetodoTSAttribute);
        public static readonly string NomeTipoIgnorarTSReflexao = nameof(IgnorarTSReflexaoAttribute);
        public static readonly string NomeTipoParametroOpcionalTS = nameof(ParametroOpcionalTSAttribute);
        public static readonly string NomeTipoIgnorarAtributoTS = nameof(IgnorarAtributoTSAttribute);
        public static readonly string NomeTipoIgnorarConstanteTS = nameof(IgnorarConstanteTSAttribute);
        public static readonly string NomeTipoConstantesTS = nameof(ConstantesTSAttribute);

        public static readonly string NomeTipoAtributoRotulo = nameof(RotuloAttribute);
        public static readonly string NomeTipoAtributoRotuloVSIntelliSenseAttribute = nameof(RotuloVSIntelliSenseAttribute);
        public static readonly string NomeTipoAtributoCriarInstanciaTS = nameof(CriarInstanciaTSAttribute);
        public static readonly string NomeTipoAtributoMensagemValidacao = nameof(MensagemValidacaoAttribute);
        public static readonly string NomeTipoAtributoProprieadeInterface = nameof(PropriedadeInterfaceAttribute);
        public static readonly string NomePropriedadeChavePrimariaEntidade = ReflexaoUtil.RetornarNomePropriedade<Entidade>(x => x.Id);

        public static Type TipoBaseAtributoValidacao { get; private set; }
        public static Type TipoBaseAtributoValidacaoAsync { get; private set; }
        public static Type TipoBaseAtributoDominio { get; private set; }
        public static Type TipoBasePropriedadeComputada { get; private set; }
        public static Type TipoBaseDominio { get; set; }
        public static Type TipoBaseViewModel { get; set; }

        public static Type[] TiposBaseDominio { get; private set; }

        public static Type TipoEntidade { get; set; }
        public static Type TipoBaseTipoComplexo { get; private set; }
        public static Type TipoInterfaceIEntidade { get; private set; }
        public static Type TipoInterfaceIImagem { get; private set; }
        public static Type TipoInterfaceIArquivo { get; private set; }
        public static Type TipoInterfaceIBaseServico { get; private set; }
        public static Type TipoIgnorarClasseTS { get; private set; }
        public static Type TipoIgnorarConstrutorTS { get; private set; }
        public static Type TipoIgnorarEnumTS { get; private set; }
        public static Type TipoInterfaceTS { get; private set; }
        public static Type TipoIgnorarGlobalizacao { get; private set; }
        public static Type TipoIgnorarPropriedadeTS { get; private set; }
        public static Type TipoIgnorarPropriedadeTSReflexao { get; private set; }
        public static Type TipoIgnorarMetedoTS { get; private set; }
        public static Type TipoIgnorarTSReflexao { get; private set; }
        public static Type TipoConstanteTS { get; private set; }
        public static Type TipoParametroOpcionalTS { get; private set; }
        public static Type TipoIgnorarAtributoTS { get; private set; }
        public static Type TipoAtributoRotulo { get; private set; }

        public static Type TipoAtributoRotuloVSIntelliSense { get; private set; }

        public static Type TipoAtributoProprieadeInterface { get; private set; }
        public static Type TipoAtributoCriarInstanciaTS { get; private set; }
        public static Type TipoMensagemValidacaoAttribute { get; private set; }
        public static Type TipoConstantesTS { get; private set; }
        public static Type TipoIgnorarConstanteTS { get; private set; }


        public static PropertyInfo PropriedadeChavePrimariaEntidade { get; private set; }


        //public static Type TipoListaEntidadesDefinicao { get; set; }

        public static string CaminhoDllAssemblySneburDominio { get; private set; }

        public static List<Type> TiposSneburDominio { get; private set; }

        public static Assembly AssemblySneburDominio { get; private set; }

        public static bool Inicializado = false;

        public static void Inicializar(bool forcar = false)
        {
            if (!Inicializado || forcar)
            {
                AppDomain.CurrentDomain.AssemblyResolve += AjudanteAssembly.Assembly_AssemblyResolve;

                AjudanteAssembly.CaminhoDllAssemblySneburDominio = AjudanteAssembly.RetornarCaminhoDllAssemblySneburDominio();
                AjudanteAssembly.TiposSneburDominio = RetornarAssembly(AjudanteAssembly.CaminhoDllAssemblySneburDominio).GetAccessibleTypes().ToList();

                //var pa = AjudanteAssembly.TiposSneburDominio.Where(x => x.Name == "PropriedadeAlterada").Single();

                //AjudanteAssembly.TiposSneburDominio = typeof(Snebur.Dominio.BaseDominio).Assembly.GetTypes().ToList();

                AjudanteAssembly.TipoBaseAtributoValidacao = AjudanteAssembly.RetornarTipo(AjudanteAssembly.NomeTipoBaseAtributoValidacao);
                AjudanteAssembly.TipoBaseAtributoValidacaoAsync = AjudanteAssembly.RetornarTipo(AjudanteAssembly.NomeTipoBaseAtributoValidacaoAsync);
                AjudanteAssembly.TipoBaseAtributoDominio = AjudanteAssembly.RetornarTipo(AjudanteAssembly.NomeTipoBaseAtributoDominio);
                AjudanteAssembly.TipoBasePropriedadeComputada = AjudanteAssembly.RetornarTipo(AjudanteAssembly.NomeTipoBasePropriedadeComputada);
                AjudanteAssembly.TipoBaseDominio = AjudanteAssembly.RetornarTipo(AjudanteAssembly.NomeTipoBaseDominio);
                AjudanteAssembly.TipoBaseViewModel = AjudanteAssembly.RetornarTipo(AjudanteAssembly.NomeTipoBaseViewModel);

                AjudanteAssembly.TiposBaseDominio = new Type[] { TipoBaseDominio, TipoBaseViewModel };

                AjudanteAssembly.TipoEntidade = AjudanteAssembly.RetornarTipo(AjudanteAssembly.NomeTipoEntidade);
                AjudanteAssembly.TipoBaseTipoComplexo = AjudanteAssembly.RetornarTipo(AjudanteAssembly.NomeTipoBaseTipoComplexo);
                AjudanteAssembly.TipoInterfaceIEntidade = AjudanteAssembly.RetornarTipo(AjudanteAssembly.NomeTipoInterfaceIEntidade);
                AjudanteAssembly.TipoInterfaceIArquivo = AjudanteAssembly.RetornarTipo(AjudanteAssembly.NomeTipoInterfaceIArquivo);
                AjudanteAssembly.TipoInterfaceIImagem = AjudanteAssembly.RetornarTipo(AjudanteAssembly.NomeTipoInterfaceIImagem);
                AjudanteAssembly.TipoInterfaceIBaseServico = AjudanteAssembly.RetornarTipo(AjudanteAssembly.NomeTipoInterfaceIBaseServico);

                AjudanteAssembly.TipoIgnorarClasseTS = AjudanteAssembly.RetornarTipo(AjudanteAssembly.NomeTipoIgnorarClasseTS);
                AjudanteAssembly.TipoIgnorarConstrutorTS = AjudanteAssembly.RetornarTipo(AjudanteAssembly.NomeTipoIgnorarConstrutorTS);
                AjudanteAssembly.TipoIgnorarEnumTS = AjudanteAssembly.RetornarTipo(AjudanteAssembly.NomeTipoIgnorarEnumTS);
                AjudanteAssembly.TipoInterfaceTS = AjudanteAssembly.RetornarTipo(AjudanteAssembly.NomeTipoIgnorarInterfaceTS);
                AjudanteAssembly.TipoIgnorarGlobalizacao = AjudanteAssembly.RetornarTipo(AjudanteAssembly.NomeTipoIgnorarGlobalizacao);
                AjudanteAssembly.TipoIgnorarPropriedadeTS = AjudanteAssembly.RetornarTipo(AjudanteAssembly.NomeTipoIgnorarPropriedadeTS);
                AjudanteAssembly.TipoIgnorarPropriedadeTSReflexao = AjudanteAssembly.RetornarTipo(AjudanteAssembly.NomeTipoIgnorarPropriedadeTSReflexao);
                AjudanteAssembly.TipoIgnorarMetedoTS = AjudanteAssembly.RetornarTipo(AjudanteAssembly.NomeTipoIgnorarMetodoTS);
                AjudanteAssembly.TipoIgnorarTSReflexao = AjudanteAssembly.RetornarTipo(AjudanteAssembly.NomeTipoIgnorarTSReflexao);
                AjudanteAssembly.TipoParametroOpcionalTS = AjudanteAssembly.RetornarTipo(AjudanteAssembly.NomeTipoParametroOpcionalTS);
                AjudanteAssembly.TipoIgnorarAtributoTS = AjudanteAssembly.RetornarTipo(AjudanteAssembly.NomeTipoIgnorarAtributoTS);
                AjudanteAssembly.TipoConstantesTS = AjudanteAssembly.RetornarTipo(AjudanteAssembly.NomeTipoConstantesTS);
                AjudanteAssembly.TipoIgnorarConstanteTS = AjudanteAssembly.RetornarTipo(AjudanteAssembly.NomeTipoIgnorarConstanteTS);

                AjudanteAssembly.TipoAtributoRotulo = AjudanteAssembly.RetornarTipo(AjudanteAssembly.NomeTipoAtributoRotulo);
                AjudanteAssembly.TipoAtributoRotuloVSIntelliSense = AjudanteAssembly.RetornarTipo(AjudanteAssembly.NomeTipoAtributoRotuloVSIntelliSenseAttribute);
                AjudanteAssembly.TipoAtributoCriarInstanciaTS = AjudanteAssembly.RetornarTipo(AjudanteAssembly.NomeTipoAtributoCriarInstanciaTS);
                AjudanteAssembly.TipoAtributoProprieadeInterface = AjudanteAssembly.RetornarTipo(AjudanteAssembly.NomeTipoAtributoProprieadeInterface);


                AjudanteAssembly.TipoMensagemValidacaoAttribute = AjudanteAssembly.RetornarTipo(AjudanteAssembly.NomeTipoAtributoMensagemValidacao);


                AjudanteAssembly.PropriedadeChavePrimariaEntidade = AjudanteAssembly.TipoEntidade.GetProperties().Where(x => x.Name == AjudanteAssembly.NomePropriedadeChavePrimariaEntidade).Single();
                Inicializado = true;
            }


            //  AjudanteAssembly.TipoListaEntidadesDefinicao = AjudanteAssembly.TiposSneburDominio.Where(x => x.Name.StartsWith("ListaEntidade")).Single();
        }

        private static string RetornarCaminhoDllAssemblySneburDominio()
        {
            var caminhos = AssemblyCaminhos[AjudanteAssembly.NAMESPACE_SNEBUR];
            foreach (var caminho in caminhos)
            {
                if (File.Exists(caminho))
                {
                    return caminho;
                }
            }
            return caminhos.First();
        }

        public static Type RetornarTipo(string nomeTipo)
        {
            var tipos = AjudanteAssembly.TiposSneburDominio.Where(x => x.Name == nomeTipo).ToList();
            if (tipos.Count == 0)
            {
                throw new Exception(String.Format("O tipo {0} não foi encontrado", nomeTipo));
            }
            if (tipos.Count > 1)
            {
                throw new Exception(String.Format("Existe mais de um tipo com nome {0}", nomeTipo));
            }
            return tipos.Single();
        }


        public static Dictionary<string, string[]> AssemblyCaminhos => ThreadUtil.RetornarValorComBloqueio(ref _assemblyCaminhos, RetornarAssemblyCaminhos);

        public static Dictionary<string, string[]> RetornarAssemblyCaminhos()
        {
            return new Dictionary<string, string[]> {
                 { "Snebur",   CaminhosUtil.CaminhoAssemblySnebur },
                 { "Zyoncore",   CaminhosUtil.CaminhoAssemblySnebur },
                 { "Snebur.Comunicacao", CaminhosUtil.CaminhoAssemblySneburComunicao },
                 { "Snebur.AcessoDados", CaminhosUtil.CaminhoAssemblySneburAcessoDados },
                 { "Snebur.AcessoDados.Servidor", CaminhosUtil.CaminhoAssemblySneburAcessoDadosServidor },
                 { "Snebur.AcessoDados.Migracao", CaminhosUtil.CaminhoAssemblySneburAcessoDadosMigracao },
                 { "Snebur.Depuracao", CaminhosUtil.CaminhoAssemblySneburDepuracao},
                 { "Newtonsoft.Json", CaminhosUtil.CaminhoAssemblyNewtonsoftJson },
                 { "Newtonsoft.Json.Alterado", CaminhosUtil.CaminhoAssemblyNewtonsoftJsonAlterado }
             };
        }

        public static Assembly Assembly_AssemblyResolve(object sender, ResolveEventArgs e)
        {
            var nome = e.Name.Split(", ".ToCharArray()).First().Trim();


            if (!AjudanteAssembly.AssemblyCaminhos.ContainsKey(nome))
            {
                if (!(nome.StartsWith("System") ||
                      nome.StartsWith("PresentationUI") ||
                      nome.StartsWith("Microsoft")))
                {
                    var erro = new Exception(String.Format("Não foi encontrado o caminho para {0}", nome));
                    LogVSUtil.LogErro(erro);
                }
                return null;

            }

            var caminhosDll = AjudanteAssembly.AssemblyCaminhos[nome];
            foreach (var caminhoDll in caminhosDll)
            {
                if (File.Exists(caminhoDll))
                {
                    return RetornarAssembly(caminhoDll);
                }
            }

            var erroArquivoNaoExiste = new FileNotFoundException($"Não foi possível o assembly, {String.Join("\r\n ", caminhosDll)}");
            LogVSUtil.LogErro(erroArquivoNaoExiste);
            return null;

        }
         
        private static Dictionary<string, Assembly> ArmazenamentoDllCarregadas = new Dictionary<string, Assembly>();
        private static object bloqueio = new object();

        public static Assembly RetornarAssembly(string caminhoProjeto, string nomeAssembly)
        {
            var caminhoDll = RetornarCaminhoAssembly(caminhoProjeto, nomeAssembly);
            return RetornarAssembly(caminhoDll);
        }

        public static Assembly RetornarAssembly(string caminhoDll)
        {
            lock (bloqueio)
            {
                var caminhoReflexao = AjudanteAssembly.RetornarCaminhoAssemblyReflexao(caminhoDll).ToLower();

                if (!File.Exists(caminhoReflexao))
                {
                    throw new Exception(String.Format("Arquivo não encontrado {0}", caminhoReflexao));
                }

                if (AjudanteAssembly.ArmazenamentoDllCarregadas.ContainsKey(caminhoReflexao))
                {
                    return ArmazenamentoDllCarregadas[caminhoReflexao];
                }
                else
                {

                    var bytes = File.ReadAllBytes(caminhoReflexao);
                    var assemly = Assembly.Load(bytes);
                    return assemly;

                }
            }

        }

        public static string RetornarCaminhoAssembly(ConfiguracaoProjeto configuracaoProjeto)
        {
            return RetornarCaminhoAssembly(configuracaoProjeto.CaminhoProjeto,
                                          configuracaoProjeto.NomeAssembly);
        }
        private static string RetornarCaminhoAssembly(string caminhoProjeto,
                                                    string nomeAssembly )
        {
            //var assembly = projeto.Properties.Cast<Property>().
            //                                         FirstOrDefault(x => x.Name == "AssemblyName");
            //var nomeAssembly = assembly?.Value ?? projeto.Name;
            var nomeArquivoDll = $"{nomeAssembly}.dll";
            var nomeArquivoExe = $"{nomeAssembly}.exe";
            //var caminhoProjeto = new FileInfo(projeto.FileName).Directory.FullName;
            var diretorioBin = Path.Combine(caminhoProjeto, "bin");
            var diretorioDebug = Path.Combine(diretorioBin, "Debug");
            var caminhoDll = Path.Combine(diretorioDebug, nomeArquivoDll);
            var caminhoExe = Path.Combine(diretorioDebug, nomeArquivoExe);
            if (File.Exists(caminhoDll))
            {
                return  caminhoDll;
            }
            if (File.Exists(caminhoExe))
            {
                return caminhoExe;
            }

            var caminhoBinDll = Path.Combine(diretorioBin, nomeArquivoDll);
            if (File.Exists(caminhoBinDll))
            {
                return caminhoBinDll;
            }

            throw new FileNotFoundException($"Não foi encontrada a DLL debug do projeto {nomeAssembly} - caminho : {caminhoDll} ");
        }
         
        private static string RetornarCaminhoAssemblyReflexao(string caminhoDll)
        {
            var caminhoReflexao = caminhoDll.Replace("\\Debug\\", "\\Reflexao\\");
            DiretorioUtil.ExcluirTodosArquivo(caminhoReflexao, true);
            var fi = new FileInfo(caminhoDll);
            var diretorioDebug = fi.Directory.FullName;
            var diretorioRefexao = Path.Combine(fi.Directory.Parent.FullName, "Reflexao");
            DiretorioUtil.CopiarTodosArquivo(diretorioDebug, diretorioRefexao);
            return caminhoReflexao;
        }

        public static void Clear()
        {
            ArmazenamentoDllCarregadas.Clear();
            _assemblyCaminhos = null;
            CaminhosUtil.Clear();
        }

      
    }
}