using Snebur.Dominio;
using System;

namespace Snebur.Utilidade
{
    public class DimensaoUtil
    {
        public static Dimensao RetornarDimencaoAlturaMaxima(Dimensao dimensao, double alturaMaxima)
        {
            return RetornarDimencaoAlturaMaxima(dimensao.Largura, dimensao.Altura, alturaMaxima);
        }

        public static Dimensao RetornarDimencaoAlturaMaxima(double largura, double altura, double alturaMaxima)
        {
            var novaAltura = alturaMaxima;
            var novaLargura = largura * novaAltura / altura;
            return new Dimensao(novaLargura, novaAltura);
        }

        public static Dimensao RetornarDimencaoLarguraMaxima(Dimensao dimensao, double alturaMaxima)
        {
            return RetornarDimencaoLarguraMaxima(dimensao.Largura, dimensao.Altura, alturaMaxima);
        }

        public static Dimensao RetornarDimencaoLarguraMaxima(double largura, double altura, double larguraMaxima)
        {
            var novaLargura = larguraMaxima;
            var novaAltura = altura * novaLargura / largura;
            return new Dimensao(novaLargura, novaAltura);
        }

        public static Dimensao RetornarDimencaoUniformeFora(Dimensao dimensao, Dimensao dimensaoRecipiente)
        {
            return RetornarDimencaoUniformeFora(dimensao.Largura, dimensao.Altura, dimensaoRecipiente.Largura, dimensaoRecipiente.Altura);
        }

        public static Dimensao RetornarDimencaoUniformeFora(double largura, double altura, double larguraRecipiente, double alturaRecipiente)
        {
            var novaLargura = 0D;
            var novaAltura = 0D;

            if (largura > altura)
            {
                novaLargura = larguraRecipiente;
                novaAltura = altura * novaLargura / largura;
                if (novaAltura < alturaRecipiente)
                {
                    novaAltura = alturaRecipiente;
                    novaLargura = largura * novaAltura / altura;
                }
            }
            else if (altura > largura)
            {
                novaAltura = alturaRecipiente;
                novaLargura = largura * novaAltura / altura;

                if (novaLargura < larguraRecipiente)
                {
                    novaLargura = larguraRecipiente;
                    novaAltura = altura * novaLargura / largura;
                }
            }
            else if (largura == altura)
            {
                novaLargura = Math.Max(larguraRecipiente, alturaRecipiente);
                novaAltura = novaLargura;
            }
            return new Dimensao(novaLargura, novaAltura);
        }

        public static void ValidarDimensaoProporcional(int larguraOrigem,
                                                       int alturaOrigem,
                                                       int largura,
                                                       int altura)
        {
            var dimensao = RetornarDimencaoUniformeFora(larguraOrigem, alturaOrigem, largura, altura);
            var larguraUniforme = (int)Math.Round(dimensao.Largura);
            var alturaUniforme = (int)Math.Round(dimensao.Altura);

            if (larguraUniforme != largura ||
               alturaUniforme != altura)
            {
                throw new Erro($"A dimensão {larguraOrigem} x {alturaOrigem} não é proporcional a {largura} x {altura} ");
            }
        }

        public static Dimensao RetornarDimencaoUniformeDentro(Dimensao dimensao, Dimensao dimensaoRecipiente)
        {
            return RetornarDimencaoUniformeDentro(dimensao.Largura, dimensao.Altura, dimensaoRecipiente.Largura, dimensaoRecipiente.Altura);
        }

        public static Dimensao RetornarDimencaoUniformeDentro(double larguraImagem, double alturaImagem, double larguraMaxima, double alturaMaxima)
        {
            var novaLargura = 0D;
            var novaAltura = 0D;

            if (larguraImagem > alturaImagem)
            {
                //IMAGEM NA HORIZONTAL
                novaLargura = larguraMaxima;
                novaAltura = alturaImagem * novaLargura / larguraImagem;

                if (novaAltura > alturaMaxima)
                {
                    novaAltura = alturaMaxima;
                    novaLargura = larguraImagem * novaAltura / alturaImagem;
                }
            }
            else if (alturaImagem > larguraImagem)
            {
                //IMAGEM NA VERTICAL
                novaAltura = alturaMaxima;
                novaLargura = larguraImagem * novaAltura / alturaImagem;

                if (novaLargura > larguraMaxima)
                {
                    novaLargura = larguraMaxima;
                    novaAltura = alturaImagem * novaLargura / larguraImagem;
                }
            }
            else if (larguraImagem == alturaImagem)
            {
                //IMAGEM QUADRADA ' SELECIONAR O MENOR LADO
                novaLargura = Math.Min(alturaMaxima, larguraMaxima);
                novaAltura = novaLargura;
            }
            return new Dimensao(novaLargura, novaAltura);
        }
    }

}