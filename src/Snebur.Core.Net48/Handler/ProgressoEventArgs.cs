using System;

namespace Snebur
{
    public delegate void ProgressoEventHandler(object sender, ProgressoEventArgs e);

    public class ProgressoEventArgs : EventArgs
    {
        public double Progresso { get; set; }

        public ProgressoEventArgs(double progresso)
        {
            this.Progresso = progresso;
        }
    }
}