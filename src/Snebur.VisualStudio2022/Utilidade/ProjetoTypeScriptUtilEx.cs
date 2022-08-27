using EnvDTE;
using System.Collections.Generic;
using System.IO;

namespace Snebur.VisualStudio
{
    public static partial class ProjetoTypeScriptUtilEx
    {
        public static HashSet<string> RetornarArquivosTypeScript(ProjectItems items)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var arquivos = new HashSet<string>();
            foreach (ProjectItem item in items)
            {
                if (item.FileCount > 0)
                {
                    for (short i = 0; i < item.FileCount; i++)
                    {
                        var arquivo = item.FileNames[i];
                        
                        if (Path.GetExtension(arquivo) == ConstantesProjeto.EXTENSAO_TYPESCRIPT)
                        {
                            //var is_dirty = item.IsDirty;
                            //var is_open = item.IsOpen;

                            //if (VSUtil.IsArquivoVisualStudio(arquivo))
                            //{
                            //    var aaa = "";
                            //}

                            arquivos.Add(arquivo);
                        }
                    }
                }
                if (item.ProjectItems != null && item.ProjectItems.Count > 0)
                {
                    arquivos.AddRange(ProjetoTypeScriptUtilEx.RetornarArquivosTypeScript(item.ProjectItems));
                }
            }
            return arquivos;
        }

        
    }
}
