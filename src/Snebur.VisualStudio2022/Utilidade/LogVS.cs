﻿using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Threading;
using Snebur.Depuracao;
using Snebur.Utilidade;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;

namespace Snebur.VisualStudio
{

    public class LogVS : ILogVS
    {
        private bool? _isMostrarHoraLogOutput;
        private static Guid OUTPUT => Microsoft.VisualStudio.VSConstants.OutputWindowPaneGuid.GeneralPane_guid;
        private static Guid DEBUG => Microsoft.VisualStudio.VSConstants.OutputWindowPaneGuid.DebugPane_guid;
        private static Guid BUILD => Microsoft.VisualStudio.VSConstants.OutputWindowPaneGuid.BuildOutputPane_guid;

        public ObservableCollection<ILogMensagemViewModel> Logs { get; } = new ObservableCollection<ILogMensagemViewModel>();

        public bool IsMostrarHoraLogOutput
        {
            get
            {
                if (this._isMostrarHoraLogOutput == null)
                {
                    this._isMostrarHoraLogOutput = ConfiguracaoGeral.Instance.IsMostrarHoraLogOutput;
                }
                return this._isMostrarHoraLogOutput.Value;
            }
            set
            {
                this._isMostrarHoraLogOutput = value;
            }
        }


        private LogVS()
        {

        }
        public void Log(string mensagem)
        {
            this.LogInterno(mensagem, EnumTipoLog.Normal);
        }

        public void Log(string mensagem, Stopwatch tempo)
        {
            this.LogInterno(mensagem, EnumTipoLog.Normal, tempo);
        }

        public void Log(string mensagem, EnumTipoLog tipoLog)
        {
            this.LogInterno(mensagem, tipoLog);
        }

        public void Sucesso(string mensagem, Stopwatch tempo, bool isPararTempo = true)
        {
            if (tempo != null)
            {
                if (isPararTempo && tempo.IsRunning)
                {
                    tempo.Stop();
                }
            }

            this.LogInterno(mensagem, EnumTipoLog.Sucesso, tempo );
            this.OutputBuild("sucesso: " + mensagem);
        }

        private string FormatarTempo(string mensagem, Stopwatch tempo)
        {
            var formatacao = tempo.ElapsedMilliseconds < 2 ? @"mm\:ss\.fffffff" : @"mm\:ss\.fff";
            var tempoFormatado = TimeSpan.FromTicks(tempo.ElapsedTicks).ToString(formatacao);
            return  $"{mensagem}. tempo: { tempoFormatado} ";
        }

        public void Alerta(string mensagem)
        {
            this.LogInterno(mensagem, EnumTipoLog.Alerta);
        }

        public void Alerta(string mensagem, Stopwatch stopwatch)
        {
            this.LogInterno(mensagem, EnumTipoLog.Alerta, stopwatch);
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
            this.LogInterno(descricao, EnumTipoLog.Acao, acao);
        }

        public void LogErro(Exception erro)
        {
            var erroAtual = erro;
            var sb = new StringBuilder();
            while (erroAtual != null)
            {
                sb.AppendLine(erroAtual.Message);
                LogInterno($"Tipo erro {erroAtual.GetType().Name} - {erroAtual.Message}", EnumTipoLog.Erro);

                if (!String.IsNullOrWhiteSpace(erroAtual.StackTrace))
                {
                    LogInterno(erroAtual.StackTrace, EnumTipoLog.Erro);
                }
                erroAtual = erroAtual.InnerException;
            }
            this.OutputBuild("erro: " + sb.ToString());
        }

        private void LogInterno(string mensagem, EnumTipoLog tipLog)
        {
            LogInterno(mensagem, tipLog, null, null);
        }
        private void LogInterno(string mensagem, EnumTipoLog tipLog, Stopwatch stopwatch)
        {
            LogInterno(mensagem, tipLog, stopwatch, null);
        }

        private void LogInterno(string mensagem, EnumTipoLog tipLog, Action acao)
        {
            LogInterno(mensagem, tipLog, null, acao);
        }

        private void LogInterno(string mensagem, EnumTipoLog tipLog, Stopwatch tempo, Action acao)
        {
            if(tempo!= null)
            {
                mensagem = this.FormatarTempo(mensagem, tempo);
            }
            _ = this.LogInternoAsync(mensagem, tipLog, acao);
        }
      

        private async Task LogInternoAsync(string mensagem, EnumTipoLog tipoLog, Action acao)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            mensagem = this.IsMostrarHoraLogOutput ? $"{DateTime.Now:HH:mm:ss:fff} {mensagem}" : mensagem;
            this.Logs?.Add(new LogMensagemViewModel(mensagem, tipoLog, acao));
            OutputWindow.Instance?.ScrollLog?.ScrollToBottom();
        }

        public void Clear()
        {
            _ = this.ClearAsync();
        }

        private async Task ClearAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            OutputWindow.Instance?.ScrollLog?.ScrollToBottom();
            this.Logs.Clear();
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
                    outputWindowPane.OutputStringThreadSafe(text + "\r\n");
                }
            }
            catch
            {

            }
        }

        private static LogVS _instance;
        internal static LogVS Instance => LazyUtil.RetornarValorLazyComBloqueio(ref _instance, () => new LogVS());

    }






}


