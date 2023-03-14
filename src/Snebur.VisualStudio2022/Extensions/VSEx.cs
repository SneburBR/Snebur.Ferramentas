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

    public static class PropertiesExtensao
    {

        public static List<PropriedadeViewModel> RetornarPropriedadesViewModel(this Properties properties)
        {
            var propriedadesViewModel = new List<PropriedadeViewModel>();
            return propriedadesViewModel;
            
            //if (!ThreadHelper.CheckAccess())
            //{
            //    return propriedadesViewModel;
            //}

            //foreach (var property in properties)
            //{
            //    try
            //    {
            //        if (property is Property propertyTipada)
            //        {
            //            var name = propertyTipada.TryGetName();
            //            var value = propertyTipada.TryGetValue();
            //            if (!String.IsNullOrWhiteSpace(name))
            //            {
            //                propriedadesViewModel.Add(new PropriedadeViewModel(name, value));
            //            }
            //        }
            //    }
            //    catch
            //    {

            //    }
            //}
            //return propriedadesViewModel;
        }

        public static string TryGetName(this Property property)
        {
            try
            {
                return property.Name;
            }
            catch
            {
                return null;
            }
        }
        public static object TryGetValue(this Property property)
        {
            try
            {
                return property.Value;
            }
            catch
            {
                return null;
            }
        }

    }
}