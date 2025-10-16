using System;
using System.Text;
using System.Text.RegularExpressions;

namespace Snebur.Utilidade
{
    public static class HexUtil
    {
        public static bool IsHex(string hexString)
        {
            return Regex.IsMatch(hexString, @"\A\b[0-9a-fA-F]+\b\Z");
        }

        public static string Encode(string conteudo)
        {
            var sb = new StringBuilder();
            var bytes = Encoding.ASCII.GetBytes(conteudo);
            foreach (var t in bytes)
            {
                sb.Append(t.ToString("X2"));
            }
            return sb.ToString();
        }

        public static string Decode(string hexString)
        {
            var bytes = new byte[hexString.Length / 2];
            for (var i = 0; i < bytes.Length; i++)
            {
                bytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }

            return Encoding.ASCII.GetString(bytes); // returns: "Hello world" for "48656C6C6F20776F726C64"
        }
    }
}
