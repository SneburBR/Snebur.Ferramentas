using System.IO;

namespace Snebur.VisualStudio
{
    public static class ProjetoUtil
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
    }
}

