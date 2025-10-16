using Snebur.Dominio;
using Snebur.Utilidade;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Snebur.UI
{
    public class CorUtil
    {
        private static Dictionary<EnumCor, string> RetornarDicionarioCoresSufixo()
        {
            var todasCores = EnumUtil.RetornarValoresEnum<EnumCor>();
            var dicionario = todasCores.ToDictionary(x => x, x => EnumUtil.RetornarDescricao(x).Trim().ToLowerInvariant().Replace("sistema", ""));
            var retorno = new Dictionary<EnumCor, string>{
                      {EnumCor.AmareloMostarda, "amarelo-mostarda" },
                      {EnumCor.CinzaAzulado, "cinza-azulado" },
                      {EnumCor.LaranjaEscuro, "laranja-escuro"},
                      {EnumCor.RoxoEscuro, "roxo-escuro"},
                      {EnumCor.RoxoClaro, "roxo-claro"},
                      {EnumCor.VerdeClaro, "verde-claro"},
                      {EnumCor.VerdeLima, "verde-lima"},
                      {EnumCor.AzulClaro, "azul-claro"}
            };

            foreach (var item in dicionario)
            {
                if (!retorno.ContainsKey(item.Key))
                {
                    retorno.Add(item.Key, item.Value);
                }
            }
            return retorno;
        }

        private static Dictionary<EnumCor, string> DicionarioCoresSufixo { get; } = RetornarDicionarioCoresSufixo();

        public static string RetornarNomeCor(EnumCor cor)
        {
            return DicionarioCoresSufixo[cor];
        }
        //private static string RetornarNomeCor(EnumCor cor, EnumTonalidade tonalidade)
        //{
        //    var sufixoCor = CorUtil.DicionarioCoresSufixo[cor];
        //    var sufixoTonalidade = CorUtil.RetornarSufixoTonalidade(tonalidade);
        //    return sufixoCor + sufixoTonalidade;
        //}

        public static string RetornarNomeOriginal(EnumCor cor, bool ignorarErro = false)
        {
            switch (cor)
            {
                case EnumCor.Amarelo:

                    return "yellow";

                case EnumCor.AmareloMostarda:

                    return "amber";

                case EnumCor.Azul:

                    return "blue";

                case EnumCor.AzulClaro:

                    return "light_blue";

                case EnumCor.Turquesa:

                    return "teal";

                case EnumCor.Branca:

                    return "white";

                case EnumCor.Ciano:

                    return "cyan";

                case EnumCor.Cinza:

                    return "grey";

                case EnumCor.CinzaAzulado:

                    return "blue_grey";

                case EnumCor.Indigo:

                    return "indigo";

                case EnumCor.Laranja:

                    return "orange";

                case EnumCor.LaranjaEscuro:

                    return "deep_orange";

                case EnumCor.Marron:

                    return "brown";

                case EnumCor.Roxo:

                    return "purple";

                case EnumCor.RoxoEscuro:

                    return "deep_purple";

                case EnumCor.Rosa:

                    return "pink";

                case EnumCor.Verde:

                    return "green";

                case EnumCor.VerdeClaro:

                    return "light_green";

                case EnumCor.VerdeLima:

                    return "lime";

                case EnumCor.Vermelha:

                    return "red";

                default:

                    if (!ignorarErro)
                    {
                        var descricao = EnumUtil.RetornarDescricao(cor);
                        throw new ErroNaoSuportado(String.Format("A cor {0} não é suportada", descricao));
                    }
                    return null;
            }
        }

        public static Cor RetornarCor(string value)
        {
            if (value != null)
            {
                if (ValidacaoUtil.IsCorRgbOuRgba(value))
                {
                    return new Cor(value);
                }
                if (ValidacaoUtil.IsCorHexa(value))
                {
                    var rgba = ConverterUtil.ConverterHexaParaRgbaInterno(value);
                    return new Cor(rgba);
                }
                throw new Exception($"não foi possível converter a string {value} para cor");
            }
            return null;
        }
    }
}