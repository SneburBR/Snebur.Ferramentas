using Snebur.Dominio.Atributos;

namespace Snebur.Dominio
{
    [IgnorarInterfaceTS]
    public interface IPermisaoCampo : IEntidadeSeguranca
    {
        [ValidacaoRequerido]
        IPermissaoEntidade AutorizacaoEntidade { get; set; }

        [ValidacaoRequerido]
        [ValidacaoTextoTamanho(100)]
        string NomeCampo { get; set; }

        IRegraOperacao Leitura { get; set; }

        IRegraOperacao Atualizar { get; set; }
    }
}
