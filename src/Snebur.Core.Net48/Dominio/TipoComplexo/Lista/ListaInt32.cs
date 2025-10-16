using System.Collections.Generic;

namespace Snebur.Dominio
{
    public class ListaInt32 : BaseListaTipoComplexo<int>
    {
        public ListaInt32()
        {
        }

        public ListaInt32(IEnumerable<int> lista) : base(lista)
        {
        }

        protected internal override BaseTipoComplexo BaseClone()
        {
            return new ListaInt32(this.ListaInterna);
        }
    }
    //[IgnorarClasseTS]
    //public class ListaInt32 : BaseTipoComplexo, ICollection<int>, IEnumerable<int>, IEnumerable, IList<int>
    //{
    //    private string _ConteudoSerializado;

    //    private List<int> ListaInterna { get; } = new List<int>();

    //    public string ConteudoSerilizado
    //    {
    //        get
    //        {
    //            return this._ConteudoSerializado;
    //        }
    //        set
    //        {
    //            this._ConteudoSerializado = value;
    //            this.DeserilizarLista();
    //        }
    //    }

    //    [XmlIgnore]
    //    [NaoMapear]
    //    public int Count => this.ListaInterna.Count;

    //    [XmlIgnore]
    //    bool ICollection<int>.IsReadOnly { get => (this.ListaInterna as ICollection<int>).IsReadOnly; }

    //    [XmlIgnore]
    //    [NaoMapear]
    //    public int this[int index] { get => this.ListaInterna[index]; set => this.ListaInterna[index] = value; }

    //    public ListaInt32()
    //    {

    //    }

    //    public ListaInt32(List<int> lista)
    //    {
    //        foreach(var n in lista)
    //        {
    //            this.ListaInterna.Add(n);
    //        }
    //        this.SerializarLista();
    //    }

    //    public void Add(int valor)
    //    {
    //        this.ListaInterna.Add(valor);
    //        this.SerializarLista();
    //    }

    //    public bool Remove(int valor)
    //    {
    //        var resultado = this.ListaInterna.Remove(valor);
    //        this.SerializarLista();
    //        return resultado;
    //    }

    //    private void SerializarLista()
    //    {
    //        this._ConteudoSerializado = String.Join(";", this.ListaInterna);
    //    }

    //    private void DeserilizarLista()
    //    {
    //        this.ListaInterna.Clear();
    //        if (!String.IsNullOrEmpty(this._ConteudoSerializado))
    //        {
    //            var valoresTipado = JsonUtil.Se this._ConteudoSerializado.Split(';').Where(x => !String.IsNullOrWhiteSpace(x)).Select(x => Int32.Parse(x));
    //            this.ListaInterna.AddRange(valoresTipado);
    //        }
    //    }

    //    #region IList

    //    public int IndexOf(int item)
    //    {
    //        return this.ListaInterna.IndexOf(item);
    //    }

    //    public void Insert(int index, int item)
    //    {
    //        this.ListaInterna.IndexOf(index, item);
    //    }

    //    public void RemoveAt(int index)
    //    {
    //        this.ListaInterna.RemoveAt(index);
    //    }

    //    public void Clear()
    //    {
    //        this.ListaInterna.Clear();
    //    }

    //    public bool Contains(int item)
    //    {
    //        return this.ListaInterna.Contains(item);
    //    }

    //    public void CopyTo(int[] array, int arrayIndex)
    //    {
    //        this.ListaInterna.CopyTo(array, arrayIndex);
    //    }

    //    #endregion

    //    #region IEnumerable

    //    public IEnumerator<int> GetEnumerator()
    //    {
    //        return this.ListaInterna.GetEnumerator();
    //    }

    //    IEnumerator IEnumerable.GetEnumerator()
    //    {
    //        return this.ListaInterna.GetEnumerator();
    //    }

    //    #endregion

    //    protected internal override BaseTipoComplexo BaseClone()
    //    {
    //        return new ListaInt32(this.ListaInterna);
    //    }
    //}
}