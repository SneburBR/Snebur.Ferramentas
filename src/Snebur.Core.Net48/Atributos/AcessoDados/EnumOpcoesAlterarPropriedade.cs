namespace Snebur.Dominio
{
    public enum EnumOpcoesAlterarPropriedade
    {
        Nenhuma = 0,
        NotificarNovoCadastro = 2,
        VerificarAlteracaoNoBanco = 4,
        AtualizarDataHoraFimAlteracao = 8,
        IgnorarZeroIgualNull = 16,
        IgnorarValorAntigoNull = 32,
        Proteger = 64,
    }
}