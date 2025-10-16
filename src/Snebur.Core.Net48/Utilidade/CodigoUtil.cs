using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Snebur.Utilidade
{
    public class CodigoUtil
    {
        public static string Formatar(
            string nome,
            EnumFormatacaoCodigo formatacaoOrigem,
            EnumFormatacaoCodigo formatacaoDestino)
        {

            var reg = new Regex(@"\s");
            if (reg.IsMatch(nome))
            {
                throw new Exception("O nome da variável não pode conter espaços");
            }

            var partes = RetornarPartes(nome.Trim(), formatacaoOrigem)
                                    .Where(x => !String.IsNullOrWhiteSpace(x))
                                    .ToArray();
            switch (formatacaoDestino)
            {
                case EnumFormatacaoCodigo.CamelCase:
                    return string.Concat(partes.Select((x, index) =>
                    {
                        if (index == 0) return x.ToLower();
                        return FormatacaoUtil.FormatarPrimeiraLetraMaiuscula(x);
                    }));

                case EnumFormatacaoCodigo.PascalCase:
                    return string.Concat(partes.Select(FormatacaoUtil.FormatarPrimeiraLetraMaiuscula));

                case EnumFormatacaoCodigo.SnakeCase:
                    return string.Join("_", partes.Select(x => x.ToLower()));

                case EnumFormatacaoCodigo.UpperCase:
                    return string.Join("_", partes.Select(x => x.ToUpper()));

                case EnumFormatacaoCodigo.KebabCase:
                    return string.Join("-", partes.Select(x => x.ToLower()));

                default:
                    throw new Exception("Formatação não suportada");
            }
        }

        private static string[] RetornarPartes(string nome, EnumFormatacaoCodigo formatacao)
        {
            switch (formatacao)
            {
                case EnumFormatacaoCodigo.CamelCase:
                case EnumFormatacaoCodigo.PascalCase:

                    //return nome.split(/(?=[A-Z0-9])/);
                    //return nome.Split(new[] { char.IsUpper }, StringSplitOptions.RemoveEmptyEntries);
                    return Regex.Split(nome, @"(?=[A-Z0-9])", RegexOptions.IgnorePatternWhitespace);

                case EnumFormatacaoCodigo.SnakeCase:
                case EnumFormatacaoCodigo.UpperCase:
                    return nome.Split('_');

                case EnumFormatacaoCodigo.KebabCase:
                    return nome.Split('-');

                default:
                    throw new Exception("Formatação não suportada");
            }
        }
    }

    public enum EnumFormatacaoCodigo
    {
        PascalCase,
        CamelCase,
        SnakeCase,
        UpperCase,
        KebabCase
    }

}

