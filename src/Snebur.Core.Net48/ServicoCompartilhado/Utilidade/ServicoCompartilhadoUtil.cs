using Snebur.Utilidade;
using System;

namespace Snebur.Servicos
{
    public class ServicoCompartilhadoUtil
    {
        public static BaseInformacaoAdicionalServicoCompartilhado RetornarInformacaoAdicionalServicoCompartilhado()
        {
            try
            {
                return AplicacaoSnebur.Atual.FuncaoRetornarInformacaoAdicionalServicoCompartilhado?.Invoke();
            }
            catch (Exception)
            {
                if (DebugUtil.IsAttached)
                {
                    throw;
                }
                return null;
            }
        }
    }
}
