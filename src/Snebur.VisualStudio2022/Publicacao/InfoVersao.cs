namespace Snebur.VisualStudio
{
    public class InfoVersao
    {
        public DateTime Data { get; set; }
        public string Versao { get; set; }
        public string Checksum { get; set; }
        public string NomePastaBuild { get; set; }
        public bool IsCompactado { get; set; }
        public bool IsEncapsulado { get; set; }
        public bool IsTeste { get; set; }
        public bool IsLibZipAsync { get; set; }
    }

}
