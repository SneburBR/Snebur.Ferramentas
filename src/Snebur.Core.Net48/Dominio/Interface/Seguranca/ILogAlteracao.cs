using Snebur.Dominio.Atributos;

namespace Snebur.Dominio
{
    [IgnorarInterfaceTS]
    public interface ILogAlteracao : IEntidadeSeguranca
    {
        IUsuario UsuarioAvalista { get; set; }

        string NomeTipoEntidadeAlterada { get; set; }

        string NomeCampo { get; set; }

        string ValorAntigo { get; set; }

        string NovoValor { get; set; }
    }
}
