using System;
using System.Collections.Generic;

using System.Linq;
using Snebur.Utilidade;

#if NET6_0_OR_GREATER
using Microsoft.Data.SqlClient;
#else
using System.Data.SqlClient;
#endif

namespace Snebur.BancoDados
{
    public class SqlParameterArray<T>
    {
        public string NomeCampo { get; }
        public string NomeParametro { get; }
        public List<T> Valores { get; }
        public SqlParameter[] Parametros { get; }
        public string[] ParametrosSql => this.Parametros.Select(x => x.ParameterName).ToArray();
        public string SqlFiltroIn => $" {this.NomeCampo} IN ({String.Join(", ", this.ParametrosSql)})";

        public SqlParameterArray(string nomeCampo, List<T> valores)
        {
            this.NomeCampo = nomeCampo;
            this.NomeParametro = TextoUtil.RetornarSomentesLetrasNumeros(nomeCampo);
            this.Valores = valores;
            this.Parametros = this.RetornarParametros();
        }

        private SqlParameter[] RetornarParametros()
        {
            var count = 0;
            var pametros = new List<SqlParameter>();
            foreach (var valor in this.Valores)
            {
                count++;
                var parametro = new SqlParameter($"@{this.NomeParametro}{count}", valor);
                pametros.Add(parametro);
            }
            return pametros.ToArray();
        }
    }
}