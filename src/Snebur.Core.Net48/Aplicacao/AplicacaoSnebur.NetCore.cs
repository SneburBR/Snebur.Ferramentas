
namespace Snebur
{
#if NET6_0_OR_GREATER

    using Microsoft.Extensions.Configuration;
    using System.Collections.Specialized;
    using System;
    using System.Linq;

    public abstract partial class AplicacaoSnebur
    {
        public static IConfiguration Configuration { get; set; }

        public void SetConfigure(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        protected NameValueCollection RetornarAppSettings()
        {
            const string CAHVE_APP_CONFICURATION = "AppConfiguration:";
            return this.RetornarConfiguracoes(CAHVE_APP_CONFICURATION);
        }

        protected NameValueCollection RetornarConnectionStrings()
        {
            const string CAHVE_CONNECTION_STRINGS = "ConnectionStrings:";
            return this.RetornarConfiguracoes(CAHVE_CONNECTION_STRINGS);
        }

        private NameValueCollection RetornarConfiguracoes(string chaveConfiguracao)
        {
            if (Configuration == null)
            {
                throw new Erro("A propriedade Configuration não foi definida");
            }

            var colecao = new NameValueCollection();
            var paresChaveValor = Configuration.AsEnumerable().ToList();
            foreach (var parChaveValor in paresChaveValor)
            {
                var chave = parChaveValor.Key;
                if (chave.StartsWith(chaveConfiguracao))
                {
                    var valor = parChaveValor.Value;
                    var chaveNormalizada = chave.Substring(chaveConfiguracao.Length);
                    colecao.Add(chaveNormalizada, valor);
                }
            }
            return colecao;
        }
    }

#endif  
}