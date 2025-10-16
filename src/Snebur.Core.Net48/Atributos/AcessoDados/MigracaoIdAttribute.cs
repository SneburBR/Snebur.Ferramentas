using System;

namespace Snebur.Dominio.Atributos
{
    public class MigracaoIdAttribute : Attribute
    {
        public string MigrationId { get; }

        public MigracaoIdAttribute(string migrationId)
        {
            this.MigrationId = migrationId;
        }

    }
}
