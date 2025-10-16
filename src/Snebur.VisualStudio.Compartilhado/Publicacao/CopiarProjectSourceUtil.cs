using Snebur.Utilidade;
using Snebur.VisualStudio;
using System;
using System.IO;
using System.Linq;

namespace Snebur.Cli
{
    internal class CopiarProjectSourceUtil
    {
        private static string[] PatternIgnorar = new string[] {
            "bin",
            "packages",
            "obj",
            "lib",
            ".vs",
            ".git",
            ".vscode",
            ".github",
            "*.g.cs",
            "*.g.i.cs",
            "*.dll",
            "*.pdb",
            "*.exe",
            "*.zip",
            "*.nupkg",
            "*.vsix",
        };

        public static void Copiar(string caminhoProjetoOrigem,
                                  string caminhoSolution,
                                  string diretorioDestino)
        {
            var caminhosReferencias = ProjetoUtil.RetornarTodosCaminhosProjetosReferenciados(caminhoProjetoOrigem);

            if (!ProjetoUtil.IsProjetoCsCharp(caminhoProjetoOrigem))
            {
                throw new ArgumentException($"O caminho informado não é de um projeto C# {caminhoProjetoOrigem}");
            }

            var diretorioProjetos = caminhosReferencias.Select(diretorio => Path.GetDirectoryName(diretorio))
                                                       .Distinct()
                                                       .ToList();

            var diretorioBase = DiretorioUtil.RetornarDiretorioPai(diretorioProjetos);

            foreach (var caminhoProjeto in caminhosReferencias)
            {
                var diretorioProjeto = Path.GetDirectoryName(caminhoProjeto);

                var caminhoRelatorio = CaminhoUtil.RetornarCaminhoRelativo(diretorioProjeto, diretorioBase);
                var diretorioProjetoDestino = Path.Combine(diretorioDestino, caminhoRelatorio);
                DiretorioUtil.CriarDiretorio(diretorioProjetoDestino);
                DiretorioUtil.CopiarDiretorio(diretorioProjeto,
                                              diretorioProjetoDestino,
                                              true,
                                              IsCopiar);

                var caminhoReferenciasExternas = ProjetoUtil.RetornarReferenciasExternas(caminhoProjeto);
                if(caminhoReferenciasExternas.Count> 0)
                {
                    foreach (var caminhoReferenciaHintPath in caminhoReferenciasExternas)
                    {
                        var caminhoReferencia = Path.Combine(diretorioProjeto, caminhoReferenciaHintPath);
                        var caminhoRelatorioReferencia = CaminhoUtil.RetornarCaminhoRelativo(caminhoReferencia, diretorioBase);
                        var caminhoReferenciaDestino = Path.Combine(diretorioDestino, caminhoRelatorioReferencia);

                        if(CaminhoUtil.CaminhoIgual(caminhoReferencia, caminhoReferenciaDestino))
                        {
                            continue;
                        }

                        ArquivoUtil.CopiarArquivo(caminhoReferencia, caminhoReferenciaDestino, true, true);
                    }
                }
            }

            var caminhoRelatorioSolution = CaminhoUtil.RetornarCaminhoRelativo(caminhoSolution, diretorioBase);
            var caminhoSolutionDestino = Path.Combine(diretorioDestino, caminhoRelatorioSolution);
            ArquivoUtil.CopiarArquivo(caminhoSolution, caminhoSolutionDestino, true);

        }

        public static bool IsCopiar(FileInfo arquivo)
        {
            string nome = arquivo.Name;
            foreach (var padrao in PatternIgnorar)
            {
                if (padrao.StartsWith("*") && padrao.EndsWith("*"))
                {
                    // Contém uma substring
                    if (nome.Contains(padrao.Trim('*')))
                        return false;
                }
                else if (padrao.StartsWith("*"))
                {
                    // Termina com
                    if (nome.EndsWith(padrao.TrimStart('*'), StringComparison.OrdinalIgnoreCase))
                        return false;
                }
                else if (padrao.EndsWith("*"))
                {
                    // Começa com
                    if (nome.StartsWith(padrao.TrimEnd('*'), StringComparison.OrdinalIgnoreCase))
                        return false;
                }
                else
                {
                    // Comparação exata
                    if (string.Equals(nome, padrao, StringComparison.OrdinalIgnoreCase))
                        return false;
                }
            }

            // Verifica se algum diretório no caminho corresponde a algum padrão ignorado
            string[] diretoriosNoCaminho = arquivo.FullName
                        .Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar)
                        .Split(Path.DirectorySeparatorChar);

            foreach (var diretorio in diretoriosNoCaminho)
            {
                if (PatternIgnorar.Any(padrao =>
                    string.Equals(diretorio, padrao, StringComparison.OrdinalIgnoreCase)))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
