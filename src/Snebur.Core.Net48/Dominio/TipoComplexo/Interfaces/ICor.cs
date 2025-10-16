namespace Snebur.Dominio
{
    public interface ICor
    {
        byte Red { get; }

        byte Green { get; }

        byte Blue { get; }

        double AlphaDecimal { get; }

        string Rgba { get; set; }
    }
}
