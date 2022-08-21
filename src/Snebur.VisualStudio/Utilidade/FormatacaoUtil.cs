using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snebur;
using System.IO;

namespace Snebur.VisualStudio.Utilidade
{
    public class FormatacaoUtil
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
                throw new Exception(String.Format("O nometipo é invalido {0} em {1} ", nomeTipo, arquivo.FullName));
            }

            var primeiraLetra = nomeTipoFormatodo.Substring(0, 1);
            if (primeiraLetra == primeiraLetra.ToLower())
            {
                throw new Exception(String.Format("Iniciar o nome da classe com caixa alta  {0} - ", nomeTipo, arquivo.FullName));
            }



            return RetornarNomeTipoSemGenerico(nomeTipoFormatodo);



        }

        public static string RetornarCaminhoRelativo(FileInfo arquivo, string caminhoProjeto)
        {
            var caminhoArquivo = arquivo.FullName;
            var caminhoRelativo = arquivo.FullName.Substring(caminhoProjeto.Length + 1);
            caminhoRelativo = caminhoRelativo.Replace("\\", "/");

            if (!caminhoRelativo.StartsWith("TypeScript"))
            {
              //  throw new Exception("Caminho relativo invalido");
            }
            return caminhoRelativo;
        }

    }
}
