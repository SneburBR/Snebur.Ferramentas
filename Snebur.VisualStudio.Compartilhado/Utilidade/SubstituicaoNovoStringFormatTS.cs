using Snebur.Linq;
using Snebur.Utilidade;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Snebur.VisualStudio
{
    public class SubstituicaoNovoStringFormatTS
    {
        public const string PESQUISAR = "String.Format(";
        public string Conteudo { get; }
        public bool IsCsharp { get; }

        public SubstituicaoNovoStringFormatTS(string conteudo,
                                              bool isCsharp)
        {
            this.Conteudo = conteudo;
            this.IsCsharp = isCsharp;
        }
        internal string RetornarConteudo()
        {
            var temp = this.Conteudo;
            var conteudo = this.Conteudo;
            int posicao;

            while ((posicao = temp.IndexOf(PESQUISAR)) > 0)
            {
                temp = temp.Substring(posicao);

                var (antigo, novo, isSucesso) = this.NoralizarStringFormat(temp);
                if (isSucesso)
                {
                    conteudo = conteudo.Replace(antigo, novo);
                    LogVSUtil.Alerta($"Substituindo : {antigo} " +
                                     $"                {novo}");
                }
                temp = temp.Substring(antigo.Length);
            }
            return conteudo;
        }

        private (string, string, bool) NoralizarStringFormat(string conteudo)
        {
            var expressaoAntigoFormat = this.RetornarExpressaoStringFormat(conteudo);
            var conteudoString = ExpressaoUtil.RetornarExpressaoAbreFecha(expressaoAntigoFormat, true, '"', '"');

            var varaveis = this.RetornarVariaveis(expressaoAntigoFormat );

            var expressoesPosicao = ExpressaoUtil.RetornarExpressoesAbreFecha(conteudoString, false, '{', '}');

            try
            {
                this.ValidarVariaveisExpressoesValidas(expressaoAntigoFormat,
                                                       expressoesPosicao,
                                                       varaveis);

                var novoStringFormat = conteudoString;

                foreach (var expressaoPosicao in expressoesPosicao)
                {
                    var strPosicaoVariavel = expressaoPosicao.Substring(1, expressaoPosicao.Length - 2);
                    if (Int32.TryParse(strPosicaoVariavel, out var posicaoVariavel))
                    {
                        if (posicaoVariavel >= varaveis.Count)
                        {
                            throw new Erro("A posição da variavel não existe");
                        }
                        var variavel = varaveis[posicaoVariavel].Trim();
                        var prefixo = IsCsharp ? String.Empty : "$";
                        var novaVariavel = $"{prefixo}{{{variavel.Trim()}}}";
                        novoStringFormat = novoStringFormat.Replace(expressaoPosicao, novaVariavel);
                    }
                    else
                    {
                        throw new Erro($"Não foi possivel formatar  expressa {expressaoAntigoFormat}");
                    }
                }
                if (IsCsharp)
                {
                    novoStringFormat = $"$\"{novoStringFormat}\"";
                }
                else
                {
                    novoStringFormat = $"`{novoStringFormat}`";
                }


                return (expressaoAntigoFormat,
                         novoStringFormat,
                         true);

            }
            catch (Exception ex)
            {
                LogVSUtil.LogErro(ex.Message);

                return (expressaoAntigoFormat,
                        null,
                       false);
            }
        }

        private void ValidarVariaveisExpressoesValidas(string expressao, List<string> expressoesPosicao, List<string> varaveis)
        {
            var possicoes = expressoesPosicao.Select(x => x.Substring(1, x.Length - 2));
            if (possicoes.All(x => int.TryParse(x, out int _)))
            {
                var posicoesInt = possicoes.Select(x => Int32.Parse(x)).ToHashSet();
                if (posicoesInt.Count == varaveis.Count)
                {
                    if (varaveis.Count == 0 || varaveis.Count == (posicoesInt.Max() + 1))
                    {
                        return;
                    }
                }
            }
            throw new Erro($" Não foi possível converter a expressão {expressao}");
        }

        private string RetornarExpressaoStringFormat(string conteudo)
        {
            var expressao = ExpressaoUtil.RetornarExpressaoAbreFecha(conteudo, false);
            var fim = PESQUISAR.Length + expressao.Length - 1;
            var retorno = conteudo.Substring(0,
                                            fim);
            return retorno;
        }

        private List<string> RetornarVariaveis(string expressaoAntigoFormat)
        {
            var expressaoInterna = ExpressaoUtil.RetornarExpressaoAbreFecha(expressaoAntigoFormat, true);
            var str = ExpressaoUtil.RetornarExpressaoAbreFecha(expressaoInterna, false, '"', '"');

            var posicao = expressaoInterna.IndexOf(str);
            var temp = expressaoInterna.Substring(posicao + str.Length).Trim();

            if (String.IsNullOrEmpty(temp))
            {
                return new List<string>();
            }
            if (!temp.StartsWith(","))
            {
                throw new Erro($"Não é possivel retornar as variaveis de {expressaoAntigoFormat}");
            }
            temp = temp.Substring(1);
            var partes = temp.Split(',');
            return partes.Select(x => x.Trim()).
                          ToList();
        }


    }
}