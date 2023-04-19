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
using Snebur.Dominio.Atributos;

namespace Snebur.VisualStudio
{
    public abstract class EstruturaPropriedade : BaseEstrutura
    {

        public PropertyInfo Propriedade { get; set; }

        public string NomePropriedade { get; set; }
        public string ValorPropriedade { get; set; }
        public bool IsPossuiValorPadraoCampoPrivado { get; }
        public bool Abstrata { get; set; }

        public EstruturaPropriedade( PropertyInfo propriedade) : base(propriedade.PropertyType)
        {
            this.Propriedade = propriedade;
            this.NomePropriedade = propriedade.Name;
            this.IsPossuiValorPadraoCampoPrivado = PropriedadeUtil.PossuiAtributo(this.Propriedade, nameof(ValorPadraoCampoPrivadoAttribute));
            this.ValorPropriedade = this.RetornarValorPropriedadeTypeScript();

        }

        public abstract List<string> RetornarLinhasTypeScript(string tabInicial);

        public string RetornarValorPropriedadeTypeScript()
        {
            if (this.IsPossuiValorPadraoCampoPrivado)
            {
                if (ReflexaoUtil.TipoRetornaTipoPrimario(this.Tipo))
                {
                    var tipoPrimarioEnum = ReflexaoUtil.RetornarTipoPrimarioEnum(this.Tipo);
                    return this.RetornaValorAtributoValorPadraoCampoPrivado(tipoPrimarioEnum);
                }
                else
                {
                    throw new Exception($"O tipo {this.Tipo.Name}  não é suportado pelo atributo {nameof(ValorPadraoCampoPrivadoAttribute)}");
                }
            }
            else
            {
                return this.RetornarValorPadraoTypeScript();
            }
        }
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

        private string RetornarParametrosTipoComplexo()
        {
            var parametros = AjudanteReflexao.RetornarParametrosConstrutor(this.Tipo);
            return String.Join(", ", parametros.Select(x => this.RetornarValorPadraoTipoPrimario(ReflexaoUtil.RetornarTipoPrimarioEnum(x.Tipo))));
        }


        private string RetornaValorAtributoValorPadraoCampoPrivado(EnumTipoPrimario tipoPrimarioEnum)
        {
            var atributo = PropriedadeUtil.RetornarAtributo(this.Propriedade, typeof(ValorPadraoCampoPrivadoAttribute), false);
            var valorPadrao = ReflexaoUtil.RetornarValorPropriedade<ValorPadraoCampoPrivadoAttribute>(x => x.ValorPadrao, atributo);
            var tipoEnum = ReflexaoUtil.RetornarValorPropriedade<ValorPadraoCampoPrivadoAttribute>(x => x.TipoEnum, atributo) as Type;
            if (tipoEnum != null)
            {
                if (!tipoEnum.IsEnum)
                {
                    throw new Exception("O tipoEnum nao é suportado");
                }
                var valorEnum = Enum.Parse(tipoEnum, valorPadrao.ToString());
                return $"{tipoEnum.Namespace}.{tipoEnum.Name}.{valorEnum.ToString()}";
            }
            return this.RetornarValorTipoPrimario(tipoPrimarioEnum, valorPadrao);
  


        }

    }
}
