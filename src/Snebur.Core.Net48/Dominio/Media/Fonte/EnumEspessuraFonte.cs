using Snebur.Dominio.Atributos;

namespace Snebur.Dominio
{
    public enum EnumEspessuraFonte
    {
        [Rotulo("Fina")]
        Fina = 100,
        [Rotulo("Extra leve")]
        ExtraLeve = 200,
        [Rotulo("Leve")]
        Leve = 300,
        [Rotulo("Normal")]
        Normal = 400,
        [Rotulo("Médio")]
        Media = 500,
        [Rotulo("Semi negrito")]
        SemiNegrito = 600,
        [Rotulo("Negrito")]
        Negrito = 700,
        [Rotulo("Extra negrito")]
        ExtraNegrito = 800,
        [Rotulo("Forte")]
        Forte = 900
    }

}
