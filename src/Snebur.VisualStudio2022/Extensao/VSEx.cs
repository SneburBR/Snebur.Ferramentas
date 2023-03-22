using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Snebur.VisualStudio
{

    public static class DteEx
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

        //public static string TryGetName(this Property property)
        //{
        //    try
        //    {
        //        ThreadHelper.ThrowIfNotOnUIThread();
        //        return property.Name;
        //    }
        //    catch
        //    {
        //        return null;
        //    }
        //}
        //public static object TryGetValue(this Property property)
        //{
        //    try
        //    {
        //        ThreadHelper.ThrowIfNotOnUIThread();
        //        return property.Value;
        //    }
        //    catch
        //    {
        //        return null;
        //    }
        //}

    }
}