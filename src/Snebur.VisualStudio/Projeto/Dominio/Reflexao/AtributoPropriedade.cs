using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snebur;
using Snebur.Utilidade;
using System.Reflection;

namespace Snebur.VisualStudio.Projeto.Dominio
{
    public class AtributoPropriedade
    {
        public PropertyInfo Propriedade { get; set; }

        public Attribute Atributo { get; set; }

        public string DeclaracaoProprieade { get; set; }

        public AtributoPropriedade(PropertyInfo propriedade, Attribute atributo)
        {
            this.Propriedade = propriedade;
            this.Atributo = atributo;
            this.DeclaracaoProprieade = AjudanteReflexao.RetornarDeclaracaoPropriedade(propriedade);
        }

        public string RetornarDeclaracaoAdicionarAtributoProprieade()
        {
            throw new NotImplementedException();
        }

    }
}
