using Snebur.Utilidade;
using System;
using System.IO;
using System.Linq;

namespace Snebur.Aplicacao.Configuracao
{
    public class ResolverFaltaApplicationSettingsPadrao : IResolverFaltaApplicationSettings
    {

        public void Resolver(IContextoConfiugracao contextoConfiguracao)
        {
            var caninhoArquivoMaisRecente = this.RetornarCaminhoArquivoConfiguracaoMaisRecente();
            if (!String.IsNullOrWhiteSpace(caninhoArquivoMaisRecente) && File.Exists(caninhoArquivoMaisRecente))
            {
                ArquivoUtil.CopiarArquivo(caninhoArquivoMaisRecente, contextoConfiguracao.CaminhoArquivoApplicationSettings);
            }
        }

        private string RetornarCaminhoArquivoConfiguracaoMaisRecente()
        {
            var caminhoLocalAppData = ConfiguracaoUtil.CaminhoAppDataAplicacao;
            var diretorioVersao = new DirectoryInfo(caminhoLocalAppData).Parent;

            if (diretorioVersao.Exists)
            {
                var diretoriosVersao = diretorioVersao.GetDirectories();
                var arquivos = diretoriosVersao.SelectMany(x => x.GetFiles(ConfiguracaoUtil.NOME_PADRAO_ARQUIVO_APPLICATIIONS_SETTING, SearchOption.AllDirectories)).ToList();
                var arquivoConfiguracaoRecente = arquivos.OrderByDescending(x => x.LastAccessTime).FirstOrDefault();
                if (arquivoConfiguracaoRecente != null)
                {
                    return arquivoConfiguracaoRecente.FullName;
                }
            }
            return null;
        }

        public object RetornarValor(string settingName, object valorPadrao, Type tipo)
        {
            return valorPadrao;
        }

        public void Dispose()
        {
        }
    }
}