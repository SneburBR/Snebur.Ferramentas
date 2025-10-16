using Snebur.Dominio.Atributos;

namespace Snebur.Imagens
{
    public enum EnumDirecaoGradiente
    {
        [Rotulo("to left")]
        ToLeft,

        [Rotulo("to right")]
        ToRight,

        [Rotulo("to top")]
        ToTop,

        [Rotulo("to top right")]
        ToTopRight,

        [Rotulo("to top left")]
        ToTopLeft,

        [Rotulo("to bottom")]
        ToBottom,

        [Rotulo("to bottom right")]
        ToBottomRight,

        [Rotulo("to bottom left")]
        ToBottomLeft,
    }
}
