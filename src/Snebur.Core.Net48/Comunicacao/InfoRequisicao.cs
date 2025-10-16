using Snebur.Dominio;

namespace Snebur.Comunicacao
{
    public class InfoRequisicao :BaseDominio
    {
        public string UserAgent { get; set; }
        public string IpRequisicao { get; set; }
        public string CredencialUsuario { get; set; }
    }
}
