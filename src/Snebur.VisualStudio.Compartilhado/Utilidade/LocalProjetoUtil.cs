using Snebur.Utilidade;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Snebur.VisualStudio
{
    public static class LocalProjetoUtil
    {
        internal static bool IsProjetoTypescript(string caminhoProjeto)
        {
            if (!String.IsNullOrEmpty(caminhoProjeto))
            {
                var caminhoTSConfig = Path.Combine(caminhoProjeto, "tsconfig.json");
                return File.Exists(caminhoTSConfig);
            }
            return false;
        }

        internal static async Task<IEnumerable<string>> RetornarTodosArquivosAsync(object projetoVS,
                                                                                    string caminhoProjeto,
                                                                                    bool isLowerCase)
        {
            return await BaseAplicacaoVisualStudio.Instancia.RetornarTodosArquivosProjetoAsync(projetoVS,
                                                                                                caminhoProjeto,
                                                                                                isLowerCase);
        }

        private static bool IsSalvar(string caminhoArquivo, string conteudo)
        {
            if (!File.Exists(caminhoArquivo))
            {
                return true;
            }
            var conteudoAtual = ArquivoUtil.LerTexto(caminhoArquivo, true);
            return !TextoUtil.IsIgual(conteudoAtual, conteudo, true, true);
        }

        public static void SalvarDominio(string caminho, string conteudo)
        {
            if (IsSalvar(caminho, conteudo))
            {
                LogVSUtil.Alerta("Atualizando domínio: " + Path.GetFileName(caminho));
                File.WriteAllText(caminho, conteudo, Encoding.UTF8);
            }
        }

        public static string RetornarChave(string caminhoProjeto)
        {
            if (Path.GetExtension(caminhoProjeto).Equals(".csproj", StringComparison.InvariantCultureIgnoreCase))
            {
                caminhoProjeto = Path.GetDirectoryName(caminhoProjeto);
            }
            return ArquivoUtil.NormalizarCaminhoArquivo(caminhoProjeto).ToLower();
        }
    }
}
