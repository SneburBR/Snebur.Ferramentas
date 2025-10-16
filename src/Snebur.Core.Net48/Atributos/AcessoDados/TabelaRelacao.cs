using System;

namespace Snebur.Dominio.Atributos
{
    [IgnorarAtributoTS]
    [AttributeUsage(AttributeTargets.Class)]
    public class TabelaRelacao : TabelaAttribute
    {
        public const string SCHEMA_ATIVIDADE = "relacao";

        public TabelaRelacao(string nomeTabela) : base(nomeTabela, SCHEMA_ATIVIDADE)
        {
        }
    }
}