using Snebur.Dominio.Atributos;
using System.Collections;
using System.Collections.Generic;

namespace Snebur.Dominio
{
    [IgnorarInterfaceTS]
    public interface IListaEntidades : IList
    {
        bool IsAberta { get; set; }

        void AdicionarEntidades(IEnumerable entidades);

        List<Entidade> EntidadesRemovida { get; set; }

        new int Count { get; }

        void Add(IEntidade value);

        bool Remove(IEntidade entidade);

        //Dictionary<string, IEntidade> EntidadesAdicionada { get; set; }
    }
    [IgnorarInterfaceTS]
    public interface IListaEntidades<TEntidade> : IListaEntidades, IList<TEntidade> where TEntidade : IEntidade
    {
    }
}