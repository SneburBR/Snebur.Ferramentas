namespace Snebur.VisualStudio
{
    public class PublicacaoConfig
    {
        public string Schema { get; set; }
        public string CaminhoPulicacao { get; set; }

        public string NomeLib { get; set; }
        public string NomePastaBuild { get; set; }
        public bool IsCriarPastaVersao { get; set; }
        public string[] Builds { get; set; }
        public BuildJsOptions BuildJsOptions { get; set; }
        public string[] ArquivosWeb { get; set; }
        public string[] ArquivosBin { get; set; }
        public string ExecutarProcessoDepois { get; set; }
        
    }

}
