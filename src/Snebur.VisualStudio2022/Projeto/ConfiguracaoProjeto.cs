using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Snebur.VisualStudio
{
    public abstract class ConfiguracaoProjeto
    {

        [XmlIgnore()]
        public List<string> ProjetoDepedencia => this.RetornarNomesProjetoDepedencia();

        protected abstract List<string> RetornarNomesProjetoDepedencia();

    }
}
