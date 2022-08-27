using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snebur;
using Snebur.VisualStudio.Utilidade;
using System.Reflection;
using Snebur.VisualStudio.Reflexao;
using Snebur.Reflexao;
using Snebur.Linq;
using Snebur.VisualStudio;

namespace Snebur.VisualStudio
{
    public partial class GeradorDominioReflexao : BaseGeradorDominio
    {

        private HashSet<Type> TiposBaseDominio { get; set; }

        private HashSet<Type> TiposEntidade { get; set; }

        private HashSet<Type> TiposComplexo { get; set; }

        private HashSet<Type> TiposEnum { get; set; }

        private HashSet<Type> TiposAtributos { get; set; }


        public GeradorDominioReflexao(ConfiguracaoProjetoDominio configuracaoDominio,
                                      string caminhoProjeto,
                                      List<Type> todosTipos,
                                      string nomeArquivo) :
                                      base(configuracaoDominio, caminhoProjeto, todosTipos, nomeArquivo)
        {
            // this.TiposReflexao = this.TiposDominio;
            this.TiposEntidade = this.RetornarTiposEntidade();
            this.TiposComplexo = this.RetornarTiposComplexo();
            this.TiposEnum = this.RetornarTiposEnum();
            this.TiposBaseDominio = this.RetornarTiposBaseDominio();
            this.TiposAtributos = this.RetornarTiposAtributos();
        }

        protected override string RetornarConteudoTypeScript()
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine();
                
            if (this.ProjetoSneburDominio)
            {
                //Tipos Primarios
                sb.AppendLine(this.RetornarConteudoTiposPrimario());
            }
            //Enums
            sb.AppendLine(this.RetornarConteudoTiposEnum());
            //BasesDominio
            sb.AppendLine(this.RetornarConteudoBaseDominio());

            //Tipo Complexo
            sb.AppendLine(this.RetornarConteudoTipoComplexo());

            //Tipo Entidades
            sb.AppendLine(this.RetornarConteudoEntidade());
            //Atributos
            sb.AppendLine(this.RetornarAssociacaoAtributosCaminhoTipo());
            //Proprieades
            sb.AppendLine(this.RetornarConteudoPropriedades());

            sb.AppendLine();

            return this.RetornarConteudoTypeScriptNormalizado(sb.ToString());
        }

        private string RetornarConteudoTypeScriptNormalizado(string conteudo)
        {
            var linhas = conteudo.ToLines();
            var linhasNormalizadas = new List<string>();
            linhasNormalizadas.Add($"namespace {ProjetoTypeScriptUtil.NamespaceReflexao(this.NomeProjeto)}");
            linhasNormalizadas.Add("{");
            foreach (var linha in linhas)
            {
                var linhaNormalizada = linha.Trim();
                if (linhaNormalizada.StartsWith("let") || 
                    linhaNormalizada.StartsWith("const"))
                {
                    linhaNormalizada = "export " + linhaNormalizada;
                }
                linhasNormalizadas.Add("\t" + linhaNormalizada);
            }
            linhasNormalizadas.Add("}");
            return String.Join(System.Environment.NewLine, linhasNormalizadas);
        }

        protected override string RetornarNomeArquivoDestino()
        {
            return "Reflexao";
        }

        #region Retornar Tipos

        protected override HashSet<Type> RetornarRetornarTiposDominios()
        {
            return this.RetornarTiposReflexao();
        }

        private HashSet<Type> RetornarTiposReflexao()
        {
            var tiposReflexao = new List<Type>();
            tiposReflexao.AddRange(this.RetornarTiposEnum());
            tiposReflexao.AddRange(this.RetornarTiposEntidade());
            tiposReflexao.AddRange(this.RetornarTiposComplexo());
            tiposReflexao.AddRange(this.RetornarTiposBaseDominio());
            return tiposReflexao.ToHashSet();
        }

        private HashSet<Type> RetornarTiposEntidade()
        {
            var tipos = this.TodosTipo.Where(x => TipoUtil.TipoIgualOuSubTipo(x, AjudanteAssembly.TipoEntidade)).ToList();
            tipos = TipoUtil.IgnorarAtributo(tipos, AjudanteAssembly.NomeTipoIgnorarTSReflexao);
            return tipos.ToHashSet();
        }

        private HashSet<Type> RetornarTiposComplexo()
        {
            var tipos = this.TodosTipo.Where(x => TipoUtil.TipoIgualOuSubTipo(x, AjudanteAssembly.TipoBaseTipoComplexo)).ToList();
            tipos = TipoUtil.IgnorarAtributo(tipos, AjudanteAssembly.NomeTipoIgnorarTSReflexao);
            return tipos.ToHashSet();
        }

        private HashSet<Type> RetornarTiposBaseDominio()
        {
            var tipos = this.TodosTipo.Where(x => TipoUtil.TipoIgualOuSubTipo(x, AjudanteAssembly.TipoBaseDominio)).ToList();
            tipos = TipoUtil.IgnorarAtributo(tipos, AjudanteAssembly.NomeTipoIgnorarTSReflexao);

            foreach (var tipoEntidade in this.RetornarTiposEntidade())
            {
                tipos.Remove(tipoEntidade);
            }

            foreach (var tipoComplexo in this.RetornarTiposComplexo())
            {
                tipos.Remove(tipoComplexo);
            }
            return tipos.ToHashSet();
        }

        private HashSet<Type> RetornarTiposEnum()
        {
            var tiposEnum = this.TodosTipo.Where(x => x.IsEnum).ToList();
            tiposEnum = TipoUtil.IgnorarAtributo(tiposEnum, AjudanteAssembly.NomeTipoIgnorarTSReflexao);
            //if (this.ProjetoSneburDominio)
            //{
            //    tiposEnum.Add(typeof(EnumTipoPrimario));
            //}
            return tiposEnum.ToHashSet();
        }

        #endregion

    }
}
