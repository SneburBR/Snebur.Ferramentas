using Snebur.Dominio.Atributos;

namespace Snebur.UI
{
    public enum EnumTipoBotao
    {
        [Rotulo("Vazio")]
        Vazio = BaseEnumApresentacao.Vazio,

        Normal = 1,

        Flat = 2,

        Circulo = 3,

        MiniCirculo = 4,

        Icone = 5,

        //IconeDireita =6,

        Link = 7,

        LinkDestaque = 8,

        Menu = 9,

        FlatBox = 10,

        Tab = 11,

    }

}