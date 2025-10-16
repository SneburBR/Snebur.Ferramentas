using Snebur.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Snebur.Utilidade
{
    public static partial class TextoUtil
    {
        public static bool IsIgual(string conteudo,
                                   string comparar,
                                   bool isIgnorarLinhasEmBranco,
                                   bool isTrim)
        {
            var linhasConteudo = conteudo.ToLines(isIgnorarLinhasEmBranco);
            var linhasComparar = comparar.ToLines(isIgnorarLinhasEmBranco);

            if (linhasConteudo.Count != linhasComparar.Count)
            {
                return false;
            }

            for (var i = 0; i < linhasConteudo.Count; i++)
            {
                var linha = linhasConteudo[i];
                var linhaComparar = linhasComparar[i];

                if (isTrim)
                {
                    if (linha.Trim() != linhaComparar.Trim())
                    {
                        return false;
                    }
                }
                else
                {
                    if (linha != linhaComparar)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public static string RemoverEspacos(string text)
        {
            //replace all spaces
            return new Regex("\\s+").Replace(text, "");
        }

        private class TextoUtilConstantes
        {
            private const string NUMEROS = "0123456789";
            private const string LETRAS = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefhgijklmnopqrstuvwxyz";
            private const string CARACTERES_PADRAO = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789ÀÁÂÃÈÉÊÌÍÎÒÓÔÕÚÛÜÇÑàáâãäèéêëìíîòóôõùúûüçñ-_.@,()+=;:~^`´&! ";

            //private const string ACENTOS = "ÀÁÂÃÈÉÊÌÍÎÒÓÔÕÚÛÜÇÑàáâãäèéêëìíîòóôõùúûüçñ";
            //private const string ACENTOS_MAPEADOS = "AAAAEEEIIIOOOOUUUCNaaaaaeeeeiiioooouuuucn";

            private const string ACENTOS = "ÀÁÂÃÈÉÊÌÍÎÒÓÔÕÚÛÜÇÑàáâãäèéêëìíîòóôõùúûüçñ´~^¨`";
            private const string ACENTOS_MAPEADOS = "AAAAEEEIIIOOOOUUUCNaaaaaeeeeiiioooouuuucn     ";
            private const string PONTOS_SINAIS = "+-.,()";
            private const string PONTOS_SINAIS_SIMBOLOS = ",.;:?!|+-_.,@~^`´&$#*/\\§%|(){}[]<>";

            private static HashSet<char> _numeros;
            private static HashSet<char> _letras;
            private static HashSet<char> _letrasNumeros;
            private static HashSet<char> _caracteresPadrao;
            private static HashSet<char> _linhasTabulacoes;
            private static HashSet<char> _pontosSinais;
            private static HashSet<char> _numerosPontosSinais;
            private static Dictionary<char, char> _acentosMapeado;

            internal static HashSet<char> Numeros => LazyUtil.RetornarValorLazy(
                    ref _numeros,
                    () => NUMEROS.ToArray().ToHashSet());
            internal static HashSet<char> PontosSinais => LazyUtil.RetornarValorLazy(
                    ref _pontosSinais,
                    () => PONTOS_SINAIS.ToArray().ToHashSet());

            internal static HashSet<char> NumerosPontosSinais => LazyUtil.RetornarValorLazy(
                    ref _numerosPontosSinais,
                    () => (NUMEROS + PONTOS_SINAIS).ToArray().ToHashSet());

            internal static HashSet<char> NumerosPontosSinaisSimbolos => LazyUtil.RetornarValorLazy(
                    ref _numerosPontosSinais,
                    () => (NUMEROS + PONTOS_SINAIS_SIMBOLOS).ToArray().ToHashSet());

            internal static HashSet<char> Letras => LazyUtil.RetornarValorLazy(
                     ref _letras,
                     () => LETRAS.ToArray().ToHashSet());

            internal static HashSet<char> LetrasNumeros => LazyUtil.RetornarValorLazy(
                    ref _letrasNumeros,
                    () => String.Concat(LETRAS, NUMEROS).ToArray().ToHashSet());

            internal static HashSet<char> CaracteresPadrao => LazyUtil.RetornarValorLazy(
                    ref _caracteresPadrao,
                    () => CARACTERES_PADRAO.ToArray().ToHashSet());

            internal static HashSet<char> LinhasTabulacoes => LazyUtil.RetornarValorLazy(
                    ref _linhasTabulacoes,
                    () =>
                    {
                        var retorno = new HashSet<char>();
                        foreach (var c in Environment.NewLine.ToArray())
                        {
                            retorno.Add(c);
                        }
                        char tab = '\u0009';
                        retorno.Add(tab);
                        return retorno;
                    });

            internal static Dictionary<char, char> AcentosMapeado => LazyUtil.RetornarValorLazy(
                    ref _acentosMapeado,
                    () =>
                    {
                        if (ACENTOS.Length != ACENTOS_MAPEADOS.Length)
                        {
                            throw new Erro("O números de ACENTOS é diferentes do ACENTOS_MAPEADOS mapeados");
                        }
                        var retorno = new Dictionary<char, char>();
                        var len = ACENTOS.Length;

                        for (var i = 0; i < len; i++)
                        {
                            var caracter = ACENTOS[i];
                            var caracterMepado = ACENTOS_MAPEADOS[i];
                            retorno.Add(caracter, caracterMepado);
                        }
                        return retorno;
                    });
        }
    }
}