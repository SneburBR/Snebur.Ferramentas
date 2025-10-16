using Snebur.Utilidade;
using System;
using System.Configuration;
using System.IO;

namespace Snebur.Aplicacao.Configuracao
{
    public abstract class BaseCaminhoPadraoApplicationSettings : SettingsProvider
    {
        protected internal const string NAME = "name";
        protected internal const string SERIALIZE_AS = "serializeAs";
        protected internal const string CONFIG = "configuration";
        protected internal const string USER_SETTINGS = "userSettings";
        protected internal const string SETTING = "setting";

        private string _caminhoArquivoConfiguracao { get; set; }
        protected bool _carregado = false;
        private static string _diretorioConfiguracao;

        public string CaminhoArquivoApplicationSettings
        {
            get
            {
                if (this._caminhoArquivoConfiguracao == null)
                {
                    this._caminhoArquivoConfiguracao = ConfiguracaoUtil.RetornarCaminhoArquivoApplicationSettingsPadrao();
                }
                return this._caminhoArquivoConfiguracao;
            }
        }

        public override string ApplicationName
        {
            get { return AplicacaoSnebur.Atual.NomeAplicacao; }
            set { }
        }

        public static string DiretorioConfiguracao
        {
            get
            {
                if (_diretorioConfiguracao == null)
                {
                    var caminhoLocalAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                    var caminhoAppDataAplicacao = Path.Combine(caminhoLocalAppData, AplicacaoSnebur.Atual.NomeEmpresa, AplicacaoSnebur.Atual.NomeAplicacao);
                    DiretorioUtil.CriarDiretorio(caminhoAppDataAplicacao);
                    _diretorioConfiguracao = caminhoAppDataAplicacao;
                }
                return _diretorioConfiguracao;
            }
        }
    }
}