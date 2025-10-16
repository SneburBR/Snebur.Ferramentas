using System;

namespace Snebur.Dominio.Atributos
{
    [IgnorarAtributoTS]
    [AttributeUsage(AttributeTargets.Class)]
    public class TabelaEstatisticaAttribute : TabelaAttribute
    {
        public const string SCHEMA = "estatistica";

        public TabelaEstatisticaAttribute(string nomeTabela) : base(nomeTabela, SCHEMA)
        {
        }
    }
}