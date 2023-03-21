using Community.VisualStudio.Toolkit;
using Microsoft.VisualStudio.Imaging;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Snebur.VisualStudio
{
    public class OutputWindow : BaseToolWindow<OutputWindow>
    {
        public static OutputWindowControl Instance { get; private set; }
        public override string GetTitle(int toolWindowId) => "Snebur output";

        public override Type PaneType => typeof(Pane);

        public override Task<FrameworkElement> CreateAsync(int toolWindowId, CancellationToken cancellationToken)
        {
            Instance = new OutputWindowControl();
            return Task.FromResult<FrameworkElement>(Instance);
        }

        internal static Task NormalizarProjetosReferenciasAsync()
        {
            if (Instance == null) return Task.CompletedTask;
            return Instance.NormalizarProjetosReferenciasAsync();
        }

        internal static Task OcuparAsync()
        {
            if (Instance == null) return Task.CompletedTask;
            return Instance.OcuparAsync();
        }

        internal static Task AtualizarEstadoServicoDepuracaoAsync()
        {
            if (Instance == null) return Task.CompletedTask;
            return Instance.AtualizarEstadoServicoDepuracaoAsync();
        }

        internal static Task DesocuparAsync()
        {
            if (Instance == null) return Task.CompletedTask;
            return Instance.DesocuparAsync();
        }

        [Guid("15f26c1c-d719-44fd-ab37-805acd9430a5")]
        //[Guid("0fdbfc2f-fb91-4a98-9cd6-3a62c467be44")]
        internal class Pane : ToolWindowPane
        {
            public Pane()
            {
                BitmapImageMoniker = KnownMonikers.ToolWindow;
            }
        }
    }
}