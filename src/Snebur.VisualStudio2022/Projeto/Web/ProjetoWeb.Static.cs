using System;
using System.IO;

namespace Snebur.VisualStudio
{
    public static class ProjetoWeb
    {
        internal static BaseConfiguracaoProjetoWeb RetornarConfiguracao(string caminhoConfiguracaoWebConfig,
                                                    string caminhoConfiguracaoTypeScript)
        {

            if (File.Exists(caminhoConfiguracaoTypeScript))
            {
                return new ConfiguracaoProjetoWebApresentacao(caminhoConfiguracaoWebConfig,
                                                              caminhoConfiguracaoTypeScript);

            }
            return new ConfiguracaoProjetoWebService(caminhoConfiguracaoWebConfig);
        }
    }
}
