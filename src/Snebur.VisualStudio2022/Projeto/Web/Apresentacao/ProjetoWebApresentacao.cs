using EnvDTE;
using EnvDTE80;

namespace Snebur.VisualStudio
{
    public class ProjetoWebApresentacao : ProjetoWeb<ConfiguracaoProjetoWebApresentacao>
    {
        public ProjetoTypeScript ProjetoTypeScript { get; }
        public ProjetoWebApresentacao(ConfiguracaoProjetoWebApresentacao configuracaProjeto,
                                      ProjetoTypeScript projetoTypeScript,
                                      DTE2 dte, 
                                      Project projetoVS,
                                      string caminhoProjeto,
                                      string caminhoConfiguracao) :
                                      base(configuracaProjeto, dte, projetoVS, caminhoProjeto, caminhoConfiguracao)
        {
            //ProjetoUtil.CompilarProjeto(dte, projetoVS);]
            this.ProjetoTypeScript = projetoTypeScript;
        }

        

        public override void InscrementarVersao()
        {
            base.InscrementarVersao();
            this.ProjetoTypeScript.InscrementarVersao();
        }
    }
}
