using Snebur.AcessoDados;
using Snebur.Dominio.Atributos;

namespace Snebur.Dominio
{
    [IgnorarInterfaceTS]
    public interface IRestricaoFiltroPropriedade : IRestricaoEntidade
    {
        [ValidacaoRequerido]
        [ValidacaoTextoTamanho(100)]
        string NomePropriedade { get; set; }

        [ValidacaoRequerido]
        EnumOperadorFiltro Operador { get; set; }

        [ValidacaoRequerido]
        string CaminhoValorPropriedadeIdentificacao { get; set; }

        //string NomeOperadorSeguranca { get; set; }
    }
}