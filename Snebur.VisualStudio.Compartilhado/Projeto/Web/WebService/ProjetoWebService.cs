namespace Snebur.VisualStudio
{
    public class ProjetoWebService : ProjetoWeb<ConfiguracaoProjetoWebService>
    {
        public ProjetoWebService(ConfiguracaoProjetoWebService configuracaProjeto,
                                 string caminhoProjeto,
                                 string caminhoConfiguracao) :
                                 base(configuracaProjeto, caminhoProjeto, caminhoConfiguracao)
        {
            //ProjetoUtil.CompilarProjeto(dte, projetoVS);

        }
    }
}
