using Snebur.Comparer;
using Snebur.Dominio;
using Snebur.Dominio.Atributos;
using Snebur.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Snebur.Utilidade
{
    public static class AutoMapearUtil
    {
        public static void Mapear(object origem, object destino)
        {
            Mapear(origem, destino, false);
        }

        public static void Mapear(object origem, object destino, bool ignorarErro, Func<(PropertyInfo PropriedadeOrigem, PropertyInfo PropriedadeDestino), bool> manipuladorIsMapearPropriedade)
        {
            Mapear(origem, destino, ignorarErro, null, manipuladorIsMapearPropriedade);
        }

        public static void Mapear(object origem, object destino, bool ignorarErro)
        {
            Mapear(origem, destino, ignorarErro, new List<string>());
        }

        public static void Mapear<TOrigem>(TOrigem origem, object destino, bool ignorarErro, params Expression<Func<TOrigem, object>>[] propriedadesIgnoraOrigem)
        {
            var propriedades = new List<PropertyInfo>();
            foreach (var expressao in propriedadesIgnoraOrigem)
            {
                propriedades.AddRange(ExpressaoUtil.RetornarPropriedades(expressao));
            }
            Mapear(origem, destino, ignorarErro, propriedades);
        }

        public static void Mapear<TOrigem>(TOrigem origem, object destino, bool ignorarErro, List<PropertyInfo> propriedades)
        {
            Mapear(origem, destino, ignorarErro, propriedades.Select(x => x.Name).ToList());
        }

        public static void Mapear(object origem, object destino, bool ignorarErro, List<string> propriedadesIgnroar, Func<(PropertyInfo PropriedadeOrigem, PropertyInfo PropriedadeDestino), bool> manipuladorIsMapearPropriedade = null)
        {
            ValidacaoUtil.ValidarReferenciaNula(origem, nameof(origem));
            ValidacaoUtil.ValidarReferenciaNula(destino, nameof(destino));

            var propriedadesOrigem = ReflexaoUtil.RetornarPropriedades(origem.GetType(), false);
            var propriedadesDestino = ReflexaoUtil.RetornarPropriedades(destino.GetType(), false);

            propriedadesOrigem = propriedadesOrigem.Where(x => x.CanRead && (x.GetGetMethod()?.IsPublic ?? false) && x.GetCustomAttribute<NotMappedAttribute>() == null).ToList();
            propriedadesDestino = propriedadesDestino.Where(x => x.CanWrite && (x.GetSetMethod()?.IsPublic ?? false) && x.GetCustomAttribute<NotMappedAttribute>() == null).ToList();

            if (destino is Entidade entidade)
            {
                var propriedadeIngorar = RetornarPropriedadesIngorarAutoMapear(entidade.GetType());
                propriedadesDestino.RemoveRange(propriedadeIngorar);
                //propriedadesDestino.Remove(EntidadeUtil.RetornarPropriedad(entidade.GetType()));
            }
            propriedadesDestino = propriedadesDestino.Intersect(propriedadesOrigem, new CompararPropriedade(EnumCompararPropriedade.NomePropriedade | EnumCompararPropriedade.TipoPropriedade)).ToList();

            foreach (var propriedadeDestino in propriedadesDestino)
            {
                var propriedadeOrigem = propriedadesOrigem.Where(x => x.Name == propriedadeDestino.Name).FirstOrDefault();
                if (propriedadeOrigem != null && (propriedadesIgnroar == null || !propriedadesIgnroar.Contains(propriedadeOrigem.Name, new IgnorarCasoSensivel())))
                {

                    if (ReflexaoUtil.IsTipoIgualOuHerda(propriedadeOrigem.PropertyType, propriedadeDestino.PropertyType) ||
                       ReflexaoUtil.IsTipoIgualOuHerda(propriedadeDestino.PropertyType, propriedadeOrigem.PropertyType))
                    {
                        var isMapearPropriedade = manipuladorIsMapearPropriedade?.Invoke((propriedadeOrigem, propriedadeDestino)) ?? true;
                        if (isMapearPropriedade)
                        {
                            try
                            {
                                var valorOrigem = propriedadeOrigem.GetValue(origem);
                                if (valorOrigem != null)
                                {
                                    propriedadeDestino.SetValue(destino, valorOrigem);
                                }
                            }
                            catch (Exception)
                            {
                                if (!ignorarErro)
                                {
                                    throw;
                                }
                            }
                        }

                    }
                }
            }
        }

        private static List<PropertyInfo> RetornarPropriedadesIngorarAutoMapear(Type tipoEntidade)
        {
            var atributosIgnorar = new List<Type>();

            atributosIgnorar.Add(typeof(ValorPadraoIDUsuarioLogadoAttribute));
            atributosIgnorar.Add(typeof(ValorPadraoDataHoraServidorAttribute));
            atributosIgnorar.Add(typeof(ValorPadraoIDUsuarioLogadoAttribute));
            atributosIgnorar.Add(typeof(ValorPadraoIPAttribute));

            var propriedadeChavePrimaria = tipoEntidade.GetProperties().Where(x => x.GetCustomAttribute<KeyAttribute>() != null).Single();

            var propridades = tipoEntidade.GetProperties();
            var retorno = new List<PropertyInfo>();
            retorno.Add(propriedadeChavePrimaria);
            retorno.AddRange(typeof(Entidade).GetProperties());

            retorno.AddRange(propridades.Where(x => atributosIgnorar.Any(a => x.GetCustomAttribute(a, false) != null)));

            if (typeof(IDeletado).IsAssignableFrom(tipoEntidade))
            {
                var propriedadesIdeletado = typeof(IDeletado).GetProperties().Select(x => x.Name).ToList();
                retorno.AddRange(propridades.Where(x => propriedadesIdeletado.Contains(x.Name)));
            }
            return retorno;
        }
    }
}