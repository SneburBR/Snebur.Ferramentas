using Snebur.Dominio.Atributos;

namespace Snebur.Imagens
{
    public enum EnumTamanhoGradiente
    {
        [Rotulo("closest-side")]
        ClosestSide,
        [Rotulo("farthest-side")]
        FarthestSide,
        [Rotulo("closest-corner")]
        ClosestCorner,
        [Rotulo("farthest-corner")]
        FarthestCorner,
    }
}
