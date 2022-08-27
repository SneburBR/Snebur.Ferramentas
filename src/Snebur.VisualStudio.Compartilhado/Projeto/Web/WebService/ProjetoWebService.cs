using Snebur.VisualStudio.DteExtensao;
using System.IO;

namespace Snebur.VisualStudio
{
    public class ProjetoWebService : ProjetoWeb<ConfiguracaoProjetoWebService>
    {
        public ProjetoWebService(Project projectVS,
                                 ConfiguracaoProjetoWebService configuracaProjeto,
                                 FileInfo arquivoProjeto,
                                 string caminhoConfiguracao) :
                                 base(projectVS, configuracaProjeto, arquivoProjeto, caminhoConfiguracao)
        {
            //ProjetoUtil.CompilarProjeto(dte, projetoVS);

        }
    }
}
