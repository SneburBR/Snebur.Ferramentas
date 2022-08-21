using System;
using System.Collections.Generic;
using System.Linq;
using Snebur.Linq;
using Snebur.VisualStudio.Reflexao;

namespace Snebur.VisualStudio
{
    public class GeradorDominioInterface : BaseGeradorDominio
    {

        public GeradorDominioInterface(ConfiguracaoProjetoDominio configuracaoDominio,
                                       string caminhoProjeto,
                                       List<Type> todosTipos,
                                       string nomeProjeto) : 
                                        base(configuracaoDominio, caminhoProjeto, todosTipos, nomeProjeto)
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

        protected override HashSet<Type> RetornarRetornarTiposDominios()
        {
            var tiposInteface = this.TodosTipo.Where(x => x.IsInterface).ToList();
            tiposInteface = TipoUtil.IgnorarAtributo(tiposInteface, AjudanteAssembly.NomeTipoIgnorarInterfaceTS);
            return tiposInteface.ToHashSet();
        }
    }
}
