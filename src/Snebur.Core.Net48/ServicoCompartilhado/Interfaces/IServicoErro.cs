using System;

namespace Snebur.Servicos
{
    public interface IServicoLogErro
    {
        Guid NotificarErro(string nomeTipoErro,
                           string mensagem,
                           string statkTrace,
                           string descricaoCompleta,
                           EnumNivelErro nivelErro,
                           BaseInformacaoAdicionalServicoCompartilhado informacaoAdicional);

        bool CapturarPrimeiroErro();
    }
}