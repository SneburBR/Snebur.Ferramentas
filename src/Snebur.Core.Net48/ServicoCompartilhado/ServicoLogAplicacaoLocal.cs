using System;

namespace Snebur.Servicos
{
    public class ServicoLogAplicacaoLocal : BaseServicoLocal, IServicoLogAplicacao
    {

        public Guid NotificarLogAplicacao(string mensagem, BaseInformacaoAdicionalServicoCompartilhado informacaoAdicional)
        {
            this.SalvarLog(mensagem);
            return Guid.NewGuid();
        }

        public void NotificarAplicacaoAtiva(BaseInformacaoAdicionalServicoCompartilhado informacaoAdicional)
        {
            ///não faz nada
        }

        public bool AtivarLogServicoOnline(BaseInformacaoAdicionalServicoCompartilhado informacaoAdicional)
        {
            return false;
        }
    }
}