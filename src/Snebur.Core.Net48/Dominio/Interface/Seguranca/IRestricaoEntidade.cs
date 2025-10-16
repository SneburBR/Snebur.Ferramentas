using Snebur.Dominio.Atributos;

namespace Snebur.Dominio
{
    [IgnorarInterfaceTS]
    public interface IRestricaoEntidade : IEntidadeSeguranca
    {
        [RelacaoPai]
        IPermissaoEntidade PermissaoEntidade { get; set; }
    }
}