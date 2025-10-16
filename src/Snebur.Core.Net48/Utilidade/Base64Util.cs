using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Snebur.Utilidade
{
    public static class Base64Util
    {
        private static readonly HashSet<char> Base64Chars = new HashSet<char>(new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '+', '/' });

        public static bool IsBase64String(string strBase64)
        {
            if (strBase64 == null || strBase64.Length == 0 ||
                strBase64.Length % 4 != 0 || strBase64.Contains(' ') ||
                strBase64.Contains('\t') || strBase64.Contains('\r') ||
                strBase64.Contains('\n'))
            {
                return false;
            }
            // 98% of all non base64 values are invalidated by this time.
            var index = strBase64.Length - 1;
            if (strBase64[index] == '=')
            {
                index--;
            }
            if (strBase64[index] == '=')
            {
                index--;
            }
            for (var i = 0; i <= index; i++)
            {
                if (!Base64Chars.Contains(strBase64[i]))
                {
                    return false;
                }
            }
            return true;
        }

        public static string Encode(string texto,
                                    bool isUtf8 = true)
        {
            if (!String.IsNullOrEmpty(texto))
            {
                if (isUtf8)
                {
                    var bytes = Encoding.UTF8.GetBytes(texto);
                    return Convert.ToBase64String(bytes);
                }
                return Convert.ToBase64String(Encoding.Default.GetBytes(texto));
            }
            return String.Empty;
        }

        public static string Decode(string strBase64,
                                    bool isUtf8 = true)
        {
            if (String.IsNullOrWhiteSpace(strBase64))
            {
                throw new Exception(String.Format("o conteudo não é uma base64 {0}", strBase64));
            }

            var bytes = Convert.FromBase64String(strBase64);
            if (isUtf8)
            {
                return Encoding.UTF8.GetString(bytes, 0, bytes.Length);
            }
            return Encoding.Default.GetString(bytes, 0, bytes.Length);
        }

        public static string UrlEncode(string text)
        {
            return UrlEncode(Encoding.UTF8.GetBytes(text));
        }
        public static string UrlEncode(byte[] data)
        {
            string base64 = Convert.ToBase64String(data);
            return base64
                .Replace("+", "-") // URL-safe character
                .Replace("/", "_") // URL-safe character
                .TrimEnd('=');     // Remove padding
        }

        public static string UrlDecodeToString(string base64Url)
        {
            return Encoding.UTF8.GetString(UrlDecode(base64Url));
        }

        public static byte[] UrlDecode(string base64Url)
        {
            string base64 = base64Url
                .Replace("-", "+")
                .Replace("_", "/");
            // Add padding if necessary
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }
    }
}