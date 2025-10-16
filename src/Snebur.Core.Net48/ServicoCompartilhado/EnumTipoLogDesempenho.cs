using System.ComponentModel;

namespace Snebur.Servicos
{
    public enum EnumTipoLogDesempenho
    {
        [Description("Lentidão no serviço comunicação")]
        LentidaoServicoComunicacao = 1,

        [Description("Lentidão no renderizador")]
        Renderizador = 2
    }
}
