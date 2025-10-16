using Snebur.Dominio;
using System.Collections;
using System.Collections.Generic;

namespace System.Linq
{
    public static class ListaEntidades
    {
 
        public static TEntidade Incluir<TEntidade>(this ListaEntidades<TEntidade> source) where TEntidade : IEntidade
        {
            return default;
        }

        public static TEntidade Incluir<TEntidade>(this IListaEntidades<TEntidade> source) where TEntidade : IEntidade
        {
            return default;
        }

        public static TEntidade Incluir<TEntidade>(this IEnumerable<TEntidade> source) where TEntidade : IEntidade
        {
            return default;
        }

        public static TEntidadeEspecializar IncluirTipado<TEntidadeEspecializar>(this IListaEntidades source) where TEntidadeEspecializar : IEntidade
        {
            return default;
        }

        public static void Add<TEntidade>(this IEnumerable<TEntidade> colecao, IEntidade entidade) where TEntidade : IEntidade
        {
            var lista = colecao as IListaEntidades;
            if (lista != null)
            {
                lista.Add(entidade);
                return;
            }
            var xxx = colecao as IList;
            if (xxx != null)
            {
                xxx.Add(entidade);
                return;
            }
            throw new Exception(" Coleção está vazia ou não foi possível converter para IlistaEntidaedes");
        }

        public static void Remove<TEntidade>(this IEnumerable<TEntidade> colecao, IEntidade entidade) where TEntidade : IEntidade
        {
            var lista = colecao as IListaEntidades;
            if (lista == null)
            {
                throw new Exception(" Coleção está vazia ou não foi possível converter para IlistaEntidaedes");
            }
            lista.Remove(entidade);
        }
    }
}