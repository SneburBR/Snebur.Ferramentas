﻿using Snebur.BancoDados;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using static Snebur.VisualStudio.ScriptGrupoArquivos;

namespace Snebur.VisualStudio
{
    public static class ScriptUtil
    {
        public static string RetornarConnectionStringBancoMastar(string nomeConnectionString)
        {
            var connectionString = ConfigurationManager.ConnectionStrings[nomeConnectionString].ConnectionString;
            var construtor = new SqlConnectionStringBuilder(connectionString);
            var nomeBancoDados = construtor.InitialCatalog;
            construtor.InitialCatalog = "master";
            return construtor.ToString();
        }

        public static string RetornarNomeBancoDados(string nomeConnectionString)
        {
            var connectionString = ConfigurationManager.ConnectionStrings[nomeConnectionString].ConnectionString;
            var construtor = new SqlConnectionStringBuilder(connectionString);
            return construtor.InitialCatalog;
        }

        public static bool ExisteBancoDados(string nomeConnectionString)
        {
            var connecionStringBancoMaster = ScriptUtil.RetornarConnectionStringBancoMastar(nomeConnectionString);
            var nomeBancoDados = ScriptUtil.RetornarNomeBancoDados(nomeConnectionString);
            var sql = $"select 1 FROM sys.databases where name = '{nomeBancoDados}'";
            var conexao = new Conexao(connecionStringBancoMaster);
            return conexao.RetornarValorScalar<bool>(sql);
        }

        public static string RetornarDiretorioBancoDados(string nomeConnectionString)
        {
            var connecionStringBancoMaster = ScriptUtil.RetornarConnectionStringBancoMastar(nomeConnectionString);
            var nomeBancoDados = ScriptUtil.RetornarNomeBancoDados(nomeConnectionString);

            var sql = $"SELECT TOP 1 DB.name As NomeBancoDados, F.physical_name  As CaminhoBanco FROM sys.databases DB JOIN sys.master_files F ON DB.database_id=F.database_id WHERE DB.name = '{nomeBancoDados}' OR DB.name = 'master' ";
            var conexao = new Conexao(connecionStringBancoMaster);
            var bancoCaminhos = conexao.Mapear<BancoCaminho>(sql);
            if (bancoCaminhos.Count == 0)
            {
                if(nomeConnectionString == "Zyoncore.FotoAlbum" || nomeBancoDados == "FotoAlbum"  )
                {
                    return @"G:\Program Files\Microsoft SQL Server\MSSQL10_50.MSSQLSERVER\MSSQL\DATA\FotoAlbum\";
                }
                throw new Exception($"A connectionString {nomeConnectionString} não possui acesso ao banco master");
            }

            if (bancoCaminhos.Count > 1)
            {
                throw new Exception($"A connectionString {nomeConnectionString}, foi encontra mais de um caminho para banco de dados {nomeBancoDados}");
            }
            return Path.GetDirectoryName(bancoCaminhos.Single().CaminhoBanco);
        }
    }
}
