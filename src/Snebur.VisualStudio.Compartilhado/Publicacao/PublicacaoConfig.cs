using System;
using System.IO;

namespace Snebur.VisualStudio
{
    public class PublicacaoConfig
    {
        public string Schema { get; set; }
        public string CaminhoPulicacao { get; set; }
        public string NomeLib { get; set; }
        public string NomePastaBuild { get; set; }
        public bool IsCriarPastaVersao { get; set; }
        public bool IsZiparBin { get; set; }
        public string[] Builds { get; set; }
        public BuildJsOptions BuildJsOptions { get; set; }
        public string[] ArquivosWeb { get; set; }
        public string[] ArquivosBin { get; set; }
        public string ExecutarProcessoDepois { get; set; }


        public string RetornarNomeArquivo(Version versao)
        {
            return String.IsNullOrWhiteSpace(this.NomePastaBuild)
                                       ? $"{versao}.zip"
                                       : $"{this.NomePastaBuild}.zip";
        }

        public string RetornarCaminhoPublicacaoBuild(Version versao,
                                                     string caminhoPublicacao)
        {
            return String.IsNullOrWhiteSpace(this.NomePastaBuild) 
                         ? caminhoPublicacao
                         : Path.Combine(caminhoPublicacao, ConstantesProjeto.PASTA_WWWROOT_BUILD, this.NomePastaBuild, versao.ToString());

        }
    }

}
