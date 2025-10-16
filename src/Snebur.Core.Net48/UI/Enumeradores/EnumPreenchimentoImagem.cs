
using Snebur.Dominio.Atributos;
using Snebur.UI;

namespace Snebur.Dominio
{
    public enum EnumPreenchimentoImagem
    {
        [Rotulo("Vazio")]
        Vazio = BaseEnumApresentacao.Vazio,

        Nenhum = 0,

        UniformeDentro = 1,

        UniformeFora = 2,

        UniformeCheio = 3,

        Esticar = 4
    }
}
