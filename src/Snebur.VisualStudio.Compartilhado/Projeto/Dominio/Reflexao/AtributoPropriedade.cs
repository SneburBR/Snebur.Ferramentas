using System;
using System.Reflection;

namespace Snebur.VisualStudio
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
