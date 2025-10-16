using Snebur.Utilidade;
using System;
using System.Linq;

namespace Snebur.Dominio.Atributos
{
    public static class FiltroPropriedadeIndexarUtil
    {
        public static FiltroPropriedadeIndexar RetornarPropriedadeFiltro(Type tipoEntidade, string[] partes)
        {
            if (partes.Count() != 3)
            {
                return null;
            }
            var nomePropriedade = partes[0];
            var operador = partes[1];
            var valor = partes[2];
            var propriedade = ReflexaoUtil.RetornarPropriedade(tipoEntidade, nomePropriedade, true);
            if (propriedade == null)
            {
                return null;
            }
            var operadorEnum = RetornarOperador(operador, nomePropriedade);
            return new FiltroPropriedadeIndexar(propriedade, operadorEnum, valor);
        }

        private static EnumOperadorComparacao RetornarOperador(string operador, string nomePropriedade)
        {
            switch (operador.Trim())
            {
                case "=":
                    return EnumOperadorComparacao.Igual;
                case "<>":
                    return EnumOperadorComparacao.Diferente;
                case ">":
                    return EnumOperadorComparacao.MaiorQue;
                case ">=":
                    return EnumOperadorComparacao.MaiorIgualA;
                case "<":
                    return EnumOperadorComparacao.MenorQue;
                case "<=":
                    return EnumOperadorComparacao.MenorIgualA;
                default:
                    throw new Exception($"O operador '{operador}' não é suportado para a propriedade '{nomePropriedade}'");
            }
        }
    }

}