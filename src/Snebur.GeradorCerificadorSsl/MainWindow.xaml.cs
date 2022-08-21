using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;

namespace Snebur.GeradorCerificadorSsl
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        //private string CaminhoWindowsSDK = @"C:\Program Files (x86)\Windows Kits\10\bin\10.0.17763.0\x64";
        private string CaminhoWindowsSDK = @"C:\Temp\ceriticados";
        
        private string CaminhoMakeCert => Path.Combine(this.CaminhoWindowsSDK, "makecert.exe");
        private string CaminhoMakePvk2Pfx => Path.Combine(this.CaminhoWindowsSDK, "pvk2pfx.exe");
        private string SenhaLocal = "temp@1243";

        private string CaminhoDestinoCertificao = @"c:\temp\ceriticados";

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += this.This_Loaded;

        }

        private void This_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!Directory.Exists(this.CaminhoDestinoCertificao))
                {
                    Directory.CreateDirectory(this.CaminhoDestinoCertificao);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"Não foi possivel criar pasta {this.CaminhoDestinoCertificao}\n {ex.Message}", "Erro criar diretorio", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnGerarCertificado_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.CriarCertificado();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"Não foi possivel criar certificado\n {ex.Message}", "Erro criar diretorio", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CriarCertificado()
        {
            var nomeDocminio = this.TxtDominio.Text;
            if (!String.IsNullOrWhiteSpace(nomeDocminio))
            {
                var nomeArquivoPvk = $"{nomeDocminio}.pvk";
                var nomeArquivoCer = $"{nomeDocminio}.cer";
                var nomeArquivoPfx = $"{nomeDocminio}.pfx";

                //var caminhoCer = Path.Combine(this.CaminhoDestinoCertificao, nomeArquivoCer);
                //var caminhoPvk = Path.Combine(this.CaminhoDestinoCertificao, nomeArquivoPvk);
                //var caminhoPfx = Path.Combine(this.CaminhoDestinoCertificao, nomeArquivoCer);

                var caminhoCer = nomeArquivoPvk;
                var caminhoPvk = nomeArquivoCer;
                var caminhoPfx = nomeArquivoPfx;

                this.DeletarArquivo(caminhoCer);
                this.DeletarArquivo(caminhoPvk);
                this.DeletarArquivo(caminhoPfx);

                var argumentosMakeCert = $"-r -pe -n \"CN={nomeDocminio}\" -eku 1.3.6.1.5.5.7.3.1 -sky exchange -sv {caminhoPvk} {caminhoCer}";

                var comandoCompleto = $"{this.CaminhoMakeCert} {argumentosMakeCert}";
                var piMakeCert = new ProcessStartInfo(this.CaminhoMakeCert, argumentosMakeCert);
                var processo = Process.Start(piMakeCert);

                while (true)
                {
                    Thread.Sleep(1000);
                    try
                    {
                        var processoRecuperado = Process.GetProcessById(processo.Id);
                        if (processoRecuperado == null)
                        {
                            break;
                        }
                    }
                    catch
                    {
                        break;
                    }
                }
                Thread.Sleep(1000);
                var argumentosPvk2Pfx = $"-pvk {caminhoPvk} -spc {caminhoCer} -pfx {caminhoPfx}";
                var piPvk2Pfx = new ProcessStartInfo(this.CaminhoMakePvk2Pfx, argumentosPvk2Pfx);
                Process.Start(piPvk2Pfx);
            }
        }

        private void DeletarArquivo(string caminhoPvk)
        {
            if (File.Exists(caminhoPvk))
            {
                File.Exists(caminhoPvk);
            }
        }
    }
}
