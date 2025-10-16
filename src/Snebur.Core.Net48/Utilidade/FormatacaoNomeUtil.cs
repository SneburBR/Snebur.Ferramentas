using System;
using System.Collections.Generic;
using System.Linq;

namespace Snebur.Utilidade
{
    public static class FormatacaoNomeUtil
    {
        private static readonly HashSet<string> ProposicoesNome = new HashSet<string>
        {
            "da",
            "de",
            "dos",
            "das"
        };

        public static string FormatarNome(string nome)
        {
            var partes = RetornarPartes(nome);
            return String.Join(" ", partes);
        }

        public static (string, string) FormatarNomeSobrenome(string nomeCompleto)
        {
            var partesNomeCompleto = RetornarPartes(nomeCompleto);

            var partesNome = new List<string>();
            var partesSobrenome = partesNomeCompleto.ToList();
            foreach (var parte in partesNomeCompleto)
            {
                partesNome.Add(parte);
                partesSobrenome.Remove(parte);

                if (!ValidacaoUtil.IsSomenteNumerosPontosSinais(parte))
                {
                    break;
                }
            }
            var primeiroNome = String.Join(" ", partesNome);
            var sobrenome = String.Join(" ", partesSobrenome);
            return (primeiroNome, sobrenome);
        }

        public static string FormatarNomeUltimoSobrenome(string nomeCompleto)
        {
            var partesNomeCompleto = RetornarPartes(nomeCompleto);
            //var nome = partesNomeCompleto.First();
            var partesNome = new List<string>();
            var partesSobrenome = partesNomeCompleto.ToList();
            foreach (var parte in partesNomeCompleto)
            {
                partesSobrenome.Remove(parte);
                if (!ValidacaoUtil.IsSomenteNumerosPontosSinais(parte))
                {
                    partesNome.Add(parte);
                    break;
                }
            }

            var primeiroNome = String.Join(" ", partesNome.Take(1));
            var sobrenome = partesSobrenome?.LastOrDefault() ?? String.Empty;
            return (primeiroNome.Trim() + " " + sobrenome.Trim()).Trim();
        }

        public static string FormatarNomeCompleto(string nome, string sobrenome)
        {
            var nomeCompleto = $"{nome} {sobrenome}";
            var partes = RetornarPartes(nomeCompleto);
            return UnirPartes(partes);
        }
        private static List<string> RetornarPartes(string nome)
        {
            if (!String.IsNullOrWhiteSpace(nome))
            {
                var partes = nome.Split(' ');
                return partes.Where(x => !String.IsNullOrEmpty(x)).Select(x => FormatarParte(x.Trim())).ToList();
            }
            return new List<string>();
        }

        private static string UnirPartes(List<string> partes)
        {
            var abreviacoes = partes.Where(x => IsAbreviacao(x)).ToList();
            if (abreviacoes.Count == 0)
            {
                return String.Join(" ", partes.Distinct());
            }
            var partesUnicas = new List<string>();
            foreach (var parte in partes)
            {
                if (IsAbreviacao(parte) ||
                    partesUnicas.Contains(parte))
                {
                    partesUnicas.Add(parte);
                }
            }
            return String.Join(" ", partes.Distinct());
        }

        private static bool IsAbreviacao(string parte)
        {
            return (parte.Length == 2 && parte[1] == '.');
        }

        private static string FormatarParte(string parte)
        {
            parte = parte.ToLower();
            if (ProposicoesNome.Contains(parte))
            {
                return parte;
            }
            return TextoUtil.RetornarPrimeiraLetraMaiuscula(parte);
        }
    }
}