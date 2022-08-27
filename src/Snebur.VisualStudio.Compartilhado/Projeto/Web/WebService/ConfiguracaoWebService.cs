using System.Collections.Generic;

namespace Snebur.VisualStudio
{
    public class ConfiguracaoProjetoWebService : BaseConfiguracaoProjetoWeb
    {
         

        public ConfiguracaoProjetoWebService(string caminhoWebConfig):
                                             base(caminhoWebConfig)
        {
            
        }
         


        protected override List<string> RetornarNomesPastaServidor()
        {
            return base.RetornarNomesPastaServidor();
        }
    }
}