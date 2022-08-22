using System;
using System.Collections.Generic;
using System.Linq;

namespace Snebur.VisualStudio.Utilidade
{
    public static class FormatarDocumentoUtil
    {
        private static HashSet<string> Modificadores { get; } = new HashSet<string>() { "public", "private", "internal", "protected" };
        public static HashSet<string> ExtensoesSuportadas { get; } = new HashSet<string> { ".cs", ".ts" };

        public static string RetornarConteudoFormatado(string conteudo,
                                                       bool isCsharp)
        {
            var conteudoFormatado = RetornarConteudoFormatadoInterno(conteudo);

            var formatar = new SubstituicaoNovoStringFormatTS(conteudoFormatado, 
                                                              isCsharp);
            return formatar.RetornarConteudo();
        }
        private static string RetornarConteudoFormatadoInterno(string conteudo)
        {
            var linhas = conteudo.ToLines();
            var linhasFormatada = new List<string>();

            for (var i = 0; i < linhas.Count; i++)
            {
                var linha = linhas[i];
                if (i > 0 && i < (linhas.Count - 1))
                {

                    var linhaAnterior = linhas[i - 1];
                    var proximaLinha = linhas[i + 1];

                    if (String.IsNullOrWhiteSpace(linha))
                    {
                        if (String.IsNullOrWhiteSpace(linhaAnterior))
                        {
                            continue;
                        }
                        if (linhaAnterior.Trim() == "{")
                        {
                            continue;
                        }

                        if (linhaAnterior.Trim() == "}")
                        {
                            var modificadorProximaLinha = proximaLinha.Trim().Split(' ').First();
                            if (!FormatarDocumentoUtil.Modificadores.Contains(modificadorProximaLinha))
                            {
                                continue;
                            }
                        }


                        if (proximaLinha.Trim() == "}" ||
                            proximaLinha.Trim() == "};" ||
                            proximaLinha.Trim() == "})" ||
                            proximaLinha.Trim() == "});")
                        {
                            continue;
                        }


                    }
                    else if (linha.Trim() == "}" && (i < (linhas.Count - 1)))
                    {
                        var modificadorProximaLinha = proximaLinha.Trim().Split(' ').First();
                        if (FormatarDocumentoUtil.Modificadores.Contains(modificadorProximaLinha))
                        {
                            linhasFormatada.Add(linha);
                            linhasFormatada.Add("");
                            continue;
                        }
                    }
                }
                linhasFormatada.Add(linha);
            }

            var conteudoFormato = String.Join(System.Environment.NewLine, linhasFormatada);
            return conteudoFormato.Trim(); ;
        }

        //internal static void FormatarTodosDocumentos()
        //{
        //    var projetos = ProjetoUtil.RetornarProjetosVisualStudio();
        //    var contar = 0;
        //    foreach (var projeto in projetos)
        //    {
        //        var todosArquivos = ProjetoUtil.RetornarTodosArquivos(projeto, false);
        //        foreach (var caminhoArquivo in todosArquivos)
        //        {
        //            var arquivo = new FileInfo(caminhoArquivo);
        //            if (FormatarDocumentoUtil.ExtensoesSuportadas.Contains(arquivo.Extension) && arquivo.Exists)
        //            {
        //                var conteudo = ArquivoUtil.LerTexto(caminhoArquivo, true).Trim();
        //                var conteudoFormato = FormatarDocumentoUtil.RetornarConteudoFormatado(conteudo);
        //                if (conteudo != conteudoFormato)
        //                {
        //                    LogVSUtil.Log($"Arquivo {arquivo.Name} formato");
        //                    ArquivoUtil.SalvarArquivoTexto(caminhoArquivo, conteudoFormato, true);
        //                    contar += 1;
        //                }
        //            }
        //        }
        //    }
        //    LogVSUtil.Log($"Formatação concluida: {contar} arquivos formatados");
        //}
    }

}
