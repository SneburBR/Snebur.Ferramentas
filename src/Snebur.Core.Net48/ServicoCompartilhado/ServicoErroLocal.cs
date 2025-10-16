using System;

namespace Snebur.Servicos
{
    public class ServicoErroLocal : BaseServicoLocal, IServicoLogErro
    {
        //= @"c:\Erros";
        public Guid NotificarErro(string nomeTipoErro, string mensagem, string statkTrace, string descricaoCompleta, EnumNivelErro nivelErro, BaseInformacaoAdicionalServicoCompartilhado informacaoAdicional)
        {
            this.SalvarLog(descricaoCompleta);
            return Guid.Empty;
        }

        public bool CapturarPrimeiroErro()
        {
            return false;
        }
    }
}