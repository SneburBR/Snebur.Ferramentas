using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Snebur.Publicacao;
using Snebur.Utilidade;

namespace Snebur.VisualStudio
{
    public abstract class BaseConfiguracaoProjetoWeb : ConfiguracaoProjeto
    {
        private const string NOME_ARQUIVO_APP_SETTING = "appSettings.config";
        public string CaminhoWebConfig { get; }
        public string CaminhoAppSetting { get; }
        public List<string> NomesPastaServidor => this.RetornarNomesPastaServidor();

        public BaseConfiguracaoProjetoWeb(string caminhoWebConfig)
        {
            this.CaminhoWebConfig = caminhoWebConfig;
            this.CaminhoAppSetting = Path.Combine(Path.GetDirectoryName(caminhoWebConfig), NOME_ARQUIVO_APP_SETTING);
        }
        protected override List<string> RetornarNomesProjetoDepedencia()
        {
            return new List<string>();
        }

        protected virtual List<string> RetornarNomesPastaServidor()
        {
            if (File.Exists(this.CaminhoAppSetting))
            {
                using (var fs = StreamUtil.OpenRead(this.CaminhoAppSetting))
                {
                    var xdoc = XDocument.Load(fs);

                    var elemento = xdoc.Elements().First().Elements().
                                                   Where(x => (string)x.Attribute("key") == ConstantesPublicacao.NOMES_PASTA_SERVIDOR).
                                                   SingleOrDefault();

                    if (elemento != null)
                    {
                        var valores = elemento.Attribute("value")?.Value?.ToString().Split(',').ToArray();
                        return new List<string>(valores);
                    }
                }
            }
            return new List<string>();
        }
    }
}