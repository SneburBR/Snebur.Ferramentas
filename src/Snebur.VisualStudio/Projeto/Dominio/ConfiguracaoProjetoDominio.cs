using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snebur;
 
namespace Snebur.VisualStudio.Projeto
{
    public class ConfiguracaoProjetoDominio
    {
        public string CaminhoDominioTypeScript { get; set; }

        public bool Globalizar { get; set; }

        public string Namespace { get; set; }

        public int PrioridadeDominio { get; set; }

        public List<string> DominiosDepentendes { get; set; } = new List<string>();
    }
}
