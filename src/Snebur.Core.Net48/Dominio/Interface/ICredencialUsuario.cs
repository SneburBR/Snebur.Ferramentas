namespace Snebur.Dominio
{
    public interface ICredencialUsuario : ICredencial
    {
        string Nome { get; set; }

        string IdentificadorAmigavel { get; set; }

    }
}
