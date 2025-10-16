using Snebur.Dominio.Atributos;

namespace Snebur.Dominio
{
    public interface IRegiao : IPosicao, IDimensao
    {
        [PropriedadeOpcionalTS]
        Posicao Posicao { get; }

        [PropriedadeOpcionalTS]
        Dimensao Dimensao { get; }
    }
}
