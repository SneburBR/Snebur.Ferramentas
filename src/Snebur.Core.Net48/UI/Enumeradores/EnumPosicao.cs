using Snebur.Dominio.Atributos;

namespace Snebur.UI
{
    public enum EnumPosicao
    {
        [Rotulo("Vazio")]
        Vazio = BaseEnumApresentacao.Vazio,

        Nenhum = 0,
        Esquerda = 1,
        Direita = 2,
        Superior = 4,
        Inferior = 8,
        Tudo = 16
    }
}