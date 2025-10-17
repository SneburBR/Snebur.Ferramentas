using Snebur.Utilidade;
using System.IO;

namespace Snebur.VisualStudio
{
    public static class ProjetoTypescriptInitUtil
    {
        private static string _diretorioProjetoInicializador;
        public static string DiretorioProjetoInicializador
            => _diretorioProjetoInicializador ?? throw new System.Exception("DiretorioProjetoInicializador não foi inicializado. Chame SetDiretorioProjetoInicializador primeiro.");
        public static ConfiguracaoProjetoTypeScript ConfiguracaoProjetoTypeScript { get; private set; }

        public static void SetDiretorioProjetoInicializador(string diretorioProjeto)
        {
            if (diretorioProjeto == null)
            {
                LogVSUtil.LogErro("Não possível definir diretorioProjetoTypescript null");
                return;
            }

            if (_diretorioProjetoInicializador != diretorioProjeto)
            {
                _diretorioProjetoInicializador = diretorioProjeto;
            }

            var caminhoTS = Path.Combine(diretorioProjeto, ConstantesProjeto.CONFIGURACAO_TYPESCRIPT);
            ConfiguracaoProjetoTypeScript = JsonUtil.TryDeserializar<ConfiguracaoProjetoTypeScript>(ArquivoUtil.TryLerTexto(caminhoTS), EnumTipoSerializacao.Javascript);
        }

        public static void ClearDiretorioProjeto()
        {
            _diretorioProjetoInicializador = null;
        }
    }
}
