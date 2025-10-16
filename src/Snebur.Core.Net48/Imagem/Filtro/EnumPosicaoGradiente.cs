using Snebur.Dominio.Atributos;

namespace Snebur.Imagens
{
    public enum EnumPosicaoGradiente
    {
        [Rotulo("left top")]
        LeftTop,
        [Rotulo("center top")]
        CenterTop,
        [Rotulo("right top")]
        RightTop,
        [Rotulo("left center")]
        LeftCenter,
        [Rotulo("center center")]
        CenterCenter,
        [Rotulo("right center")]
        RightCenter,
        [Rotulo("left bottom")]
        LeftBottom,
        [Rotulo("center bottom")]
        CenterBottom,
        [Rotulo("right bottom")]
        RightBottom
    }
}
