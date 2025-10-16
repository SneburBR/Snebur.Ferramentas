namespace Snebur.Imagens
{
    public abstract class SobrePosicao
    {
        //public EnumMixagem Mixagem { get; set; }
        //Por impedimentos da renderização, somente o mixagem soft-light está implementada;
        public EnumMixagem Mixagem
        {
            get => EnumMixagem.SoftLight;
            set { }
        }

        //public double Opacidade { get; set; }
    }
}
