using Snebur.Dominio.Atributos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Snebur.Utilidade
{
    public static partial class ReflexaoUtil
    {
        public static object RetornarValorPadraoPropriedade(PropertyInfo propriedade)
        {
            var aceitaNulo = !propriedade.PropertyType.IsValueType || IsTipoNullable(propriedade.PropertyType);

            var atributoValorPadrao = propriedade.GetCustomAttribute<ValorPadraoAttribute>();
            if (atributoValorPadrao != null)
            {
                return ConverterUtil.Converter(atributoValorPadrao.ValorPadrao, propriedade.PropertyType);
            }
            if (aceitaNulo)
            {
                return null;
            }
            else
            {
                var tipoPrimarioEnum = RetornarTipoPrimarioEnum(propriedade.PropertyType);
                return ConverterUtil.RetornarValorNuloPadrao(tipoPrimarioEnum);
            }
        }

        public static string RetornarRotulo(PropertyInfo pi)
        {
            RotuloAttribute atributoRotulo = (RotuloAttribute)pi.GetCustomAttributes(typeof(RotuloAttribute), true).FirstOrDefault();
            if (atributoRotulo == null)
            {
                return pi.Name;
            }
            else
            {
                return atributoRotulo.Rotulo;
            }
        }

        public static bool TipoPossuiPropriedade(Type tipo, PropertyInfo propriedade)
        {
            return propriedade.DeclaringType == tipo || tipo.IsSubclassOf(propriedade.DeclaringType);
        }

        public static string RetornarPluralClasse(Type tipoClasse)
        {
            var atributoRotulo = (RotuloAttribute)tipoClasse.GetCustomAttributes(typeof(RotuloAttribute), true).FirstOrDefault();
            if (atributoRotulo == null)
            {
                return string.Concat(tipoClasse.Name, "s");
            }
            else
            {
                if (string.IsNullOrWhiteSpace(atributoRotulo.RotuloPlural))
                {
                    return string.Concat(tipoClasse.Name, "s");
                }
                else
                {
                    return atributoRotulo.RotuloPlural;
                }
            }
        }

        public static List<PropertyInfo> RetornarPropriedadesEntidade(Type tipo)
        {
            return RetornarPropriedades(tipo, true).
                                    Where(x =>
                                    {
                                        if (IsPropriedadeRetornaTipoPrimario(x, true) &&
                                            x.GetGetMethod() != null && x.GetGetMethod().IsPublic &&
                                            x.GetSetMethod() != null && x.GetSetMethod().IsPublic)
                                        {
                                            var atrituboNaoMapear = x.GetCustomAttribute<NaoMapearAttribute>();
                                            return (atrituboNaoMapear == null);
                                        }
                                        return false;

                                    }).ToList();
        }
    }
}