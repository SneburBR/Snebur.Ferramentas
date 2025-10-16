namespace System
{
    public class ByteSpan
    {
        public const long TOTAL_BYTES_KB = 1024;
        public const long TOTAL_BYTES_MB = TOTAL_BYTES_KB * 1024;
        public const long TOTAL_BYTES_GB = TOTAL_BYTES_MB * 1024;
        public const long TOTAL_BYTES_TB = TOTAL_BYTES_GB * 1024;

        public long TotalBytes { get; private set; }

        public double TotalMegaBytes
        {
            get
            {
                return this.TotalBytes / (double)TOTAL_BYTES_MB;
            }
        }

        public double TotalKilobytes
        {
            get
            {
                return this.TotalBytes / (double)TOTAL_BYTES_KB;
            }
        }

        public double TotalGibaBytes
        {
            get
            {
                return this.TotalBytes / (double)TOTAL_BYTES_GB;
            }
        }

        public double TotalTeraBytes
        {
            get
            {
                return this.TotalBytes / (double)TOTAL_BYTES_TB;
            }
        }

        public ByteSpan(long totalBytes)
        {
            this.TotalBytes = totalBytes;
        }

        public static ByteSpan FromBytes(long totalBytes)
        {
            return new ByteSpan(totalBytes);
        }

        public static ByteSpan FromKiloBytes(double totalKiloBytes)
        {
            long totalBytes = Convert.ToInt64(totalKiloBytes * TOTAL_BYTES_KB);
            return new ByteSpan(totalBytes);
        }

        public static ByteSpan FromMegaBytes(double totalMegaBytes)
        {
            long totalBytes = Convert.ToInt64(totalMegaBytes * TOTAL_BYTES_MB);
            return new ByteSpan(totalBytes);
        }

        public static ByteSpan FromGigaBytes(double totalGigaBytes)
        {
            long totalBytes = Convert.ToInt64(totalGigaBytes * TOTAL_BYTES_GB);
            return new ByteSpan(totalBytes);
        }

        public static ByteSpan FromTeraBytes(double totalTeraBytes)
        {
            long totalBytes = Convert.ToInt64(totalTeraBytes * TOTAL_BYTES_TB);
            return new ByteSpan(totalBytes);
        }
    }
}