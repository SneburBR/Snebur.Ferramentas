using System;

namespace Snebur.Utilidade
{

    public static class FormatarByteUtil
    {
        public const double TOTAL_BYTES_KB = 1024;
        public const double TOTAL_BYTES_MB = TOTAL_BYTES_KB * 1024;
        public const double TOTAL_BYTES_GB = TOTAL_BYTES_MB * 1024;
        public const double TOTAL_BYTES_TB = TOTAL_BYTES_GB * 1024;

        public static string Formatar(long totalBytes)
        {
            return Formatar(totalBytes, 2);
        }

        public static string Formatar(long totalBytes, int casasDecimais)
        {
            EnumFormatacaoBytes formato;
            if (totalBytes < TOTAL_BYTES_KB)
            {
                formato = EnumFormatacaoBytes.Bytes;
            }
            else if (totalBytes < TOTAL_BYTES_MB)
            {
                formato = EnumFormatacaoBytes.Kilobytes;
            }
            else if (totalBytes < TOTAL_BYTES_GB)
            {
                formato = EnumFormatacaoBytes.Megabytes;
            }
            else if (totalBytes < TOTAL_BYTES_TB)
            {
                formato = EnumFormatacaoBytes.Gigabytes;
            }
            else
            {
                formato = EnumFormatacaoBytes.Terabytes;
            }
            return Formatar(totalBytes, formato, casasDecimais);
        }

        public static string Formatar(long totalBytes, EnumFormatacaoBytes formato)
        {
            return Formatar(totalBytes, formato, 2);
        }

        public static string Formatar(long totalBytes, EnumFormatacaoBytes formato, int casasDecimais)
        {
            switch (formato)
            {
                case EnumFormatacaoBytes.Bytes:
                    return String.Format("{0} bytes", totalBytes);
                case EnumFormatacaoBytes.Kilobytes:
                    return String.Format("{0} Kb", FormatacaoUtil.FormatarDecimal(ConverterParaKB(totalBytes), casasDecimais));
                case EnumFormatacaoBytes.Megabytes:
                    return string.Format("{0} Mb", FormatacaoUtil.FormatarDecimal(ConverterParaMB(totalBytes), casasDecimais));
                case EnumFormatacaoBytes.Gigabytes:
                    return string.Format("{0} Gb", FormatacaoUtil.FormatarDecimal(ConverterParaGB(totalBytes), casasDecimais));
                case EnumFormatacaoBytes.Terabytes:
                    return string.Format("{0} Tb", FormatacaoUtil.FormatarDecimal(ConverterParaGB(totalBytes), casasDecimais));
                default:
                    throw new NotSupportedException("Formato não suportado.");
            }
        }

        public static double ConverterParaKB(long totalBytes)
        {
            return Convert.ToDouble(totalBytes / TOTAL_BYTES_KB);
        }

        public static double ConverterParaMB(long bytes)
        {
            return Convert.ToDouble(bytes / TOTAL_BYTES_MB);
        }

        public static double ConverterParaGB(long totalBytes)
        {
            return Convert.ToDouble(totalBytes / TOTAL_BYTES_GB);
        }

        public static double ConverterParaTB(long totalBytes)
        {
            return Convert.ToDouble(totalBytes / TOTAL_BYTES_TB);
        }
    }

    public enum EnumFormatacaoBytes
    {
        Bytes = 1,
        Kilobytes = 2,
        Megabytes = 3,
        Gigabytes = 4,
        Terabytes = 5
    }
}