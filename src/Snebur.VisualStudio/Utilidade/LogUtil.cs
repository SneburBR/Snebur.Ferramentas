using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snebur;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace Snebur.VisualStudio.Utilidade
{
    public class LogMensagemUtil
    {
        public static ObservableCollection<LogMensagemViewModel> Logs { get; set; }

        public static  ScrollViewer ScrollLog { get; set; }

        public static void Log(string mensagem, params string[] args)
        {
            Log(String.Format(mensagem, args), false);
        }

        public static void LogErro(Exception erro)
        {
            var erroAtual = erro;
            while(erroAtual!= null)
            {
                Log(erro.Message, true);

                if (!String.IsNullOrWhiteSpace(erro.StackTrace))
                {
                    Log(erro.StackTrace, true);
                }
                erroAtual = erroAtual.InnerException;
            }
        }

        private static void Log(string mensagem, bool erro )
        {
            if (!Application.Current.Dispatcher.CheckAccess())
            {
                Application.Current.Dispatcher.Invoke(() => {
                    LogMensagemUtil.Log(mensagem, erro);
                });
            }
            else
            {
                LogMensagemUtil.Logs.Add(new LogMensagemViewModel(mensagem, erro));
                LogMensagemUtil.ScrollLog.ScrollToBottom();
            }
        }
    }
}
