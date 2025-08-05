using EnvDTE;
using EnvDTE80;
using System.Threading.Tasks;

namespace Snebur.VisualStudio
{

    public static class DteUtil
    {
      

        public static async Task<DTE2> GetDTEAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            return Package.GetGlobalService(typeof(DTE)) as DTE2;
        }

        public static string GetCurrentBuildConfiguration(this DTE2 dte)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (dte == null)
            {
                throw new ArgumentNullException(nameof(dte));
            }

            // Get the active solution configuration
            SolutionBuild solutionBuild = dte.Solution.SolutionBuild;
            if (solutionBuild == null)
            {
                throw new InvalidOperationException("No active solution.");
            }

            string currentConfiguration = solutionBuild.ActiveConfiguration.Name;
            return currentConfiguration; // Returns "Debug", "Release", or custom configuration names
        }
    }
}