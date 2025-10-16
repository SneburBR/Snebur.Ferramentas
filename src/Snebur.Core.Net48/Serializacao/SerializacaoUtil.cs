using Newtonsoft.Json;
using Snebur.Dominio;
using Snebur.Dominio.Atributos;
using Snebur.Utilidade;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;

namespace Snebur.Serializacao
{
    public static class SerializacaoUtil
    {
        public static List<PropertyInfo> RetornarPropriedades(Type tipo)
        {
            var propriedades = ReflexaoUtil.RetornarPropriedades(tipo, false);
            propriedades = propriedades.Where(x => x.CanRead && x.CanWrite &&
                                                   x.GetGetMethod() != null && x.GetGetMethod().IsPublic &&
                                                   x.GetSetMethod() != null && x.GetSetMethod().IsPublic).
                                        Where(x =>
            {
                if (x.GetGetMethod().GetParameters().Length > 0)
                {
                    return false;
                }
                var atributoPropriedadeInterface = x.GetCustomAttribute(typeof(PropriedadeInterfaceAttribute));
                return atributoPropriedadeInterface == null;

            }).ToList();
            return propriedades;
        }

        public static string SerializarTipoSimples(object valor,
                                                   bool isNotNumericWithinSingleQuotes = false)
        {
            if (valor == null)
            {
                return null;
            }

            if (valor is BaseTipoComplexo tipoComplexto)
            {
                return JsonUtil.Serializar(tipoComplexto, EnumTipoSerializacao.Javascript);
            }

            var tipoP = ReflexaoUtil.RetornarTipoPrimarioEnum(valor.GetType());
            if (tipoP == Reflexao.EnumTipoPrimario.Desconhecido ||
                tipoP == Reflexao.EnumTipoPrimario.Object)
            {
                throw new Erro($"Tipo {tipoP} não suportado");
            }

            var tipo = ReflexaoUtil.RetornarTipoSemNullable(valor.GetType());
            if (tipo.IsEnum)
            {
                return Convert.ToInt32(valor).ToString();
            }

            switch (valor)
            {
                case Guid valorTipado:

                    return isNotNumericWithinSingleQuotes
                              ? $"\'{valorTipado}\'"
                              : valorTipado.ToString();

                case string valorTipado:

                    return isNotNumericWithinSingleQuotes
                              ? $"\'{EscapeSingleQuote(valorTipado)}\'"
                              : valorTipado;

                case decimal valorTipado:

                    return valorTipado.ToString(CultureInfo.InvariantCulture);

                case double valorTipado:

                    return valorTipado.ToString(CultureInfo.InvariantCulture);

                case float valorTipado:

                    return valorTipado.ToString(CultureInfo.InvariantCulture);

                case TimeSpan valorTipado:

                    var valorTimeSpanFormatado = valorTipado.ToString(null, CultureInfo.InvariantCulture);
                    return isNotNumericWithinSingleQuotes
                            ? $"\'{valorTimeSpanFormatado}\'"
                            : valorTimeSpanFormatado;

                case DateTime valorTipado:

                    var valorDateTimeFormatado = valorTipado.ToString(AplicacaoSnebur.Atual.CulturaPadrao);
                    return isNotNumericWithinSingleQuotes
                            ? $"\'{valorDateTimeFormatado}\'"
                            : valorDateTimeFormatado;

                case int valorTipado:

                    return valorTipado.ToString(CultureInfo.InvariantCulture);

                case long valorTipado:

                    return valorTipado.ToString(CultureInfo.InvariantCulture);

                case bool valorTipado:

                    return valorTipado.ToString(CultureInfo.InvariantCulture);

                default:

                    if (!tipo.IsValueType)
                    {
                        throw new Erro("Tipo não suportado");
                    }
                    var valorString = Convert.ToString(valor, CultureInfo.InvariantCulture);
                    return isNotNumericWithinSingleQuotes
                            ? $"\'{EscapeSingleQuote(valorString)}\'"
                            : valorString;
            }
        }

        public static object DeserilizarTipoSimples(Type tipo, string valorSerializado,
                                                    bool isNotNumericRemoveSingleQuotes = false)
        {
            if (String.IsNullOrWhiteSpace(valorSerializado))
            {
                return null;
            }

            if (tipo.IsSubclassOf(typeof(BaseTipoComplexo)))
            {
                return JsonUtil.Deserializar(valorSerializado, tipo, EnumTipoSerializacao.Javascript);
            }

            var tipoSemNullable = ReflexaoUtil.RetornarTipoSemNullable(tipo);
            if (tipoSemNullable.IsEnum)
            {
                return Convert.ToInt32(valorSerializado).ToString();
            }

            switch (tipo.Name)
            {
                case nameof(String):

                    return isNotNumericRemoveSingleQuotes
                              ? RemoveSingleQuotes(valorSerializado)
                              : valorSerializado;

                case nameof(Guid):

                    return isNotNumericRemoveSingleQuotes
                             ? Guid.Parse(RemoveSingleQuotes(valorSerializado))
                             : Guid.Parse(valorSerializado);

                case nameof(Int32):

                    return Int32.Parse(valorSerializado, CultureInfo.InvariantCulture);

                case nameof(Int64):

                    return Int64.Parse(valorSerializado, CultureInfo.InvariantCulture);

                case nameof(Boolean):

                    return Boolean.Parse(valorSerializado);

                case nameof(Decimal):

                    return Decimal.Parse(valorSerializado, CultureInfo.InvariantCulture);

                case nameof(Double):

                    return Double.Parse(valorSerializado, CultureInfo.InvariantCulture);

                case nameof(DateTime):

                    return isNotNumericRemoveSingleQuotes
                            ? DateTime.Parse(RemoveSingleQuotes(valorSerializado), CultureInfo.InvariantCulture)
                            : DateTime.Parse(valorSerializado, CultureInfo.InvariantCulture);

                case nameof(TimeSpan):

                    return isNotNumericRemoveSingleQuotes
                            ? TimeSpan.Parse(RemoveSingleQuotes(valorSerializado), CultureInfo.InvariantCulture)
                            : TimeSpan.Parse(valorSerializado, CultureInfo.InvariantCulture);

                case nameof(Single):

                    return Single.Parse(valorSerializado, CultureInfo.InvariantCulture);

                case nameof(Char):

                    return Char.Parse(valorSerializado);

                default:
                    if (!tipo.IsValueType)
                    {
                        throw new Erro("Tipo não suportado");
                    }

                    if (isNotNumericRemoveSingleQuotes)
                    {
                        valorSerializado = RemoveSingleQuotes(valorSerializado);
                    }
                    return Convert.ChangeType(valorSerializado, tipo, CultureInfo.InvariantCulture);

            }
        }

        private static string RemoveSingleQuotes(string valorString)
        {
            if (valorString[0] == '\'' && valorString[valorString.Length - 1] == '\'')
            {
                return valorString.Substring(1, valorString.Length - 2);
            }
            if (valorString[0] == '\'')
            {
                return valorString.Substring(1);
            }
            if (valorString[valorString.Length - 1] == '\'')
            {
                return valorString.Substring(0, valorString.Length - 1);
            }
            return valorString;
        }

        private static string EscapeSingleQuote(string valorString)
        {
            return valorString.Replace("\'", "\'\'");
        }

        internal static bool IsPoderDerializarPropriedade(PropertyInfo propriedade,
                                                          EnumTipoSerializacao tipoSerializacao)
        {
           

            if (propriedade.GetGetMethod().IsPublic)
            {
                var atributopsIgnorar = new List<Type>
                {
                    typeof(XmlIgnoreAttribute),
                    typeof(JsonIgnoreAttribute),
                };

                if (tipoSerializacao == EnumTipoSerializacao.Javascript)
                {
                    atributopsIgnorar.Add(typeof(IgnorarPropriedadeTSAttribute));
                }
                else if (tipoSerializacao == EnumTipoSerializacao.DotNet)
                {
                    atributopsIgnorar.Add(typeof(IgnorarPropriedadeDotNetAttribute));
                }

                return !atributopsIgnorar.Any(atributo => ReflexaoUtil.IsPropriedadePossuiAtributo(propriedade, atributo));
            }
            return false;
        }
    }
}
