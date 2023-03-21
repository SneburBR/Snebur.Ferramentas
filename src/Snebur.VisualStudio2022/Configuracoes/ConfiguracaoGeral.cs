using System.ComponentModel;

namespace Snebur.VisualStudio
{
    public class ConfiguracaoGeral : BaseOptionModel<ConfiguracaoGeral>, IConfiguracaoGeral
    {
        private bool _isMostrarHoraLogOutput = false;

        [Category("Geral")]
        [DisplayName("Caminho VS")]
        [Description("Caminho da instalação do Visual Studio 2022")]
        [DefaultValue(@"C:\Program Files\Microsoft Visual Studio\2022\Community\")]
        public string CaminhoInstalacaoVisualStudio { get; set; } = @"C:\Program Files\Microsoft Visual Studio\2022\Community\";

        [Category("Geral")]
        [DisplayName("Projetos Snebur")]
        [Description("Caminho do diretorio que contem os projetios Snebur.Framework e Snebur.TS ")]
        [DefaultValue(@"D:\OneDrive\GitHub\Snebur")]
        public string CaminhoProjetosSnebur { get; set; } = @"D:\OneDrive\GitHub\Snebur";

        [Category("Geral")]
        [DisplayName("Diretório dos Itens templates")]
        [Description("Caminho dos itens templates")]
        [DefaultValue(@"")]
        public string DiretorioItensTemplate { get; set; } = @"";

        [Category("Depuração")]
        [DisplayName("Porta de depuração aleatória")]
        [Description("Utilizar porta de depuração aleatória")]

        public bool IsUtilizarPortaDepuradaRandomica { get; set; } = true;

        [Category("Depuração")]
        [DisplayName("Porta de depuração")]
        [Description("Porta de depuração de 1 até 65535")]
        public ushort PortaDepuracao { get; set; } = 43812;

        [Category("Depuração")]
        [DisplayName("Mostrar hora")]
        [Description("Mostrar na hora do logs na janela do output da extensão")]
        public bool IsMostrarHoraLogOutput
        {
            get => this._isMostrarHoraLogOutput ;
            set
            {
                if (this._isMostrarHoraLogOutput != value)
                {
                    LogVSUtil.IsMostrarHoraLogOutput = value;
                    this._isMostrarHoraLogOutput = value;
                }
            }
        }

        //[Category("Depuração")]
        //[DisplayName("Copiar binários")]
        //[Description("Copiar os arquivos binários após compilar incrementar versão")]
        //[DefaultValue(false)]
        //public bool IsCopiarArquivosBinarios { get; set; } = false;

        //[Category("Depuração")]
        //[DisplayName("Destino cópia binários")]
        //[Description("Caminho do diretorio destino para copiar os binários da versão ")]
        //[DefaultValue("")]
        //public string DiretorioDestinoBinarios { get; set; } = "";

        public ConfiguracaoGeral() : base()
        {

        }
    }
}
