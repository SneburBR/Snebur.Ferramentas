using Snebur.Dominio.Atributos;

namespace Snebur.Dominio
{
    public interface IBorda
    {
        [NaoMapear]
        Cor Cor { get; set; }

        string CorRgba { get; set; }

        bool IsInterna { get; set; }

        double Afastamento { get; set; }

        double Espessura { get; set; }

        int Arredondamento { get; set; }

    }
}
