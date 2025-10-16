using System;

namespace Snebur.Utilidade
{
    public static class LazyUtil
    {
        private static readonly object _bloqueioLazy = new object();

        public static T RetornarValorLazyComBloqueio<T>(ref T? valor, Func<T> retornarValor) where T : struct
        {
            if (!valor.HasValue)
            {
                lock (_bloqueioLazy)
                {
                    if (!valor.HasValue)
                    {
                        valor = retornarValor.Invoke();
                    }
                }
            }
            return valor.Value;
        }

        public static T RetornarValorLazyComBloqueio<T>(ref T valor, Func<T> retornarValor) where T : class
        {
            if (valor == null)
            {
                lock (_bloqueioLazy)
                {
                    if (valor == null)
                    {
                        valor = retornarValor.Invoke();
                    }
                }
            }
            return valor;
        }

        public static T RetornarValorLazy<T>(ref T? valor, Func<T> retornarValor) where T : struct
        {
            if (!valor.HasValue)
            {
                lock (_bloqueioLazy)
                {
                    if (!valor.HasValue)
                    {
                        valor = retornarValor.Invoke();
                    }
                }
            }
            return valor.Value;
        }

        public static T RetornarValorLazy<T>(ref T valor, Func<T> retornarValor) where T : class
        {
            if (valor == null)
            {
                var novoValor = retornarValor.Invoke();
                if (valor == null)
                {
                    valor = novoValor;
                }
            }
            return valor;
        }
    }
}