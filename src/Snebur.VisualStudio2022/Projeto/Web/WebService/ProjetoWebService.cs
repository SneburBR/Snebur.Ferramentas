using EnvDTE;
using EnvDTE80;

namespace Snebur.VisualStudio
{
    public class ProjetoWebService : ProjetoWeb<ConfiguracaoProjetoWebService>
    {
        public ProjetoWebService(ConfiguracaoProjetoWebService configuracaProjeto,
                                  DTE2 dte, Project projetoVS,
                                 string caminhoProjeto,
                                 string caminhoConfiguracao) :
                                 base(configuracaProjeto, dte, projetoVS, caminhoProjeto, caminhoConfiguracao)
        {
            //ProjetoUtil.CompilarProjeto(dte, projetoVS);

        }
    }
}
