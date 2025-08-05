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
            menuItem.Enabled = project != null && project is Project && this.IsProjetoCsCharp(project.FullPath);
        }

        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e, SolutionItem item)
        {
            if (item is Project project)
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                var buildType = await SolutionUtil.GetCurrentBuildConfigurationAsync();
                var tipoCompilacao = PublicacaoUtil.RetornarTipoCompilacao(buildType);

                if (tipoCompilacao == EnumTipoCompilacao.Custom)
                {
                    LogVSUtil.LogErro("Tipo de compilação não suportado. Utilize Debug ou Release");
                    return;
                }

                //var project = await VS.Solutions.GetActiveProjectAsync();
                if (this.IsProjetoCsCharp(project?.FullPath))
                {
                    try
                    {
                        var nomeProjeto = project.Name;
                        var caminhoProjeto = project.FullPath;
                        var diretorioProjeto = Path.GetDirectoryName(project.FullPath);
                         
                        var tempo = Stopwatch.StartNew();
                        var tipoProjeto = PublicacaoUtil.RetornarTipoProjeto(diretorioProjeto);

                        if (tipoProjeto == EnumTipoProjeto.ExtensaoVisualStudio)
                        {
                            VsixUtil.IncrementarVersaoExtensaoVisualStudio(diretorioProjeto);
                        }
                        else
                        {
                            ProjetoUtil.IncrementarVersao(caminhoProjeto, true);
                        }

                        if (ProjetoUtil.IsProjetoTypescript(diretorioProjeto))
                        {
                            await OutputWindow.NormalizarProjetosReferenciasAsync();
                        }

                        GerenciadorProjetos.Instancia.DesativarEventosBuild();

                        if (await VS.Build.BuildProjectAsync(project, BuildAction.Build))
                        {
                            var solution = await VS.Solutions.GetCurrentSolutionAsync();
                            var caminhoSolution = solution.FullPath;

                            await PublicacaoUtil.PublicarVersaoAsync(
                                nomeProjeto,
                                tipoProjeto,
                                tipoCompilacao,
                                caminhoProjeto,
                                caminhoSolution,
                                tempo);
                        }

                    }
                    catch (Exception ex)
                    {
                        LogVSUtil.LogErro(ex);

                    }
                    finally
                    {
                        GerenciadorProjetos.Instancia.AtivarEventosBuild();
                    }
                }
            }

        }
         
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
