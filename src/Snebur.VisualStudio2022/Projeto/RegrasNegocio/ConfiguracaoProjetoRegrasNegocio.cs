using System.Collections.Generic;
using System.IO;

namespace Snebur.VisualStudio
{
    public class ConfiguracaoProjetoRegrasNegocio : ConfiguracaoProjeto
    {
        public string CaminhoExtensaoTypeScript { get; set; }

        public string CaminhoExtensaoCSharp { get; set; }

        public string RetornarCaminhoExtensaoTypeScriptCompleto(string caminhoReferencia)
        {
            var caminho = Path.Combine(caminhoReferencia, this.CaminhoExtensaoTypeScript);
            return Path.GetFullPath(caminho);
        }

        public string RetornarCaminhoExtensaoCSharpCompleto(string caminhoReferencia)
        {
            var caminho = Path.Combine(caminhoReferencia, this.CaminhoExtensaoCSharp);
            return Path.GetFullPath(caminho);
        }

        protected override List<string> RetornarNomesProjetoDepedencia()
        {
            return new List<string>();
        }
    }
}
