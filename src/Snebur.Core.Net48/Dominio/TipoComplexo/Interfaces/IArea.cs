using Snebur.Dominio.Atributos;

namespace Snebur.Dominio
{
    public interface IArea : IMargem, IDimensao
    {
        [PropriedadeOpcionalTS]
        Margem Margem { get; set; }

        [PropriedadeOpcionalTS]
        Dimensao Dimensao { get; set; }

        [MetodoOpcionalTS]
        Regiao CalcularRegiao(Dimensao dimensaoRecipiente);

        //Posicao CalcularPosicao(Dimensao dimensaoRecipiente);

        //Dimensao CalcularDimensao(Dimensao dimensaoRecipiente);

    }
}
