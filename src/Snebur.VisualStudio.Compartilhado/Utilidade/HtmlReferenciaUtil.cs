using System;
using System.Collections.Generic;
using Snebur.Utilidade;
using System.IO;
using System.Web;

namespace Snebur.VisualStudio
{
    public static class HtmlReferenciaUti
    {
        internal const string NOME_ARQUIVO_HTML_REFENCIAS = "Html.Referencias.ts";

        internal static string RetornarRepositorioHtmlsReferencias(FileInfo arquivo)
        {
            var diretorioAtual = arquivo.Directory;
            while (diretorioAtual != null)
            {
                var caminhoHtmlRefencia = Path.Combine(diretorioAtual.FullName, NOME_ARQUIVO_HTML_REFENCIAS);
                if (File.Exists(caminhoHtmlRefencia))
                {
                    return caminhoHtmlRefencia;
                }
                diretorioAtual = diretorioAtual.Parent;
            }

            throw new Exception(String.Format("O arquivo {0} não foi encontrado ", NOME_ARQUIVO_HTML_REFENCIAS));

        }

      

        private static string RetornarHtmlEncode(string html)
        {
            html = html.Replace("\r", " ");
            html = html.Replace("\n", " ");
            html = html.Replace("\t", " ");

            while (html.Contains("  "))
            {
                html = html.Replace("  ", " ");
            }
            var resultado = HttpUtility.HtmlEncode(html.Trim());
            return resultado;
        }

        internal static object RetornarUrlDesenvolvimentoAbsoluta(ArquivoTypeScript arquivoTypeScript, FileInfo arquivoHtmlReferencia)
        {
            var urlDesenvolvimento = arquivoTypeScript.ConfiguracaoProjetoTypeScript.UrlDesenvolvimento;
            var caminhoRelatio = HtmlReferenciaUti.RetornarUrlDesenvolvimentoRelativa(arquivoTypeScript, arquivoHtmlReferencia);

            if (!urlDesenvolvimento.EndsWith("/"))
            {
                urlDesenvolvimento += "/";
            }
            return urlDesenvolvimento + caminhoRelatio;
        }

        internal const string DIRETORIO_TYPE_SCRIPTS = "TypeScripts";
        internal static object RetornarUrlDesenvolvimentoRelativa(ArquivoTypeScript arquivoTypeScript, FileInfo arquivoHtmlReferencia)
        {
            var pastas = new List<string>();
            var caminhoProjeto = arquivoTypeScript.CaminhoProjeto;
            var diretorio = arquivoTypeScript.Arquivo.Directory;


            while (!ArquivoUtil.DiretorioIgual(diretorio.FullName, caminhoProjeto))
            {
                if (diretorio.Name == DIRETORIO_TYPE_SCRIPTS)
                {
                    pastas.Add(DIRETORIO_TYPE_SCRIPTS);
                    break;
                }

                pastas.Add(diretorio.Name);
                diretorio = diretorio.Parent;
                if (diretorio == null)
                {
                    throw new DirectoryNotFoundException("O caminho do projeto não foi encontrado " + caminhoProjeto);
                }
            }
            pastas.Reverse();

            var urlRelativa = String.Join("/", pastas);
            return urlRelativa + "/" + arquivoHtmlReferencia.Name;
        }
    }
}
