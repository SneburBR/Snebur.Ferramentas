namespace Snebur.VisualStudio
{
    public interface IGerenciadorProjetos
    {
        void AtualizarProjetoTS(ProjetoTypeScript projetoTypeScript);
        void AtualizarProjetoSass(ProjetoSass projetoEstilo);
    }
}
