using System;

namespace Snebur.Dominio.Atributos
{
    [IgnorarAtributoTS]
    [AttributeUsage(AttributeTargets.Class)]
    public class TabelaSegurancaAttribute : TabelaAttribute
    {
        public const string SCHEMA_SEGURANCA = "seguranca";

        public TabelaSegurancaAttribute(string nomeTabela) : base(nomeTabela, SCHEMA_SEGURANCA)
        {
        }
    }
}