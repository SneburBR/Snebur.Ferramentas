using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Snebur.VisualStudio
{
    public abstract class ConfiguracaoProjeto
    {

        [XmlIgnore()]
        public List<string> ProjetoDepedencia => this.RetornarNomesProjetoDepedencia();

        protected abstract List<string> RetornarNomesProjetoDepedencia();

        public string CaminhoProjeto => throw new NotImplementedException();
        public string NomeAssembly => throw new NotImplementedException();

    }
}
