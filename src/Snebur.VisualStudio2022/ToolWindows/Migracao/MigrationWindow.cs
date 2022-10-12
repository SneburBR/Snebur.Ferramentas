using Community.VisualStudio.Toolkit;
using Microsoft.VisualStudio.Imaging;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Snebur.VisualStudio
{
    public class MigrationWindow : BaseToolWindow<MigrationWindow>
    {
        public static MigrationWindowControl Instance { get; private set; }
        public override string GetTitle(int toolWindowId) => "Snebur migrations";

        public override Type PaneType => typeof(Pane);

        public override Task<FrameworkElement> CreateAsync(int toolWindowId, CancellationToken cancellationToken)
        {
            Instance = new MigrationWindowControl();
            return Task.FromResult<FrameworkElement>(Instance);
        }

        [Guid("fcb6c96c-3393-4152-9d4e-9fcfb4e99ff9")]
        //[Guid("620b7a5a-74e1-43d9-b55f-2b34700ca858")]
        internal class Pane : ToolWindowPane
        {
            public Pane()
            {
                BitmapImageMoniker = KnownMonikers.ToolWindow;
            }
        }
    }
}