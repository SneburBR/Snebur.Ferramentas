using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using Snebur.Dominio.Atributos;
using Snebur.Utilidade;
using Snebur.VisualStudio.Reflexao;
using Snebur.VisualStudio.Utilidade;

namespace Snebur.VisualStudio
{
    public class GeradorDominioAtributo : BaseGeradorDominio
    {

        private HashSet<Type> TiposAtributos { get; set; }

        public GeradorDominioAtributo(ConfiguracaoProjetoDominio configuracaoDominio,
                                      string caminhoProjeto,
                                      List<Type> todosTipos,
                                      string nomeProjeto) :
                                      base(configuracaoDominio, caminhoProjeto, todosTipos, nomeProjeto)
        {
            //this.TipoBaseAtributoValidacaoAsync = AjudanteAssembly.TipoBaseAtributoValidacaoAsync;
            //this.TipoBaseAtributoDominio = AjudanteAssembly.TipoBaseAtributoDominio;
            this.TiposAtributos = this.TiposDominio;
        }

        protected override HashSet<Type> RetornarRetornarTiposDominios()
        {
            return this.RetornarTiposAtributos();
        }

        protected override string RetornarConteudoTypeScript()
        {

            var sb = new System.Text.StringBuilder();
            var gruposNamespace = this.TiposAtributos.GroupBy(x => x.Namespace).OrderBy(x => x.Key).ToList();

            foreach (var grupoNamespace in gruposNamespace)
            {
                var _namespace = grupoNamespace.Key;
                var tiposOrdenados = this.RetornarTiposOrdenados(grupoNamespace.ToList(),
                                                                AjudanteAssembly.TipoBaseAtributoDominio,
                                                                AjudanteAssembly.TipoBaseAtributoValidacaoAsync,
                                                                AjudanteAssembly.TipoBasePropriedadeComputada,
                                                                typeof(ValidationAttribute),
                                                                typeof(BaseValidacaoEntidadeAttribute),
                                                                typeof(CompareAttribute),
                                                                typeof(DataTypeAttribute),
                                                                typeof(MaxLengthAttribute),
                                                                typeof(MinLengthAttribute),
                                                                typeof(RangeAttribute),
                                                                typeof(RegularExpressionAttribute),
                                                                typeof(RequiredAttribute),
                                                                typeof(StringLengthAttribute),
                                                                typeof(ForeignKeyAttribute),
                                                                typeof(ScaffoldColumnAttribute));


                TipoUtil.RemoverTipo(tiposOrdenados, AjudanteAssembly.TipoBaseAtributoValidacaoAsync);
                TipoUtil.RemoverTipo(tiposOrdenados, AjudanteAssembly.TipoBaseAtributoValidacao);
                TipoUtil.RemoverTipo(tiposOrdenados, AjudanteAssembly.TipoBaseAtributoDominio);
                TipoUtil.RemoverTipo(tiposOrdenados, typeof(ValidationAttribute));

                sb.AppendLine("");
                sb.AppendLine(String.Format("namespace {0}", _namespace));
                sb.AppendLine("{");

                foreach (var tipoAtributo in tiposOrdenados)
                {
                    if (tipoAtributo.Name == "ChaveEstrangeiraAttribute")
                    {
                        var massa = "";
                        massa += "";
                    }

                    var estruturaAtributo = new EstruturaClasseAtributo(tipoAtributo);

                    sb.AppendLine("");
                    foreach (var linha in estruturaAtributo.RetornarLinhasTypeScriptClasse())
                    {
                        sb.AppendLine(String.Format("{0}{1}", TAB, linha));
                    }
                    sb.AppendLine("");

                    if (TipoUtil.TipoSubTipo(tipoAtributo, typeof(ValidationAttribute)))
                    {
                        this.AdicionarClasseValidacaoPartial(tipoAtributo);
                    }
                }
                sb.AppendLine("}");

            }
            return sb.ToString();

        }

        protected override string RetornarNomeArquivoDestino()
        {
            return "Atributos";
        }



        private void AdicionarClasseValidacaoPartial(Type tipoAtributo)
        {

            var nomeAtributo = TipoUtil.RetornarNomeAtributo(tipoAtributo);
            var nomeArquivo = String.Format("{0}.Partial.ts", nomeAtributo);
            var fi = new FileInfo(this.CaminhoArquivoDestino);
            var caminhoDiretorio = Path.Combine(fi.Directory.FullName, "Atributos", "Validacao");
            var caminhoArquivo = Path.Combine(caminhoDiretorio, nomeArquivo);
            var conteudoTypeScript = this.RetornarConteudoAtributoPartialTypeScript(tipoAtributo);

            if (!File.Exists(caminhoArquivo))
            {
                ArquivoUtil.SalvarArquivoTexto(caminhoArquivo, conteudoTypeScript, true);
            }
        }

        private string RetornarConteudoAtributoPartialTypeScript(Type tipoAtributo)
        {
            var modelo = RecursoUtil.RetornarRecursoTexto("Snebur.VisualStudio.Resources.Modelos.AtributoPartial.ts");
            var _namespace = TipoUtil.RetornarNameSpace(tipoAtributo);
            var nomeTipoAtributo = TipoUtil.RetornarNomeTipoTS(tipoAtributo);
            var novoConteudo = modelo.Replace("$namespace", _namespace);
            novoConteudo = novoConteudo.Replace("$nomeAtributo", nomeTipoAtributo);
            return novoConteudo;
        }
    }
}
