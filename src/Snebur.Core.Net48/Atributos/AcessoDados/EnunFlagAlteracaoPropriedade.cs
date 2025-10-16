namespace Snebur.Dominio
{
    public enum EnunFlagAlteracaoPropriedade
    {
        None = 0,
        NotificarNovoCadastro = 2,
        VerificarAlteracaoNoBanco = 4,
        AtualizarDataHoraFimAlteracao = 8
    }
}