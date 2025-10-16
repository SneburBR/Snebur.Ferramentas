using Snebur.Dominio.Atributos;

namespace Snebur.Dominio
{
    public interface IOrdenacao /*: IEntidade*/
    {
        [Rotulo("Ordenação")]
        [Indexar]
        double? Ordenacao { get; set; }
    }
    //[IgnorarInterfaceTS]
    public interface IOrdenacaoEntidade : IEntidade, IOrdenacao
    {
    }
}