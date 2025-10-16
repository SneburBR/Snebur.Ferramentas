using System;
using System.Net.NetworkInformation;

namespace Snebur.Utilidade
{
    public static class RedeUtil
    {
        //10 megabits
        public const int MINIMUM_SPEED = 10000000;

        public static bool RedeConectada()
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                return false;
            }
            return true;
        }

        public static bool InternetConectada()
        {
            //if (DebugUtil.IsAttached)
            //{
            //    return true;
            //} 

            if (!RedeConectada())
            {
                return false;
            }

            if (!String.IsNullOrEmpty(AplicacaoSnebur.Atual.UrlPingInternetConectada))
            {
                var urlPing = String.Format("{0}?State={1}", AplicacaoSnebur.Atual.UrlPingInternetConectada, Guid.NewGuid().ToString());
                var resultadoPing = HttpUtil.RetornarString(urlPing, TimeSpan.FromSeconds(2), true);
                if (Boolean.TryParse(resultadoPing, out bool ping) && ping)
                {
                    return ping;
                }
            }
            var pingGoogle = Ping("www.google.com");
            if (pingGoogle)
            {
                return true;
            }
            var pingFacebook = Ping("www.facebook.com");

            if (pingFacebook)
            {
                return true;
            }
            return false;
        }

        public static bool Ping(string host)
        {
            var ping = new Ping();
            try
            {
                var resultado = ping.Send(host, 2000);
                return resultado.Status == IPStatus.Success;
            }
            catch
            {
                return false;
            }
        }

        public static string RetornarNomeComputador(string caminhoRede)
        {
            ValidacaoUtil.ValidarReferenciaNula(caminhoRede, nameof(caminhoRede));

            string nomeComputador = "";
            if (caminhoRede.StartsWith(@"\\"))
            {
                nomeComputador = caminhoRede.Substring(2);
            }
            if (nomeComputador.Contains(@"\"))
            {
                var posicao = nomeComputador.IndexOf(@"\");
                return nomeComputador.Substring(0, posicao);
            }
            return nomeComputador;
        }
    }
}