global using Microsoft.VisualStudio.Shell;
global using System;
//global using ProjectTK = Community.VisualStudio.Toolkit.Project;
//global using Project = EnvDTE.Project;
global using Task = System.Threading.Tasks.Task;
using Community.VisualStudio.Toolkit;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Snebur.Utilidade;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Snebur.VisualStudio
{
    [Guid(PackageGuids.guidSneburVisualStudio2022String)]
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration(Vsix.Name, Vsix.Description, Vsix.Version)]
    [ProvideAutoLoad(UIContextGuids80.SolutionHasSingleProject, PackageAutoLoadFlags.BackgroundLoad)]
    [ProvideAutoLoad(UIContextGuids80.SolutionHasMultipleProjects, PackageAutoLoadFlags.BackgroundLoad)]
    [ProvideAutoLoad(UIContextGuids80.NoSolution, PackageAutoLoadFlags.BackgroundLoad)]
    [ProvideAutoLoad(UIContextGuids80.SolutionExists, PackageAutoLoadFlags.BackgroundLoad)]
    [ProvideAutoLoad(UIContextGuids80.EmptySolution, PackageAutoLoadFlags.BackgroundLoad)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideToolWindow(typeof(OutputWindow.Pane), Style = VsDockStyle.Tabbed, Window = WindowGuids.OutputWindow)]
    [ProvideToolWindowVisibility(typeof(OutputWindow.Pane), VSConstants.UICONTEXT.SolutionHasSingleProject_string)]
    [ProvideToolWindowVisibility(typeof(OutputWindow.Pane), VSConstants.UICONTEXT.SolutionHasMultipleProjects_string)]
    [ProvideToolWindowVisibility(typeof(OutputWindow.Pane), VSConstants.UICONTEXT.NoSolution_string)]
    [ProvideToolWindowVisibility(typeof(OutputWindow.Pane), VSConstants.UICONTEXT.EmptySolution_string)]
    [ProvideToolWindowVisibility(typeof(OutputWindow.Pane), VSConstants.UICONTEXT.SolutionOpening_string)]
    [ProvideOptionPage(typeof(DialogPageProvider.Geral), "Snebur", "Geral", 0, 0, true, 0, ProvidesLocalizedCategoryName = false)]
    [ProvideProfile(typeof(DialogPageProvider.Geral), "Snebur", "Geral", 0, 0, true)]
    [ProvideToolWindow(typeof(MigrationWindow.Pane), Style = VsDockStyle.AlwaysFloat, Width = 800, Height = 400)]
    public sealed class SneburVisualStudio2022Package : ToolkitPackage
    {
        public static bool IsVsixInialized { get; private set; }

        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            var app = new AplicacaoVisualStudio();

            await this.RegisterCommandsAsync();
            this.RegisterToolWindows();
            
            try
            {
                await GerenciadorProjetos.Instancia.InicializarAsync(this);
                //InstalarItensTemplate.Instalar();
                //await HtmlIntellisense.InicializarAsync();
                //JsonUtil.Serializar(true, true);
                _ = Task.Factory.StartNew(() =>
                 {
                     try
                     {
                         AppDomain.CurrentDomain.UnhandledException += this.This_UnhandledException;
                         Application.Current.DispatcherUnhandledException += this.This_DispatcherUnhandledException;
                     }
                     catch
                     {

                     }
                     
                 }, cancellationToken, TaskCreationOptions.None, TaskScheduler.Default);

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
            e.Handled = true;
            LogUtil.ErroAsync(e.Exception);
        }
    }
}