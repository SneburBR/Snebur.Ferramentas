using System.Collections.Generic;
using System.Linq;

namespace Snebur.VisualStudio
{
    public class ConfiguracaoProjetoEstilo : ConfiguracaoProjeto
    {
        public string outputFile { get; set; }

        public string inputFile { get; set; }

        public bool IsIgnorar { get; set; } = false;

        public bool IsProjetoApresentacao { get; set; } = false;

        public Dictionary<string, string> Depedencias { get; set; } = new Dictionary<string, string>();

        public List<string> NomesPastaServidor { get; set; } = new List<string>();

        protected override List<string> RetornarNomesProjetoDepedencia()
        {
            return this.Depedencias.Select(x => x.Key).ToList();
        }
    }
}
