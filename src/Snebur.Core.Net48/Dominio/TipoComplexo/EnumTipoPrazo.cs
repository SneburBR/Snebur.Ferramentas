using Snebur.Dominio.Atributos;

namespace Snebur.Dominio
{
    public enum EnumTipoPrazo
    {
        [Rotulo("Dias úteis")]
        DiasUteis = 0,

        [Rotulo("Dias corridos")]
        DiasCorrido = 1,

        [Rotulo("Horas")]
        Horas = 2,

        [Rotulo("Horas úteis")]
        HorasUteis = 3,
    }
}