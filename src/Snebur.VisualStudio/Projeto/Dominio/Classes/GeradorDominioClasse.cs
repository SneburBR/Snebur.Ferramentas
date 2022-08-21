using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snebur;
using System.Reflection;
using Snebur.VisualStudio.Utilidade;
using Snebur.VisualStudio.Reflexao;
using Snebur.VisualStudio.Projeto.Dominio.Estrutura;

namespace Snebur.VisualStudio.Projeto.Dominio
{
    public class GeradorDominioClasse : BaseGeradorDominio
    {
        private List<Type> TiposBaseDominio { get; set; }

        public GeradorDominioClasse(ConfiguracaoProjetoDominio configuracaoDominio,
                                    List<Type> todosTipos,
                                    string nomeArquivo) : base(configuracaoDominio,  todosTipos, nomeArquivo)
        {
            this.TiposBaseDominio = this.TiposDominio;
        }

        protected override List<Type> RetornarRetornarTiposDominios()
        {
            return this.RetornarTiposBaseDominio();
        }

        protected override string RetornarConteudoTypeScript()
        {
            var sb = new System.Text.StringBuilder();
            var gruposNamespace = this.TiposBaseDominio.GroupBy(x => x.Namespace).OrderBy(x => x.Key).ToList();

            foreach (var grupoNamespace in gruposNamespace)
            {
                var _namespace = grupoNamespace.Key;
                var tiposOrdenados = this.RetornarTiposOrdenados(grupoNamespace.ToList(), AjudanteAssembly.TipoBaseDominio);

                TipoUtil.RemoverTipo(tiposOrdenados, AjudanteAssembly.TipoBaseDominio);
                TipoUtil.RemoverTipo(tiposOrdenados, AjudanteAssembly.TipoEntidade);

                sb.AppendLine("");
                sb.AppendLine(String.Format("namespace {0}", _namespace));
                sb.AppendLine("{");

                foreach (var tipoBaseDominio in tiposOrdenados)
                {
                    var estruturaClasse = new EstruturaClasseDominio(tipoBaseDominio);

                    sb.AppendLine("");
                    foreach (var linha in estruturaClasse.RetornarLinhasTypeScriptClasse())
                    {
                        sb.AppendLine(String.Format("{0}{1}", TAB, linha));
                    }
                    sb.AppendLine("");
                }
                sb.AppendLine("}");
            }

            return sb.ToString();


        }

        protected override string RetornarNomeArquivoDestino()
        {
            return "Classes";
        }

        private List<Type> RetornarTiposBaseDominio()
        {
            var tiposBaseDominio = this.RetornarSubTipos(AjudanteAssembly.TipoBaseDominio).ToList();
            tiposBaseDominio = TipoUtil.IgnorarAtributo(tiposBaseDominio, AjudanteAssembly.NomeTipoIgnorarClasseTS);
            return tiposBaseDominio;
        }


    }
}
