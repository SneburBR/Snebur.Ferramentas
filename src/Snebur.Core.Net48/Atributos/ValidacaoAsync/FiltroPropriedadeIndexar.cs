using System;
using System.Reflection;

namespace Snebur.Dominio.Atributos
{
    public class FiltroPropriedadeIndexar
    {
        public PropertyInfo Propriedade { get; private set; }
        public EnumOperadorComparacao Operador { get; }
        public string Valor { get; }
        public string OperadoprString
        {
            get
            {
                switch (this.Operador)
                {
                    case EnumOperadorComparacao.Igual:
                        return " = ";
                    case EnumOperadorComparacao.Diferente:
                        return " <> ";
                    case EnumOperadorComparacao.MaiorQue:
                        return " > ";
                    case EnumOperadorComparacao.MenorQue:
                        return " < ";
                    case EnumOperadorComparacao.MaiorIgualA:
                        return " >= ";
                    case EnumOperadorComparacao.MenorIgualA:
                        return " <= ";
                    default:
                        throw new Exception("Operador não suportador");
                }
            }
        }

        public FiltroPropriedadeIndexar(PropertyInfo propriedade,
                                        EnumOperadorComparacao operadopr,
                                        string valor)
        {
            this.Propriedade = propriedade;
            this.Operador = operadopr;
            this.Valor = valor;
        }
    }
}