using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snebur;
using Snebur.Utilidade;
using System.Reflection;
using Snebur.VisualStudio.Reflexao;

namespace Snebur.VisualStudio.Projeto.Dominio
{
    public class AjudantePropriedades
    {
        public static BindingFlags Estaticas = BindingFlags.Public | BindingFlags.Static;

        public static BindingFlags Publicas = BindingFlags.Public |
                                              BindingFlags.Instance |
                                              BindingFlags.DeclaredOnly;

        public static BindingFlags PropriedadesInterface = BindingFlags.NonPublic |
                                                           BindingFlags.Instance |
                                                           BindingFlags.DeclaredOnly;

        public static List<PropertyInfo> RetornarPropriedadesClasseEstaticas(Type tipo)
        {
            var propriedades = ReflexaoUtil.RetornarPropriedades(tipo, AjudantePropriedades.Estaticas, true);
            propriedades = propriedades.Where(x => x.GetGetMethod().IsStatic && x.GetSetMethod().IsStatic).ToList();
            propriedades = propriedades.Where(x => !x.GetCustomAttributes().Any(k => k.GetType().Name == AjudanteAssembly.NomeTipoIgnorarPropriedadeTS)).ToList();
            return propriedades;
        }


        public static List<PropertyInfo> RetornarPropriedadesClassePublicas(Type tipo)
        {

            var propriedades = ReflexaoUtil.RetornarPropriedades(tipo, AjudantePropriedades.Publicas, true);

            propriedades = propriedades.Where(x => x.GetGetMethod().IsPublic && x.GetSetMethod().IsPublic).ToList();

            //propriedades = propriedades.Where(x => !x.GetSetMethod().IsAbstract && !x.GetGetMethod().IsAbstract).ToList();
            propriedades = propriedades.Where(x =>
            {
                var getMetodo = x.GetGetMethod(false);
                var getBaseMetodo = getMetodo.GetBaseDefinition();
                return getMetodo == getBaseMetodo;

            }).ToList();

            propriedades = propriedades.Where(x => !x.GetCustomAttributes().Any(k => k.GetType().Name == AjudanteAssembly.NomeTipoIgnorarPropriedadeTS)).ToList();

            return propriedades;
        }


        public static List<PropertyInfo> RetornarPropriedadesReflexao(Type tipo, bool ignorarTipoBase = true)
        {

            var propriedades = ReflexaoUtil.RetornarPropriedades(tipo, AjudantePropriedades.Publicas, ignorarTipoBase);

            propriedades = propriedades.Where(x => x.GetGetMethod() != null && x.GetSetMethod() != null &&  
                                                  x.GetGetMethod().IsPublic && x.GetSetMethod().IsPublic).ToList();
            //propriedades = propriedades.Where(x => !x.GetSetMethod().IsAbstract && !x.GetGetMethod().IsAbstract).ToList();

            //remover propriedes overrides
            propriedades = propriedades.Where(x =>
            {
                var getMetodo = x.GetGetMethod(false);
                var getBaseMetodo = getMetodo.GetBaseDefinition();
                return getMetodo == getBaseMetodo;

            }).ToList();


            propriedades = propriedades.Where(x => !x.GetCustomAttributes().Any(k => k.GetType().Name == AjudanteAssembly.NomeTipoIgnorarPropriedadeTSReflexao)).ToList();

            return propriedades;

        }

        public static List<PropertyInfo> RetornarPropriedadesInterface(Type tipo)
        {
            var propriedades = ReflexaoUtil.RetornarPropriedades(tipo, AjudantePropriedades.PropriedadesInterface, true);
            propriedades = propriedades.Where(x => PropriedadeUtil.PossuiAtributo(x, AjudanteAssembly.NomeTipoAtributoProprieadeInterface)).ToList();
            propriedades = propriedades.Where(x => !x.GetCustomAttributes().Any(k => k.GetType().Name == AjudanteAssembly.NomeTipoIgnorarPropriedadeTS)).ToList();
            return propriedades;
        }
    }
}
