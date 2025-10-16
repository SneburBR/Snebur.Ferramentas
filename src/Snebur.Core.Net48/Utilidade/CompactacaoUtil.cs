using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

#if NET40 == false && NET6_0_OR_GREATER == false
using SevenZip;
#endif

namespace Snebur.Utilidade
{
    public static class CompactacaoUtil
    {
        private static string[] _extensoes;

        public static string[] ExtensoesZip = { ".zip" };

        public static string[] ExtensoesRar = { ".rar" };

        public static string[] Extensoes7z = { ".7z" };

        public static string[] Extensoes
        {
            get
            {
                if (_extensoes == null)
                {
                    var extensoes = new List<string>();
                    extensoes.AddRange(ExtensoesZip);
                    extensoes.AddRange(ExtensoesRar);
                    extensoes.AddRange(Extensoes7z);

                    _extensoes = extensoes.ToArray();
                }
                return _extensoes;
            }
        }

        public static List<string> Descompactar(string caminhoArquivo, string diretorioDestino)
        {
            ValidacaoUtil.ValidarReferenciaNula(diretorioDestino, nameof(diretorioDestino));

            DiretorioUtil.CriarDiretorio(diretorioDestino);

            if (IsZip(caminhoArquivo))
            {
                return ZipUtil.Descompactar(caminhoArquivo, diretorioDestino);
            }
            else
            {
#if NET6_0_OR_GREATER || NET40
                throw new NotSupportedException();
#else

                SevenZipBase.SetLibraryPath(CaminhoBilioteca7zip);
                using (var sevenZip = new SevenZipExtractor(caminhoArquivo))
                {
                    sevenZip.ExtractFiles(diretorioDestino, sevenZip.ArchiveFileNames.ToArray());
                    return Directory.GetFiles(diretorioDestino, "*.*", SearchOption.AllDirectories).ToList();
                }
#endif
            }
        }

        public static bool IsArquiviCompactado(string caminhoArquivo)
        {
            return IsArquiviCompactado(new FileInfo(caminhoArquivo));
        }

        public static bool IsArquiviCompactado(FileInfo fi)
        {
            return Extensoes.Contains(fi.Extension, new IgnorarCasoSensivel());
        }

        public static bool IsZip(string caminhoArquivo)
        {
            return IsZip(new FileInfo(caminhoArquivo));
        }

        public static bool IsZip(FileInfo fi)
        {
            return ExtensoesZip.Contains(fi.Extension, new IgnorarCasoSensivel());
        }

        public static bool IsRar(string caminhoArquivo)
        {
            return IsRar(new FileInfo(caminhoArquivo));
        }

        public static bool IsRar(FileInfo fi)
        {
            return ExtensoesRar.Contains(fi.Extension, new IgnorarCasoSensivel());
        }

        public static bool IsZz(string caminhoArquivo)
        {
            return IsZz(new FileInfo(caminhoArquivo));
        }

        public static bool IsZz(FileInfo fi)
        {
            return Extensoes7z.Contains(fi.Extension, new IgnorarCasoSensivel());
        }

        private static string CaminhoBilioteca7zip
        {
            get
            {
                if (SistemaUtil.IsAplicacao64Bits)
                {
                    return CaminhoUtil.Combine(AplicacaoSnebur.Atual.DiretorioAplicacao, "x64", "7z.dll");
                }
                else
                {
                    return CaminhoUtil.Combine(AplicacaoSnebur.Atual.DiretorioAplicacao, "x86", "7z.dll");
                }
            }
        }
    }
}