using Community.VisualStudio.Toolkit;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Snebur.VisualStudio
{

    public static class SolutionsExtension
    {
        public async static Task<IEnumerable<Project>> GetStartupProjectsAsync(this Solutions solutions)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            var dte = await DteEx.GetDTEAsync();
            var sb = dte.Solution.SolutionBuild;
            var projectsName = ((Array)sb.StartupProjects).Cast<string>().Select(x => Path.GetFileNameWithoutExtension(x));
            var allProjects = await solutions.GetAllProjectsAsync();
            return allProjects.Where(x => projectsName.Contains(x.Name));
        }

        public async static Task SaveAllProjetsAsync(this Solutions solutions)
        {
            var allProjects = await solutions.GetAllProjectsAsync();
            foreach (var project in allProjects)
            {
                await project.SaveAsync();
            }

        }

        public async static Task<PhysicalFile> AddExistingFileAsync(this PhysicalFolder folder, string fullPath)
        {
            var arquivos = await folder.AddExistingFilesAsync(fullPath);
            return arquivos.FirstOrDefault();
        }

        public static PhysicalFile GetPhysicalFile(this Project project, string fullPath)
        {
            return SolutionUtil.GetPhysicalFile(project.Children, fullPath);
        }
    }

}