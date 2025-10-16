using System;

namespace Snebur.Dominio.Atributos
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Enum | AttributeTargets.Assembly)]
    public class IgnorarGlobalizacaoAttribute : Attribute
    {
    }
}

