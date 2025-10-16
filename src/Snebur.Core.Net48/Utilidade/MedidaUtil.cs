using Snebur.Dominio;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Snebur.Utilidade
{
    public static class MedidaUtil
    {
        public const int BITS_POR_PIXELS_RGBA = 32;
        public const int BITS_POR_PIXELS_JPEG = 24;

        public const double DPI_IMPRESSAO_PADRAO = 300;
        public const double DPI_APRESENTACAO_MAXIMO = 200;
        public const double POLEGADA = 2.54;
        public const double DPI_VISUALIZACAO_WPF = 96;
        public const double DPI_VISUALIZACAO_WEB = 72;

        public const double MAXIMO_AREA_IMPRESSAO_SUPORTADA = 200 * 1024 * 1024;

        public static readonly double LARGURA_APRESENTACAO = 1920;
        public static readonly double ALTURA_APRESENTACAO = 1080;
        public static readonly Dimensao DIMENSAO_RECIPIENTE_APRESENTACAO = new Dimensao(LARGURA_APRESENTACAO, ALTURA_APRESENTACAO);

        public static readonly Func<double?, double> FUNCAO_DPI_VISAULIZACAO = (value) => MAXIMO_AREA_IMPRESSAO_SUPORTADA;
        public static double RetornarPixelsImpressao(double medidaEmCentimetros)
        {
            return ParaPixels(medidaEmCentimetros, DPI_IMPRESSAO_PADRAO);
        }

        public static int RetornarPixelsVisualizacao(double medidiaEmCentimetros, double dpiVisualizacao)
        {
            return ParaPixels(medidiaEmCentimetros, dpiVisualizacao);
        }

        public static int ParaPixels(double medidaEmCentimetos, double dpi)
        {
            return Convert.ToInt32((medidaEmCentimetos / POLEGADA) * dpi);
        }

        public static double ParaCentimentros(double mediaEmPixels, double dpi)
        {
            return (mediaEmPixels / dpi) * POLEGADA;
        }

        public static double RetornarDpiVisualizacao(double medidaEmCentimetros, double medidaEmPixel)
        {
            return (medidaEmPixel * 300) / RetornarPixelsImpressao(medidaEmCentimetros);
        }

        public static int CentimetrosParaDecimilimetros(double espessuraLaminasCm)
        {
            return Convert.ToInt32(Math.Round(espessuraLaminasCm * 10 * 10));
        }

        public static int RetornarDpiVisualizacao(
            Dimensao dimensaoCentimetros, 
            Dimensao dimensaoPixels,
            double tolerancia = 1)
        {
            var dpiX = (int)Math.Round(RetornarDpiVisualizacao(dimensaoCentimetros.Largura, dimensaoPixels.Largura));
            var dpiY = (int)Math.Round(RetornarDpiVisualizacao(dimensaoCentimetros.Altura, dimensaoPixels.Altura));
            if (Math.Abs(dpiX - dpiX) > tolerancia)
            {
                throw new Erro($"DPI X '{dpiX}' e diferente do Y '{dpiY}', fora da margem tolerância: {tolerancia}");
            }
            return Math.Min(dpiX, dpiY);
        }

        public static void DefinirDpiImpressao(object objeto)
        {
            DefinirDpiVisualizacao(objeto, DPI_IMPRESSAO_PADRAO);
        }

        public static void DefinirDpiVisualizacao(object objeto, double dpi)
        {
            Func<double?, double> funcao = (value) =>
            {
                return dpi;
            };
            DefinirDpiVisualizacao(objeto, funcao);
        }

        public static void DefinirDpiVisualizacao(object objeto, Func<double?, double> funcaoRetornarDpi)
        {
            if (objeto != null)
            {
                var objetosAnalisados = new HashSet<object>();
                DefinirDpiTipoComplexo(objeto, funcaoRetornarDpi, objetosAnalisados);
            }
        }

        private static void DefinirDpiTipoComplexo(object objeto, Func<double?, double> funcaoRetornarDpi, HashSet<object> objetosAnalisados)
        {
            var tipo = objeto.GetType();
            if (ReflexaoUtil.IsTipoRetornaColecao(tipo))
            {
                foreach (var item in (IEnumerable)objeto)
                {
                    DefinirDpiTipoComplexo(item, funcaoRetornarDpi, objetosAnalisados);
                }
            }
            else
            {
                if ((objetosAnalisados.Contains(objeto)))
                {
                    return;
                }
                objetosAnalisados.Add(objeto);

                if (objeto is IDpiVisualizacao objetoDpi)
                {
                    objetoDpi.FuncaoNormamlizarDpiVisualizacao = funcaoRetornarDpi;
                }
                if (!tipo.IsValueType && tipo != typeof(string) && !tipo.IsSubclassOf(typeof(BaseTipoComplexo)))
                {
                    var propriedades = objeto.GetType().GetProperties().Where(x => x.GetSetMethod() != null && x.GetSetMethod().IsPublic);
                    foreach (var p in propriedades)
                    {
                        var objetoFilho = p.GetValue(objeto);
                        if (objetoFilho != null)
                        {
                            DefinirDpiTipoComplexo(objetoFilho, funcaoRetornarDpi, objetosAnalisados);
                        }
                    }
                }
            }
        }

        public static double FuncaoRetornarDpiImpressao()
        {
            return DPI_IMPRESSAO_PADRAO;
        }
        public static double RetornarDpiImpressao(IDimensao dimensao)
        {
            var larguraPixels = RetornarPixelsImpressao(dimensao.Largura);
            var alturaPixels = RetornarPixelsImpressao(dimensao.Largura);
            var escala = Math.Sqrt(MAXIMO_AREA_IMPRESSAO_SUPORTADA) / Math.Sqrt((larguraPixels * alturaPixels));
            if (escala > 1)
            {
                return DPI_IMPRESSAO_PADRAO;
            }
            var dpi = DPI_IMPRESSAO_PADRAO * escala;
            if (DebugUtil.IsAttached)
            {
                var larguraPixelsNormalizada = RetornarPixelsVisualizacao(dimensao.Largura, dpi);
                var alturaPixelsNormalizada = RetornarPixelsVisualizacao(dimensao.Altura, dpi);

                if (((larguraPixelsNormalizada * alturaPixelsNormalizada) * 0.95) > MAXIMO_AREA_IMPRESSAO_SUPORTADA)
                {
                    throw new Exception("Falha no calculo dpi de impressão");
                }
            }
            return dpi;
        }
    }
}