using System.Collections.Generic;

namespace Snebur.Dominio
{
    public class ListaDouble : BaseListaTipoComplexo<double>
    {
        public ListaDouble()
        {
        }

        public ListaDouble(IEnumerable<double> lista) : base(lista)
        {
        }

        protected internal override BaseTipoComplexo BaseClone()
        {
            return new ListaDouble(this.ListaInterna);
        }
    }
    //[IgnorarClasseTS]
    //public class ListaDouble :/* BaseTipoComplexo,*/ IEnumerable, IEnumerable<double>
    //{
    //    private string _valores;

    //    private List<double> Lista { get; } = new List<double>();

    //    public string Valores
    //    {
    //        get
    //        {
    //            return this._valores;
    //        }
    //        set
    //        {
    //            this._valores = value;
    //            this.Lista.Clear();
    //            var valoresTipado = this._valores.Split(';').Where(x => !String.IsNullOrWhiteSpace(x)).Select(x => Double.Parse(x));
    //            this.Lista.AddRange(valoresTipado);
    //        }
    //    }

    //    public void Add(double valor)
    //    {
    //        this.Lista.Add(valor);
    //    }

    //    public bool Remove(double valor)
    //    {
    //        var resultado = this.Lista.Remove(valor);
    //        this.AtualizarLista();
    //        return resultado;
    //    }

    //    #region IEnumerable

    //    public IEnumerator<double> GetEnumerator()
    //    {
    //        return this.Lista.GetEnumerator();
    //    }

    //    IEnumerator IEnumerable.GetEnumerator()
    //    {
    //        return this.Lista.GetEnumerator();
    //    }

    //    #endregion

    //    private void AtualizarLista()
    //    {
    //        this._valores = String.Join(";", this.Lista);
    //    }

    //}
}