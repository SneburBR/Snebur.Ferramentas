using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snebur;
using System.Reflection;
using Snebur.VisualStudio.Utilidade;
using Snebur.VisualStudio.Reflexao;
using Snebur.Reflexao;
using Snebur.Utilidade;

namespace Snebur.VisualStudio.Projeto.Dominio.Estrutura
{
    public abstract class EstruturaPropriedade : BaseEstrutura
    {

        public PropertyInfo Propriedade { get; set; }

        public string NomePropriedade { get; set; }

        public string ValorPropriedade { get; set; }

        public bool Abstrata { get; set; }

        public EstruturaPropriedade(PropertyInfo propriedade) : base(propriedade.PropertyType)
        {
            this.Propriedade = propriedade;
            this.NomePropriedade = propriedade.Name;
            this.ValorPropriedade = this.RetornarValorPadraoTypeScript();
        }

        public abstract List<string> RetornarLinhasTypeScript(string tabInicial);

        public string RetornarValorPadraoTypeScript()
        {
            if (ReflexaoUtil.TipoRetornaColecao(this.Tipo))
            {

                return String.Format(" new {0}()", this.CaminhoTipo);

            }

            if (TipoUtil.TipoSubTipo(this.Tipo, AjudanteAssembly.TipoBaseTipoComplexo))
            {
                var parametrops = this.RetornarParametrosTipoComplexo();
                return String.Format(" new {0}({1})", this.CaminhoTipo, parametrops);
            }

            if (ReflexaoUtil.IsTipoNullable(this.Tipo) || !this.Tipo.IsValueType)
            {
                if (PropriedadeUtil.PossuiAtributo(this.Propriedade, AjudanteAssembly.NomeTipoAtributoCriarInstanciaTS))
                {
                    return String.Format(" new {0}()", this.CaminhoTipo);
                }
                return "null";
            }


            if (ReflexaoUtil.TipoRetornaTipoPrimario(this.Tipo))
            {
                var tipoPrimarioEnum = ReflexaoUtil.RetornarTipoPrimarioEnum(this.Tipo);
                return this.RetornarValorPadraoTipoPrimario(tipoPrimarioEnum);
            }

            return null;
        }

        private String RetornarParametrosTipoComplexo()
        {
            var parametros = AjudanteReflexao.RetornarParametrosConstrutor(this.Tipo);
            return String.Join(", ", parametros.Select(x => this.RetornarValorPadraoTipoPrimario(ReflexaoUtil.RetornarTipoPrimarioEnum(x.Tipo))));
        }

    }
}
