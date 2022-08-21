using System.ComponentModel;

namespace Snebur.VisualStudio
{
    public class ConfiguracaoGeral : BaseOptionModel<ConfiguracaoGeral>
    {
        [Category("Geral")]
        [DisplayName("Caminho VS")]
        [Description("Caminho da instalação do Visual Studio 2022")]
        [DefaultValue(@"C:\Program Files\Microsoft Visual Studio\2022\Community\")]
        public string CaminhoInstalacaoVisualStudio { get; set; } = @"C:\Program Files\Microsoft Visual Studio\2022\Community\";

        [Category("Geral")]
        [DisplayName("Projetos")]
        [Description("Caminho dos projetos")]
        [DefaultValue(@"E:\Projetos\TFS")]
        public string CaminhoProjetos { get; set; } = @"E:\GitHub\";

        [Category("Depuração")]
        [DisplayName("Porta de depuração aleatória")]
        [Description("Utilizar porta de depuração aleatória")]

        public bool IsUtilizarPortaDepuradaRandomica { get; set; } = true;

        [Category("Depuração")]
        [DisplayName("Porta de depuração")]
        [Description("Porta de depuração de 1 até 65535")]
        public ushort PortaDepuracao { get; set; } = 43812;

        //[Category("Depuração")]
        //[DisplayName("Copiar binários")]
        //[Description("Copiar os arquivos binários após compliar incrementar versão")]
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
