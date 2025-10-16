using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Snebur.Utilidade
{
    public static class ChecksumUtil
    {
        private const int TAMANHO_BUFFEER = 1024 * 1024;

        public static string RetornarChecksum(FileInfo arquivo)
        {
            return RetornarChecksum(arquivo.FullName);
        }
        public static string RetornarChecksum(string caminhoArquivo)
        {
            using (var fs = StreamUtil.OpenRead(caminhoArquivo))
            {
                return RetornarChecksum(fs);
            }
        }

        public static string RetornarChecksum(Stream stream)
        {
            if (stream.CanSeek)
            {
                stream.Seek(0, SeekOrigin.Begin);
            }
            var bufferStream = new BufferedStream(stream, TAMANHO_BUFFEER);
            using (var md5 = MD5.Create())
            {
                var checksumHash = md5.ComputeHash(bufferStream);
                return FormatarChecksum(checksumHash);
            }
        }
        /// <summary>
        /// Retornar checksum em MD5
        /// </summary>
        /// <param name="caminhoArquivo"></param>
        /// <returns></returns>
        public static string RetornarChecksum(byte[] bytes)
        {
            using (var md5 = MD5.Create())
            {
                var checksumHash = md5.ComputeHash(bytes);
                return FormatarChecksum(checksumHash);
            }
        }
        /// <summary>
        /// Retornar checksum em Sh256
        /// </summary>
        /// <param name="caminhoArquivo"></param>
        /// <returns></returns>
        public static string RetornarChecksumSh256(string caminhoArquivo)
        {
            using (var stream = new BufferedStream(File.OpenRead(caminhoArquivo), TAMANHO_BUFFEER))
            {
                using (var sh256 = SHA256.Create())
                {
                    var checksumHash = sh256.ComputeHash(stream);
                    return FormatarChecksum(checksumHash);
                }
            }
        }

        public static string RetornarChecksumSh256(byte[] bytes)
        {
            using (var sh256 = SHA256.Create())
            {
                var checksumHash = sh256.ComputeHash(bytes);
                return FormatarChecksum(checksumHash);
            }
        }

        //public static string RetornarChecksumCrc2(string texto)
        //{
        //    return ChecksumUtil.RetornarChecksumCrc2(Encoding.UTF8.GetBytes(texto));
        //}

        //public static string RetornarChecksumCrc2(byte[] bytes)
        //{
        //    using (var crc32 = new Crc32Algorithm())
        //    {
        //        return ChecksumUtil.FormatarChecksum(crc32.ComputeHash(bytes));
        //    }
        //}

        private static string FormatarChecksum(byte[] checksumHash)
        {
            var sb = new StringBuilder();
            foreach (var b in checksumHash)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }
    }
}