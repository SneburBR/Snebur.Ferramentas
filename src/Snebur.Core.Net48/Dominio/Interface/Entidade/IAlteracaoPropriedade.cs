using Snebur.Dominio.Atributos;
using System;

namespace Snebur.Dominio
{
    public interface IAlteracaoPropriedade : IAtividadeUsuario
    {

        [IgnorarPropriedade]
        [IgnorarPropriedadeTSReflexao]
        object ValorPropriedadeRelacao { get; set; }

        [IgnorarPropriedade]
        [IgnorarPropriedadeTSReflexao]
        object ValorPropriedadeAntigo { get; set; }

        [IgnorarPropriedade]
        [IgnorarPropriedadeTSReflexao]
        object ValorPropriedadeAlterada { get; set; }

        //[IgnorarPropriedadeTS]
        //[IgnorarPropriedadeTSReflexao]
        DateTime? DataHoraFimAlteracao { get; set; }
    }

}