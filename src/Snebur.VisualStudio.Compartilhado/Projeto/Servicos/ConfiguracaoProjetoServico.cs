using System.Collections.Generic;

namespace Snebur.VisualStudio
{
    public class ConfiguracaoProjetoServico :  ConfiguracaoProjeto
    {
        public List<ServicoCaminhoTypeScript> Servicos { get; set; }

        protected override List<string> RetornarNomesProjetoDepedencia()
        {
            return new List<string>();
        }
    }

    public class ServicoCaminhoTypeScript
    {
        public string NomeInterface { get; set; }

        public string CaminhoTypeScript { get; set; }

        public string CaminhoDotNet { get; set; }

    }
}
