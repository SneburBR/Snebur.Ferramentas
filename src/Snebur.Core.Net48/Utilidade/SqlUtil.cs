using System;
using System.Data;
using System.Globalization;

namespace Snebur.Utilidade
{
    public static class SqlUtil
    {
        public static string SqlValueString(object value)
        {
            switch (value)
            {
                case int _int:
                    return $"{_int}";
                case long _long:
                    return $"{_long}";
                case float _float:
                    return $"{_float.ToString(CultureInfo.InvariantCulture)}";
                case double _double:
                    return $"{_double.ToString(CultureInfo.InvariantCulture)}";
                case decimal _decimal:
                    return $"{_decimal.ToString(CultureInfo.InvariantCulture)}";
                case DateTime _dateTime:
                    return $"'{_dateTime:yyyy-MM-dd HH:mm:ss}'";
                case bool _bool:
                    return $"{Convert.ToInt32(_bool)}";
                case byte _byte:
                    return $"{_byte}";
                case char _char:
                    return $"'{_char}'";
                case short _short:
                    return $"{_short}";
                case Guid _guid:
                    return $"'{_guid}'";
                case string _string:
                    return $"'{_string}'";
                case uint _uint:
                    return $"{_uint}";
                case ulong _ulong:
                    return $"{_ulong}";
                case ushort _ushort:
                    return $"{_ushort}";
                case sbyte _sbyte:
                    return $"{_sbyte}";
                case TimeSpan _timeSpan:
                    return $"'{_timeSpan:hh\\:mm\\:ss}'";
                default:
                    throw new Exception($"The type {value.GetType()} is not supported");
            }
        }

        public static SqlDbType GetBetterSqlDbType(object value)
        {
            if (value == null)
            {
                return SqlDbType.Variant;
            }

            var type = value.GetType();

            if (ReflexaoUtil.IsTipoNullable(type))
            {
                type = ReflexaoUtil.RetornarTipoSemNullable(type);
            }

            switch (Type.GetTypeCode(type))
            {
                case TypeCode.String:
                    return SqlDbType.NVarChar;
                case TypeCode.Int32:
                    return SqlDbType.Int;
                case TypeCode.Int64:
                    return SqlDbType.BigInt;
                case TypeCode.Double:
                    return SqlDbType.Float;
                case TypeCode.Decimal:
                    return SqlDbType.Decimal;
                case TypeCode.DateTime:
                    return SqlDbType.DateTime;
                case TypeCode.Boolean:
                    return SqlDbType.Bit;
                case TypeCode.Byte:
                    return SqlDbType.TinyInt;
                case TypeCode.Char:
                    return SqlDbType.Char;
                case TypeCode.Int16:
                    return SqlDbType.SmallInt;
                case TypeCode.Single:
                    return SqlDbType.Real;
                default:
                    if (type == typeof(Guid))
                    {
                        return SqlDbType.UniqueIdentifier;
                    }
                    if (type.IsEnum)
                    {
                        return SqlDbType.Int;
                    }
                    throw new Exception($"The type {type.Name} is not supported");
            }
        }
    }
}