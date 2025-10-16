using Snebur.Dominio;
using Snebur.Dominio.Atributos;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Snebur.Utilidade
{
    public static class DataHoraUtil
    {
        private static readonly long TICKS_JAVASCRIPT_UTC = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks;
        private static readonly long TICKS_JAVASCRIPT = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local).Ticks;
        public static DateTime CalcularDataUtil(int diasUtil)
        {
            return CalcularDataUtil(DateTime.Today, diasUtil);
        }
        public static DateTime CalcularDataUtil(DateTime inicio, int diasUtil)
        {
            var c = 0;
            var data = inicio;
            var feriados = DataFeriadoUtil.RetornarDatasFeriadosIntervalo(inicio, inicio.AddDays(diasUtil * 3));
            while (true)
            {
                data.AddDays(1);
                if (IsDiaUtil(data, feriados))
                {
                    c += 1;
                }
                if (c >= diasUtil)
                {
                    break;
                }
            }
            return data;
        }

        public static DateTime? RetornarDataHoraMiliesegundosJavascript(long? minilegundos, bool isUtc = true)
        {
            if (minilegundos.HasValue)
            {
                return RetornarDataHoraMiliesegundosJavascript(minilegundos.Value, isUtc);
            }
            return null;

        }
        public static DateTime RetornarDataHoraMiliesegundosJavascript(long minilegundos, bool isUtc = true)
        {
            var ticks = isUtc ? TICKS_JAVASCRIPT_UTC :
                                TICKS_JAVASCRIPT;

            return new DateTime(ticks + (minilegundos * 10000), isUtc ? DateTimeKind.Utc : DateTimeKind.Local);
        }

        public static long RetornarMiliesegundosJavascript(DateTime dataHora, bool isUtc = false)
        {
            //var dataInicial = new DateTime(1970, 1, 1);
            var ticks = isUtc ? TICKS_JAVASCRIPT_UTC :
                                 TICKS_JAVASCRIPT;

            var ts = new TimeSpan(dataHora.Ticks - ticks);
            return (long)ts.TotalMilliseconds;
        }

        public static int RetornarTotalDiasUtieis(DateTime dataInicio, DateTime dataFim)
        {
            var totalDiasNaoUteis = RetornarTotalDiasNaoUtieis(dataInicio, dataFim);
            return (dataInicio - dataFim).Days - totalDiasNaoUteis;
        }

        public static int RetornarTotalDiasNaoUtieis(DateTime dataInicio, DateTime dataFim)
        {
            var feriados = DataFeriadoUtil.RetornarDatasFeriadosIntervalo(dataInicio, dataFim.AddDays(10));
            return EachDay(dataInicio, dataFim).Where(x => IsDiaUtil(x, feriados)).Count();
        }

        private static bool IsDiaUtil(DateTime data, List<DateTime> feriados)
        {
            return !(data.DayOfWeek == DayOfWeek.Sunday ||
                     data.DayOfWeek == DayOfWeek.Saturday ||
                     feriados.Contains(data, new DiaMesAnoIgual()));
        }

        public static IEnumerable<DateTime> EachDay(DateTime from, DateTime thru)
        {
            for (var day = from.Date; day.Date <= thru.Date; day = day.AddDays(1))
            {
                yield return day;
            }
        }

        public static bool SaoIgual(DateTime dataHora1, DateTime dataHora2)
        {
            return SaoIgual(dataHora1, dataHora2, OpcoesCompararData.Data);
        }

        public static bool SaoIgual(DateTime dataHora1, DateTime dataHora2, OpcoesCompararData opcoesData)
        {
            return SaoIgual(dataHora1, dataHora2, opcoesData, OpcoesCompararHora.Padrao);
        }

        public static bool SaoIgual(DateTime dataHora1, DateTime dataHora2, OpcoesCompararHora opcoesHora)
        {
            return SaoIgual(dataHora1, dataHora2, OpcoesCompararData.Data, opcoesHora);
        }

        public static bool SaoIgual(DateTime dataHora1, DateTime dataHora2, OpcoesCompararData opcoesData, OpcoesCompararHora opcoesHora)
        {
            var resultado = true;

            switch (opcoesData)
            {
                case OpcoesCompararData.Data:

                    resultado = dataHora1.Year == dataHora2.Year &&
                                dataHora1.Month == dataHora2.Month &&
                                dataHora1.Day == dataHora2.Day;
                    break;

                case OpcoesCompararData.Dia:

                    resultado = dataHora1.Day == dataHora2.Day;
                    break;

                case OpcoesCompararData.DiaMes:

                    resultado = dataHora1.Day == dataHora2.Day &&
                                dataHora1.Month == dataHora2.Month;
                    break;

                case OpcoesCompararData.MesAno:

                    resultado = dataHora1.Year == dataHora2.Year &&
                                dataHora1.Month == dataHora2.Month;
                    break;
                case OpcoesCompararData.Ignorar:

                    resultado = true;
                    break;

                default:

                    throw new NotSupportedException(String.Format("Opções data não suportado {0] ", EnumUtil.RetornarDescricao(opcoesData)));
            }
            if (!resultado)
            {
                return resultado;
            }
            switch (opcoesHora)
            {
                case OpcoesCompararHora.Padrao:
                case OpcoesCompararHora.HoraMinutoSegundo:

                    resultado = dataHora1.Hour == dataHora2.Hour &&
                                dataHora1.Minute == dataHora2.Minute &&
                                dataHora1.Second == dataHora2.Second;

                    break;

                case OpcoesCompararHora.HoraMinuto:

                    resultado = dataHora1.Hour == dataHora2.Hour &&
                                dataHora1.Minute == dataHora2.Minute;

                    break;

                case OpcoesCompararHora.HoraMinutoSegundosMilesegundos:

                    resultado = dataHora1.Hour == dataHora2.Hour &&
                                dataHora1.Minute == dataHora2.Minute &&
                                dataHora1.Second == dataHora2.Second &&
                                dataHora1.Millisecond == dataHora2.Millisecond;
                    break;

                case OpcoesCompararHora.Ignorar:

                    resultado = true;
                    break;

                default:

                    throw new NotSupportedException(String.Format("Opções hora não suportado {0] ", EnumUtil.RetornarDescricao(opcoesHora)));
            }
            return resultado;
        }

        public static DateTime NovaDataVencimento(DateTime dataVencimento,
                                                  int diaVencimento,
                                                  int meses)
        {
            dataVencimento = dataVencimento.AddMonths(meses);
            return NormalizarDataVencimento(dataVencimento, diaVencimento);
        }
        public static DateTime NormalizarDataVencimento(DateTime dataVencimento,
                                                    int diaVencimento)
        {
            if (diaVencimento > 28 || diaVencimento != dataVencimento.Day)
            {
                var ultimoDiaMes = DateTime.DaysInMonth(dataVencimento.Year, dataVencimento.Month);
                if (diaVencimento > ultimoDiaMes || diaVencimento != dataVencimento.Day)
                {
                    return new DateTime(dataVencimento.Year,
                                        dataVencimento.Month,
                                        ultimoDiaMes,
                                        dataVencimento.Hour,
                                        dataVencimento.Minute,
                                        dataVencimento.Second,
                                        dataVencimento.Kind);
                }

            }
            return dataVencimento;
        }

        public static int RetornarUltimoDiaMes(int ano, EnumMes mes)
        {
            return DateTime.DaysInMonth(ano, (int)mes);
        }

        public static DateTime RetornarDataMinima(EnumTipoData tipoData)
        {
            var hoje = DateTime.Today;
            switch (tipoData)
            {
                case EnumTipoData.Normal:
                case EnumTipoData.DataMuitoPassado:

                    return hoje.AddYears(-100);

                case EnumTipoData.DataNascimento:

                    return hoje.AddYears(-120);

                case EnumTipoData.DataFuturaProxima:
                case EnumTipoData.DataFutura:
                case EnumTipoData.DataMuitoFutura:

                    return hoje;

                case EnumTipoData.DataPassadoRecente:
                case EnumTipoData.DataPassadoFuturoProximo:
                    return hoje.AddYears(-5);
                case EnumTipoData.DataPassado:
                case EnumTipoData.DataPassadoFuturo:
                    return hoje.AddYears(-20);

                default:
                    throw new Exception("tipo de data não suportado");
            }
        }

        public static DateTime RetornarDataMaxima(EnumTipoData tipoData)
        {
            var hoje = DateTime.Now;
            switch (tipoData)
            {
                case EnumTipoData.Normal:
                case EnumTipoData.DataMuitoFutura:
                    return hoje.AddYears(+100);
                case EnumTipoData.DataNascimento:
                case EnumTipoData.DataPassado:
                case EnumTipoData.DataPassadoRecente:
                case EnumTipoData.DataMuitoPassado:
                    return hoje;
                case EnumTipoData.DataFuturaProxima:
                case EnumTipoData.DataPassadoFuturoProximo:
                    return hoje.AddYears(5);
                case EnumTipoData.DataFutura:
                case EnumTipoData.DataPassadoFuturo:
                    return hoje.AddYears(20);
                default:
                    throw new Exception("tipo de data não suportado");
            }
        }

    }
    internal class DiaMesIgual : IEqualityComparer<DateTime>
    {
        public DiaMesIgual()
        {
        }

        public bool Equals(DateTime x, DateTime y)
        {
            return x.Month == y.Month && x.Day == y.Day;
        }

        public int GetHashCode(DateTime obj)
        {
             return HashCode.Combine(obj.Day,
                                    obj.Month);
 
        }
    }

    internal class DiaMesAnoIgual : IEqualityComparer<DateTime>
    {
        public DiaMesAnoIgual()
        {
        }

        public bool Equals(DateTime x, DateTime y)
        {
            return x.Month == y.Month && x.Day == y.Day && x.Year == y.Year;
        }

        public int GetHashCode(DateTime obj)
        {
            return HashCode.Combine(obj.Day,
                                    obj.Month,
                                    obj.Year);
            //return obj.Day * 100 + obj.Month * 100 + obj.Year * 1000;
        }
    }
    public enum OpcoesCompararData
    {
        /// <summary>
        /// Dia, Mes e Ano
        /// </summary>
        Data = 1,
        //DiasMesAno = Data,
        Dia = 2,
        DiaMes = 3,
        MesAno = 4,
        Ignorar = 5
    }

    public enum OpcoesCompararHora
    {
        Padrao = 1,
        HoraDoDia = 2,
        HoraMinuto = 3,
        HoraMinutoSegundo = 4,
        HoraMinutoSegundosMilesegundos = 5,
        Ignorar = 7,
    }
}

namespace Snebur.Dominio
{
    public enum EnumMes
    {
        Janeiro = 1,
        Fevereiro = 2,
        [Rotulo("Março")]
        Marco = 3,
        Abril = 4,
        Maio = 5,
        Junho = 6,
        Julho = 7,
        Agosto = 8,
        Setembro = 9,
        Outubro = 10,
        Novembro = 11,
        Dezembro = 12
    }

    public enum EnumTipoData
    {
        Normal,
        DataPassadoFuturoProximo,
        DataPassadoFuturo,
        DataNascimento,
        DataFuturaProxima,
        DataFutura,
        DataMuitoFutura,
        DataPassadoRecente,
        DataPassado,
        DataMuitoPassado,
    }

    public enum EnumDiaSemana
    {
        [Rotulo("Domingo")]
        Domingo = 0,
        [Rotulo("Segunda-feira")]
        SegundaFeira = 1,
        [Rotulo("Terça-feira")]
        TercaFeira = 2,
        [Rotulo("Quarta-feira")]
        QuartaFeira = 3,
        [Rotulo("Quinta-feira")]
        QuintaFeira = 4,
        [Rotulo("Sexta-feira")]
        SextaFeira = 5,
        [Rotulo("Sábado")]
        Sabado = 6,
    }
}