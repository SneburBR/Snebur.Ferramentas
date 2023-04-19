using Snebur.Utilidade;
using Snebur.VisualStudio.ToolWindows.Output;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Snebur.VisualStudio
{

    public partial class OutputWindowControl : UserControl
    {
        public ObservableCollection<ILogMensagemViewModel> Logs => LogVSUtil.Logs;
        public int PortaDepuracao => ConfiguracaoVSUtil.PortaDepuracao;

        public bool IsOcupado { get; private set; }

        public OutputWindowControl()
        {
            this.InitializeComponent();
            this.DataContext = this;
            this.Loaded += this.Janela_Loaded;
        }

        private bool _isInicializado;
        public void Janela_Loaded(object sender, RoutedEventArgs e)
        {
            if (!this._isInicializado)
            {
                this._isInicializado = true;
                _ = this.InicializarAsync();
            }
            //LogVSUtil.Logs = this.Logs;
        }

        private async Task InicializarAsync()
        {
            await this.OcuparAsync();
            this.TxtTitulo.Text = "Snebur v." + this.GetType().Assembly.GetName().Version.ToString();
            LogUtil.CriarEspacoSneburVisualizadorEventos();

            await GerenciadorProjetos.Instancia.AnalisarNecessidadeServicoDepuracaoAsync();
            await this.AtualizarEstadoServicoDepuracaoAsync();
            await this.DesocuparAsync();
            this._isInicializado = true;
            LogVSUtil.Log("Extensão inicializada");
        }

        private void BtnNormalizar_Click(object sender, RoutedEventArgs e)
        {
            _ = this.NormalizarProjetosReferenciasAsync();
        }

        public async Task NormalizarProjetosReferenciasAsync()
        {
            if (ConfiguracaoVSUtil.IsNormalizandoTodosProjetos ||
                this.IsOcupado)
            {
                return;
            }

            try
            {
                await this.OcuparAsync();
                this.Logs.Clear();
                ConfiguracaoVSUtil.IsNormalizandoTodosProjetos = true;
                await AjudanteNormalizarProjetos.NormalizarInternoAsync(false);
            }
            catch (Exception ex)
            {
                LogVSUtil.LogErro(ex);
            }
            finally
            {
                ConfiguracaoVSUtil.IsNormalizandoTodosProjetos = false;
                await this.DesocuparAsync();
            }
        }


        private void BtnReiniciarGerenciadorProjetosTS_Click(object sender, RoutedEventArgs e)
        {
            GerenciadorProjetos.Reiniciar();
            GerenciadorProjetos.Reiniciar();
        }

        private void BtnLimpar_Click(object sender, RoutedEventArgs e)
        {
            this.LimparLogs();
        }

        public void LimparLogs()
        {
            this.Logs.Clear();
        }

        private async void BtnIniciarPararServicoDepuracao_Click(object sender, RoutedEventArgs e)
        {
            this.BtnIniciarPararServicoDepuracao.IsEnabled = false;
            if (GerenciadorProjetos.EstadoServicoDepuracao == EnumEstadoServicoDepuracao.Ativo)
            {
                GerenciadorProjetos.Instancia.PararServicoDepuracao();
            }
            else
            {
                await GerenciadorProjetos.Instancia.IniciarServicoDepuracaoAsync();
            }
            await Task.Delay(3000);
        }


        public async Task AtualizarEstadoServicoDepuracaoAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            if (GerenciadorProjetos.EstadoServicoDepuracao == EnumEstadoServicoDepuracao.Ativo)
            {
                this.TxtPortaDepuracao.Text = $"Depuração porta: {this.PortaDepuracao}";
                this.BtnIniciarPararServicoDepuracao.Content = "Parar depuração";
            }
            else
            {
                this.TxtPortaDepuracao.Text = String.Empty;
                this.BtnIniciarPararServicoDepuracao.Content = "Iniciar depuração";
            }
            this.BtnIniciarPararServicoDepuracao.IsEnabled = true;
        }

        private void Link_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement element &&
                element.DataContext is LogMensagemViewModel logVM)
            {
                logVM.Acao?.Invoke();
            }
        }

        private   void BtnFormatarStringFormat_Click(object sender, RoutedEventArgs e)
        {
            _ = this.FormatarStringFormatAsync();
        }

        private async Task FormatarStringFormatAsync()
        {
            try
            {
                LogVSUtil.Logs?.Clear();
                await TsNewStringFormatUtil.FormatarNovoStringFormatAsync();
            }
            catch (Exception erro)
            {
                LogVSUtil.LogErro(erro);
            }
        }

        private async void BtnHtmlIntellisense_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var stopwatch = Stopwatch.StartNew();
                this.IsEnabled = false;
                await HtmlIntelliSense.InicializarAsync();
                await InstalarItensTemplate.InstalarAsync();

                LogVSUtil.Sucesso("IntelliSense inicializado, Reinicie o VisualStudio e configure a extensão .shtml para 'HTML (WebForms) Editor'", stopwatch);
            }
            catch (Exception ex)
            {
                LogVSUtil.LogErro(ex);
            }
            finally
            {
                this.IsEnabled = true;
            }
        }

        private void BtnDeletarBinAndObj_Click(object sender, RoutedEventArgs e)
        {
            _ = this.DeletarBinAndObjAsync();
        }

        private async Task DeletarBinAndObjAsync()
        {
            this.IsEnabled = false;
            try
            {
                var stopwatch = Stopwatch.StartNew();
                await SolutionUtil.DeletarBinAndObjAsync();
                LogVSUtil.Sucesso("Pastas bin e obj deletados com sucesso", stopwatch);
            }
            catch (Exception ex)
            {
                LogVSUtil.LogErro("Falha os excluir pasta Bin e Obj", ex);
            }
            finally
            {
                this.IsEnabled = true;
            }
        }

        internal async Task OcuparAsync()
        {
            this.IsOcupado = true;
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            this.IsEnabled = false;
            this.Cursor = Cursors.Wait;
        }

        internal async Task DesocuparAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            this.IsEnabled = true;
            this.Cursor = Cursors.Arrow;
            this.IsOcupado = false;
        }

    }
}