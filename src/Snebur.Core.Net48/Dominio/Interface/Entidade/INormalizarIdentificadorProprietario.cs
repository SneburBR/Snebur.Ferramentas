using Snebur.Dominio.Atributos;

namespace Snebur.Dominio
{

    public interface INormalizarIdentificadorProprietario
    {
        [IgnorarMetodoTS]
        string NormalizarIdentificadorProprietario(string identificadorProprietario);
    }
}
