using Snebur.Dominio.Atributos;

namespace Snebur.Dominio
{
    public interface IPosicao
    {
        double X { get; set; }

        double Y { get; set; }

        [IgnorarPropriedade]
        int XVisualizacao { get; }

        [IgnorarPropriedade]
        int YVisualizacao { get; }
    }
}