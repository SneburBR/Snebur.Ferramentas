using Snebur.Dominio.Atributos;

namespace Snebur.UI
{
    public enum EnumBotoesAlerta
    {
        SimNao,
        Fechar,
        FecharVoltar,
        Nenhum,
        //Cancelar,
        //Continuar,
        [Rotulo("OK")]
        Ok,
        [Rotulo("OK, cancelar")]
        OkCancelar,
        Personalizado
    }
}
