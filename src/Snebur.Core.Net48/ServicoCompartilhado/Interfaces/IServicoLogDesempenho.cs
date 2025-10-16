using System;

namespace Snebur.Servicos
{
    public interface IServicoLogDesempenho
    {
        Guid NotificarLogDesempenho(string mensagem,
                                    string stackTrace,
                                    EnumTipoLogDesempenho tipoLogDesempenho,
                                    BaseInformacaoAdicionalServicoCompartilhado informacaoAdicional);
    }
}
