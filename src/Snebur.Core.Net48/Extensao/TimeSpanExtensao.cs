using System;

namespace Snebur.Utilidade
{
    public static class TimeSpanExtensao
    {
        public static string Semantico(this TimeSpan timeSpan)
        {
            if (timeSpan.TotalDays > 0)
            {
                return $"{timeSpan.TotalDays:0} dias e {timeSpan.Hours:00}h";
            }
            if (timeSpan.TotalHours > 0)
            {
                return $"{timeSpan.TotalHours:0}h e {timeSpan.Minutes:00}m";
            }
            if (timeSpan.TotalMinutes > 0)
            {
                return $"{timeSpan.TotalMinutes:0}m";
            }
            return $"{timeSpan.TotalSeconds:0}s";
        }
    }
}
