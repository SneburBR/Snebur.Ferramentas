using Snebur.Dominio;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Snebur.Linq
{
    public static class LinqExtensao
    {
        public static readonly object __lock = new object();

        public static object SyncLock(this IEnumerable enumerable)
        {
            if (IsConcurrentCollection(enumerable))
            {
                return __lock;
            }
            return (enumerable as ICollection)?.SyncRoot ?? __lock;
        }

        public static T TryGet<T>(this IList<T> colecao, int index)
        {
            if (index < colecao.Count)
            {
                return colecao[index];
            }
            return default;
        }

        public static string TryGet(this string[] colecao, int index)
        {
            if (index < colecao.Length)
            {
                return colecao[index];
            }
            return null;
        }

        public static void AddIsNotNull<T>(this ICollection<T> colecao, T item)
        {
            if (item != null)
            {
                colecao.Add(item);
            }
        }

        public static T Pop<T>(this IList<T> colecao)
        {
            if (colecao?.Count > 0)
            {
                lock (colecao.SyncLock())
                {
                    var item = colecao[colecao.Count - 1];
                    colecao.RemoveAt(colecao.Count - 1);
                    return item;
                }
            }
            return default;
        }

        //shift
        public static T Shift<T>(this IList<T> colecao)
        {
            if (colecao?.Count > 0)
            {
                lock (colecao.SyncLock())
                {
                    var item = colecao[0];
                    colecao.RemoveAt(0);
                    return item;
                }
            }
            return default;

        }

        public static void AddIfTrue<T>(this ICollection<T> colecao, T item, bool isAdd)
        {
            if (isAdd)
            {
                colecao.Add(item);
            }
        }
        public static void AddIfNotExits<T>(this ICollection<T> colecao, T item)
        {
            if (!colecao.Contains(item))
            {
                colecao.Add(item);
            }
        }

        public static void AddRangeIfNotExits<T>(this ICollection<T> colecao, IEnumerable<T> itens)
        {
            if (itens == null)
            {
                return;
            }
            foreach (var item in itens)
            {
                colecao.AddIfNotExits(item);
            }
        }

        public static void AddRangeNew<T>(this ICollection<T> colecao, IEnumerable<T> itens)
        {
            if (itens == null)
            {
                return;
            }

            lock (colecao.SyncLock())
            {
                colecao.Clear();
                foreach (var item in itens)
                {
                    colecao.Add(item);
                }
            }
        }

        public static void AddRange<T>(this ICollection<T> colecao, IEnumerable<T> itens)
        {
            if (itens == null)
            {
                return;
            }

            foreach (var item in itens)
            {
                colecao.Add(item);
            }
        }

        public static void AddRangeNotNull<T>(this ICollection<T> colecao, IEnumerable<T> itens)
        {
            if (itens == null)
            {
                return;
            }

            foreach (var item in itens)
            {
                colecao.AddIsNotNull(item);
            }
        }

        public static void RemoveRange<T>(this ICollection<T> colecao, IEnumerable<T> itens)
        {
            if (itens == null)
            {
                return;
            }

            foreach (var item in itens.ToList())
            {
                colecao.Remove(item);
            }
        }

        public static IEnumerable<T> Duplicados<T>(this IEnumerable<T> item)
        {
            return item.GroupBy(x => x).
                        SelectMany(g => g.Skip(1)).
                        Distinct();
        }

        public static ListaEntidades<TEntidade> ToList<TEntidade>(this ListaEntidades<TEntidade> colecao) where TEntidade : Entidade
        {
            var lista = new ListaEntidades<TEntidade>();
            lista.AddRange(colecao);
            return lista;
        }

        public static ListaEntidades<TEntidade> ToListaEntidades<TEntidade>(this IEnumerable<TEntidade> colecao) where TEntidade : IEntidade
        {
            var listaEntidades = new ListaEntidades<TEntidade>();
            listaEntidades.AddRange(colecao);
            return listaEntidades;
        }
        public static Queue<T> ToQueue<T>(this IEnumerable<T> colecao)
        {
            var fila = new Queue<T>();
            foreach (var item in colecao)
            {
                fila.Enqueue(item);
            }
            return fila;
        }

        public static T Random<T>(this IList<T> colecao)
        {
            return colecao[new Random().Next(colecao.Count)];
        }

        public static TSource MinOrDefault<TSource>(this IEnumerable<TSource> colecao)
        {
            if (colecao.Count() > 0)
            {
                return colecao.Min();
            }
            return default;
        }

        public static TResult MinOrDefault<TSource, TResult>(this IEnumerable<TSource> colecao, Func<TSource, TResult> selector)
        {
            if (colecao.Count() > 0)
            {
                return colecao.Min(selector);
            }
            return default;
        }

        public static TSource MaxOrDefault<TSource>(this ICollection<TSource> colecao)
        {
            if (colecao.Count > 0)
            {
                return colecao.Max();
            }
            return default;
        }

        public static TResult MaxOrDefault<TSource, TResult>(this ICollection<TSource> colecao, Func<TSource, TResult> selector)
        {
            if (colecao.Count > 0)
            {
                return colecao.Max(selector);
            }
            return default(TResult);
        }

        public static TSource Random<TSource>(this ICollection<TSource> colecao)
        {
            var count = colecao.Count;
            if (count > 0)
            {
                if (count == 1)
                {
                    return colecao.First();
                }
                var posicaoRandow = new Random().Next(0, count);
                if (posicaoRandow < count)
                {
                    return colecao.ElementAt(posicaoRandow);
                }
                return colecao.First();
            }
            throw new Erro("A coleção não possui nenhum elemento");
        }

        public static TSource RandomOrDefault<TSource>(this ICollection<TSource> colecao)
        {
            var count = colecao.Count;
            if (count > 0)
            {
                return colecao.Random();
            }
            return default;
        }

        #region Sum

        public static float SumOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, float> selector)
        {
            if (source.Count() > 0)
            {
                return source.Sum(selector);
            }
            return default;
        }

        public static int SumOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, int> selector)
        {
            if (source.Count() > 0)
            {
                return source.Sum(selector);
            }
            return default;
        }

        public static long SumOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, long> selector)
        {
            if (source.Count() > 0)
            {
                return source.Sum(selector);
            }
            return default;
        }

        public static decimal? SumOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal?> selector)
        {
            if (source.Count() > 0)
            {
                return source.Sum(selector);
            }
            return default;
        }

        public static double SumOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, double> selector)
        {
            if (source.Count() > 0)
            {
                return source.Sum(selector);
            }
            return default;
        }

        public static int? SumOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, int?> selector)
        {
            if (source.Count() > 0)
            {
                return source.Sum(selector);
            }
            return default;
        }

        public static long? SumOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, long?> selector)
        {
            if (source.Count() > 0)
            {
                return source.Sum(selector);
            }
            return default;
        }

        public static float? SumOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, float?> selector)
        {
            if (source.Count() > 0)
            {
                return source.Sum(selector);
            }
            return default;
        }

        public static double? SumOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, double?> selector)
        {
            if (source.Count() > 0)
            {
                return source.Sum(selector);
            }
            return default;
        }

        public static decimal SumOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal> selector)
        {
            if (source.Count() > 0)
            {
                return source.Sum(selector);
            }
            return default;
        }

        public static double? SumOrDefault(this IEnumerable<double?> source)
        {
            if (source.Count() > 0)
            {
                return source.Sum();
            }
            return default;
        }

        public static float? SumOrDefault(this IEnumerable<float?> source)
        {
            if (source.Count() > 0)
            {
                return source.Sum();
            }
            return default;
        }

        public static decimal SumOrDefault(this IEnumerable<decimal> source)
        {
            if (source.Count() > 0)
            {
                return source.Sum();
            }
            return default;
        }

        public static double SumOrDefault(this IEnumerable<double> source)
        {
            if (source.Count() > 0)
            {
                return source.Sum();
            }
            return default;
        }

        public static int SumOrDefault(this IEnumerable<int> source)
        {
            if (source.Count() > 0)
            {
                return source.Sum();
            }
            return default;
        }
        //     source is null.
        public static float SumOrDefault(this IEnumerable<float> source)
        {
            if (source.Count() > 0)
            {
                return source.Sum();
            }
            return default;
        }

        public static decimal? SumOrDefault(this IEnumerable<decimal?> source)
        {
            if (source.Count() > 0)
            {
                return source.Sum();
            }
            return default;
        }

        public static int? SumOrDefault(this IEnumerable<int?> source)
        {
            if (source.Count() > 0)
            {
                return source.Sum();
            }
            return default;
        }

        public static long? SumOrDefault(this IEnumerable<long?> source)
        {
            if (source.Count() > 0)
            {
                return source.Sum();
            }
            return default;
        }

        public static long SumOrDefault(this IEnumerable<long> source)
        {
            if (source.Count() > 0)
            {
                return source.Sum();
            }
            return default;
        }
        #endregion

        #region String

        public static bool Contains(this List<string> source,
            string value, StringComparison stringComparison)
        {
            return source.IndexOf(value, stringComparison) >= 0;
        }

        public static bool Contains(this string[] source, string value, StringComparison stringComparison)
        {
            return source.IndexOf(value, stringComparison) >= 0;
        }

        public static int IndexOf(this List<string> source, string value, StringComparison stringComparison)
        {
            for (var i = 0; i < source.Count; i++)
            {
                var item = source[i];
                if (item != null)
                {
                    if (item.Equals(value, stringComparison))
                    {
                        return i;
                    }
                }
                else
                {
                    if (item == value)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        public static int IndexOf(this string[] source, string value, StringComparison stringComparison)
        {
            for (var i = 0; i < source.Length; i++)
            {
                var item = source[i];
                if (item != null)
                {
                    if (item.Equals(value, stringComparison))
                    {
                        return i;
                    }
                }
                else
                {
                    if (item == value)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }
        #endregion

        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> colecao, bool aceitarNull) where T : class
        {
            var hashSet = colecao.ToHashSet();
            if (aceitarNull)
            {
                return hashSet;
            }
            hashSet.Remove(null);
            return hashSet;
        }

        private static bool IsConcurrentCollection(IEnumerable enumerable)
        {
            var type = enumerable.GetType();
            if (type.IsGenericType)
            {
                var typeDefinition = type.GetGenericTypeDefinition();
                return typeDefinition == typeof(ConcurrentDictionary<,>) ||
                       typeDefinition == typeof(ConcurrentQueue<>) ||
                       typeDefinition == typeof(ConcurrentStack<>) ||
                       typeDefinition == typeof(ConcurrentBag<>);
            }
            return false;
        }

        public static T PeekOrDefault<T>(this Queue<T> queue)
        {
            if (queue.Count > 0)
            {
                return queue.Peek();
            }
            return default;
        }

#if NET45 || NET40
        public static IEnumerable<T> Prepend<T>(this IEnumerable<T> source, T item)
        {
            yield return item;
            foreach (var element in source)
            {
                yield return element;
            }
        }

        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> colecao)
        {
            return new HashSet<T>(colecao);
        }
#endif
    }
}