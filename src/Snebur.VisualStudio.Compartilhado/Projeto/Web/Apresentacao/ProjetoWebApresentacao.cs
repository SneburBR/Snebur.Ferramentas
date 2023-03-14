using System.IO;

namespace Snebur.VisualStudio
{
    public class ProjetoWebApresentacao : ProjetoWeb<ConfiguracaoProjetoWebApresentacao>
    {
        public ProjetoTypeScript ProjetoTypeScript { get; }
        public ProjetoWebApresentacao(ProjetoViewModel projetoVM,
                                      ConfiguracaoProjetoWebApresentacao configuracaProjeto,
                                      ProjetoTypeScript projetoTypeScript,
                                      FileInfo arquivoProjeto,
                                      string caminhoConfiguracao) :
                                      base(projetoVM, 
                                          configuracaProjeto,
                                          arquivoProjeto,
                                          caminhoConfiguracao)
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
