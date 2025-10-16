namespace Snebur.Dominio.Atributos
{
    [IgnorarGlobalizacao]
    [IgnorarEnumTS]
    [IgnorarTSReflexao]
    public enum EnumTipoRelacao
    {
        RelacaoPai = 1,

        RelacaoUmUm = 2,

        RelacaoUmUmReversa = 3,

        RelacaoFilhos = 4,

        RelacaoNn = 5,

        RelacaoNnEspecializada = 6
    }
}