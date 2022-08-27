using Community.VisualStudio.Toolkit;
using EnvDTE;
using EnvDTE80;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Snebur.VisualStudio.Commands
{
    [Command(PackageIds.GoToLayoutCommand)]
    internal sealed class GoToLayoutCommand : BaseCommand<GoToLayoutCommand>
    {
        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            var dte = await VSEx.GetDTEAsync();
            if (dte.ActiveDocument != null)
            {

                var nomeArquivo = dte.ActiveDocument.Name;
                //if (!(nomeArquivo.EndsWith(".shtml.ts") || nomeArquivo.EndsWith(".shtml.scss")))
                if (!nomeArquivo.EndsWith(".ts") && !nomeArquivo.EndsWith(".shtml.scss"))
                {
                    dte.ExecuteCommand("View.ViewDesigner");
                    return;
                }

                if (nomeArquivo.EndsWith(".shtml.ts") || nomeArquivo.EndsWith(".shtml.scss"))
                {
                    var caminhoArquivoAtual = dte.ActiveDocument.FullName;
                    if (caminhoArquivoAtual.EndsWith(".ts"))
                    {
                        caminhoArquivoAtual = caminhoArquivoAtual.Substring(0, caminhoArquivoAtual.Length - 3);
                    }

                    if (caminhoArquivoAtual.EndsWith(".scss"))
                    {
                        caminhoArquivoAtual = caminhoArquivoAtual.Substring(0, caminhoArquivoAtual.Length - 5);
                    }

                    var caminhoLayout = caminhoArquivoAtual;
                    if (File.Exists(caminhoLayout))
                    {
                        dte.AbrirArquivo(caminhoLayout);
                    }

                }

                if (nomeArquivo.EndsWith(".ts"))
                {
                    var arquivosLayout = this.RetornarArquivosLayout(dte);
                    if (arquivosLayout.Count == 1)
                    {
                        dte.AbrirArquivo(arquivosLayout.Single());
                    }
                    if (arquivosLayout.Count > 0)
                    {
                        await this.LogAcaoMultplosLayoutsAsync(dte, arquivosLayout);
                    }
                }
            }
        }

        private async Task LogAcaoMultplosLayoutsAsync(DTE2 dte, List<string> arquivosLayout)
        {
            await OutputWindow.ShowAsync();
            OutputWindow.Instance?.LimparLogs();
            LogVSUtil.Log($"Foram encontrados {arquivosLayout.Count} arquivos de apresentacao ");
            foreach (var arquivo in arquivosLayout)
            {
                LogVSUtil.LogAcaoLink(Path.GetFileName(arquivo), () =>
                {
                    dte.AbrirArquivo(arquivo);
                });
            }
        }

        private List<string> RetornarArquivosLayout(DTE2 dte)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var retorno = new List<string>();
            var projectItemPai = dte.ActiveDocument.ProjectItem.RetornarProjectItemPai();
            if (projectItemPai.Name.EndsWith(".shtml"))
            {
                if (projectItemPai.FileCount > 0)
                {
                    retorno.Add(projectItemPai.FileNames[0]);
                    return retorno;
                }
            }
            else
            {
                foreach (ProjectItem item in projectItemPai.ProjectItems)
                {
                    if (item.Name.EndsWith(".shtml"))
                    {
                        if (item.FileCount > 0)
                        {
                            retorno.Add(item.FileNames[0]);
                        }
                    }
                }
            }
            return retorno;
        }
    }
}
