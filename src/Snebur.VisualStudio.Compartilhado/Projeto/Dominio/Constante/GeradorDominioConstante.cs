using Snebur.Linq;
using Snebur.VisualStudio.Reflexao;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Snebur.VisualStudio
{
    public class GeradorDominioConstante : BaseGeradorDominio
    {

        public GeradorDominioConstante(ConfiguracaoProjetoDominio configuracaoDominio,
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
                    var estruturaInterface = new EstruturaConstantes(tipoBaseDominio);
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
            return "Constantes";
        }

        protected override HashSet<Type> RetornarRetornarTiposDominios()
        {
            var tipos = this.TodosTipo;
            return tipos.Where(x => TipoUtil.TipoPossuiAtributo(x, AjudanteAssembly.NomeTipoConstantesTS)).ToHashSet();
        }
    }
}
