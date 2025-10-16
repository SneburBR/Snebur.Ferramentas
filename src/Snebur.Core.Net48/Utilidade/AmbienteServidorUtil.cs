using System;

namespace Snebur.Utilidade
{
    public static class AmbienteServidorUtil
    {
        public static string NormalizarUrl(string url)
        {
            return url;

            //if (String.IsNullOrEmpty(url))
            //{
            //    return url;
            //}
            //if (!ValidacaoUtil.IsUrl(url))
            //{
            //    return url;
            //}

            //var uri = new Uri(url);
            //var host = AmbienteServidorUtil.NormalizarDominio(uri.Host);

            //switch (ConfiguracaoUtil.AmbienteServidor)
            //{
            //    case EnumAmbienteServidor.Producao:
            //    case EnumAmbienteServidor.Interno:

            //        return $"https://{host}:{uri.Port}{uri.PathAndQuery}";

            //    default:

            //        var porta = uri.Port == 443 ? 80 : uri.Port;
            //        return $"http://{host}:{porta}{uri.PathAndQuery}";
            //}
        }

        public static string NormalizarDominio(string dominio)
        {
            if (IpUtil.IsIP(dominio))
            {
                return dominio;
            }
            if (dominio.EndsWith(ConstantesDominioSuperior.DOMIMIO_SUPERIOR_PRODUCAO))
            {
                var ambienteServidor = ConfiguracaoUtil.AmbienteServidor;
                switch (ambienteServidor)
                {
                    case EnumAmbienteServidor.Localhost:

                        return dominio.Replace(ConstantesDominioSuperior.DOMIMIO_SUPERIOR_PRODUCAO,
                                               ConstantesDominioSuperior.DOMIMIO_SUPERIOR_LOCALHOST);

                    case EnumAmbienteServidor.Interno:

                        return dominio.Replace(ConstantesDominioSuperior.DOMIMIO_SUPERIOR_PRODUCAO,
                                               ConstantesDominioSuperior.DOMIMIO_SUPERIOR_INTERNO);

                    case EnumAmbienteServidor.Teste:

                        return dominio.Replace(ConstantesDominioSuperior.DOMIMIO_SUPERIOR_PRODUCAO,
                                               ConstantesDominioSuperior.DOMIMIO_SUPERIOR_TESTE);

                    case EnumAmbienteServidor.Producao:

                        return dominio;

                    default:
                        throw new Exception("O ambiente IIS não é suportado, conferir configuração do AppSettings");
                }
            }
            return dominio;
        }
    }
}