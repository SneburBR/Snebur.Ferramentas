using System;
using System.Collections.Generic;
using System.IO;
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
    }
}
