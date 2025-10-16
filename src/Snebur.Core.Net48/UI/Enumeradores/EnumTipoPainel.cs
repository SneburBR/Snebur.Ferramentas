using Snebur.Dominio.Atributos;

namespace Snebur.UI
{
    public enum EnumTipoPainel
    {
        [Rotulo("Vazio")]
        Vazio = BaseEnumApresentacao.Vazio,
        Bloco = 1,
        BlocoVertical = 2,
        BlocoPilha = 3,
        PilhaHorizontal = 4,
        PilhaVertical = 5,
        PilhaHorizontalCheia = 6,
        PilhaVerticalCheia = 7,
        PilhaHorizontalEmLinha = 8,
    }
}
