using Snebur.Dominio;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Snebur.Serializacao
{
    internal abstract class Referencia
    {

    }
    internal class ReferenciaRaiz : Referencia
    {

    }

    internal class ReferenciaColecao : Referencia
    {
        internal IList Colecao { get; set; }
        internal int Posicao { get; set; }
    }
    internal class ReferenciaDicionario : Referencia
    {
        public IDictionary Dicionario { get; internal set; }
        public object Chave { get; internal set; }

        public ReferenciaDicionario()
        {

        }
    }
    internal class ReferenciaPropriedade : Referencia
    {
        internal object ObjetoPai { get; set; }
        internal PropertyInfo Propriedade { get; set; }
    }

    internal class BaseDominioRefenciada
    {
        internal IBaseDominioReferencia BaseDominio { get; set; }
        internal Referencia Referencia { get; set; }
        internal Guid IdentificadorReferencia { get; set; }
    }

    internal class BaseDominioOrigem
    {
        public IBaseDominioReferencia BaseDominio { get; }
        public List<Referencia> Referencias { get; } = new List<Referencia>();
        public List<BaseDominioRefenciada> BaseDominioRefenciadas { get; } = new List<BaseDominioRefenciada>();
        public Referencia ReferenciaOrigem { get; internal set; }

        public BaseDominioOrigem(BaseDominio baseDominio)
        {
            this.BaseDominio = baseDominio;
        }

    }
}
