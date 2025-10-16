using Snebur.Dominio;

namespace Snebur.Comunicacao
{
    public class ResultadoAutenticacao : BaseDominio
    {
        public bool IsSucesso { get; set; }

        public EnumResultadoAutenticacao Resultado { get; set; }

        public bool IsAlterarSenhaProximoAcesso { get; set; }

        public int TempoEsperar { get; set; }
    }
}