using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Snebur.Linq;

namespace Snebur.VisualStudio
{
    public class DeclaracaoTiposUI
    {

        private const string NOME_TIPO_BASE_UI_ELEMENTO = "BaseUIElemento";
        private const string NOME_TIPO_BASE_VIEW_MODEL = "BaseViewModel";

        public List<BaseArquivoTypeScript> ArquivosTypeScriptOrdenados { get; private set; } = new List<BaseArquivoTypeScript>();
        private Dictionary<string, ArquivoTypeScript> DicionariosArquivosTypeScript { get; } = new Dictionary<string, ArquivoTypeScript>();

        private string NamespaceReflexao { get; }
        public DeclaracaoTiposUI(List<BaseArquivoTypeScript> arquivosTypeScriptOrdenados,
                              Dictionary<string, ArquivoTypeScript> dicionariosArquivosTypeScript,
                              string namespaceReflexao)
        {
            this.ArquivosTypeScriptOrdenados = arquivosTypeScriptOrdenados;
            this.DicionariosArquivosTypeScript = dicionariosArquivosTypeScript;
            this.NamespaceReflexao = namespaceReflexao;
        }

        public string RetornarDeclaracao()
        {
            var sb = new StringBuilder();

            var arquivosBaseUIElemento = this.RetornarArquivosTiposBaseUIElemento();
            var arquivosTipoHtmlReferencia = this.ArquivosTypeScriptOrdenados.OfType<ArquivoTypeScript>().Where(x => x.IsTipoHtmlReferencia).ToHashSet();
            var arquivosBaseViewModel = this.RetornarArquivosTiposViewModel();
            var arquivosTipoBaseHtmlReferencia = this.RetornarArquivosTipoBaseHtmlReferencia();

            arquivosTipoBaseHtmlReferencia.RemoveRange(arquivosBaseUIElemento);

            var todos = new List<ArquivoTypeScript>();
            todos.AddRange(arquivosBaseUIElemento);
            todos.AddRange(arquivosTipoHtmlReferencia);
            todos.AddRange(arquivosBaseViewModel);
            todos.AddRange(arquivosTipoBaseHtmlReferencia);


            foreach (var arquivoTypeScript in todos)
            {
                var declaracao = this.RetornarDeclaracaoCaminhoTipo(arquivoTypeScript);
                sb.AppendLine(declaracao.TrimEnd());
            }


            foreach (var arquivoTypeScript in arquivosBaseUIElemento)
            {
                var declaracao = this.RetornarDeclaracaoTipoUI(arquivoTypeScript, "TipoUI");
                sb.AppendLine(declaracao.TrimEnd());
            }

            foreach (var arquivoTypeScript in arquivosTipoHtmlReferencia)
            {
                var declaracao = this.RetornarDeclaracaoTipoUI(arquivoTypeScript, "TipoUIHtml");
                sb.AppendLine(declaracao.TrimEnd());
            }

            foreach (var arquivoTypeScript in arquivosBaseViewModel)
            {
                var declaracao = this.RetornarDeclaracaoTipoUI(arquivoTypeScript, "TipoBaseViewModel");
                sb.AppendLine(declaracao.TrimEnd());
            }

            foreach (var arquivoTypeScript in arquivosTipoBaseHtmlReferencia)
            {
                var declaracao = this.RetornarDeclaracaoTipoUI(arquivoTypeScript, "TipoUI");
                sb.AppendLine(declaracao.TrimEnd());
            }


            return sb.ToString();
        }

        private List<ArquivoTypeScript> RetornarArquivosTiposViewModel()
        {
            var arquivos = this.ArquivosTypeScriptOrdenados.OfType<ArquivoTypeScript>().Where(x => !x.IsTipoHtmlReferencia && this.IsTipoBaseViewModel(x)).ToList();
            return arquivos;
        }

        private List<ArquivoTypeScript> RetornarArquivosTipoBaseHtmlReferencia()
        {
            var arquivos = this.ArquivosTypeScriptOrdenados.OfType<ArquivoTypeScript>().
                                                            Where(x => !x.IsTipoAbstrato && !x.IsTipoHtmlReferencia && this.IsTipoBaseHtmlReferencia(x)).ToList();
            return arquivos;
        }
        

        private bool IsTipoBaseViewModel(ArquivoTypeScript arquivoTypeScript)
        {
            if (arquivoTypeScript.IsExisteTipoBase && arquivoTypeScript.IsExisteTipo)
            {
                if (arquivoTypeScript.NomeTipoBase == NOME_TIPO_BASE_VIEW_MODEL)
                {
                    return true;
                }

                if (arquivoTypeScript.IsExisteTipoBase && this.DicionariosArquivosTypeScript.ContainsKey(arquivoTypeScript.CaminhoTipoBase))
                {
                    return this.IsTipoBaseViewModel(this.DicionariosArquivosTypeScript[arquivoTypeScript.CaminhoTipoBase]);
                }
            }
            return false;
        }

        private bool IsTipoBaseHtmlReferencia(ArquivoTypeScript arquivoTypeScript, bool isTipoBase = false)
        {
            if (isTipoBase && arquivoTypeScript.IsTipoHtmlReferencia)
            {
                return true;
            }

            if (arquivoTypeScript.IsExisteTipoBase && arquivoTypeScript.IsExisteTipo)
            {
                if (arquivoTypeScript.IsExisteTipoBase && this.DicionariosArquivosTypeScript.ContainsKey(arquivoTypeScript.CaminhoTipoBase))
                {
                    return this.IsTipoBaseHtmlReferencia(this.DicionariosArquivosTypeScript[arquivoTypeScript.CaminhoTipoBase], true);
                }
            }
            return false;
        }



        private List<ArquivoTypeScript> RetornarArquivosTiposBaseUIElemento()
        {
            var arquivos = this.ArquivosTypeScriptOrdenados.OfType<ArquivoTypeScript>().Where(x => !x.IsTipoHtmlReferencia && this.IsTipoBaseUIElemento(x)).ToList();
            return arquivos;
        }

        private bool IsTipoBaseUIElemento(ArquivoTypeScript arquivoTypeScript)
        {
            if (arquivoTypeScript.IsExisteTipoBase && arquivoTypeScript.IsExisteTipo)
            {
                if (arquivoTypeScript.NomeTipo == NOME_TIPO_BASE_UI_ELEMENTO)
                {
                    return true;
                }

                if (arquivoTypeScript.IsExisteTipoBase && this.DicionariosArquivosTypeScript.ContainsKey(arquivoTypeScript.CaminhoTipoBase))
                {
                    return this.IsTipoBaseUIElemento(this.DicionariosArquivosTypeScript[arquivoTypeScript.CaminhoTipoBase]);
                }
            }
            return false;
        }

        private string RetornarDeclaracaoCaminhoTipo(ArquivoTypeScript arquivoTypeScript)
        {
            var sb = new StringBuilder();


            if (String.IsNullOrEmpty(arquivoTypeScript.CaminhoTipo))
            {
                throw new Erro("O caminho do tipo não está definido");
            }
            sb.AppendLine(String.Format("\t{0}.{1} = \"{0}\";", arquivoTypeScript.CaminhoTipo, ProjetoTypeScript.NOME_PROPRIEDADE_CAMINHO_TIPO));
            return sb.ToString();

        }
        private string RetornarDeclaracaoTipoUI(ArquivoTypeScript arquivoTypeScript, string nomeTipo)
        {

            var sb = new StringBuilder();
            var caminhoTipoBase = this.RetornarCaminhoTipoBase(arquivoTypeScript);

            sb.AppendLine($"\t$Reflexao.Tipos.Add({arquivoTypeScript.CaminhoTipo}.{ProjetoTypeScript.NOME_PROPRIEDADE_CAMINHO_TIPO}, " +
                          $"new {NamespaceReflexao}.{nomeTipo}(\"{arquivoTypeScript.NomeTipo}\", \"{arquivoTypeScript.Namespace}\"," +
                          $"{caminhoTipoBase}, " +
                          $"{arquivoTypeScript.IsTipoAbstrato.ToString().ToLower()}));");

            return sb.ToString();
        }



        private string RetornarCaminhoTipoBase(ArquivoTypeScript arquivoTypeScript)
        {
            if (arquivoTypeScript.IsExisteTipoBase)
            {
                return $"{arquivoTypeScript.CaminhoTipoBase}.{ProjetoTypeScript.NOME_PROPRIEDADE_CAMINHO_TIPO}";
            }
            return " null ";
        }
    }
}
