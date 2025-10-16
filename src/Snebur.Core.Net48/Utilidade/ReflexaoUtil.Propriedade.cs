using Snebur.Dominio;
using Snebur.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Snebur.Utilidade
{
    public static partial class ReflexaoUtil
    {
        public static List<string> RetornarCaminhosPropriedade<T>(params Expression<Func<T, object>>[] expressoesCaminhoPropriedade)
        {
            var caminhos = new List<string>();
            foreach (var expressao in expressoesCaminhoPropriedade)
            {
                caminhos.Add(RetornarCaminhoPropriedade(expressao));
            }
            return caminhos;
        }

        public static List<string> RetornarNomesPropriedade<T>(params Expression<Func<T, object>>[] expressoesPropriedade)
        {
            var propriedades = RetornarPropriedades(expressoesPropriedade);
            return propriedades.Select(x => x.Name).ToList();
        }

        public static string RetornarNomePropriedade<T>(Expression<Func<T, object>> expressaoPropriedade)
        {
            return RetornarPropriedade(expressaoPropriedade).Name;
        }

        public static string RetornarCaminhoPropriedade<T>(Expression<Func<T, object>> expressaoCaminhoPropriedade)
        {
            var propriedades = RetornarTodasPropriedades(expressaoCaminhoPropriedade);
            return String.Join(".", propriedades.Select(x => x.Name));
        }

        public static PropertyInfo RetornarPropriedade<T>(Expression<Func<T, object>> expressaoCaminhoPropriedade)
        {
            var expressao = (Expression)expressaoCaminhoPropriedade;
            return ExpressaoUtil.RetornarPropriedades(expressao).Last();
        }

        public static PropertyInfo RetornarPropriedade<T>(Expression<Func<T, object>> expressaoCaminhoPropriedade, Type tipo)
        {
            var nomePropriedade = RetornarNomePropriedade(expressaoCaminhoPropriedade);
            return tipo.GetProperty(nomePropriedade);
        }

        public static List<PropertyInfo> RetornarPropriedades<T>()
        {
            return RetornarPropriedades(typeof(T));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tipo"></param>
        /// <param name="ignorarPropriedadesTipoBase"></param>
        /// <param name="publica">Se métodos get e set são publicos</param>
        /// <returns></returns>
        public static List<PropertyInfo> RetornarPropriedades(Type tipo,
                                                             bool ignorarPropriedadesTipoBase = false, 
                                                             bool publica = true)
        {
            var propriedades = tipo.GetProperties(BindingFlags).AsEnumerable();
            if (ignorarPropriedadesTipoBase && tipo.BaseType != null)
            {
                propriedades = propriedades.Where(x => x.DeclaringType == tipo);
            }
            if (publica)
            {
                propriedades = propriedades.Where(x => (x.GetGetMethod()?.IsPublic) ?? false &&
                                                       (x.GetSetMethod()?.IsPublic ?? false));
            }
            return propriedades.ToList();
        }

        public static List<PropertyInfo> RetornarPropriedadePossuiAtributo<TAttribute>(Type tipoEntidade) where TAttribute : Attribute
        {
            var propriedades = RetornarPropriedades(tipoEntidade);
            return propriedades.Where(x => x.GetCustomAttribute<TAttribute>() != null).ToList();
        }

        public static List<PropertyInfo> RetornarTodasPropriedades<T>(Expression<Func<T, object>> expressaoCaminhoPropriedade)
        {
            var expressao = (Expression)expressaoCaminhoPropriedade;
            return ExpressaoUtil.RetornarPropriedades(expressao);
        }

        public static List<PropertyInfo> RetornarPropriedades<T>(params Expression<Func<T, object>>[] expressoesCaminhoPropriedade)
        {
            var propriedades = new List<PropertyInfo>();
            foreach (var expressao in expressoesCaminhoPropriedade)
            {
                propriedades.Add(RetornarPropriedade(expressao));
            }
            return propriedades;
        }

        public static List<PropertyInfo> RetornarPropriedades(Type tipo, BindingFlags bindingFlags)
        {
            return tipo.GetProperties(bindingFlags).ToList();
        }

        public static List<PropertyInfo> RetornarPropriedades(Type tipo, BindingFlags bindingFlags, bool ignorarPropriedadesTipoBase = false)
        {
            if (ignorarPropriedadesTipoBase)
            {
                bindingFlags = bindingFlags | BindingFlags.DeclaredOnly;
            }

            var propriedades = RetornarPropriedades(tipo, bindingFlags);
            if (ignorarPropriedadesTipoBase && tipo.BaseType != null)
            {
                return propriedades.Where(x => ReferenceEquals(x.DeclaringType, tipo)).ToList();
            }
            else
            {
                return propriedades;
            }
        }

        public static bool IsPropriedadePublica(PropertyInfo pi)
        {
            return pi.CanWrite && pi.GetSetMethod(true).IsPublic;
        }
        /// <summary>
        /// Retornar lista da propriedade até chegar no caminho
        /// </summary>
        /// <param name="tipo"></param>
        /// <param name="caminhoPropriedade"></param>
        /// /// <param name="procurarTiposEspecializado"> Se true, procurar o nos tipos especializados, Tipo (Pessoa), Caminho (PessoaFisica) Cpf</param>
        /// <returns></returns>
        /// 
        public static List<PropertyInfo> RetornarPropriedadesCaminho(Type tipo, string caminhoPropriedade)
        {
            return RetornarPropriedadesCaminho(tipo, caminhoPropriedade, ResolverPropriedadeNaoEncontrada);
        }
        public static List<PropertyInfo> RetornarPropriedadesCaminho(Type tipo,
                                                                     string caminhoPropriedade,
                                                                     Func<Type, string, PropertyInfo> resolverPropriedadeNaoEncontrada)
        {
            var nomesPropriedade = caminhoPropriedade.Split(".".ToCharArray()).Select(x => x.Trim());
            var propriedades = new List<PropertyInfo>();
            var tipoAtual = tipo;

            foreach (var nomePropriedade in nomesPropriedade)
            {
                if (!String.IsNullOrEmpty(nomePropriedade))
                {
                    var propriedade = RetornarPropriedadeInterno(tipoAtual, nomePropriedade, resolverPropriedadeNaoEncontrada);

                    ErroUtil.ValidarReferenciaNula(propriedade, nameof(propriedade));
                    tipoAtual = propriedade.PropertyType;
                    propriedades.Add(propriedade);

                    if (propriedade.PropertyType.IsGenericType && IsTipoRetornaColecaoEntidade(propriedade.PropertyType))
                    {
                        tipoAtual = tipoAtual.GetGenericArguments().First();
                    }
                }
            }
            return propriedades;
        }

        private static PropertyInfo RetornarPropriedadeInterno(Type tipoAtual,
                                                             string nomePropriedade,
                                                             Func<Type, string, PropertyInfo> resolverPropriedadeNaoEncontrada)
        {
            var propriedade = tipoAtual.GetProperties(BindingFlags).Where(x => x.Name == nomePropriedade).SingleOrDefault();
            if (propriedade == null)
            {
                return resolverPropriedadeNaoEncontrada?.Invoke(tipoAtual, nomePropriedade);
            }
            return propriedade;
        }

        private static PropertyInfo ResolverPropriedadeNaoEncontrada(Type tipoAtual, string nomePropriedade)
        {
            if (tipoAtual.IsAbstract)
            {
                var subsTipo = tipoAtual.Assembly.GetAccessibleTypes().Where(x => x.IsSubclassOf(tipoAtual));
                var proprieades = subsTipo.Select(x => x.GetProperty(nomePropriedade)).Where(x => x != null).ToHashSet();

                if (proprieades.Count == 1)
                {
                    return proprieades.Single();
                }
                if (proprieades.Count == 0)
                {
                    return null;
                }
                var tiposEncontradao = proprieades.Select(x => x.DeclaringType);
                throw new Erro($"A propriedade {nomePropriedade} foi encotrada em mais de um tipo {String.Join(",", tiposEncontradao.Select(x => x.Name))}");
            }
            return null;

        }

        public static PropertyInfo RetornarPropriedade(Type tipo, string nomePropriedade)
        {
            return RetornarPropriedade(tipo, nomePropriedade, false);
        }

        public static PropertyInfo RetornarPropriedade(Type tipo, 
                                                       string nomePropriedade, 
                                                       bool ignorarPropriedadeNaoEncontrada)
        {
            Type tipoAtual = tipo;

            PropertyInfo pi = default(PropertyInfo);

            while (!ReferenceEquals(tipoAtual, typeof(object)))
            {
                pi = tipoAtual.GetProperty(nomePropriedade, BindingFlags);
                pi = tipoAtual.GetProperty(nomePropriedade);
                if (pi != null)
                {
                    return pi;
                }
                else
                {
                    tipoAtual = tipoAtual.BaseType;
                }
            }
            if (ignorarPropriedadeNaoEncontrada)
            {
                return null;
            }
            else
            {
                throw new Erro(String.Format("A propriedade '{0}' não foi encontrada em '{1}'.", nomePropriedade, tipo.Name));
            }
        }
        //Métodos para centralizar o retorno dos valores das propriedades
        public static object RetornarValorPropriedade(object objeto, string nomePropriedade)
        {
            var pi = RetornarPropriedade(objeto.GetType(),
                                                      nomePropriedade, false);

            var valorPropriedade = pi.GetValue(objeto, null);
            return valorPropriedade;
        }

        public static object RetornarValorPropriedade<T>(Expression<Func<T, object>> expressaoPropriedade, object objeto)
        {
            var propriedade = RetornarPropriedade(expressaoPropriedade, objeto.GetType());
            return propriedade.GetValue(objeto);
        }

        public static object RetornarValorPropriedade(object objeto, PropertyInfo pi)
        {
            var valorPropriedade = pi.GetValue(objeto, null);
            return valorPropriedade;
        }

        public static void AtribuirValorPropriedade(object objeto, PropertyInfo pi, object valor)
        {
            pi.SetValue(objeto, valor, BindingFlags, null, null, null);
        }

        public static bool IsPropriedadeRetornaColecao(PropertyInfo pi)
        {
            return IsTipoRetornaColecao(pi.PropertyType);
        }

        public static bool IsPropriedadePossuiAtributo(PropertyInfo propriedade, Type tipoAtributo, bool herdado = true)
        {
            return propriedade.GetCustomAttributes(tipoAtributo, herdado).FirstOrDefault() != null;
        }

        public static bool IsPropriedadeRetornaTipoPrimario(PropertyInfo propriedade, bool removerNullable = false)
        {
            return TipoRetornaTipoPrimario(propriedade.PropertyType, removerNullable);
        }

        public static bool IsPropriedadeRetornaTipoComplexo(PropertyInfo propriedade, bool removerNullable = false)
        {
            return propriedade.PropertyType.IsSubclassOf(typeof(BaseTipoComplexo));
        }
        /// <summary>
        /// Retornar a propriedade,
        /// </summary>
        /// <param name="tipo"></param>
        /// <param name="nomePropriedade">Primeiro procura um propriedade com mesmo nome,</param>
        /// <param name="atributo">Caso não encontra por nome, procura a propriedade que possui o atributo</param>
        /// <returns>Retornar PropertyInfo </returns>
        public static PropertyInfo RetornarPropriedade(Type tipo, string nomePropriedade, Type tipoAtributo)
        {
            var propriedades = RetornarPropriedades(tipo);
            var propriedadesEncontrada = propriedades.Where(x => x.Name == nomePropriedade).ToList();
            if (propriedadesEncontrada.Count == 0)
            {
                propriedadesEncontrada = propriedades.Where(x => IsPropriedadePossuiAtributo(x, tipoAtributo)).ToList();
            }
            if (propriedadesEncontrada.Count == 0)
            {
                throw new Erro(String.Format("Não foi encontrado a propriedade {0} em {1}", nomePropriedade, tipo));
            }
            if (propriedadesEncontrada.Count > 1)
            {
                throw new Erro(String.Format("Mais de uma propriedade  {0} foi encontrada em {1}", nomePropriedade, tipo));
            }
            return propriedadesEncontrada.Single();
        }
    }
    //public enum EnumFiltroPropriedade
    //{
    //    IgnorarTipoBase =1,
    //    Publica =2,
    //    GetPublico =4,
    //    SetPublico = 8,
    //    RetornarColecao = 16,
    //    IgnorarColecao =  32,

    //}
}