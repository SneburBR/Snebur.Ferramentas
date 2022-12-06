using Snebur.Depuracao;
using Snebur.Utilidade;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Snebur.VisualStudio
{
    public static class LogVSUtil
    {
        public static ILogVS LogVS => BaseAplicacaoVisualStudio.Instancia.LogVS;

        public static ObservableCollection<ILogMensagemViewModel> Logs => LogVS.Logs;


        public static void Log(string mensagem)
        {
            LogVS.Log(mensagem);
        }
        public static void Log(string mensagem, EnumTipoLog tipoLog)
        {
            LogVS.Log(mensagem, tipoLog);
        }
        public static void Alerta(string mensagem)
        {
            LogVS.Alerta(mensagem);
        }
        public static void LogErro(string mensagem)
        {
            LogVS.LogErro(mensagem);
        }
        public static void LogErro(string mensagem, Exception erroInterno)
        {
            LogVS.LogErro(mensagem, erroInterno);
        }
        public static void LogErro(Exception erro)
        {
            LogVS.LogErro(erro);
        }

        public static void Sucesso(string mensagem, Stopwatch tempo, bool isPararTempo = true)
        {
            LogVS.Sucesso(mensagem, tempo, isPararTempo);
        }
        public static void LogAcaoLink(string descricao, Action acao)
        {
            LogVS.LogAcaoLink(descricao, acao);
        }
        public static void OutputGeral(string mensagem)
        {
            LogVS.OutputGeral(mensagem);
        }

        public static void Clear()
        {
            LogVS.Clear();
        }
    }

    public interface ILogVS
    {
        ObservableCollection<ILogMensagemViewModel> Logs { get; }

        void Log(string mensagem);
        void Log(string mensagem, EnumTipoLog tipoLog);
        void Alerta(string mensagem);
        void LogErro(string mensagem);
        void LogErro(string mensagem, Exception erroInterno);
        void LogErro(Exception erro);
        void Sucesso(string mensagem, Stopwatch tempo, bool isPararTempo = true);
        void LogAcaoLink(string descricao, Action acao);
        void OutputGeral(string mensagem);

        void Clear();
    }

}