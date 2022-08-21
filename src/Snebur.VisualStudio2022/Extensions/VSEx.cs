using Community.VisualStudio.Toolkit;
using EnvDTE;
using EnvDTE80;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Snebur.VisualStudio
{
    public static class SolutionsExtension
    {
        public async static Task<IEnumerable<Community.VisualStudio.Toolkit.Project>> GetStartupProjectsAsync(this Solutions solutions)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            var dte = await VSEx.GetDTEAsync();
            var sb = dte.Solution.SolutionBuild;
            var projectsName = ((Array)sb.StartupProjects).Cast<string>().Select(x => Path.GetFileNameWithoutExtension(x));
            var allProjects = await solutions.GetAllProjectsAsync();
            return allProjects.Where(x => projectsName.Contains(x.Name));
        }
    }

    public static class VSEx
    {
        public static async Task<DTE2> GetDTEAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            return Package.GetGlobalService(typeof(DTE)) as DTE2;
        }
    }


}