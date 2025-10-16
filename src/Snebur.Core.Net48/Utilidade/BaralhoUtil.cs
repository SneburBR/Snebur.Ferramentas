using System;
using System.Linq;

namespace Snebur.Utilidade
{
    public partial class BaralhoUtil
    {
        private const int CHAVE_PADRAO = 17003812;

        public static string Embaralhar(string texto)
        {
            return Embaralhar(texto, CHAVE_PADRAO);

        }
        public static string Embaralhar(string texto, int chave)
        {
            if (String.IsNullOrEmpty(texto))
            {
                return texto;
            }
            return new string(Embaralhar(texto.ToArray(), chave));
        }

        public static T[] Embaralhar<T>(T[] conteudo)
        {
            return Embaralhar(conteudo, CHAVE_PADRAO);
        }

        public static T[] Embaralhar<T>(T[] conteudo, int chave)
        {
            var tamanhos = conteudo.Length;
            var combinacoes = RetornarCombinacao(tamanhos, chave);
            for (int i = tamanhos - 1; i > 0; i--)
            {
                var n = combinacoes[tamanhos - 1 - i];
                var tmp = conteudo[i];
                conteudo[i] = conteudo[n];
                conteudo[n] = tmp;
            }
            return conteudo;
        }

        public static string Desembaralhar(string texto)
        {
            if (String.IsNullOrEmpty(texto))
            {
                return texto;
            }
            return new string(Desembaralhar(texto.ToArray(), CHAVE_PADRAO));
        }

        public static T[] Desembaralhar<T>(T[] conteudo)
        {
            return Desembaralhar(conteudo, CHAVE_PADRAO);
        }
        public static T[] Desembaralhar<T>(T[] conteudo, int chave)
        {
            var tramanho = conteudo.Length;
            var combinacoes = RetornarCombinacao(tramanho, chave);
            for (int i = 1; i < tramanho; i++)
            {
                var n = combinacoes[tramanho - i - 1];
                var tmp = conteudo[i];
                conteudo[i] = conteudo[n];
                conteudo[n] = tmp;
            }
            return conteudo;
        }

        private static int[] RetornarCombinacao(int tamanho, int chave)
        {
            var retorno = new int[tamanho - 1];
            var random = new SnRandom(chave);
            for (int i = tamanho - 1; i > 0; i--)
            {
                var trocar = random.Next(i + 1);
                retorno[tamanho - 1 - i] = trocar;
            }
            return retorno;
        }
    }
}
