using Snebur.Dominio.Atributos;

namespace Snebur.Dominio
{
    public interface IAtivo : IEntidade
    {
        [ValidacaoRequerido]
        bool IsAtivo { get; set; }
    }

}
