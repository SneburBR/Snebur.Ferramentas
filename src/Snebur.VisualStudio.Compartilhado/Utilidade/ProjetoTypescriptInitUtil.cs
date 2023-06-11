using Snebur.Utilidade;
using System.IO;

namespace Snebur.VisualStudio
{
    public static class ProjetoTypescriptInitUtil
    {
        public static string DiretorioProjeto { get; private set; }
        public static ConfiguracaoProjetoTypeScript ConfiguracaoProjetoTypeScript { get; private set; }

        public static void SetDiretorioProjeto(string diretorioProjeto)
        {
            if (diretorioProjeto == null)
            {
                LogVSUtil.LogErro("Não possível definir diretorioProjetoTypescript null");
                return;
            }

            if (DiretorioProjeto != diretorioProjeto)
            {
                DiretorioProjeto = diretorioProjeto;
            }

            var caminhoTS = Path.Combine(diretorioProjeto, ConstantesProjeto.CONFIGURACAO_TYPESCRIPT);
            ConfiguracaoProjetoTypeScript = JsonUtil.TryDeserializar<ConfiguracaoProjetoTypeScript>(ArquivoUtil.TryLerTexto(caminhoTS), EnumTipoSerializacao.Javascript);
        }

        public static void ClearDiretorioProjeto()
        {
            DiretorioProjeto = null;
        }
    }
}
