using System.Collections.Generic;

namespace Snebur.Dominio
{
    public class ListaString : BaseListaTipoComplexo<string>
    {
        public ListaString()
        {

        }

        public ListaString(IEnumerable<string> lista) : base(lista)
        {

        }
        protected internal override BaseTipoComplexo BaseClone()
        {
            return new ListaString(this.ListaInterna);
        }
    }
}
