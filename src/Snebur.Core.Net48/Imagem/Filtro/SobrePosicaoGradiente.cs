namespace Snebur.Imagens
{
    public abstract class SobrePosicaoGradiente : SobrePosicao
    {
        public string Cor1 { get; set; }

        public string Cor2 { get; set; }

        public double LimiteCor1 { get; set; }

        public double LimiteCor2 { get; set; }
    }
}
