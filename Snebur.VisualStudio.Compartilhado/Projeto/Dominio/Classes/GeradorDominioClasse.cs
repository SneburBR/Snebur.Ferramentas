using System;
using System.Collections.Generic;
using System.Linq;
using Snebur.Linq;
using Snebur.VisualStudio.Reflexao;

namespace Snebur.VisualStudio
{
    public class GeradorDominioClasse : BaseGeradorDominio
    {
        private HashSet<Type> TiposBaseDominio { get; set; }

        public GeradorDominioClasse(ConfiguracaoProjetoDominio configuracaoDominio,
                                    string caminhoProjeto,
                                    List<Type> todosTipos,
                                    string nomeArquivo) :
                                    base(configuracaoDominio, caminhoProjeto, todosTipos, nomeArquivo)
        {
            this.TiposBaseDominio = this.TiposDominio;
        }

        protected override HashSet<Type> RetornarRetornarTiposDominios()
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
                var todosTipos = grupoNamespace.ToList();

                var tiposOrdenados = this.RetornarTiposOrdenados(todosTipos, AjudanteAssembly.TiposBaseDominio).ToHashSet();

                TipoUtil.RemoverTipo(tiposOrdenados, AjudanteAssembly.TipoBaseDominio);
                TipoUtil.RemoverTipo(tiposOrdenados, AjudanteAssembly.TipoEntidade);
                TipoUtil.RemoverTipo(tiposOrdenados, AjudanteAssembly.TipoBaseViewModel);

                var diTodosTipos = todosTipos.ToDictionary(x => x.Name);

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

        private HashSet<Type> RetornarTiposBaseDominio()
        {
            var tiposBaseDominio = this.RetornarSubTipos(AjudanteAssembly.TipoBaseDominio).ToList();
            tiposBaseDominio = TipoUtil.IgnorarAtributo(tiposBaseDominio, AjudanteAssembly.NomeTipoIgnorarClasseTS);
            //.OrderBy(x=> x.Name).ToList();
            return tiposBaseDominio.ToHashSet();
        }

        //private Type RetornarTipoNaoAbstrato(Type tipoBaseDominio)
        //{
        //    if (tipoBaseDominio.IsAbstract)
        //    {
        //        var tipoNaoAbstrato = this.TiposBaseDominio.Where(x => x.IsSubclassOf(tipoBaseDominio)).OrderBy(x => x.GetConstructors().Length == 0 ? -1 : x.GetConstructors().Min(c => c.GetParameters().Length)).FirstOrDefault(); ;
        //        if (tipoNaoAbstrato == null)
        //        {

        //            if (tipoBaseDominio.Name == nameof(Snebur.Servico.BaseInformacaoAdicionalServicoCompartilhado))
        //            {
        //                return null;
        //            }
        //            throw new Exception($"Não foi possivel encontrar um tipo especializado não abstrato de {tipoNaoAbstrato.Name}");
        //        }
        //        return tipoNaoAbstrato;
        //    }
        //    return tipoBaseDominio;

        //}


    }
}
