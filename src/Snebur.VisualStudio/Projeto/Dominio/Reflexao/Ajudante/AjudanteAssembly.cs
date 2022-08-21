using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snebur;
using System.Reflection;
using System.IO;
using Snebur.VisualStudio.Utilidade;
using System.Security.Policy;
using Snebur.Utilidade;

namespace Snebur.VisualStudio
{
    public static class AjudanteAssembly
    {
        public const string NamespaceSneburDominio = "Snebur.Dominio";

        public const string NomeTipoBaseAtributoValidacaoAsync = "BaseAtributoValidacaoAsync";
        public const string NomeTipoBaseAtributoValidacao = "BaseAtributoValidacao";
        public const string NomeTipoBaseAtributoDominio = "BaseAtributoDominio";
        public const string NomeTipoBaseDominio = "BaseDominio";
        public const string NomeTipoEntidade = "Entidade";
        public const string NomeTipoBaseTipoComplexo = "BaseTipoComplexo";
        public const string NomeTipoInterfaceIEntidade = "IEntidade";
        public const string NomeTipoInterfaceIImagem = "IImagem";
        public const string NomeTipoInterfaceIArquivo = "IArquivo";

        public const string NomeTipoIgnorarClasseTS = "IgnorarClasseTSAttribute";
        public const string NomeTipoIgnorarConstrutorTS = "IgnorarConstrutorTSAttribute";
        public const string NomeTipoIgnorarEnumTS = "IgnorarEnumTSAttribute";
        public const string NomeTipoIgnorarInterfaceTS = "IgnorarInterfaceTSAttribute";
        public const string NomeTipoIgnorarGlobalizacao = "IgnorarGlobalizacaoAttribute";
        public const string NomeTipoIgnorarPropriedadeTS = "IgnorarPropriedadeTSAttribute";
        public const string NomeTipoIgnorarPropriedadeTSReflexao = "IgnorarPropriedadeTSReflexaoAttribute";
        public const string NomeTipoIgnorarMetodoTS = "IgnorarMetodoTSAttribute";
        public const string NomeTipoIgnorarTSReflexao = "IgnorarTSReflexaoAttribute";
        public const string NomeTipoParametroOpcionalTS = "ParametroOpcionalTSAttribute";
        public const string NomeTipoIgnorarAtributoTS = "IgnorarAtributoTSAttribute";
        public const string NomeTipoIgnorarConstanteTS = "IgnorarConstanteTSAttribute";
        public const string NomeTipoConstantesTS = "ConstantesTSAttribute";


        public const string NomeTipoAtributoRotulo = "RotuloAttribute";
        public const string NomeTipoAtributoCriarInstanciaTS = "CriarInstanciaTSAttribute";
        public const string NomeTipoAtributoMensagemValidacao = "MensagemValidacaoAttribute";
        public const string NomeTipoAtributoProprieadeInterface = "PropriedadeInterfaceAttribute";
        public const string NomePropriedadeChavePrimariaEntidade = "Id";


        public static Type TipoBaseAtributoValidacaoAsync { get; set; }
        public static Type TipoBaseAtributoDominio { get; set; }
        public static Type TipoBaseDominio { get; set; }
        public static Type TipoEntidade { get; set; }
        public static Type TipoBaseTipoComplexo { get; set; }
        public static Type TipoInterfaceIEntidade { get; set; }
        public static Type TipoInterfaceIImagem { get; set; }
        public static Type TipoInterfaceIArquivo { get; set; }

        public static Type TipoIgnorarClasseTS { get; set; }
        public static Type TipoIgnorarConstrutorTS { get; set; }
        public static Type TipoIgnorarEnumTS { get; set; }
        public static Type TipoInterfaceTS { get; set; }
        public static Type TipoIgnorarGlobalizacao { get; set; }
        public static Type TipoIgnorarPropriedadeTS { get; set; }
        public static Type TipoIgnorarPropriedadeTSReflexao { get; set; }
        public static Type TipoIgnorarMetedoTS { get; set; }
        public static Type TipoIgnorarTSReflexao { get; set; }
        public static Type TipoConstanteTS { get; set; }
        public static Type TipoParametroOpcionalTS { get; set; }
        public static Type TipoIgnorarAtributoTS { get; set; }
        public static Type TipoAtributoRotulo { get; set; }
        public static Type TipoAtributoProprieadeInterface { get; set; }
        public static Type TipoAtributoCriarInstanciaTS { get; set; }
        public static Type TipoMensagemValidacaoAttribute { get; set; }
        public static Type TipoConstantesTS { get; set; }
        public static Type TipoIgnorarConstanteTS { get; set; }


        public static PropertyInfo PropriedadeChavePrimariaEntidade { get; set; }


        //public static Type TipoListaEntidadesDefinicao { get; set; }

        public static string CaminhoDllAssemblySneburDominio { get; set; }

        public static List<Type> TiposSneburDominio { get; set; }

        public static Assembly AssemblySneburDominio { get; set; }


        public static void Inicializar()
        {

            AppDomain.CurrentDomain.AssemblyResolve += AjudanteAssembly.Assembly_AssemblyResolve;

            AjudanteAssembly.CaminhoDllAssemblySneburDominio = AssemblyCaminhos[AjudanteAssembly.NamespaceSneburDominio];

            AjudanteAssembly.TiposSneburDominio = RetornarAssembly(AjudanteAssembly.CaminhoDllAssemblySneburDominio).GetTypes().ToList();
            AjudanteAssembly.TipoBaseAtributoValidacaoAsync = AjudanteAssembly.RetornarTipo(AjudanteAssembly.NomeTipoBaseAtributoValidacaoAsync);
            AjudanteAssembly.TipoBaseAtributoDominio = AjudanteAssembly.RetornarTipo(AjudanteAssembly.NomeTipoBaseAtributoDominio);
            AjudanteAssembly.TipoBaseDominio = AjudanteAssembly.RetornarTipo(AjudanteAssembly.NomeTipoBaseDominio);
            AjudanteAssembly.TipoEntidade = AjudanteAssembly.RetornarTipo(AjudanteAssembly.NomeTipoEntidade);
            AjudanteAssembly.TipoBaseTipoComplexo = AjudanteAssembly.RetornarTipo(AjudanteAssembly.NomeTipoBaseTipoComplexo);
            AjudanteAssembly.TipoInterfaceIEntidade = AjudanteAssembly.RetornarTipo(AjudanteAssembly.NomeTipoInterfaceIEntidade);
            AjudanteAssembly.TipoInterfaceIArquivo = AjudanteAssembly.RetornarTipo(AjudanteAssembly.NomeTipoInterfaceIArquivo);
            AjudanteAssembly.TipoInterfaceIImagem = AjudanteAssembly.RetornarTipo(AjudanteAssembly.NomeTipoInterfaceIImagem);

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
            AjudanteAssembly.TipoAtributoCriarInstanciaTS = AjudanteAssembly.RetornarTipo(AjudanteAssembly.NomeTipoAtributoCriarInstanciaTS);
            AjudanteAssembly.TipoAtributoProprieadeInterface = AjudanteAssembly.RetornarTipo(AjudanteAssembly.NomeTipoAtributoProprieadeInterface);


            AjudanteAssembly.TipoMensagemValidacaoAttribute = AjudanteAssembly.RetornarTipo(AjudanteAssembly.NomeTipoAtributoMensagemValidacao);


            AjudanteAssembly.PropriedadeChavePrimariaEntidade = AjudanteAssembly.TipoEntidade.GetProperties().Where(x => x.Name == AjudanteAssembly.NomePropriedadeChavePrimariaEntidade).Single();
            //  AjudanteAssembly.TipoListaEntidadesDefinicao = AjudanteAssembly.TiposSneburDominio.Where(x => x.Name.StartsWith("ListaEntidade")).Single();
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

        public static Dictionary<string, string> AssemblyCaminhos
        {
            get
            {
                var dicionario = new Dictionary<string, string>();

                dicionario.Add("Snebur", @"E:\Projetos\TFS\Snebur.Framework\Snebur\bin\Debug\Snebur.dll");

                dicionario.Add(AjudanteAssembly.NamespaceSneburDominio, @"E:\Projetos\TFS\Snebur.Framework\Snebur\bin\Debug\Snebur.dll");

                dicionario.Add("Snebur.Comunicacao", @"E:\Projetos\TFS\Snebur.Framework\Snebur.Comunicacao\bin\Debug\Snebur.Comunicacao.dll");
                dicionario.Add("Snebur.Comunicacao.Cliente", @"E:\Projetos\TFS\Snebur.Framework\Snebur.Comunicacao.Cliente\bin\Debug\Snebur.Comunicacao.Cliente.dll");
                dicionario.Add("Snebur.AcessoDados", @"E:\Projetos\TFS\Snebur.Framework\Snebur.AcessoDados\bin\Debug\Snebur.AcessoDados.dll");

                dicionario.Add("Snebur.Monitor.Entidades", @"E:\Projetos\TFS\Snebur.Monitor\Snebur.Monitor.Entidades\bin\Debug\Snebur.Monitor.Entidades.dll");

                return dicionario;
            }
        }

        public static Assembly Assembly_AssemblyResolve(object sender, ResolveEventArgs e)
        {

            var nome = e.Name.Split(", ".ToCharArray()).First().Trim();
            if (!AjudanteAssembly.AssemblyCaminhos.ContainsKey(nome))
            {
                if (!(nome.StartsWith("System") || nome.StartsWith("PresentationUI") || nome.StartsWith("Microsoft")))
                {
                    var erro = new Exception(string.Format("Não foi encontrado o caminho para {0}", nome));
                     LogMensagemUtil.LogErro(erro);
                }
                return null;

            }

            var caminhoDll = AjudanteAssembly.AssemblyCaminhos[nome];
            if (!File.Exists(caminhoDll))
            {
                var erro = new FileNotFoundException(string.Format("Não foi possivel o assembly, {0}", caminhoDll));
                LogMensagemUtil.LogErro(erro);
                return null;
            }

            var assembly = RetornarAssembly(caminhoDll);
            return assembly;
        }


        private static Dictionary<string, Assembly> ArmazenamentoDllCarregadas = new Dictionary<string, Assembly>();
        private static object bloqueio = new object();

        public static Assembly RetornarAssembly(string caminhoAssembly)
        {
            lock (bloqueio)
            {
                var caminhoReflexao = AjudanteAssembly.RetornarCaminhoAssemblyReflexao(caminhoAssembly).ToLower();

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
                    return Assembly.Load(bytes);

                    //AjudanteAssembly.AppDomainReflexao.Load(xxx.GetName());
                    //var dllCarregada = AjudanteAssembly.AppDomainReflexao.Load(bytes);
                    //AjudanteAssembly.ArmazenamentoDllCarregadas.Add(caminhoDll, dllCarregada);
                    //return dllCarregada;
                }
            }

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

        public static void Dispensar()
        {
            ArmazenamentoDllCarregadas.Clear();
        }

    }
}