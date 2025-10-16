using System;
using System.Collections;
using System.Collections.Generic;

namespace Snebur.Dominio
{
    //[JsonObject]
    //https://stackoverflow.com/questions/14383736/how-to-serialize-deserialize-a-custom-collection-with-additional-properties-usin
    public class ListaEntidades<TEntidade> : List<TEntidade>,
                                             IListaEntidades,
                                             IListaEntidades<TEntidade> where TEntidade : IEntidade
    {

        //public Guid IdentificadorEntidadesRemovida { get; set; }
        public bool IsAberta { get; set; } = false;

        //public Dictionary<string, IEntidade> EntidadesRemovida { get; set; } = new Dictionary<string, IEntidade>();
        public List<Entidade> EntidadesRemovida { get; set; } = new List<Entidade>();

        public void AdicionarEntidades(IEnumerable entidades)
        {
            foreach (var entidade in entidades)
            {
                this.Add((TEntidade)entidade);
            }
        }

        public new void Add(TEntidade item)
        {
            if (this.EntidadesRemovida.Contains(item as Entidade))
            {
                this.EntidadesRemovida.Remove(item as Entidade);
            }
            base.Add(item);
        }

        public new void AddRange(IEnumerable<TEntidade> entidades)
        {
            if (entidades != null)
            {
                foreach (var entidade in entidades)
                {
                    this.Add(entidade);
                }
            }
        }

        public new bool Remove(TEntidade entidade)
        {
            if (!this.EntidadesRemovida.Contains(entidade as Entidade))
            {
                this.EntidadesRemovida.Add(entidade as Entidade);
            }
            return (base.Remove(entidade));
        }

        public new int RemoveAll(Predicate<TEntidade> match)
        {
            throw new ErroNaoImplementado();
        }

        public new void RemoveAt(int index)
        {
            throw new ErroNaoImplementado();
        }

        public new void RemoveRange(int index, int count)
        {
            throw new ErroNaoImplementado();
        }

        public void Add(IEntidade value)
        {
            base.Add((TEntidade)value);
        }

        #region  IListaEntidades 

        int IListaEntidades.Count
        {
            get
            {
                return this.Count;
            }
        }
        bool IListaEntidades.Remove(IEntidade entidade)
        {
            return this.Remove((TEntidade)entidade);
        }
        #endregion

        ///// <summary>
        ///// Método usando para expressoes, ex AbriRelacao(Cliente.Pedidos.Incluir().Produtos.Incluir().ConfiguracaoProduto)
        ///// </summary>
        ///// <returns></returns>
        //public TEntidade Incluir<T>() where T : IEntidade
        //{
        //    var xxx = new List<string>();
        //    xxx.First

        //    return default(TEntidade);
        //}

    }
}