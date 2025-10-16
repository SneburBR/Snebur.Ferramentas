using Snebur.Dominio.Atributos;
using Snebur.Reflexao;
using System;

namespace Snebur.Dominio
{
    public interface IAlteracaoPropriedadeGenerica : IAtividadeUsuario
    {
        [Indexar]
        long IdEntidade { get; set; }

        [Indexar]
        [ValidacaoRequerido]
        int IdNamespace { get; set; }

        [Indexar]
        [ValidacaoRequerido]
        [ValidacaoTextoTamanho(255)]
        string NomeTipoEntidade { get; set; }

        [Indexar]
        [ValidacaoRequerido]
        [ValidacaoTextoTamanho(255)]
        string NomePropriedade { get; set; }

        [ValidacaoRequerido]
        EnumTipoPrimario? TipoPrimario { get; set; }
        bool IsTipoComplexo { get; set; }

        [ValidacaoTextoTamanho(5000)]
        string ValorPropriedadeAntigo { get; set; }

        [ValidacaoTextoTamanho(5000)]
        string ValorPropriedadeAlterada { get; set; }

        DateTime? DataHoraFimAlteracao { get; set; }
 
    }
}
