using System.Data.Entity.Migrations;
using System.Linq;
using System.Reflection;

namespace Snebur.VisualStudio
{
    internal class AjudanteRetornarProximaMigracao
    {
        private Assembly AssemblyMigracao { get; }
        public string MensagemErro { get; private set; }
        public bool IsErro { get; private set; }

        public AjudanteRetornarProximaMigracao(Assembly assemblyMigracao)
        {
            this.AssemblyMigracao = assemblyMigracao;
        }


        public string RetornarProximaMigracao()
        {
            var assemblyMigracao = this.AssemblyMigracao;
            var tiposDbMigrations = assemblyMigracao.GetAccessibleTypes().Where(x => x.IsSubclassOf(typeof(DbMigration))).ToList();
            if(tiposDbMigrations.Count == 0)
            {
                return "1.0.0.0";
            }

            var tipoConfiguracao = assemblyMigracao.GetAccessibleTypes().Where(x => x.IsSubclassOf(typeof(DbMigrationsConfiguration))).Single();
            var instanciaConfiguracao = Activator.CreateInstance(tipoConfiguracao) as DbMigrationsConfiguration;
            var migrador = new DbMigrator(instanciaConfiguracao);
            var pendentes = migrador.GetPendingMigrations().ToList();

            if (pendentes.Count > 0)
            {
                throw new Exception($"Migração pendente {pendentes.Last()}");
            }
            Version ultimaVersao = this.RetornarUltimaVersao(migrador);
            if (ultimaVersao != null)
            {
                var proximaVersao = new Version(ultimaVersao.Major, ultimaVersao.Minor, ultimaVersao.Build, ultimaVersao.Revision + 1);
                return $"Versao_{proximaVersao.Major}.{proximaVersao.Minor}.{proximaVersao.Build}.{proximaVersao.Revision}";
            }
            return "Versao_1.0.0.0";
        }

        private Version RetornarUltimaVersao(DbMigrator migrador)
        {
            var migracoesExecuta = migrador.GetDatabaseMigrations();
            if (migracoesExecuta.Count() > 0)
            {
                var versoes = migracoesExecuta.Select(x => this.RetornarVersaoMigracao(x)).ToList();
                return versoes.OrderBy(x => x.Major).ThenBy(x => x.Minor).ThenBy(x => x.Build).ThenBy(x => x.Revision).Last();
            }
            return null;
        }

        private Version RetornarVersaoMigracao(string versaoMigracao)
        {
            if (Version.TryParse(versaoMigracao.Substring(versaoMigracao.LastIndexOf('_') + 1), out Version versao))
            {
                return versao;
            }
            throw new Exception($"Não foi possivel convertrer a string '{versaoMigracao}' numa tipo {nameof(Version)}");
        }
    }
}