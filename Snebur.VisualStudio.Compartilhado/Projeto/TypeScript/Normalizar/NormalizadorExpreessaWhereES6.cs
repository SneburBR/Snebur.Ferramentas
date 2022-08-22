using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Snebur.Utilidade;

namespace Snebur.VisualStudio
{
    internal class NormalizadorExpreessaWhereES6 : BaseNormalizadorExpreessaoWhere
    {
        public string DicionaroSerializado { get; private set; }

        internal override bool IsNormalizarExpressaoWhere
        {
            get
            {
                var expressao = this.Expressao;
                if (expressao.Contains("x =>") && !expressao.Contains(EXPRESSAO_NORMALIZADA))
                {
                    foreach (var metodo in NormalizadorExpreessaoWhere.MetodosIgnorar)
                    {
                        if (expressao.Contains(metodo))
                        {
                            return false;
                        }
                    }
                    return true;
                }
                return false;
            }
        }

        internal NormalizadorExpreessaWhereES6(string expressao) : base(expressao)
        {

        }

        internal override string Normalizar()
        {


            var expressao = this.Expressao;
            //throw new NotImplementedException("Não foi implementado");
            //var prefixo = ExpressaoUtil.RetornarExpressaoAbreFecha(expressao.Substring(1, expressao.Length - 2));
            var epxressaInterna = this.RetornarExpressaInterna(expressao);
            var dicionarioSerializado = this.RetornarDicionarioSerializado(epxressaInterna);
            var expressaoSemFechar = expressao.Substring(0, expressao.Length - 1);
            this.DicionaroSerializado = dicionarioSerializado;

            var retorno = $"{expressaoSemFechar}, {dicionarioSerializado})";
            return retorno;
        }

        private string RetornarExpressaInterna(string expressao)
        {
            var epxressaInterna = ExpressaoUtil.RetornarExpressaoAbreFecha(expressao, true, '(', ')');
            var posicaoMaior = epxressaInterna.IndexOf(">");
            if (posicaoMaior > 0)
            {
                epxressaInterna = epxressaInterna.Substring(posicaoMaior +1);
                if (epxressaInterna.EndsWith(";"))
                {
                    epxressaInterna = epxressaInterna.Substring(0, epxressaInterna.Length - 1).Trim();
                }
                return epxressaInterna;

            }
            throw new Erro("Falha a normalizar a expressao ES2020");



            //if (epxressaInterna.StartsWith("return"))
            //{
            //    epxressaInterna = epxressaInterna.Substring("return".Length).Trim();
            //}
            //if (epxressaInterna.EndsWith(";"))
            //{
            //    epxressaInterna = epxressaInterna.Substring(0, epxressaInterna.Length - 1).Trim();
            //}
            //return epxressaInterna;
        }

        private string RetornarDicionarioSerializado(string epxressaInterna)
        {
            var variaveis = this.RetornarVariaveis(epxressaInterna);
            var dicionario = variaveis.Distinct().ToDictionary(x => this.RetornarChaveVariavel(x));
            var serializacoes = dicionario.Select(x => $"{x.Key} : {x.Value}");
            return "{" + String.Join(",", serializacoes) + "}";
        }

        private string RetornarChaveVariavel(string nomeVariavel)
        {
            var sb = new StringBuilder();
            var leitor = new StringReader(nomeVariavel);
            sb.Append("$");
            sb.Append("_");
            int intCaracter;
            while (true)
            {
                intCaracter = leitor.Read();
                if (intCaracter == -1) break;

                var caracter = (char)intCaracter;
                //if (caracter == '.' || caracter == '(' || caracter == ')')
                if (!TextoUtil.IsLetraOuNumero(caracter))
                {
                    sb.Append("_");
                }
                else
                {
                    sb.Append(caracter);
                }
            }
            return sb.ToString();
        }



        private List<string> RetornarVariaveis(string epxressaInterna)
        {
            var variaveis = new List<string>();
            var leitor = new StringReader(epxressaInterna);
            int intCaracter;
            while (true)
            {
                intCaracter = leitor.Read();
                if (intCaracter == -1) break;
                var caracter = (char)intCaracter;

                if (this.IsOperador(leitor, caracter) || this.IsMetodo(leitor, caracter))
                {
                    caracter = (char)leitor.Read();

                    while (Char.IsWhiteSpace(caracter) || caracter == '(')
                    {
                        caracter = (char)leitor.Read();
                    }
                    var sb = new StringBuilder();

                    while (!Char.IsWhiteSpace(caracter) && caracter != ')' && caracter != '(')
                    {
                        sb.Append(caracter);
                        intCaracter = leitor.Read();
                        if (intCaracter == -1) break;
                        caracter = (char)intCaracter;
                    }
                    if (caracter == '(' && leitor.Peek() == ')')
                    {
                        sb.Append(caracter);
                        caracter = (char)leitor.Read();
                        sb.Append(caracter);
                    }
                    var nomeVariavel = sb.ToString();
                    variaveis.Add(nomeVariavel);
                }
            }
            return variaveis;
        }


        private bool IsOperador(StringReader leitor, char caracter)
        {
            var proximo = leitor.Peek();
            if (proximo == '=' && (caracter == '=' || caracter == '!') ||
               ((caracter == '>' || caracter == '<') && (proximo == ' ' || proximo == '=')))
            {
                while (leitor.Peek() == '=')
                {
                    leitor.Read();
                }
                return true;
            }
            return false;
        }

        private bool IsMetodo(StringReader leitor, char caracter)
        {
            switch (caracter)
            {
                case 'S':
                    return this.IsMetodoStartsWith(leitor, caracter);
                case 'E':

                    var proximo = leitor.Peek();
                    if (proximo == 'q')
                    {
                        return this.IsMetodoEquals(leitor, caracter);
                    }
                    else if (proximo == 'n')
                    {
                        return this.IsMetodoEndsWith(leitor, caracter);
                    }
                    return false;

                case 'C':
                    return this.IsMetodoContains(leitor, caracter);
                default:
                    return false;
            }
        }


        private bool IsMetodoEquals(StringReader leitor, char caracter)
        {
            return this.IsMetodo(leitor, caracter, METODO_EQUALS);
        }

        private bool IsMetodoStartsWith(StringReader leitor, char caracter)
        {
            return this.IsMetodo(leitor, caracter, METODO_STARTS_WITH);
        }

        private bool IsMetodoEndsWith(StringReader leitor, char caracter)
        {
            return this.IsMetodo(leitor, caracter, METODO_ENDS_WITH);
        }

        private bool IsMetodoContains(StringReader leitor, char caracter)
        {

            return this.IsMetodo(leitor, caracter, METODO_CONTAINS);
        }



        private bool IsMetodo(StringReader leitor, char caracter, string metodo)
        {
            var posicao = 0;
            while (posicao < metodo.Length)
            {
                if (caracter != metodo[posicao])
                {
                    return false;
                }
                posicao += 1;
                caracter = (char)leitor.Read();
            }
            return true;
        }

        public void Dispoese()
        {
            this.DicionaroSerializado = null;
            base.Dispose();
        }
    }
}
