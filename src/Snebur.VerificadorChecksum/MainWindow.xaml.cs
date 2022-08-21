using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Snebur.Utilidade;

namespace Checksum
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var caminho = System.IO.Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "TESTE");
            Directory.CreateDirectory(caminho);

            var caminhoArquivo = System.IO.Path.Combine(caminho, "teste.txt");
            File.WriteAllText(caminhoArquivo, " TESTE ");

        }
        private void BtnChecksum1_Click(object sender, RoutedEventArgs e)
        {
            this.AbrirArquivo(this.TxtChecksumMd5, this.TxtChecksumSh256);
        }

        private void BtnChecksum2_Click(object sender, RoutedEventArgs e)
        {
            this.AbrirArquivo(this.TxtChecksumMd5_2, this.TxtChecksumSh256_2);
        }

        private void AbrirArquivo(TextBox caixaDestinoMd5, TextBox caixaDestinoSh256)
        {
            var ofd = new OpenFileDialog();
            ofd.Multiselect = false;
            ofd.ShowDialog();

            if (File.Exists(ofd.FileName))
            {
                var checksum = ChecksumUtil.RetornarChecksum(ofd.FileName);
                var checksumSh256 = ChecksumUtil.RetornarChecksumSh256(ofd.FileName);

                caixaDestinoMd5.Text = checksum;
                caixaDestinoSh256.Text = checksumSh256;
            }

            this.AnalisarCheckum();
        }

       

        private void AnalisarCheckum()
        {
            var checksum1 = this.TxtChecksumMd5.Text;
            var checksum2 = this.TxtChecksumMd5_2.Text;

            this.TxtStatus.Text = (!String.IsNullOrWhiteSpace(checksum1)) && (checksum1 == checksum2) ? " OK " : "";

        }


    }
}
