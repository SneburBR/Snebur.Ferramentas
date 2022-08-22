using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Snebur.Depuracao;
using Snebur.VisualStudio.Utilidade;

namespace Snebur.VisualStudio
{
    public class LogMensagemViewModel: ILogMensagemViewModel
    {
        public string Mensagem { get; }

        public Brush BrushColor { get; }

        public Action Acao { get; }

        public Cursor Cursor { get; }

        public TextDecorationCollection TextoDecoracao { get; }

        public Thickness Margem { get; }

        public LogMensagemViewModel(string mensagem, EnumTipoLog tipoLog) : this(mensagem, tipoLog, null)
        {

        }

        public LogMensagemViewModel(string mensagem, EnumTipoLog tipoLog, Action acao)
        {
            this.Mensagem = mensagem;
            this.BrushColor = this.RetornarBrushColor(tipoLog);
            this.Acao = acao;
            this.Cursor = this.RetornarCursor();
            this.TextoDecoracao = this.RetornarTextDecoration();
            this.Margem = this.RetornarMargem();
        }

        private TextDecorationCollection RetornarTextDecoration()
        {
            if (this.Acao != null)
            {
                return TextDecorations.Underline;
            }
            return null;
        }

        private Cursor RetornarCursor()
        {
            if (this.Acao != null)
            {
                return Cursors.Hand;
            }
            return null;
        }

        private Thickness RetornarMargem()
        {
            if (this.Acao != null)
            {
                return new Thickness(0, 2, 0, 2);
            }
            return new Thickness(0);
        }

        private Brush RetornarBrushColor(EnumTipoLog tipoLog)
        {
            switch (tipoLog)
            {
                case EnumTipoLog.Normal:

                    return Repositorio.BrushWindowText;

                case EnumTipoLog.Alerta:

                    return this.RetornarCorAlerta();

                case EnumTipoLog.Erro:

                    return Brushes.Red;

                case EnumTipoLog.Sucesso:

                    return Brushes.Green;

                case EnumTipoLog.Acao:

                    if (this.IsColorEscura(Repositorio.BrushWindowText.Color))
                    {
                        return Brushes.Turquoise;
                    }
                    else
                    {
                        return Brushes.RoyalBlue; 
                    }
                    

                default:

                    throw new Exception($"O tipo de log não é suportado");
            }
        }

        private Brush RetornarCorAlerta()
        {
            if (this.IsColorEscura(Repositorio.BrushWindowText.Color))
            {
                return Brushes.DarkOrange;
            }
            else
            {
                return Brushes.Orange;
            }
        }

        private bool IsColorEscura(Color color)
        {
            return color.R < 70 && color.B < 70 && color.G < 50;
        }
    }
}