using System.IO;

namespace Snebur.VisualStudio
{
    public class ProjetoWebService : ProjetoWeb<ConfiguracaoProjetoWebService>
    {
        public ProjetoWebService(ProjetoViewModel projetoVM,
                                 ConfiguracaoProjetoWebService configuracaProjeto,
                                 FileInfo arquivoProjeto,
                                 string caminhoConfiguracao) : base(projetoVM, configuracaProjeto, arquivoProjeto, caminhoConfiguracao)
        {
            //ProjetoUtil.CompilarProjeto(dte, projetoVS);

        }
    }
}
