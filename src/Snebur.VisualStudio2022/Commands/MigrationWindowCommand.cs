using Community.VisualStudio.Toolkit;

namespace Snebur.VisualStudio
{
    [Command(PackageIds.SneburMigrationCommand)]
    internal sealed class MigrationWindowCommand : BaseCommand<MigrationWindowCommand>
    {
        protected override Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            return MigrationWindow.ShowAsync();
        }
    }
}
