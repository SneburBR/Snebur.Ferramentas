using Snebur.Dominio.Atributos;

namespace Snebur.Dominio
{
    [IgnorarInterfaceTS]
    public interface IPermissaoCampo : IEntidadeSeguranca
    {
        [ValidacaoRequerido]
        IPermissaoEntidade PermissaoEntidade { get; set; }

        [PropriedadeDescricao]
        [ValidacaoRequerido]
        [ValidacaoTextoTamanho(100)]
        string NomeCampo { get; set; }

        IRegraOperacao Leitura { get; set; }

        IRegraOperacao Atualizar { get; set; }
    }
}
