using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;

using System.Text;
using System.Threading;

namespace Snebur.Utilidade
{

    public static class HttpUtil
    {
        public const int TIMEOUT_PADRAO = 15;
        private const int TAMANHO_BUFFER_PADRAO = 8 * 1024;

#if NET6_0_OR_GREATER

     
#endif

        public static string RetornarString(string url)
        {
            return RetornarString(url, TimeSpan.FromSeconds(TIMEOUT_PADRAO), false);
        }
        public static string RetornarString(Uri uri, bool ignorarErro)
        {
            return RetornarString(uri, TimeSpan.FromSeconds(TIMEOUT_PADRAO), ignorarErro);
        }

        public static string RetornarString(string url, bool ignorarErro)
        {
            return RetornarString(url, TimeSpan.FromSeconds(TIMEOUT_PADRAO), ignorarErro);
        }

        public static string RetornarString(Uri uri, TimeSpan timeout, bool ignorarErro)
        {
            return RetornarString(uri, null, timeout, ignorarErro);
        }
        public static string RetornarString(string url, TimeSpan timeout, bool ignorarErro)
        {
            return RetornarString(url, null, timeout, ignorarErro);
        }

        public static string RetornarString(Uri uri, Dictionary<string, string> cabecalho, TimeSpan timeout, bool ignorarErro)
        {
            var resultado = RetornarStringCabecalhoResposta(uri, cabecalho, timeout, ignorarErro);
            return resultado.Item1;
        }
        public static string RetornarString(string url, Dictionary<string, string> cabecalho, TimeSpan timeout, bool ignorarErro)
        {
            var resultado = RetornarStringCabecalhoResposta(url, cabecalho, timeout, ignorarErro);
            return resultado.Item1;

        }

        public static Tuple<string, WebHeaderCollection> RetornarStringCabecalhoResposta(string url,
                                                                                         Dictionary<string, string> cabecalho,
                                                                                         TimeSpan timeout,
                                                                                         bool ignorarErro)
        {
            return RetornarStringCabecalhoResposta(new Uri(url), cabecalho, timeout, ignorarErro);
        }
        public static Tuple<string, WebHeaderCollection> RetornarStringCabecalhoResposta(Uri url, Dictionary<string, string> cabecalho, TimeSpan timeout, bool ignorarErro)
        {
            var resultado = RetornarBytesCabecalhoResposta(url, cabecalho, timeout, ignorarErro);
            var resultadoString = (resultado?.Item1 == null) ? String.Empty : Encoding.UTF8.GetString(resultado.Item1);
            var cabecalhoResposta = resultado?.Item2 ?? new WebHeaderCollection();
            return new Tuple<string, WebHeaderCollection>(resultadoString, cabecalhoResposta);
        }

        public static byte[] RetornarBytes(string url)
        {
            return RetornarBytes(url, null, TimeSpan.FromSeconds(TIMEOUT_PADRAO), false);
        }

        public static byte[] RetornarBytes(string url, bool ignorarErro)
        {
            return RetornarBytes(url, null, TimeSpan.FromSeconds(TIMEOUT_PADRAO), ignorarErro);
        }

        public static byte[] RetornarBytes(string url, Dictionary<string, string> cabecalhos)
        {
            return RetornarBytes(url, cabecalhos, TimeSpan.FromSeconds(TIMEOUT_PADRAO), false);
        }

        public static byte[] RetornarBytes(string url, Dictionary<string, string> cabecalhos, bool ignorarErro)
        {
            return RetornarBytes(url, cabecalhos, TimeSpan.FromSeconds(TIMEOUT_PADRAO), ignorarErro);
        }

        public static byte[] RetornarBytes(string url, Dictionary<string, string> cabecalho, TimeSpan timeout, bool ignorarErro)
        {
            var bytesCabecalho = RetornarBytesCabecalhoResposta(url, cabecalho, timeout, ignorarErro);
            return bytesCabecalho.Item1;
        }

        public static Tuple<byte[], WebHeaderCollection> RetornarBytesCabecalhoResposta(string url, Dictionary<string, string> cabecalho)
        {
            return RetornarBytesCabecalhoResposta(url, cabecalho, TimeSpan.FromSeconds(TIMEOUT_PADRAO), false);
        }

        public static Tuple<byte[], WebHeaderCollection> RetornarBytesCabecalhoResposta(string url, Dictionary<string, string> cabecalho, TimeSpan timeout, bool ignorarErro)
        {
            return RetornarBytesCabecalhoResposta(new Uri(url), cabecalho, timeout, ignorarErro);
        }

        public static Tuple<byte[], WebHeaderCollection> RetornarBytesCabecalhoResposta(Uri uri,
                                                                                        Dictionary<string, string> cabecalho,
                                                                                        TimeSpan timeout,
                                                                                        bool ignorarErro)
        {
            return RetornarBytesCabecalhoResposta(uri, cabecalho, timeout, ignorarErro, 0);
        }
        private static Tuple<byte[], WebHeaderCollection> RetornarBytesCabecalhoResposta(Uri uri,
                                                                                         Dictionary<string, string> cabecalho,
                                                                                         TimeSpan timeout,
                                                                                         bool ignorarErro,
                                                                                         int tentativa)
        {
            try
            {
#pragma warning disable SYSLIB0014
                var requisicao = (HttpWebRequest)WebRequest.Create(uri);
#pragma warning restore SYSLIB0014
                if (cabecalho != null)
                {
                    foreach (var item in cabecalho)
                    {
                        requisicao.Headers.Add(item.Key, item.Value);
                    }
                }
                //requisicao.Method = metodo;
                requisicao.Timeout = RetornarTimeout(timeout);
                requisicao.Proxy = null;
                requisicao.ContentType = "text/json";

                using (var resposta = (HttpWebResponse)requisicao.GetResponse())
                {
                    using (var streamResposta = resposta.GetResponseStream())
                    {
                        using (var ms = StreamUtil.RetornarMemoryStreamBuferizada(streamResposta, TAMANHO_BUFFER_PADRAO))
                        {
                            return new Tuple<byte[], WebHeaderCollection>(ms.ToArray(), resposta.Headers);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (tentativa > 5)
                {
                    LogUtil.ErroAsync(ex);
                    if (!ignorarErro)
                    {
                        throw;
                    }
                    return null;
                }

                Thread.Sleep(tentativa * 1000);

                return RetornarBytesCabecalhoResposta(
                    uri,
                    cabecalho,
                    timeout,
                    ignorarErro,
                    tentativa += 1);

            }
        }

        public static MemoryStream RetornarMemoryStream(string url)
        {
            return RetornarMemoryStream(url, null, TimeSpan.FromSeconds(30), false);
        }
        public static MemoryStream RetornarMemoryStream(Uri url)
        {
            return RetornarMemoryStream(url, null, TimeSpan.FromSeconds(30), false);
        }

        public static MemoryStream RetornarMemoryStream(string url,
                                                       Dictionary<string, string> cabecalho, TimeSpan timeout, bool ignorarErro)
        {
            return RetornarMemoryStream(new Uri(url), cabecalho, timeout, ignorarErro);
        }
        public static MemoryStream RetornarMemoryStream(Uri uri, Dictionary<string, string> cabecalho, TimeSpan timeout, bool ignorarErro)
        {
            var resultado = RetornarMemoryStreamCabecalhoResposta(uri, cabecalho, timeout, ignorarErro);
            return resultado.Item1;
        }

        public static Tuple<MemoryStream, WebHeaderCollection> RetornarMemoryStreamCabecalhoResposta(string url, Dictionary<string, string> cabecalho, TimeSpan timeout, bool ignorarErro)
        {
            return RetornarMemoryStreamCabecalhoResposta(new Uri(url), cabecalho, timeout, ignorarErro);
        }
        public static Tuple<MemoryStream, WebHeaderCollection> RetornarMemoryStreamCabecalhoResposta(Uri uri, Dictionary<string, string> cabecalho, TimeSpan timeout, bool ignorarErro)
        {
            try
            {
#pragma warning disable SYSLIB0014
                var requisicao = (HttpWebRequest)WebRequest.Create(uri);
#pragma warning restore SYSLIB0014
                if (cabecalho != null)
                {
                    foreach (var item in cabecalho)
                    {
                        requisicao.Headers.Add(item.Key, item.Value);
                    }
                }
                requisicao.Timeout = RetornarTimeout(timeout);
                requisicao.Proxy = null;

                using (var resposta = (HttpWebResponse)requisicao.GetResponse())
                {
                    using (var streamResposta = resposta.GetResponseStream())
                    {
                        var ms = StreamUtil.RetornarMemoryStreamBuferizada(streamResposta, TAMANHO_BUFFER_PADRAO);

                        return new Tuple<MemoryStream, WebHeaderCollection>(ms, resposta.Headers);
                    }
                }
            }
            catch (Exception)
            {
                if (!ignorarErro)
                {
                    throw;
                }
            }
            return null;
        }

        private static int RetornarTimeout(TimeSpan timeout)
        {
            if (DebugUtil.IsAttached)
            {
                return (int)TimeSpan.FromHours(1).TotalMilliseconds;
            }
            return (int)timeout.TotalMilliseconds;
        }
    }

    public static class NameValueCollectionExtensao
    {
        public static Dictionary<string, string> ToDictionary(this NameValueCollection origem)
        {
            var retorno = new Dictionary<string, string>();
            foreach (var chave in origem.AllKeys.Distinct())
            {
                if (chave == null)
                {
                    throw new Exception("A chave não pode ser null");
                }

                if (retorno.ContainsKey(chave))
                {
                    throw new Exception("A chave já existe no dicionário");
                }

                var valores = origem.GetValues(chave);
                var valor = String.Join(", ", valores);
                retorno.Add(chave, valor);
            }
            return retorno;
        }

        public static NameValueCollection AddRange(this NameValueCollection origem, NameValueCollection items)
        {
            foreach (var chave in items.AllKeys)
            {
                if (!origem.AllKeys.Contains(chave))
                {
                    var valor = items[chave];
                    origem.Add(chave, valor);
                }
            }
            return origem;
        }
    }
}