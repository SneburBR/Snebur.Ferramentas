using Snebur.Dominio;
using Snebur.Utilidade;
using System;
using System.Globalization;

namespace Snebur.VisualStudio
{
    public class TipoComplexoTypeScriptUtil
    {
        internal static string RetornarValor(dynamic tipoComplexo)
        {
            switch (tipoComplexo.GetType().Name)
            {
                case nameof(Cor):

                    return $"new d.Cor({tipoComplexo.Red}, {tipoComplexo.Green}, {tipoComplexo.Blue}, {tipoComplexo.AlphaDecimal.ToString(CultureInfo.InvariantCulture)})";

                case nameof(Margem):

                    return $"new d.Margem({tipoComplexo.Esquerda?.ToString(CultureInfo.InvariantCulture) ?? "0"}," +
                                         $"{tipoComplexo.Superior?.ToString(CultureInfo.InvariantCulture) ?? "0"}, " +
                                         $"{tipoComplexo.Direita?.ToString(CultureInfo.InvariantCulture) ?? "0"}, " +
                                         $"{tipoComplexo.Inferior?.ToString(CultureInfo.InvariantCulture) ?? "0"})";
                case nameof(Dimensao):

                    return $"new d.Dimensao( {tipoComplexo.Largura.ToString(CultureInfo.InvariantCulture)}, {tipoComplexo.Altura.ToString(CultureInfo.InvariantCulture)})";

                case nameof(Posicao):

                    return $"new d.Posicao( {tipoComplexo.X.ToString(CultureInfo.InvariantCulture)}, {tipoComplexo.Y.ToString(CultureInfo.InvariantCulture)})";

                case nameof(Regiao):

                    return $"new d.Regiao({tipoComplexo.X.ToString(CultureInfo.InvariantCulture)}, " +
                                         $"{tipoComplexo.Y.ToString(CultureInfo.InvariantCulture)}, " +
                                         $"{tipoComplexo.Largura.ToString(CultureInfo.InvariantCulture)}, " +
                                         $"{tipoComplexo.Altura.ToString(CultureInfo.InvariantCulture)})";

                case nameof(Borda):

                    return $"new d.Borda(\"{tipoComplexo.CorRgba.ToString(CultureInfo.InvariantCulture)}\", " +
                                       $"{tipoComplexo.IsInterna.ToString().ToLower()}, " +
                                       $"{tipoComplexo.Afastamento.ToString(CultureInfo.InvariantCulture)}, " +
                                       $"{tipoComplexo.Espessura.ToString(CultureInfo.InvariantCulture)}, " +
                                       $"{tipoComplexo.Arredondamento.ToString(CultureInfo.InvariantCulture)})";
                default:

                    throw new NotImplementedException($"RetornarValor em Typescript do tipo {tipoComplexo.GetType().Name}");
            }
        }
    }
}
