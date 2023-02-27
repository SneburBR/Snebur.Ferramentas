using System.Collections.Generic;
using Snebur.Utilidade;

namespace Snebur.VisualStudio
{
    public class ConfiguracaoProjetoWebApresentacao : BaseConfiguracaoProjetoWeb
    {
        private List<string> _nomesPastaServidor;
        public string CaminhoConfiguracaoTypeScript { get; }

        public ConfiguracaoProjetoWebApresentacao(string caminhoWebConfig,
                                                  string caminhoConfiguracaoTypeScript) :
                                                  base(caminhoWebConfig)
        {
            this.CaminhoConfiguracaoTypeScript = caminhoConfiguracaoTypeScript;
        }

        protected override List<string> RetornarNomesPastaServidor()
        {
            if (this._nomesPastaServidor == null)
            {
                this._nomesPastaServidor = LazyUtil.RetornarValorLazyComBloqueio(ref this._nomesPastaServidor,
                                                                                 base.RetornarNomesPastaServidor);



            }
            return this._nomesPastaServidor;
        }
    }
}