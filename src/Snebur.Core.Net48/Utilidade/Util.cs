using Snebur.Dominio;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Snebur.Utilidade
{
    public static class Util
    {
        public static bool IsDate(object valor)
        {
            if (valor is DateTime)
            {
                return true;
            }
            if (valor is string)
            {
                DateTime teste;
                if (DateTime.TryParse((string)valor, out teste))
                {
                    return teste != DateTime.MinValue && teste != DateTime.MinValue;
                }
            }
            return false;
        }

        public static bool IsNumeric(object valor)
        {
            if (valor is short || valor is int || valor is long || valor is decimal || valor is float || valor is double || valor is bool)
            {
                return true;
            }
            if (valor is string)
            {
                if (Double.TryParse((string)valor, out double teste))
                {
                    return !Double.IsNaN(teste);
                }
            }
            return false;
        }

        public static string RetornarRelacoesAbertas<T>(Expression<Func<T, object>> expressao)
        {
            return ReflexaoUtil.RetornarCaminhoPropriedade(expressao);
        }

        public static string RetornarRelacoesAbertas<T>(params Expression<Func<T, object>>[] expressoes)
        {
            return String.Join(", ", expressoes.Select(x => ReflexaoUtil.RetornarCaminhoPropriedade(x)));
        }

        public static bool SaoIgual<T>(T valor1, T valor2)
        {
            return EqualityComparer<T>.Default.Equals(valor1, valor2);
        }

        public static bool SaoIgual(object objeto1, object objeto2)
        {
            if (objeto1 != null && objeto2 != null)
            {
                return objeto1.Equals(objeto2);
            }

            if (objeto1 == null && objeto2 is string str2)
            {
                return String.IsNullOrEmpty(str2);
            }

            if(objeto2 == null && objeto1 is string str1)
            {
                return String.IsNullOrEmpty(str1);
            }

            if (objeto1 == null ^ objeto2 == null)
            {
                return false;
            }
            return Equals(objeto1, objeto2);
        }

        public static T RetornarSeVerdadeiro<T>(bool condicao, T valor)
        {
            if (condicao)
            {
                return valor;
            }
            return default;
        }

        public static T[] RetornarTodosObjetoTipo<T>(object objeto)
        {
            var objetos = new HashSet<T>();
            var objetosAnalisados = new HashSet<object>();
            VarrerObjeto(objeto, objetos, objetosAnalisados);
            return objetos.ToArray();
        }

        private static void VarrerObjeto<T>(object objeto, HashSet<T> objetos, HashSet<object> objetosAnalisados)
        {
            if (objeto == null)
            {
                return;
            }
            if (objetosAnalisados.Contains(objeto))
            {
                return;
            }
            objetosAnalisados.Add(objeto);

            if (objeto is T tipado)
            {
                objetos.Add(tipado);
            }
            if (objeto is ICollection lista)
            {
                foreach (var item in lista)
                {
                    VarrerObjeto(item, objetos, objetosAnalisados);
                }
            }
            else
            {
                if (objeto is BaseTipoComplexo)
                {
                    return;
                }
                var proprieades = objeto.GetType().GetProperties().Where(x => x.GetGetMethod()?.IsPublic ?? false);
                foreach (var p in proprieades)
                {
                    if (p.CanRead && p.CanWrite)
                    {
                        var valor = p.GetValue(objeto);
                        VarrerObjeto(valor, objetos, objetosAnalisados);
                    }
                }
            }
        }

        public static string GetDefaultIfNullOrEmpty(string text, string defaultValue)
        {
            if (String.IsNullOrWhiteSpace(text))
            {
                return defaultValue;
            }
            return text;
            
        }
    }
}