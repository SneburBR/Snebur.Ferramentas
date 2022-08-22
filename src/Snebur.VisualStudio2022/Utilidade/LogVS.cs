using Microsoft.VisualStudio.Shell.Interop;
using Snebur.Depuracao;
using Snebur.Utilidade;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Windows;

namespace Snebur.VisualStudio
{


    //public class ConfiguracaoVSUtil
    //{
    //    public static bool IsNormalizandoTodosProjetos { get; set; }

    //    public static int PortaDepuracao { get; set; }
    //}

    public class LogVS : ILogVS
    {

        private static Guid OUTPUT => Microsoft.VisualStudio.VSConstants.OutputWindowPaneGuid.GeneralPane_guid;
        private static Guid DEBUG => Microsoft.VisualStudio.VSConstants.OutputWindowPaneGuid.DebugPane_guid;
        private static Guid BUILD => Microsoft.VisualStudio.VSConstants.OutputWindowPaneGuid.BuildOutputPane_guid;

        public ObservableCollection<ILogMensagemViewModel> Logs { get; } = new ObservableCollection<ILogMensagemViewModel>();

        private LogVS()
        {

        }
        public void Log(string mensagem)
        {
            this.LogInternoAsync(mensagem, EnumTipoLog.Normal);
            //LogVSUtil.LogInterno(String.Format(mensagem, args), EnumTipoLog.Normal);
        }

        public void Log(string mensagem, EnumTipoLog tipoLog)
        {
            this.LogInternoAsync(mensagem, tipoLog);
        }

        public void Sucesso(string mensagem, Stopwatch tempo, bool isPararTempo = true)
        {
            if (tempo != null)
            {
                if (isPararTempo && tempo.IsRunning)
                {
                    tempo.Stop();
                }
                var formatacao = tempo.ElapsedMilliseconds < 2 ? @"mm\:ss\.fffffff" : @"mm\:ss\.fff";
                var tempoFormatado = TimeSpan.FromTicks(tempo.ElapsedTicks).ToString(formatacao);
                mensagem = mensagem + " tempo: " + tempoFormatado;

            }

            this.LogInternoAsync(mensagem, EnumTipoLog.Sucesso);
            this.OutputBuild("sucesso: " + mensagem);
        }

        public void Alerta(string mensagem)
        {
            this.LogInternoAsync(mensagem, EnumTipoLog.Alerta);
        }

        public void LogErro(string mensagem)
        {
            this.LogErro(new Exception(mensagem));
        }

        public void LogErro(string mensagem, Exception erroInterno)
        {
            this.LogErro(new Exception(mensagem, erroInterno));
        }

        public void LogAcaoLink(string descricao, Action acao)
        {
            this.LogInternoAsync(descricao, EnumTipoLog.Acao, acao);
        }

        public void LogErro(Exception erro)
        {
            var erroAtual = erro;
            var sb = new StringBuilder();
            while (erroAtual != null)
            {
                sb.AppendLine(erroAtual.Message);
                LogInternoAsync($"Tipo erro {erroAtual.GetType().Name} - {erroAtual.Message}", EnumTipoLog.Erro);

                if (!String.IsNullOrWhiteSpace(erroAtual.StackTrace))
                {
                    LogInternoAsync(erroAtual.StackTrace, EnumTipoLog.Erro);
                }
                erroAtual = erroAtual.InnerException;
            }
            this.OutputBuild("erro: " + sb.ToString());
        }

        private void LogInternoAsync(string mensagem, EnumTipoLog tipLogo, Action acao = null)
        {
            Application.Current.Dispatcher.BeginInvoke(() =>
            {
                this.Logs?.Add(new LogMensagemViewModel(mensagem, tipLogo, acao));
                OutputWindowControl.Instacia?.ScrollLog?.ScrollToBottom();
            });
        }

        public void Clear()
        {
            Application.Current.Dispatcher.BeginInvoke(() =>
                       {
                           this.Logs.Clear();
                       });
        }



        public void OutputDebug(string mensagem)
        {
            this.OutputString(mensagem, DEBUG);
        }

        public void OutputBuild(string mensagem)
        {
            this.OutputString(mensagem, BUILD);
        }

        public void OutputGeral(string mensagem)
        {
            this.OutputString(mensagem, OUTPUT);
        }

        private void OutputString(string text, Guid guidPane)
        {
            _ = OutputStringInternoAsync(text, guidPane);
        }

        private async Task OutputStringInternoAsync(string text, Guid guidPane)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            const int VISIBLE = 1;
            const int DO_NOT_CLEAR_WITH_SOLUTION = 0;

            IVsOutputWindow outputWindow;
            IVsOutputWindowPane outputWindowPane = null;
            int hr;

            // Get the output window
            outputWindow = await GerenciadorProjetos.Instancia.GetServiceAsync(typeof(SVsOutputWindow)) as IVsOutputWindow;

            // The General pane is not created by default. We must force its creation
            if (guidPane == Microsoft.VisualStudio.VSConstants.OutputWindowPaneGuid.GeneralPane_guid)
            {
                hr = outputWindow.CreatePane(guidPane, "General", VISIBLE, DO_NOT_CLEAR_WITH_SOLUTION);
                Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(hr);
            }

            // Get the pane
            hr = outputWindow.GetPane(guidPane, out outputWindowPane);
            try
            {
                Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(hr);
                if (outputWindowPane != null)
                {
                    outputWindowPane.Activate();
                    outputWindowPane.OutputString(text + "\r\n");
                }
            }
            catch
            {

            }
        }

        private static LogVS _instance;
        internal static LogVS Instance => ThreadUtil.RetornarValorComBloqueio(ref _instance, () => new LogVS());

    }






}


