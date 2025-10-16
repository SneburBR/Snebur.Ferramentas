using System;

namespace Snebur.Dominio.Atributos
{
    [IgnorarAtributoTS]
    [AttributeUsage(AttributeTargets.Class)]
    public class TabelaAtividadeAttribute : TabelaAttribute
    {
        public const string SCHEMA_ATIVIDADE = "atividade";

        public TabelaAtividadeAttribute(string nomeTabela) : base(nomeTabela, SCHEMA_ATIVIDADE)
        {
        }
    }
}