using Snebur.Dominio;
using Snebur.Reflexao;
using System;
using System.Globalization;
using System.Linq;

namespace Snebur.Utilidade
{
    public class ConverterUtil
    {
        #region Convert

        #endregion

        public static T Converter<T>(object valor)
        {
            return (T)Converter(valor, typeof(T));
        }

        public static object Converter(object valor, Type tipo)
        {
            var tipoValor = valor?.GetType();
            if (valor == null || valor == DBNull.Value)
            {
                if (tipo.IsValueType)
                {
                    if (ReflexaoUtil.IsTipoNullable(tipo))
                    {
                        return null;
                    }
                    return RetornarValorNuloPadrao(ReflexaoUtil.RetornarTipoPrimarioEnum(tipo));
                }
                return null;
            }

            if (ReflexaoUtil.IsTipoNullable(tipo))
            {
                tipo = ReflexaoUtil.RetornarTipoSemNullable(tipo);
            }

            if (ReflexaoUtil.IsTipoIgualOuHerda(tipoValor, tipo))
            {
                return valor;
            }

            if (tipo.IsValueType || tipo == typeof(string))
            {
                if (tipo.IsEnum)
                {
                    return Enum.ToObject(tipo, Convert.ToInt32(valor));
                }
                return ConverterTipoPrimario(valor, ReflexaoUtil.RetornarTipoPrimarioEnum(tipo));
            }

            if (tipo.IsInterface)
            {
                if (tipo.IsAssignableFrom(tipoValor))
                {
                    return valor;
                }
            }

            return Convert.ChangeType(valor, tipo);
        }

        public static object ConverterTipoPrimario(object valor,
                                                  EnumTipoPrimario tipoPrimarioEnum)
        {
            if (IsValorVazioOuNull(valor))
            {
                return RetornarValorNuloPadrao(tipoPrimarioEnum);
            }
            else
            {
                switch (tipoPrimarioEnum)
                {
                    case EnumTipoPrimario.String:

                        return ParaString(valor);

                    case EnumTipoPrimario.Boolean:

                        return ParaBoolean(valor);

                    case EnumTipoPrimario.Integer:

                        return ParaInt32(valor);

                    case EnumTipoPrimario.Long:

                        return ParaInt64(valor);

                    case EnumTipoPrimario.Decimal:

                        return ParaDecimal(valor);

                    case EnumTipoPrimario.Double:

                        return ParaDouble(valor);

                    case EnumTipoPrimario.Guid:

                        return ParaGuid(valor);

                    case EnumTipoPrimario.TimeSpan:

                        return TimeSpan.Parse(valor.ToString());

                    case EnumTipoPrimario.DateTime:

                        return ParaDateTime(valor);

                    case EnumTipoPrimario.Single:

                        return ParaSingle(valor);

                    //case EnumTipoPrimario.Uri:

                    //    return new Uri(valor.ToString());

                    case EnumTipoPrimario.EnumValor:

                        return Int32.Parse(valor.ToString());

                    case EnumTipoPrimario.Char:

                        return Char.Parse(valor.ToString());

                    case EnumTipoPrimario.Byte:

                        return Convert.ToByte(valor);

                    default:
                        throw new ErroNaoSuportado(String.Format("O tipo {0} não é suportado", EnumUtil.RetornarDescricao(tipoPrimarioEnum)));
                }
            }
        }

        private static Guid ParaGuid(object valor)
        {
            Guid.TryParse(valor?.ToString(), out var resultado);
            return resultado;
        }

        public static bool ParaBoolean(object valor)
        {
            if (valor == null || valor == DBNull.Value || String.IsNullOrWhiteSpace(valor.ToString()))
            {
                return false;
            }
            else
            {
                if (valor is string)
                {
                    var valorString = valor.ToString().ToLower().Trim();
                    switch (valorString)
                    {
                        case "true":
                        case "yes":
                        case "sim":
                        case "1":

                            return true;

                        case "false":
                        case "no":
                        case "nao":
                        case "não":
                        case "0":

                            return false;

                        default:

                            if (Boolean.TryParse(valorString, out var resultdo))
                            {
                                return resultdo;
                            }
                            return false;
                            //throw new ErroNaoSuportado(String.Format("Não é possível converter o valor {0} em bollean", valorString));
                    }
                }
                else
                {
                    if (valor is IConvertible convertible)
                    {
                        return convertible.ToBoolean(CultureInfo.CurrentCulture);
                    }
                    return false;
                }
            }
        }

        public static object Para(object valor, Type tipo)
        {
            if (valor == null || DBNull.Value == valor)
            {
                return default;
            }

            if (valor.GetType() == ReflexaoUtil.RetornarTipoSemNullable(tipo))
            {
                return valor;
            }

            var tipoPrimario = ReflexaoUtil.RetornarTipoPrimarioEnum(tipo);
            if (tipoPrimario == EnumTipoPrimario.Desconhecido)
            {
                return Convert.ChangeType(valor, tipo);
            }
            return ConverterTipoPrimario(valor, tipoPrimario);
        }

        public static T Try<T>(object valor)
        {
            try
            {
                return Para<T>(valor);
            }
            catch
            {
                return default;
            }

        }
        public static T Para<T>(object valor)
        {
            if (valor == null || DBNull.Value == valor ||
               (valor is string str && String.IsNullOrEmpty(str)))
            {
                return default;
            }

            var tipo = typeof(T);
            var tipoPrimario = ReflexaoUtil.RetornarTipoPrimarioEnum(tipo);
            if (tipoPrimario == EnumTipoPrimario.Desconhecido)
            {
                return (T)Convert.ChangeType(valor, tipo);
            }

            return (T)ConverterTipoPrimario(valor, tipoPrimario);
        }

        public static int ParaInt32(object valor, IFormatProvider provider = null)
        {
            if (valor is int)
            {
                return (int)valor;
            }
            if (IsValorVazioOuNull(valor))
            {
                return 0;
            }

            if (valor is IConvertible convertible)
            {
                return convertible.ToInt32(provider ?? CultureInfo.InvariantCulture);
            }
            return 0;
        }

        public static long ParaInt64(object valor, IFormatProvider provider = null)
        {
            if (valor is long lvalor)
            {
                return lvalor;
            }
            if (IsValorVazioOuNull(valor))
            {
                return 0L;
            }
            if (valor is IConvertible convertible)
            {
                return convertible.ToInt64(provider ?? CultureInfo.InvariantCulture);
            }
            return 0L;
        }

        public static decimal ParaDecimal(object valor, IFormatProvider provider = null)
        {
            if (valor is decimal d)
            {
                return d;
            }
            if (IsValorVazioOuNull(valor))
            {
                return 0M;
            }
            if (valor is IConvertible convertible)
            {
                return convertible.ToDecimal(provider ?? CultureInfo.InvariantCulture);
            }
            return 0m;
        }

        public static double ParaSingle(object valor, IFormatProvider provider = null)
        {
            if (valor is float d)
            {
                return d;
            }
            if (IsValorVazioOuNull(valor))
            {
                return 0D;
            }
            if (valor is IConvertible convertible)
            {
                return convertible.ToSingle(provider ?? CultureInfo.InvariantCulture);
            }
            return 0f;
        }

        public static double ParaDouble(object valor, IFormatProvider provider = null)
        {
            if (valor is double d)
            {
                return d;
            }

            if (IsValorVazioOuNull(valor))
            {
                return 0D;
            }

            if(valor is string str)
            {
                if(provider== null)
                {
                    var indexLastVirgual = str.LastIndexOf(',');
                    var indexLastPonto = str.LastIndexOf('.');
                    if (indexLastVirgual > indexLastPonto)
                    {
                        provider = new CultureInfo("pt-BR");
                    }
                    else
                    {
                        provider = CultureInfo.InvariantCulture;
                    }
                }
            }

            if (valor is IConvertible convertible)
            {
                return convertible.ToDouble(provider ?? CultureInfo.InvariantCulture);
            }
            return 0d;
        }

        public static DateTime ParaDateTime(object valor, IFormatProvider provider = null)
        {
            if (valor is DateTime d)
            {
                return d;
            }

            if (IsValorVazioOuNull(valor))
            {
                return DateTime.Now;
            }
            var dataString = valor.ToString();

            if (DateTime.TryParse(dataString,
                                  CultureInfo.CurrentCulture,
                                  DateTimeStyles.AssumeUniversal,
                                  out DateTime data))
            {
                return data;
            }
            if (DateTime.TryParse(dataString,
                              CultureInfo.InvariantCulture,
                              DateTimeStyles.AssumeUniversal,
                              out DateTime data2))
            {
                return data2;
            }

            if (Int64.TryParse(dataString, out long ticks) && ticks > DateTime.Now.AddYears(-100).Ticks)
            {
                return new DateTime(ticks);
            }

            if (!(dataString.Contains("/") ^ dataString.Contains('-')))
            {
                throw new Erro(String.Format("O valor não pode ser convertido para data {0}", dataString));
            }
            var divisorData = dataString.Contains('/') ? '/' : '-';
            var divisorHora = ':';

            var partes = dataString.Split(' ');
            var partaDataString = partes[0].Trim();
            var parteHoraString = partes.Length > 1 ? partes[1].Trim() : null;

            var partesData = partaDataString.Split(divisorData);
            var partesHora = (parteHoraString == null) ? null : parteHoraString.Split(divisorHora);

            if (partesData.Length >= 3)
            {
                var parteAno = partesData[0];
                var parteMes = partesData[1];
                var parteDia = partesData[2];

                if (int.TryParse(parteAno, out int ano) &&
                    int.TryParse(parteMes, out int mes) &&
                    int.TryParse(parteDia, out int dia))
                {
                    if (partesHora == null)
                    {
                        return new DateTime(ano, mes, dia);
                    }
                }
            }

            if (valor is IConvertible convertible)
            {
                return convertible.ToDateTime(provider ?? CultureInfo.InvariantCulture);
            }

            return DateTime.MinValue;
        }

        private static bool IsValorVazioOuNull(object valor)
        {
            return valor == null ||
                   valor == DBNull.Value ||
                   String.IsNullOrWhiteSpace(valor.ToString());
        }

        public static object RetornarValorNuloPadrao(EnumTipoPrimario tipoPrimarioEnum)
        {
            switch (tipoPrimarioEnum)
            {
                case EnumTipoPrimario.Desconhecido:

                    return null;

                case EnumTipoPrimario.String:

                    return null;

                case EnumTipoPrimario.Boolean:

                    return false;

                case EnumTipoPrimario.Integer:

                    return 0;

                case EnumTipoPrimario.Long:

                    return 0L;

                case EnumTipoPrimario.Decimal:

                    return 0M;

                case EnumTipoPrimario.Double:

                    return 0D;

                case EnumTipoPrimario.Guid:

                    return Guid.Empty;

                case EnumTipoPrimario.TimeSpan:

                    return DateTime.Now.TimeOfDay;

                case EnumTipoPrimario.DateTime:

                    return DateTime.Now;

                //case EnumTipoPrimario.Uri:
                //    return null;
                //case EnumTipoPrimario.Object:
                //    return null;
                case EnumTipoPrimario.EnumValor:

                    return 0;

                case EnumTipoPrimario.Single:

                    return 0F;

                case EnumTipoPrimario.Char:

                    return ' ';

                case EnumTipoPrimario.Byte:

                    return Byte.MinValue;

                default:

                    throw new ErroNaoSuportado(String.Format("O tipo {0} não é suportado", EnumUtil.RetornarDescricao(tipoPrimarioEnum)));
            }
        }

        public static string ParaString(object valorTipado, bool isAceitarNulo = false)
        {
            if (valorTipado == null)
            {
                if (isAceitarNulo)
                {
                    return null;
                }
                return String.Empty;
            }
            return valorTipado.ToString();
        }

        public static string ConverterHexaParaRgbaInterno(string corHexa)
        {
            if (corHexa.StartsWith("#"))
            {
                corHexa = corHexa.Substring(1);
            }

            if (corHexa.Length == 3)
            {
                var r = (byte)Convert.ToInt32(corHexa.Substring(0, 1), 16);
                var g = (byte)Convert.ToInt32(corHexa.Substring(1, 1), 16);
                var b = (byte)Convert.ToInt32(corHexa.Substring(2, 1), 16);
                return RetornarRgbaCorInterno(r, g, b, 255);
            }
            else
            {
                var strR = corHexa.Substring(0, 2) ?? "00";
                var strG = corHexa.Substring(2, 2) ?? "00";
                var strB = corHexa.Substring(4, 2) ?? "00";
                var strA = corHexa.Length == 8 ? corHexa.Substring(6, 8) : "FF";

                var r = (byte)Convert.ToInt32(strR, 16);
                var g = (byte)Convert.ToInt32(strG, 16);
                var b = (byte)Convert.ToInt32(strB, 16);
                var a = (byte)Convert.ToInt32(strA, 16);

                return RetornarRgbaCorInterno(r, g, b, a);
            }
            //return null;
        }
        private static string RetornarRgbaCorInterno(byte red, byte green, byte blue, byte alpha)
        {
            var alphaDecimal = (alpha / (decimal)byte.MaxValue);
            return $"rgba({red},{green},{blue},{Math.Round(alphaDecimal, 2)})";
        }

        internal static Dimensao ParaDimensao(object valor)
        {
            if(valor== null)
            {
                return Dimensao.Empty;
            }

            if (valor is Dimensao dimensao)
            {
                return dimensao;
            }

            if (valor is IDimensao _dimensao)
            {
                return new Dimensao(_dimensao.Largura, _dimensao.Altura);
            }

            if(valor is string str)
            {
                return Dimensao.Parse(str);
            }
            
            throw new ErroNaoSuportado($"Não foi possível converter o valor {valor} para Dimensão");
        }
    }
}