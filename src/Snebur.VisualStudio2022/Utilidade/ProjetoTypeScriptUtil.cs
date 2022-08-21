using EnvDTE;
using EnvDTE80;
using Snebur.Utilidade;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Snebur.VisualStudio
{

    public static partial class ProjetoTypeScriptUtil
    {

        private const string EXTENSAO_TYPESCRIPT = ".ts";

        public static ProjetoTypeScript RetornarProjetoTypeScript(ConfiguracaoProjetoTypeScript configuracaoTS, 
                                                                  DTE2 dte, 
                                                                  Project projetoVS,
            string caminhoProjeto,
            string caminhoConfiguracao)
        {
    

            return new ProjetoTypeScript(configuracaoTS, dte, projetoVS, caminhoProjeto, caminhoConfiguracao );
        }

        public static ConfiguracaoProjetoTypeScript RetornarConfiguracaoProjetoTypeScript(string caminhoConfiguracao)
        {
            var json = File.ReadAllText(caminhoConfiguracao, Encoding.UTF8);
            return JsonUtil.Deserializar<ConfiguracaoProjetoTypeScriptFramework>(json);
        }

        public static HashSet<string> RetornarArquivosTypeScript(ProjectItems items)
        {

            var arquivos = new HashSet<string>();
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

                            if (VSUtil.IsArquivoVisualStudio(arquivo))
                            {
                                var aaa = "";
                            }

                            arquivos.Add(arquivo);
                        }
                    }
                }
                if (item.ProjectItems != null && item.ProjectItems.Count > 0)
                {
                    arquivos.AddRange(ProjetoTypeScriptUtil.RetornarArquivosTypeScript(item.ProjectItems));
                }
            }
            return arquivos;
        }

        internal static DateTime? RetornarDataHoraScriptNormalizado(string caminhoScript)
        {
            try
            {
                if (File.Exists(caminhoScript))
                {
                    using (var fs = StreamUtil.OpenRead(caminhoScript))
                    using (var sr = new StreamReader(fs))
                    {
                        var primeiraLinha = sr.ReadToEnd();
                        var indiceAbre = primeiraLinha.IndexOf(NormalizarCompilacaoJavascript.ABRE_DATA_NORMALIZACAO);
                        if (indiceAbre > 0)
                        {
                            var temp = primeiraLinha.Substring(indiceAbre);
                            var indiceTicks = temp.IndexOf(NormalizarCompilacaoJavascript.TICKS);
                            if (indiceTicks > 0)
                            {
                                var tempTicks = temp.Substring(indiceTicks + NormalizarCompilacaoJavascript.TICKS.Length);
                                var fimTicks = tempTicks.IndexOf(" ");
                                if (fimTicks > 0)
                                {
                                    var ticksString = tempTicks.Substring(0, fimTicks);
                                    if (Int64.TryParse(ticksString, out var ticks))
                                    {
                                        return new DateTime(ticks);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogVSUtil.LogErro(ex);

            }
            return null;
        }
    }
}
