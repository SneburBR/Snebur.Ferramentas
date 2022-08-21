using System.IO;
using System.Text;
using Snebur.Utilidade;

namespace Snebur.VisualStudio
{
    public class ProjetoRegrasNegocioUtil
    {
        internal static ConfiguracaoProjetoRegrasNegocio RetornarConfiguracao(string caminhoConfiguracao)
        {
            var json = File.ReadAllText(caminhoConfiguracao, UTF8Encoding.UTF8);
            return JsonUtil.Deserializar<ConfiguracaoProjetoRegrasNegocio>(json);
        }

    }
}
