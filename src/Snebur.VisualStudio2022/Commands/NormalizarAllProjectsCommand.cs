using Community.VisualStudio.Toolkit;
using EnvDTE;

namespace Snebur.VisualStudio.Commands
{
    [Command(PackageIds.NormalizarAllProjectsCommand)]
    internal sealed class NormalizarAllProjectsCommand : BaseCommand<NormalizarAllProjectsCommand>
    {
        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            await OutputWindow.NormalizarProjetosReferenciasAsync();
        }
    }
}
