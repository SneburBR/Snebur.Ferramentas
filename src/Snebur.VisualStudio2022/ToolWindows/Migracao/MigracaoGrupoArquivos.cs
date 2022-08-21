using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Snebur.Dominio;
using Snebur.Dominio.Atributos;
using Snebur.Utilidade;

namespace Snebur.VisualStudio
{
    public class ScriptGrupoArquivos
    {
        //public const string CAMINHO_BANCO_DADOS = @"C:\Program Files\Microsoft SQL Server\MSSQL13.SQLEXPRESS\MSSQL\DATA\";
        //public const string CAMINHO_BANCO_DADOS = @"C:\Program Files\Microsoft SQL Server\MSSQL12.SQLEXPRESS\MSSQL\DATA\";


        public static List<string> RetonarScripts(Assembly assemblyEntidades, string nomeConnectionString)
        {


            var atributosTabelas = assemblyEntidades.GetAccessibleTypes().Where(x => x.IsSubclassOf(typeof(Entidade))).
                                            Select(x => x.GetCustomAttribute<TabelaAttribute>(false)).
                                            Where(x => x != null && x.GetType() != typeof(TabelaAttribute)).ToList();

            var gruosArquivoDados = atributosTabelas.Select(x => x.GrupoArquivoDados).Where(x => x != null).ToList();

            var gruosArquivoIndioces = atributosTabelas.Select(x => x.GrupoArquivoIndices).Where(x => x != null).ToList();

            var nomesGrupoArquivos = new HashSet<string>();
            nomesGrupoArquivos.AddRange(gruosArquivoDados);
            nomesGrupoArquivos.AddRange(gruosArquivoIndioces);

            return RetonarScripts(nomesGrupoArquivos, nomeConnectionString);
        }


        public static List<string> RetonarScripts(HashSet<string> nomesGrupoArquivo, string nomeConnectionString)
        {
            var construtor = new SqlConnectionStringBuilder(ConfigurationManager.ConnectionStrings[nomeConnectionString].ConnectionString);
            var nomeBancoDados = construtor.InitialCatalog;

            var diretorioBancoDados = ScriptUtil.RetornarDiretorioBancoDados(nomeConnectionString);
            var scripts = new List<string>();

            foreach (var grupoArquivo in nomesGrupoArquivo)
            {
                if(grupoArquivo!= null)
                {
                    var nomeGrupoArquivo = TextoUtil.RetornarPrimeiraLetraMaiuscula(grupoArquivo);
                    scripts.AddRange(ScriptGrupoArquivos.RetornarScriptsAdicioanarFileGroup(diretorioBancoDados, nomeBancoDados, nomeGrupoArquivo));
                }
                
            }
            return scripts;
        }

        private static List<string> RetornarScriptsAdicioanarFileGroup(string diretorioBancoDados, string nomeBancoDados, string nomeGrupoArquivo)
        {
            var sqls = new List<string>();
            //var nomeGrupoArquivo = $"{nomeFonteDados}_ {grupoArquivo}";

            var nomeArquivo = $"{nomeBancoDados}_{nomeGrupoArquivo}.mdf";

            var caminhoArquivo = Path.Combine(diretorioBancoDados, nomeArquivo);

            var sql = $" IF NOT EXISTS(select * from sys.filegroups where name = '{nomeGrupoArquivo}')"
                            + $"\n      ALTER DATABASE[{nomeBancoDados}] ADD FILEGROUP [{nomeGrupoArquivo}];";

            sqls.Add(sql);
            sqls.Add(ScriptGrupoArquivos.RetornarSqlAdicioanarFileGroup(nomeBancoDados, nomeGrupoArquivo, caminhoArquivo));
            return sqls;
        }

        private static string RetornarSqlAdicioanarFileGroup(string nomeBancoDados, string nomeGrupoArquivo, string caminhoArquivo)
        {
            return $" IF NOT EXISTS(select * from sys.sysfiles where name = '{nomeGrupoArquivo}' )"
                             + $"\n   ALTER DATABASE [{nomeBancoDados}]"
                             + $"\n    ADD FILE ( "
                             + $"\n              NAME = [{nomeGrupoArquivo}], "
                             + $"\n              FILENAME = '{caminhoArquivo}', "
                             + $"\n              SIZE = 8MB, "
                             + $"\n              FILEGROWTH = 32MB  ) "
                             + $"\n TO FILEGROUP [{nomeGrupoArquivo}]; ";
        }

        

        public class BancoCaminho
        {
            public string NomeBancoDados { get; set; }
            public string CaminhoBanco { get; set; }
        }
    }
}
