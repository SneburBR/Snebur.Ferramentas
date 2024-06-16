using Community.VisualStudio.Toolkit;
using Snebur.Utilidade;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Snebur.VisualStudio
{
    public static class SolutionUtil
    {
        private static bool _isDefinidoProjetoInicializacao = false;
        private static string[] IgrnorarPastas = new string[] { "wwwroot", "build", "bin", "obj", ".vs", ".git" };

        public static async Task DefinirProjetosInicializacaoAsync()
        {
            if (_isDefinidoProjetoInicializacao)
            {
                return;
            }

            try
            {
                _isDefinidoProjetoInicializacao = true;
                var startupProjects = await VS.Solutions.GetStartupProjectsAsync();
                if (startupProjects?.Count() > 0)
                {
                    SolutionUtil.DefinirProjetosInicializacao(startupProjects);
                }
            }
            catch (Exception ex)
            {
                LogVSUtil.LogErro(ex);
            }
            finally
            {
                _isDefinidoProjetoInicializacao = false;
            }
            
        }

        private static void DefinirProjetosInicializacao(IEnumerable<Project> startupProjects)
        {
            foreach (var startupProject in startupProjects)
            {
                var diretorioProjeto = Path.GetDirectoryName(startupProject.FullPath);
                var caminhoTS = Path.Combine(diretorioProjeto, ConstantesProjeto.CONFIGURACAO_TYPESCRIPT);
                if (File.Exists(caminhoTS))
                {
                    try
                    {
                        ProjetoTypescriptInitUtil.SetDiretorioProjeto(diretorioProjeto);
                    }
                    catch (Exception ex)
                    {
                        LogVSUtil.LogErro(ex);
                    }
                }
            }
        }

        public static async Task<HashSet<string>> RetornarTodosArquivosAsync(Project project, bool isLowerCase)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            var projectRecuperado = await VS.Solutions.FindProjectsAsync(project.Text);
            var arquivos = new HashSet<string>();

            project = projectRecuperado ?? project;

            VarrerTodosArquivos(arquivos, project.Children, isLowerCase);
            return arquivos;

        }
        private static void VarrerTodosArquivos(HashSet<string> arquivos,
                                               IEnumerable<SolutionItem> items,
                                               bool isLowerCase)
        {
            if (items != null && items.Count() > 0)
            {
                foreach (SolutionItem item in items)
                {
                    switch (item)
                    {
                        case PhysicalFile physicalFile:

                            var nomeArquivo = Path.GetFileName(physicalFile.FullPath).ToLower();
                            var isNonVisibleItem = physicalFile.IsNonVisibleItem;
                            if (!isNonVisibleItem)
                            {
                                var caminhoArquivo = isLowerCase ? physicalFile.FullPath.ToLower() : physicalFile.FullPath;
                                arquivos.Add(caminhoArquivo);
                                if (physicalFile.Children.Count() > 0)
                                {
                                    VarrerTodosArquivos(arquivos, item.Children, isLowerCase);
                                }
                            }

                            break;
                        case PhysicalFolder physicalFolder:

                            if (IsIgnorarPasta(physicalFolder.Text))
                            {
                                continue;
                            }
                            VarrerTodosArquivos(arquivos, item.Children, isLowerCase);
                            break;
                        case SolutionFolder:

                            VarrerTodosArquivos(arquivos, item.Children, isLowerCase);
                            break;
                        case VirtualFolder:

                            VarrerTodosArquivos(arquivos, item.Children, isLowerCase);
                            break;
                        case Project:
                        case Solution:

                            LogVSUtil.Alerta($"Analisar VarrerTodosArquivos {item.GetType().Name} ");
                            VarrerTodosArquivos(arquivos, item.Children, isLowerCase);
                            break;
                        default:

                            //LogVSUtil.Alerta($"Analisar VarrerTodosArquivos default {item.Type} ");
                            //VarrerTodosArquivos(arquivos, item.Children, isLowerCase);
                            break;
                    }
                }
            }
        }


        private static bool IsIgnorarPasta(string pasta)
        {
            return IgrnorarPastas.Any(x
                => x.Equals(pasta, StringComparison.InvariantCultureIgnoreCase));
        }

        internal static string RetornarChave(Project projeto)
        {
            return BaseProjeto.RetornarChave(projeto.FullPath);
        }

        public static async Task DeletarBinAndObjAsync()
        {
            var projetosVS = await VS.Solutions.GetAllProjectsAsync();
            foreach (var projeto in projetosVS)
            {
                var caminhoProjeto = Path.GetDirectoryName(projeto.FullPath);
                var pastaBin = Path.Combine(caminhoProjeto, "bin");
                var pastaObj = Path.Combine(caminhoProjeto, "obj");

                DiretorioUtil.ExcluirTodosArquivo(pastaBin, true, true);
                DiretorioUtil.ExcluirTodosArquivo(pastaObj, true, true);
            }
        }

        internal static Task<PhysicalFolder> GetPhysicalFolderAsync(IEnumerable<SolutionItem> children,
                                                                    string fullPath)
        {
            return SolutionUtil.GetSolutionItemAsync<PhysicalFolder>(children, fullPath);
        }

        internal static Task<PhysicalFile> GetPhysicalFileAsync(IEnumerable<SolutionItem> children, string fullPath)
        {
            return SolutionUtil.GetSolutionItemAsync<PhysicalFile>(children, fullPath);
        }

        internal static async Task<TSolutionItem> GetSolutionItemAsync<TSolutionItem>(IEnumerable<SolutionItem> children,
                                                                    string fullPathOrName,
                                                                    bool isExcludeNonVisible = true) where TSolutionItem : SolutionItem
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            return await GetSolutionItemInternoAsync<TSolutionItem>(children, fullPathOrName, isExcludeNonVisible);
        }
        private static async Task<TSolutionItem> GetSolutionItemInternoAsync<TSolutionItem>(IEnumerable<SolutionItem> children,
                                                                                            string fullPathOrName,
                                                                                            bool isExcludeNonVisible) where TSolutionItem : SolutionItem
        {
            foreach (var item in children)
            {
                if (isExcludeNonVisible && item.IsNonVisibleItem)
                {
                    continue;
                }

                if (item is TSolutionItem solutionItem)
                {
                    if (solutionItem.Text?.Equals(fullPathOrName, StringComparison.InvariantCultureIgnoreCase) == true)
                    {
                        return solutionItem;
                    }

                    if (solutionItem.FullPath?.Equals(fullPathOrName, StringComparison.InvariantCultureIgnoreCase) == true)
                    {
                        return solutionItem;
                    }

                   
                }

                if (item.Type != SolutionItemType.VirtualFolder &&
                    item.Children?.Count() > 0)
                {
                    var child = await GetSolutionItemInternoAsync<TSolutionItem>(item.Children,
                                                                                 fullPathOrName,
                                                                                 isExcludeNonVisible);
                    if (child != null)
                    {
                        return child;
                    }
                }
            }
            return null;
        }

    }
}