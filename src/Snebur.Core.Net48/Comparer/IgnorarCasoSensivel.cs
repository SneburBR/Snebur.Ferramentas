using System;
using System.Collections.Generic;

namespace Snebur
{
    public class IgnorarCasoSensivel : IEqualityComparer<string>
    {
        public bool Equals(string x, string y)
        {
            return x.ToLower() == y.ToLower();
        }

        public int GetHashCode(string obj)
        {
            if (obj == null)
            {
                return 0;
            }
            return obj.ToLower().GetHashCode();
        }
    }

    public class IgnorarCasoSensivelCaracter : IEqualityComparer<char>
    {
        public bool Equals(char x, char y)
        {
            return Char.ToLowerInvariant(x) == Char.ToLowerInvariant(y);
        }

        public int GetHashCode(char obj)
        {
            return Char.ToLowerInvariant(obj).GetHashCode();
        }
    }
}