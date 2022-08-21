using Snebur.Depuracao;

namespace Snebur.VisualStudio
{
    public class MensagemEventArgs<TMensagem> where TMensagem : Mensagem
    {
        public TMensagem Mensagem { get; }
        public SessaoConectada SessaoConectada { get; }
        public MensagemEventArgs(SessaoConectada sessaoConectada, TMensagem mensagem)
        {
            this.Mensagem = mensagem;
            this.SessaoConectada = sessaoConectada;
        }
    }
}
