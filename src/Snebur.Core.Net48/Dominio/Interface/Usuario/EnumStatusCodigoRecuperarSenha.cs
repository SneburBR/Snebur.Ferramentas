namespace Snebur.Dominio
{
    public enum EnumStatusCodigoRecuperarSenha
    {
        Nenhum = 0,
        Novo = 1,
        TentativaInvalida = 2,
        Sucesso = 3,
        Expirado = 4,
        MaximoTentativaAtingido = 5,
        SenhaRecuperada = 6,
        TempoMaximoTentativaExpirado = 7
    }
}
