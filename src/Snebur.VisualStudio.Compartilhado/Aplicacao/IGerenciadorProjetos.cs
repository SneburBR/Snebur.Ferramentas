namespace Snebur.VisualStudio
{
    public interface IGerenciadorProjetos
    {
        string DiretorioProjetoTypescriptInicializacao { get; }
        ConfiguracaoProjetoTypeScript ConfiguracaoProjetoTypesriptInicializacao { get; }

        void AtualizarProjetoTS(ProjetoTypeScript projetoTypeScript);
        void AtualizarProjetoSass(ProjetoSass projetoEstilo);
    }
}
