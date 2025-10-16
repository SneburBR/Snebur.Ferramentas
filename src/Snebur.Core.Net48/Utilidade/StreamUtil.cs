using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Snebur.Utilidade
{
    public static class StreamUtil
    {
        //private const int TAMANHO_BUFFER = 5;
        private const int TAMANHO_BUFFER_MAXIMO = 1024 * 1024;
        private const int TAMANHO_BUFFER_PADRAO = 256 * 1024;
        private const int TAMANHO_BUFFER_MINIMO = 32 * 1024;

        /// <summary>
        /// Ler as stream, buffer a buffer e retornar um MemoryStream, util,
        /// Quando usar, em stream conecta remotamente.
        /// Ex. request.GetRequestStream ou response.GetReponseStream
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static MemoryStream RetornarMemoryStream(Stream stream,
                                                        Action<StreamProgressEventArgs> callbackProgresso = null)
        {
            return RetornarMemoryStreamBuferizada(stream,
                                                             TAMANHO_BUFFER_PADRAO,
                                                             callbackProgresso);
        }
        public static MemoryStream RetornarMemoryStreamBuferizada(Stream stream,
                                                                  Action<StreamProgressEventArgs> callbackProgresso = null,
                                                                  long contentLenght = 0)
        {
            return RetornarMemoryStreamBuferizada(stream,
                                                             TAMANHO_BUFFER_PADRAO,
                                                             callbackProgresso,
                                                             contentLenght);
        }

        public static MemoryStream RetornarMemoryStreamBuferizada(Stream streamOrigem,
                                                                  int tamanhoBuffer,
                                                                  Action<StreamProgressEventArgs> callbackProgresso = null,
                                                                  long contentLenght = 0)
        {
            if (streamOrigem.CanSeek)
            {
                streamOrigem.Seek(0, SeekOrigin.Begin);
            }
            var ms = new MemoryStream();
            SalvarStreamBufferizada(streamOrigem, ms, tamanhoBuffer, callbackProgresso, contentLenght);
            return ms;
        }
        public static void SalvarStreamBufferizada(Stream streamOrigem, Stream streamDestino)
        {
            SalvarStreamBufferizada(streamOrigem, streamDestino, TAMANHO_BUFFER_PADRAO);
        }

        public static void SalvarStreamBufferizada(Stream streamOrigem,
                                                   Stream streamDestino,
                                                   int tamanhoBuffer,
                                                   Action<StreamProgressEventArgs> callbackProgresso = null,
                                                   long contentLenght = 0)
        {
            var totalBytesRecebitos = 0;
            var totalBytes = streamOrigem.CanSeek ? streamOrigem.Length : contentLenght;

            if (streamOrigem.CanSeek)
            {
                streamOrigem.Seek(0, SeekOrigin.Begin);
            }

            tamanhoBuffer = Math.Min(Math.Max(TAMANHO_BUFFER_MINIMO, tamanhoBuffer), TAMANHO_BUFFER_MAXIMO);

            byte[] buffer = new byte[tamanhoBuffer];
            int bytesRecebido;
            while ((bytesRecebido = streamOrigem.Read(buffer, 0, buffer.Length)) != 0)
            {
                streamDestino.Write(buffer, 0, bytesRecebido);
                totalBytesRecebitos += bytesRecebido;

                if (callbackProgresso != null)
                {
                    callbackProgresso(new StreamProgressEventArgs(totalBytesRecebitos, totalBytes));
                }
                //streamDestino.Flush();
            }

            if (streamDestino.CanSeek)
            {
                streamDestino.Seek(0, SeekOrigin.Begin);
            }
        }
 

        public static async Task SalvarStreamBufferizadaAsync(Stream streamOrigem,
                                                              Stream streamDestino,
                                                              int tamanhoBuffer = TAMANHO_BUFFER_PADRAO)
        {

            if (streamOrigem.CanSeek)
            {
                streamOrigem.Seek(0, SeekOrigin.Begin);
            }
            while (true)
            {
                var buffer = new byte[tamanhoBuffer];
                var bytesRecebido = streamOrigem.Read(buffer, 0, tamanhoBuffer);
#if NET40
                streamDestino.Write(buffer, 0, bytesRecebido);
                await TaskUtil.Delay(0);
#else
                await streamDestino.WriteAsync(buffer, 0, bytesRecebido);
#endif
                if (bytesRecebido == 0)
                {
                    break;
                }
#if NET40
                streamDestino.Flush();
#else
                await streamDestino.FlushAsync();
#endif

            }
            if (streamDestino.CanSeek)
            {
                streamDestino.Seek(0, SeekOrigin.Begin);
            }
        }

        public static void SalvarStream(string caminhoArquivo, Stream stream)
        {
            if (stream is MemoryStream)
            {
                SalvarMemoryStream(caminhoArquivo, (MemoryStream)stream);
            }
            else
            {
                using (var ms = RetornarMemoryStream(stream))
                {
                    SalvarMemoryStream(caminhoArquivo, ms);
                }
            }
        }

        private static void SalvarMemoryStream(string caminhoArquivo, MemoryStream stream)
        {
            File.WriteAllBytes(caminhoArquivo, stream.ToArray());
        }

        public static FileStream OpenRead(string caminhoArquivo, int bufferSize = TAMANHO_BUFFER_PADRAO)
        {
            if (!File.Exists(caminhoArquivo))
            {
                throw new ErroArquivoNaoEncontrado(caminhoArquivo);
            }
            return new FileStream(caminhoArquivo,
                                  FileMode.Open,
                                  FileAccess.Read,
                                  FileShare.Read,
                                  bufferSize);
        }

        public static FileStream CreateWrite(string caminhoArquivo,
                                             int bufferSize = TAMANHO_BUFFER_PADRAO)
        {
            if (File.Exists(caminhoArquivo))
            {
                ArquivoUtil.DeletarArquivo(caminhoArquivo, false, true);
            }
            DiretorioUtil.CriarDiretorio(Path.GetDirectoryName(caminhoArquivo));
            return new FileStream(caminhoArquivo, FileMode.Create, FileAccess.Write, FileShare.Write, bufferSize);
        }
        public static FileStream CreateOrOpenWrite(string caminhoArquivo, bool isForcar = false)
        {
            DiretorioUtil.CriarDiretorio(Path.GetDirectoryName(caminhoArquivo));
            var tentativa = 0;
            while (true)
            {
                try
                {

                    return new FileStream(caminhoArquivo, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write);
                }
                catch (IOException)
                {
                    tentativa += 1;
                    if (!isForcar || (tentativa > 10))
                    {
                        throw;
                    }
                    Thread.Sleep(500);
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        public static byte[] RetornarTodosBytes(string caminhoParte)
        {
            using (var fs = OpenRead(caminhoParte))
            {
                using (var ms = RetornarMemoryStreamBuferizada(fs))
                {
                    return ms.ToArray();
                }
            }
        }

        public static FileStream OpenWrite(string caminhoArquivo)
        {
            if (File.Exists(caminhoArquivo))
            {
                ArquivoUtil.DeletarArquivo(caminhoArquivo, false, true);
            }
            return new FileStream(caminhoArquivo, FileMode.Create, FileAccess.Write, FileShare.Write);
        }
        public static void SetarPonteiroInicio(Stream stream)
        {
            if (stream.CanSeek)
            {
                stream.Seek(0, SeekOrigin.Begin);
            }
        }

        public static void CopiarArquivo(string caminhoOrigem,
                                         string caminhoDestinio)
        {
            CopiarArquivo(caminhoOrigem, caminhoDestinio, TAMANHO_BUFFER_PADRAO);
        }

        public static void CopiarArquivo(string caminhoOrigem,
                                         string caminhoDestinio,
                                         Action<StreamProgressEventArgs> callbackProgresso = null)
        {
            CopiarArquivo(caminhoOrigem,
                                     caminhoDestinio,
                                     TAMANHO_BUFFER_PADRAO,
                                     callbackProgresso);

        }
        public static void CopiarArquivo(string caminhoOrigem,
                                         string caminhoDestinio,
                                         int tamanhoBuffer = TAMANHO_BUFFER_PADRAO,
                                         Action<StreamProgressEventArgs> callbackProgresso = null)
        {
            if (!File.Exists(caminhoOrigem))
            {
                throw new FileNotFoundException(
                    message: caminhoOrigem,
                    fileName: caminhoOrigem);
            }

            using (var streamOrigem = OpenRead(caminhoOrigem, tamanhoBuffer))
            using (var streamDestino = CreateWrite(caminhoDestinio, tamanhoBuffer))
            {
                SalvarStreamBufferizada(streamOrigem, streamDestino,
                                                  tamanhoBuffer,
                                                  callbackProgresso);
            }
        }
    }
}