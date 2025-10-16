namespace Snebur.Dominio
{
    public enum EnumResultadoAutenticacao
    {
        Sucesso = 1,
        UsuarioNaoExiste = 2,
        SenhaInvalida = 3,
        MaximoTentativa = 4,
        UsuarioBloqueado = 5
    }
}
