using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Snebur.Utilidade
{
    public class ZipUtil
    {
        private const string NOME_ARQUIVO_TEXTO = "texto.txt";

        public static void CompactarTexto(string texto, string caminhoDestino)
        {
            ArquivoUtil.DeletarArquivo(caminhoDestino);
            var bytes = CompactarTexto(texto);
            DiretorioUtil.CriarDiretorio(Path.GetDirectoryName(caminhoDestino));
            File.WriteAllBytes(caminhoDestino, bytes);
        }

        public static byte[] CompactarTexto(string texto)
        {
            return RetornarBytes(texto, NOME_ARQUIVO_TEXTO);
        }

        public static byte[] RetornarBytes(string texto, string nomeArquivo)
        {
            using (var msZip = new MemoryStream())
            {
                using (var zip = new ZipFile())
                {
                    var bytesUft8 = Encoding.UTF8.GetBytes(texto);
                    zip.AddEntry(nomeArquivo, bytesUft8);
                    zip.Save(msZip);
                }
                return msZip.ToArray();
            }
        }

        public static void CompactarArquivo(string caminhoArquivo,
                                            string caminhoDestino)
        {
            ArquivoUtil.DeletarArquivo(caminhoDestino, true);

            if (!File.Exists(caminhoArquivo))
            {
                throw new ErroArquivoNaoEncontrado(caminhoArquivo);
            }
            var fiOrigem = new FileInfo(caminhoArquivo);
            using (var fsOrigem = StreamUtil.OpenRead(caminhoArquivo))
            {
                using (var fsOrigemBuferizada = new BufferedStream(fsOrigem))
                {
                    using (var fsDestino = new FileStream(caminhoDestino, FileMode.Create, FileAccess.Write))
                    {
                        using (var fsDestinoBuferizada = new BufferedStream(fsDestino))
                        {
                            using (var zip = new ZipFile())
                            {
                                zip.AddEntry(fiOrigem.Name, fsOrigemBuferizada);
                                zip.Save(fsDestinoBuferizada);
                            }
                        }
                    }
                }
            }
        }

        public static byte[] CompactarArquivo(string caminhoArquivo)
        {
            return RetornarMemoryStreamCompactada(caminhoArquivo).ToArray();
        }

        public static MemoryStream RetornarMemoryStreamCompactada(string caminhoArquivo)
        {
            var nomeArquivo = Path.GetFileName(caminhoArquivo);
            using (var fs = StreamUtil.OpenRead(caminhoArquivo))
            {
                using (var bs = new BufferedStream(fs, TAMANHO_BUFFEER))
                {
                    var msZip = new MemoryStream();
                    using (var zip = new ZipFile())
                    {
                        zip.AddEntry(nomeArquivo, fs);
                        zip.Save(msZip);
                    }
                    msZip.Seek(0, SeekOrigin.Begin);

                    return msZip;
                }
            }
        }

        public static string DescompactarTexto(Stream stream)
        {
            if (stream != null)
            {
                stream.Seek(0, SeekOrigin.Begin);
                using (var ms = new MemoryStream())
                {
                    stream.CopyTo(ms);

                    ms.Seek(0, SeekOrigin.Begin);

                    using (var zip = ZipFile.Read(ms))
                    {
                        using (var arquivo = zip.Entries.First().OpenReader())
                        {
                            using (var sr = new StreamReader(arquivo, Encoding.UTF8))
                            {
                                var resultado = sr.ReadToEnd();
                                return resultado;
                            }
                        }
                    }
                }
            }
            return null;
        }

        public static void DescompactarArquivo(string caminhoArquivo, string caminhoDestino)
        {
            ArquivoUtil.DeletarArquivo(caminhoDestino, true);

            if (!File.Exists(caminhoArquivo))
            {
                throw new ErroArquivoNaoEncontrado(caminhoArquivo);
            }
            using (var fsOrigem = StreamUtil.OpenRead(caminhoArquivo))
            {
                using (var fsOrigemBuferizada = new BufferedStream(fsOrigem))
                {
                    using (var fsDestino = new FileStream(caminhoDestino, FileMode.Create, FileAccess.Write))
                    {
                        ZipUtil.DescompactarArquivo(fsOrigemBuferizada, fsDestino);
                    }
                }
            }
        }

        public static MemoryStream DescompactarArquivo(string caminhoArquivo)
        {
            using (var fsOrigem = new FileStream(caminhoArquivo, FileMode.Open))
            {
                return DescompactarArquivo(fsOrigem);
            }
        }

        public static MemoryStream DescompactarArquivo(Stream stream)
        {
            if (stream != null)
            {
                using (var ms = new MemoryStream())
                {
                    stream.CopyTo(ms);

                    ms.Seek(0, SeekOrigin.Begin);

                    using (var zip = ZipFile.Read(ms))
                    {
                        if (zip.Entries.Count != 1)
                        {
                            throw new Erro(String.Format("O numero de arquivo dentro do zip não é suportado {0}", zip.Entries.Count));
                        }
                        using (var arquivo = zip.Entries.First().OpenReader())
                        {
                            return StreamUtil.RetornarMemoryStream(arquivo);
                        }
                    }
                }
            }
            return null;
        }

        private static int TAMANHO_BUFFEER = 32 * 1024;

        public static void DescompactarArquivo(Stream streamOrigem, Stream streamDestino)
        {
            using (var zip = ZipFile.Read(streamOrigem))
            {
                if (zip.Entries.Count != 1)
                {
                    throw new Erro(String.Format("O numero de arquivo dentro do zip não é suportado {0}", zip.Entries.Count));
                }
                using (var streamArquivoZipado = zip.Entries.First().OpenReader())
                {
                    using (var streamZipBufferizada = new BufferedStream(streamArquivoZipado, TAMANHO_BUFFEER))
                    {
                        StreamUtil.SalvarStreamBufferizada(streamZipBufferizada, streamDestino);
                    }
                }
            }
        }
        public static List<string> Descompactar(string caminhoArquivo, string diretorioDestino)
        {
            using (var fs = new FileStream(caminhoArquivo, FileMode.Open))
            {
                return ZipUtil.DescompactarArquivos(fs, diretorioDestino);
            }
        }

        public static List<string> Descompactar(Stream stream, string diretorioDestino)
        {
            return ZipUtil.DescompactarArquivos(stream, diretorioDestino);
        }
        public static List<string> DescompactarArquivos(Stream stream, string diretorioDestino)
        {
            var arquivos = new List<string>();
            if (stream != null)
            {
                using (var ms = StreamUtil.RetornarMemoryStream(stream))
                {
                    using (var zip = ZipFile.Read(ms))
                    {
                        foreach (var entry in zip.Entries)
                        {
                            if (entry.IsDirectory)
                            {
                                //throw new NotImplementedException();
                                continue;
                            }
                            using (var zipStream = entry.OpenReader())
                            {
                                using (var msArquivo = StreamUtil.RetornarMemoryStream(zipStream))
                                {
                                    var arquivoDestino = new FileInfo(Path.Combine(diretorioDestino, entry.FileName));
                                    ArquivoUtil.DeletarArquivo(arquivoDestino);
                                    DiretorioUtil.CriarDiretorio(arquivoDestino.Directory.FullName);
                                    File.WriteAllBytes(arquivoDestino.FullName, msArquivo.ToArray());
                                    arquivos.Add(arquivoDestino.FullName);
                                }
                            }
                        }
                    }
                }
            }
            return arquivos;
        }
    }
}