using System;
using System.Collections.Generic;
using System.Linq;

namespace Snebur.Utilidade
{
    public static class DataFeriadoUtil
    {
        public static List<DateTime> RetornarDatasFeriadosIntervalo(DateTime dataInicio, DateTime dataFim)
        {
            return RetornarFeriadosIntervalo(dataInicio, dataFim).Select(x => x.Data).ToList();
        }
        public static List<Feriado> RetornarFeriadosIntervalo(DateTime dataInicio, DateTime dataFim)
        {
            dataInicio = dataInicio.DataPrimeiraHoraDia();
            dataFim = dataFim.DataPrimeiraHoraDia();

            var anoInicio = Math.Min(dataInicio.Year, dataFim.Year);
            var anoFim = Math.Min(dataInicio.Year, dataFim.Year);
            var retorno = new List<Feriado>();
            for (var ano = anoInicio; ano <= anoFim; ano++)
            {
                var feriados = RetornarFeriados(ano);
                var filtro = feriados.Where(x => x.Data <= dataInicio && x.Data <= dataFim);
                retorno.AddRange(filtro);
            }
            return retorno;
        }
        public static List<Feriado> RetornarFeriados(int ano)
        {
            var feriados = new List<Feriado>();
            var pascoa = RetornarDataPascoa(ano);

            feriados.Add(new Feriado
            {
                Data = new DateTime(ano, 1, 1),
                Descricao = "Confraternização Universal"
            });

            feriados.Add(new Feriado
            {
                Data = pascoa.AddDays(-48),
                Descricao = "2º feira de Carnaval"
            });

            feriados.Add(new Feriado
            {
                Data = pascoa.AddDays(-47),
                Descricao = "Carnaval"
            });

            feriados.Add(new Feriado
            {
                Data = pascoa,
                Descricao = "Páscoa"
            });

            feriados.Add(new Feriado
            {
                Data = pascoa.AddDays(-2),
                Descricao = "6º feira Santa"
            });

            feriados.Add(new Feriado
            {
                Data = pascoa.AddDays(60),
                Descricao = "Corpus Cirist"
            });

            feriados.Add(new Feriado
            {
                Data = new DateTime(ano, 4, 21),
                Descricao = "Tiradentes"
            });

            feriados.Add(new Feriado
            {
                Data = new DateTime(ano, 5, 1),
                Descricao = "Dia do Trabalhador"
            });

            feriados.Add(new Feriado
            {
                Data = new DateTime(ano, 9, 7),
                Descricao = "Dia da Independência"
            });

            feriados.Add(new Feriado
            {
                Data = new DateTime(ano, 10, 12),
                Descricao = "N. S. Aparecida (dias das crianças)"
            });

            feriados.Add(new Feriado
            {
                Data = new DateTime(ano, 11, 2),
                Descricao = "Todos os santos (finados)"
            });

            feriados.Add(new Feriado
            {
                Data = new DateTime(ano, 11, 15),
                Descricao = "Proclamação da República"
            });

            feriados.Add(new Feriado
            {
                Data = new DateTime(ano, 12, 25),
                Descricao = "Natal"
            });

            return feriados;
        }

        public static DateTime RetornarDataPascoa(int ano)
        {
            int r1 = ano % 19;
            int r2 = ano % 4;
            int r3 = ano % 7;
            int r4 = (19 * r1 + 24) % 30;
            int r5 = (6 * r4 + 4 * r3 + 2 * r2 + 5) % 7;
            DateTime dataPascoa = new DateTime(ano, 3, 22).AddDays(r4 + r5);
            int dia = dataPascoa.Day;
            switch (dia)
            {
                case 26:
                    dataPascoa = new DateTime(ano, 4, 19);
                    break;
                case 25:
                    if (r1 > 10)
                        dataPascoa = new DateTime(ano, 4, 18);
                    break;
            }
            return dataPascoa.Date;

            //var a = ano % 19;
            //var b = ano % 4;
            //var c = ano % 7;
            //var d = (19 * a + 24) % 30;
            //var e = (2 * b + 4 * c + 6 * d + 5) % 7;
            //var soma = d + e;

            //int dia, mes;
            //if (soma > 9)
            //{
            //    dia = (d + e - 9);
            //    mes = 3;
            //}
            //else
            //{
            //    dia = (d + e + 22);
            //    mes = 2;
            //}
            //return new DateTime(ano, mes, dia);
        }
    }

    public class Feriado
    {
        public DateTime Data { get; internal set; }
        public string Descricao { get; internal set; }
    }
}
