using Snebur.Dominio.Atributos;
using System.Collections.Generic;

namespace Snebur.Dominio
{
    [IgnorarInterfaceTS]
    public interface IMembrosDe : IEntidadeSeguranca
    {
        IEnumerable<IGrupoUsuario> MembrosDe { get; }
    }
}
