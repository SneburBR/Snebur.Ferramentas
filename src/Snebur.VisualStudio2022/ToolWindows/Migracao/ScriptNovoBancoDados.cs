using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Snebur.VisualStudio
{
    class ScriptNovoBancoDados
    {
        public static List<string> RetornarScripts(string nomeConnectionString)
        {
            var nomeBancoDados = ScriptUtil.RetornarNomeBancoDados(nomeConnectionString);
            var novoBancoDadosScript = ScriptNovoBancoDados.RetornarScriptNovoBanco(nomeConnectionString);
            var scritps = new List<string>
            {
                novoBancoDadosScript,
                $"ALTER DATABASE [{nomeBancoDados}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;",
                $"ALTER DATABASE [{nomeBancoDados}] SET READ_COMMITTED_SNAPSHOT ON;",
                $"ALTER DATABASE [{nomeBancoDados}] SET ALLOW_SNAPSHOT_ISOLATION ON;",
                $"ALTER DATABASE [{nomeBancoDados}] SET MULTI_USER;"
            };
            return scritps;
        }
        private static string RetornarScriptNovoBanco(string nomeConnectionString)

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
            sb.AppendLine($@" ( NAME = N'{nomeBancoDados}', FILENAME = N'{caminhoArquivoDados}' , SIZE = 50MB , MAXSIZE = UNLIMITED, FILEGROWTH = 64MB )");
            sb.AppendLine($@"  LOG ON (NAME = N'{nomeBancoDados}_log', FILENAME = N'{caminhoArquivoLog}' , SIZE = 2MB , MAXSIZE = UNLIMITED , FILEGROWTH = 64MB ) ");
            sb.AppendLine(" COLLATE Latin1_General_CI_AI ");
            return sb.ToString();
        }

    }

}
