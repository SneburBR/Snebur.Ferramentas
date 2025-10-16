using Snebur.Dominio;

namespace Snebur.Comunicacao
{
    public abstract class BaseResultadoRecuperarSenha : BaseDominio
    {

        public bool IsSucesso { get; set; }

        public int LimiteTentantivaAtingido { get; set; }

        public int TempoRestante { get; set; }
        public string MensagemErro { get; set; }
    }
}