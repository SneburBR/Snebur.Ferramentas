using System;

namespace Snebur.Dominio.Atributos
{
    [IgnorarAtributoTS]
    [AttributeUsage(AttributeTargets.Class)]
    public class TabelaUsuarioAttribute : TabelaAttribute
    {
        public const string SCHEMA_USUARIO = "usuario";

        public TabelaUsuarioAttribute(string nomeTabela) : base(nomeTabela, SCHEMA_USUARIO)
        {
        }
    }
}