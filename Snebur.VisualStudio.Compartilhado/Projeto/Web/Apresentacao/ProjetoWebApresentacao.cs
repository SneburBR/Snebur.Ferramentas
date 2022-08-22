using Snebur.VisualStudio.DteExtensao;
using System.IO;

namespace Snebur.VisualStudio
{
    public class ProjetoWebApresentacao : ProjetoWeb<ConfiguracaoProjetoWebApresentacao>
    {
        public ProjetoTypeScript ProjetoTypeScript { get; }
        public ProjetoWebApresentacao(Project projectVS,
                                      ConfiguracaoProjetoWebApresentacao configuracaProjeto,
                                      ProjetoTypeScript projetoTypeScript,
                                      FileInfo arquivoProjeto,
                                      string caminhoConfiguracao) :
                                      base(projectVS, 
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
