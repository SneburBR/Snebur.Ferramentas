using Community.VisualStudio.Toolkit;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Snebur.VisualStudio.MenuSnebur
{
    [Command(PackageGuids.guidCtxMenuSneburCmdSetString, PackageIds.PublishCtxMenuCommand)]
    internal sealed class PublishCtxMenuCommand : BaseDynamicCommand<PublishCtxMenuCommand, SolutionItem>
    {
        protected override IReadOnlyList<SolutionItem> GetItems()
        {
            return ThreadHelper.JoinableTaskFactory.Run(async () =>
            {
                var item = await VS.Solutions.GetActiveItemAsync();
                return new SolutionItem[] { item };
            });
        }

        protected override void BeforeQueryStatus(OleMenuCommand menuItem, EventArgs e, SolutionItem project)
        {
            menuItem.Visible = true;
            menuItem.Enabled = project != null && project is ProjectTK && this.IsProjetoCsCharp(project.FullPath);
        }

        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e, SolutionItem item)
        {
            if(item is ProjectTK project)
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                //var project = await VS.Solutions.GetActiveProjectAsync();
                if (this.IsProjetoCsCharp(project?.FullPath))
                {
                    try
                    {
                        var caminhoProjeto = Path.GetDirectoryName(project.FullPath);
                        var caminhoAssemblyInfo = AssemblyInfoUtil.RetornarCaminhoAssemblyInfo(caminhoProjeto);

                        if (!File.Exists(caminhoAssemblyInfo))
                        {
                            LogVSUtil.LogErro($"O arquivo da versão {caminhoAssemblyInfo} não foi encontrado");
                            return;
                        }

                        var tempo = Stopwatch.StartNew();

                        AssemblyInfoUtil.InscrementarVersao(caminhoAssemblyInfo);
                        var versao = AssemblyInfoUtil.RetornarVersaoAssemblyInfo(caminhoAssemblyInfo);
                        var nomeProjeto = Path.GetFileNameWithoutExtension(project.FullPath);
                        LogVSUtil.OutputGeral($"Versão do projeto {nomeProjeto} incrementada {versao}");

                        var tipoProjeto = PublicacaoUtil.RetornarTipoProjeto(caminhoProjeto);

                        if (tipoProjeto == EnumTipoProjeto.ExtensaoVisualStudio)
                        {
                            PublicacaoUtil.AtribuirVersaoExtensaoVisualStudio(versao, caminhoProjeto);
                        }

                        if (ProjetoUtil.IsProjetoTypescript(caminhoProjeto))
                        {
                            await OutputWindow.Instance?.NormalizarProjetosReferenciasAsync();
                        }

                        if (await VS.Build.BuildProjectAsync(project, BuildAction.Build))
                        {
                            var isDebug = true;
                            var caminhoDestino = PublicacaoUtil.PublicarVersao(tipoProjeto,
                                                                               caminhoProjeto,
                                                                               isDebug);
                            if (Directory.Exists(caminhoDestino))
                            {
                                LogVSUtil.Sucesso($"Arquivos publicados  {caminhoDestino}", tempo);
                                Process.Start(caminhoDestino);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LogVSUtil.LogErro(ex);
                    }
                }
            }
          
        }

        //protected override IReadOnlyList<ProjectTK> GetItems()
        //{
        //    return ThreadHelper.JoinableTaskFactory.Run(async () =>
        //    {
        //        var project = await VS.Solutions.GetActiveProjectAsync();
        //        return new Community.VisualStudio.Toolkit.Project[] { project };
        //    });
        //}

        //protected override void BeforeQueryStatus(OleMenuCommand menuItem, EventArgs e,
        //                                          Community.VisualStudio.Toolkit.Project project)
        //{
        //    if (project != null && this.IsProjetoCsCharp(project.FullPath))
        //    {
        //        menuItem.Visible = menuItem.Enabled = true;
        //        return;
        //    }
        //}

        private bool IsProjetoCsCharp(string fullName)
        {
            if (fullName is not null)
            {
                var extensao = Path.GetExtension(fullName);
                return extensao.Equals(".csproj", StringComparison.InvariantCultureIgnoreCase);
            }
            return false;
        }


    }
}
