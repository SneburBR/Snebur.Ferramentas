using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Snebur.Utilidade
{
    public class CriptografiaUtil
    {
        public static string Criptografar(string chave, string conteudo)
        {
            ValidarChave(chave);
            chave = chave.Substring(0, 16);

            var senhaBytes = Encoding.UTF8.GetBytes(chave);
            var iv = Encoding.UTF8.GetBytes(chave);

            
            using (var aes = Aes.Create())
            {
                aes.Key = senhaBytes;
                aes.IV = iv;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                aes.FeedbackSize = 128;

                var criptografador = aes.CreateEncryptor(senhaBytes, iv);

                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, criptografador, CryptoStreamMode.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(cs))
                        {
                            sw.Write(conteudo);
                        }
                        return Convert.ToBase64String(ms.ToArray());
                    }
                }
            }
        }

        public static string Descriptografar(string chave, string conteudo)
        {
            ValidarChave(chave);
            chave = chave.Substring(0, 16);

            var senhaBytes = Encoding.UTF8.GetBytes(chave);
            var iv = Encoding.UTF8.GetBytes(chave);

            byte[] bytesCriptografado = Convert.FromBase64String(conteudo);
            using (var aes = Aes.Create())
            {
                aes.Key = senhaBytes;
                aes.IV = iv;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                aes.FeedbackSize = 128;

                var descritografador = aes.CreateDecryptor(senhaBytes, iv);

                using (var ms = new MemoryStream(bytesCriptografado))
                {
                    using (var cs = new CryptoStream(ms, descritografador, CryptoStreamMode.Read))
                    {
                        using (var sr = new StreamReader(cs))
                        {
                            return sr.ReadToEnd();
                        }
                    }
                }
            }
        }
        public static string CriptografarID(long id)
        {
            if (!(id > 0))
            {
                throw new Erro("O id deve ser um numero positivo maior que zero");
            }
            var str = id.ToString();
            if (id < 100)
            {
                str = str.PadLeft(4, '0');
            }
            var bytes = BaralhoUtil.Embaralhar(Encoding.ASCII.GetBytes(str));
            var strCript = Encoding.ASCII.GetString(bytes.Reverse().ToArray());
            var idCripgrafado = HexUtil.Encode(strCript);

            if (DebugUtil.IsAttached)
            {
                var idValida = DescriptografarID(idCripgrafado);
                if (id != idValida)
                {
                    throw new Exception("Falha na criptografia de ir");
                }
            }
            return idCripgrafado;
        }

        public static long DescriptografarID(string valorParametro)
        {
            var strCrpt = HexUtil.Decode(valorParametro);
            var bytes = BaralhoUtil.Desembaralhar(Encoding.ASCII.GetBytes(strCrpt).Reverse().ToArray());
            var str = Encoding.ASCII.GetString(bytes);
            var id = TextoUtil.RetornarSomenteNumeros(str);
            return Int64.Parse(id);
        }

        private static void ValidarChave(string chave)
        {
            if (chave.Length < 16)
            {
                throw new NotSupportedException("Chave não suportada");
            }
        }
    }
}