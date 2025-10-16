using Snebur.Dominio.Atributos;
using Snebur.Linq;
using Snebur.Utilidade;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Snebur.Dominio
{
    //public abstract class BaseListaTipoComplexo<T> : BaseTipoComplexo, /*ICollection<T>, */ IEnumerable<T>, IEnumerable /*,  IList<T>*/
    [IgnorarClasseTS]
    public abstract class BaseListaTipoComplexo<T> : BaseTipoComplexo /*, IEnumerable ,  IEnumerable<T>, IEnumerable */
    {
        private string _json = "[]";

        protected readonly HashSet<T> ListaInterna = new HashSet<T>();
        private bool IsSerializandoLista;

        [ValidacaoTextoTamanho(Int16.MaxValue)]
        public string Json
        {
            get
            {
                return this._json;
            }
            set
            {
                this.NotificarValorPropriedadeAlterada(this._json, this._json = value);
                this.DeserilizarLista();
            }
        }
        [XmlIgnore]
        [NaoSerializar]
        [NaoMapear]
        [IgnorarPropriedade]
        public int Count => this.ListaInterna.Count;

        //public bool IsReadOnly => throw new NotImplementedException();

        //[XmlIgnore]
        //[NaoSerializar]
        //[IgnorarPropriedadeTS]
        //bool ICollection<T>.IsReadOnly { get => (this.ListaInterna as ICollection<T>).IsReadOnly; }

        //[XmlIgnore]
        //[NaoSerializar]
        //[NaoMapear]
        //[IgnorarPropriedadeTSReflexaoAttribute]
        //public T this[int index] { get => this.ListaInterna[index]; set => this.ListaInterna[index] = value; }

        public BaseListaTipoComplexo()
        {
        }

        public BaseListaTipoComplexo(IEnumerable<T> lista)
        {
            foreach (var n in lista)
            {
                this.ListaInterna.Add(n);
            }
            this.SerializarLista();
        }

        public void Add(T valor)
        {
            this.ListaInterna.Add(valor);
            this.SerializarLista();
        }

        public void AddRange(List<T> valores)
        {
            this.ListaInterna.AddRange(valores);
            this.SerializarLista();
        }
        public void Clear()
        {
            this.ListaInterna.Clear();
            this.SerializarLista();
        }
        public bool Remove(T valor)
        {
            var resultado = this.ListaInterna.Remove(valor);
            this.SerializarLista();
            return resultado;
        }

        private void SerializarLista()
        {
            this.IsSerializandoLista = true;
            this.Json = JsonUtil.Serializar(this.ListaInterna, EnumTipoSerializacao.Javascript);
            this.IsSerializandoLista = false;
        }

        private void DeserilizarLista()
        {
            if (!this.IsSerializandoLista)
            {
                this.ListaInterna.Clear();
                if (!String.IsNullOrEmpty(this._json))
                {
                    var valoresTipado = JsonUtil.Deserializar<List<T>>(this._json, EnumTipoSerializacao.Javascript);
                    this.ListaInterna.AddRange(valoresTipado);
                }
            }
        }

        #region IList

        public bool Contains(T item)
        {
            return this.ListaInterna.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            this.ListaInterna.CopyTo(array, arrayIndex);
        }

        public List<T> ToList()
        {
            return this.ListaInterna.ToList();
        }
        #endregion

        //#region IEnumerable

        //public IEnumerator<T> GetEnumerator()
        //{
        //    return this.ListaInterna.GetEnumerator();
        //}

        //IEnumerator IEnumerable.GetEnumerator()
        //{
        //    return this.ListaInterna.GetEnumerator();
        //}

        //#endregion
    }
}