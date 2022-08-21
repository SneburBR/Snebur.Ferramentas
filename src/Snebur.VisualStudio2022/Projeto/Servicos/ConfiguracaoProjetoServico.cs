using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snebur;
using Snebur.Utilidade;
using Snebur.Dominio;

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
