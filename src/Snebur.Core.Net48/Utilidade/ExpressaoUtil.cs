using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Snebur.Utilidade
{
    public static class ExpressaoUtil
    {
        public static string INCLUIR = "Incluir";
        //public static string INCLUIR = "IncluirTipado";

        public static List<string> RetornarExpressoesAbreFecha(string conteudo,
                                                              bool isRemoverAbreFechaExtremidades = false,
                                                              char abre = '(',
                                                              char fecha = ')')
        {

            var expressoes = new List<string>();

            using (var leitor = new StringReader(conteudo))
            {
                while (true)
                {
                    var expressao = RetornarExpressaoAbreFecha(leitor,
                                                                         isRemoverAbreFechaExtremidades,
                                                                         abre,
                                                                         fecha,
                                                                         true);
                    if (!String.IsNullOrWhiteSpace(expressao))
                    {
                        expressoes.Add(expressao);
                    }
                    else
                    {
                        break;
                    }
                }

            }

            return expressoes;
        }
        public static string RetornarExpressaoAbreFecha(string expressao, bool removerAbreFechaExtremidades = false, char abre = '(', char fecha = ')', bool ignorarErro = false)
        {
            return RetornarExpressaoAbreFecha(new StringReader(expressao), removerAbreFechaExtremidades, abre, fecha, ignorarErro);
        }
        public static string RetornarExpressaoAbreFecha(TextReader leitor, bool removerAbreFecha = false, char abre = '(', char fecha = ')', bool ignorarErro = false)
        {
            var sb = new StringBuilder();
            var aberto = false;
            var contatorParentesAbre = 0;
            var isIgual = abre == fecha;
            int intCaracter;
            var isAbreIgualEncontrado = false;

            while (true)
            {
                intCaracter = leitor.Read();
                if (intCaracter == -1)
                {
                    break;
                }
                var caracter = (char)intCaracter;
                if (caracter == abre && !isAbreIgualEncontrado)
                {

                    if (!aberto)
                    {
                        aberto = true;
                        if (isIgual)
                        {
                            isAbreIgualEncontrado = true;
                            contatorParentesAbre += 1;
                        }
                    }
                    contatorParentesAbre += 1;
                }
                if (aberto)
                {
                    sb.Append(caracter);

                    if (caracter == fecha)
                    {
                        contatorParentesAbre -= 1;
                        if (contatorParentesAbre == 0)
                        {
                            var resultado = sb.ToString();
                            if (removerAbreFecha)
                            {
                                return resultado.Substring(1, resultado.Length - 2).Trim();
                            }
                            return resultado;
                        }
                    }
                }
            }
            if (!ignorarErro)
            {
                throw new Erro($"Não foram encontrados na expressao  , abre = '{abre}' e fecha = '{fecha}'");
            }
            return String.Empty;
        }

        public static string RetornarExpressaoAbreFechaObsoleto(string expressao, char abre = '(', char fecha = ')', bool ignorarErro = false)
        {
            var len = expressao.Length;
            var aberto = false;
            var posicaoAbre = -1;
            var posicaoFecha = -1;

            var contatorParentesAbre = 0;

            for (var i = 0; i < len; i++)
            {
                var caracter = expressao[i];
                if (caracter == '<')
                {
                    if (!aberto)
                    {
                        aberto = true;
                        posicaoAbre = i;
                    }
                    contatorParentesAbre += 1;
                }
                if (aberto)
                {
                    if (caracter == '>')
                    {
                        contatorParentesAbre -= 1;
                        if (contatorParentesAbre == 0)
                        {
                            posicaoFecha = i;
                            var contar = posicaoFecha - posicaoAbre;
                            var retorno = expressao.Substring(posicaoAbre, contar + 1);
                            return retorno;
                        }
                    }
                }
            }
            if (!ignorarErro)
            {
                throw new Erro($"Não foram encontrados na expressao {expressao}, abre = '{abre}' e fecha = '{fecha}'");
            }
            return String.Empty;
        }

        public static PropertyInfo RetornarPropriedade(Expression expressao, bool permitirMetodos = true)
        {
            return RetornarPropriedades(expressao, permitirMetodos).Last();
        }

        public static List<PropertyInfo> RetornarPropriedades(Expression expressao, bool permitirMetodos = true)
        {
            var propriedades = new List<PropertyInfo>();
            while (expressao != null)
            {
                if (expressao is LambdaExpression)
                {
                    var descricaoExpressao = expressao.ToString();
                    var lambdaExpression = (LambdaExpression)expressao;

                    switch (lambdaExpression.Body.NodeType)
                    {
                        case ExpressionType.Convert:

                            var unaryExpression = (UnaryExpression)lambdaExpression.Body;
                            expressao = unaryExpression;
                            break;

                        case ExpressionType.Invoke:

                            var invocationExpression = (InvocationExpression)lambdaExpression.Body;
                            expressao = invocationExpression;
                            break;

                        case ExpressionType.MemberAccess:

                            var memberExpression = (MemberExpression)lambdaExpression.Body;
                            expressao = memberExpression;
                            break;

                        default:

                            var mensagemCorpoNaoSuportado = String.Format("O tipo do corpo da expressão não é suportado {0} ", EnumUtil.RetornarDescricao(lambdaExpression.Body.NodeType));
                            throw new ErroNaoSuportado(mensagemCorpoNaoSuportado);
                    }
                }
                else
                {
                    expressao = RetornarExpressaoInterna(expressao, propriedades);
                }
            }
            propriedades.Reverse();
            return propriedades;
        }

        public static Expression RetornarExpressaoInterna(Expression expressao, List<PropertyInfo> propriedades, bool permitirMetodos = true)
        {
            if (expressao is UnaryExpression)
            {
                var unaryExpression = (UnaryExpression)expressao;
                return unaryExpression.Operand;
            }
            if (expressao is ParameterExpression)
            {
                var parameterExpression = (ParameterExpression)expressao;
                return null;
            }
            if (expressao is InvocationExpression)
            {
                var invocationExpression = (InvocationExpression)expressao;
                return invocationExpression.Expression;
            }
            if (expressao is MethodCallExpression)
            {
                var methodCallExpression = (MethodCallExpression)expressao;
                var nomeMetodo = methodCallExpression.Method.Name;

                if (!permitirMetodos || (nomeMetodo != INCLUIR && nomeMetodo != "IncluirTipado"))
                {
                    throw new ErroNaoSuportado(String.Format("O Método {0} não é suportado na expressao {0} ", expressao.ToString()));
                }
                return expressao = methodCallExpression.Arguments.First();
            }
            if (expressao is MemberExpression)
            {
                var memberExpression = (MemberExpression)expressao;
                if (!(memberExpression.Member is PropertyInfo))
                {
                    var mensagemMembroNaoPropriedade = String.Format("O tipo do corpo de expressão não é suportado {0} ", expressao.ToString());
                    throw new ErroNaoSuportado(mensagemMembroNaoPropriedade);
                }
                propriedades.Add((PropertyInfo)memberExpression.Member);

                return memberExpression.Expression;
            }
            var mensagemExpressaoNaoSuportado = String.Format("O tipo de expressão não é suportado {0} : {1} ", expressao.GetType().Name, expressao.ToString());
            throw new ErroNaoSuportado(mensagemExpressaoNaoSuportado);
        }

        public static Expression[] RetornarExpressoes<TEntidade>(params Expression<Func<TEntidade, object>>[] expressoes)
        {
            return expressoes;
        }

        public static Expression RetornarExpressao<T>(Expression<Func<T, object>> expressao)
        {
            return expressao;
        }

        public static string RetornarTextoEntrarApas(string linhaVersao)
        {
            return RetornarTextosEntraApas(linhaVersao).SingleOrDefault();
        }

        public static List<string> RetornarTextosEntraApas(string expressao)
        {
            //"\"([^\"]*)\""
            var reg = new Regex("\".*?\"");
            var resultados = reg.Matches(expressao);
            var texto = new List<string>();
            foreach (var resultado in resultados)
            {
                var r = resultado.ToString();
                texto.Add(r.Substring(1, r.Length - 2));
            }
            return texto;
        }
    }
}