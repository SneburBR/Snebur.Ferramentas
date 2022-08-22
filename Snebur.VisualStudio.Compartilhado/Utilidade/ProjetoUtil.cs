using System;
using System.Collections.Generic;
using System.IO;

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

        internal static IEnumerable<string> RetornarTodosArquivos(object projetoVS,
                                                                  string caminhoProjeto, 
                                                                  bool isLowerCase)
        {
            return BaseAplicacaoVisualStudio.Instancia.RetornarTodosArquivosProjeto(projetoVS,
                                                                                    caminhoProjeto,
                                                                                    isLowerCase);
        }
    }
}
