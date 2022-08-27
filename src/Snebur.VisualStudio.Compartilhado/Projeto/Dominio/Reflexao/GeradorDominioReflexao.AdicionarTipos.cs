using System;

namespace Snebur.VisualStudio
{
    public partial class GeradorDominioReflexao : BaseGeradorDominio
    {
        public string RetornarAdicionarTipo(string caminhoTipo, string declaracao)
        {
            return String.Format("$Reflexao.Tipos.Adicionar(\"{0}\",{1});", caminhoTipo, declaracao);
        }
    }
}
