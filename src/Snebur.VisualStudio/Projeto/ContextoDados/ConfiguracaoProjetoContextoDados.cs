using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snebur;
using Snebur.Utilidade;
using Snebur.Dominio;

namespace Snebur.VisualStudio.Projeto
{
    public class ConfiguracaoProjetoContextoDados
    {

        public string NamespaceEntidades { get; set; }

        public string CaminhoContextoDadosEntity { get; set; }

        public string CaminhoContextoDadosNET { get; set; }

        public string CaminhoContextoDadosTS { get; set; }


    }
}
