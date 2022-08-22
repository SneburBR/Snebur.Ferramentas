//using Microsoft.VisualStudio.Shell;
//using Microsoft.VisualStudio.Shell.Interop;
//using System;
//using System.Collections.ObjectModel;
//using System.Diagnostics;
//using System.Text;
//using System.Windows;
//using System.Windows.Controls;
//using Snebur.Depuracao;

//namespace Snebur.VisualStudio
//{
//    public class MainTrhad
//    {

//    }

//    public class LogVSUtil
//    {
//        private static int _portaDepuracao;

//        private static readonly Guid OUTPUT = Microsoft.VisualStudio.VSConstants.OutputWindowPaneGuid.GeneralPane_guid;
//        private static readonly Guid DEBUG = Microsoft.VisualStudio.VSConstants.OutputWindowPaneGuid.DebugPane_guid;
//        private static readonly Guid BUILD = Microsoft.VisualStudio.VSConstants.OutputWindowPaneGuid.BuildOutputPane_guid;

//        public static ObservableCollection<LogMensagemViewModel> Logs { get; } = new ObservableCollection<LogMensagemViewModel>();



//        public static bool IsNormalizandoTodosProjetos { get; set; }






//        public static void Log(string mensagem)
//        {
//            LogVSUtil.LogInternoAsync(mensagem, EnumTipoLog.Normal);
//            //LogVSUtil.LogInterno(String.Format(mensagem, args), EnumTipoLog.Normal);
//        }

//        public static void Log(string mensagem, EnumTipoLog tipoLog)
//        {
//            LogVSUtil.LogInternoAsync(mensagem, tipoLog);
//        }


//        public static int PortaDepuracao { get => LogVSUtil._portaDepuracao; set => LogVSUtil._portaDepuracao = value; }

//        public static void Sucesso(string mensagem, Stopwatch tempo, bool isPararTempo= true)
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

//            LogInternoAsync(mensagem, EnumTipoLog.Sucesso);
//            LogVSUtil.OutputBuild("sucesso: " + mensagem);
//        }

//        public static void Alerta(string mensagem)
//        {
//            LogVSUtil.LogInternoAsync(mensagem, EnumTipoLog.Alerta);
//        }

//        public static void LogErro(string mensagem)
//        {
//            LogVSUtil.LogErro(new Exception(mensagem));
//        }

//        public static void LogErro(string mensagem, Exception erroInterno)
//        {
//            LogVSUtil.LogErro(new Exception(mensagem, erroInterno));
//        }

//        internal static void LogAcaoLink(string descricao, Action acao)
//        {
//            LogVSUtil.LogInternoAsync(descricao, EnumTipoLog.Acao, acao);
//        }


//        public static void LogErro(Exception erro)
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
//            LogVSUtil.OutputBuild("erro: " + sb.ToString());
//        }

//        private static void LogInternoAsync(string mensagem, EnumTipoLog tipLogo, Action acao = null)
//        {
//            Application.Current.Dispatcher.BeginInvoke(() =>
//            {
//                LogVSUtil.Logs?.Add(new LogMensagemViewModel(mensagem, tipLogo, acao));
//                OutputWindowControl.Instacia?.ScrollLog?.ScrollToBottom();
//            });
//        }

//        internal static void ClearAsync()
//        {
//            Application.Current.Dispatcher.BeginInvoke(() =>
//            {
//                LogVSUtil.Logs.Clear();
//            });
//        }



//        public static void OutputDebug(string mensagem)
//        {
//            LogVSUtil.OutputString(mensagem, DEBUG);
//        }

//        public static void OutputBuild(string mensagem)
//        {
//            LogVSUtil.OutputString(mensagem, BUILD);
//        }

//        public static void OutputGeral(string mensagem)
//        {
//            LogVSUtil.OutputString(mensagem, OUTPUT);
//        }

//        private static void OutputString(string text, Guid guidPane)
//        {
//            _ = OutputStringInternoAsync(text, guidPane);
//        }

//        private static async Task OutputStringInternoAsync(string text, Guid guidPane)
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


