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

namespace Snebur.VisualStudio.Projeto.Dominio
{
    public partial class GeradorDominioReflexao : BaseGeradorDominio
    {

        private List<Type> TiposBaseDominio { get; set; }

        private List<Type> TiposEntidade { get; set; }

        private List<Type> TiposComplexo { get; set; }

        private List<Type> TiposEnum { get; set; }

        private List<Type> TiposAtributos { get; set; }


        public GeradorDominioReflexao(ConfiguracaoProjetoDominio configuracaoDominio,
                                      List<Type> todosTipos,
                                      string nomeArquivo) : base(configuracaoDominio, todosTipos, nomeArquivo)
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
            return sb.ToString();
        }

        protected override string RetornarNomeArquivoDestino()
        {
            return "Reflexao";
        }

        #region Retornar Tipos

        protected override List<Type> RetornarRetornarTiposDominios()
        {
            return this.RetornarTiposReflexao();
        }

        private List<Type> RetornarTiposReflexao()
        {
            var tiposReflexao = new List<Type>();
            tiposReflexao.AddRange(this.RetornarTiposEnum());
            tiposReflexao.AddRange(this.RetornarTiposEntidade());
            tiposReflexao.AddRange(this.RetornarTiposComplexo());
            tiposReflexao.AddRange(this.RetornarTiposBaseDominio());
            return tiposReflexao;
        }

        private List<Type> RetornarTiposEntidade()
        {
            var tipos = this.TodosTipo.Where(x => TipoUtil.TipoIgualOuSubTipo(x, AjudanteAssembly.TipoEntidade)).ToList();
            tipos = TipoUtil.IgnorarAtributo(tipos, AjudanteAssembly.NomeTipoIgnorarTSReflexao);
            return tipos;
        }

        private List<Type> RetornarTiposComplexo()
        {
            var tipos = this.TodosTipo.Where(x => TipoUtil.TipoIgualOuSubTipo(x, AjudanteAssembly.TipoBaseTipoComplexo)).ToList();
            tipos = TipoUtil.IgnorarAtributo(tipos, AjudanteAssembly.NomeTipoIgnorarTSReflexao);
            return tipos;
        }

        private List<Type> RetornarTiposBaseDominio()
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
            return tipos;
        }

        private List<Type> RetornarTiposEnum()
        {
            var tiposEnum = this.TodosTipo.Where(x => x.IsEnum).ToList();
            tiposEnum = TipoUtil.IgnorarAtributo(tiposEnum, AjudanteAssembly.NomeTipoIgnorarTSReflexao);
            if (this.ProjetoSneburDominio)
            {
                tiposEnum.Add(typeof(EnumTipoPrimario));
            }
            return tiposEnum;
        }

        #endregion

    }
}
