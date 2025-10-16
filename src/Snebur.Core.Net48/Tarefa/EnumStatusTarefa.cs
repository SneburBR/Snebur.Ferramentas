using Snebur.Dominio.Atributos;

namespace Snebur.Tarefa
{
    [IgnorarEnumTS]
    public enum EnumStatusTarefa
    {
        Aguardando = 1,
        Executando = 2,
        Concluida = 3,
        Pausando = 4,
        Cancelando = 5,
        Pausada = 6,
        Cancelada = 99,
        Erro = 999
    }
}
