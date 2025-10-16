using Snebur.Dominio;
using System;

namespace Snebur.Utilidade
{
    public static class JurosUtil
    {
        public static decimal CalcularJuros(EnumTipoJuros tipoJuros,
                                            decimal valorCapital,
                                            int numeroParcelas,
                                            decimal jurosAoMes)
        {
            switch (tipoJuros)
            {
                case EnumTipoJuros.Simples:

                    return CalcularJurosSimples(valorCapital,
                                                          numeroParcelas,
                                                          jurosAoMes);

                case EnumTipoJuros.Composto:

                    return CalcularJurosComposto(valorCapital,
                                                          numeroParcelas,
                                                          jurosAoMes);
                case EnumTipoJuros.Amortizado:

                    return CalcularJurosAmortizado(valorCapital,
                                                             numeroParcelas,
                                                             jurosAoMes);

                default:

                    throw new ErroNaoSuportado($"O tipo de juros {tipoJuros} não é suportado");
            }

        }

        public static decimal CalcularJurosSimples(decimal valorCapital,
                                                   int numeroParcelas,
                                                   decimal jurosAoMes)
        {
            if (jurosAoMes > 0)
            {
                var juros = (1 + ((jurosAoMes / 100M) * numeroParcelas));
                return (valorCapital * juros);
            }
            return valorCapital;
        }
        public static decimal CalcularJurosComposto(decimal valorCapital,
                                                    int numeroParcelas,
                                                    decimal jurosAoMes)
        {
            if (jurosAoMes > 0)
            {
                double juros = 1 + ((double)jurosAoMes / 100D);
                double valorFinal = (double)valorCapital * Math.Pow(juros, numeroParcelas);
                return Convert.ToDecimal(valorFinal);
            }
            return valorCapital;
        }

        public static decimal CalcularJurosAmortizado(decimal valorCapital,
                                                      int numeroParcelas,
                                                      decimal jurosAoMes)
        {
            if (jurosAoMes > 0)
            {
                return CalcularJurosAmortizadoInterno((double)valorCapital,
                                                      numeroParcelas,
                                                      (double)jurosAoMes);
            }
            return valorCapital;
        }

        private static decimal CalcularJurosAmortizadoInterno(double valorCapital,
                                                              double numeroParcelas,
                                                              double jurosAoMes)
        {
            var jurosNormalizado = (jurosAoMes / 100D);
            var valorParcela = (valorCapital * Math.Pow((1 + jurosNormalizado), numeroParcelas) * jurosNormalizado) / (Math.Pow((1 + jurosNormalizado), numeroParcelas) - 1);
            return Convert.ToDecimal(valorParcela * numeroParcelas);
        }
    }
}
