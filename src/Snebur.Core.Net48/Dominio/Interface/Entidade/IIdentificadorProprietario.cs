using Snebur.Dominio.Atributos;

namespace Snebur.Dominio
{
    public interface IIdentificadorProprietario
    {
        [Indexar]
        [ValidacaoRequerido]
        [ValidacaoTextoTamanho(50)]
        string IdentificadorProprietario { get; set; }
    }
}
