using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace Snebur.Utilidade
{
    public static class ZipUtil
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
                using (var zip = new ZipArchive(msZip, ZipArchiveMode.Create))
                {
                    var arquivo = zip.CreateEntry(nomeArquivo);

                    var bytesUft8 = Encoding.UTF8.GetBytes(texto);
                    using (var sr = arquivo.Open())
                    {
                        sr.Write(bytesUft8, 0, bytesUft8.Length);
                        sr.Flush();
                    }
                }
                return msZip.ToArray();
            }
        }

        public static void CompactarArquivo(string caminhoArquivo,
                                            string caminhoDestino,
                                            string senha)
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
                            using (var zip = new ZipArchive(fsDestinoBuferizada, ZipArchiveMode.Create))
                            {
                                var arquivo = zip.CreateEntry(fiOrigem.Name);
                                using (var sr = arquivo.Open())
                                {
                                    var buffer = new byte[ushort.MaxValue];
                                    while (true)
                                    {
                                        var lidos = fsOrigem.Read(buffer, 0, buffer.Length);
                                        if (lidos == 0)
                                        {
                                            break;
                                        }
                                        sr.Write(buffer, 0, lidos);
                                    }
                                    sr.Flush();
                                }
                            }
                        }
                    }
                }
            }
        }
        public static byte[] CompactarArquivo(string caminhoArquivo, string senha)
        {
            return RetornarMemoryStreamCompactada(caminhoArquivo, senha).ToArray();
        }

        public static MemoryStream RetornarMemoryStreamCompactada(string caminhoArquivo, string senha)
        {
            var nomeArquivo = Path.GetFileName(caminhoArquivo);
            using (var fsOrigem = StreamUtil.OpenRead(caminhoArquivo))
            {
                using (var bs = new BufferedStream(fsOrigem, TAMANHO_BUFFEER))
                {
                    var msZip = new MemoryStream();
                    using (var zip = new ZipArchive(msZip, ZipArchiveMode.Create))
                    {
                        var arquivo = zip.CreateEntry(fsOrigem.Name);
                        using (var sr = arquivo.Open())
                        {
                            var buffer = new byte[ushort.MaxValue];
                            while (true)
                            {
                                var lidos = fsOrigem.Read(buffer, 0, buffer.Length);
                                if (lidos == 0)
                                {
                                    break;
                                }
                                sr.Write(buffer, 0, lidos);
                            }
                            sr.Flush();
                        }
                    }
                    msZip.Seek(0, SeekOrigin.Begin);

                    return msZip;
                }
            }
        }

        public static string DescompactarTexto(Stream stream, string senha = null)
        {
            if (stream != null)
            {
                stream.Seek(0, SeekOrigin.Begin);
                using (var ms = new MemoryStream())
                {
                    stream.CopyTo(ms);

                    ms.Seek(0, SeekOrigin.Begin);

                    using (var zip = new ZipArchive(stream, ZipArchiveMode.Read))
                    {
                        if (!String.IsNullOrEmpty(senha))

                            using (var arquivo = zip.Entries.First().Open())
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
                        DescompactarArquivo(fsOrigemBuferizada, fsDestino);
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

                    using (var zip = new ZipArchive(ms, ZipArchiveMode.Read))
                    {
                        if (zip.Entries.Count != 1)
                        {
                            throw new Erro(String.Format("O numero de arquivo dentro do zip não é suportado {0}", zip.Entries.Count));
                        }
                        using (var arquivo = zip.Entries.First().Open())
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
            using (var zip = new ZipArchive(streamOrigem, ZipArchiveMode.Read))
            {
                if (zip.Entries.Count != 1)
                {
                    throw new Erro(String.Format("O numero de arquivo dentro do zip não é suportado {0}", zip.Entries.Count));
                }
                using (var streamArquivoZipado = zip.Entries.First().Open())
                {
                    using (var streamZipBufferizada = new BufferedStream(streamArquivoZipado, TAMANHO_BUFFEER))
                    {
                        StreamUtil.SalvarStreamBufferizada(streamZipBufferizada, streamDestino);
                    }
                }
            }
        }

        public static List<string> Extrair(string caminhoArquivo, 
                                           string diretorioDestino,
                                           bool isSobreEscrever)
        {
            using (var fs = new FileStream(caminhoArquivo, FileMode.Open))
            {
                return DescompactarArquivos(fs, diretorioDestino, isSobreEscrever);
            }
        }
        public static List<string> Descompactar(string caminhoArquivo, 
                                                string diretorioDestino, 
                                                bool isSobreEscrever = true)
        {
            using (var fs = new FileStream(caminhoArquivo, FileMode.Open))
            {
                return DescompactarArquivos(fs, diretorioDestino, isSobreEscrever);
            }
        }

        public static List<string> Descompactar(Stream stream, string diretorioDestino)
        {
            return DescompactarArquivos(stream, diretorioDestino, null);
        }

        public static List<string> DescompactarArquivos(Stream stream, 
                                                       string diretorioDestino,
                                                        bool isSobreEscrever = true)
        {
            return DescompactarArquivos(stream, diretorioDestino, null, isSobreEscrever);
        }

        public static List<string> DescompactarArquivos(Stream stream, 
                                                        string diretorioDestino, 
                                                        string senha,
                                                        bool isSobreEscrever = true)
        {
            var arquivos = new List<string>();
            if (stream != null)
            {
                using (var ms = StreamUtil.RetornarMemoryStream(stream))
                {
                    using (var zip = new ZipArchive(ms, ZipArchiveMode.Read))
                    {
                        foreach (var entry in zip.Entries)
                        {
                            //if (entry.IsDirectory)
                            //{
                            //    //throw new NotImplementedException();
                            //    continue;
                            //}
                            using (var zipStream = entry.Open())
                            {
                                using (var msArquivo = StreamUtil.RetornarMemoryStream(zipStream))
                                {
                                    var arquivoDestino = new FileInfo(CaminhoUtil.Combine(diretorioDestino, entry.Name));
                                    if (arquivoDestino.Exists)
                                    {
                                        if (!isSobreEscrever)
                                        {
                                            continue;
                                        }
                                        ArquivoUtil.DeletarArquivo(arquivoDestino);
                                    }
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

        public static void CompactarPasta(string diretorioOrigem,
                                          string caminhoDestino,
                                          bool isDeletarArquivo)
        {
            var arquivosParaDeletar = new List<string>();

            var di = new DirectoryInfo(diretorioOrigem);
            using (var fs = StreamUtil.CreateWrite(caminhoDestino))
            using (var zip = new ZipArchive(fs, ZipArchiveMode.Create, true))
            {
                //var arquivos = di.GetFiles();
                var arquivos = Directory.EnumerateFiles(diretorioOrigem, "*", SearchOption.AllDirectories);

                foreach (var arquivo in arquivos.Select(path=> new FileInfo(path)))
                {
                    if (arquivo.Attributes == (arquivo.Attributes & FileAttributes.Hidden) ||
                        caminhoDestino.Equals(arquivo.FullName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        continue;
                    }

                    var relativePath = CaminhoUtil.RetornarCaminhoRelativo(arquivo.FullName, diretorioOrigem); 
                     
                    var entry = zip.CreateEntry(relativePath);
                    using (var sr = entry.Open())
                    {
                        using (var fsArquivo = StreamUtil.OpenRead(arquivo.FullName))
                        {
                            fsArquivo.CopyTo(sr);
                        }
                    }

                    if (isDeletarArquivo)
                    {
                        arquivosParaDeletar.Add(arquivo.FullName);
                    }
                }
            }

            if (isDeletarArquivo)
            {
                arquivosParaDeletar.ForEach(arquivo => ArquivoUtil.DeletarArquivo(arquivo, true));
            }
        }
    }
}