using Snebur.Utilidade;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Snebur.Dominio.Atributos
{
    [IgnorarAtributoTS]
    [AttributeUsage(AttributeTargets.Class)]
    public class TabelaAttribute : TableAttribute
    {
        public string GrupoArquivoDados { get; }

        public string GrupoArquivoIndices { get; }

        public TabelaAttribute(string nomeTabela) : base(nomeTabela)
        {
            // this.Schema = "dbo";
        }

        public TabelaAttribute(string nomeTabela, string schema) : base(nomeTabela)
        {
            this.Schema = schema;
            this.GrupoArquivoDados = $"Dados{TextoUtil.RetornarPrimeiraLetraMaiuscula(schema)}";
            this.GrupoArquivoIndices = $"Indices{TextoUtil.RetornarPrimeiraLetraMaiuscula(schema)}";
        }

        public TabelaAttribute(string nomeTabela, string schema, string grupoArquivo) : base(nomeTabela)
        {
            this.Schema = schema;
            this.GrupoArquivoDados = $"Dados{grupoArquivo}";
            this.GrupoArquivoIndices = $"Dados{grupoArquivo}";
        }

        public TabelaAttribute(string nomeTabela, string schema, string grupoArquivoDados, string grupoArquivoIndices) : base(nomeTabela)
        {
            this.Schema = schema;

            this.GrupoArquivoDados = grupoArquivoDados;
            this.GrupoArquivoIndices = grupoArquivoIndices;
        }
    }
}
