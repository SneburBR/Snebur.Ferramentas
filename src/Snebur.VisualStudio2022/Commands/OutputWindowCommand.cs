using Community.VisualStudio.Toolkit;

namespace Snebur.VisualStudio
{
    [Command(PackageIds.SneburOutputCommand)]
    internal sealed class OutputWindowCommand : BaseCommand<OutputWindowCommand>
    {
        protected override Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            return OutputWindow.ShowAsync();
        }
    }
}
