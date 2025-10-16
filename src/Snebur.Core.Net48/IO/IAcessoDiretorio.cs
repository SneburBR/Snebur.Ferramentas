using Snebur.Dominio.Atributos;

namespace Snebur.IO
{
    public interface IAcessoDiretorio
    {
        string Caminho { get; }

        bool IsAutenticar { get; }

        bool IsRede { get; }

        string Dominio { get; }

        string Usuario { get; }

        string Senha { get; }

        [IgnorarPropriedade]
        [IgnorarPropriedadeTSReflexao]
        string NomeComputador { get; }
    }
}
