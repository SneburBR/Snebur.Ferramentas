namespace Snebur.Dominio
{
    public interface IPrazoTempo
    {
        double Prazo { get; set; }
        EnumTipoPrazo TipoPrazo { get; set; }
    }
}