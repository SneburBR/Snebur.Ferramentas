using System;
using System.Text.RegularExpressions;

namespace Snebur.Utilidade
{
    public static  class Base36Util
    {
        private const int Base = 36;
        private const string Chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        public static bool IsBase36(string valor, 
                                    bool isPermitirEspaco, 
                                    bool isIgnorarCase,
                                    string caracteresExtra = null)
        {
            if (String.IsNullOrWhiteSpace(valor))
            {
                return false;
            }

            if (isIgnorarCase)
            {
                valor = valor.ToUpper();
            }
             
            var regexString = @"^[0-9A-Z";
            if (isPermitirEspaco)
            {
                regexString += " ";
            }
            if (!String.IsNullOrWhiteSpace(caracteresExtra))
            {
                regexString += caracteresExtra;
            }
            regexString += "]+$";
            var regex = new Regex(regexString);
            return regex.IsMatch(valor);
        }

        public static string ToBase36(int valor)
        {
            var sb = new System.Text.StringBuilder();
            do
            {
                sb.Insert(0, Chars[valor % Base]);
                valor /= Base;
            } 
            while (valor != 0);
            return sb.ToString();
        }

        public static int FromBase36(string valor)
        {
            int result = 0;
            for (int i = 0; i < valor.Length; i++)
            {
                result = (result * Base) + Chars.IndexOf(valor[i]);
            }
            return result;
        }
    }
}
