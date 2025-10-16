using Snebur.Dominio.Atributos;
using System;

namespace Snebur.Utilidade
{
    public static class DateTimeExtensao
    {
        public static DateTime DataInicioUnix = new DateTime(1970, 1, 1, 0, 0, 0, 0);

        public static long RetornarHashValidacaoUnico(this DateTime dateTime)
        {
            var dias = (int)Math.Floor((dateTime - DataInicioUnix).TotalDays);
            return (long)new TimeSpan(dias, dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Millisecond).TotalMilliseconds;
        }

        public static DateTime RetornarDataComHoraZerada(this DateTime dataHora)
        {
            return new DateTime(dataHora.Year, dataHora.Month, dataHora.Day, 0, 0, 0);
        }

        public static long RetornarMilisegundosJavascript(this DateTime dataHora, bool isUtc = false)
        {
            if (isUtc && dataHora.Kind != DateTimeKind.Utc)
            {
                dataHora = dataHora.ToUniversalTime();
            }
            return DataHoraUtil.RetornarMiliesegundosJavascript(dataHora, isUtc);
        }

        public static bool Equals(this DateTime? data,
                                  DateTime? dataComparar,
                                  OpcoesCompararData opcoesData,
                                  OpcoesCompararHora opcoesHora = OpcoesCompararHora.Ignorar)
        {
            if (data.HasValue && dataComparar.HasValue)
            {
                return DataHoraUtil.SaoIgual(data.Value,
                                             dataComparar.Value,
                                             opcoesData,
                                             opcoesHora);
            }
            return false;
        }
        public static bool Equals(this DateTime data,
                                  DateTime? dataComparar,
                                  OpcoesCompararData opcoesData,
                                  OpcoesCompararHora opcoesHora = OpcoesCompararHora.Ignorar)
        {
            if (dataComparar.HasValue)
            {
                return DataHoraUtil.SaoIgual(data,
                                             dataComparar.Value,
                                             opcoesData,
                                             opcoesHora);
            }
            return false;
        }

        public static DateTime DataPrimeiraHoraDia(this DateTime data, bool isUtc = true)
        {
            var tipo = isUtc ? DateTimeKind.Utc : DateTimeKind.Local;
            return DataPrimeiraHoraDia(data, tipo);
        }
        public static DateTime DataPrimeiraHoraDia(this DateTime data, DateTimeKind kind)
        {
            return new DateTime(data.Year, data.Month, data.Day, 0, 0, 0, kind);
        }

        public static DateTime DataUltimaHora(this DateTime data, bool isUtc = true)
        {
            var tipo = isUtc ? DateTimeKind.Utc : DateTimeKind.Local;
            return DataUltimaHora(data, tipo);
        }

        public static DateTime DataUltimaHora(this DateTime data, DateTimeKind kind)
        {
            return new DateTime(data.Year, data.Month, data.Day, 23, 59, 59, kind);
        }

        public static string ToDirectoryName(this DateTime dateTime,
                                             OptionsDateTimeDiretoryName options = OptionsDateTimeDiretoryName.YYYY_MM_DD)
        {
            switch (options)
            {
                case OptionsDateTimeDiretoryName.YYYY_MM_DD:
                    return dateTime.ToString("yyyy-MM-dd");
                case OptionsDateTimeDiretoryName.YYYY_MM_DD_HH_mm:
                    return dateTime.ToString("yyyy-MM-dd HH-mm");
                case OptionsDateTimeDiretoryName.YYYY_MM_DD_HH_MM_SS:
                    return dateTime.ToString("yyyy-MM-dd HH-mm-ss");
                case OptionsDateTimeDiretoryName.YYYY_MM_DD_HH_MM_SS_FFF:
                    return dateTime.ToString("yyyy-MM-dd HH-mm-ss-fff");
                case OptionsDateTimeDiretoryName.YYYY_MM_DD_HH_MM_SS_FFFFFF:
                    return dateTime.ToString("yyyy-MM-dd HH-mm-ss-ffffff");
                default:
                    throw new Exception($"Options {options} não suportado");

            }
        }
    }

    [IgnorarEnumTS]
    [IgnorarTSReflexao]
    public enum OptionsDateTimeDiretoryName
    {
        YYYY_MM_DD,
        YYYY_MM_DD_HH_mm,
        YYYY_MM_DD_HH_MM_SS,
        YYYY_MM_DD_HH_MM_SS_FFF,
        YYYY_MM_DD_HH_MM_SS_FFFFFF,
    }
}
