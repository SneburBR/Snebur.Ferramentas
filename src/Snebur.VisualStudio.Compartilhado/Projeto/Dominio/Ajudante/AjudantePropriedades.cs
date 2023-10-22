using Snebur.Dominio.Atributos;
using Snebur.Linq;
using Snebur.Utilidade;
using Snebur.VisualStudio.Reflexao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Snebur.VisualStudio
{
    public class AjudantePropriedades
    {
        public static BindingFlags Estaticas = BindingFlags.Public | BindingFlags.Static;

        public static BindingFlags Publicas = BindingFlags.Public |
                                              BindingFlags.Instance  ;

        public static BindingFlags PropriedadesInterface = BindingFlags.NonPublic |
                                                           BindingFlags.Instance ;

        public static List<PropertyInfo> RetornarPropriedadesClasseEstaticas(Type tipo)
        {
            var propriedades = ReflexaoUtil.RetornarPropriedades(tipo, AjudantePropriedades.Estaticas, true);
            propriedades = propriedades.Where(x => x.GetGetMethod().IsStatic && x.GetSetMethod().IsStatic).ToList();
            propriedades = propriedades.Where(x => !x.GetCustomAttributes().Any(k => IsAtributoIgnorarPropriedade(k.GetType().Name))).ToList();
            return propriedades;
        }


        public static List<PropertyInfo> RetornarPropriedadesClassePublicas(Type tipo,
                                                                            bool ignoratTipoBase, 
                                                                            bool isIgnorarPropriedadeOverride = true)
        {

            var propriedades = ReflexaoUtil.RetornarPropriedades(tipo, AjudantePropriedades.Publicas, ignoratTipoBase);

            propriedades = propriedades.Where(x => x.GetGetMethod() != null && x.GetGetMethod().IsPublic).ToList();

            //propriedades = propriedades.Where(x => !x.GetSetMethod().IsAbstract && !x.GetGetMethod().IsAbstract).ToList();
            if (ignoratTipoBase && isIgnorarPropriedadeOverride)
            {
                propriedades = propriedades.Where(x =>
                {
                    var getMetodo = x.GetGetMethod(false);
                    var getBaseMetodo = getMetodo.GetBaseDefinition();
                    return getMetodo == getBaseMetodo;

                }).ToList();
            }
           

            propriedades = propriedades.Where(x => !x.GetCustomAttributes().Any(k => IsAtributoIgnorarPropriedade(k.GetType().Name))).
                                        Where(x => !x.GetCustomAttributes().Any(k => k.GetType().Name == nameof(PropriedadeTSEspecializadaAttribute))).ToList();

            return propriedades;
        }

        public static List<PropertyInfo> RetornarPropriedadesClasseEspecializada(Type tipo, bool isIgnorarTipoBase)
        {
            var propriedades = ReflexaoUtil.RetornarPropriedades(tipo, AjudantePropriedades.Publicas, isIgnorarTipoBase);

            propriedades = propriedades.Where(x => x.GetGetMethod() != null && x.GetGetMethod().IsPublic).ToList();

            if (propriedades.Count > 0)
            {
                var p = propriedades.First();
            }


            propriedades = propriedades.Where(x => x.GetCustomAttributes().
                                                     Any(k => k.GetType().Name == nameof(PropriedadeTSEspecializadaAttribute))).
                                        ToList();

            return propriedades;
        }


        public static List<PropertyInfo> RetornarPropriedadesReflexao(Type tipo, bool ignorarTipoBase = true)
        {

            var propriedades = ReflexaoUtil.RetornarPropriedades(tipo, AjudantePropriedades.Publicas, ignorarTipoBase);

            var query = propriedades.Where(x => (x.GetMethod != null &&
                                                 x.SetMethod != null &&
                                                 x.GetMethod.IsPublic &&
                                                 x.SetMethod.IsPublic) ||
                                                 x.Name == "__PropriedadesAlteradas" ||
                                                 x.Name == "__NomeTipoEntidade");
            //propriedades = propriedades.Where(x => !x.GetSetMethod().IsAbstract && !x.GetGetMethod().IsAbstract).ToList();
            //remover propriedades overrides

            query = query.Where(x =>
            {
                var getMetodo = x.GetGetMethod(false);
                var getBaseMetodo = getMetodo.GetBaseDefinition();
                return getMetodo == getBaseMetodo;
            });

            //query = query.Where(x =>
            //{
            //    if (x.SetMethod == null)
            //    {
            //        return ReflexaoUtil.TipoRetornaColecao(x.PropertyType);
            //    }
            //    return x.SetMethod.IsPublic;

            //});

            query = query.Where(x => !x.GetCustomAttributes().Any(k => k.GetType().Name == AjudanteAssembly.NomeTipoIgnorarPropriedadeTSReflexao));



            return query.ToList();

        }

        public static List<PropertyInfo> RetornarPropriedadesInterface(Type tipo)
        {
            var propriedades = ReflexaoUtil.RetornarPropriedades(tipo, AjudantePropriedades.PropriedadesInterface, true);
            propriedades = propriedades.Where(x => PropriedadeUtil.PossuiAtributo(x, AjudanteAssembly.NomeTipoAtributoProprieadeInterface)).ToList();
            propriedades = propriedades.Where(x => !x.GetCustomAttributes().Any(k => IsAtributoIgnorarPropriedade( k.GetType().Name))).ToList();
            return propriedades;
        }

        public static bool IsAtributoIgnorarPropriedade(string nomeAtributo)
        {
            return nomeAtributo == AjudanteAssembly.NomeTipoIgnorarPropriedade ||
                   nomeAtributo == AjudanteAssembly.NomeTipoIgnorarPropriedadeTS;
        }
    }
}
