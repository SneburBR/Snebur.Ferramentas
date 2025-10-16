using System;

namespace Snebur.Dominio.Atributos
{
    [IgnorarAtributoTS]
    [AttributeUsage(AttributeTargets.Property)]
    public class AutorizarAlteracaoPropriedadeAttribute : Attribute
    {

    }
}