using System.Collections.Generic;

namespace System
{
    public static class IListExtensao
    {
        public static List<(T, int)> ToTupleItemIndex<T>(this IList<T> lista)
        {
            var retorno = new List<(T, int)>();
            var len = lista.Count;
            for (var i = 0; i < len; i++)
            {
                var item = lista[i];
                retorno.Add((item, i));
            }
            return retorno;
        }
        public static List<(T, int)> ToTupleItemCount<T>(this IList<T> lista)
        {
            var retorno = new List<(T, int)>();
            var len = lista.Count;
            for (var i = 0; i < len; i++)
            {
                var item = lista[i];
                retorno.Add((item, i + 1));
            }
            return retorno;
        }

        public static List<(T, double)> ToTupleItemProgresso<T>(this IList<T> lista, double portentagem = 100)
        {
            var retorno = new List<(T, double)>();
            double len = lista.Count;
            for (var i = 0; i < len; i++)
            {
                var progresso = ((i + 1) / len) * portentagem;
                var item = lista[i];
                retorno.Add((item, progresso));
            }
            return retorno;
        }
    }
}
