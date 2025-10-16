using Snebur.Dominio.Atributos;

namespace Snebur.Aplicacao.Configuracao
{
    [IgnorarInterfaceTS]
    public interface IContextoConfiugracao
    {
        void CriarArquivoConfiguracaoVazio();

        string CaminhoArquivoApplicationSettings { get; }
    }
}
