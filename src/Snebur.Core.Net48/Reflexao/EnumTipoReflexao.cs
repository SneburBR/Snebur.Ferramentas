using Snebur.Dominio.Atributos;

namespace Snebur.Reflexao
{
    [IgnorarEnumTS]
    public enum EnumTipoReflexao
    {
        TipoPrimario = 1,

        TipoBaseDominio = 2,

        TipoEntidade = 3,

        TipoEnum = 4,

        TipoListaTipoPrimario = 5,

        TipoListaBaseDominio = 6,

        TipoListaEntidade = 7,

        TipoListaEnum = 8,

        Dicionario = 9,

        Generico = 10
    }
}
