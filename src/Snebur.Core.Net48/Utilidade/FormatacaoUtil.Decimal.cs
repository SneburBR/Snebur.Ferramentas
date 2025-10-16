using System;

namespace Snebur.Utilidade
{
    public static partial class FormatacaoUtil
	{

		public static string FormatarDecimal(double? valor)
		{
			return FormatarDecimal(valor, CASAS_DECIMAL_PADRAO);
		}

		public static string FormatarDecimal(double valor)
		{
			return FormatarDecimal(valor, CASAS_DECIMAL_PADRAO);
		}
		public static string FormatarDecimal(double? valor,
											 EnumDivisorDecimal divisorDecimal = EnumDivisorDecimal.CulturaAtual)
		{
			return FormatarDecimal(valor, CASAS_DECIMAL_PADRAO, divisorDecimal);
		}
		public static string FormatarDecimal(double valor,
											 EnumDivisorDecimal divisorDecimal = EnumDivisorDecimal.CulturaAtual)
		{
			return FormatarDecimal(valor, CASAS_DECIMAL_PADRAO, divisorDecimal);
		}
		public static string FormatarDecimal(double? valor,
											 int casasDecimal,
											 EnumDivisorDecimal divisorDecimal = EnumDivisorDecimal.CulturaAtual)
		{
			if (valor == null)
			{
				return "0";
			}
			return FormatarDecimal(valor.Value, casasDecimal, divisorDecimal);
		}

		public static string FormatarDecimal(decimal? valor)
		{
			return FormatarDecimal(valor, CASAS_DECIMAL_PADRAO);
		}

		public static string FormatarDecimal(decimal valor)
		{
			return FormatarDecimal(valor, CASAS_DECIMAL_PADRAO);
		}
		public static string FormatarDecimal(decimal? valor,
											 EnumDivisorDecimal divisorDecimal = EnumDivisorDecimal.CulturaAtual)
		{
			return FormatarDecimal(valor, CASAS_DECIMAL_PADRAO, divisorDecimal);
		}
		public static string FormatarDecimal(decimal valor,
											 EnumDivisorDecimal divisorDecimal = EnumDivisorDecimal.CulturaAtual)
		{
			return FormatarDecimal(valor, CASAS_DECIMAL_PADRAO, divisorDecimal);
		}
		public static string FormatarDecimal(decimal? valor,
											 int casasDecimal,
											 EnumDivisorDecimal divisorDecimal = EnumDivisorDecimal.CulturaAtual)
		{
			if (valor == null)
			{
				return "0";
			}
			return FormatarDecimal(valor.Value, casasDecimal, divisorDecimal);
		}

		public static string FormatarDecimal(double valor,
											 int casasDecimal,
											 EnumDivisorDecimal divisorDecimal = EnumDivisorDecimal.CulturaAtual)
		{
			return FormatarDecimal(Convert.ToDecimal(valor), casasDecimal, divisorDecimal);
		}

		private static string FormatarDecimal(decimal valor,
											  int casasDecimal,
											  EnumDivisorDecimal divisorDecimal)
		{
			var resultado = FormatarDecimalInterno(valor, casasDecimal);
			if (divisorDecimal == EnumDivisorDecimal.CulturaAtual)
			{
				return resultado;
			}

			if (divisorDecimal == EnumDivisorDecimal.Ponto)
			{
				return resultado.Replace(",", ".");
			}
			return resultado.Replace(".", ",");

		}

		private static string FormatarDecimalInterno(decimal valor, int digitos)
		{
			if (digitos == 0 || valor % 1 == 0)
			{
				return $"{valor:0}";
			}

			if (digitos == 1 ||
				((valor - Math.Floor(valor)) * 10) % 1 == 0)
			{
				return $"{valor:0.0}";
			}

			if (digitos == 2 ||
				 ((valor - Math.Floor(valor)) * 100) % 1 == 0)
			{
				return $"{valor:0.00}";
			}

			if (digitos == 3 ||
				((valor - Math.Floor(valor)) * 1000) % 1 == 0)
			{
				return $"{valor:0.000}";
			}

			string formatar = "0." + new string('0', digitos);
			return valor.ToString(formatar);

		}
	}
}
