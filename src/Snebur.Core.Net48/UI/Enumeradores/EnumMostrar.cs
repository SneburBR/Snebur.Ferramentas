using Snebur.Dominio.Atributos;

namespace Snebur.UI
{
    public enum EnumMostrar
    {
        [Rotulo("Vazio")]
        Vazio = BaseEnumApresentacao.Vazio,

        Normal,
        Pequeno,
        Medido,
        Grande,
        SubTitulo,
        Titulo
    }
}
