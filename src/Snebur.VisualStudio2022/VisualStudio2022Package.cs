global using Microsoft.VisualStudio.Shell;
global using System;
global using ProjectTK = Community.VisualStudio.Toolkit.Project;
global using Project = EnvDTE.Project;
global using Task = System.Threading.Tasks.Task;
using Community.VisualStudio.Toolkit;
using Microsoft.VisualStudio;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using Snebur.Utilidade;

namespace Snebur.VisualStudio
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration(Vsix.Name, Vsix.Description, Vsix.Version)]
    [ProvideOptionPage(typeof(DialogPageProvider.Geral), "Snebur", "Geral", 0, 0, true, 0, ProvidesLocalizedCategoryName = false)]
    [ProvideProfile(typeof(DialogPageProvider.Geral), "Snebur", "Geral", 0, 0, true)]
    [ProvideToolWindow(typeof(OutputWindow.Pane), Style = VsDockStyle.Tabbed, Window = WindowGuids.OutputWindow)]
    [ProvideToolWindow(typeof(MigrationWindow.Pane), Style = VsDockStyle.Float, Window = WindowGuids.MainWindow)]
    [ProvideToolWindowVisibility(typeof(OutputWindow.Pane), VSConstants.UICONTEXT.SolutionHasSingleProject_string)]
    //[ProvideToolWindowVisibility(typeof(OutputWindow.Pane), VSConstants.UICONTEXT.SolutionHasSingleProject_string)]
    [ProvideToolWindowVisibility(typeof(OutputWindow.Pane), VSConstants.UICONTEXT.SolutionHasMultipleProjects_string)]
    [ProvideToolWindowVisibility(typeof(OutputWindow.Pane), VSConstants.UICONTEXT.NoSolution_string)]
    [ProvideToolWindowVisibility(typeof(OutputWindow.Pane), VSConstants.UICONTEXT.EmptySolution_string)]
    [ProvideToolWindowVisibility(typeof(OutputWindow.Pane), VSConstants.UICONTEXT.SolutionOpening_string)]

    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(PackageGuids.guidSneburVisualStudio2022String)]
    public sealed class SneburVisualStudio2022Package : ToolkitPackage
    {
        public static bool IsVsixInialized { get; private set; }

        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            AplicacaoSnebur.Atual = new AplicacaoVisualStudio();

            await this.RegisterCommandsAsync();
            this.RegisterToolWindows();

            try
            {
                await GerenciadorProjetos.Instancia.InicializarAsync(this);
                //InstalarItensTemplate.Instalar();
                //await HtmlIntellisense.InicializarAsync();
                //JsonUtil.Serializar(true, true);
                AppDomain.CurrentDomain.UnhandledException += this.This_UnhandledException;
                Application.Current.DispatcherUnhandledException += this.This_DispatcherUnhandledException;
            }
            catch (Exception ex)
            {
                LogVSUtil.LogErro(ex);
            }
            finally
            {
                SneburVisualStudio2022Package.IsVsixInialized = true;
            }
        }

        private void This_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception ex)
            {
                LogUtil.ErroAsync(ex);
            }
        }

        private void This_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            LogUtil.ErroAsync(e.Exception);
        }
    }
}