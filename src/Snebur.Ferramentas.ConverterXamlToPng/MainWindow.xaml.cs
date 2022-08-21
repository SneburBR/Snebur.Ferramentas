using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Snebur.Dominio;
using Snebur.Imagem;
using Snebur.Utilidade;

namespace Snebur.Ferramentas.ConverterXamlToPng
{

    public partial class MainWindow : Window
    {
        private string Caminho = @"\\cha-arquivos\LIXO\Suellen\Cliparts FOTOLIVRO\Cliparts FOTOLIVRO";
        private string CaminhoDestino = @"\\cha-arquivos\LIXO\Suellen\Cliparts FOTOLIVRO\Png";
        private Dimensao DimensaoBase1200 { get; }
        private Dimensao DimensaoBase300 { get; }

        public MainWindow()
        {
            this.InitializeComponent();
            this.Loaded += this.This_Loaded;
            this.DimensaoBase1200 = new Dimensao(30, 30, 1200);
            this.DimensaoBase300 = new Dimensao(30, 30, 300);
        }

        private void This_Loaded(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
            //ThreadUtil.ExecutarStaAsync(this.Inicializar);
        }

        private void Inicializar()
        {
            var arquivos = Directory.GetFiles(this.Caminho, "*.xaml", SearchOption.AllDirectories);

            foreach (var (caminhoArquivo, processso) in arquivos.ToTupleItemProgresso())
            {
                var caminhoDestino = this.RetornarCaminhoDestinho(caminhoArquivo);
                
                var xaml = File.ReadAllText(caminhoArquivo, Encoding.UTF8);
                var xamlObject = XamlReader.Parse(xaml);


                if (xamlObject is Viewbox viewBox)
                {
                    RenderOptions.SetBitmapScalingMode(viewBox, BitmapScalingMode.Fant);
                    RenderOptions.SetEdgeMode(viewBox, EdgeMode.Aliased);


                    var dimensao1200 = DimensaoUtil.RetornarDimencaoUniformeDentro(viewBox.Width, viewBox.Height, this.DimensaoBase1200.LarguraVisualizacao, this.DimensaoBase1200.AlturaVisualizacao);
                    var dimensao300 = DimensaoUtil.RetornarDimencaoUniformeDentro(dimensao1200.Largura, dimensao1200.Altura, 
                                                                                  this.DimensaoBase300.LarguraVisualizacao, 
                                                                                  this.DimensaoBase300.AlturaVisualizacao);


                    viewBox.Width = dimensao1200.Largura;
                    viewBox.Height = dimensao1200.Altura;
                    viewBox.Measure(new Size(dimensao1200.Largura, dimensao1200.Altura));
                    viewBox.Arrange(new Rect(new Point(0, 0), new Size(dimensao1200.Largura, dimensao1200.Altura)));

                    RenderOptions.SetBitmapScalingMode(viewBox, BitmapScalingMode.Fant);
                    RenderOptions.SetEdgeMode(viewBox, EdgeMode.Aliased);

                    var imagemRenderizada = new RenderTargetBitmap((int)dimensao1200.Largura, (int)dimensao1200.Altura, MedidaUtil.DPI_VISUALIZACAO_WPF, MedidaUtil.DPI_VISUALIZACAO_WPF, PixelFormats.Pbgra32);

                    RenderOptions.SetBitmapScalingMode(imagemRenderizada, BitmapScalingMode.Fant);
                    RenderOptions.SetEdgeMode(imagemRenderizada, EdgeMode.Aliased);

                    imagemRenderizada.Render(viewBox);

                    RenderOptions.SetEdgeMode(imagemRenderizada, EdgeMode.Aliased);

                    var scalarX = dimensao300.Largura / dimensao1200.Largura;
                    var scalarY = dimensao300.Altura / dimensao1200.Altura;
                    var scale = new ScaleTransform(scalarX, scalarY);


                    var imagemRedimensionada = new TransformedBitmap(imagemRenderizada, scale);
                    var imagem300Dpi = BitmapSourceUtil.AjustarDpi(imagemRedimensionada, 300);

                    using (var fs = StreamUtil.OpenWrite(caminhoDestino))
                    {
                        var encoder = new PngBitmapEncoder();
                        encoder.Frames.Add(BitmapFrame.Create(imagem300Dpi));
                        encoder.Save(fs);
                    }
                }

                this.Dispatcher.Invoke(() =>
                {
                    this.BarraProgresso.Value = processso;
                });
            }

            this.Dispatcher.Invoke(() =>
            {
                MessageBox.Show(this, "Finalizado");
            });


        }

        private string RetornarCaminhoDestinho(string caminhoArquivo)
        {
            var nomeArquivo = Path.ChangeExtension(Path.GetFileName(caminhoArquivo), ".png");
            var diretorio = Path.Combine(this.CaminhoDestino, new FileInfo(caminhoArquivo).Directory.Name);
            DiretorioUtil.CriarDiretorio(diretorio);
            return Path.Combine(diretorio, nomeArquivo);
        }
    }
}
