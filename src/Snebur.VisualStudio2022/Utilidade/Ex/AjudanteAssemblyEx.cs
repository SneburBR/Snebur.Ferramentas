using EnvDTE;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Snebur.VisualStudio
{
    internal class AjudanteAssemblyEx
    {
        internal static Assembly RetornarAssembly(Project projeto)
        {
            //ThreadHelper.ThrowIfNotOnUIThread();

            var caminhoAssembly = RetornarCaminhoAssembly(projeto);
            return AjudanteAssembly.RetornarAssembly(caminhoAssembly);
        }

        internal static string RetornarCaminhoAssembly(Project projeto)
        {
           //ThreadHelper.ThrowIfNotOnUIThread();

            var assembly = projeto.Properties.Cast<Property>().
                                              FirstOrDefault(x => x.Name == "AssemblyName");

            var nomeAssembly = (string)assembly?.Value ?? projeto.Name;
            var caminhoProjeto = new FileInfo(projeto.FileName).Directory.FullName;
            return AjudanteAssembly.RetornarCaminhoAssembly(caminhoProjeto, nomeAssembly);
        }
    }
}
