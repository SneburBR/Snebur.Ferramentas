using EnvDTE;
using EnvDTE80;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Snebur.Utilidade;

namespace Snebur.VisualStudio
{
    internal class DeclaracaoComponentes
    {
        //const string INICIO_REGION_DECLARACAO_APRESENTACAO_ANTIGA = "//#region Apresentecao Gerado automaticamente Extensao Snebur";
        const string INICIO_REGION_DECLARACAO_APRESENTACAO = "//#region Elementos da apresentação - código gerado automaticamente #";
        const string FIM_REGION_DECLARACAO_APRESENTACAO = "//#endregion";
        public DTE2 DTE { get; }
        public string CaminhoApresentacao { get; }
        public Document DocumentoCodigo { get; }
        private string HtmlApresentacao { get; }
        public string ConteudoCodigo { get; }
        public string NomeClasse { get; private set; }

        internal DeclaracaoComponentes(DTE2 dte,
                                       string caminhoApresentacao,
                                       string htmlApresentacao,
                                       Document documentoCodigo)
        {
            this.DTE = dte;
            this.CaminhoApresentacao = caminhoApresentacao;
            this.DocumentoCodigo = documentoCodigo;
            this.HtmlApresentacao = htmlApresentacao;
            this.ConteudoCodigo = this.RetornarConteudoCodigo();
            this.NomeClasse = this.RetornarNomeClasse();
        }

        private string RetornarConteudoCodigo()
        {
            if (this.DocumentoCodigo?.Selection is TextSelection selecao)
            {
                selecao.SelectAll();
                return selecao.Text;
            }
            return String.Empty;

        }

        private string RetornarNomeClasse()
        {
            var procurar = " class ";
            var linhas = this.ConteudoCodigo.ToLines();
            foreach (var linha in linhas)
            {
                if (!linha.StartsWith("//") && linha.Contains(procurar))
                {
                    var posicao = linha.IndexOf(" class ");
                    var temp = linha.Substring(posicao + procurar.Length).Trim();
                    var posicaoEspaco = temp.IndexOf(' ');

                    var nomeClasse = temp.Substring(0, posicaoEspaco);

                    var posicaoAbreGenerics = temp.IndexOf('<');
                    if (posicaoAbreGenerics > -1 && posicaoAbreGenerics < posicaoEspaco)
                    {
                        nomeClasse = nomeClasse.Substring(0, posicaoAbreGenerics);
                        var parteGenerics = ExpressaoUtil.RetornarExpressaoAbreFecha(temp, false, '<', '>');
                        return nomeClasse + parteGenerics;
                    }
                    return nomeClasse;


                }
            }
            return String.Empty;
        }

        internal void Declarar()
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();

            if (!String.IsNullOrEmpty(this.NomeClasse))
            {
                var conteudoDeclaracao = this.RetornarConteudoDeclaracao();

                if (this.DocumentoCodigo.Selection is TextSelection selecao)
                {
                    selecao.StartOfDocument();

                    if (!selecao.FindText(INICIO_REGION_DECLARACAO_APRESENTACAO))
                    {
                        this.InserirRegionDeclaracao(selecao);
                    }
                    selecao.SelectAll();
                    var x = selecao.Text;


                    selecao.StartOfDocument();
                    if (selecao.FindText(INICIO_REGION_DECLARACAO_APRESENTACAO))
                    {
                        selecao.SelectLine();

                        while (!selecao.Text.Contains(FIM_REGION_DECLARACAO_APRESENTACAO))
                        {
                            selecao.Delete();
                            if (selecao.ActivePoint.AtEndOfDocument)
                            {
                                break;
                            }
                            //selecao.LineDown(true);
                            selecao.SelectLine();
                        }
                        selecao.Delete();
                        selecao.Insert(conteudoDeclaracao);

                        selecao.StartOfDocument();

                        var isClasse = selecao.FindText("export class") ||
                                       selecao.FindText("export abstract class");

                    }
                }

            }

        }

        private void InserirRegionDeclaracao(TextSelection selecao)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();

            var sb = new StringBuilder();
            sb.AppendLine("");
            sb.AppendLine("");
            sb.AppendLine($"\t{INICIO_REGION_DECLARACAO_APRESENTACAO}");
            sb.AppendLine("\t//declarações aqui");
            sb.AppendLine($"\t{FIM_REGION_DECLARACAO_APRESENTACAO}");

            var declaracao = sb.ToString();

            selecao.EndOfDocument();
            selecao.SelectLine();

            while (!selecao.Text.Contains("}"))
            {
                selecao.LineUp();
                selecao.SelectLine();
            }
            selecao.LineUp();
            selecao.EndOfLine();
            selecao.Insert(declaracao);
        }

        private string RetornarConteudoDeclaracao()
        {
            var nomesTipo = this.RetornarNomesTipo();
            var sb = new StringBuilder();
            sb.AppendLine($"\t{INICIO_REGION_DECLARACAO_APRESENTACAO}");
            sb.AppendLine();
            sb.AppendLine($"\texport interface {this.NomeClasse}");
            sb.AppendLine("\t{");
            foreach (var (chave, tipo) in nomesTipo.Select(x => (x.Key, x.Value)))
            {
                sb.AppendLine($"\t\treadonly {chave}: {tipo};");
            }
            sb.AppendLine("\t}");
            sb.AppendLine();
            sb.AppendLine($"\t{FIM_REGION_DECLARACAO_APRESENTACAO}");
            return sb.ToString();
        }

        private Dictionary<string, string> RetornarNomesTipo()
        {
            var retorno = new Dictionary<string, string>();
            var elementosComNome = this.RetornarElementosComNome();
            foreach (var (nome, elemento) in elementosComNome.Select(x => (x.Key, x.Value)))
            {
                var tipo = this.RetornarTipo(elemento);
                retorno.Add(nome, tipo);
            }
            return retorno;
        }

        private string RetornarTipo(HtmlNode elemento)
        {
            var tagName = elemento.Name.Trim().ToLower();
            if (tagName == TagElementoUtil.TAB_CONTROLE_USUARIO)
            {
                if (elemento.Attributes[TagElementoUtil.ATRIBUTO_CONTROLE] != null)
                {
                    var tipo = elemento.Attributes[TagElementoUtil.ATRIBUTO_CONTROLE].Value;
                    if (!String.IsNullOrEmpty(tipo))
                    {
                        return tipo;
                    }
                }
            }
            return TagElementoUtil.RetornarTipo(tagName);
        }

        private Dictionary<string, HtmlNode> RetornarElementosComNome()
        {
            var documentoHtml = new HtmlDocument();
            documentoHtml.LoadHtml(this.HtmlApresentacao);

            var elementos = documentoHtml.DocumentNode.Descendants().
                                          Where(x => x != null &&
                                                     x.NodeType == HtmlNodeType.Element &&
                                                     x.Attributes["sn-nome"] != null &&
                                                     x.Attributes["sn-nome"].Value?.Length > 0).ToList();

            var retorno = new Dictionary<string, HtmlNode>();
            foreach (var elemento in elementos)
            {
                var tagName = elemento.Name;
                var nome = elemento.Attributes["sn-nome"].Value;
                if (!String.IsNullOrEmpty(nome))
                {
                    if (retorno.ContainsKey(nome))
                    {
                        LogVSUtil.LogErro($"O elemento {elemento.Name} sn-nome='{nome}' está duplicado em {this.CaminhoApresentacao}");
                        continue;
                    }
                    retorno.Add(nome, elemento);
                }
            }
            return retorno;
        }
    }


}
