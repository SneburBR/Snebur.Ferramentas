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
    public class ConfiguracaoProjetoContextoDados : ConfiguracaoProjeto
    {
        public string NamespaceEntidades { get; set; }
        public string CaminhoAssemblyEntidades { get; set; }

        public string CaminhoContextoDadosEntity { get; set; }

        public string CaminhoContextoDadosNETInterface { get; set; }

        public string CaminhoContextoDadosNETServidor { get; set; }

        public string CaminhoContextoDadosNETCliente { get; set; }

        public string CaminhoContextoDadosTS { get; set; }

        protected override List<string> RetornarNomesProjetoDepedencia()
        {
            return new List<string>();
        }
    }
}
