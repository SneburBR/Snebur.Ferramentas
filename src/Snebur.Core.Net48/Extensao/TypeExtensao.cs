using System.Collections.Generic;

namespace System
{
    public static class TypeExtensao
    {
        private static HashSet<Type> NumericTypes = new HashSet<Type> {
            typeof(byte),
            typeof(sbyte),
            typeof(short),
            typeof(ushort),
            typeof(int),
            typeof(uint),
            typeof(long),
            typeof(ulong),
            typeof(float),
            typeof(double),
            typeof(decimal)
        };

        private static HashSet<Type> IdTypes = new HashSet<Type> {
            typeof(byte),
            typeof(short),
            typeof(ushort),
            typeof(int),
            typeof(uint),
            typeof(long),
            typeof(ulong),
        };

        public static string RetornarCaminhoTipo(this Type tipo)
        {
            return $"{tipo.Namespace}.{tipo.Name}";
        }

        internal static bool IsNumericType(this Type type)
        {
            return NumericTypes.Contains(type) ||
                   NumericTypes.Contains(Nullable.GetUnderlyingType(type));
        }

        internal static bool IsIdType(this Type type)
        {
            return IdTypes.Contains(type) ||
                   IdTypes.Contains(Nullable.GetUnderlyingType(type));
        }
    }
}
