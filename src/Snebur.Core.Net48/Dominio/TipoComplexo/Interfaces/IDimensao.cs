namespace Snebur.Dominio
{
    public interface IDimensao
    {
        double Largura { get; set; }

        double Altura { get; set; }

        //[IgnorarPropriedadeTS]
        //int LarguraVisualizacao { get; }

        //[IgnorarPropriedadeTS]
        //int AlturaVisualizacao { get; }
    }
}
