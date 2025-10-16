using Snebur.Dominio;

namespace Snebur.Comunicacao
{
    public class ResultadoValidarCodigoRecuperarSenha : BaseResultadoRecuperarSenha
    {
        public bool IsUsuarioEncontrado { get; set; }
        public EnumStatusCodigoRecuperarSenha Status { get; set; }
        public int TempoEsperar { get; set; }
    }
}
 