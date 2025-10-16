using Snebur.Utilidade;
using System;
using System.Reflection;

namespace Snebur.Dominio.Atributos
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ValidacaoComparacaoAttribute : BaseAtributoValidacao, IAtributoValidacao
    {
        [MensagemValidacao]
        public static string MensagemValidacao { get; set; } = "O {0} deve ser {1} à '{2}' ";

        public object Valor { set; get; }

        public EnumOperadorComparacao Operador { get; set; }

        public ValidacaoComparacaoAttribute(object valor, EnumOperadorComparacao operador)
        {
            this.Valor = valor;
            this.Operador = operador;
        }

        #region IAtributoValidacao

        public override bool IsValido(PropertyInfo propriedade, object paiPropriedade, object valorPropriedade)
        {
            if (!ValidacaoUtil.IsDefinido(valorPropriedade))
            {
                return !ValidacaoUtil.IsPropriedadeRequerida(propriedade);
            }
            switch (this.Operador)
            {
                case EnumOperadorComparacao.Igual:

                    return Util.SaoIgual(valorPropriedade, this.Valor);

                case EnumOperadorComparacao.Diferente:

                    return !Util.SaoIgual(valorPropriedade, this.Valor);

                case EnumOperadorComparacao.MaiorQue:
                case EnumOperadorComparacao.MenorQue:
                case EnumOperadorComparacao.MaiorIgualA:
                case EnumOperadorComparacao.MenorIgualA:
                    {
                        if (valorPropriedade != null &&
                            this.Valor != null &&
                            Double.TryParse(String.Concat(valorPropriedade, ""), out double valorPropriedadeTipado) &&
                            Double.TryParse(String.Concat(this.Valor, ""), out double valorTipado))
                        {
                            return this.Comparar(valorPropriedadeTipado, valorTipado);
                        }
                        return false;
                    }
                default:

                    throw new ErroNaoSuportado($"O operador '{EnumUtil.RetornarDescricao(this.Operador)}' não é suportado ");
            }
        }

        public override string RetornarMensagemValidacao(PropertyInfo propriedade, object paiPropriedade, object valorPropriedade)
        {
            var rotulo = ReflexaoUtil.RetornarRotulo(propriedade);
            var descricaoOperador = EnumUtil.RetornarDescricao(this.Operador);
            return String.Format(MensagemValidacao, rotulo, descricaoOperador, String.Concat(this.Valor, ""));
        }
        #endregion

        private bool Comparar(double valorPropriedade, double valor)
        {
            switch (this.Operador)
            {
                case EnumOperadorComparacao.MaiorQue:
                    return valorPropriedade > valor;

                case EnumOperadorComparacao.MenorQue:

                    return valorPropriedade < valor;

                case EnumOperadorComparacao.MaiorIgualA:

                    return valorPropriedade >= valor;

                case EnumOperadorComparacao.MenorIgualA:

                    return valorPropriedade <= valor;
            }
            return false;
        }
    }
}