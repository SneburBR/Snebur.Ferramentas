using System.Collections.Generic;

namespace System.Collections.ObjectModel
{
    public static class ObservableCollectionEx
    {
        public static ObservableCollection<TSource> ToListaObservacao<TSource>(this IEnumerable<TSource> source)
        {
            var retorno = new ObservableCollection<TSource>();
            foreach (var item in source)
            {
                retorno.Add(item);
            }
            return retorno;
        }
    }
}