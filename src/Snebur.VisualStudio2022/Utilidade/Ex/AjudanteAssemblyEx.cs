
using Community.VisualStudio.Toolkit;
using Snebur.Utilidade;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Snebur.VisualStudio
{
    internal class AjudanteAssemblyEx
    {
        internal static Assembly RetornarAssembly(EnvDTE.Project projeto)
        {
            //ThreadHelper.ThrowIfNotOnUIThread();

            var caminhoAssembly = RetornarCaminhoAssembly(projeto);
            return AjudanteAssembly.RetornarAssembly(caminhoAssembly);
        }

        internal static Assembly RetornarAssembly(Project projeto)
        {
            //ThreadHelper.ThrowIfNotOnUIThread();
            var caminhoAssembly = RetornarCaminhoAssembly(projeto);
            return AjudanteAssembly.RetornarAssembly(caminhoAssembly);
        }

        internal static string RetornarCaminhoAssembly(EnvDTE.Project projeto)
        {
            //ThreadHelper.ThrowIfNotOnUIThread();

            var assembly = projeto.Properties.Cast<EnvDTE.Property>().
                                              FirstOrDefault(x => x.Name == "AssemblyName");

            var nomeAssembly = (string)assembly?.Value ?? projeto.Name;
            var caminhoProjeto = new FileInfo(projeto.FileName).Directory.FullName;
            var tipoCsPro = TipoCsProjUtil.RetornarTipoCsProjet(projeto.FullName) ;
            return AjudanteAssembly.RetornarCaminhoAssembly(tipoCsPro,
                                                            caminhoProjeto,
                                                            nomeAssembly);
        }

        internal static string RetornarCaminhoAssembly(Project projeto)
        {
            var assemblyName = ThreadHelper.JoinableTaskFactory.Run(async () =>
            {
                return await projeto.GetAttributeAsync("AssemblyName");
            });

            var nomeAssembly = assemblyName ?? projeto.Name;
            var caminhoProjeto = new FileInfo(projeto.FullPath).Directory.FullName;
            var tipoCsPro = TipoCsProjUtil.RetornarTipoCsProjet(projeto.FullPath);
            return AjudanteAssembly.RetornarCaminhoAssembly(tipoCsPro,
                                                            caminhoProjeto,
                                                            nomeAssembly);
        }
    }
}
