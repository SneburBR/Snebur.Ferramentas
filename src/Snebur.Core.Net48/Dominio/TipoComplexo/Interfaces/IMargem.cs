using Snebur.Dominio.Atributos;

namespace Snebur.Dominio
{
    public interface IMargem
    {
        double? Esquerda { get; set; }

        double? Superior { get; set; }

        double? Direita { get; set; }

        double? Inferior { get; set; }

        [IgnorarPropriedade]
        int? EsquerdaVisualizacao { get; }

        [IgnorarPropriedade]
        int? SuperiorVisualizacao { get; }

        [IgnorarPropriedade]
        int? DireitaVisualizacao { get; }

        [IgnorarPropriedade]
        int? InferiorVisualizacao { get; }
    }
}
