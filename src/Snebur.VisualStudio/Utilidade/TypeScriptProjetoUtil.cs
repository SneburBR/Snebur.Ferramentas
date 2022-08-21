using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snebur;
using Snebur.VisualStudio.Projeto;
using EnvDTE;
using System.IO;
using Snebur.VisualStudio.Projeto.TypeScript;

namespace Snebur.VisualStudio.Utilidade
{
    
    public class TypeScriptProjetoUtil
    {

        public const string NOME_PROJETO_PRINCIPAL = "Snebur.TypeScript";

        private const string EXTENSAO_TYPESCRIPT = ".ts";

        public static ProjetoTypeScript RetornarProjetoTypeScript(Project projetoVS, string caminhoProjeto, string caminhoConfiguracao)
        {
            var arquivosTypeScript = TypeScriptProjetoUtil.RetornarArquivosTypeScript(projetoVS.ProjectItems);
            LogMensagemUtil.Log("Total de arquivos typeScript : {0}", arquivosTypeScript.Count.ToString());

            if (projetoVS.Name == NOME_PROJETO_PRINCIPAL)
            {
                return new ProjetoTypeScript(projetoVS, caminhoProjeto, caminhoConfiguracao, arquivosTypeScript);
            }
            else
            {
                return new ProjetoTypeScript(projetoVS, caminhoProjeto, caminhoConfiguracao, arquivosTypeScript);
            }
        }

        private static List<FileInfo> RetornarArquivosTypeScript(ProjectItems items)
        {

            var arquivos = new List<FileInfo>();
            foreach (ProjectItem item in items)
            {
                if (item.FileCount > 0)
                {
                    for (short i = 0; i < item.FileCount; i++)
                    {
                    
                        var arquivo = item.FileNames[i];
                        if (Path.GetExtension(arquivo) == EXTENSAO_TYPESCRIPT)
                        {
                            var is_dirty = item.IsDirty;
                            var is_open = item.IsOpen;
                            
                            arquivos.Add(new FileInfo(arquivo));
                        }
                    }
                }
                if (item.ProjectItems != null && item.ProjectItems.Count > 0)
                {
                    arquivos.AddRange(TypeScriptProjetoUtil.RetornarArquivosTypeScript(item.ProjectItems));
                }
            }
            return arquivos;
        }

    }
}
