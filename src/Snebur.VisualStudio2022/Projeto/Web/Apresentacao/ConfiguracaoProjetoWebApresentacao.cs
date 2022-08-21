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
            if (_nomesPastaServidor == null)
            {
                _nomesPastaServidor = ThreadUtil.RetornarValorComBloqueio(ref this._nomesPastaServidor,
                                                                       base.RetornarNomesPastaServidor);



            }
            return _nomesPastaServidor;
        }
    }
}