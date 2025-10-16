using Snebur.Dominio.Atributos;

//https://github.com/asadm/NETFx
namespace Snebur.Imagens
{
    public enum EnumMixagem
    {
        //[Rotulo("normal")]
        //Normal = 1,

        //[Rotulo("multiply")]
        //Multiply = 2,

        //[Rotulo("screen")]
        //Screen = 3,

        //[Rotulo("overlay")]
        //Overlay = 4,

        //[Rotulo("darken")]
        //Darken = 5,

        //Lighten = 6,
        //[Rotulo("color-dodge")]
        //ColorBodge = 7,
        //[Rotulo("color-burn")]
        //ColorBurn = 8,
        //[Rotulo("hard-light")]
        //HardLight = 9,
        //[Rotulo("soft-light")]

        [Rotulo("soft-light")]
        SoftLight = 10,

        //[Rotulo("difference")]
        //Difference = 11,

        //[Rotulo("exclusion")]
        //Exclusion = 12

        //Hue = 13,
        //Saturation = 14,

        //Color = 15,
        //Luminosity = 16,
    }
}
