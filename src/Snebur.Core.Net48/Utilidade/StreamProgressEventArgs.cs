using System;

namespace Snebur.Utilidade
{
    public class StreamProgressEventArgs : EventArgs
    {
        public long TotalBytesRecebitos { get; set; }
        public long TotalBytes { get; set; }

        public double Progresso
        {
            get
            {
                if (this.TotalBytes == 0)
                {
                    return 0;
                }
                return (double)this.TotalBytesRecebitos / this.TotalBytes;
            }
        }

        public StreamProgressEventArgs(long totalBytesRecebitos, long totalBytes)
        {
            this.TotalBytesRecebitos = totalBytesRecebitos;
            this.TotalBytes = totalBytes;
        }

    }
}