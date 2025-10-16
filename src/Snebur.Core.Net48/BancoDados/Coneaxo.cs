using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;

using System.Linq;
using System.Reflection;
using Snebur.Utilidade;
using Snebur.Dominio;
using System.Data.Common;
using System.ComponentModel.DataAnnotations.Schema;


#if NET6_0_OR_GREATER
using Microsoft.Data.SqlClient;
using Snebur;
using Snebur.BancoDados;
#else
using System.Data.SqlClient;
#endif

namespace Snebur.BancoDados
{
    public class Conexao : IDisposable
    {
        public string NomeConnectionString;
        private string ConnectionString;

        public int? CommandTimeout { get; set; }

        public Conexao(string nomeConnectionString)
        {
            this.NomeConnectionString = nomeConnectionString;
            
            if (AplicacaoSnebur.Atual?.ConnectionStrings[nomeConnectionString] != null)
            {
                this.ConnectionString = AplicacaoSnebur.Atual.ConnectionStrings[nomeConnectionString];
            }
            else
            {
               
                this.ConnectionString = nomeConnectionString;
            }

            if (!this.IsConnecionStringValida(this.ConnectionString))
            {
                throw new Exception($"A string de conexão '{this.ConnectionString}' não é válida");
            }
        }

        private bool IsConnecionStringValida(string connectionString)
        {
            try
            {
                var builder = new DbConnectionStringBuilder
                {
                    ConnectionString = connectionString
                };
                return true;
            }
            catch (ArgumentException)
            {
                return false;
            }
        }

        private DataTable RetornarDataTable(string sql,
                                          List<PropertyInfo> propriedadesChavePrimaria,
                                          SqlParameter[] parametros)
        {
            var dt = new DataTable();
            using (var conexao = new SqlConnection(this.ConnectionString))
            {
                conexao.Open();
                try
                {
                    using (var cmd = new SqlCommand(sql, conexao))
                    {
                        foreach (var parametro in parametros)
                        {
                            cmd.Parameters.Add(parametro);
                        }
                        cmd.CommandTimeout = TimeSpan.FromMinutes(5).Seconds;
                        using (var ad = new SqlDataAdapter(cmd))
                        {
                            ad.Fill(dt);
                        }
                    }
                    if (propriedadesChavePrimaria != null && propriedadesChavePrimaria.Count > 0)
                    {
                        var chavesPrimerias = propriedadesChavePrimaria.Select(x => dt.Columns[x.Name]).ToArray();
                        dt.PrimaryKey = chavesPrimerias;
                    }
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    conexao.Close();
                }
            }
            return dt;
        }

        //public List<T> Mapear<T>(string sql, params SqlParameter[] parametros)
        //{
        //    return this.Mapear<T>(sql, parametros);
        //}

        public List<T> Mapear<T>(string sql, params SqlParameter[] parametros)
        {
            var tipo = typeof(T);
            var propriedades = tipo.GetProperties(BindingFlags.Public | BindingFlags.Instance).
                                    Where(x => x.DeclaringType != typeof(BaseDominio) &&
                                               x.GetCustomAttribute<NotMappedAttribute>(true) == null &&
                                               x.GetGetMethod().IsPublic && (x.GetSetMethod()?.IsPublic ?? false));
            
            var propriedadesChavePrimaria = tipo.GetProperties().
                                                 Where(x => x.GetCustomAttribute<KeyAttribute>() != null  ).
                                                 ToList();

            var dataTable = this.RetornarDataTable(sql,
                                                   propriedadesChavePrimaria,
                                                   parametros);

            var retorno = new List<T>();

            foreach (DataRow row in dataTable.Rows)
            {
                var item = Activator.CreateInstance<T>();
                if (propriedades.Any())
                {
                    foreach (var propriedade in propriedades)
                    {
                        var valor = row[propriedade.Name];
                        var valorTipado = ConverterUtil.Converter(valor, propriedade.PropertyType);
                        propriedade.SetValue(item, valorTipado);
                    }
                    retorno.Add(item);
                }
                else
                {
                    retorno.Add((T)Convert.ChangeType(row[0], typeof(T)));
                }

            }
            return retorno;
        }

        public T RetornarValorScalar<T>(string sql,
                                       params SqlParameter[] parametros)
        {
            object valorEscalor;
            using (var conexao = new SqlConnection(this.ConnectionString))
            {
                conexao.Open();
                try
                {
                    using (var cmd = new SqlCommand(sql, conexao))
                    {
                        foreach (var parametro in parametros)
                        {
                            cmd.Parameters.Add(parametro);
                        }
                        valorEscalor = cmd.ExecuteScalar();
                    }
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    conexao.Close();
                }
            }
            if (valorEscalor == DBNull.Value)
            {
                return default;
            }
            return ConverterUtil.Para<T>(valorEscalor);
        }

        public bool TryExecutarComando(string sql, params SqlParameter[] parametros)
        {
            try
            {
                this.ExecutarComando(sql, parametros);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public int ExecutarComando(string sql,
                                   params SqlParameter[] parametros)
        {
            using (var conexao = new SqlConnection(this.ConnectionString))
            {
                conexao.Open();
                try
                {
                    using (var cmd = new SqlCommand(sql, conexao))
                    {
                        if(this.CommandTimeout.HasValue)
                        {
                            cmd.CommandTimeout = this.CommandTimeout.Value;
                        }
                        foreach (var parametro in parametros)
                        {
                            cmd.Parameters.Add(parametro);
                        }
                       return cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    conexao.Close();
                }
            }
        }

        public bool ExecutarComandos(IEnumerable<string> sqls,
                                      bool isIgnorarErro = false)
        {
            return this.ExecutarComandos(sqls, null, isIgnorarErro);
        }
        /// <summary>
        /// Executa o varios scripts 'sql' transacionado
        /// </summary>
        /// <param name="sqls"></param>
        /// <param name="acao"> A acao será invokada em cadas script </param>
        public bool ExecutarComandos(IEnumerable<string> sqls,
                                     Action<SqlCommand> acao,
                                     bool isIgnorarErro,
                                     params SqlParameter[] parametros)
        {
            using (var conexao = new SqlConnection(this.ConnectionString))
            {
                conexao.Open();

                using (var trans = conexao.BeginTransaction())
                {
                    try
                    {
                        foreach (var sql in sqls)
                        {
                            using (var cmd = new SqlCommand(sql, conexao, trans))
                            {
                                foreach (var parametro in parametros)
                                {
                                    cmd.Parameters.Add(parametro);
                                }
                                cmd.CommandTimeout = (int)TimeSpan.FromMinutes(100).TotalSeconds;
                                acao?.Invoke(cmd);
                                cmd.ExecuteNonQuery();
                            }
                        }
                        trans.Commit();
                        return true;
                    }
                    catch (Exception)
                    {

                        if (isIgnorarErro)
                        {
                            try
                            {
                                trans.Rollback();
                            }
                            catch { }
                            return false;
                        }

                        trans.Rollback();
                        throw;
                    }
                }
            }
        }
        private SqlDbType RetornarSqlDbType(Type tipo)
        {
            if (ReflexaoUtil.IsTipoNullable(tipo))
            {
                tipo = ReflexaoUtil.RetornarTipoSemNullable(tipo);
            }
            switch (tipo.Name)
            {
                case nameof(Char):
                case nameof(String):

                    return SqlDbType.NVarChar;

                case nameof(Int32):

                    return SqlDbType.Int;

                default:

                    throw new Exception($"o tipo {tipo.Name} não é suportado");
            }
        }

        public void Dispose()
        {
            this.ConnectionString = null;
            this.NomeConnectionString = null;
        }
    }
}