using System;
using System.IO;
using System.Linq;
using System.Globalization;

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
        public bool IsCopiarSource { get; set; }
        public string CaminhoCompilacao { get; set; }
        public string[] CaminhosCompilacao { get; set; }
        public string[] Builds { get; set; }
        public BuildJsOptions BuildJsOptions { get; set; }
        public string[] ArquivosWeb { get; set; }
        public string[] ArquivosBin { get; set; }
        public string ExecutarProcessoDepois { get; set; }
        public string[] IgnorarArquivos { get; set; }  
        public string[] IgnorarArquivosRelease { get; set; } 

        public string RetornarNomeArquivo(
            string nomeProjeto,
            Version versao,
            EnumTipoCompilacao tipoCompilacao,
            string compilacao)
        {
            return String.IsNullOrWhiteSpace(this.NomePastaBuild)
                                       ? $"{nomeProjeto}_{versao}_{tipoCompilacao}_{compilacao}.zip"
                                       : $"{nomeProjeto}_{this.NomePastaBuild}_{tipoCompilacao}_{compilacao}.zip";
        }

        public string RetornarCaminhoPublicacaoBuild(Version versao,
                                                     string caminhoPublicacao)
        {
            return String.IsNullOrWhiteSpace(this.NomePastaBuild)
                         ? caminhoPublicacao
                         : Path.Combine(caminhoPublicacao, ConstantesProjeto.PASTA_WWWROOT_BUILD, this.NomePastaBuild, versao.ToString());

        }

        internal string[] RetornarCompilacoes()
        {
            if(this.CaminhosCompilacao?.Length > 0)
            {
                if (!String.IsNullOrWhiteSpace(this.CaminhoCompilacao) &&
                   !this.CaminhosCompilacao.Contains(this.CaminhoCompilacao))
                {
                    return this.CaminhosCompilacao.Concat(new string[] { this.CaminhoCompilacao }).ToArray();
                }
                return this.CaminhosCompilacao;
            }
            return new string[] { this.CaminhoCompilacao ?? "" };
        }

        internal bool IsIgnorarArquivo(string arquivo)
        {
            return IgIgnorarInterno(this.IgnorarArquivos, arquivo);
    
        }

        internal bool IsIgnorarArquivoRelease(string arquivo)
        {
            return IgIgnorarInterno(this.IgnorarArquivosRelease, arquivo);
        }

        private bool IgIgnorarInterno(string[] ignorarArquivos, string arquivo)
        {
            if (ignorarArquivos is null || ignorarArquivos.Length == 0)
            {
                return false;
            }

            foreach (var ignorar in ignorarArquivos)
            {
                if (arquivo.Contains(ignorar, System.Globalization.CompareOptions.OrdinalIgnoreCase))
                {
                    return true;
                }
                //check pattern
                if (ignorar.Contains("*"))
                {
                    var pattern = ignorar.Replace("*", "");
                    if (arquivo.Contains(pattern, System.Globalization.CompareOptions.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }

}
