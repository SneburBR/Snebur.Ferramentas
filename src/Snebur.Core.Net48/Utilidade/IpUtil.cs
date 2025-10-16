using Snebur.Dominio;
using Snebur.Utilidade;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace Snebur
{
    public static class IpUtil
    {
        private static string _ipPublico;
        /// <summary>
        /// retornar 0.0.0.0
        /// </summary>
        public static string Empty
        {
            get
            {
                return ConstantesIP.IP_VAZIO;
            }
        }

        private static DadosIPInformacao _ipDadosInformacao;

        public static DadosIPInformacao IPInformacao =>
            LazyUtil.RetornarValorLazyComBloqueio(ref _ipDadosInformacao, RetornarIPInformacaoInterno);

        private static DadosIPInformacao RetornarIPInformacaoInterno()
        {
            return RetornarIPInformacao(String.Empty);
        }

        public static string RetornarIpPublico()
        {
            if (_ipPublico == null)
            {
                var ipString = HttpUtil.RetornarString("https://api.ipify.org", null, TimeSpan.FromSeconds(5), true);
                if (ValidacaoUtil.IsIp(ipString))
                {
                    _ipPublico = ipString;
                }
                else
                {
                    _ipPublico = RetornarIPInformacao(String.Empty).IP;
                }
            }
            return _ipPublico;

        }

        public static DadosIPInformacao RetornarIPInformacao(string ip)
        {
            if(String.IsNullOrEmpty(ip))
            {
                ip = RetornarIpPublico();
            }   

            var url = (IsIpVazioOuLocal(ip)) ? "http://ipinfo.io/json" :
                                                       String.Format("http://ipinfo.io/{0}/json", ip);

            //var cabecalho = RetornarCabecalhoUrlIpInfo();
            var json = HttpUtil.RetornarString(url, null, TimeSpan.FromSeconds(5), true);
            if (!String.IsNullOrWhiteSpace(json))
            {
                var ipInfo = JsonUtil.Deserializar<IpInfo>(json, EnumTipoSerializacao.Javascript);
                var localizacao = Localizacao.Parse(ipInfo.loc);
                var mascaraIp = RetornarMascaraIp4(ipInfo.ip);

                return new DadosIPInformacao
                {
                    IP = ipInfo.ip,
                    Cidade = ipInfo.city,
                    CodigoPostal = ipInfo.postal,
                    Hostname = ipInfo.hostname,
                    Localizacao = localizacao,
                    Regiao = ipInfo.region,
                    Pais = ipInfo.country,
                    ProvedorInternet = ipInfo.org,
                    MascaraIp4 = RetornarMascaraIp4(ipInfo.ip)
                };
            }
            return DadosIPInformacao.Vazio;
        }

        //private static string RetornarUrlIpInfo()
        //{
        //    if (AplicacaoSnebur.Atual.TipoAplicacao == EnumTipoAplicacao.DotNet_WebService)
        //    {
        //        return "https://ipinfo.snebur.com.br/";
        //    }
        //    return "http://ipinfo.io/json";
        //}

        //private static Dictionary<string, string> RetornarCabecalhoUrlIpInfo()
        //{
        //    if (AplicacaoSnebur.Atual.TipoAplicacao == EnumTipoAplicacao.DotNet_WebService)
        //    {
        //        var d = new Dictionary<string, string>();
        //        throw new NotImplementedException();
        //    }
        //    return null;
        //}

        public static string RetornarMascaraIp4(string ip)
        {
            IPAddress ipaddress;
            if (IPAddress.TryParse(ip, out ipaddress))
            {
                if (ipaddress.AddressFamily == AddressFamily.InterNetwork)
                {
                    var partes = ipaddress.ToString().Split('.');
                    if (byte.TryParse(partes[0], out byte parte1) &&
                        byte.TryParse(partes[1], out byte parte2) &&
                        byte.TryParse(partes[2], out byte parte3))
                    {
                        var mascara = String.Format("{0}.{1}.{2}", parte1.ToString(), parte2.ToString(), parte3.ToString());
                        return mascara;
                    }
                }
            }
            return null;
        }

        public static bool IsIP(string ip)
        {
            return ValidacaoUtil.IsIp(ip);
        }

        public static bool IsIpVazioOuLocal(string ip)
        {
            if (String.IsNullOrEmpty(ip))
            {
                return true;
            }
            if (ip == ConstantesIP.IP_LOCAL || ip == ConstantesIP.IP6_LOCAL || ip == ConstantesIP.IP_VAZIO)
            {
                return true;
            }
            return false;
        }

        private static Localizacao RetornarLocalizacao(string localicacaoString)
        {
            var partes = localicacaoString.Split(',');
            if (partes.Length != 2)
            {
                return Localizacao.Empty;
            }
            var latituraString = partes.First();
            var longitudeString = partes.Last();

            if (Double.TryParse(latituraString, out double latitude) &&
                Double.TryParse(longitudeString, out double longitude))
            {
                return new Localizacao(latitude, longitude);
            }
            return new Localizacao(0, 0);
        }

        public static string RetornarIpLocal(bool isIgnorarErro = false)
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            if (isIgnorarErro)
            {
                return null;
            }
            throw new Exception("O rede local não existe ou está desativada  ");
        }

    }
}

namespace Snebur.Dominio
{
    public class DadosIPInformacao : BaseDominio, IIPInformacao
    {
        public string IP { get; set; }

        public string MascaraIp4 { get; set; }

        public string Cidade { get; set; }

        public string CodigoPostal { get; set; }

        public string Hostname { get; set; }

        public Localizacao Localizacao { get; set; }

        public string Pais { get; set; }

        public string ProvedorInternet { get; set; }

        public string Regiao { get; set; }

        public static DadosIPInformacao Vazio
        {
            get
            {
                return new DadosIPInformacao
                {
                    IP = IpUtil.Empty
                };
            }
        }

        internal static DadosIPInformacao SneburChapeco
        {
            get
            {
                var ipLocal = "177.128.0.19";
                return new DadosIPInformacao
                {
                    IP = ipLocal,
                    Hostname = "CHA-SNEBUR",
                    Regiao = "Santa Catarina",
                    Pais = "BR",
                    Cidade = "Chapecó",
                    Localizacao = new Localizacao(-27.1000, -52.6000),
                    ProvedorInternet = "AS262361 Deznet Telecom Ltda.",
                    MascaraIp4 = IpUtil.RetornarMascaraIp4(ipLocal)
                };
            }
        }
    }

    public class IpInfo
    {
        public string ip { get; set; }
        public string hostname { get; set; }
        public string city { get; set; }
        public string region { get; set; }
        public string country { get; set; }
        public string loc { get; set; }
        public string org { get; set; }
        public string postal { get; set; }
    }
}