using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Snebur.VisualStudio
{
    public class LogMensagemUtil
    {

        public string Mensagem { get; set; }

        public Brush BrushColor { get; set; }

        public LogMensagemUtil(string mensagem, bool erro = false)
        {
            this.Mensagem = mensagem;
            this.BrushColor = (erro) ? Brushes.Red : Repositorio.BrushWindowText;
        }
    }
}
