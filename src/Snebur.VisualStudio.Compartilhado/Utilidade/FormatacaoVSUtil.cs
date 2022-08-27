using System;
using System.IO;

namespace Snebur.VisualStudio.Utilidade
{
    public static class FormatacaoVSUtil
    {
        private static string RetornarNomeTipoSemGenerico(string nomeTipo)
        {
            if (nomeTipo.Trim().Contains("<"))
            {
                var nomeTipoSemGenerecio = nomeTipo.Substring(0, nomeTipo.IndexOf("<"));
                return nomeTipoSemGenerecio;
            }
            return nomeTipo;
        }

        public static string RetornarNomeFormatado(string nomeTipo, FileInfo arquivo)
        {
            var nomeTipoFormatodo = nomeTipo.Trim();

            if (nomeTipoFormatodo.EndsWith("{"))
            {
                nomeTipoFormatodo = nomeTipoFormatodo.Substring(0, nomeTipoFormatodo.Length - 1).Trim();
            }
            if (nomeTipoFormatodo.StartsWith("."))
            {
                nomeTipoFormatodo = nomeTipoFormatodo.Substring(1);
            }
            if ((nomeTipoFormatodo.Contains(" ") || String.IsNullOrEmpty(nomeTipoFormatodo)))
            {
                throw new Exception(String.Format("O nometipo é invalido {0} em {1} ", nomeTipo, arquivo?.FullName));
            }
            var primeiraLetra = nomeTipoFormatodo.Substring(0, 1);
            if (primeiraLetra == primeiraLetra.ToLower())
            {
                throw new Exception($"Iniciar o nome da classe com caixa alta  {nomeTipo} - {arquivo?.FullName} ");
            }
            return RetornarNomeTipoSemGenerico(nomeTipoFormatodo);

        }
    }
}