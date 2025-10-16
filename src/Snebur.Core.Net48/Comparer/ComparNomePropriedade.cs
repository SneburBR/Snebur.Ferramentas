using Snebur.Utilidade;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Snebur.Comparer
{
    public class CompararPropriedade : IEqualityComparer<PropertyInfo>
    {
        private bool ComprarNome { get; set; }
        private bool CompararTipoProprieade { get; set; }
        private bool Igual { get; set; }

        public CompararPropriedade()
        {
            this.Igual = true;
        }

        public CompararPropriedade(EnumCompararPropriedade comparar)
        {
            var valores = EnumUtil.RetornarFlags<EnumCompararPropriedade>(comparar);
            foreach (var valor in valores)
            {
                switch (valor)
                {
                    case EnumCompararPropriedade.Igual:

                        this.Igual = true;
                        break;

                    case EnumCompararPropriedade.NomePropriedade:

                        this.ComprarNome = true;
                        break;
                    case EnumCompararPropriedade.TipoPropriedade:

                        this.CompararTipoProprieade = true;
                        break;

                    default:
                        break;
                }
            }
        }

        public bool Equals(PropertyInfo x, PropertyInfo y)
        {
            if (this.Igual)
            {
                return Object.Equals(x, y);
            }
            bool resultado = false;
            if (this.ComprarNome)
            {
                resultado = x.Name == y.Name;
            }
            if (!resultado)
            {
                return resultado;
            }
            if (this.CompararTipoProprieade)
            {
                resultado = x.PropertyType == y.PropertyType;
            }
            return resultado;
        }

        public int GetHashCode(PropertyInfo obj)
        {
            var hash = String.Concat(obj.Name, obj.PropertyType.Name);
            return hash.GetHashCode();
        }
    }

    public enum EnumCompararPropriedade
    {
        Igual = 1,
        NomePropriedade = 2,
        TipoPropriedade = 4,
    }
}