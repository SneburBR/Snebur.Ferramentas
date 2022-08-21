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
using Snebur.Utilidade;

namespace Snebur.VisualStudio.Projeto.Dominio
{
    public partial class GeradorDominioReflexao : BaseGeradorDominio
    {

        private string RetornarConteudoTiposPrimario()
        {
            var sb = new System.Text.StringBuilder();

            sb.AppendLine(this.RetornarDeclaracoesTipoPrimario());
            sb.AppendLine("//Lista de tipos primarios");
            sb.AppendLine(this.RetornarDeclaracoesListaTipoPrimario());
            sb.AppendLine("//Adicionar os tipos");
            sb.AppendLine("//Adicionar  Tipos primarios");
            sb.AppendLine(this.RetornarConteudoAdicionarTiposTipoPrimario());
            sb.AppendLine(this.RetornarConteudoAdicionarTiposListaTipoPrimario());
            return sb.ToString();
        }

        private string RetornarDeclaracoesTipoPrimario()
        {
            var sb = new System.Text.StringBuilder();

            var tiposPrimarioEnum = EnumUtil.RetornarValoresEnum(typeof(EnumTipoPrimario));
            foreach (EnumTipoPrimario tipoPrimarioEnum in tiposPrimarioEnum)
            {
                if (tipoPrimarioEnum != EnumTipoPrimario.Desconhecido)
                {
                    var declaracaoTipoPrimario = AjudanteReflexao.RetornarDeclaracaoTipoPrimario(tipoPrimarioEnum);
                    var descricao = EnumUtil.RetornarDescricao(tipoPrimarioEnum);
                    //AjudanteReflexao.RetornarCaminhoTipoPrimario
                    var enumTipoPrimario = String.Format("{0}.{1}", TipoUtil.RetornarCaminhoTipo(typeof(EnumTipoPrimario)), descricao);

                    var declaracao = String.Format("var {0} = new Snebur.Reflexao.TipoPrimario(\"{1}\", {2});",
                                                   declaracaoTipoPrimario,
                                                   descricao,
                                                   enumTipoPrimario);

                    sb.AppendLine(declaracao);

                }
            }
            return sb.ToString();
        }

        private string RetornarDeclaracoesListaTipoPrimario()
        {
            var sb = new System.Text.StringBuilder();

            var tiposPrimarioEnum = EnumUtil.RetornarValoresEnum(typeof(EnumTipoPrimario));
            foreach (EnumTipoPrimario tipoPrimarioEnum in tiposPrimarioEnum)
            {
                if (tipoPrimarioEnum != EnumTipoPrimario.Desconhecido)
                {
                    var declaracaoListaTipoPrimario = AjudanteReflexao.RetornarDeclaracaoListaTipoPrimario(tipoPrimarioEnum);
                    var declaracaoTipoPrimario = AjudanteReflexao.RetornarDeclaracaoTipoPrimario(tipoPrimarioEnum);



                    var declaracao = String.Format("var {0} = new Snebur.Reflexao.TipoListaTipoPrimario({1});",
                                                   declaracaoListaTipoPrimario,
                                                   declaracaoTipoPrimario);

                    sb.AppendLine(declaracao);

                }
            }
            return sb.ToString();
        }

        private string RetornarConteudoAdicionarTiposTipoPrimario()
        {
            var sb = new System.Text.StringBuilder();

            var tiposPrimarioEnum = EnumUtil.RetornarValoresEnum(typeof(EnumTipoPrimario));
            foreach (EnumTipoPrimario tipoPrimarioEnum in tiposPrimarioEnum)
            {
                if (tipoPrimarioEnum != EnumTipoPrimario.Desconhecido)
                {
                    var declaracaoTioPrimario = AjudanteReflexao.RetornarDeclaracaoTipoPrimario(tipoPrimarioEnum);
                    var caminhoTipo = EnumUtil.RetornarDescricao(tipoPrimarioEnum);
                    sb.AppendLine(this.RetornarAdicionarTipo(caminhoTipo, declaracaoTioPrimario));

                }
            }
            return sb.ToString();
        }

        private string RetornarConteudoAdicionarTiposListaTipoPrimario()
        {
            var sb = new System.Text.StringBuilder();

            var tiposPrimarioEnum = EnumUtil.RetornarValoresEnum(typeof(EnumTipoPrimario));
            foreach (EnumTipoPrimario tipoPrimarioEnum in tiposPrimarioEnum)
            {
                if (tipoPrimarioEnum != EnumTipoPrimario.Desconhecido)
                {
                    var declaracaoListaTipoPrimario = AjudanteReflexao.RetornarDeclaracaoListaTipoPrimario(tipoPrimarioEnum);
                    var caminhoListaTipoPrimario = AjudanteReflexao.RetornarCaminhoListaTipoPrimario(tipoPrimarioEnum);

                    sb.AppendLine(this.RetornarAdicionarTipo(caminhoListaTipoPrimario, declaracaoListaTipoPrimario));

                }
            }
            return sb.ToString();
        }

    }
}
