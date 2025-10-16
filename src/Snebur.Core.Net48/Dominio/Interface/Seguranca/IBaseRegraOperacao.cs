using Snebur.Dominio.Atributos;

namespace Snebur.Dominio
{
    [IgnorarInterfaceTS]
    public interface IBaseRegraOperacao /*: IEntidadeSeguranca*/
    {
        int MaximoRegistro { get; set; }

        bool Autorizado { get; set; }

        bool AvalistaRequerido { get; set; }

        bool AtivarLogSeguranca { get; set; }

        bool AtivarLogAlteracao { get; set; }

        //bool AvalistaRequerido { get; set; }
    }
}
