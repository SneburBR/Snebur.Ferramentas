using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snebur.VisualStudio
{
    class ScriptNovoBancoDados
    {
       public static string RetornarScript(string nomeConnectionString)
        {
            var nomeBancoDados = ScriptUtil.RetornarNomeBancoDados(nomeConnectionString);
            var diretorioBancoDados = ScriptUtil.RetornarDiretorioBancoDados(nomeConnectionString);

            var caminhoArquivoDados = Path.Combine(diretorioBancoDados, $"{nomeBancoDados}.mdf");
            var caminhoArquivoLog = Path.Combine(diretorioBancoDados, $"{nomeBancoDados}.ldf");


            //var caminhoBancoDados = 
            var sb = new StringBuilder();
            sb.AppendLine($" CREATE DATABASE[{nomeBancoDados}]");
            sb.AppendLine("         CONTAINMENT = NONE ");
            sb.AppendLine("            ON PRIMARY ");
            sb.AppendLine($@" ( NAME = N'{nomeBancoDados}', FILENAME = N'{caminhoArquivoDados}' , SIZE = 50MB , MAXSIZE = UNLIMITED, FILEGROWTH = 64MB ");
            sb.AppendLine("  )");
            sb.AppendLine($@"  LOG ON (NAME = N'{nomeBancoDados}_log', FILENAME = N'{caminhoArquivoLog}' , SIZE = 2MB , MAXSIZE = UNLIMITED , FILEGROWTH = 64MB ) ");

            return sb.ToString();
 

        }

    }
     
}
