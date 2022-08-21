using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snebur;
using Snebur.Utilidade;
using Snebur.Dominio;
using Snebur.VisualStudio.Reflexao;
using Snebur.VisualStudio.Projeto.Dominio.Estrutura;

namespace Snebur.VisualStudio.Projeto.Dominio
{
    public class GeradorDominioInterface : BaseGeradorDominio
    {

        public GeradorDominioInterface(ConfiguracaoProjetoDominio configuracaoDominio,
                                       List<Type> todosTipos,
                                       string nomeProjeto) : base(configuracaoDominio, todosTipos, nomeProjeto)
        {

        }

        protected override string RetornarConteudoTypeScript()
        {
            var sb = new System.Text.StringBuilder();
            var gruposNamespace = this.TiposDominio.GroupBy(x => x.Namespace).OrderBy(x => x.Key).ToList();

            foreach (var grupoNamespace in gruposNamespace)
            {
                var _namespace = grupoNamespace.Key;
                sb.AppendLine("");
                sb.AppendLine(String.Format("namespace {0}", _namespace));
                sb.AppendLine("{");
                var tipos = grupoNamespace.ToList();
                foreach (var tipoBaseDominio in tipos)
                {
                    var estruturaInterface = new EstruturaInterface(tipoBaseDominio);
                    sb.AppendLine("");
                    foreach (var linha in estruturaInterface.RetornarLinhasTypeScriptInterface())
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
            return "Interfaces";
        }

        protected override List<Type> RetornarRetornarTiposDominios()
        {
            var tiposInteface = this.TodosTipo.Where(x => x.IsInterface).ToList();
            tiposInteface = TipoUtil.IgnorarAtributo(tiposInteface, AjudanteAssembly.NomeTipoIgnorarInterfaceTS);
            return tiposInteface;
        }
    }
}
