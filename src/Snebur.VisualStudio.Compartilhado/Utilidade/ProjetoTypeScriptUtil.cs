using Snebur.Utilidade;
using System;
using System.IO;
using System.Text;

namespace Snebur.VisualStudio
{
    public static partial class ProjetoTypeScriptUtil
    {
        public static ProjetoTypeScript RetornarProjetoTypeScript(ConfiguracaoProjetoTypeScript configuracaoTS,
                                                                  ProjetoViewModel projetoVM,
                                                                  FileInfo arquivoProjeto,
                                                                  string caminhoConfiguracao)
        {
            return new ProjetoTypeScript(projetoVM, 
                                         configuracaoTS, 
                                         arquivoProjeto, 
                                         caminhoConfiguracao);
        }

        public static ConfiguracaoProjetoTypeScript RetornarConfiguracaoProjetoTypeScript(string caminhoConfiguracao)
        {
            var json = File.ReadAllText(caminhoConfiguracao, Encoding.UTF8);
            return JsonUtil.Deserializar<ConfiguracaoProjetoTypeScriptFramework>(json);
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
