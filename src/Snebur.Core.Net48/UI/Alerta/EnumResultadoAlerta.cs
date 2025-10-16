using Snebur.Dominio.Atributos;

namespace Snebur.UI
{
    public enum EnumResultadoAlerta
    {
        Sim,
        Nao,
        Continuar,
        Cancelar,
        Fechar,
        [Rotulo("OK")]
        Ok,
        EfetuarPgto
    }
}
