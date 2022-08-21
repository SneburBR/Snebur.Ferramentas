//using Microsoft.VisualStudio.Shell.Interop;
//using Snebur.Depuracao;
//using Snebur.Utilidade;
//using System.Collections.ObjectModel;
//using System.Diagnostics;
//using System.Text;
//using System.Windows;

//namespace Snebur.VisualStudio
//{
//    public class LogVSUtil
//    {
//        private static ILogVS _logVS;
//        public static ILogVS LogVS => ThreadUtil.RetornarValorComBloqueio(ref _logVS, () => new LogVS());

//       public static ObservableCollection<LogMensagemViewModel> Logs => LogVS.Logs;

//        public static bool IsNormalizandoTodosProjetos { get; set; }

//        public static int PortaDepuracao => throw new ErroNaoImplementado();
//        public static void Log(string mensagem)
//        {
//            LogVS.Log(mensagem);
//        }
//        public static void Log(string mensagem, EnumTipoLog tipoLog)
//        {
//            LogVS.Log(mensagem, tipoLog);
//        }
//        public static void Alerta(string mensagem)
//        {
//            LogVS.Alerta(mensagem);
//        }
//        public static void LogErro(string mensagem)
//        {
//            LogVS.LogErro(mensagem);
//        }
//        public static void LogErro(string mensagem, Exception erroInterno)
//        {
//            LogVS.LogErro(mensagem, erroInterno);
//        }
//        public static void LogErro(Exception erro)
//        {
//            LogVS.LogErro(erro);
//        }

//        public static void Sucesso(string mensagem, Stopwatch tempo, bool isPararTempo = true)
//        {
//            LogVS.Sucesso(mensagem, tempo, isPararTempo);
//        }
//    }

//    public interface ILogVS
//    {
//        ObservableCollection<LogMensagemViewModel> Logs { get; }

//        void Log(string mensagem);
//        void Log(string mensagem, EnumTipoLog tipoLog);
//        void Alerta(string mensagem);
//        void LogErro(string mensagem);
//        void LogErro(string mensagem, Exception erroInterno);
//        void LogErro(Exception erro);
//        void Sucesso(string mensagem, Stopwatch tempo, bool isPararTempo = true);
//    }

//    public class LogVS : ILogVS
//    {

//        private readonly Guid OUTPUT = Microsoft.VisualStudio.VSConstants.OutputWindowPaneGuid.GeneralPane_guid;
//        private readonly Guid DEBUG = Microsoft.VisualStudio.VSConstants.OutputWindowPaneGuid.DebugPane_guid;
//        private readonly Guid BUILD = Microsoft.VisualStudio.VSConstants.OutputWindowPaneGuid.BuildOutputPane_guid;

//        public ObservableCollection<LogMensagemViewModel> Logs { get; } = new ObservableCollection<LogMensagemViewModel>();

//        public bool IsNormalizandoTodosProjetos { get; set; }

//        public void Log(string mensagem)
//        {
//            this.LogInternoAsync(mensagem, EnumTipoLog.Normal);
//            //LogVSUtil.LogInterno(String.Format(mensagem, args), EnumTipoLog.Normal);
//        }

//        public void Log(string mensagem, EnumTipoLog tipoLog)
//        {
//            this.LogInternoAsync(mensagem, tipoLog);
//        }


//        public int PortaDepuracao { get; set; }

//        public void Sucesso(string mensagem, Stopwatch tempo, bool isPararTempo = true)
//        {
//            if (tempo != null)
//            {
//                if (isPararTempo && tempo.IsRunning)
//                {
//                    tempo.Stop();
//                }
//                var formatacao = tempo.ElapsedMilliseconds < 2 ? @"mm\:ss\.fffffff" : @"mm\:ss\.fff";
//                var tempoFormatado = TimeSpan.FromTicks(tempo.ElapsedTicks).ToString(formatacao);
//                mensagem = mensagem + " tempo: " + tempoFormatado;

//            }

//            this.LogInternoAsync(mensagem, EnumTipoLog.Sucesso);
//            this.OutputBuild("sucesso: " + mensagem);
//        }

//        public void Alerta(string mensagem)
//        {
//            this.LogInternoAsync(mensagem, EnumTipoLog.Alerta);
//        }

//        public void LogErro(string mensagem)
//        {
//            this.LogErro(new Exception(mensagem));
//        }

//        public void LogErro(string mensagem, Exception erroInterno)
//        {
//            this.LogErro(new Exception(mensagem, erroInterno));
//        }

//        internal void LogAcaoLink(string descricao, Action acao)
//        {
//            this.LogInternoAsync(descricao, EnumTipoLog.Acao, acao);
//        }

//        public void LogErro(Exception erro)
//        {
//            var erroAtual = erro;
//            var sb = new StringBuilder();
//            while (erroAtual != null)
//            {
//                sb.AppendLine(erroAtual.Message);
//                LogInternoAsync($"Tipo erro {erroAtual.GetType().Name} - {erroAtual.Message}", EnumTipoLog.Erro);

//                if (!String.IsNullOrWhiteSpace(erroAtual.StackTrace))
//                {
//                    LogInternoAsync(erroAtual.StackTrace, EnumTipoLog.Erro);
//                }
//                erroAtual = erroAtual.InnerException;
//            }
//            this.OutputBuild("erro: " + sb.ToString());
//        }

//        private void LogInternoAsync(string mensagem, EnumTipoLog tipLogo, Action acao = null)
//        {
//            Application.Current.Dispatcher.BeginInvoke(() =>
//            {
//                this.Logs?.Add(new LogMensagemViewModel(mensagem, tipLogo, acao));
//                OutputWindowControl.Instacia?.ScrollLog?.ScrollToBottom();
//            });
//        }

//        public async void ClearAsync()
//        {
//            _ =Application.Current.Dispatcher.BeginInvoke(() =>
//           {
//               this.Logs.Clear();
//           });
//        }



//        public void OutputDebug(string mensagem)
//        {
//            this.OutputString(mensagem, DEBUG);
//        }

//        public void OutputBuild(string mensagem)
//        {
//            this.OutputString(mensagem, BUILD);
//        }

//        public void OutputGeral(string mensagem)
//        {
//            this.OutputString(mensagem, OUTPUT);
//        }

//        private void OutputString(string text, Guid guidPane)
//        {
//            _ = OutputStringInternoAsync(text, guidPane);
//        }

//        private async Task OutputStringInternoAsync(string text, Guid guidPane)
//        {
//            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

//            const int VISIBLE = 1;
//            const int DO_NOT_CLEAR_WITH_SOLUTION = 0;

//            IVsOutputWindow outputWindow;
//            IVsOutputWindowPane outputWindowPane = null;
//            int hr;

//            // Get the output window
//            outputWindow = await GerenciadorProjetos.Instancia.GetServiceAsync(typeof(SVsOutputWindow)) as IVsOutputWindow;

//            // The General pane is not created by default. We must force its creation
//            if (guidPane == Microsoft.VisualStudio.VSConstants.OutputWindowPaneGuid.GeneralPane_guid)
//            {
//                hr = outputWindow.CreatePane(guidPane, "General", VISIBLE, DO_NOT_CLEAR_WITH_SOLUTION);
//                Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(hr);
//            }

//            // Get the pane
//            hr = outputWindow.GetPane(guidPane, out outputWindowPane);
//            try
//            {
//                Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(hr);
//                if (outputWindowPane != null)
//                {
//                    outputWindowPane.Activate();
//                    outputWindowPane.OutputString(text + "\r\n");
//                }
//            }
//            catch
//            {

//            }
//        }


//    }






//}


