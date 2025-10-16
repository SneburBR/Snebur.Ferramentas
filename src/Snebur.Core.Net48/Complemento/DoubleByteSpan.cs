namespace System
{
    public class DoubleByteSpan
    {
        public const double TOTAL_BYTES_KB = 1024;
        public const double TOTAL_BYTES_MB = TOTAL_BYTES_KB * 1024;
        public const double TOTAL_BYTES_GB = TOTAL_BYTES_MB * 1024;
        public const double TOTAL_BYTES_TB = TOTAL_BYTES_GB * 1024;
        public const double TOTAL_BYTES_PB = TOTAL_BYTES_TB * 1024;
        public const double TOTAL_BYTES_HB = TOTAL_BYTES_PB * 1024;
        public const double TOTAL_BYTES_ZB = TOTAL_BYTES_HB * 1024d;
        public const double TOTAL_BYTES_YB = TOTAL_BYTES_ZB * 1024d;

        public double TotalBytes { get; private set; }
        public double TotalMegaBytes => this.TotalBytes / TOTAL_BYTES_MB;
        public double TotalKilobytes => this.TotalBytes / TOTAL_BYTES_KB;
        public double TotalGibaBytes => this.TotalBytes / TOTAL_BYTES_GB;
        public double TotalTeraBytes => this.TotalBytes / TOTAL_BYTES_TB;
        public double TotalPettaBytes => this.TotalBytes / TOTAL_BYTES_PB;
        public double TotalExaBytes => this.TotalBytes / TOTAL_BYTES_HB;
        public double TotalZettaBytes => this.TotalBytes / TOTAL_BYTES_ZB;
        public double TotalYottaBytes => this.TotalBytes / TOTAL_BYTES_YB;

        public DoubleByteSpan(double totalBytes)
        {
            this.TotalBytes = totalBytes;
        }

        public static DoubleByteSpan FromBytes(double totalBytes)
        {
            return new DoubleByteSpan(totalBytes);
        }

        public static DoubleByteSpan FromKiloBytes(double totalKiloBytes)
        {
            var totalBytes = Convert.ToUInt64(totalKiloBytes * TOTAL_BYTES_KB);
            return new DoubleByteSpan(totalBytes);
        }
        public static DoubleByteSpan FromMegaBytes(double totalMegaBytes)
        {
            var totalBytes = Convert.ToInt64(totalMegaBytes * TOTAL_BYTES_MB);
            return new DoubleByteSpan(totalBytes);
        }

        public static DoubleByteSpan FromGigaBytes(double totalGigaBytes)
        {
            var totalBytes = Convert.ToUInt64(totalGigaBytes * TOTAL_BYTES_GB);
            return new DoubleByteSpan(totalBytes);
        }

        public static DoubleByteSpan FromTeraBytes(double totalTeraBytes)
        {
            var totalBytes = Convert.ToUInt64(totalTeraBytes * TOTAL_BYTES_TB);
            return new DoubleByteSpan(totalBytes);
        }
    }
}