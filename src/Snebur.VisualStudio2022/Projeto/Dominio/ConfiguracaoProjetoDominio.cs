using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snebur;

namespace Snebur.VisualStudio
{
    public class ConfiguracaoProjetoDominio : ConfiguracaoProjeto
    {
       
        public string CaminhoDominioTypeScript { get; set; }

        public bool Globalizar { get; set; }

        public string Namespace { get; set; }

        public int PrioridadeDominio { get; set; }
        
        public bool IsVersaoManual { get; set; }

        public DominioDependente[] DominiosDepentendes { get; set; }
 


        public string RetornarCaminhoAbsolutoDominioTypeScript(string caminhoProjeto)
        {
            var caminho = Path.Combine(caminhoProjeto, this.CaminhoDominioTypeScript);
            return Path.GetFullPath(caminho);
        }

        protected override List<string> RetornarNomesProjetoDepedencia()
        {
            return this.DominiosDepentendes.Select(x => x.Nome).ToList().ToList();
        }
    }

    public class DominioDependente
    {
        public string Nome { get; set; }
        public string Caminho { get; set; }
    }
}
