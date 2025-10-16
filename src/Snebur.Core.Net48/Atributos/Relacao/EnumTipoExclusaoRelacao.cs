namespace Snebur.Dominio.Atributos
{
    //[IgnorarEnumTS]
    //[IgnorarTSReflexaoAttribute]
    [IgnorarGlobalizacao]
    public enum EnumTipoExclusaoRelacao
    {
        NaoDeletar = 1,

        Cascata = 2,

        Zerar = 3
    }
}