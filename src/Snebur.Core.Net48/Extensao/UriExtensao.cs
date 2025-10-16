using Snebur.Utilidade;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace System
{
    public static class UriExtensao
    {
        public static string GetQuery(this Uri uri)
        {
            if (uri.IsAbsoluteUri)
            {
                return uri.Query;
            }
            var url = uri.ToString();
            if (url.Contains("?"))
            {
                return url.Substring(url.IndexOf("?") + 1);
            }
            return String.Empty;
        }

        public static string GetPath(this Uri uri, bool isRemoveFileName)
        {
            var path = uri.GetPath();
            if (String.IsNullOrWhiteSpace(path))
            {
                return path;
            }

            if (isRemoveFileName)
            {
                var fileName = IO.Path.GetFileName(path);
                if (fileName.Contains("."))
                {
                    return IO.Path.GetDirectoryName(path);
                }
            }
            return path;

        }
        public static string GetPath(this Uri uri)
        {
            if (uri.IsAbsoluteUri)
            {
                return uri.AbsolutePath;
            }

            var url = uri.ToString();
            if (url.Contains("?"))
            {
                return url.Substring(0, url.IndexOf("?"));
            }
            if (url.Contains("#"))
            {
                return url.Substring(0, url.IndexOf("#"));
            }
            return uri.ToString();
        }

        public static Uri UriSchemeHttps(this Uri uri)
        {
            if (uri.Scheme == Uri.UriSchemeHttps)
            {
                return uri;
            }

            return (new UriBuilder(uri)
            {
                Scheme = Uri.UriSchemeHttps,
                Port = -1 // default port for scheme
            }).Uri;
        }
        public static Uri UriSchemeHttp(this Uri uri)
        {
            if (uri.Scheme == Uri.UriSchemeHttp)
            {
                return uri;
            }
            return (new UriBuilder(uri)
            {
                Scheme = Uri.UriSchemeHttp,
                Port = -1 // default port for scheme
            }).Uri;
        }

        public static Uri NormalizarWWW(this Uri uri)
        {
            var host = uri.Host.ToLower();
            if (!host.StartsWith("www."))
            {
                return (new UriBuilder(uri)
                {
                    Host = "www." + uri.Host,
                }).Uri;
            }
            return uri;
        }
        public static Uri RemoverWWW(this Uri uri)
        {
            var host = uri.Host.ToLower();
            if (host.StartsWith("www."))
            {
                return (new UriBuilder(uri)
                {
                    Host = uri.Host.Substring(4),
                }).Uri;
            }
            return uri;
        }

        public static Task<bool> IsSslValidoAsync(this Uri uri)
        {
            var uriHttps = uri.UriSchemeHttps();
            var promise = new TaskCompletionSource<bool>();

            TaskUtil.Run(() =>
            {
                try
                {
#pragma warning disable SYSLIB0014
                    var request = (HttpWebRequest)WebRequest.Create(uriHttps);
#pragma warning restore SYSLIB0014
                    request.Timeout = (int)TimeSpan.FromSeconds(15).TotalMilliseconds;

#if NET40 == false
                    request.ServerCertificateValidationCallback += (object sender,
                                                                    X509Certificate certificate,
                                                                    X509Chain chain,
                                                                    SslPolicyErrors sslPolicyErrors) =>
                    {
                        if (sslPolicyErrors == SslPolicyErrors.None)
                        {
                            promise.TrySetResult(true);
                            return true;
                        }
                        else
                        {
                            promise.TrySetResult(false);
                            return false;
                        }
                    };
#endif

                    request.GetResponse();
                    promise.TrySetResult(true);
                }
                catch
                {
                    promise.TrySetResult(false);
                }

            });
            return promise.Task;
        }

        public static bool IsWWW(this Uri uri)
        {
            return uri.Host.ToLower().StartsWith("www");
        }
    }
}
