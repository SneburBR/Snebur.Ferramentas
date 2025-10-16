using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace Snebur.Utilidade
{
    public static class UriUtil
    {
        public static Uri RetornarUriParent(Uri uri)
        {
            char[] divisor = @"/".ToCharArray();

            string[] seguementos = uri.AbsolutePath.Split(divisor);
            List<string> paths = new List<string>();
            for (int i = 0; i <= seguementos.Length - 2; i++)
            {
                paths.Add(seguementos[i]);
            }
            string path = String.Join(@"/", paths);
            UriBuilder uriBuilder = new UriBuilder(uri.Scheme, uri.Host, uri.Port, path);
            return uriBuilder.Uri;
        }

        public static string CombinarCaminhos(string endereco1, string endereco2)
        {
            return AjustarBarraFinal(endereco1) + RemoverBarraInicial(endereco2);
        }

        public static string AjustarBarraInicialFinal(string endereco)
        {
            return AjustarBarraInicial(AjustarBarraFinal(endereco));
        }
        public static string AjustarBarraInicial(string endereco)
        {
            endereco = InverterBarras(endereco);
            return "/" + RemoverBarraInicial(endereco);
        }

        public static string RetornarURL(string urlVisualizar, Dictionary<string, string> parametros)
        {
            var query = String.Join("&", parametros.Select(x => $"{Uri.EscapeDataString((Base64Util.Encode(x.Key)))}={Uri.EscapeDataString(Base64Util.Encode(x.Value))}"));
            return AjustarInterrogacaoFunal(urlVisualizar) + query;
        }

        private static string AjustarInterrogacaoFunal(string urlVisualizar)
        {
            if (!urlVisualizar.EndsWith("?"))
            {
                return urlVisualizar + "?";
            }
            return urlVisualizar;
        }

        public static string AjustarBarraFinal(string endereco)
        {
            endereco = InverterBarras(endereco);
            return RemoverBarraFinal(endereco) + "/";
        }

        public static string RemoverBarrasFinalInicial(string endereco)
        {
            return RemoverBarraInicial(RemoverBarraFinal(endereco));
        }

        public static string RemoverBarraInicial(string endereco)
        {
            endereco = InverterBarras(endereco);
            while (endereco.StartsWith("/"))
            {
                endereco = endereco.Remove(0, 1);
            }
            return endereco;
        }

        public static string RemoverBarraFinal(string endereco)
        {
            endereco = InverterBarras(endereco);
            while (endereco.EndsWith("/"))
            {
                endereco = endereco.Remove(endereco.Length - 1, 1);
            }
            return endereco;
        }

        private static string InverterBarras(string endereco)
        {
            return endereco.Replace(@"\", "/");
        }

        public static Dictionary<string, string> RetornarPartesConsulta(Uri uri)
        {
            return RetornarPartesConsulta(uri.Query);
        }

        public static Dictionary<string, string> RetornarPartesConsulta(string query)
        {
            var retorno = new Dictionary<string, string>();

            if (query.Trim().Length > 0)
            {
                if (query.StartsWith("?"))
                {
                    query = query.Substring(1);
                }
                string[] partes = query.Split("&".ToCharArray());
                foreach (string parte in partes)
                {
                    if (parte.Contains("="))
                    {
                        string[] valores = parte.Split("=".ToCharArray());
                        string chave = valores.First();
                        string valor = parte.Replace(String.Format("{0}=", chave), String.Empty);

                        if (retorno.ContainsKey(chave))
                        {
                            retorno[chave] += "," + Uri.UnescapeDataString(valor);
                        }
                        else
                        {
                            retorno.Add(chave, Uri.UnescapeDataString(valor));
                        }
                    }
                }
            }
            return retorno;
        }

        public static string ConstruirQuery(NameValueCollection parametros)
        {
            if (parametros == null || parametros.Count == 0)
            {
                return String.Empty;
            }

            foreach (var chaveNull in parametros.AllKeys.Where(x => x == null))
            {
                parametros.Remove(chaveNull);
            }

            var d = parametros.ToDictionary();
            return ConstruirQuery(d);
        }

        public static string ConstruirQuery(IDictionary<string, string> di)
        {
            string retornarValor(string valor)
            {
                return (valor != null) ? Uri.EscapeDataString(valor) : String.Empty;
            }
            return String.Join("&", di.Select(x => $"{x.Key}={retornarValor(x.Value)}"));
        }

        public static string RetornarValorQuery(string query, string chave)
        {
            var partes = RetornarPartesConsulta(query);
            if (partes.ContainsKey(chave))
            {
                return partes[chave];
            }
            return null;
        }

        public static string RetornarHost(string url)
        {
            if(Uri.TryCreate(url,UriKind.Absolute, out var uri))
            {
                return uri.Host;
            }
            return null;
        }
    }
}