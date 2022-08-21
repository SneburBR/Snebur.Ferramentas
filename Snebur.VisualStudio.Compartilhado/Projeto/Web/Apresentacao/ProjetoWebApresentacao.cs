namespace Snebur.VisualStudio
{
    public class ProjetoWebApresentacao : ProjetoWeb<ConfiguracaoProjetoWebApresentacao>
    {
        public ProjetoTypeScript ProjetoTypeScript { get; }
        public ProjetoWebApresentacao(ConfiguracaoProjetoWebApresentacao configuracaProjeto,
                                      ProjetoTypeScript projetoTypeScript,
                                      string caminhoProjeto,
                                      string caminhoConfiguracao) :
                                      base(configuracaProjeto, 
                                          caminhoProjeto,
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
