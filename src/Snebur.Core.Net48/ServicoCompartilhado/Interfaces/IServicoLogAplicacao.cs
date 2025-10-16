using System;

namespace Snebur.Servicos
{
    public interface IServicoLogAplicacao
    {
        Guid NotificarLogAplicacao(string mensagem,
                                   BaseInformacaoAdicionalServicoCompartilhado informacaoAdicional);

        void NotificarAplicacaoAtiva(BaseInformacaoAdicionalServicoCompartilhado informacaoAdicional);

        bool AtivarLogServicoOnline(BaseInformacaoAdicionalServicoCompartilhado informacaoAdicional);
    }
}